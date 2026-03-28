import { api } from "./instance";

export interface ForumAuthor {
  id: number;
  fullName: string;
  avatar: string;
  stars: number;
  role: "admin" | "teacher" | "student" | "parent";
}

export interface ForumCategory {
  id: number;
  name: string;
  description: string;
  topicsCount: number;
}

export interface ForumTopic {
  id: number;
  title: string;
  content: string;
  author: ForumAuthor;
  categoryId: number;
  views: number;
  isSolved: boolean;
  createdAt: string;
  replies: number;
}

export interface ForumComment {
  id: number;
  content: string;
  author: ForumAuthor;
  isSolution: boolean;
  createdAt: string;
}

interface TopicDetail {
  topic: ForumTopic;
  comments: ForumComment[] | null;
}

interface CreateTopicBody {
  title: string;
  content: string;
  categoryId: number;
}

interface CreateCategoryBody {
  name: string;
  description: string;
  order: number;
}

export const getForumCategories = () => api<ForumCategory[] | null>("/forum/categories");

export const createForumCategory = (body: CreateCategoryBody) =>
  api<ForumCategory>("/forum/categories", { method: "POST", body });

export const getForumTopics = (categoryId?: number) =>
  api<ForumTopic[] | null>("/forum/topics", {
    query: categoryId ? { categoryId } : undefined,
  });

export const getForumTopic = (id: number) => api<TopicDetail>(`/forum/topics/${id}`);

export const createForumTopic = (body: CreateTopicBody) =>
  api<ForumTopic>("/forum/topics", { method: "POST", body });

export const createForumComment = (topicId: number, content: string) =>
  api<ForumComment>(`/forum/topics/${topicId}/comments`, {
    method: "POST",
    body: { content },
  });

export const markSolution = (commentId: number, topicId: number) =>
  api(`/forum/comments/${commentId}/solution`, {
    method: "PUT",
    query: { topicId },
  });
