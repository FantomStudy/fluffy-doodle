package repository

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/gorm"
)

type AuthRepository interface {
	SignUp(req *presenter.SignUpRequest) (*models.User, error)
	GetUser(login string) (*models.User, error)
	SetRefresh(token string, id int) (*models.User, error)
}

type authRepository struct {
	db *gorm.DB
}

func NewAuthRepo(db *gorm.DB) AuthRepository {
	return &authRepository{
		db: db,
	}
}

func (r *authRepository) SignUp(req *presenter.SignUpRequest) (*models.User, error) {
	user := &models.User{
		Login:       req.Login,
		Password:    req.Password,
		FullName:    req.FullName,
		PhoneNumber: req.PhoneNumber,
	}

	if err := r.db.Create(user).Error; err != nil {
		return nil, err
	}

	return user, nil
}

func (r *authRepository) GetUser(login string) (*models.User, error) {
	var user models.User

	if err := r.db.Where("login = ?", login).First(&user).Error; err != nil {
		return nil, err
	}

	return &user, nil
}

func (r *authRepository) SetRefresh(token string, id int) (*models.User, error) {
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
