package handlers

import (
	"errors"
	"fmt"
	"path/filepath"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	_ "github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

// @Summary Get user profile
// @Description Получение профиля текущего пользователя
// @Produce json
// @Tags user
// @Success 200 {object} models.User
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/profile [get]
func GetProfile(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userId := c.Locals("userId").(uint)

		user, err := s.GetUserByID(userId)
		if err != nil {
			if err == gorm.ErrRecordNotFound {
				return c.Status(404).JSON(presenter.AuthErrorResponse(errors.New("Пользователь не найден")))
			}
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(200).JSON(user)
	}
}

// @Summary Upload avatar
// @Description Загрузка аватарки пользователя в S3
// @Accept multipart/form-data
// @Produce json
// @Tags user
// @Param avatar formData file true "Файл аватарки"
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/avatar [post]
func UploadAvatar(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userId := c.Locals("userId").(uint)

		file, err := c.FormFile("avatar")
		if err != nil {
			return c.Status(400).JSON(presenter.AuthErrorResponse(errors.New("Файл не найден")))
		}

		// Открываем файл для чтения
		src, err := file.Open()
		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(errors.New("Не удалось открыть файл")))
		}
		defer src.Close()

		// Создаем уникальное имя файла
		filename := fmt.Sprintf("avatar_%d%s", userId, filepath.Ext(file.Filename))
		
		// Загружаем в S3 через сервис
		url, err := s.UploadAvatar(c.Context(), filename, src, file.Size, file.Header.Get("Content-Type"))
		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		user, err := s.GetUserByID(userId)
		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		user.Avatar = url
		_, err = s.UpdateUser(user)
		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(200).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Invite student
// @Description Регистрация студента
// @Accept json
// @Produce json
// @Tags user
// @Param body body presenter.SignUpRequest true "Данные для регистрации ученика"
// @Success 201 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/student-invitation [post]
func InviteUser(s service.UserService) fiber.Handler {
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

		role, err := s.FindRole("student")

		if err == gorm.ErrRecordNotFound {
			return c.Status(404).JSON(presenter.AuthErrorResponse(errors.New("Роль не найдена")))
		}

		user.Password = hashedPassword

		_, err = s.InviteUser(&user, role.ID)

		if err != nil {
			return c.Status(500).JSON(presenter.AuthErrorResponse(errors.New("Не удалось создать пользователя")))
		}

		return c.Status(201).JSON(presenter.AuthSuccessResponse())
	}
}
