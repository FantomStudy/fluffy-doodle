package routes

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/handlers"
	"github.com/FantomStudy/fluffy-doodle/internal/api/middleware"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

func CourseRoutes(app *fiber.App, courseService service.CourseService, db *gorm.DB) {
	app.Get("/courses", handlers.ListCourses(courseService))
	app.Get("/courses/:id", handlers.GetCourseByID(courseService))
	app.Get("/courses/:id/lessons", handlers.ListCourseLessons(courseService))

	adminRoutes := app.Group("/admin",
		middleware.Protected(),
		middleware.RequireRole(db, "admin"),
	)

	adminRoutes.Post("/courses", handlers.CreateCourse(courseService))
	adminRoutes.Put("/courses/:id", handlers.UpdateCourse(courseService))
	adminRoutes.Delete("/courses/:id", handlers.DeleteCourse(courseService))
	adminRoutes.Post("/courses/:id/lessons", handlers.CreateLesson(courseService))
}
