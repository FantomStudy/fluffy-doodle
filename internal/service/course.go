package service

import (
	"context"
	"errors"
	"fmt"
	"io"
	"slices"
	"strings"
	"time"

	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
	"github.com/FantomStudy/fluffy-doodle/pkg/storage"
	"gorm.io/gorm"
)

var (
	ErrCourseNotFound      = errors.New("course not found")
	ErrCategoryNotExists   = errors.New("category does not exist")
	ErrUnsupportedLevel    = errors.New("unsupported course level")
	ErrEmptyCourseTitle    = errors.New("course title is required")
	ErrInvalidHours        = errors.New("hours must be greater than 0")
	ErrStorageNotAvailable = errors.New("storage is not available")
	ErrLessonNotFound      = errors.New("lesson not found")
	ErrTaskNotFound        = errors.New("task not found")
	ErrUnsupportedTaskType = errors.New("unsupported task type")
	ErrInvalidTaskConfig   = errors.New("invalid task config")
	ErrQuizAnswerRequired  = errors.New("quiz answer is required")
)

type CourseService interface {
	CreateCourse(ctx context.Context, req CreateCourseInput) (*models.Course, error)
	UpdateCourse(ctx context.Context, id int, req UpdateCourseInput) (*models.Course, error)
	DeleteCourse(id int) error
	GetCourseByID(id int, userID *uint) (*CourseWithStatus, error)
	ListCourses(userID *uint) ([]CourseWithStatus, error)
	CreateLesson(courseID int, req CreateLessonInput) (*models.Lesson, error)
	ListLessons(courseID int, userID *uint) ([]LessonWithStatus, error)
	SubmitLessonTask(userID uint, lessonID int, taskID int, req SubmitLessonTaskInput) (*SubmitLessonTaskResult, error)
}

type CreateCourseInput struct {
	Title       string
	Description string
	Level       string
	Hours       int
	CategoryID  uint
	Image       io.Reader
	ImageSize   int64
	ContentType string
}

type UpdateCourseInput struct {
	Title       string
	Description string
	Level       string
	Hours       int
	CategoryID  uint
	Image       io.Reader
	ImageSize   int64
	ContentType string
}

type CreateLessonInput struct {
	Title            string
	Description      string
	Order            int
	EstimatedMinutes int
	IsFreePreview    bool
	Tasks            []CreateLessonTaskInput
}

type CreateLessonTaskInput struct {
	Type             string
	Title            string
	Description      string
	Question         string
	Options          []models.TaskOption
	CorrectOptionIDs []string
}

type SubmitLessonTaskInput struct {
	SelectedOptionIDs []string
	Solved            bool
}

type SubmitLessonTaskResult struct {
	TaskID       uint
	IsSolved     bool
	WasSolved    bool
	AwardedStars int
	AwardedExp   int
	CurrentStars int
	CurrentExp   int
	CurrentLevel int
}

type CourseWithStatus struct {
	Course   models.Course
	IsSolved bool
}

type LessonWithStatus struct {
	Lesson        models.Lesson
	IsSolved      bool
	SolvedTaskIDs []uint
}

type courseService struct {
	repo    repository.CourseRepository
	storage *storage.MinioStorage
}

func NewCourseService(repo repository.CourseRepository, st *storage.MinioStorage) CourseService {
	return &courseService{repo: repo, storage: st}
}

