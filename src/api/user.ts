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

interface AuthResponse {
  status: boolean;
  error?: unknown;
}

export const getProfile = () => api<User>("/user/profile");

export const uploadAvatar = (file: File) => {
  const formData = new FormData();
  formData.append("avatar", file);

  return api<AuthResponse>("/user/avatar", { method: "POST", body: formData });
};

export const inviteStudent = (body: InviteStudentRequest) =>
  api<AuthResponse>("/user/student-invitation", { method: "POST", body });
