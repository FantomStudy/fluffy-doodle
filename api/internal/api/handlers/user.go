package handlers

import (
	"errors"
	"fmt"
	"path/filepath"
	"strconv"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/service"
	"github.com/gofiber/fiber/v2"
	"gorm.io/gorm"
)

// @Summary Get user profile
// @Description Получение профиля текущего пользователя
// @Produce json
// @Tags user
// @Success 200 {object} presenter.UserProfileResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/profile [get]
func GetProfile(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)

		user, err := s.GetUserByID(userID)
		if err != nil {
			if err == gorm.ErrRecordNotFound {
				return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("user not found")))
			}
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		if user.Role.Name == "parent" {
			child, childErr := s.GetChildByParentID(user.ID)
			if childErr == gorm.ErrRecordNotFound {
				return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("child not linked to this parent")))
			}
			if childErr != nil {
				return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(childErr))
			}
			user = child
		}

		user, err = s.EnsureStudentInvitationCode(user)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.UserProfileResponse{
			ID:             user.ID,
			Login:          user.Login,
			FullName:       user.FullName,
			PhoneNumber:    user.PhoneNumber,
			Avatar:         user.Avatar,
			InvitationCode: user.InvitationCode,
			RoleID:         user.RoleID,
			Stars:          user.Stars,
			Exp:            user.Exp,
			Streak:         user.StreakDays,
			Level:          models.CalculateLevel(user.Exp),
			ExpToNextLevel: models.ExpToNextLevel(user.Exp),
			ActiveFrame:    presenter.MapFrame(user.ActiveFrame),
			Achievements:   presenter.MapAchievements(user.Achievements),
		})
	}
}

// @Summary Get current user
// @Description Returns current authenticated user with role name
// @Produce json
// @Tags user
// @Success 200 {object} presenter.MeResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /me [get]
func GetMe(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)

		user, err := s.GetUserByID(userID)
		if err != nil {
			if err == gorm.ErrRecordNotFound {
				return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("user not found")))
			}
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		user, err = s.EnsureStudentInvitationCode(user)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		ownedFrameIDs := make([]uint, 0, len(user.Frames))
		for _, f := range user.Frames {
			ownedFrameIDs = append(ownedFrameIDs, f.ID)
		}

		return c.Status(fiber.StatusOK).JSON(presenter.MeResponse{
			ID:             user.ID,
			Login:          user.Login,
			FullName:       user.FullName,
			PhoneNumber:    user.PhoneNumber,
			Avatar:         user.Avatar,
			InvitationCode: user.InvitationCode,
			RoleID:         user.RoleID,
			RoleName:       user.Role.Name,
			Stars:          user.Stars,
			Exp:            user.Exp,
			Streak:         user.StreakDays,
			Level:          models.CalculateLevel(user.Exp),
			ExpToNextLevel: models.ExpToNextLevel(user.Exp),
			ActiveFrame:    presenter.MapFrame(user.ActiveFrame),
			OwnedFrames:    ownedFrameIDs,
			Achievements:   presenter.MapAchievements(user.Achievements),
		})
	}
}

func CompleteGameLevel(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		levelID := c.Params("levelId")

		var req presenter.CompleteGameLevelRequest
		if err := c.BodyParser(&req); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("invalid request payload")))
		}
		if !req.Completed {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("completed must be true")))
		}

		result, err := s.CompleteGameLevel(c.Locals("userId").(uint), levelID)
		if err != nil {
			if err.Error() == "invalid game level id" {
				return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("levelId is required")))
			}
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.SuccessResponseWithData("game level completed", presenter.CompleteGameLevelResponse{
			LevelID:      result.LevelID,
			IsCompleted:  result.IsCompleted,
			WasCompleted: result.WasCompleted,
			AwardedStars: result.AwardedStars,
			AwardedExp:   result.AwardedExp,
			CurrentStars: result.CurrentStars,
			CurrentExp:   result.CurrentExp,
			CurrentLevel: result.CurrentLevel,
		}))
	}
}

