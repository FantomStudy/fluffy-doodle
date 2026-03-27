const BASE_URL = import.meta.env.VITE_API_URL ?? "";

interface ApiResponse {
  success: boolean;
  error: string | null;
}

interface ApiDataResponse<T> extends ApiResponse {
  data: T;
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

const requestJson = async <T>(path: string, init?: RequestInit): Promise<T> => {
  const res = await fetch(`${BASE_URL}${path}`, {
    ...init,
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...init?.headers,
    },
  });

  const data = await res.json();

  if (!res.ok) {
    throw new ApiError(res.status, data.error ?? `HTTP ${res.status}`);
  }

  return data as T;
};

const requestRaw = async (path: string, init?: RequestInit): Promise<ApiResponse> => {
  const res = await fetch(`${BASE_URL}${path}`, {
    ...init,
    credentials: "include",
  });

  const data: ApiResponse = await res.json();

  if (!res.ok) {
    throw new ApiError(res.status, data.error ?? `HTTP ${res.status}`);
  }

  return data;
};

export { request, requestJson, requestRaw, ApiError };
export type { ApiResponse, ApiDataResponse };
