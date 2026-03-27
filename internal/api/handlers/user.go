package handlers

import (
	"errors"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

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
