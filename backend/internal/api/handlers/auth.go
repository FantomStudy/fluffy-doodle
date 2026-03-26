package handlers

import (
	"crypto/sha256"
	"encoding/hex"
	"errors"
	"fmt"
	"time"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"github.com/golang-jwt/jwt"
	"golang.org/x/crypto/bcrypt"
	"gorm.io/gorm"
)

// @Summary Registration
// @Description Регистрация пользователя
// @Accept json
// @Produce json
// @Tags auth
// @Param body body presenter.SignUpRequest true "Данные для регистрации"
// @Success 201 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /auth/sign-up [post]
func SignUp(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var user presenter.SignUpRequest

		if err := c.BodyParser(&user); err != nil {
			return c.Status(400).JSON(presenter.AuthErrorResponse(err))
		}

		// Валидация
		if user.Login == "" || len(user.Login) < 3 {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("Логин должен содержать минимум 3 символа")))
		}

		if user.FullName == "" || len(user.FullName) < 5 {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("ФИО должно содержать минимум 5 символов")))
		}

		if user.PhoneNumber == "" {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("Номер телефона обязателен для заполнения")))
		}

		if user.Password == "" || len(user.Password) < 8 {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("Пароль должен содержать минимум 8 символов")))
		}
		if len(user.Password) > 72 {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("Пароль не должен превышать 72 символа")))
		}

		// Проверка на существование пользователя
		checkUser, err := s.GetUser(user.Login)

		if err != nil && err != gorm.ErrRecordNotFound {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		if checkUser != nil && checkUser.ID != 0 {
			return c.Status(409).JSON(presenter.AuthErrorResponse(errors.New("Пользователь с таким логином уже существует")))
		}

		// хеширование пароля
		hashedPassword, err := hashPassword(user.Password)

		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(errors.New("Не удалось захешировать пароль")))
		}

		user.Password = hashedPassword

		_, err = s.SignUp(&user)

		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(errors.New("Не удалось создать пользователя")))
		}

		return c.Status(201).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Login
// @Description Авторизация пользователя
// @Accept json
// @Produce json
// @Tags auth
// @Param body body presenter.SignInRequest true "Данные для авторизации"
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /auth/sign-in [post]
func SignIn(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var user presenter.SignInRequest

		if err := c.BodyParser(&user); err != nil {
			return c.Status(400).JSON(presenter.AuthErrorResponse(err))
		}

		// Валидация
		if user.Login == "" {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("Логин не может быть пустым")))
		}
		if user.Password == "" {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("Пароль не может быть пустым")))
		}

		checkUser, err := s.GetUser(user.Login)

		if err == gorm.ErrRecordNotFound {
			return c.Status(404).JSON(presenter.AuthErrorResponse(errors.New("Пользователь не найден")))
		}

		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		// Debug: проверяем что получили из БД
		fmt.Printf("DEBUG: Login=%s, Password hash length=%d\n", checkUser.Login, len(checkUser.Password))

		if !CheckPasswordHash(user.Password, checkUser.Password) {
			return c.Status(401).JSON(presenter.AuthErrorResponse(errors.New("Неверный пароль")))
		}

		accessToken, err := CreateToken(checkUser, "access")
		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}
		refreshToken, err := CreateToken(checkUser, "refresh")
		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}
		sha := sha256.Sum256([]byte(refreshToken))
		hashedRefresh := hex.EncodeToString(sha[:])
		// Update refresh token in User table
		_, err = s.SetToken(hashedRefresh, int(checkUser.ID))
		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		// Set Cookie
		c.Cookie(&fiber.Cookie{
			Name:     "access_token",
			Value:    accessToken,
			HTTPOnly: true,
		})

		c.Cookie(&fiber.Cookie{
			Name:     "refresh_token",
			Value:    refreshToken,
			HTTPOnly: true,
		})

		return c.Status(200).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Update tokens
// @Description Обновление токенов
// @Accept json
// @Produce json
// @Tags auth
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /auth/refresh [post]
func RefreshToken(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		refreshTokenString := c.Cookies("refresh_token")

		if refreshTokenString == "" {
			return c.Status(401).JSON(presenter.AuthErrorResponse(errors.New("Refresh token не найден")))
		}

		// Parse and validate token
		cfg := config.Load()
		token, err := jwt.Parse(refreshTokenString, func(token *jwt.Token) (interface{}, error) {
			if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
				return nil, errors.New("Неверный метод подписи токена")
			}
			return []byte(cfg.RefreshSecret), nil
		})

		if err != nil || !token.Valid {
			return c.Status(401).JSON(presenter.AuthErrorResponse(errors.New("Токен невалидный или истёк")))
		}

		// Extract claims
		claims, ok := token.Claims.(jwt.MapClaims)
		if !ok {
			return c.Status(401).JSON(presenter.AuthErrorResponse(errors.New("Не удалось извлечь claims")))
		}
		// Get user
		user, err := s.GetUser(claims["login"].(string))

		if err == gorm.ErrRecordNotFound {
			return c.Status(404).JSON(presenter.AuthErrorResponse(errors.New("Пользователь не найден")))
		}

		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		accessToken, err := CreateToken(user, "access")

		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(errors.New("Не удалось сгенерировать токен доступа")))
		}

		refreshToken, err := CreateToken(user, "refresh")

		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(errors.New("Не удалось сгенерировать токен доступа")))
		}

		// Set Cookie
		c.Cookie(&fiber.Cookie{
			Name:     "access_token",
			Value:    accessToken,
			HTTPOnly: true,
		})

		c.Cookie(&fiber.Cookie{
			Name:     "refresh_token",
			Value:    refreshToken,
			HTTPOnly: true,
		})

		return c.Status(200).JSON(presenter.AuthSuccessResponse())

	}
}

// @Summary Exit
// @Description Выход пользователя из системы
// @Accept json
// @Produce json
// @Tags auth
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Router /auth/logout [post]
func Logout() fiber.Handler {
	return func(c *fiber.Ctx) error {
		c.Cookie(&fiber.Cookie{
			Name:   "access_token",
			Value:  "",
			Path:   "/",
			MaxAge: -1,
		})
		c.Cookie(&fiber.Cookie{
			Name:   "refresh_token",
			Value:  "",
			Path:   "/",
			MaxAge: -1,
		})

		return c.Status(200).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Check role
// @Description Проверяет пользователя на роль администратора
// @Accept json
// @Produce json
// @Tags auth
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Router /auth/is-admin [get]
func IsAdmin(s service.AuthService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		return c.Status(200).JSON(presenter.AuthSuccessResponse())
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

// Generate access and refresh token
func CreateToken(user *models.User, tokenType string) (string, error) {
	token := jwt.New(jwt.SigningMethodHS256)

	claims := token.Claims.(jwt.MapClaims)
	cfg := config.Load()

	claims["id"] = user.ID
	claims["login"] = user.Login
	switch tokenType {
	case "access":
		claims["exp"] = time.Now().Add(time.Minute * 15).Unix()
		t, err := token.SignedString([]byte(cfg.JwtSecret))
		return t, err
	case "refresh":
		claims["exp"] = time.Now().Add(time.Hour * 168).Unix()
		t, err := token.SignedString([]byte(cfg.RefreshSecret))
		return t, err
	default:
		return "", errors.New("Неверный тип токена")
	}

}
