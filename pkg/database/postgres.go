package database

import (
	"fmt"
	"log"
	"os"
	"time"

	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/driver/postgres"
	"gorm.io/gorm"
	"gorm.io/gorm/logger"
)

func NewPostgresDB(cfg *config.Config) (*gorm.DB, error) {
	// Добавляем таймауты в DSN для предотвращения зависаний
	dsn := fmt.Sprintf("host=%s user=%s password=%s dbname=%s port=%s sslmode=disable connect_timeout=10", cfg.DbHost, cfg.DbUser, cfg.DbPassword, cfg.DbName, cfg.DbPort)

	// Настройка логирования
	newLogger := logger.New(
		log.New(os.Stdout, "\r\n", log.LstdFlags),
		logger.Config{
			LogLevel:                  logger.Error, // Только ошибки
			IgnoreRecordNotFoundError: true,         // Не логировать ошибки "record not found"
			Colorful:                  false,        // Отключить цветной вывод
		},
	)

	database, err := gorm.Open(postgres.Open(dsn), &gorm.Config{
		Logger: newLogger,
	})

	if err != nil {
		return nil, err
	}

	// Настройка connection pool для предотвращения таймаутов
	sqlDB, err := database.DB()
	if err != nil {
		return nil, err
	}

	// Настройки пула подключений
	sqlDB.SetMaxIdleConns(10)                  // Максимум неактивных подключений
	sqlDB.SetMaxOpenConns(100)                 // Максимум открытых подключений
	sqlDB.SetConnMaxLifetime(time.Hour)        // Максимальное время жизни подключения
	sqlDB.SetConnMaxIdleTime(10 * time.Minute) // Максимальное время простоя подключения

	return database, nil
}

func AutoMigrate(connection *gorm.DB) {
	connection.AutoMigrate(&models.User{}, &models.Role{})

	// createDefaultRoles(connection)
	// fixOldUsers(connection)

}

// func fixOldUsers(db *gorm.DB) {
// 	// Находим роль "user"
// 	var userRole models.Role
// 	if err := db.Where("name = ?", "teacher").First(&userRole).Error; err != nil {
// 		log.Println("Role 'user' not found, skipping old users fix")
// 		return
// 	}

// 	// Обновляем пользователей с role_id = 0 или NULL
// 	result := db.Model(&models.User{}).
// 		Where("role_id = ? OR role_id IS NULL", 0).
// 		Update("role_id", userRole.ID)

// 	if result.Error != nil {
// 		log.Printf("Failed to update old users: %v", result.Error)
// 	} else if result.RowsAffected > 0 {
// 		log.Printf("Fixed %d old users with missing roles", result.RowsAffected)
// 	}
// }

// func createDefaultRoles(db *gorm.DB) {
// 	roles := []models.Role{
// 		{Name: "admin"},
// 		{Name: "teacher"},
// 		{Name: "student"},
// 	}

// 	for _, role := range roles {
// 		var existingRole models.Role
// 		result := db.Where("name = ?", role.Name).First(&existingRole)

// 		if result.Error != nil {
// 			// Роль не найдена, создаём
// 			if err := db.Create(&role).Error; err != nil {
// 				log.Printf("Failed to create role %s: %v", role.Name, err)
// 			} else {
// 				log.Printf("Created role: %s", role.Name)
// 			}
// 		}
// 	}
// }