// @Summary Upload avatar
// @Description Upload user avatar to S3
// @Accept multipart/form-data
// @Produce json
// @Tags user
// @Param avatar formData file true "Avatar file"
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/avatar [post]
func UploadAvatar(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)

		file, err := c.FormFile("avatar")
		if err != nil {
			fmt.Printf("UploadAvatar Error: %v\n", err)
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("file not found")))
		}

		src, err := file.Open()
		if err != nil {
			fmt.Printf("UploadAvatar File Open Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(errors.New("failed to open file")))
		}
		defer src.Close()

		filename := fmt.Sprintf("avatar_%d%s", userID, filepath.Ext(file.Filename))
		url, err := s.UploadAvatar(c.Context(), filename, src, file.Size, file.Header.Get("Content-Type"))
		if err != nil {
			fmt.Printf("UploadAvatar S3 Upload Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		user, err := s.GetUserByID(userID)
		if err != nil {
			fmt.Printf("UploadAvatar GetUser Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		user.Avatar = url
		if _, err = s.UpdateUser(user); err != nil {
			fmt.Printf("UploadAvatar UpdateUser Error: %v\n", err)
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Parent child progress
// @Description Returns current child progress for authenticated parent
// @Produce json
// @Tags user
// @Success 200 {object} presenter.ParentChildProgressResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 404 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/parent/child-progress [get]
func ParentChildProgress(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		parentID := c.Locals("userId").(uint)

		child, err := s.GetChildByParentID(parentID)
		if err == gorm.ErrRecordNotFound {
			return c.Status(fiber.StatusNotFound).JSON(presenter.AuthErrorResponse(errors.New("child not linked to this parent")))
		}
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.ParentChildProgressResponse{
			StudentID:      child.ID,
			StudentName:    child.FullName,
			StudentLogin:   child.Login,
			Stars:          child.Stars,
			Exp:            child.Exp,
			Level:          models.CalculateLevel(child.Exp),
			Achievements:   len(child.Achievements),
			InvitationCode: child.InvitationCode,
			ActiveFrame:    presenter.MapFrame(child.ActiveFrame),
			AchievementsList: presenter.MapAchievements(child.Achievements),
		})
	}
}

// @Summary Get stars leaderboard
// @Description Returns users sorted by stars
// @Produce json
// @Tags user
// @Param limit query int false "Limit"
// @Success 200 {array} presenter.LeaderboardResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/leaderboard/stars [get]
func GetLeaderboard(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		limit, _ := strconv.Atoi(c.Query("limit", "10"))
		users, err := s.GetLeaderboard(limit)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		response := make([]presenter.LeaderboardResponse, 0, len(users))
		for _, u := range users {
			response = append(response, presenter.LeaderboardResponse{
				ID:          u.ID,
				FullName:    u.FullName,
				Avatar:      u.Avatar,
				Stars:       u.Stars,
				Level:       models.CalculateLevel(u.Exp),
				ActiveFrame: presenter.MapFrame(u.ActiveFrame),
			})
		}

		return c.Status(fiber.StatusOK).JSON(response)
	}
}

// @Summary Get available frames
// @Description Returns all available frames and marks owned ones
// @Produce json
// @Tags user
// @Success 200 {array} presenter.FrameResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/frames [get]
func GetFrames(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)
		user, err := s.GetUserByID(userID)
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		frames, err := s.GetFrames()
		if err != nil {
			return c.Status(fiber.StatusInternalServerError).JSON(presenter.AuthErrorResponse(err))
		}

		ownedFrames := make(map[uint]bool)
		for _, f := range user.Frames {
			ownedFrames[f.ID] = true
		}

		response := make([]presenter.FrameResponse, 0, len(frames))
		for _, f := range frames {
			response = append(response, presenter.FrameResponse{
				ID:    f.ID,
				Name:  f.Name,
				Price: f.Price,
				Image: f.Image,
				Owned: ownedFrames[f.ID],
			})
		}

		return c.Status(fiber.StatusOK).JSON(response)
	}
}

// @Summary Buy a frame
// @Description Allows user to buy a frame using stars
// @Produce json
// @Tags user
// @Param frameId path int true "Frame ID"
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/frames/{frameId}/buy [post]
func BuyFrame(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)
		frameID, err := strconv.ParseUint(c.Params("frameId"), 10, 32)
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("invalid frame id")))
		}

		if err := s.BuyFrame(userID, uint(frameID)); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}

// @Summary Set active frame
// @Description Sets a frame as active for the user. Set frameId to 0 to remove frame.
// @Produce json
// @Tags user
// @Param frameId path int true "Frame ID"
// @Success 200 {object} presenter.AuthSwaggerSuccessResponse
// @Failure 400 {object} presenter.AuthSwaggerErrorResponse
// @Failure 401 {object} presenter.AuthSwaggerErrorResponse
// @Failure 500 {object} presenter.AuthSwaggerErrorResponse
// @Router /user/frames/{frameId}/active [post]
func SetActiveFrame(s service.UserService) fiber.Handler {
	return func(c *fiber.Ctx) error {
		userID := c.Locals("userId").(uint)
		frameID, err := strconv.ParseUint(c.Params("frameId"), 10, 32)
		if err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(errors.New("invalid frame id")))
		}

		if err := s.SetActiveFrame(userID, uint(frameID)); err != nil {
			return c.Status(fiber.StatusBadRequest).JSON(presenter.AuthErrorResponse(err))
		}

		return c.Status(fiber.StatusOK).JSON(presenter.AuthSuccessResponse())
	}
}
