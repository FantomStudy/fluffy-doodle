package routes

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/handlers"
	"github.com/FantomStudy/fluffy-doodle/internal/api/middleware"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

func UserRoutes(app *fiber.App, service service.UserService, db *gorm.DB) {
	app.Get("/user/parent/child-progress", middleware.Protected(), middleware.RequireRole(db, "parent"), handlers.ParentChildProgress(service))
	app.Get("/user/profile", middleware.Protected(), handlers.GetProfile(service))
	app.Post("/user/avatar", middleware.Protected(), handlers.UploadAvatar(service))
	app.Post("/game/levels/:levelId/complete", middleware.Protected(), handlers.CompleteGameLevel(service))
}
