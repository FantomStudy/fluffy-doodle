package handlers

import (
	"errors"
	"fmt"
	"strconv"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
)

// @Summary Create course
// @Description Creates a new course with image, category and level
// @Accept multipart/form-data
// @Produce json
// @Tags courses
// @Param title formData string true "Course title"
// @Param description formData string true "Course description"
// @Param categoryId formData int true "Course category ID"
// @Param level formData string true "Course level: beginner|intermediate|advanced"
// @Param hours formData int true "Course hours"
// @Param image formData file true "Course image"
// @Success 201 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /admin/courses [post]
func CreateCourse(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		title := c.FormValue("title")
		description := c.FormValue("description")
		level := c.FormValue("level")

		categoryID, err := strconv.Atoi(c.FormValue("categoryId"))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid categoryId"))
		}

		hours, err := strconv.Atoi(c.FormValue("hours"))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid hours"))
		}

		file, err := c.FormFile("image")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("course image is required"))
		}

		src, err := file.Open()
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to open image"))
		}
		defer src.Close()

		input := service.CreateCourseInput{
			Title:       title,
			Description: description,
			Level:       level,
			Hours:       hours,
			CategoryID:  uint(categoryID),
			Image:       src,
			ImageSize:   file.Size,
			ContentType: file.Header.Get("Content-Type"),
		}

		course, err := s.CreateCourse(c.Context(), input)
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusCreated).JSON(
			presenter.SuccessResponseWithData("course created", presenter.CourseToResponse(course)),
		)
	}
}

// @Summary Update course
// @Description Updates existing course
// @Accept multipart/form-data
// @Produce json
// @Tags courses
// @Param id path int true "Course ID"
// @Param title formData string true "Course title"
// @Param description formData string true "Course description"
// @Param categoryId formData int true "Course category ID"
// @Param level formData string true "Course level: beginner|intermediate|advanced"
// @Param hours formData int true "Course hours"
// @Param image formData file false "Course image"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /admin/courses/{id} [put]
func UpdateCourse(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		id, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid course id"))
		}

		title := c.FormValue("title")
		description := c.FormValue("description")
		level := c.FormValue("level")

		categoryID, err := strconv.Atoi(c.FormValue("categoryId"))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid categoryId"))
		}

		hours, err := strconv.Atoi(c.FormValue("hours"))
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid hours"))
		}

		input := service.UpdateCourseInput{
			Title:       title,
			Description: description,
			Level:       level,
			Hours:       hours,
			CategoryID:  uint(categoryID),
		}

		file, fileErr := c.FormFile("image")
		if fileErr == nil {
			src, openErr := file.Open()
			if openErr != nil {
				return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to open image"))
			}
			defer src.Close()

			input.Image = src
			input.ImageSize = file.Size
			input.ContentType = file.Header.Get("Content-Type")
		}

		course, err := s.UpdateCourse(c.Context(), id, input)
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("course updated", presenter.CourseToResponse(course)),
		)
	}
}

// @Summary Delete course
// @Description Deletes course by ID
// @Produce json
// @Tags courses
// @Param id path int true "Course ID"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /admin/courses/{id} [delete]
func DeleteCourse(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		id, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid course id"))
		}

		if err := s.DeleteCourse(id); err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusOK).JSON(presenter.SuccessMessageResponse("course deleted"))
	}
}

// @Summary List courses
// @Description Returns all courses
// @Produce json
// @Tags courses
// @Success 200 {object} presenter.SuccessResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /courses [get]
func ListCourses(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		courses, err := s.ListCourses()
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to list courses"))
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("courses fetched", presenter.CoursesToResponse(courses)),
		)
	}
}

// @Summary Get course by ID
// @Description Returns course detail by id
// @Produce json
// @Tags courses
// @Param id path int true "Course ID"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /courses/{id} [get]
func GetCourseByID(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		id, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid course id"))
		}

		course, err := s.GetCourseByID(id)
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("course fetched", presenter.CourseToResponse(course)),
		)
	}
}

// @Summary Create lesson in course
// @Description Creates lesson for selected course
// @Accept json
// @Produce json
// @Tags courses
// @Param id path int true "Course ID"
// @Param body body presenter.CreateLessonRequest true "Lesson payload"
// @Success 201 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /admin/courses/{id}/lessons [post]
func CreateLesson(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		courseID, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid course id"))
		}

		var req presenter.CreateLessonRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid request payload"))
		}

		lesson, err := s.CreateLesson(courseID, service.CreateLessonInput{
			Title:            req.Title,
			Description:      req.Description,
			Order:            req.Order,
			EstimatedMinutes: req.EstimatedMinutes,
			IsFreePreview:    req.IsFreePreview,
		})
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusCreated).JSON(
			presenter.SuccessResponseWithData("lesson created", presenter.LessonsToResponse([]models.Lesson{*lesson})[0]),
		)
	}
}

// @Summary List course lessons
// @Description Returns ordered lessons for course
// @Produce json
// @Tags courses
// @Param id path int true "Course ID"
// @Success 200 {object} presenter.SuccessResponse
// @Failure 400 {object} presenter.ErrorResponse
// @Failure 404 {object} presenter.ErrorResponse
// @Failure 500 {object} presenter.ErrorResponse
// @Router /courses/{id}/lessons [get]
func ListCourseLessons(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		courseID, err := c.ParamsInt("id")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid course id"))
		}

		lessons, err := s.ListLessons(courseID)
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("lessons fetched", presenter.LessonsToResponse(lessons)),
		)
	}
}

func mapCourseError(c *fiber.Ctx, err error) error {
	switch {
	case errors.Is(err, service.ErrCourseNotFound):
		return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponseFunc("course not found"))
	case errors.Is(err, service.ErrCategoryNotExists):
		return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponseFunc("category not found"))
	case errors.Is(err, service.ErrUnsupportedLevel):
		return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("level must be beginner, intermediate or advanced"))
	case errors.Is(err, service.ErrEmptyCourseTitle):
		return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("title is required"))
	case errors.Is(err, service.ErrInvalidHours):
		return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("hours must be greater than 0"))
	case errors.Is(err, service.ErrStorageNotAvailable):
		return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("file storage is not configured"))
	default:
		return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc(fmt.Sprintf("internal error: %v", err)))
	}
}
