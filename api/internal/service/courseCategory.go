package service

import (
	"errors"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
	"gorm.io/gorm"
)

var (
	ErrCategoryAlreadyExists = errors.New("course category already exists")
	ErrCategoryNotFound      = errors.New("course category not found")
)

type CourseCategoryService interface {
	InsertCategory(req *presenter.InsertCategoryRequest) (*models.CourseCategory, error)
	UpdateCategory(req *presenter.UpdateCategoryRequest, id int) (*models.CourseCategory, error)
	DeleteCourseCategory(id int) (*models.CourseCategory, error)
	FetchCategories() (*[]models.CourseCategory, error)
	FetchCategoryById(id int) (*models.CourseCategory, error)
}

type courseCategoryService struct {
	r repository.CourseCategoryRepository
}

func NewCourseCategoryService(r repository.CourseCategoryRepository) CourseCategoryService {
	return &courseCategoryService{r: r}
}

func (s *courseCategoryService) InsertCategory(req *presenter.InsertCategoryRequest) (*models.CourseCategory, error) {
	checkCategory, err := s.r.FetchCategory(req.Name)
	if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, err
	}
	if checkCategory != nil && (checkCategory.ID != 0 || checkCategory.Name != "") {
		return nil, ErrCategoryAlreadyExists
	}

	return s.r.InsertCategory(req)
}

func (s *courseCategoryService) UpdateCategory(req *presenter.UpdateCategoryRequest, id int) (*models.CourseCategory, error) {
	checkCategory, err := s.r.FetchCategory(req.Name)
	if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, err
	}
	if checkCategory != nil && checkCategory.Name == req.Name && checkCategory.ID != uint(id) {
		return nil, ErrCategoryAlreadyExists
	}

	category, err := s.r.UpdateCategory(req, id)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCategoryNotFound
	}
	if err != nil {
		return nil, err
	}

	return category, nil
}

func (s *courseCategoryService) DeleteCourseCategory(id int) (*models.CourseCategory, error) {
	_, err := s.r.FetchCategoryById(id)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCategoryNotFound
	}
	if err != nil {
		return nil, err
	}

	return s.r.DeleteCourseCategory(id)
}

func (s *courseCategoryService) FetchCategories() (*[]models.CourseCategory, error) {
	return s.r.FetchCategories()
}

func (s *courseCategoryService) FetchCategoryById(id int) (*models.CourseCategory, error) {
	category, err := s.r.FetchCategoryById(id)
	if errors.Is(err, gorm.ErrRecordNotFound) {
		return nil, ErrCategoryNotFound
	}
	if err != nil {
		return nil, err
	}

	return category, nil
}
