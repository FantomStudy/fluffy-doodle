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
  stars: number;
  exp: number;
  level: number;
  expToNextLevel: number;
  roleId: number;
  role: Role;
  achievements: Achievement[];
  createdAt: string;
  updatedAt: string;
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
