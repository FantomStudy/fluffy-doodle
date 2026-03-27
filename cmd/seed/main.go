package main

import (
	"context"
	"fmt"
	"log"
	"mime"
	"os"
	"path/filepath"
	"strings"

	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/FantomStudy/fluffy-doodle/pkg/database"
	"github.com/FantomStudy/fluffy-doodle/pkg/storage"
)

func main() {
	cfg := config.Load()

	db, err := database.NewPostgresDB(&cfg)
	if err != nil {
		log.Fatalf("database connection error: %v", err)
	}

	database.AutoMigrate(db)

	courseImageURL, err := uploadCourseImage(&cfg)
	if err != nil {
		log.Fatalf("failed to upload course image: %v", err)
	}

	if err := database.SeedTestData(db, courseImageURL); err != nil {
		log.Fatalf("failed to seed test data: %v", err)
	}

	log.Println("test data seeded successfully")
}

func uploadCourseImage(cfg *config.Config) (string, error) {
	imagePath, err := findRootImage()
	if err != nil {
		return "", err
	}

	file, err := os.Open(imagePath)
	if err != nil {
		return "", err
	}
	defer file.Close()

	info, err := file.Stat()
	if err != nil {
		return "", err
	}

	minioStorage, err := storage.NewMinioStorage(cfg)
	if err != nil {
		return "", err
	}

	fileName := fmt.Sprintf("seed/course-image%s", filepath.Ext(imagePath))
	contentType := mime.TypeByExtension(strings.ToLower(filepath.Ext(imagePath)))
	if contentType == "" {
		contentType = "application/octet-stream"
	}

	return minioStorage.UploadFile(context.Background(), fileName, file, info.Size(), contentType)
}

func findRootImage() (string, error) {
	entries, err := os.ReadDir(".")
	if err != nil {
		return "", err
	}

	for _, entry := range entries {
		if entry.IsDir() {
			continue
		}

		ext := strings.ToLower(filepath.Ext(entry.Name()))
		switch ext {
		case ".png", ".jpg", ".jpeg", ".webp":
			return filepath.Join(".", entry.Name()), nil
		}
	}

	return "", fmt.Errorf("image file not found in project root")
}
