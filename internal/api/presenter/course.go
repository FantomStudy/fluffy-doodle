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
	ID               uint   `json:"id"`
	CourseID         uint   `json:"courseId"`
	Title            string `json:"title"`
	Description      string `json:"description"`
	Order            int    `json:"order"`
	EstimatedMinutes int    `json:"estimatedMinutes"`
	IsFreePreview    bool   `json:"isFreePreview"`
}

type CreateLessonRequest struct {
	Title            string `json:"title" example:"Знакомство с Python"`
	Description      string `json:"description" example:"Первый урок курса"`
	Order            int    `json:"order" example:"1"`
	EstimatedMinutes int    `json:"estimatedMinutes" example:"25"`
	IsFreePreview    bool   `json:"isFreePreview" example:"true"`
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
		})
	}
	return result
}
