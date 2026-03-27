import { ApiError, request } from "./client";

interface SignInPayload {
  login: string;
  password: string;
}

interface SignUpPayload {
  login: string;
  password: string;
  fullName: string;
  phoneNumber: string;
}

const signIn = (payload: SignInPayload) =>
  request("/auth/sign-in", {
    method: "POST",
    body: JSON.stringify(payload),
  });

const signUp = (payload: SignUpPayload) =>
  request("/auth/sign-up", {
    method: "POST",
    body: JSON.stringify(payload),
  });

const refresh = () => request("/auth/refresh", { method: "POST" });

const logout = () => request("/auth/logout", { method: "POST" });

export { signIn, signUp, refresh, logout, ApiError };
