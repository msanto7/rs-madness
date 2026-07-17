import axios, { AxiosError } from 'axios';
import type { InternalAxiosRequestConfig } from 'axios';
import { emitAuthExpired } from '../auth/sessionEvents';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5202/api';

export interface AppApiError {
    status: number;
    errorCode?: string;
    message: string;
    errors: string[];
    type?: string;
}

type ProblemDetailsPayload = {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    errorCode?: string;
    errors?: string[] | Record<string, string[]>;
}

type RetriableRequestConfig = InternalAxiosRequestConfig & {
  _retry?: boolean;
}

function flattenErrors(value: ProblemDetailsPayload['errors']): string[] {
  if (!value) return [];
  if (Array.isArray(value)) return value.filter((x): x is string => typeof x === 'string');
  return Object.values(value).flat().filter((x): x is string => typeof x === 'string');
}

function toAppApiError(error: AxiosError): AppApiError {
  const data = (error.response?.data ?? {}) as ProblemDetailsPayload;
  const errors = flattenErrors(data.errors);

  return {
    status: error.response?.status ?? 0,
    errorCode: data.errorCode,
    message: data.detail ?? data.title ?? 'Request failed.',
    errors,
    type: data.type
  };
}

// axios calls will use this base URL
const apiClient = axios.create({
    baseURL: apiBaseUrl,
    withCredentials: true,
    headers: {
        'Content-Type': 'application/json',
    },
});

// shared across concurrent 401s so a burst of parallel requests rotates the refresh token once, not once per request
let refreshPromise: Promise<void> | null = null;

function refreshAccessToken(): Promise<void> {
  if (!refreshPromise) {
    refreshPromise = apiClient.post('/auth/refresh')
      .then(() => undefined)
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const apiError = toAppApiError(error);
    const originalRequest = error.config as RetriableRequestConfig | undefined;
    const requestUrl = originalRequest?.url ?? '';

    if (apiError.status === 401) {
      const isAuthAction =
        requestUrl.includes('/auth/login') ||
        requestUrl.includes('/auth/register') ||
        requestUrl.includes('/auth/logout') ||
        requestUrl.includes('/auth/refresh');

      if (originalRequest && !originalRequest._retry && !isAuthAction) {
        originalRequest._retry = true;

        try {
          await refreshAccessToken();
          return apiClient(originalRequest);
        } catch {
          emitAuthExpired();
          return Promise.reject(apiError);
        }
      }

      emitAuthExpired();
    }

    return Promise.reject(apiError);
  }
);

export default apiClient;
