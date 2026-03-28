package models

import "gorm.io/gorm"

type CourseCategory struct {
	gorm.Model
	Name    string   `gorm:"not null; unique" json:"name"`
	Courses []Course `gorm:"foreignKey:CategoryID" json:"courses,omitempty"`
}
