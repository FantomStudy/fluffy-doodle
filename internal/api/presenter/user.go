package presenter

import "github.com/gofiber/fiber/v2"

// Swagger success response
type AuthSwaggerSuccessResponse struct {
	Status bool `json:"status"`
	Error  any  `json:"error"`
}

// Swagger error response
type AuthSwaggerErrorResponse struct {
	Status bool  `json:"status"`
	Error  error `json:"error"`
}

type SignUpRequest struct {
	Login          string `json:"login"`
	Password       string `json:"password"`
	FullName       string `json:"fullName"`
	PhoneNumber    string `json:"phoneNumber"`
	InvitationCode string `json:"-"`
}

type SignInRequest struct {
	Login    string `json:"login"`
	Password string `json:"password"`
}

type UserProfileResponse struct {
	ID          uint   `json:"id" example:"1"`
	Login       string `json:"login" example:"teacher01"`
	FullName    string `json:"fullName" example:"Иван Иванов"`
	PhoneNumber string `json:"phoneNumber" example:"+79001234567"`
	Avatar      string `json:"avatar" example:"https://minio.local/avatar_1.png"`
	RoleID      uint   `json:"roleId" example:"2"`
}

type ParentSignUpRequest struct {
	Login       string `json:"login" example:"parent01"`
	Password    string `json:"password" example:"superSecret123"`
	FullName    string `json:"fullName" example:"Мария Иванова"`
	PhoneNumber string `json:"phoneNumber" example:"+79001234567"`
	StudentCode string `json:"studentCode" example:"STU-8A7KQ21M"`
}

type StudentInviteResponse struct {
	StudentCode string `json:"studentCode" example:"STU-8A7KQ21M"`
}

type ParentChildProgressResponse struct {
	StudentID      uint   `json:"studentId" example:"12"`
	StudentName    string `json:"studentName" example:"Петя Иванов"`
	StudentLogin   string `json:"studentLogin" example:"petya_ivanov"`
	Stars          int    `json:"stars" example:"18"`
	Achievements   int    `json:"achievements" example:"4"`
	InvitationCode string `json:"studentCode" example:"STU-8A7KQ21M"`
}

func AuthErrorResponse(err error) *fiber.Map {
	return &fiber.Map{
		"success": false,
		"error":   err.Error(),
	}
}

func AuthSuccessResponse() *fiber.Map {
	return &fiber.Map{
		"success": true,
		"error":   nil,
	}
}
