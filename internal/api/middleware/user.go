package middleware

import (
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

func RequireRole(db *gorm.DB, allowedRoles ...string) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID, ok := c.Locals("userId").(uint)
		if !ok {
			return c.Status(401).JSON(fiber.Map{
				"error": "Пользователь не авторизован",
			})
		}

		var user models.User
		if err := db.Preload("Role").First(&user, userID).Error; err != nil {
			return c.Status(404).JSON(fiber.Map{
				"error": "Пользователь не найден",
			})
		}

		// Проверяем, есть ли у пользователя нужная роль
		for _, allowedRole := range allowedRoles {
			if user.Role.Name == allowedRole {
				return c.Next()
			}
		}

		return c.Status(403).JSON(fiber.Map{
			"error": "Недостаточно прав для выполнения операции",
		})
	}
}
