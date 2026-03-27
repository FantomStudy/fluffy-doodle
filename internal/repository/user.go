package repository

import (
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
