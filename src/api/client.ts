const BASE_URL = import.meta.env.VITE_API_URL ?? "";

interface ApiResponse {
  success: boolean;
  error: string | null;
}

class ApiError extends Error {
  constructor(
    public status: number,
    message: string,
  ) {
    super(message);
  }
}

const request = async (path: string, init?: RequestInit): Promise<ApiResponse> => {
  const res = await fetch(`${BASE_URL}${path}`, {
    ...init,
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...init?.headers,
    },
  });

  const data: ApiResponse = await res.json();

  if (!res.ok) {
    throw new ApiError(res.status, data.error ?? `HTTP ${res.status}`);
  }

  return data;
};

export { request, ApiError };
export type { ApiResponse };
