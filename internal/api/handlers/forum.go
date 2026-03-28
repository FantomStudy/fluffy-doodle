package handlers

import (
	"strconv"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
)

// @Summary Get forum categories
// @Description Returns a list of forum categories
// @Produce json
// @Tags forum
// @Success 200 {array} presenter.ForumCategoryResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /forum/categories [get]
func GetForumCategories(s service.ForumService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		categories, err := s.GetCategories()
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}
		return c.Status(fiber.StatusOK).JSON(categories)
	}
}

// @Summary Create forum category
// @Description Creates a new forum category
// @Accept json
// @Produce json
// @Tags forum
// @Param body body presenter.CreateForumCategoryRequest true "Category data"
// @Success 201 {object} presenter.ForumCategoryResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 401 {object} presenter.ErrorResponse
// @Router /forum/categories [post]
func CreateForumCategory(s service.ForumService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var req presenter.CreateForumCategoryRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}

		category, err := s.CreateCategory(req)
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}

		return c.Status(fiber.StatusCreated).JSON(category)
	}
}

// @Summary Get forum topics
// @Description Returns a list of forum topics. Can be filtered by categoryId.
// @Produce json
// @Tags forum
// @Param categoryId query int false "Category ID"
// @Success 200 {array} presenter.ForumTopicResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /forum/topics [get]
func GetForumTopics(s service.ForumService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		catIDStr := c.Query("categoryId")
		var catID uint = 0
		if catIDStr != "" {
			if id, err := strconv.Atoi(catIDStr); err == nil {
				catID = uint(id)
			}
		}

		topics, err := s.GetTopics(catID)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}
		return c.Status(fiber.StatusOK).JSON(topics)
	}
}

// @Summary Get topic details
// @Description Returns topic details along with comments
// @Produce json
// @Tags forum
// @Param id path int true "Topic ID"
// @Success 200 {object} presenter.ForumTopicDetailResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Router /forum/topics/{id} [get]
func GetForumTopicDetail(s service.ForumService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		id, err := strconv.Atoi(c.Params("id"))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   "invalid id",
			})
		}

		detail, err := s.GetTopicDetail(uint(id))
		if err != nil {
			return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}
		return c.Status(fiber.StatusOK).JSON(detail)
	}
}

// @Summary Create topic
// @Description Creates a new forum topic
// @Accept json
// @Produce json
// @Tags forum
// @Param body body presenter.CreateForumTopicRequest true "Topic data"
// @Success 201 {object} presenter.ForumTopicResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 401 {object} presenter.ErrorResponse
// @Router /forum/topics [post]
func CreateForumTopic(s service.ForumService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)

		var req presenter.CreateForumTopicRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}

		topic, err := s.CreateTopic(userID, req)
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}

		return c.Status(fiber.StatusCreated).JSON(topic)
	}
}

// @Summary Create comment
// @Description Creates a new comment in a topic
// @Accept json
// @Produce json
// @Tags forum
// @Param id path int true "Topic ID"
// @Param body body presenter.CreateForumCommentRequest true "Comment data"
// @Success 201 {object} presenter.ForumCommentResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 401 {object} presenter.ErrorResponse
// @Router /forum/topics/{id}/comments [post]
func CreateForumComment(s service.ForumService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)
		topicID, err := strconv.Atoi(c.Params("id"))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   "invalid topic id",
			})
		}

		var req presenter.CreateForumCommentRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}

		comment, err := s.CreateComment(userID, uint(topicID), req)
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}

		return c.Status(fiber.StatusCreated).JSON(comment)
	}
}

// @Summary Mark comment as solution
// @Description Marks a specific comment as the solution to the topic (only topic author can do this)
// @Produce json
// @Tags forum
// @Param id path int true "Comment ID"
// @Param topicId query int true "Topic ID"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 401 {object} presenter.ErrorResponse
// @Router /forum/comments/{id}/solution [put]
func MarkForumCommentAsSolution(s service.ForumService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)
		commentID, err := strconv.Atoi(c.Params("id"))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   "invalid comment id",
			})
		}

		topicIDStr := c.Query("topicId")
		topicID, err := strconv.Atoi(topicIDStr)
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   "invalid topicId query parameter",
			})
		}

		err = s.MarkAsSolution(userID, uint(topicID), uint(commentID))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponse{
				Success: false,
				Error:   err.Error(),
			})
		}

		return c.Status(fiber.StatusOK).JSON(presenter.SuccessResponse{
			Success: true,
		})
	}
}
