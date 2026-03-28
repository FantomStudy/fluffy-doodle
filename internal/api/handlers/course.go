package handlers

import (
	"errors"
	"fmt"
	"strconv"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"github.com/golang-jwt/jwt"
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
			presenter.SuccessResponseWithData("course created", presenter.CourseToResponse(course, false)),
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
			presenter.SuccessResponseWithData("course updated", presenter.CourseToResponse(course, false)),
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
		courses, err := s.ListCourses(optionalUserID(c))
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc("failed to list courses"))
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("courses fetched", coursesToResponse(courses)),
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

		course, err := s.GetCourseByID(id, optionalUserID(c))
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("course fetched", presenter.CourseToResponse(&course.Course, course.IsSolved)),
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

		tasks := make([]service.CreateLessonTaskInput, 0, len(req.Tasks))
		for _, task := range req.Tasks {
			options := make([]models.TaskOption, 0, len(task.Options))
			for _, option := range task.Options {
				options = append(options, models.TaskOption{
					ID:   option.ID,
					Text: option.Text,
				})
			}

			tasks = append(tasks, service.CreateLessonTaskInput{
				Type:             task.Type,
				Title:            task.Title,
				Description:      task.Description,
				Question:         task.Question,
				Options:          options,
				CorrectOptionIDs: task.CorrectOptionIDs,
			})
		}

		lesson, err := s.CreateLesson(courseID, service.CreateLessonInput{
			Title:            req.Title,
			Description:      req.Description,
			Order:            req.Order,
			EstimatedMinutes: req.EstimatedMinutes,
			IsFreePreview:    req.IsFreePreview,
			Tasks:            tasks,
		})
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusCreated).JSON(
			presenter.SuccessResponseWithData("lesson created", presenter.LessonToResponse(lesson, false, nil)),
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

		lessons, err := s.ListLessons(courseID, optionalUserID(c))
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusOK).JSON(
			presenter.SuccessResponseWithData("lessons fetched", lessonsToResponse(lessons)),
		)
	}
}

func coursesToResponse(courses []service.CourseWithStatus) []presenter.CourseResponse {
	result := make([]presenter.CourseResponse, 0, len(courses))
	for _, course := range courses {
		result = append(result, presenter.CourseToResponse(&course.Course, course.IsSolved))
	}

	return result
}

func lessonsToResponse(lessons []service.LessonWithStatus) []presenter.LessonResponse {
	result := make([]presenter.LessonResponse, 0, len(lessons))
	for _, lesson := range lessons {
		result = append(result, presenter.LessonToResponse(&lesson.Lesson, lesson.IsSolved, lesson.SolvedTaskIDs))
	}

	return result
}

func SubmitLessonTask(s service.CourseService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		lessonID, err := c.ParamsInt("lessonId")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid lesson id"))
		}

		taskID, err := c.ParamsInt("taskId")
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid task id"))
		}

		var req presenter.SubmitLessonTaskRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid request payload"))
		}

		result, err := s.SubmitLessonTask(c.Locals("userId").(uint), lessonID, taskID, service.SubmitLessonTaskInput{
			SelectedOptionIDs: req.SelectedOptionIDs,
			Solved:            req.Solved,
		})
		if err != nil {
			return mapCourseError(c, err)
		}

		return c.Status(fiber.StatusOK).JSON(presenter.SuccessResponseWithData("task submitted", presenter.SubmitLessonTaskResponse{
			TaskID:       result.TaskID,
			IsSolved:     result.IsSolved,
			WasSolved:    result.WasSolved,
			AwardedStars: result.AwardedStars,
			AwardedExp:   result.AwardedExp,
			CurrentStars: result.CurrentStars,
			CurrentExp:   result.CurrentExp,
			CurrentLevel: result.CurrentLevel,
		}))
	}
}

func optionalUserID(c *fiber.Ctx) *uint {
	tokenString := c.Cookies("access_token")
	if tokenString == "" {
		return nil
	}

	cfg := config.Load()
	token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
		if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
			return nil, errors.New("invalid token signing method")
		}
		return []byte(cfg.JwtSecret), nil
	})
	if err != nil || !token.Valid {
		return nil
	}

	claims, ok := token.Claims.(jwt.MapClaims)
	if !ok {
		return nil
	}

	rawID, ok := claims["id"].(float64)
	if !ok {
		return nil
	}

	userID := uint(rawID)
	return &userID
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
	case errors.Is(err, service.ErrLessonNotFound):
		return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponseFunc("lesson not found"))
	case errors.Is(err, service.ErrTaskNotFound):
		return c.Status(fiber.StatusNotFound).JSON(presenter.ErrorResponseFunc("task not found"))
	case errors.Is(err, service.ErrUnsupportedTaskType):
		return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("task type must be quiz, flowchart or algorithm"))
	case errors.Is(err, service.ErrInvalidTaskConfig):
		return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("invalid task configuration"))
	case errors.Is(err, service.ErrQuizAnswerRequired):
		return c.Status(fiber.StatusBadRequest).JSON(presenter.ErrorResponseFunc("quiz answers are required"))
	default:
		return c.Status(fiber.StatusInternalServerError).JSON(presenter.ErrorResponseFunc(fmt.Sprintf("internal error: %v", err)))
	}
}
