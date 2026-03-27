package service

import (
	"context"
	"errors"
	"fmt"
	"io"
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
)

type CourseService interface {
	CreateCourse(ctx context.Context, req CreateCourseInput) (*models.Course, error)
	UpdateCourse(ctx context.Context, id int, req UpdateCourseInput) (*models.Course, error)
	DeleteCourse(id int) error
	GetCourseByID(id int) (*models.Course, error)
	ListCourses() ([]models.Course, error)
	CreateLesson(courseID int, req CreateLessonInput) (*models.Lesson, error)
	ListLessons(courseID int) ([]models.Lesson, error)
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

func (s *courseService) GetCourseByID(id int) (*models.Course, error) {
	course, err := s.repo.GetByID(id)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCourseNotFound
	}
	if err != nil {
		return nil, err
	}

	return course, nil
}

func (s *courseService) ListCourses() ([]models.Course, error) {
	return s.repo.List()
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

	lesson := &models.Lesson{
		CourseID:         course.ID,
		Title:            req.Title,
		Description:      req.Description,
		Order:            req.Order,
		EstimatedMinutes: req.EstimatedMinutes,
		IsFreePreview:    req.IsFreePreview,
	}

	created, err := s.repo.CreateLesson(lesson)
	if err != nil {
		return nil, err
	}

	course.TotalLessons++
	if _, err = s.repo.Update(course); err != nil {
		return nil, err
	}

	return created, nil
}

func (s *courseService) ListLessons(courseID int) ([]models.Lesson, error) {
	course, err := s.repo.GetByID(courseID)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCourseNotFound
	}
	if err != nil {
		return nil, err
	}

	return s.repo.ListLessonsByCourseID(course.ID)
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
	case "начальный", "beginner":
		return models.CourseLevelBeginner
	case "средний", "intermediate":
		return models.CourseLevelIntermediate
	case "сложный", "advanced":
		return models.CourseLevelAdvanced
	default:
		return strings.TrimSpace(strings.ToLower(level))
	}
}
