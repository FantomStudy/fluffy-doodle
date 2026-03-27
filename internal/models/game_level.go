package models

import (
	"time"

	"gorm.io/gorm"
)

const (
	GameLevelRewardStars = 3
	GameLevelRewardExp   = 10
)

type UserGameLevelProgress struct {
	gorm.Model
	UserID      uint       `gorm:"not null;index:idx_user_game_level,unique" json:"userId"`
	LevelID     string     `gorm:"not null;index:idx_user_game_level,unique" json:"levelId"`
	IsCompleted bool       `gorm:"default:false" json:"isCompleted"`
	Awarded     bool       `gorm:"default:false" json:"awarded"`
	CompletedAt *time.Time `json:"completedAt,omitempty"`
}
