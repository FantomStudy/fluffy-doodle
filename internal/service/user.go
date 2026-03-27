package service

import (
	"context"
	"io"

	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
	"github.com/FantomStudy/fluffy-doodle/pkg/storage"
)

type UserService interface {
	GetUser(login string) (*models.User, error)
	GetUserByID(id uint) (*models.User, error)
	GetUserByInvitationCode(code string) (*models.User, error)
	AssignParentToStudent(studentID uint, parentID uint) error
	GetChildByParentID(parentID uint) (*models.User, error)
	UpdateUser(user *models.User) (*models.User, error)
	SetToken(token string, id int) (*models.User, error)
	UploadAvatar(ctx context.Context, fileName string, reader io.Reader, fileSize int64, contentType string) (string, error)
}

func (s *userService) GetUserByID(id uint) (*models.User, error) {
	return s.repository.GetUserByID(id)
}

func (s *userService) GetUserByInvitationCode(code string) (*models.User, error) {
	return s.repository.GetUserByInvitationCode(code)
}

func (s *userService) AssignParentToStudent(studentID uint, parentID uint) error {
	return s.repository.AssignParentToStudent(studentID, parentID)
}

func (s *userService) GetChildByParentID(parentID uint) (*models.User, error) {
	return s.repository.GetChildByParentID(parentID)
}

func (s *userService) UpdateUser(user *models.User) (*models.User, error) {
	return s.repository.UpdateUser(user)
}

func (s *userService) UploadAvatar(ctx context.Context, fileName string, reader io.Reader, fileSize int64, contentType string) (string, error) {
	return s.storage.UploadFile(ctx, fileName, reader, fileSize, contentType)
}

type userService struct {
	repository repository.UserRepository
	storage    *storage.MinioStorage
}

func NewUserService(r repository.UserRepository, st *storage.MinioStorage) UserService {
	return &userService{
		repository: r,
		storage:    st,
	}
}

func (s *userService) GetUser(login string) (*models.User, error) {
	return s.repository.GetUser(login)
}

func (s *userService) SetToken(token string, id int) (*models.User, error) {
	return s.repository.SetRefresh(token, id)
}
