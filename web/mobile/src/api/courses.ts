import { api } from "./instance";

export interface Category {
  id: number;
  name: string;
  createdAt: string;
  updatedAt: string;
}

export interface Course {
  id: number;
  title: string;
  description: string;
  imageUrl: string;
  level: string;
  hours: number;
  categoryId: number;
  categoryName: string;
  totalLessons: number;
  createdAt: string;
  updatedAt: string;
}

interface TaskOption {
  id: string;
  text: string;
}

export interface Task {
  id: number;
  lessonId: number;
  type: "quiz" | "flowchart" | "algorithm";
  title: string;
  description: string;
  question: string;
  options: TaskOption[] | null;
  rewardStars: number;
  rewardExp: number;
}

export interface Lesson {
  id: number;
  courseId: number;
  title: string;
  description: string;
  order: number;
  estimatedMinutes: number;
  isFreePreview: boolean;
  tasks: Task[];
}

interface ApiWrapped<T> {
  success: boolean;
  message: string;
  data: T;
}

export const getCategories = () => api<ApiWrapped<Category[]>>("/categories").then((r) => r.data);

export const getCourses = () => api<ApiWrapped<Course[]>>("/courses").then((r) => r.data);

export const getCourse = (id: number) =>
  api<ApiWrapped<Course>>(`/courses/${id}`).then((r) => r.data);

export const getCourseLessons = (courseId: number) =>
  api<ApiWrapped<Lesson[]>>(`/courses/${courseId}/lessons`).then((r) => r.data);

interface QuizSubmit {
  selectedOptionIds: string[];
}

interface SolvedSubmit {
  solved: boolean;
}

export const submitTask = (lessonId: number, taskId: number, body: QuizSubmit | SolvedSubmit) =>
  api(`/lessons/${lessonId}/tasks/${taskId}/submit`, {
    method: "POST",
    body,
  });
