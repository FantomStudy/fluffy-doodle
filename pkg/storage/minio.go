package storage

import (
	"context"
	"io"
	"log"

	"github.com/FantomStudy/fluffy-doodle/internal/config"
	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
)

type MinioStorage struct {
	Client     *minio.Client
	BucketName string
	Endpoint   string
}

func NewMinioStorage(cfg *config.Config) (*MinioStorage, error) {
	useSSL := cfg.MinioUseSSL == "true"
	minioClient, err := minio.New(cfg.MinioEndpoint, &minio.Options{
		Creds:  credentials.NewStaticV4(cfg.MinioAccessKey, cfg.MinioSecretKey, ""),
		Secure: useSSL,
	})
	if err != nil {
		return nil, err
	}

	// Проверка/создание бакета
	ctx := context.Background()
	exists, err := minioClient.BucketExists(ctx, cfg.MinioBucket)
	if err != nil {
		return nil, err
	}
	if !exists {
		err = minioClient.MakeBucket(ctx, cfg.MinioBucket, minio.MakeBucketOptions{})
		if err != nil {
			return nil, err
		}
		log.Printf("Bucket %s created", cfg.MinioBucket)
	}

	// Установка политики публичного доступа для чтения (ReadOnly)
	policy := `{
		"Version": "2012-10-17",
		"Statement": [
			{
				"Effect": "Allow",
				"Principal": {"AWS": ["*"]},
				"Action": ["s3:GetObject"],
				"Resource": ["arn:aws:s3:::` + cfg.MinioBucket + `/*"]
			}
		]
	}`
	err = minioClient.SetBucketPolicy(ctx, cfg.MinioBucket, policy)
	if err != nil {
		log.Printf("Error setting bucket policy: %v", err)
	}

	return &MinioStorage{
		Client:     minioClient,
		BucketName: cfg.MinioBucket,
		Endpoint:   cfg.MinioEndpoint,
	}, nil
}

func (s *MinioStorage) UploadFile(ctx context.Context, fileName string, reader io.Reader, fileSize int64, contentType string) (string, error) {
	_, err := s.Client.PutObject(ctx, s.BucketName, fileName, reader, fileSize, minio.PutObjectOptions{
		ContentType: contentType,
	})
	if err != nil {
		return "", err
	}

	// Возвращаем постоянный URL
	protocol := "http://"
	// Если используем SSL, то https
	// Но для простоты берем из эндпоинта. Если в эндпоинте нет протокола, добавляем http.
	url := protocol + s.Endpoint + "/" + s.BucketName + "/" + fileName

	return url, nil
}
