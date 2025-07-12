import { publicApiClient } from '../../utils/httpClient';
import { API_CONFIG } from '../../utils/constants';

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
      
      // Store token in localStorage
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
        email: response.data.email,
        firstName: response.data.firstName,
        lastName: response.data.lastName,
      }));
      
      return response.data;
    } catch (error) {
      console.error('Registration error:', error);
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
  getCurrentUser(): { email: string; firstName: string; lastName: string } | null {
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
