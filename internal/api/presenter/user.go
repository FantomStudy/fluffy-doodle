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
	InvitationCode string `json:"studentCode,omitempty"`
}

type SignInRequest struct {
	Login    string `json:"login"`
	Password string `json:"password"`
}

type UserProfileResponse struct {
	ID             uint   `json:"id" example:"1"`
	Login          string `json:"login" example:"teacher01"`
	FullName       string `json:"fullName" example:"Ivan Ivanov"`
	PhoneNumber    string `json:"phoneNumber" example:"+79001234567"`
	Avatar         string `json:"avatar" example:"https://minio.local/avatar_1.png"`
	InvitationCode string `json:"studentCode,omitempty" example:"STU-8A7KQ21M"`
	RoleID         uint   `json:"roleId" example:"2"`
	Stars          int    `json:"stars" example:"10"`
	Exp            int    `json:"exp" example:"45"`
	Streak         int    `json:"streak" example:"4"`
	Level          int    `json:"level" example:"1"`
	ExpToNextLevel int    `json:"expToNextLevel" example:"55"`
}

type MeResponse struct {
	ID             uint   `json:"id" example:"1"`
	Login          string `json:"login" example:"teacher01"`
	FullName       string `json:"fullName" example:"Ivan Ivanov"`
	PhoneNumber    string `json:"phoneNumber" example:"+79001234567"`
	Avatar         string `json:"avatar" example:"https://minio.local/avatar_1.png"`
	InvitationCode string `json:"studentCode,omitempty" example:"STU-8A7KQ21M"`
	RoleID         uint   `json:"roleId" example:"2"`
	RoleName       string `json:"roleName" example:"student"`
	Stars          int    `json:"stars" example:"10"`
	Exp            int    `json:"exp" example:"45"`
	Streak         int    `json:"streak" example:"4"`
	Level          int    `json:"level" example:"1"`
	ExpToNextLevel int    `json:"expToNextLevel" example:"55"`
}

type ParentChildProgressResponse struct {
	StudentID      uint   `json:"studentId" example:"12"`
	StudentName    string `json:"studentName" example:"Petya Ivanov"`
	StudentLogin   string `json:"studentLogin" example:"petya_ivanov"`
	Stars          int    `json:"stars" example:"18"`
	Exp            int    `json:"exp" example:"45"`
	Level          int    `json:"level" example:"1"`
	Achievements   int    `json:"achievements" example:"4"`
	InvitationCode string `json:"studentCode" example:"STU-8A7KQ21M"`
}

type CompleteGameLevelRequest struct {
	Completed bool `json:"completed"`
}

type CompleteGameLevelResponse struct {
	LevelID      string `json:"levelId"`
	IsCompleted  bool   `json:"isCompleted"`
	WasCompleted bool   `json:"wasCompleted"`
	AwardedStars int    `json:"awardedStars"`
	AwardedExp   int    `json:"awardedExp"`
	CurrentStars int    `json:"currentStars"`
	CurrentExp   int    `json:"currentExp"`
	CurrentLevel int    `json:"currentLevel"`
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
