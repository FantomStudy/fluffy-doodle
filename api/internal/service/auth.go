package service

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
)

type AuthService interface {
	SignUp(*presenter.SignUpRequest) (*models.User, error)
	GetUser(login string) (*models.User, error)
	SetToken(token string, id int) (*models.User, error)
}

type authService struct {
	repository repository.AuthRepository
}

func NewAuthService(r repository.AuthRepository) AuthService {
	return &authService{
		repository: r,
	}
}

func (s *authService) SignUp(req *presenter.SignUpRequest) (*models.User, error) {
	return s.repository.SignUp(req)
}

func (s *authService) GetUser(login string) (*models.User, error) {
	return s.repository.GetUser(login)
}

func (s *authService) SetToken(token string, id int) (*models.User, error) {
	return s.repository.SetRefresh(token, id)
}
