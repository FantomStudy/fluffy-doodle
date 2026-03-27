package models

import "gorm.io/gorm"

type User struct {
	gorm.Model
	Login          string `gorm:"not null;unique;" json:"login"`
	Password       string `gorm:"not null" json:"-"`
	FullName       string `gorm:"not null" json:"fullName"`
	PhoneNumber    string `gorm:"not null" json:"phoneNumber"`
	RefreshToken   string `json:"-"`
	Avatar         string `json:"avatar"`
	Stars          int    `gorm:"default:0" json:"stars"`
	Exp            int    `gorm:"default:0" json:"exp"`
	InvitationCode string `gorm:"uniqueIndex" json:"invitationCode,omitempty"`
	ParentID       *uint  `gorm:"index" json:"parentId,omitempty"`
	// Роль
	RoleID uint `json:"roleId"`
	Role   Role `gorm:"foreignKey:RoleID" json:"role,omitempty"`
	// Достижения
	Achievements []Achievement           `gorm:"many2many:user_achievements;" json:"achievements"`
	Children     []User                  `gorm:"foreignKey:ParentID" json:"children,omitempty"`
	TaskProgress []UserTaskProgress      `gorm:"foreignKey:UserID" json:"taskProgress,omitempty"`
	GameProgress []UserGameLevelProgress `gorm:"foreignKey:UserID" json:"gameProgress,omitempty"`
}
