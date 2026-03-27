package repository

import (
	"errors"
	"strings"
	"time"

	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/gorm"
)

type UserRepository interface {
	GetUser(login string) (*models.User, error)
	GetUserByID(id uint) (*models.User, error)
	GetUserByInvitationCode(code string) (*models.User, error)
	AssignParentToStudent(studentID uint, parentID uint) error
	GetChildByParentID(parentID uint) (*models.User, error)
	UpdateUser(user *models.User) (*models.User, error)
	SetRefresh(token string, id int) (*models.User, error)
	CompleteGameLevel(userID uint, levelID string) (*models.UserGameLevelProgress, *models.User, int, int, error)
}

func (r *userRepository) GetUserByID(id uint) (*models.User, error) {
	var user models.User

	if err := r.db.Preload("Role").Preload("Achievements").Where("id = ?", id).First(&user).Error; err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *userRepository) UpdateUser(user *models.User) (*models.User, error) {
	if err := r.db.Save(user).Error; err != nil {
		return nil, err
	}

	return user, nil
}

func (r *userRepository) GetUserByInvitationCode(code string) (*models.User, error) {
	var user models.User
	if err := r.db.Preload("Role").Where("invitation_code = ?", code).First(&user).Error; err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *userRepository) AssignParentToStudent(studentID uint, parentID uint) error {
	if err := r.db.Model(&models.User{}).Where("id = ?", studentID).Update("parent_id", parentID).Error; err != nil {
		return err
	}

	return nil
}

func (r *userRepository) GetChildByParentID(parentID uint) (*models.User, error) {
	var student models.User
	if err := r.db.Preload("Achievements").Where("parent_id = ?", parentID).First(&student).Error; err != nil {
		return nil, err
	}

	return &student, nil
}

type userRepository struct {
	db *gorm.DB
}

func NewUserRepo(db *gorm.DB) UserRepository {
	return &userRepository{
		db: db,
	}
}

func (r *userRepository) GetUser(login string) (*models.User, error) {
	var user models.User

	if err := r.db.Where("login = ?", login).First(&user).Error; err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *userRepository) SetRefresh(token string, id int) (*models.User, error) {
	var user models.User

	if err := r.db.Where("id = ?", id).First(&user).Error; err != nil {
		return nil, err
	}

	user.RefreshToken = token

	if err := r.db.Save(&user).Error; err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *userRepository) CompleteGameLevel(userID uint, levelID string) (*models.UserGameLevelProgress, *models.User, int, int, error) {
	var progress models.UserGameLevelProgress
	var user models.User
	awardedStars := 0
	awardedExp := 0
	levelID = strings.TrimSpace(levelID)

	err := r.db.Transaction(func(tx *gorm.DB) error {
		if err := tx.Where("id = ?", userID).First(&user).Error; err != nil {
			return err
		}

		err := tx.Where("user_id = ? AND level_id = ?", userID, levelID).First(&progress).Error
		if err != nil {
			if !errors.Is(err, gorm.ErrRecordNotFound) {
				return err
			}

			progress = models.UserGameLevelProgress{
				UserID:  userID,
				LevelID: levelID,
			}
		}

		now := time.Now()
		progress.IsCompleted = true
		if progress.CompletedAt == nil {
			progress.CompletedAt = &now
		}

		if !progress.Awarded {
			progress.Awarded = true
			awardedStars = models.GameLevelRewardStars
			awardedExp = models.GameLevelRewardExp
			user.Stars += awardedStars
			user.Exp += awardedExp

			if err := tx.Save(&user).Error; err != nil {
				return err
			}
		}

		if progress.ID == 0 {
			return tx.Create(&progress).Error
		}

		return tx.Save(&progress).Error
	})
	if err != nil {
		return nil, nil, 0, 0, err
	}

	return &progress, &user, awardedStars, awardedExp, nil
}
