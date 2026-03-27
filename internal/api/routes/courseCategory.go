package routes

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/handlers"
	"github.com/FantomStudy/fluffy-doodle/internal/api/middleware"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

func CourseCategoryRoutes(app *fiber.App, categoryService service.CourseCategoryService, db *gorm.DB) {
	app.Get("/categories", handlers.GetCourseCategories(categoryService))
	app.Get("/categories/:id", handlers.GetCourseCategoryById(categoryService))

	adminRoutes := app.Group("/admin",
		middleware.Protected(),
		middleware.RequireRole(db, "admin"),
	)

	adminRoutes.Post("/categories", handlers.InsertCourseCategory(categoryService))
	adminRoutes.Put("/categories/:id", handlers.UpdateCourseCategory(categoryService))
	adminRoutes.Delete("/categories/:id", handlers.DeleteCourseCategory(categoryService))
}