func (s *courseService) CreateCourse(ctx context.Context, req CreateCourseInput) (*models.Course, error) {
	if strings.TrimSpace(req.Title) == "" {
		return nil, ErrEmptyCourseTitle
	}
	if req.Hours <= 0 {
		return nil, ErrInvalidHours
	}
	level := normalizeLevel(req.Level)
	if !isValidLevel(level) {
		return nil, ErrUnsupportedLevel
	}

	ok, err := s.repo.CategoryExists(req.CategoryID)
	if err != nil {
		return nil, err
	}
	if !ok {
		return nil, ErrCategoryNotExists
	}

	imageURL := ""
	if req.Image != nil {
		if s.storage == nil {
			return nil, ErrStorageNotAvailable
		}
		imageName := fmt.Sprintf("course_%d_%d", req.CategoryID, time.Now().UnixNano())
		imageURL, err = s.storage.UploadFile(ctx, imageName, req.Image, req.ImageSize, req.ContentType)
		if err != nil {
			return nil, err
		}
	}

	course := &models.Course{
		Title:       req.Title,
		Description: req.Description,
		ImageURL:    imageURL,
		Level:       level,
		Hours:       req.Hours,
		CategoryID:  req.CategoryID,
	}

	return s.repo.Create(course)
}

func (s *courseService) UpdateCourse(ctx context.Context, id int, req UpdateCourseInput) (*models.Course, error) {
	course, err := s.repo.GetByID(id)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCourseNotFound
	}
	if err != nil {
		return nil, err
	}

	if strings.TrimSpace(req.Title) == "" {
		return nil, ErrEmptyCourseTitle
	}
	if req.Hours <= 0 {
		return nil, ErrInvalidHours
	}
	level := normalizeLevel(req.Level)
	if !isValidLevel(level) {
		return nil, ErrUnsupportedLevel
	}

	ok, err := s.repo.CategoryExists(req.CategoryID)
	if err != nil {
		return nil, err
	}
	if !ok {
		return nil, ErrCategoryNotExists
	}

	if req.Image != nil {
		if s.storage == nil {
			return nil, ErrStorageNotAvailable
		}
		imageName := fmt.Sprintf("course_%d_%d", req.CategoryID, time.Now().UnixNano())
		uploadedURL, uploadErr := s.storage.UploadFile(ctx, imageName, req.Image, req.ImageSize, req.ContentType)
		if uploadErr != nil {
			return nil, uploadErr
		}
		course.ImageURL = uploadedURL
	}

	course.Title = req.Title
	course.Description = req.Description
	course.Level = level
	course.Hours = req.Hours
	course.CategoryID = req.CategoryID

	return s.repo.Update(course)
}

func (s *courseService) DeleteCourse(id int) error {
	course, err := s.repo.GetByID(id)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return ErrCourseNotFound
	}
	if err != nil {
		return err
	}

	return s.repo.Delete(course)
}

func (s *courseService) GetCourseByID(id int, userID *uint) (*CourseWithStatus, error) {
	course, err := s.repo.GetByID(id)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCourseNotFound
	}
	if err != nil {
		return nil, err
	}

	solvedTaskIDs, err := s.buildSolvedTaskSet(userID)
	if err != nil {
		return nil, err
	}

	return &CourseWithStatus{
		Course:   *course,
		IsSolved: isCourseSolved(course.Lessons, solvedTaskIDs),
	}, nil
}

func (s *courseService) ListCourses(userID *uint) ([]CourseWithStatus, error) {
	courses, err := s.repo.List()
	if err != nil {
		return nil, err
	}

	solvedTaskIDs, err := s.buildSolvedTaskSet(userID)
	if err != nil {
		return nil, err
	}

	result := make([]CourseWithStatus, 0, len(courses))
	for _, course := range courses {
		result = append(result, CourseWithStatus{
			Course:   course,
			IsSolved: isCourseSolved(course.Lessons, solvedTaskIDs),
		})
	}

	return result, nil
}

func (s *courseService) CreateLesson(courseID int, req CreateLessonInput) (*models.Lesson, error) {
	if strings.TrimSpace(req.Title) == "" {
		return nil, errors.New("lesson title is required")
	}
	if req.Order <= 0 {
		return nil, errors.New("lesson order must be greater than 0")
	}

	course, err := s.repo.GetByID(courseID)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCourseNotFound
	}
	if err != nil {
		return nil, err
	}

	tasks, err := buildLessonTasks(req.Tasks)
	if err != nil {
		return nil, err
	}

	lesson := &models.Lesson{
		CourseID:         course.ID,
		Title:            req.Title,
		Description:      req.Description,
		Order:            req.Order,
		EstimatedMinutes: req.EstimatedMinutes,
		IsFreePreview:    req.IsFreePreview,
	}

	return s.repo.CreateLessonWithTasks(lesson, tasks)
}

