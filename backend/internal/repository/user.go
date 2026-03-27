package repository

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/gorm"
)

type UserRepository interface {
	SignUp(req *presenter.SignUpRequest, roleId uint) (*models.User, error)
	FindRole(role string) (*models.Role, error)
	GetUser(login string) (*models.User, error)
	SetRefresh(token string, id int) (*models.User, error)
}

type userRepository struct {
	db *gorm.DB
}

func NewUserRepo(db *gorm.DB) UserRepository {
	return &userRepository{
		db: db,
	}
}

func (r *userRepository) SignUp(req *presenter.SignUpRequest, roleId uint) (*models.User, error) {

	user := &models.User{
		Login:       req.Login,
		Password:    req.Password,
		FullName:    req.FullName,
		PhoneNumber: req.PhoneNumber,
		RoleID:      uint(roleId),
	}

	if err := r.db.Create(user).Error; err != nil {
		return nil, err
	}

	return user, nil
}

func (r *userRepository) FindRole(role string) (*models.Role, error) {
	var defaultRole models.Role
	if err := r.db.Where("name = ?", role).First(&defaultRole).Error; err != nil {
		return nil, err
	}

	return &defaultRole, nil
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
