package models

import "gorm.io/gorm"

type Frame struct {
	gorm.Model
	Name  string `gorm:"not null" json:"name"`
	Price int    `gorm:"not null" json:"price"`
	Image string `gorm:"not null" json:"image"`
}
