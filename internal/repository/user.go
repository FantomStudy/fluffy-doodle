package repository

import (
	"crypto/rand"
	"encoding/base32"
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
	SetInvitationCode(userID uint, code string) error
	CompleteGameLevel(userID uint, levelID string) (*models.UserGameLevelProgress, *models.User, int, int, error)
	GetLeaderboard(limit int) ([]models.User, error)
	GetFrames() ([]models.Frame, error)
	GetFrameByID(id uint) (*models.Frame, error)
	BuyFrame(userID uint, frameID uint) error
	SetActiveFrame(userID uint, frameID uint) error
}

type userRepository struct {
	db *gorm.DB
}

func NewUserRepo(db *gorm.DB) UserRepository {
	return &userRepository{
		db: db,
	}
}

func (r *userRepository) GetUserByID(id uint) (*models.User, error) {
	var user models.User

	if err := r.db.Preload("Role").Preload("Achievements").Preload("Frames").Preload("ActiveFrame").Where("id = ?", id).First(&user).Error; err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *userRepository) UpdateUser(user *models.User) (*models.User, error) {
	updates := map[string]any{
		"full_name":       user.FullName,
		"phone_number":    user.PhoneNumber,
		"avatar":          user.Avatar,
		"stars":           user.Stars,
		"exp":             user.Exp,
		"parent_id":       user.ParentID,
		"role_id":         user.RoleID,
		"refresh_token":   user.RefreshToken,
		"active_frame_id": user.ActiveFrameID,
	}

	if err := r.db.Model(&models.User{}).Where("id = ?", user.ID).Updates(updates).Error; err != nil {
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
	if err := r.db.Preload("Achievements").Preload("Frames").Preload("ActiveFrame").Where("parent_id = ?", parentID).First(&student).Error; err != nil {
		return nil, err
	}

	return &student, nil
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

	if err := r.db.Model(&models.User{}).Where("id = ?", id).Update("refresh_token", token).Error; err != nil {
		return nil, err
	}

	user.RefreshToken = token
	return &user, nil
}

func (r *userRepository) SetInvitationCode(userID uint, code string) error {
	return r.db.Model(&models.User{}).Where("id = ?", userID).Update("invitation_code", code).Error
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

func (r *userRepository) GetLeaderboard(limit int) ([]models.User, error) {
	var users []models.User
	if err := r.db.Preload("ActiveFrame").Order("stars desc").Limit(limit).Find(&users).Error; err != nil {
		return nil, err
	}
	return users, nil
}

func (r *userRepository) GetFrames() ([]models.Frame, error) {
	var frames []models.Frame
	if err := r.db.Find(&frames).Error; err != nil {
		return nil, err
	}
	return frames, nil
}

func (r *userRepository) GetFrameByID(id uint) (*models.Frame, error) {
	var frame models.Frame
	if err := r.db.First(&frame, id).Error; err != nil {
		return nil, err
	}
	return &frame, nil
}

func (r *userRepository) BuyFrame(userID uint, frameID uint) error {
	return r.db.Transaction(func(tx *gorm.DB) error {
		var user models.User
		if err := tx.Where("id = ?", userID).First(&user).Error; err != nil {
			return err
		}

		var frame models.Frame
		if err := tx.First(&frame, frameID).Error; err != nil {
			return err
		}

		if user.Stars < frame.Price {
			return errors.New("not enough stars")
		}

		// Check if user already has the frame
		var count int64
		if err := tx.Table("user_frames").Where("user_id = ? AND frame_id = ?", userID, frameID).Count(&count).Error; err != nil {
			return err
		}
		if count > 0 {
			return errors.New("frame already owned")
		}

		user.Stars -= frame.Price
		if err := tx.Save(&user).Error; err != nil {
			return err
		}

		if err := tx.Model(&user).Association("Frames").Append(&frame); err != nil {
			return err
		}

		return nil
	})
}

func (r *userRepository) SetActiveFrame(userID uint, frameID uint) error {
	return r.db.Transaction(func(tx *gorm.DB) error {
		var count int64
		if frameID != 0 {
			if err := tx.Table("user_frames").Where("user_id = ? AND frame_id = ?", userID, frameID).Count(&count).Error; err != nil {
				return err
			}
			if count == 0 {
				return errors.New("frame not owned")
			}
			return tx.Model(&models.User{}).Where("id = ?", userID).Update("active_frame_id", frameID).Error
		}
		return tx.Model(&models.User{}).Where("id = ?", userID).Update("active_frame_id", nil).Error
	})
}

func GenerateInvitationCode(prefix string) (string, error) {
	b := make([]byte, 5)
	if _, err := rand.Read(b); err != nil {
		return "", err
	}

	code := strings.TrimRight(base32.StdEncoding.WithPadding(base32.NoPadding).EncodeToString(b), "=")
	return prefix + "-" + code, nil
}
