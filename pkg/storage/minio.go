package storage

import (
	"context"
	"io"
	"log"
	"net/url"
	"time"

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

	// Генерируем URL (в данном случае предполагаем, что бакет публичный для чтения или возвращаем преподписанный URL)
	// Для простоты вернем путь, который можно сконструировать, если Minio настроен на публичный доступ
	// Или сгенерируем преподписанный URL на долгий срок (например, 7 дней)
	
	reqParams := make(url.Values)
	presignedURL, err := s.Client.PresignedGetObject(ctx, s.BucketName, fileName, time.Hour*24*7, reqParams)
	if err != nil {
		return "", err
	}

	return presignedURL.String(), nil
}
