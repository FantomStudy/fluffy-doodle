package models

import "gorm.io/gorm"

const (
	CourseLevelBeginner     = "beginner"
	CourseLevelIntermediate = "intermediate"
	CourseLevelAdvanced     = "advanced"
)

type Course struct {
	gorm.Model
	Title        string         `gorm:"not null" json:"title"`
	Description  string         `gorm:"type:text" json:"description"`
	ImageURL     string         `json:"imageUrl"`
	Level        string         `gorm:"not null;index" json:"level"`
	Hours        int            `gorm:"not null" json:"hours"`
	CategoryID   uint           `gorm:"not null;index" json:"categoryId"`
	Category     CourseCategory `gorm:"foreignKey:CategoryID" json:"category,omitempty"`
	TotalLessons int            `gorm:"default:0" json:"totalLessons"`
	Published    bool           `gorm:"default:false" json:"published"`
	Lessons      []Lesson       `gorm:"foreignKey:CourseID" json:"lessons,omitempty"`
}

type Lesson struct {
	gorm.Model
	CourseID         uint         `gorm:"not null;index" json:"courseId"`
	Title            string       `gorm:"not null" json:"title"`
	Description      string       `gorm:"type:text" json:"description"`
	Order            int          `gorm:"not null;index" json:"order"`
	EstimatedMinutes int          `gorm:"default:0" json:"estimatedMinutes"`
	IsFreePreview    bool         `gorm:"default:false" json:"isFreePreview"`
	Tasks            []LessonTask `gorm:"foreignKey:LessonID" json:"tasks,omitempty"`
}
