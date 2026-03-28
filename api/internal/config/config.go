package config

import (
	"os"
	"path/filepath"

	"github.com/joho/godotenv"
)

type Config struct {
	Port           string
	DbHost         string
	DbPort         string
	DbUser         string
	DbPassword     string
	DbName         string
	Cors           string
	JwtSecret      string
	RefreshSecret  string
	MinioEndpoint  string
	MinioAccessKey string
	MinioSecretKey string
	MinioBucket    string
	MinioUseSSL    string
}

func Load() Config {
	loadDotEnv()

	return Config{
		Port:           LoadEnv("PORT", "3000"),
		DbHost:         LoadEnv("DB_HOST", "127.0.0.1"),
		DbPort:         LoadEnv("DB_PORT", "5432"),
		DbUser:         LoadEnv("DB_USER", "postgres"),
		DbPassword:     LoadEnv("DB_PASSWORD", "postgres"),
		DbName:         LoadEnv("DB_NAME", "fluffyDoodle"),
		Cors:           LoadEnv("CORS", "http://localhost:5173"),
		JwtSecret:      LoadEnv("JWT_SECRET", ""),
		RefreshSecret:  LoadEnv("REFRESH_SECRET", ""),
		MinioEndpoint:  LoadEnv("MINIO_ENDPOINT", "localhost:9000"),
		MinioAccessKey: LoadEnv("MINIO_ACCESS_KEY", "minioadmin"),
		MinioSecretKey: LoadEnv("MINIO_SECRET_KEY", "minioadmin"),
		MinioBucket:    LoadEnv("MINIO_BUCKET", "avatars"),
		MinioUseSSL:    LoadEnv("MINIO_USE_SSL", "false"),
	}
}

func loadDotEnv() {
	candidates := []string{".env", "../.env", "../../.env", "../../../.env", "../../../../.env"}
	for _, candidate := range candidates {
		if _, err := os.Stat(candidate); err == nil {
			_ = godotenv.Load(candidate)
			return
		}
	}

	wd, err := os.Getwd()
	if err != nil {
		return
	}

	dir := wd
	for i := 0; i < 8; i++ {
		path := filepath.Join(dir, ".env")
		if _, err = os.Stat(path); err == nil {
			_ = godotenv.Load(path)
			return
		}

		parent := filepath.Dir(dir)
		if parent == dir {
			return
		}
		dir = parent
	}
}

func LoadEnv(key string, replacement string) string {
	res := os.Getenv(key)

	if res == "" {
		return replacement
	}

	return res
}
