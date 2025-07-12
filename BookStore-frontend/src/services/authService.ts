import { publicApiClient } from '../utils/httpClient';
import { API_CONFIG } from '../utils/constants';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  expiration: string;
}

export interface VerifyEmailRequest {
  userId: string;
  token: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
}

export interface ResendVerificationRequest {
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

// ...existing interfaces...

export const authService = {
  // Login user
  async login(loginData: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await publicApiClient.post<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.LOGIN, loginData);
      
      // Store token in localStorage
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
        userId: response.data.userId,
        email: response.data.email,
        firstName: response.data.firstName,
        lastName: response.data.lastName,
      }));
      
      return response.data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  },

  // Register user
  async register(registerData: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await publicApiClient.post<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.REGISTER, registerData);
      
      // DO NOT store token after registration - user must verify email first
      
      return response.data;
    } catch (error: unknown) {
      console.error('Registration error:', error);
      
      // Handle validation errors with user-friendly messages
      if (error && typeof error === 'object' && 'response' in error) {
        const axiosError = error as { response?: { data?: unknown; status?: number } };
        
        if (axiosError.response?.status === 400) {
          // Handle validation errors
          const errorData = axiosError.response.data;
          
          // Check if it's a validation error
          if (errorData && typeof errorData === 'object') {
            // Handle ModelState validation errors
            if ('errors' in errorData && errorData.errors) {
              const validationErrors = [];
              const errors = errorData.errors as Record<string, string[]>;
              for (const field in errors) {
                const fieldErrors = errors[field];
                if (Array.isArray(fieldErrors)) {
                  validationErrors.push(...fieldErrors);
                }
              }
              if (validationErrors.length > 0) {
                throw new Error(validationErrors.join('. '));
              }
            }
            
            // Handle single validation error messages
            if ('title' in errorData && errorData.title && typeof errorData.title === 'string' && errorData.title.includes('validation')) {
              throw new Error('Please check your input: Password must contain at least 8 characters including uppercase, lowercase, digit and special character.');
            }
            
            // Handle string error messages
            if (typeof errorData === 'string') {
              throw new Error(errorData);
            }
          }
          
          // Default validation message for 400 errors
          throw new Error('Please check your input: Password must contain at least 8 characters including uppercase, lowercase, digit and special character.');
        }
      }
      
      throw error;
    }
  },

  // Logout user
  logout(): void {
    localStorage.removeItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    localStorage.removeItem(API_CONFIG.STORAGE_KEYS.USER);
  },

  // Check if user is authenticated
  isAuthenticated(): boolean {
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    return token !== null;
  },

  // Get current user
  getCurrentUser(): { userId: string; email: string; firstName: string; lastName: string } | null {
    const userStr = localStorage.getItem(API_CONFIG.STORAGE_KEYS.USER);
    return userStr ? JSON.parse(userStr) : null;
  },

  // Get auth token
  getToken(): string | null {
    return localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
  },

  // Verify email
  async verifyEmail(verifyData: VerifyEmailRequest): Promise<void> {
    try {
      await publicApiClient.post(API_CONFIG.ENDPOINTS.AUTH.VERIFY_EMAIL, verifyData);
    } catch (error) {
      console.error('Email verification error:', error);
      throw error;
    }
  },

  // Forgot password
  async forgotPassword(forgotData: ForgotPasswordRequest): Promise<void> {
    try {
      await publicApiClient.post(API_CONFIG.ENDPOINTS.AUTH.FORGOT_PASSWORD, forgotData);
    } catch (error) {
      console.error('Forgot password error:', error);
      throw error;
    }
  },

  // Reset password
  async resetPassword(resetData: ResetPasswordRequest): Promise<void> {
    try {
      await publicApiClient.post(API_CONFIG.ENDPOINTS.AUTH.RESET_PASSWORD, resetData);
    } catch (error) {
      console.error('Reset password error:', error);
      throw error;
    }
  },

  // Resend verification email
  async resendVerification(resendData: ResendVerificationRequest): Promise<void> {
    try {
      await publicApiClient.post(API_CONFIG.ENDPOINTS.AUTH.RESEND_VERIFICATION, resendData);
    } catch (error) {
      console.error('Resend verification error:', error);
      throw error;
    }
  },

  // Change password
  async changePassword(changeData: ChangePasswordRequest): Promise<void> {
    try {
      await publicApiClient.post(API_CONFIG.ENDPOINTS.AUTH.CHANGE_PASSWORD, changeData);
    } catch (error) {
      console.error('Change password error:', error);
      throw error;
    }
  },
};

export default authService;
