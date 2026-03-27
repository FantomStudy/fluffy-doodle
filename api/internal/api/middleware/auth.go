package middleware

import (
	"errors"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/gofiber/fiber/v2"
	"github.com/golang-jwt/jwt"
)

func Protected() fiber.Handler {
	return func(c *fiber.Ctx) error {
		tokenString := c.Cookies("access_token")

		if tokenString == "" {
			return c.Status(401).JSON(presenter.AuthErrorResponse(errors.New("Вы не авторизованы")))
		}

		// Parse and validate token
		cfg := config.Load()
		token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
			if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
				return nil, errors.New("Неверный метод подписи токена")
			}
			return []byte(cfg.JwtSecret), nil
		})

		if err != nil || !token.Valid {
			return c.Status(401).JSON(presenter.AuthErrorResponse(errors.New("Токен невалидный или истёк")))
		}

		// Extract claims
		claims, ok := token.Claims.(jwt.MapClaims)
		if !ok {
			return c.Status(401).JSON(presenter.AuthErrorResponse(errors.New("Не удалось извлечь claims")))
		}

		// Save user info to context
		c.Locals("userId", uint(claims["id"].(float64)))
		c.Locals("userLogin", claims["login"].(string))

		return c.Next()
	}
}
