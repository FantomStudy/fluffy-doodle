package repository

import (
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"gorm.io/gorm"
)

type ForumRepository interface {
	GetCategories() ([]models.ForumCategory, error)
	GetCategoryByID(id uint) (*models.ForumCategory, error)
	CreateCategory(category *models.ForumCategory) (*models.ForumCategory, error)
	CreateTopic(topic *models.ForumTopic) (*models.ForumTopic, error)
	GetTopics(categoryID uint) ([]models.ForumTopic, error)
	GetTopicByID(id uint) (*models.ForumTopic, error)
	IncrementTopicViews(id uint) error
	CreateComment(comment *models.ForumComment) (*models.ForumComment, error)
	GetCommentByID(id uint) (*models.ForumComment, error)
	MarkCommentAsSolution(topicID uint, commentID uint) error
}

type forumRepository struct {
	db *gorm.DB
}

func NewForumRepository(db *gorm.DB) ForumRepository {
	return &forumRepository{db: db}
}

func (r *forumRepository) GetCategories() ([]models.ForumCategory, error) {
	var categories []models.ForumCategory
	err := r.db.Preload("Topics").Order("\"order\" ASC").Find(&categories).Error
	return categories, err
}

func (r *forumRepository) GetCategoryByID(id uint) (*models.ForumCategory, error) {
	var category models.ForumCategory
	err := r.db.First(&category, id).Error
	return &category, err
}

func (r *forumRepository) CreateCategory(category *models.ForumCategory) (*models.ForumCategory, error) {
	err := r.db.Create(category).Error
	return category, err
}

func (r *forumRepository) CreateTopic(topic *models.ForumTopic) (*models.ForumTopic, error) {
	err := r.db.Create(topic).Error
	return topic, err
}

func (r *forumRepository) GetTopics(categoryID uint) ([]models.ForumTopic, error) {
	var topics []models.ForumTopic
	query := r.db.Preload("Author").Preload("Author.Role").Preload("Comments").Order("created_at DESC")
	if categoryID > 0 {
		query = query.Where("category_id = ?", categoryID)
	}
	err := query.Find(&topics).Error
	return topics, err
}

func (r *forumRepository) GetTopicByID(id uint) (*models.ForumTopic, error) {
	var topic models.ForumTopic
	err := r.db.Preload("Author").Preload("Author.Role").
		Preload("Comments", func(db *gorm.DB) *gorm.DB {
			return db.Order("created_at ASC")
		}).
		Preload("Comments.Author").Preload("Comments.Author.Role").
		First(&topic, id).Error
	return &topic, err
}

func (r *forumRepository) IncrementTopicViews(id uint) error {
	return r.db.Model(&models.ForumTopic{}).Where("id = ?", id).UpdateColumn("views", gorm.Expr("views + ?", 1)).Error
}

func (r *forumRepository) CreateComment(comment *models.ForumComment) (*models.ForumComment, error) {
	err := r.db.Create(comment).Error
	return comment, err
}

func (r *forumRepository) GetCommentByID(id uint) (*models.ForumComment, error) {
	var comment models.ForumComment
	err := r.db.First(&comment, id).Error
	return &comment, err
}

func (r *forumRepository) MarkCommentAsSolution(topicID uint, commentID uint) error {
	tx := r.db.Begin()
	// Reset all comments in topic
	if err := tx.Model(&models.ForumComment{}).Where("topic_id = ?", topicID).Update("is_solution", false).Error; err != nil {
		tx.Rollback()
		return err
	}
	// Mark the specific comment
	if err := tx.Model(&models.ForumComment{}).Where("id = ?", commentID).Update("is_solution", true).Error; err != nil {
		tx.Rollback()
		return err
	}
	// Mark topic as solved
	if err := tx.Model(&models.ForumTopic{}).Where("id = ?", topicID).Update("is_solved", true).Error; err != nil {
		tx.Rollback()
		return err
	}
	return tx.Commit().Error
}
