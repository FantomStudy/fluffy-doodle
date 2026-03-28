import { api } from "./instance";

interface Achievement {
  id: number;
  name: string;
  description: string;
  icon: string;
  createdAt: string;
  updatedAt: string;
}

interface Role {
  id: number;
  name: string;
  createdAt: string;
  updatedAt: string;
}

export interface User {
  id: number;
  fullName: string;
  login: string;
  phoneNumber: string;
  avatar: string;
  studentCode: string;
  stars: number;
  exp: number;
  level: number;
  expToNextLevel: number;
  streak: number;
  roleId: number;
  role: Role;
  achievements: Achievement[];
  createdAt: string;
  updatedAt: string;
}

export interface Me {
  id: number;
  login: string;
  fullName: string;
  phoneNumber: string;
  avatar: string;
  studentCode?: string;
  roleId: number;
  roleName: "admin" | "teacher" | "student" | "parent";
  stars: number;
  exp: number;
  level: number;
  expToNextLevel: number;
  streak: number;
}

interface InviteStudentRequest {
  fullName: string;
  login: string;
  password: string;
  phoneNumber: string;
}

interface ApiResponse {
  success: boolean;
  error: string | null;
}

export const getProfile = () => api<User>("/user/profile");

export const getMe = () => api<Me>("/me");

export const uploadAvatar = async (file: File) => {
  const formData = new FormData();
  formData.append("avatar", file);

  const baseURL = import.meta.env.VITE_API_URL || "http://localhost:3000";
  const res = await fetch(`${baseURL}/user/avatar`, {
    method: "POST",
    credentials: "include",
    body: formData,
  });

  if (!res.ok) {
    const data = await res.json().catch(() => null);
    throw new Error(data?.error || `Ошибка загрузки: ${res.status}`);
  }

  return res.json() as Promise<ApiResponse>;
};

export const inviteStudent = (body: InviteStudentRequest) =>
  api<ApiResponse>("/user/student-invitation", { method: "POST", body });

export interface ChildProgress {
  studentId: number;
  studentName: string;
  studentLogin: string;
  stars: number;
  exp: number;
  level: number;
  achievements: number;
  studentCode: string;
}

export const getChildProgress = () => api<ChildProgress>("/user/parent/child-progress");
