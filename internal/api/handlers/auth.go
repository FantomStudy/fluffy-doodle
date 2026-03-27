package handlers

import (
	"crypto/rand"
	"crypto/sha256"
	"encoding/base32"
	"encoding/hex"
	"errors"
	"fmt"
	"strings"
	"time"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"github.com/golang-jwt/jwt"
	"golang.org/x/crypto/bcrypt"
	"gorm.io/gorm"
)

// @Summary Registration
// @Description Student signs up without code. Parent signs up with studentCode.
// @Accept json
// @Produce json
// @Tags auth
// @Param body body presenter.SignUpRequest true "Sign up payload"
// @Success 201 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 409 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /auth/sign-up [post]
func SignUp(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var user presenter.SignUpRequest
		if err := c.BodyParser(&user); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
		}

		if user.Login == "" || len(user.Login) < 3 {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("login must contain at least 3 symbols")))
		}
		if user.FullName == "" || len(user.FullName) < 5 {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("fullName must contain at least 5 symbols")))
		}
		if user.PhoneNumber == "" {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("phoneNumber is required")))
		}
		if user.Password == "" || len(user.Password) < 8 {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("password must contain at least 8 symbols")))
		}
		if len(user.Password) > 72 {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("password must not exceed 72 symbols")))
		}

		checkUser, err := s.GetUser(user.Login)
		if err != nil && err != gorm.ErrRecordNotFound {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}
		if checkUser != nil && checkUser.ID != 0 {
			return c.Status(fiber.StatusConflict).JSON(presenter.AuthErrorResponse(errors.New("user with this login already exists")))
		}

		hashedPassword, err := hashPassword(user.Password)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to hash password")))
		}
		user.Password = hashedPassword

		isParentSignUp := strings.TrimSpace(user.InvitationCode) != ""
		if !isParentSignUp {
			code, codeErr := generateStudentCode()
			if codeErr != nil {
				return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to generate student code")))
			}
			user.InvitationCode = code
		}

		createdUser, err := s.SignUp(&user)
		if err != nil {
			switch {
			case errors.Is(err, gorm.ErrRecordNotFound):
				return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("student code not found")))
			case errors.Is(err, repository.ErrParentAlreadyLinked):
				return c.Status(fiber.StatusConflict).JSON(presenter.AuthErrorResponse(errors.New("student is already linked to parent")))
			default:
				return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to create user")))
			}
		}

		if !isParentSignUp {
			return c.Status(fiber.StatusCreated).JSON(fiber.Map{
				"success":     true,
				"studentCode": createdUser.InvitationCode,
			})
		}

		return c.Status(fiber.StatusCreated).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Login
// @Description User login
// @Accept json
// @Produce json
// @Tags auth
// @Param body body presenter.SignInRequest true "Credentials"
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /auth/sign-in [post]
func SignIn(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var user presenter.SignInRequest
		if err := c.BodyParser(&user); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
		}
		if user.Login == "" {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("login is required")))
		}
		if user.Password == "" {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("password is required")))
		}

		checkUser, err := s.GetUser(user.Login)
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("user not found")))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		fmt.Printf("DEBUG: Login=%s, Password hash length=%d\n", checkUser.Login, len(checkUser.Password))
		if !CheckPasswordHash(user.Password, checkUser.Password) {
			return c.Status(fiber.StatusUnauthorized).JSON(presenter.AuthErrorResponse(errors.New("invalid password")))
		}

		accessToken, err := CreateToken(checkUser, "access")
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}
		refreshToken, err := CreateToken(checkUser, "refresh")
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		sha := sha256.Sum256([]byte(refreshToken))
		hashedRefresh := hex.EncodeToString(sha[:])
		if _, err = s.SetToken(hashedRefresh, int(checkUser.ID)); err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		c.Cookie(&fiber.Cookie{Name: "access_token", Value: accessToken, HTTPOnly: true})
		c.Cookie(&fiber.Cookie{Name: "refresh_token", Value: refreshToken, HTTPOnly: true})
		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Update tokens
// @Description Refresh access and refresh tokens
// @Accept json
// @Produce json
// @Tags auth
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /auth/refresh [post]
func RefreshToken(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		refreshTokenString := c.Cookies("refresh_token")
		if refreshTokenString == "" {
			return c.Status(fiber.StatusUnauthorized).JSON(presenter.AuthErrorResponse(errors.New("refresh token not found")))
		}

		cfg := config.Load()
		token, err := jwt.Parse(refreshTokenString, func(token *jwt.Token) (interface{}, error) {
			if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
				return nil, errors.New("invalid signing method")
			}
			return []byte(cfg.RefreshSecret), nil
		})
		if err != nil || !token.Valid {
			return c.Status(fiber.StatusUnauthorized).JSON(presenter.AuthErrorResponse(errors.New("token is invalid or expired")))
		}

		claims, ok := token.Claims.(jwt.MapClaims)
		if !ok {
			return c.Status(fiber.StatusUnauthorized).JSON(presenter.AuthErrorResponse(errors.New("failed to parse token claims")))
		}
		user, err := s.GetUser(claims["login"].(string))
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("user not found")))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		accessToken, err := CreateToken(user, "access")
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to generate access token")))
		}
		refreshToken, err := CreateToken(user, "refresh")
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to generate refresh token")))
		}

		c.Cookie(&fiber.Cookie{Name: "access_token", Value: accessToken, HTTPOnly: true})
		c.Cookie(&fiber.Cookie{Name: "refresh_token", Value: refreshToken, HTTPOnly: true})
		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Exit
// @Description Logout
// @Accept json
// @Produce json
// @Tags auth
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Router /auth/logout [post]
func Logout() fiber.Handler {
	return func(c *fiber.Ctx) error {
		c.Cookie(&fiber.Cookie{Name: "access_token", Value: "", Path: "/", MaxAge: -1})
		c.Cookie(&fiber.Cookie{Name: "refresh_token", Value: "", Path: "/", MaxAge: -1})
		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Check role
// @Description Checks role (protected endpoint)
// @Accept json
// @Produce json
// @Tags auth
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Router /auth/is-admin [get]
func IsAdmin(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}

func hashPassword(password string) (string, error) {
	bytes, err := bcrypt.GenerateFromPassword([]byte(password), 10)
	return string(bytes), err
}

func CheckPasswordHash(password, hash string) bool {
	err := bcrypt.CompareHashAndPassword([]byte(hash), []byte(password))
	return err == nil
}

func CreateToken(user *models.User, tokenType string) (string, error) {
	token := jwt.New(jwt.SigningMethodHS256)
	claims := token.Claims.(jwt.MapClaims)
	cfg := config.Load()

	claims["id"] = user.ID
	claims["login"] = user.Login
	switch tokenType {
	case "access":
		claims["exp"] = time.Now().Add(time.Minute * 15).Unix()
		return token.SignedString([]byte(cfg.JwtSecret))
	case "refresh":
		claims["exp"] = time.Now().Add(time.Hour * 168).Unix()
		return token.SignedString([]byte(cfg.RefreshSecret))
	default:
		return "", errors.New("invalid token type")
	}
}

func generateStudentCode() (string, error) {
	b := make([]byte, 5)
	if _, err := rand.Read(b); err != nil {
		return "", err
	}
	code := strings.TrimRight(base32.StdEncoding.WithPadding(base32.NoPadding).EncodeToString(b), "=")
	return "STU-" + code, nil
}
