package database

import (
	"fmt"
	"strings"

	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"golang.org/x/crypto/bcrypt"
	"gorm.io/gorm"
)

type lessonSeed struct {
	Title            string
	Description      string
	Order            int
	EstimatedMinutes int
	IsFreePreview    bool
	Tasks            []models.LessonTask
}

type courseSeed struct {
	Title       string
	Description string
	Level       string
	Hours       int
	Category    string
	Lessons     []lessonSeed
}

func SeedTestData(db *gorm.DB, courseImageURL string) error {
	if err := cleanupLegacySeedCourses(db); err != nil {
		return err
	}

	roles, err := seedRoles(db)
	if err != nil {
		return err
	}

	achievements, err := seedAchievements(db)
	if err != nil {
		return err
	}

	categories, err := seedCategories(db)
	if err != nil {
		return err
	}

	if _, err := seedFrames(db); err != nil {
		return err
	}

	users, err := seedUsers(db, roles)
	if err != nil {
		return err
	}

	if err := attachAchievements(db, users["student_demo"], []models.Achievement{
		achievements["Первые шаги"],
		achievements["Молодец"],
	}); err != nil {
		return err
	}

	if err := seedCourses(db, categories, courseImageURL); err != nil {
		return err
	}

	return nil
}

func cleanupLegacySeedCourses(db *gorm.DB) error {
	legacyTitles := []string{
		"Python Basics",
		"Junior Algorithms",
	}

	for _, title := range legacyTitles {
		var course models.Course
		err := db.Where("title = ?", title).First(&course).Error
		if err != nil {
			if err == gorm.ErrRecordNotFound {
				continue
			}
			return err
		}

		var lessons []models.Lesson
		if err := db.Where("course_id = ?", course.ID).Find(&lessons).Error; err != nil {
			return err
		}

		lessonIDs := make([]uint, 0, len(lessons))
		for _, lesson := range lessons {
			lessonIDs = append(lessonIDs, lesson.ID)
		}

		if len(lessonIDs) > 0 {
			if err := db.Where("lesson_id IN ?", lessonIDs).Delete(&models.LessonTask{}).Error; err != nil {
				return err
			}
			if err := db.Where("id IN ?", lessonIDs).Delete(&models.Lesson{}).Error; err != nil {
				return err
			}
		}

		if err := db.Delete(&course).Error; err != nil {
			return err
		}
	}

	return nil
}

func seedRoles(db *gorm.DB) (map[string]models.Role, error) {
	roleNames := []string{"admin", "teacher", "student", "parent"}
	result := make(map[string]models.Role, len(roleNames))

	for _, name := range roleNames {
		role := models.Role{Name: name}
		if err := db.Where("name = ?", name).FirstOrCreate(&role).Error; err != nil {
			return nil, err
		}
		result[name] = role
	}

	return result, nil
}

func seedAchievements(db *gorm.DB) (map[string]models.Achievement, error) {
	definitions := []models.Achievement{
		{Name: "Первые шаги", Description: "Выполнил первое задание", Icon: "boots"},
		{Name: "Молодец", Description: "Решил несколько простых тестов", Icon: "star"},
		{Name: "Схема готова", Description: "Справился с блок-схемой", Icon: "diagram"},
	}

	result := make(map[string]models.Achievement, len(definitions))
	for _, definition := range definitions {
		achievement := models.Achievement{}
		err := db.Where("name = ?", definition.Name).First(&achievement).Error
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return nil, err
			}

			achievement = definition
			if err := db.Create(&achievement).Error; err != nil {
				return nil, err
			}
		} else {
			achievement.Description = definition.Description
			achievement.Icon = definition.Icon
			if err := db.Save(&achievement).Error; err != nil {
				return nil, err
			}
		}

		result[achievement.Name] = achievement
	}

	return result, nil
}

func seedCategories(db *gorm.DB) (map[string]models.CourseCategory, error) {
	definitions := []models.CourseCategory{
		{Name: "Программирование"},
		{Name: "Математика"},
		{Name: "Логика"},
	}

	result := make(map[string]models.CourseCategory, len(definitions))
	for _, definition := range definitions {
		category := models.CourseCategory{}
		err := db.Where("name = ?", definition.Name).First(&category).Error
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return nil, err
			}

			category = definition
			if err := db.Create(&category).Error; err != nil {
				return nil, err
			}
		}

		result[definition.Name] = category
	}

	return result, nil
}

