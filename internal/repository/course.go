package repository

import (
	"errors"
	"time"

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
	CreateLessonWithTasks(lesson *models.Lesson, tasks []models.LessonTask) (*models.Lesson, error)
	ListLessonsByCourseID(courseID uint) ([]models.Lesson, error)
	GetLessonTask(lessonID int, taskID int) (*models.LessonTask, error)
	SaveTaskProgress(userID uint, task *models.LessonTask, isSolved bool, submittedOptionIDs []string) (*models.UserTaskProgress, *models.User, int, int, error)
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
	if err := r.db.Preload("Category").
		Preload("Lessons", func(db *gorm.DB) *gorm.DB {
			return db.Order("\"order\" ASC")
		}).
		Preload("Lessons.Tasks", func(db *gorm.DB) *gorm.DB {
			return db.Order("id ASC")
		}).
		First(&course, id).Error; err != nil {
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

func (r *courseRepository) CreateLessonWithTasks(lesson *models.Lesson, tasks []models.LessonTask) (*models.Lesson, error) {
	err := r.db.Transaction(func(tx *gorm.DB) error {
		if err := tx.Create(lesson).Error; err != nil {
			return err
		}

		for i := range tasks {
			tasks[i].LessonID = lesson.ID
			if err := tx.Create(&tasks[i]).Error; err != nil {
				return err
			}
		}

		if err := tx.Model(&models.Course{}).
			Where("id = ?", lesson.CourseID).
			Update("total_lessons", gorm.Expr("total_lessons + 1")).Error; err != nil {
			return err
		}

		return nil
	})
	if err != nil {
		return nil, err
	}

	lesson.Tasks = tasks
	return lesson, nil
}

func (r *courseRepository) ListLessonsByCourseID(courseID uint) ([]models.Lesson, error) {
	var lessons []models.Lesson
	if err := r.db.Where("course_id = ?", courseID).
		Preload("Tasks", func(db *gorm.DB) *gorm.DB {
			return db.Order("id ASC")
		}).
		Order("\"order\" ASC").
		Find(&lessons).Error; err != nil {
		return nil, err
	}
	return lessons, nil
}

func (r *courseRepository) GetLessonTask(lessonID int, taskID int) (*models.LessonTask, error) {
	var task models.LessonTask
	if err := r.db.Where("lesson_id = ? AND id = ?", lessonID, taskID).First(&task).Error; err != nil {
		return nil, err
	}
	return &task, nil
}

func (r *courseRepository) SaveTaskProgress(userID uint, task *models.LessonTask, isSolved bool, submittedOptionIDs []string) (*models.UserTaskProgress, *models.User, int, int, error) {
	var progress models.UserTaskProgress
	var user models.User
	awardedStars := 0
	awardedExp := 0

	err := r.db.Transaction(func(tx *gorm.DB) error {
		if err := tx.Where("id = ?", userID).First(&user).Error; err != nil {
			return err
		}

		err := tx.Where("user_id = ? AND task_id = ?", userID, task.ID).First(&progress).Error
		if err != nil {
			if !errors.Is(err, gorm.ErrRecordNotFound) {
				return err
			}

			progress = models.UserTaskProgress{
				UserID: userID,
				TaskID: task.ID,
			}
		}

		progress.SubmittedOptionIDs = submittedOptionIDs

		if isSolved {
			now := time.Now()
			progress.IsSolved = true
			if progress.SolvedAt == nil {
				progress.SolvedAt = &now
			}

			if !progress.Awarded {
				progress.Awarded = true
				awardedStars = task.RewardStars
				awardedExp = task.RewardExp
				user.Stars += awardedStars
				user.Exp += awardedExp

				if err := tx.Save(&user).Error; err != nil {
					return err
				}
			}
		} else if !progress.Awarded {
			progress.IsSolved = false
			progress.SolvedAt = nil
		}

		if progress.ID == 0 {
			if err := tx.Create(&progress).Error; err != nil {
				return err
			}
			return nil
		}

		if err := tx.Save(&progress).Error; err != nil {
			return err
		}
		return nil
	})
	if err != nil {
		return nil, nil, 0, 0, err
	}

	return &progress, &user, awardedStars, awardedExp, nil
}