func (s *courseService) ListLessons(courseID int, userID *uint) ([]LessonWithStatus, error) {
	course, err := s.repo.GetByID(courseID)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCourseNotFound
	}
	if err != nil {
		return nil, err
	}

	lessons, err := s.repo.ListLessonsByCourseID(course.ID)
	if err != nil {
		return nil, err
	}

	solvedTaskIDs, err := s.buildSolvedTaskSet(userID)
	if err != nil {
		return nil, err
	}

	result := make([]LessonWithStatus, 0, len(lessons))
	for _, lesson := range lessons {
		result = append(result, LessonWithStatus{
			Lesson:        lesson,
			IsSolved:      isLessonSolved(lesson, solvedTaskIDs),
			SolvedTaskIDs: solvedTaskIDsForLesson(lesson, solvedTaskIDs),
		})
	}

	return result, nil
}

func (s *courseService) SubmitLessonTask(userID uint, lessonID int, taskID int, req SubmitLessonTaskInput) (*SubmitLessonTaskResult, error) {
	task, err := s.repo.GetLessonTask(lessonID, taskID)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrTaskNotFound
	}
	if err != nil {
		return nil, err
	}

	isSolved, submittedOptionIDs, err := evaluateTaskSubmission(task, req)
	if err != nil {
		return nil, err
	}

	progress, user, awardedStars, awardedExp, err := s.repo.SaveTaskProgress(userID, task, isSolved, submittedOptionIDs)
	if err != nil {
		return nil, err
	}

	return &SubmitLessonTaskResult{
		TaskID:       task.ID,
		IsSolved:     progress.IsSolved,
		WasSolved:    progress.IsSolved && awardedStars == 0 && awardedExp == 0,
		AwardedStars: awardedStars,
		AwardedExp:   awardedExp,
		CurrentStars: user.Stars,
		CurrentExp:   user.Exp,
		CurrentLevel: models.CalculateLevel(user.Exp),
	}, nil
}

func evaluateTaskSubmission(task *models.LessonTask, req SubmitLessonTaskInput) (bool, []string, error) {
	switch task.Type {
	case models.TaskTypeQuiz:
		submittedOptionIDs := normalizeOptionIDs(req.SelectedOptionIDs)
		if len(submittedOptionIDs) == 0 {
			return false, nil, ErrQuizAnswerRequired
		}
		return slices.Equal(submittedOptionIDs, normalizeOptionIDs(task.CorrectOptionIDs)), submittedOptionIDs, nil
	case models.TaskTypeFlowchart, models.TaskTypeAlgorithm:
		return req.Solved, nil, nil
	default:
		return false, nil, ErrUnsupportedTaskType
	}
}

