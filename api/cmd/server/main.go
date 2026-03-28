package main

import (
	"fmt"
	"log"

	_ "github.com/FantomStudy/fluffy-doodle/docs"
	"github.com/FantomStudy/fluffy-doodle/internal/api/routes"
	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/FantomStudy/fluffy-doodle/pkg/database"
	"github.com/FantomStudy/fluffy-doodle/pkg/storage"
	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/fiber/v2/middleware/cors"
	"github.com/gofiber/swagger"
)

// @title Fluffy Doodle REST API
// @version 1.0
// @description This is Rest Api for Fluffy Doodle
// @termsOfService http://swagger.io/terms/
// @host localhost:3000
// @BasePath /
func main() {
	// Load config
	cfg := config.Load()
	db, err := database.NewPostgresDB(&cfg)

	if err != nil {
		log.Fatal("Database connection error:", err.Error())
	}

	database.AutoMigrate(db)

	// init storage
	minioStorage, err := storage.NewMinioStorage(&cfg)
	if err != nil {
		log.Printf("Warning: Minio storage initialization failed: %v", err)
	}

	// init auth
	authRepo := repository.NewAuthRepo(db)
	authService := service.NewAuthService(authRepo)
	// init user
	userRepo := repository.NewUserRepo(db)
	userService := service.NewUserService(userRepo, minioStorage)
	// init course category
	courseCategoryRepo := repository.NewCourseCategoryRepository(db)
	courseCategoryService := service.NewCourseCategoryService(courseCategoryRepo)
	// init courses
	courseRepo := repository.NewCourseRepository(db)
	courseService := service.NewCourseService(courseRepo, minioStorage)
	// init forum
	forumRepo := repository.NewForumRepository(db)
	forumService := service.NewForumService(forumRepo)

	app := fiber.New(fiber.Config{
		BodyLimit: 100 * 1024 * 1024,
	})

	app.Use(cors.New(cors.Config{
		AllowOrigins:     cfg.Cors,
		AllowMethods:     "GET,POST,PUT,DELETE,OPTIONS",
		AllowHeaders:     "Origin, Content-Type, Accept, Authorization",
		AllowCredentials: true,
	}))

	app.Get("/swagger/*", swagger.HandlerDefault)

	app.Static("/uploads", "./uploads")

	// routes
	routes.AuthRoutes(app, authService)
	routes.UserRoutes(app, userService, db)
	routes.CourseCategoryRoutes(app, courseCategoryService, db)
	routes.CourseRoutes(app, courseService, db)
	routes.ForumRoutes(app, forumService)

	fmt.Printf("App started successfully on port %s\n", cfg.Port)
	app.Listen(":" + cfg.Port)

}
