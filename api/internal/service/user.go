package service

import (
	"context"
	"errors"
	"io"
	"strings"

	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
	"github.com/FantomStudy/fluffy-doodle/pkg/storage"
	"gorm.io/gorm"
)

var ErrInvalidGameLevelID = errors.New("invalid game level id")

type UserService interface {
	GetUser(login string) (*models.User, error)
	GetUserByID(id uint) (*models.User, error)
	EnsureStudentInvitationCode(user *models.User) (*models.User, error)
	GetUserByInvitationCode(code string) (*models.User, error)
	AssignParentToStudent(studentID uint, parentID uint) error
	GetChildByParentID(parentID uint) (*models.User, error)
	UpdateUser(user *models.User) (*models.User, error)
	SetToken(token string, id int) (*models.User, error)
	UploadAvatar(ctx context.Context, fileName string, reader io.Reader, fileSize int64, contentType string) (string, error)
	CompleteGameLevel(userID uint, levelID string) (*CompleteGameLevelResult, error)
	GetLeaderboard(limit int) ([]models.User, error)
	GetFrames() ([]models.Frame, error)
	BuyFrame(userID uint, frameID uint) error
	SetActiveFrame(userID uint, frameID uint) error
}

type CompleteGameLevelResult struct {
	LevelID      string
	IsCompleted  bool
	WasCompleted bool
	AwardedStars int
	AwardedExp   int
	CurrentStars int
	CurrentExp   int
	CurrentLevel int
}

func (s *userService) GetUserByID(id uint) (*models.User, error) {
	return s.repository.GetUserByID(id)
}

func (s *userService) EnsureStudentInvitationCode(user *models.User) (*models.User, error) {
	if user == nil || user.RoleID != 3 || strings.TrimSpace(user.InvitationCode) != "" {
		return user, nil
	}

	for range 5 {
		code, err := repository.GenerateInvitationCode("STU")
		if err != nil {
			return nil, err
		}

		existing, err := s.repository.GetUserByInvitationCode(code)
		if err != nil && !errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, err
		}
		if existing != nil && existing.ID != 0 {
			continue
		}

		if err := s.repository.SetInvitationCode(user.ID, code); err != nil {
			return nil, err
		}
		user.InvitationCode = code
		return user, nil
	}

	return nil, errors.New("failed to generate unique student code")
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

func (s *userService) CompleteGameLevel(userID uint, levelID string) (*CompleteGameLevelResult, error) {
	levelID = strings.TrimSpace(levelID)
	if levelID == "" {
		return nil, ErrInvalidGameLevelID
	}

	progress, user, awardedStars, awardedExp, err := s.repository.CompleteGameLevel(userID, levelID)
	if err != nil {
		return nil, err
	}

	return &CompleteGameLevelResult{
		LevelID:      progress.LevelID,
		IsCompleted:  progress.IsCompleted,
		WasCompleted: progress.IsCompleted && awardedStars == 0 && awardedExp == 0,
		AwardedStars: awardedStars,
		AwardedExp:   awardedExp,
		CurrentStars: user.Stars,
		CurrentExp:   user.Exp,
		CurrentLevel: models.CalculateLevel(user.Exp),
	}, nil
}

func (s *userService) GetLeaderboard(limit int) ([]models.User, error) {
	if limit <= 0 {
		limit = 10
	}
	return s.repository.GetLeaderboard(limit)
}

func (s *userService) GetFrames() ([]models.Frame, error) {
	return s.repository.GetFrames()
}

func (s *userService) BuyFrame(userID uint, frameID uint) error {
	return s.repository.BuyFrame(userID, frameID)
}

func (s *userService) SetActiveFrame(userID uint, frameID uint) error {
	return s.repository.SetActiveFrame(userID, frameID)
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
