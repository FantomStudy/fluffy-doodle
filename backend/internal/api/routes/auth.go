package routes

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/handlers"
	"github.com/FantomStudy/fluffy-doodle/internal/api/middleware"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
)

func AuthRoutes(app *fiber.App, service service.AuthService) {
	app.Post("/auth/sign-up", handlers.SignUp(service))
	app.Post("/auth/sign-in", handlers.SignIn(service))
	app.Post("/auth/refresh", handlers.RefreshToken(service))
	app.Post("/auth/logout", middleware.Protected(), handlers.Logout())
	app.Get("/auth/is-admin", middleware.Protected(), handlers.IsAdmin(service))
}
