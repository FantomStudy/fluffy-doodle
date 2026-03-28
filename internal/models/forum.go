package models

import "gorm.io/gorm"

type ForumCategory struct {
	gorm.Model
	Name        string       `gorm:"not null;unique" json:"name"`
	Description string       `json:"description"`
	Order       int          `gorm:"default:0" json:"order"`
	Topics      []ForumTopic `gorm:"foreignKey:CategoryID" json:"topics,omitempty"`
}

type ForumTopic struct {
	gorm.Model
	Title      string         `gorm:"not null" json:"title"`
	Content    string         `gorm:"type:text;not null" json:"content"`
	AuthorID   uint           `gorm:"not null;index" json:"authorId"`
	Author     User           `gorm:"foreignKey:AuthorID" json:"author,omitempty"`
	CategoryID uint           `gorm:"not null;index" json:"categoryId"`
	Category   ForumCategory  `gorm:"foreignKey:CategoryID" json:"category,omitempty"`
	Views      int            `gorm:"default:0" json:"views"`
	IsSolved   bool           `gorm:"default:false" json:"isSolved"`
	Comments   []ForumComment `gorm:"foreignKey:TopicID" json:"comments,omitempty"`
}

type ForumComment struct {
	gorm.Model
	Content    string     `gorm:"type:text;not null" json:"content"`
	TopicID    uint       `gorm:"not null;index" json:"topicId"`
	Topic      ForumTopic `gorm:"foreignKey:TopicID" json:"-"`
	AuthorID   uint       `gorm:"not null;index" json:"authorId"`
	Author     User       `gorm:"foreignKey:AuthorID" json:"author,omitempty"`
	IsSolution bool       `gorm:"default:false" json:"isSolution"`
}
