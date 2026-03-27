package presenter

import "github.com/FantomStudy/fluffy-doodle/internal/models"

type CourseResponse struct {
	ID           uint   `json:"id"`
	Title        string `json:"title"`
	Description  string `json:"description"`
	ImageURL     string `json:"imageUrl"`
	Level        string `json:"level"`
	Hours        int    `json:"hours"`
	CategoryID   uint   `json:"categoryId"`
	CategoryName string `json:"categoryName"`
	TotalLessons int    `json:"totalLessons"`
}

type LessonResponse struct {
	ID               uint                 `json:"id"`
	CourseID         uint                 `json:"courseId"`
	Title            string               `json:"title"`
	Description      string               `json:"description"`
	Order            int                  `json:"order"`
	EstimatedMinutes int                  `json:"estimatedMinutes"`
	IsFreePreview    bool                 `json:"isFreePreview"`
	Tasks            []LessonTaskResponse `json:"tasks"`
}

type CreateLessonRequest struct {
	Title            string                    `json:"title" example:"Intro to Python"`
	Description      string                    `json:"description" example:"First lesson of the course"`
	Order            int                       `json:"order" example:"1"`
	EstimatedMinutes int                       `json:"estimatedMinutes" example:"25"`
	IsFreePreview    bool                      `json:"isFreePreview" example:"true"`
	Tasks            []CreateLessonTaskRequest `json:"tasks"`
}

type CreateLessonTaskRequest struct {
	Type             string                    `json:"type" example:"quiz"`
	Title            string                    `json:"title" example:"Базовый тест"`
	Description      string                    `json:"description" example:"Проверьте понимание темы"`
	Question         string                    `json:"question" example:"Какие варианты верные?"`
	Options          []CreateTaskOptionRequest `json:"options,omitempty"`
	CorrectOptionIDs []string                  `json:"correctOptionIds,omitempty"`
}

type CreateTaskOptionRequest struct {
	ID   string `json:"id" example:"a"`
	Text string `json:"text" example:"Ответ A"`
}

type LessonTaskResponse struct {
	ID          uint                 `json:"id"`
	LessonID    uint                 `json:"lessonId"`
	Type        string               `json:"type"`
	Title       string               `json:"title"`
	Description string               `json:"description"`
	Question    string               `json:"question"`
	Options     []TaskOptionResponse `json:"options,omitempty"`
	RewardStars int                  `json:"rewardStars"`
	RewardExp   int                  `json:"rewardExp"`
}

type TaskOptionResponse struct {
	ID   string `json:"id"`
	Text string `json:"text"`
}

type SubmitLessonTaskRequest struct {
	SelectedOptionIDs []string `json:"selectedOptionIds,omitempty"`
	Solved            bool     `json:"solved"`
}

type SubmitLessonTaskResponse struct {
	TaskID       uint `json:"taskId"`
	IsSolved     bool `json:"isSolved"`
	WasSolved    bool `json:"wasSolved"`
	AwardedStars int  `json:"awardedStars"`
	AwardedExp   int  `json:"awardedExp"`
	CurrentStars int  `json:"currentStars"`
	CurrentExp   int  `json:"currentExp"`
	CurrentLevel int  `json:"currentLevel"`
}

func CourseToResponse(course *models.Course) CourseResponse {
	return CourseResponse{
		ID:           course.ID,
		Title:        course.Title,
		Description:  course.Description,
		ImageURL:     course.ImageURL,
		Level:        course.Level,
		Hours:        course.Hours,
		CategoryID:   course.CategoryID,
		CategoryName: course.Category.Name,
		TotalLessons: course.TotalLessons,
	}
}

func CoursesToResponse(courses []models.Course) []CourseResponse {
	result := make([]CourseResponse, 0, len(courses))
	for _, course := range courses {
		result = append(result, CourseToResponse(&course))
	}
	return result
}

func LessonsToResponse(lessons []models.Lesson) []LessonResponse {
	result := make([]LessonResponse, 0, len(lessons))
	for _, lesson := range lessons {
		result = append(result, LessonResponse{
			ID:               lesson.ID,
			CourseID:         lesson.CourseID,
			Title:            lesson.Title,
			Description:      lesson.Description,
			Order:            lesson.Order,
			EstimatedMinutes: lesson.EstimatedMinutes,
			IsFreePreview:    lesson.IsFreePreview,
			Tasks:            LessonTasksToResponse(lesson.Tasks),
		})
	}
	return result
}

func LessonTasksToResponse(tasks []models.LessonTask) []LessonTaskResponse {
	result := make([]LessonTaskResponse, 0, len(tasks))
	for _, task := range tasks {
		options := make([]TaskOptionResponse, 0, len(task.Options))
		for _, option := range task.Options {
			options = append(options, TaskOptionResponse{
				ID:   option.ID,
				Text: option.Text,
			})
		}

		result = append(result, LessonTaskResponse{
			ID:          task.ID,
			LessonID:    task.LessonID,
			Type:        task.Type,
			Title:       task.Title,
			Description: task.Description,
			Question:    task.Question,
			Options:     options,
			RewardStars: task.RewardStars,
			RewardExp:   task.RewardExp,
		})
	}
	return result
}
