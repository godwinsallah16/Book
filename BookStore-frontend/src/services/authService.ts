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
  roles?: string[];
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

export const authService = {
  // Login user
  async login(loginData: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await publicApiClient.post<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.LOGIN, loginData);
      
      // Store tokens in localStorage
      console.log('[authService] login: Setting auth token with key', API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, 'and value', response.data.token);
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('refreshTokenExpiration', response.data.refreshTokenExpiration);
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
        userId: response.data?.userId ?? '',
        email: response.data?.email ?? '',
        firstName: response.data?.firstName ?? '',
        lastName: response.data?.lastName ?? '',
        emailConfirmed: response.data?.emailConfirmed ?? false,
        roles: response.data?.roles ?? []
      }));
      
      // Add a small delay to ensure localStorage is properly set
      await new Promise(resolve => setTimeout(resolve, 100));
      
      console.log('[authService] login: Login successful, tokens stored');
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
        console.log('[authService] register: Setting auth token with key', API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, 'and value', response.data.data.token);
        localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.data.token);
        localStorage.setItem('refreshToken', response.data.data.refreshToken);
        localStorage.setItem('refreshTokenExpiration', response.data.data.refreshTokenExpiration);
        localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
          userId: response.data?.data?.userId ?? '',
          email: response.data?.data?.email ?? '',
          firstName: response.data?.data?.firstName ?? '',
          lastName: response.data?.data?.lastName ?? '',
          emailConfirmed: response.data?.data?.emailConfirmed ?? false,
          roles: response.data?.data?.roles ?? []
        }));
        
        // Add a small delay to ensure localStorage is properly set
        await new Promise(resolve => setTimeout(resolve, 100));
      }
      
      return response.data;
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
  },

  // Logout user
  logout(): void {
    console.warn('[authService] logout called. Removing all auth tokens and user info from localStorage.');
    console.log('[authService] logout: Removing auth token with key', API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    localStorage.removeItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('refreshTokenExpiration');
    localStorage.removeItem(API_CONFIG.STORAGE_KEYS.USER);
  },

  // Simple check if user has valid tokens (without API call)
  hasValidTokens(): boolean {
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    const user = this.getCurrentUser();
    
    if (!token || !user) {
      return false;
    }
    
    // Check if token is expired
    if (this.isJwtExpired(token)) {
      return false;
    }
    
    return true;
  },

  // Check if user is authenticated by verifying token with backend
  async isAuthenticated(): Promise<boolean> {
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    console.log('[authService] isAuthenticated: Read auth token with key', API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, 'value:', token);
    
    if (!token) {
      console.log('[authService] isAuthenticated: No token found in localStorage.');
      return false;
    }

    // Check if token is expired
    if (this.isJwtExpired(token)) {
      console.log('[authService] isAuthenticated: Token is expired, attempting refresh.');
      const newToken = await this.refreshToken();
      if (!newToken) {
        console.warn('[authService] isAuthenticated: Token expired and refresh failed.');
        return false;
      }
      console.log('[authService] isAuthenticated: Token refreshed successfully.');
    }

    try {
      const response = await apiClient.get<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.ME);
      
      // Only treat as authenticated if status is 200 and userId is present
      if (response.status === 200 && response.data && response.data.userId) {
        // Update stored user data if received
        localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
          userId: response.data?.userId ?? '',
          email: response.data?.email ?? '',
          firstName: response.data?.firstName ?? '',
          lastName: response.data?.lastName ?? '',
          emailConfirmed: response.data?.emailConfirmed ?? false,
          roles: response.data?.roles ?? []
        }));
        
        // Update refresh token if provided
        if (response.data.refreshToken) {
          localStorage.setItem('refreshToken', response.data.refreshToken);
        }
        if (response.data.refreshTokenExpiration) {
          localStorage.setItem('refreshTokenExpiration', response.data.refreshTokenExpiration);
        }
        
        return true;
      }
      
      console.warn('[authService] isAuthenticated: /auth/me response did not contain userId or status was not 200.');
      return false;
    } catch (error) {
      console.error('[authService] isAuthenticated: /auth/me error:', error);
      
      // Handle specific error cases
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const errResp = (error as { response?: { status?: number } }).response;
        if (errResp?.status === 204) {
          console.warn('[authService] isAuthenticated: /auth/me returned 204 No Content.');
          return false;
        }
        // Don't immediately logout on 401/403 - might be temporary
        if (errResp?.status === 401 || errResp?.status === 403) {
          console.warn('[authService] isAuthenticated: Received 401/403 from /auth/me');
          return false;
        }
      }
      
      // For network errors or other issues, don't clear auth state
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
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    console.log('[authService] getToken: Read auth token with key', API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, 'value:', token);
    if (!token) return null;
    if (this.isJwtExpired(token)) {
      // Do not logout here, just return null
      return null;
    }
    return token;
  },

  // Refresh access token using refresh token
  async refreshToken(): Promise<string | null> {
    const user = this.getCurrentUser();
    const refreshToken = localStorage.getItem('refreshToken');
    if (!user || !refreshToken) {
      console.warn('[authService] refreshToken: No user or refreshToken found.');
      return null;
    }
    try {
      const response = await publicApiClient.post<AuthResponse>('/auth/refresh-token', {
        userId: user.userId,
        refreshToken,
      });
      console.log('[authService] refreshToken: Setting auth token with key', API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, 'and value', response.data.token);
      localStorage.setItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('refreshTokenExpiration', response.data.refreshTokenExpiration);
      console.log('[authService] refreshToken: Token refreshed successfully.');
      return response.data.token;
    } catch (error) {
      console.error('[authService] refreshToken error:', error);
      // Type guard for AxiosError
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const errResp = (error as { response?: { status?: number } }).response;
        if (errResp?.status === 401 || errResp?.status === 403) {
          console.warn('[authService] refreshToken: Received 401/403. Not logging out automatically.');
        }
      }
      // Do not log out for any errors, just return null
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

  // Fetch current user
  async fetchCurrentUser(): Promise<{ userId: string; email: string; firstName: string; lastName: string; emailConfirmed?: boolean; roles?: string[] } | null> {
    try {
      const response = await apiClient.get<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.ME);
      if (response.data) {
        localStorage.setItem(API_CONFIG.STORAGE_KEYS.USER, JSON.stringify({
          userId: response.data?.userId ?? '',
          email: response.data?.email ?? '',
          firstName: response.data?.firstName ?? '',
          lastName: response.data?.lastName ?? '',
          emailConfirmed: response.data?.emailConfirmed ?? false,
          roles: response.data?.roles ?? []
        }));
        return response.data;
      }
      return null;
    } catch (error) {
      console.error('Fetch current user error:', error);
      return null;
    }
  },

  // Check if JWT token is expired (with 30 second buffer)
  isJwtExpired(token: string): boolean {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp;
      if (!exp) return true;
      
      const now = Math.floor(Date.now() / 1000);
      const bufferTime = 30; // 30 second buffer
      
      console.log('[authService] isJwtExpired: Token exp:', exp, 'Current time:', now, 'Expired:', exp < (now + bufferTime));
      return exp < (now + bufferTime);
    } catch (error) {
      console.error('[authService] isJwtExpired: Error parsing token:', error);
      return true;
    }
  },
};

export default authService;