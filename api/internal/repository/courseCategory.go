package repository

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/gorm"
)

type CourseCategoryRepository interface {
	InsertCategory(req *presenter.InsertCategoryRequest) (*models.CourseCategory, error)
	UpdateCategory(req *presenter.UpdateCategoryRequest, id int) (*models.CourseCategory, error)
	DeleteCourseCategory(id int) (*models.CourseCategory, error)
	FetchCategories() (*[]models.CourseCategory, error)
	FetchCategory(name string) (*models.CourseCategory, error)
	FetchCategoryById(id int) (*models.CourseCategory, error)
}

type courseCategoryRepository struct {
	db *gorm.DB
}

func NewCourseCategoryRepository(db *gorm.DB) CourseCategoryRepository {
	return &courseCategoryRepository{
		db: db,
	}
}

func (r *courseCategoryRepository) InsertCategory(req *presenter.InsertCategoryRequest) (*models.CourseCategory, error) {
	category := &models.CourseCategory{
		Name: req.Name,
	}

	if err := r.db.Create(category).Error; err != nil {
		return nil, err
	}

	return category, nil
}

func (r *courseCategoryRepository) UpdateCategory(req *presenter.UpdateCategoryRequest, id int) (*models.CourseCategory, error) {
	var category models.CourseCategory

	if err := r.db.Where("id = ?", id).First(&category).Error; err != nil {
		return nil, err
	}

	category.Name = req.Name

	if err := r.db.Save(&category).Error; err != nil {
		return nil, err
	}

	return &category, nil
}

func (r *courseCategoryRepository) DeleteCourseCategory(id int) (*models.CourseCategory, error) {
	var category models.CourseCategory
	if err := r.db.Where("id = ?", id).First(&category).Error; err != nil {
		return nil, err
	}
	if err := r.db.Delete(&category).Error; err != nil {
		return nil, err
	}

	return &category, nil
}

func (r *courseCategoryRepository) FetchCategories() (*[]models.CourseCategory, error) {
	var categories []models.CourseCategory

	if err := r.db.Find(&categories).Error; err != nil {
		return nil, err
	}

	return &categories, nil
}

func (r *courseCategoryRepository) FetchCategory(name string) (*models.CourseCategory, error) {
	var category models.CourseCategory

	if err := r.db.Where("name = ?", name).First(&category).Error; err != nil {
		return nil, err
	}

	return &category, nil
}

func (r *courseCategoryRepository) FetchCategoryById(id int) (*models.CourseCategory, error) {
	var category models.CourseCategory

	if err := r.db.Where("id = ?", id).First(&category).Error; err != nil {
		return nil, err
	}

	return &category, nil
}
