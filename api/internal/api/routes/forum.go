package routes

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/handlers"
	"github.com/FantomStudy/fluffy-doodle/internal/api/middleware"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
)

func ForumRoutes(app *fiber.App, forumService service.ForumService) {
	forumGroup := app.Group("/forum")

	// Public routes or protected, depending on requirements. 
	// The plan says we just need authentication for creating, maybe reading is public.
	// But let's protect everything for a school community to be safe.
	forumGroup.Use(middleware.Protected())

	forumGroup.Get("/categories", handlers.GetForumCategories(forumService))
	forumGroup.Post("/categories", handlers.CreateForumCategory(forumService))
	forumGroup.Get("/topics", handlers.GetForumTopics(forumService))
	forumGroup.Get("/topics/:id", handlers.GetForumTopicDetail(forumService))
	forumGroup.Post("/topics", handlers.CreateForumTopic(forumService))
	forumGroup.Post("/topics/:id/comments", handlers.CreateForumComment(forumService))
	forumGroup.Put("/comments/:id/solution", handlers.MarkForumCommentAsSolution(forumService))
}
