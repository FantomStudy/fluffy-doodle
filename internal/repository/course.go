package repository

import (
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/gorm"
)

type CourseRepository interface {
	Create(course *models.Course) (*models.Course, error)
	Update(course *models.Course) (*models.Course, error)
	Delete(course *models.Course) error
	GetByID(id int) (*models.Course, error)
	List() ([]models.Course, error)
	CategoryExists(categoryID uint) (bool, error)
	CreateLesson(lesson *models.Lesson) (*models.Lesson, error)
	ListLessonsByCourseID(courseID uint) ([]models.Lesson, error)
}

type courseRepository struct {
	db *gorm.DB
}

func NewCourseRepository(db *gorm.DB) CourseRepository {
	return &courseRepository{db: db}
}

func (r *courseRepository) Create(course *models.Course) (*models.Course, error) {
	if err := r.db.Create(course).Error; err != nil {
		return nil, err
	}
	return course, nil
}

func (r *courseRepository) Update(course *models.Course) (*models.Course, error) {
	if err := r.db.Save(course).Error; err != nil {
		return nil, err
	}
	return course, nil
}

func (r *courseRepository) Delete(course *models.Course) error {
	if err := r.db.Delete(course).Error; err != nil {
		return err
	}
	return nil
}

func (r *courseRepository) GetByID(id int) (*models.Course, error) {
	var course models.Course
	if err := r.db.Preload("Category").Preload("Lessons", func(db *gorm.DB) *gorm.DB {
		return db.Order("\"order\" ASC")
	}).First(&course, id).Error; err != nil {
		return nil, err
	}
	return &course, nil
}

func (r *courseRepository) List() ([]models.Course, error) {
	var courses []models.Course
	if err := r.db.Preload("Category").Order("created_at DESC").Find(&courses).Error; err != nil {
		return nil, err
	}
	return courses, nil
}

func (r *courseRepository) CategoryExists(categoryID uint) (bool, error) {
	var count int64
	if err := r.db.Model(&models.CourseCategory{}).Where("id = ?", categoryID).Count(&count).Error; err != nil {
		return false, err
	}
	return count > 0, nil
}

func (r *courseRepository) CreateLesson(lesson *models.Lesson) (*models.Lesson, error) {
	if err := r.db.Create(lesson).Error; err != nil {
		return nil, err
	}
	return lesson, nil
}

func (r *courseRepository) ListLessonsByCourseID(courseID uint) ([]models.Lesson, error) {
	var lessons []models.Lesson
	if err := r.db.Where("course_id = ?", courseID).Order("\"order\" ASC").Find(&lessons).Error; err != nil {
		return nil, err
	}
	return lessons, nil
}
