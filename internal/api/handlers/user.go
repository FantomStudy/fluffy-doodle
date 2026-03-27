package handlers

import (
	"errors"
	"fmt"
	"path/filepath"

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
			fmt.Printf("UploadAvatar Error: %v\n", err)
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("file not found")))
		}

		src, err := file.Open()
		if err != nil {
			fmt.Printf("UploadAvatar File Open Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to open file")))
		}
		defer src.Close()

		filename := fmt.Sprintf("avatar_%d%s", userID, filepath.Ext(file.Filename))
		url, err := s.UploadAvatar(c.Context(), filename, src, file.Size, file.Header.Get("Content-Type"))
		if err != nil {
			fmt.Printf("UploadAvatar S3 Upload Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		user, err := s.GetUserByID(userID)
		if err != nil {
			fmt.Printf("UploadAvatar GetUser Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		user.Avatar = url
		if _, err = s.UpdateUser(user); err != nil {
			fmt.Printf("UploadAvatar UpdateUser Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
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
