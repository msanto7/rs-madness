import axios, { AxiosError } from 'axios';
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
    headers: {
        'Content-Type': 'application/json',
    },
});

// interceptor for outgoing requests to the API - jwt token attached in header
apiClient.interceptors.request.use((config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}));

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    const apiError = toAppApiError(error);

    if (apiError.status === 401) {
      localStorage.removeItem('token');
      emitAuthExpired();
    }

    return Promise.reject(apiError);
  }
);

export default apiClient;
