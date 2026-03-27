package presenter

import (
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/gofiber/fiber/v2"
)

// CourseCategory структура для ответа
type CourseCategory struct {
	ID   int    `json:"id" example:"1"`                  // ID категории
	Name string `json:"name" example:"Программирование"` // Название категории
}

// InsertCategoryRequest запрос на создание категории
type InsertCategoryRequest struct {
	Name string `json:"name" validate:"required" example:"Веб-разработка"` // Название категории
}

// UpdateCategoryRequest запрос на обновление категории
type UpdateCategoryRequest struct {
	Name string `json:"name" validate:"required" example:"Frontend разработка"` // Новое название категории
}

// SuccessResponse общий успешный ответ (для Swagger)
type SuccessResponse struct {
	Success bool        `json:"success" example:"true"`
	Message string      `json:"message" example:"Операция выполнена успешно"`
	Data    interface{} `json:"data,omitempty"`
}

// ErrorResponse общий ответ с ошибкой (для Swagger)
type ErrorResponse struct {
	Success bool   `json:"success" example:"false"`
	Error   string `json:"error" example:"Описание ошибки"`
}

// CourseCategorySuccessResponse форматирует ответ с одной категорией
func CourseCategorySuccessResponse(category *models.CourseCategory) *CourseCategory {
	return &CourseCategory{
		ID:   int(category.ID),
		Name: category.Name,
	}
}

// CourseCategoriesSuccessResponse форматирует ответ со списком категорий
func CourseCategoriesSuccessResponse(categories *[]models.CourseCategory) []CourseCategory {
	var result []CourseCategory
	for _, cat := range *categories {
		result = append(result, CourseCategory{
			ID:   int(cat.ID),
			Name: cat.Name,
		})
	}
	return result
}

// SuccessResponseWithData возвращает успешный ответ с данными
func SuccessResponseWithData(message string, data interface{}) *fiber.Map {
	return &fiber.Map{
		"success": true,
		"message": message,
		"data":    data,
	}
}

// SuccessMessageResponse возвращает успешный ответ без данных
func SuccessMessageResponse(message string) *fiber.Map {
	return &fiber.Map{
		"success": true,
		"message": message,
	}
}

// ErrorResponseFunc возвращает ответ с ошибкой
func ErrorResponseFunc(message string) *fiber.Map {
	return &fiber.Map{
		"success": false,
		"error":   message,
	}
}
