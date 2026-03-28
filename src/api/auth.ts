import { api } from "./instance";

interface SignInRequest {
  login: string;
  password: string;
}

interface SignUpRequest {
  fullName: string;
  login: string;
  password: string;
  phoneNumber: string;
  studentCode?: string;
}

interface AuthResponse {
  status: boolean;
  error?: unknown;
}

export const signIn = (body: SignInRequest) =>
  api<AuthResponse>("/auth/sign-in", { method: "POST", body });

export const signUp = (body: SignUpRequest) =>
  api<AuthResponse>("/auth/sign-up", { method: "POST", body });

export const logout = () => api<AuthResponse>("/auth/logout", { method: "POST" });

export const refresh = () => api<AuthResponse>("/auth/refresh", { method: "POST" });

export const isAdmin = () => api<AuthResponse>("/auth/is-admin");
