package repository

import (
	"crypto/rand"
	"encoding/base32"
	"errors"
	"strings"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/gorm"
)

var (
	ErrParentAlreadyLinked = errors.New("student is already linked to parent")
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
	return &authRepository{db: db}
}

func (r *authRepository) SignUp(req *presenter.SignUpRequest) (*models.User, error) {
	if strings.TrimSpace(req.InvitationCode) == "" {
		return r.signUpStudent(req)
	}
	return r.signUpParent(req)
}

func (r *authRepository) signUpStudent(req *presenter.SignUpRequest) (*models.User, error) {
	var studentRole models.Role
	if err := r.db.Where("name = ?", "student").First(&studentRole).Error; err != nil {
		return nil, errors.New("default student role not found")
	}

	user := &models.User{
		Login:          req.Login,
		Password:       req.Password,
		FullName:       req.FullName,
		PhoneNumber:    req.PhoneNumber,
		RoleID:         studentRole.ID,
		InvitationCode: req.InvitationCode,
	}

	if err := r.db.Create(user).Error; err != nil {
		return nil, err
	}

	return user, nil
}

func (r *authRepository) signUpParent(req *presenter.SignUpRequest) (*models.User, error) {
	var student models.User
	if err := r.db.Preload("Role").Where("invitation_code = ?", req.InvitationCode).First(&student).Error; err != nil {
		return nil, err
	}
	if student.Role.Name != "student" {
		return nil, errors.New("provided code does not belong to student")
	}
	if student.ParentID != nil {
		return nil, ErrParentAlreadyLinked
	}

	var parentRole models.Role
	if err := r.db.Where("name = ?", "parent").First(&parentRole).Error; err != nil {
		return nil, errors.New("parent role not found")
	}

	var createdParent *models.User
	err := r.db.Transaction(func(tx *gorm.DB) error {
		parentCode, codeErr := generateInvitationCode("PAR")
		if codeErr != nil {
			return codeErr
		}

		parent := &models.User{
			Login:          req.Login,
			Password:       req.Password,
			FullName:       req.FullName,
			PhoneNumber:    req.PhoneNumber,
			RoleID:         parentRole.ID,
			InvitationCode: parentCode,
		}
		if err := tx.Create(parent).Error; err != nil {
			return err
		}

		if err := tx.Model(&models.User{}).Where("id = ?", student.ID).Update("parent_id", parent.ID).Error; err != nil {
			return err
		}

		createdParent = parent
		return nil
	})
	if err != nil {
		return nil, err
	}

	return createdParent, nil
}

func generateInvitationCode(prefix string) (string, error) {
	b := make([]byte, 5)
	if _, err := rand.Read(b); err != nil {
		return "", err
	}

	code := strings.TrimRight(base32.StdEncoding.WithPadding(base32.NoPadding).EncodeToString(b), "=")
	return prefix + "-" + code, nil
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

	if err := r.db.Model(&models.User{}).Where("id = ?", id).Update("refresh_token", token).Error; err != nil {
		return nil, err
	}

	user.RefreshToken = token
	return &user, nil
}
