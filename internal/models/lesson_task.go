package models

import (
	"time"

	"gorm.io/gorm"
)

const (
	TaskTypeQuiz      = "quiz"
	TaskTypeFlowchart = "flowchart"
	TaskTypeAlgorithm = "algorithm"
)

const (
	TaskRewardStars = 5
	TaskRewardExp   = 15
	ExpPerLevel     = 100
)

type TaskOption struct {
	ID   string `json:"id"`
	Text string `json:"text"`
}

type LessonTask struct {
	gorm.Model
	LessonID         uint         `gorm:"not null;index" json:"lessonId"`
	Title            string       `gorm:"not null" json:"title"`
	Description      string       `gorm:"type:text" json:"description"`
	Type             string       `gorm:"not null;index" json:"type"`
	Question         string       `gorm:"type:text" json:"question"`
	Options          []TaskOption `gorm:"type:jsonb;serializer:json" json:"options,omitempty"`
	CorrectOptionIDs []string     `gorm:"type:jsonb;serializer:json" json:"correctOptionIds,omitempty"`
	RewardStars      int          `gorm:"default:5" json:"rewardStars"`
	RewardExp        int          `gorm:"default:15" json:"rewardExp"`
}

type UserTaskProgress struct {
	gorm.Model
	UserID             uint       `gorm:"not null;index:idx_user_task_progress,unique" json:"userId"`
	TaskID             uint       `gorm:"not null;index:idx_user_task_progress,unique" json:"taskId"`
	IsSolved           bool       `gorm:"default:false" json:"isSolved"`
	Awarded            bool       `gorm:"default:false" json:"awarded"`
	SubmittedOptionIDs []string   `gorm:"type:jsonb;serializer:json" json:"submittedOptionIds,omitempty"`
	SolvedAt           *time.Time `json:"solvedAt,omitempty"`
}

func CalculateLevel(exp int) int {
	if exp < 0 {
		exp = 0
	}

	return exp/ExpPerLevel + 1
}

func ExpToNextLevel(exp int) int {
	if exp < 0 {
		exp = 0
	}

	nextLevelExp := CalculateLevel(exp) * ExpPerLevel
	return nextLevelExp - exp
}
