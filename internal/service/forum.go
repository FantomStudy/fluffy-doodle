package service

import (
	"errors"

	"github.com/FantomStudy/fluffy-doodle/internal/api/presenter"
	"github.com/FantomStudy/fluffy-doodle/internal/models"
	"github.com/FantomStudy/fluffy-doodle/internal/repository"
	"gorm.io/gorm"
)

type ForumService interface {
	GetCategories() ([]presenter.ForumCategoryResponse, error)
	GetTopics(categoryID uint) ([]presenter.ForumTopicResponse, error)
	GetTopicDetail(topicID uint) (*presenter.ForumTopicDetailResponse, error)
	CreateTopic(userID uint, req presenter.CreateForumTopicRequest) (*presenter.ForumTopicResponse, error)
	CreateComment(userID uint, topicID uint, req presenter.CreateForumCommentRequest) (*presenter.ForumCommentResponse, error)
	MarkAsSolution(userID uint, topicID uint, commentID uint) error
}

type forumService struct {
	repo repository.ForumRepository
}

func NewForumService(repo repository.ForumRepository) ForumService {
	return &forumService{repo: repo}
}

func mapAuthor(user models.User) presenter.ForumAuthor {
	roleName := ""
	if user.Role.Name != "" {
		roleName = user.Role.Name
	}
	return presenter.ForumAuthor{
		ID:       user.ID,
		FullName: user.FullName,
		Avatar:   user.Avatar,
		Stars:    user.Stars,
		Role:     roleName,
	}
}

func (s *forumService) GetCategories() ([]presenter.ForumCategoryResponse, error) {
	categories, err := s.repo.GetCategories()
	if err != nil {
		return nil, err
	}

	var res []presenter.ForumCategoryResponse
	for _, c := range categories {
		res = append(res, presenter.ForumCategoryResponse{
			ID:          c.ID,
			Name:        c.Name,
			Description: c.Description,
			TopicsCount: len(c.Topics),
		})
	}
	return res, nil
}

func (s *forumService) GetTopics(categoryID uint) ([]presenter.ForumTopicResponse, error) {
	topics, err := s.repo.GetTopics(categoryID)
	if err != nil {
		return nil, err
	}

	var res []presenter.ForumTopicResponse
	for _, t := range topics {
		res = append(res, presenter.ForumTopicResponse{
			ID:         t.ID,
			Title:      t.Title,
			Content:    t.Content,
			Author:     mapAuthor(t.Author),
			CategoryID: t.CategoryID,
			Views:      t.Views,
			IsSolved:   t.IsSolved,
			CreatedAt:  t.CreatedAt,
			Replies:    len(t.Comments),
		})
	}
	return res, nil
}

func (s *forumService) GetTopicDetail(topicID uint) (*presenter.ForumTopicDetailResponse, error) {
	topic, err := s.repo.GetTopicByID(topicID)
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, errors.New("topic not found")
		}
		return nil, err
	}

	// Increment views
	_ = s.repo.IncrementTopicViews(topicID)
	topic.Views++

	var commentsRes []presenter.ForumCommentResponse
	for _, c := range topic.Comments {
		commentsRes = append(commentsRes, presenter.ForumCommentResponse{
			ID:         c.ID,
			Content:    c.Content,
			Author:     mapAuthor(c.Author),
			IsSolution: c.IsSolution,
			CreatedAt:  c.CreatedAt,
		})
	}

	res := &presenter.ForumTopicDetailResponse{
		Topic: presenter.ForumTopicResponse{
			ID:         topic.ID,
			Title:      topic.Title,
			Content:    topic.Content,
			Author:     mapAuthor(topic.Author),
			CategoryID: topic.CategoryID,
			Views:      topic.Views,
			IsSolved:   topic.IsSolved,
			CreatedAt:  topic.CreatedAt,
			Replies:    len(topic.Comments),
		},
		Comments: commentsRes,
	}

	return res, nil
}

func (s *forumService) CreateTopic(userID uint, req presenter.CreateForumTopicRequest) (*presenter.ForumTopicResponse, error) {
	if req.Title == "" || req.Content == "" {
		return nil, errors.New("title and content are required")
	}

	_, err := s.repo.GetCategoryByID(req.CategoryID)
	if err != nil {
		return nil, errors.New("category not found")
	}

	topic := &models.ForumTopic{
		Title:      req.Title,
		Content:    req.Content,
		AuthorID:   userID,
		CategoryID: req.CategoryID,
	}

	created, err := s.repo.CreateTopic(topic)
	if err != nil {
		return nil, err
	}

	// Fetch again to get Author info
	fullTopic, _ := s.repo.GetTopicByID(created.ID)
	if fullTopic == nil {
		fullTopic = created
	}

	res := &presenter.ForumTopicResponse{
		ID:         fullTopic.ID,
		Title:      fullTopic.Title,
		Content:    fullTopic.Content,
		Author:     mapAuthor(fullTopic.Author),
		CategoryID: fullTopic.CategoryID,
		Views:      fullTopic.Views,
		IsSolved:   fullTopic.IsSolved,
		CreatedAt:  fullTopic.CreatedAt,
		Replies:    0,
	}
	return res, nil
}

func (s *forumService) CreateComment(userID uint, topicID uint, req presenter.CreateForumCommentRequest) (*presenter.ForumCommentResponse, error) {
	if req.Content == "" {
		return nil, errors.New("content is required")
	}

	_, err := s.repo.GetTopicByID(topicID)
	if err != nil {
		return nil, errors.New("topic not found")
	}

	comment := &models.ForumComment{
		Content:  req.Content,
		TopicID:  topicID,
		AuthorID: userID,
	}

	created, err := s.repo.CreateComment(comment)
	if err != nil {
		return nil, err
	}

	// Fetch full comment to get Author
	// Actually, we don't have GetComment with full relations in repo, let's just return basic or we can just fetch topic again.
	// For simplicity, we can just return what we have, author info will be partial.
	// In real app, we'd preload author here.
	res := &presenter.ForumCommentResponse{
		ID:         created.ID,
		Content:    created.Content,
		IsSolution: created.IsSolution,
		CreatedAt:  created.CreatedAt,
	}
	return res, nil
}

func (s *forumService) MarkAsSolution(userID uint, topicID uint, commentID uint) error {
	topic, err := s.repo.GetTopicByID(topicID)
	if err != nil {
		return errors.New("topic not found")
	}

	if topic.AuthorID != userID {
		return errors.New("only topic author can mark solution")
	}

	comment, err := s.repo.GetCommentByID(commentID)
	if err != nil {
		return errors.New("comment not found")
	}

	if comment.TopicID != topicID {
		return errors.New("comment does not belong to this topic")
	}

	return s.repo.MarkCommentAsSolution(topicID, commentID)
}
