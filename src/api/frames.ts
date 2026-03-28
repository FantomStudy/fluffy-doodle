import { api } from "./instance";

export interface Frame {
  id: number;
  name: string;
  price: number;
  image: string;
  owned: boolean;
}

interface ApiResponse {
  success: boolean;
  error: string | null;
}

export const getFrames = () => api<Frame[]>("/user/frames");

export const buyFrame = (frameId: number) =>
  api<ApiResponse>(`/user/frames/${frameId}/buy`, { method: "POST" });

export const setActiveFrame = (frameId: number) =>
  api<ApiResponse>(`/user/frames/${frameId}/active`, { method: "POST" });