func seedFrames(db *gorm.DB) (map[string]models.Frame, error) {
	definitions := []models.Frame{
		{Name: "Обычная рамка", Price: 0, Image: "frame_common.png"},
		{Name: "Бронзовая рамка", Price: 50, Image: "frame_bronze.png"},
		{Name: "Серебряная рамка", Price: 150, Image: "frame_silver.png"},
		{Name: "Золотая рамка", Price: 500, Image: "frame_gold.png"},
	}

	result := make(map[string]models.Frame, len(definitions))
	for _, definition := range definitions {
		frame := models.Frame{}
		err := db.Where("name = ?", definition.Name).First(&frame).Error
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return nil, err
			}

			frame = definition
			if err := db.Create(&frame).Error; err != nil {
				return nil, err
			}
		} else {
			frame.Price = definition.Price
			frame.Image = definition.Image
			if err := db.Save(&frame).Error; err != nil {
				return nil, err
			}
		}

		result[frame.Name] = frame
	}

	return result, nil
}

func seedUsers(db *gorm.DB, roles map[string]models.Role) (map[string]*models.User, error) {
	passwordHash, err := bcrypt.GenerateFromPassword([]byte("Password123!"), bcrypt.DefaultCost)
	if err != nil {
		return nil, err
	}

	type userSeed struct {
		Login          string
		FullName       string
		PhoneNumber    string
		RoleName       string
		InvitationCode string
		Stars          int
		Exp            int
	}

	seeds := []userSeed{
		{Login: "admin_demo", FullName: "Admin Demo", PhoneNumber: "+79990000001", RoleName: "admin", InvitationCode: "ADM-DEMO01", Stars: 120, Exp: 450},
		{Login: "teacher_demo", FullName: "Teacher Demo", PhoneNumber: "+79990000002", RoleName: "teacher", InvitationCode: "TCH-DEMO01", Stars: 80, Exp: 320},
		{Login: "student_demo", FullName: "Student Demo", PhoneNumber: "+79990000003", RoleName: "student", InvitationCode: "STU-DEMO01", Stars: 25, Exp: 75},
		{Login: "student_anna", FullName: "Anna Student", PhoneNumber: "+79990000004", RoleName: "student", InvitationCode: "STU-DEMO02", Stars: 40, Exp: 120},
		{Login: "parent_demo", FullName: "Parent Demo", PhoneNumber: "+79990000005", RoleName: "parent", InvitationCode: "PAR-DEMO01", Stars: 10, Exp: 15},
	}

	result := make(map[string]*models.User, len(seeds))
	for _, seed := range seeds {
		user := models.User{}
		err := db.Where("login = ?", seed.Login).First(&user).Error
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return nil, err
			}

			user = models.User{Login: seed.Login}
		}

		user.FullName = seed.FullName
		user.PhoneNumber = seed.PhoneNumber
		user.Password = string(passwordHash)
		user.RoleID = roles[seed.RoleName].ID
		user.Stars = seed.Stars
		user.Exp = seed.Exp
		user.InvitationCode = seed.InvitationCode

		if user.ID == 0 {
			if err := db.Create(&user).Error; err != nil {
				return nil, err
			}
		} else {
			if err := db.Save(&user).Error; err != nil {
				return nil, err
			}
		}

		result[seed.Login] = &user
	}

	parent := result["parent_demo"]
	student := result["student_demo"]
	if err := db.Model(&models.User{}).Where("id = ?", student.ID).Update("parent_id", parent.ID).Error; err != nil {
		return nil, err
	}
	student.ParentID = &parent.ID

	return result, nil
}

func attachAchievements(db *gorm.DB, user *models.User, achievements []models.Achievement) error {
	return db.Model(user).Association("Achievements").Replace(achievements)
}

