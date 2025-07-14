import { publicApiClient, apiClient, API_CONFIG } from '../utils';

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
  refreshToken: string;
  refreshTokenExpiration: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  expiration: string;
  emailConfirmed: boolean;
  roles: string[];
}

export interface RegisterResponse {
  success: boolean;
  message: string;
  data?: AuthResponse;
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
      // Store tokens in localStorage
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('refreshTokenExpiration', response.data.refreshTokenExpiration);
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
        userId: response.data.userId,
        email: response.data.email,
        firstName: response.data.firstName,
        lastName: response.data.lastName,
        emailConfirmed: response.data.emailConfirmed,
        roles: response.data.roles
      }));
      return response.data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  },
  // Register user
  async register(registerData: RegisterRequest): Promise<RegisterResponse> {
    try {
      const response = await publicApiClient.post<RegisterResponse>(API_CONFIG.ENDPOINTS.AUTH.REGISTER, registerData);
      // If registration returns auth data, store tokens
      if (response.data && response.data.data) {
        localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.data.token);
        localStorage.setItem('refreshToken', response.data.data.refreshToken);
        localStorage.setItem('refreshTokenExpiration', response.data.data.refreshTokenExpiration);
        localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
          userId: response.data.data.userId,
          email: response.data.data.email,
          firstName: response.data.data.firstName,
          lastName: response.data.data.lastName,
          emailConfirmed: response.data.data.emailConfirmed,
          roles: response.data.data.roles
        }));
      }
      return response.data;
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
  },
  // ...existing code...

  // Logout user
  logout(): void {
    localStorage.removeItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('refreshTokenExpiration');
    localStorage.removeItem(API_CONFIG.STORAGE_KEYS.USER);
  },

  // Check if user is authenticated by verifying token with backend
  async isAuthenticated(): Promise<boolean> {
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    if (!token) return false;
    try {
      // Attempt to fetch current user with token
      const response = await apiClient.get<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.ME);
      return !!response.data && !!response.data.userId;
    } catch {
      // If token is invalid or expired, treat as unauthenticated
      return false;
    }
  },

  // Get current user
  getCurrentUser(): { userId: string; email: string; firstName: string; lastName: string; emailConfirmed?: boolean; roles?: string[] } | null {
    const userStr = localStorage.getItem(API_CONFIG.STORAGE_KEYS.USER);
    return userStr ? JSON.parse(userStr) : null;
  },

  // Check if current user's email is verified
  isEmailVerified(): boolean {
    const user = this.getCurrentUser();
    return user ? user.emailConfirmed === true : false;
  },

  // Get auth token
  getToken(): string | null {
    let token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    // Optionally, check expiration and refresh if needed
    // For demo, always try to refresh if token is missing
    if (!token) {
      // Try to refresh
      this.refreshToken().then(newToken => {
        if (newToken) {
          token = newToken;
        }
      });
    }
    return token;
  },
  // Refresh access token using refresh token
  async refreshToken(): Promise<string | null> {
    const user = this.getCurrentUser();
    const refreshToken = localStorage.getItem('refreshToken');
    if (!user || !refreshToken) return null;
    try {
      const response = await publicApiClient.post<AuthResponse>('/auth/refresh-token', {
        userId: user.userId,
        refreshToken,
      });
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('refreshTokenExpiration', response.data.refreshTokenExpiration);
      return response.data.token;
    } catch (error) {
      console.error('Refresh token error:', error);
      this.logout();
      return null;
    }
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

  // ...existing code...

  // Fetch current user
  async fetchCurrentUser(): Promise<{ userId: string; email: string; firstName: string; lastName: string; emailConfirmed?: boolean; roles?: string[] } | null> {
    try {
      const response = await apiClient.get<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.ME);
      if (response.data) {
        localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
          userId: response.data.userId,
          email: response.data.email,
          firstName: response.data.firstName,
          lastName: response.data.lastName,
          emailConfirmed: response.data.emailConfirmed,
          roles: response.data.roles,
        }));
        return response.data;
      }
      return null;
    } catch (error) {
      console.error('Fetch current user error:', error);
      return null;
    }
  },
};

export default authService;
