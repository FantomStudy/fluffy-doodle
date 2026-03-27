package config

import (
	"os"

	"github.com/joho/godotenv"
)

type Config struct {
	Port          string
	DbHost        string
	DbPort        string
	DbUser        string
	DbPassword    string
	DbName        string
	Cors          string
	JwtSecret     string
	RefreshSecret string
}

func Load() Config {
	godotenv.Load("../../.env")

	return Config{
		Port:          LoadEnv("PORT", "3000"),
		DbHost:        LoadEnv("DB_HOST", "localhost"),
		DbPort:        LoadEnv("DB_PORT", "5432"),
		DbUser:        LoadEnv("DB_USER", "postgres"),
		DbPassword:    LoadEnv("DB_PASSWORD", "postgres"),
		DbName:        LoadEnv("DB_NAME", "fluffyDoodle"),
		Cors:          LoadEnv("CORS", "http://localhost:5173"),
		JwtSecret:     LoadEnv("JWT_SECRET", ""),
		RefreshSecret: LoadEnv("REFRESH_SECRET", ""),
	}
}

func LoadEnv(key string, replacement string) string {
	res := os.Getenv(key)

	if res == "" {
		return replacement
	}

	return res
}
