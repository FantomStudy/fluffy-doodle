package models

import "gorm.io/gorm"

type Achievement struct {
	gorm.Model
	Name        string `gorm:"not null" json:"name"`
	Description string `json:"description"`
	Icon        string `json:"icon"`
}
