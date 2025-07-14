import axios from 'axios';
import type { AxiosInstance, AxiosResponse, AxiosError } from 'axios';
import { API_CONFIG, getApiBaseUrl } from './constants';

// Create a custom axios instance with default configuration
export const createApiClient = (options: {
  includeAuth?: boolean;
  timeout?: number;
} = {}): AxiosInstance => {
  const { includeAuth = true, timeout = API_CONFIG.DEFAULT_TIMEOUT } = options;

  const client = axios.create({
    baseURL: getApiBaseUrl(),
    timeout,
    headers: {
      'Content-Type': API_CONFIG.HEADERS.CONTENT_TYPE,
    },
  });

  // Request interceptor for auth token
  if (includeAuth) {
    client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );
  }

  // Response interceptor for error handling
  client.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error: AxiosError) => {
      // Handle common HTTP errors
      if (error.response?.status === API_CONFIG.STATUS_CODES.UNAUTHORIZED) {
        // Log detailed error info for debugging
        console.warn('User session expired (401)');
        console.error('401 Unauthorized error details:', {
          url: error.config?.url,
          method: error.config?.method,
          headers: error.config?.headers,
          responseData: error.response?.data,
          responseHeaders: error.response?.headers,
        });
      }
      return Promise.reject(error);
    }
  );

  return client;
};

// Default API client instance
export const apiClient = createApiClient();

// API client without auth for public endpoints
export const publicApiClient = createApiClient({ includeAuth: false });

// Types for API responses
export interface ApiError {
  message: string;
  statusCode: number;
  errors?: Record<string, string[]>;
}

export interface ApiResponse<T = unknown> {
  data: T;
  success: boolean;
  message?: string;
}

// Helper function to handle API errors
export const handleApiError = (error: AxiosError): ApiError => {
  const defaultError: ApiError = {
    message: 'An unexpected error occurred',
    statusCode: 500,
  };

  if (!error.response) {
    return {
      ...defaultError,
      message: 'Network error - please check your connection',
    };
  }

  const { status, data } = error.response;
  
  return {
    message: (data as { message?: string })?.message || defaultError.message,
    statusCode: status,
    errors: (data as { errors?: Record<string, string[]> })?.errors,
  };
};
