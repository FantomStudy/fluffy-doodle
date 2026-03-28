package presenter

import (
	"time"
)

type ForumAuthor struct {
	ID       uint   `json:"id"`
	FullName string `json:"fullName"`
	Avatar   string `json:"avatar"`
	Stars    int    `json:"stars"`
	Role     string `json:"role"`
}

type ForumCategoryResponse struct {
	ID          uint   `json:"id"`
	Name        string `json:"name"`
	Description string `json:"description"`
	TopicsCount int    `json:"topicsCount"`
}

type ForumTopicResponse struct {
	ID         uint        `json:"id"`
	Title      string      `json:"title"`
	Content    string      `json:"content"`
	Author     ForumAuthor `json:"author"`
	CategoryID uint        `json:"categoryId"`
	Views      int         `json:"views"`
	IsSolved   bool        `json:"isSolved"`
	CreatedAt  time.Time   `json:"createdAt"`
	Replies    int         `json:"replies"`
}

type ForumTopicDetailResponse struct {
	Topic    ForumTopicResponse     `json:"topic"`
	Comments []ForumCommentResponse `json:"comments"`
}

type ForumCommentResponse struct {
	ID         uint        `json:"id"`
	Content    string      `json:"content"`
	Author     ForumAuthor `json:"author"`
	IsSolution bool        `json:"isSolution"`
	CreatedAt  time.Time   `json:"createdAt"`
}

type CreateForumTopicRequest struct {
	Title      string `json:"title" example:"Как использовать GORM?"`
	Content    string `json:"content" example:"У меня вопрос по связям..."`
	CategoryID uint   `json:"categoryId" example:"1"`
}

type CreateForumCategoryRequest struct {
	Name        string `json:"name" example:"Вопросы по Go"`
	Description string `json:"description" example:"Обсуждение языка программирования Go"`
	Order       int    `json:"order" example:"1"`
}

type CreateForumCommentRequest struct {
	Content string `json:"content" example:"Вот решение вашей проблемы..."`
}