func buildLessonTasks(inputs []CreateLessonTaskInput) ([]models.LessonTask, error) {
	tasks := make([]models.LessonTask, 0, len(inputs))
	for _, input := range inputs {
		taskType := strings.TrimSpace(strings.ToLower(input.Type))
		if taskType == "" {
			return nil, ErrInvalidTaskConfig
		}

		task := models.LessonTask{
			Type:        taskType,
			Title:       strings.TrimSpace(input.Title),
			Description: strings.TrimSpace(input.Description),
			Question:    strings.TrimSpace(input.Question),
			RewardStars: models.TaskRewardStars,
			RewardExp:   models.TaskRewardExp,
		}

		if task.Title == "" {
			return nil, ErrInvalidTaskConfig
		}

		switch taskType {
		case models.TaskTypeQuiz:
			if task.Question == "" {
				return nil, ErrInvalidTaskConfig
			}

			options := make([]models.TaskOption, 0, len(input.Options))
			seenIDs := map[string]struct{}{}
			for index, option := range input.Options {
				optionID := strings.TrimSpace(option.ID)
				if optionID == "" {
					optionID = fmt.Sprintf("option_%d", index+1)
				}
				if _, exists := seenIDs[optionID]; exists {
					return nil, ErrInvalidTaskConfig
				}
				seenIDs[optionID] = struct{}{}

				text := strings.TrimSpace(option.Text)
				if text == "" {
					return nil, ErrInvalidTaskConfig
				}

				options = append(options, models.TaskOption{
					ID:   optionID,
					Text: text,
				})
			}

			if len(options) < 2 {
				return nil, ErrInvalidTaskConfig
			}

			correctOptionIDs := normalizeOptionIDs(input.CorrectOptionIDs)
			if len(correctOptionIDs) == 0 {
				return nil, ErrInvalidTaskConfig
			}
			for _, correctID := range correctOptionIDs {
				if _, exists := seenIDs[correctID]; !exists {
					return nil, ErrInvalidTaskConfig
				}
			}

			task.Options = options
			task.CorrectOptionIDs = correctOptionIDs
		case models.TaskTypeFlowchart, models.TaskTypeAlgorithm:
			task.Options = nil
			task.CorrectOptionIDs = nil
		default:
			return nil, ErrUnsupportedTaskType
		}

		tasks = append(tasks, task)
	}

	return tasks, nil
}

func normalizeOptionIDs(optionIDs []string) []string {
	if len(optionIDs) == 0 {
		return nil
	}

	seen := map[string]struct{}{}
	normalized := make([]string, 0, len(optionIDs))
	for _, optionID := range optionIDs {
		value := strings.TrimSpace(optionID)
		if value == "" {
			continue
		}
		if _, exists := seen[value]; exists {
			continue
		}
		seen[value] = struct{}{}
		normalized = append(normalized, value)
	}

	slices.Sort(normalized)
	return normalized
}

func (s *courseService) buildSolvedTaskSet(userID *uint) (map[uint]struct{}, error) {
	if userID == nil {
		return map[uint]struct{}{}, nil
	}

	solvedTaskIDs, err := s.repo.GetSolvedTaskIDsByUser(*userID)
	if err != nil {
		return nil, err
	}

	taskSet := make(map[uint]struct{}, len(solvedTaskIDs))
	for _, taskID := range solvedTaskIDs {
		taskSet[taskID] = struct{}{}
	}

	return taskSet, nil
}

func isLessonSolved(lesson models.Lesson, solvedTaskIDs map[uint]struct{}) bool {
	if len(lesson.Tasks) == 0 {
		return false
	}

	for _, task := range lesson.Tasks {
		if _, ok := solvedTaskIDs[task.ID]; !ok {
			return false
		}
	}

	return true
}

func isCourseSolved(lessons []models.Lesson, solvedTaskIDs map[uint]struct{}) bool {
	if len(lessons) == 0 {
		return false
	}

	for _, lesson := range lessons {
		if !isLessonSolved(lesson, solvedTaskIDs) {
			return false
		}
	}

	return true
}

func solvedTaskIDsForLesson(lesson models.Lesson, solvedTaskIDs map[uint]struct{}) []uint {
	result := make([]uint, 0, len(lesson.Tasks))
	for _, task := range lesson.Tasks {
		if _, ok := solvedTaskIDs[task.ID]; ok {
			result = append(result, task.ID)
		}
	}

	return result
}

func isValidLevel(level string) bool {
	switch level {
	case models.CourseLevelBeginner, models.CourseLevelIntermediate, models.CourseLevelAdvanced:
		return true
	default:
		return false
	}
}

func normalizeLevel(level string) string {
	switch strings.TrimSpace(strings.ToLower(level)) {
	case "beginner":
		return models.CourseLevelBeginner
	case "intermediate":
		return models.CourseLevelIntermediate
	case "advanced":
		return models.CourseLevelAdvanced
	default:
		return strings.TrimSpace(strings.ToLower(level))
	}
}
