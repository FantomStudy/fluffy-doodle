package models

import "gorm.io/gorm"

type Role struct {
	gorm.Model
	Name  string `gorm:"not null; unique" json:"name"`
	Users []User `gorm:"foreignKey:RoleID" json:"users,omitempty"`
}