func seedCourses(db *gorm.DB, categories map[string]models.CourseCategory, courseImageURL string) error {
	courses := []courseSeed{
		{
			Title:       "Питон для детей",
			Description: "Очень лёгкий курс про переменные, ввод и if.",
			Level:       models.CourseLevelBeginner,
			Hours:       8,
			Category:    "Программирование",
			Lessons: []lessonSeed{
				{
					Title:            "Переменные",
					Description:      "Самый простой урок про переменные и ввод.",
					Order:            1,
					EstimatedMinutes: 10,
					IsFreePreview:    true,
					Tasks: []models.LessonTask{
						newQuizTask(
							"Тест про переменные",
							"Очень лёгкий тест.",
							"Что верно?",
							[]models.TaskOption{
								{ID: "a", Text: "Переменная хранит значение."},
								{ID: "b", Text: "В имени переменной можно ставить пробел."},
								{ID: "c", Text: "Ввод можно сохранить в переменную."},
							},
							[]string{"a", "c"},
						),
						newStatusTask(models.TaskTypeFlowchart, "Блок-схема про ввод", "Построй простую блок-схему для ввода числа."),
						newStatusTask(models.TaskTypeAlgorithm, "Задача про привет", "Напиши алгоритм, который выводит привет."),
					},
				},
				{
					Title:            "Условие if",
					Description:      "Очень простой урок про if.",
					Order:            2,
					EstimatedMinutes: 10,
					IsFreePreview:    false,
					Tasks: []models.LessonTask{
						newQuizTask(
							"Тест про if",
							"Выбери правильные ответы.",
							"Когда нужен if?",
							[]models.TaskOption{
								{ID: "a", Text: "Когда программе нужно выбрать действие."},
								{ID: "b", Text: "Когда нужно просто запомнить число."},
								{ID: "c", Text: "Когда ответ зависит от условия."},
							},
							[]string{"a", "c"},
						),
					},
				},
			},
		},
		{
			Title:       "Простые алгоритмы",
			Description: "Очень лёгкий курс про шаги и порядок действий.",
			Level:       models.CourseLevelBeginner,
			Hours:       6,
			Category:    "Логика",
			Lessons: []lessonSeed{
				{
					Title:            "Шаги решения",
					Description:      "Учимся решать задачу по шагам.",
					Order:            1,
					EstimatedMinutes: 10,
					IsFreePreview:    true,
					Tasks: []models.LessonTask{
						newQuizTask(
							"Тест про алгоритм",
							"Самый простой тест.",
							"Что важно в алгоритме?",
							[]models.TaskOption{
								{ID: "a", Text: "Понятные шаги."},
								{ID: "b", Text: "Случайный порядок шагов."},
								{ID: "c", Text: "У алгоритма есть конец."},
							},
							[]string{"a", "c"},
						),
						newStatusTask(models.TaskTypeAlgorithm, "Задача про большее число", "Найди большее из двух чисел."),
					},
				},
			},
		},
	}

	for _, courseSeed := range courses {
		category := categories[courseSeed.Category]
		course := models.Course{}
		err := db.Where("title = ?", courseSeed.Title).First(&course).Error
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return err
			}

			course = models.Course{Title: courseSeed.Title}
		}

		course.Description = courseSeed.Description
		course.Level = courseSeed.Level
		course.Hours = courseSeed.Hours
		course.CategoryID = category.ID
		course.ImageURL = courseImageURL
		course.Published = true
		course.TotalLessons = len(courseSeed.Lessons)

		if course.ID == 0 {
			if err := db.Create(&course).Error; err != nil {
				return err
			}
		} else {
			if err := db.Save(&course).Error; err != nil {
				return err
			}
		}

		for _, lesson := range courseSeed.Lessons {
			if err := upsertLesson(db, course, lesson); err != nil {
				return err
			}
		}
	}

	return nil
}

func upsertLesson(db *gorm.DB, course models.Course, seed lessonSeed) error {
	lesson := models.Lesson{}
	err := db.Where("course_id = ? AND \"order\" = ?", course.ID, seed.Order).First(&lesson).Error
	if err != nil {
		if err != gorm.ErrRecordNotFound {
			return err
		}

		lesson = models.Lesson{CourseID: course.ID, Order: seed.Order}
	}

	lesson.Title = seed.Title
	lesson.Description = seed.Description
	lesson.EstimatedMinutes = seed.EstimatedMinutes
	lesson.IsFreePreview = seed.IsFreePreview

	if lesson.ID == 0 {
		if err := db.Create(&lesson).Error; err != nil {
			return err
		}
	} else {
		if err := db.Save(&lesson).Error; err != nil {
			return err
		}
	}

	for _, taskSeed := range seed.Tasks {
		if err := upsertTask(db, lesson, taskSeed); err != nil {
			return err
		}
	}

	return nil
}

func upsertTask(db *gorm.DB, lesson models.Lesson, seed models.LessonTask) error {
	task := models.LessonTask{}
	err := db.Where("lesson_id = ? AND title = ?", lesson.ID, seed.Title).First(&task).Error
	if err != nil {
		if err != gorm.ErrRecordNotFound {
			return err
		}

		task = models.LessonTask{LessonID: lesson.ID, Title: seed.Title}
	}

	task.Description = seed.Description
	task.Type = strings.TrimSpace(seed.Type)
	task.Question = seed.Question
	task.Options = seed.Options
	task.CorrectOptionIDs = seed.CorrectOptionIDs
	task.RewardStars = models.TaskRewardStars
	task.RewardExp = models.TaskRewardExp

	if task.ID == 0 {
		return db.Create(&task).Error
	}

	return db.Save(&task).Error
}

func newQuizTask(title, description, question string, options []models.TaskOption, correct []string) models.LessonTask {
	return models.LessonTask{
		Title:            title,
		Description:      description,
		Type:             models.TaskTypeQuiz,
		Question:         question,
		Options:          options,
		CorrectOptionIDs: correct,
		RewardStars:      models.TaskRewardStars,
		RewardExp:        models.TaskRewardExp,
	}
}

func newStatusTask(taskType, title, description string) models.LessonTask {
	return models.LessonTask{
		Title:       title,
		Description: description,
		Type:        taskType,
		Question:    fmt.Sprintf("Отметь как решённое, когда %s", strings.ToLower(description)),
		RewardStars: models.TaskRewardStars,
		RewardExp:   models.TaskRewardExp,
	}
}
