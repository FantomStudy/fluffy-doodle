package service

import (
	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
)

type UserService interface {
	InviteUser(req *presenter.SignUpRequest, roleId uint) (*models.User, error)
	FindRole(role string) (*models.Role, error)
	GetUser(login string) (*models.User, error)
	SetToken(token string, id int) (*models.User, error)
}

type userService struct {
	repository repository.UserRepository
}

func NewUserService(r repository.UserRepository) UserService {
	return &userService{
		repository: r,
	}
}

func (s *userService) InviteUser(req *presenter.SignUpRequest, roleId uint) (*models.User, error) {
	return s.repository.SignUp(req, roleId)
}

func (s *userService) FindRole(role string) (*models.Role, error) {
	return s.repository.FindRole(role)
}

func (s *userService) GetUser(login string) (*models.User, error) {
	return s.repository.GetUser(login)
}

func (s *userService) SetToken(token string, id int) (*models.User, error) {
	return s.repository.SetRefresh(token, id)
}
