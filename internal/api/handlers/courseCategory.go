package handlers

import (
	"errors"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
)

// @Summary Create course category
// @Description Creates a new course category
// @Accept json
// @Produce json
// @Tags course-categories
// @Param body body presenter.InsertCategoryRequest true "Category payload"
// @Success 201 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 409 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /admin/categories [post]
func InsertCourseCategory(s service.CourseCategoryService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		var req presenter.InsertCategoryRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid request body"))
		}

		if req.Name == "" {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("category name is required"))
		}

		category, err := s.InsertCategory(&req)
		if errors.Is(err, service.ErrCategoryAlreadyExists) {
			return c.Status(fiber.StatusConflict).JSON(presenter.ErrorResponseFunc("category already exists"))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc(err.Error()))
		}

		return c.Status(fiber.StatusCreated).JSON(
			presenter.SuccessResponseWithData("category created", presenter.CourseCategorySuccessResponse(category)),
		)
	}
}

// @Summary List course categories
// @Description Returns all course categories
// @Accept json
// @Produce json
// @Tags course-categories
// @Success 200 {object} presenter.SuccessResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /categories [get]
func GetCourseCategories(s service.CourseCategoryService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		categories, err := s.FetchCategories()
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to fetch categories"))
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("categories fetched", presenter.CourseCategoriesSuccessResponse(categories)),
		)
	}
}

// @Summary Get course category by id
// @Description Returns a category by its id
// @Accept json
// @Produce json
// @Tags course-categories
// @Param id path int true "Category ID"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /categories/{id} [get]
func GetCourseCategoryById(s service.CourseCategoryService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		id, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid category id"))
		}

		category, err := s.FetchCategoryById(id)
		if errors.Is(err, service.ErrCategoryNotFound) {
			return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponseFunc("category not found"))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to fetch category"))
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("category fetched", presenter.CourseCategorySuccessResponse(category)),
		)
	}
}

// @Summary Update course category
// @Description Updates category by id
// @Accept json
// @Produce json
// @Tags course-categories
// @Param id path int true "Category ID"
// @Param body body presenter.UpdateCategoryRequest true "Category payload"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 409 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /admin/categories/{id} [put]
func UpdateCourseCategory(s service.CourseCategoryService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		id, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid category id"))
		}

		var req presenter.UpdateCategoryRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid request body"))
		}

		if req.Name == "" {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("category name is required"))
		}

		category, err := s.UpdateCategory(&req, id)
		if errors.Is(err, service.ErrCategoryAlreadyExists) {
			return c.Status(fiber.StatusConflict).JSON(presenter.ErrorResponseFunc("category already exists"))
		}
		if errors.Is(err, service.ErrCategoryNotFound) {
			return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponseFunc("category not found"))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to update category"))
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("category updated", presenter.CourseCategorySuccessResponse(category)),
		)
	}
}

// @Summary Delete course category
// @Description Deletes category by id
// @Accept json
// @Produce json
// @Tags course-categories
// @Param id path int true "Category ID"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /admin/categories/{id} [delete]
func DeleteCourseCategory(s service.CourseCategoryService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		id, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid category id"))
		}

		category, err := s.DeleteCourseCategory(id)
		if errors.Is(err, service.ErrCategoryNotFound) {
			return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponseFunc("category not found"))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to delete category"))
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("category deleted", presenter.CourseCategorySuccessResponse(category)),
		)
	}
}
