package handlers

import (
	"crypto/rand"
	"encoding/base32"
	"errors"
	"fmt"
	"path/filepath"
	"strings"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

// @Summary Get user profile
// @Description Получение профиля текущего пользователя
// @Produce json
// @Tags user
// @Success 200 {object} presenter.UserProfileResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/profile [get]
func GetProfile(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)

		user, err := s.GetUserByID(userID)
		if err != nil {
			if err == gorm.ErrRecordNotFound {
				return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("user not found")))
			}
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(user)
	}
}

// @Summary Upload avatar
// @Description Upload user avatar to S3
// @Accept multipart/form-data
// @Produce json
// @Tags user
// @Param avatar formData file true "Avatar file"
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/avatar [post]
func UploadAvatar(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)

		file, err := c.FormFile("avatar")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("file not found")))
		}

		src, err := file.Open()
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to open file")))
		}
		defer src.Close()

		filename := fmt.Sprintf("avatar_%d%s", userID, filepath.Ext(file.Filename))
		url, err := s.UploadAvatar(c.Context(), filename, src, file.Size, file.Header.Get("Content-Type"))
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		user, err := s.GetUserByID(userID)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		user.Avatar = url
		if _, err = s.UpdateUser(user); err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Invite student
// @Description Teacher creates student account and receives student code
// @Accept json
// @Produce json
// @Tags user
// @Param body body presenter.SignUpRequest true "Student registration data"
// @Success 201 {object} presenter.StudentInviteResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 409 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/student-invitation [post]
func InviteUser(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var user presenter.SignUpRequest
		if err := c.BodyParser(&user); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
		}

		if err := validateSignUp(user); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
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

		role, err := s.FindRole("student")
		if err == gorm.ErrRecordNotFound {
			return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("student role not found")))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		code, err := generateStudentCode()
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to generate student code")))
		}

		user.Password = hashedPassword
		user.InvitationCode = code

		if _, err = s.InviteUser(&user, role.ID); err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to create student")))
		}

		return c.Status(fiber.StatusCreated).JSON(presenter.StudentInviteResponse{
			StudentCode: code,
		})
	}
}

// @Summary Parent registration by student code
// @Description Parent creates account and links to student using student code
// @Accept json
// @Produce json
// @Tags user
// @Param body body presenter.ParentSignUpRequest true "Parent registration data"
// @Success 201 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 409 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/parent-sign-up [post]
func ParentSignUp(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var req presenter.ParentSignUpRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
		}

		signUp := presenter.SignUpRequest{
			Login:       req.Login,
			Password:    req.Password,
			FullName:    req.FullName,
			PhoneNumber: req.PhoneNumber,
		}
		if err := validateSignUp(signUp); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
		}
		if strings.TrimSpace(req.StudentCode) == "" {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("studentCode is required")))
		}

		existingParent, err := s.GetUser(req.Login)
		if err != nil && err != gorm.ErrRecordNotFound {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}
		if existingParent != nil && existingParent.ID != 0 {
			return c.Status(fiber.StatusConflict).JSON(presenter.AuthErrorResponse(errors.New("user with this login already exists")))
		}

		student, err := s.GetUserByInvitationCode(req.StudentCode)
		if err == gorm.ErrRecordNotFound {
			return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("student with this code not found")))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}
		if student.Role.Name != "student" {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("code does not belong to a student")))
		}
		if student.ParentID != nil {
			return c.Status(fiber.StatusConflict).JSON(presenter.AuthErrorResponse(errors.New("student is already linked to parent")))
		}

		parentRole, err := s.FindRole("parent")
		if err == gorm.ErrRecordNotFound {
			return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("parent role not found")))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		hashedPassword, err := hashPassword(req.Password)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to hash password")))
		}

		signUp.Password = hashedPassword
		parentUser, err := s.InviteUser(&signUp, parentRole.ID)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to create parent user")))
		}

		if err = s.AssignParentToStudent(student.ID, parentUser.ID); err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to link parent with student")))
		}

		return c.Status(fiber.StatusCreated).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Parent child progress
// @Description Returns current child progress for authenticated parent
// @Produce json
// @Tags user
// @Success 200 {object} presenter.ParentChildProgressResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/parent/child-progress [get]
func ParentChildProgress(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		parentID := c.Locals("userId").(uint)

		child, err := s.GetChildByParentID(parentID)
		if err == gorm.ErrRecordNotFound {
			return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("child not linked to this parent")))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.ParentChildProgressResponse{
			StudentID:      child.ID,
			StudentName:    child.FullName,
			StudentLogin:   child.Login,
			Stars:          child.Stars,
			Achievements:   len(child.Achievements),
			InvitationCode: child.InvitationCode,
		})
	}
}

func validateSignUp(user presenter.SignUpRequest) error {
	if user.Login == "" || len(user.Login) < 3 {
		return errors.New("login must contain at least 3 characters")
	}
	if user.FullName == "" || len(user.FullName) < 5 {
		return errors.New("fullName must contain at least 5 characters")
	}
	if user.PhoneNumber == "" {
		return errors.New("phoneNumber is required")
	}
	if user.Password == "" || len(user.Password) < 8 {
		return errors.New("password must contain at least 8 characters")
	}
	if len(user.Password) > 72 {
		return errors.New("password length must be less than or equal to 72")
	}
	return nil
}

func generateStudentCode() (string, error) {
	b := make([]byte, 5)
	if _, err := rand.Read(b); err != nil {
		return "", err
	}
	code := strings.TrimRight(base32.StdEncoding.WithPadding(base32.NoPadding).EncodeToString(b), "=")
	return "STU-" + code, nil
}
