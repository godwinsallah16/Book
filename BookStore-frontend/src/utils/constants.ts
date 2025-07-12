// API Configuration Constants
export const API_CONFIG = {
  // Base URLs - Use environment variable or fallback to localhost
  BASE_URL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api',
  
  // Timeouts
  DEFAULT_TIMEOUT: 10000,
  
  // HTTP Headers
  HEADERS: {
    CONTENT_TYPE: 'application/json',
    AUTHORIZATION: 'Bearer',
  },
  
  // Storage Keys
  STORAGE_KEYS: {
    AUTH_TOKEN: 'authToken',
    USER: 'user',
  },
  
  // API Endpoints
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/auth/login',
      REGISTER: '/auth/register',
      VERIFY_EMAIL: '/auth/verify-email',
      FORGOT_PASSWORD: '/auth/forgot-password',
      RESET_PASSWORD: '/auth/reset-password',
      RESEND_VERIFICATION: '/auth/resend-verification',
      CHANGE_PASSWORD: '/auth/change-password',
    },
    BOOKS: {
      BASE: '/books',
      BY_ID: (id: string | number) => `/books/${id}`,
      CATEGORIES: '/books/categories',
      AUTHORS: '/books/authors',
    },
    CART: {
      BASE: '/cart',
      BY_ID: (id: string | number) => `/cart/${id}`,
      SUMMARY: '/cart/summary',
    },
    FAVORITES: {
      BASE: '/favorites',
      BY_ID: (id: string | number) => `/favorites/${id}`,
    },
    HEALTH: '/health',
  },
  
  // HTTP Status Codes
  STATUS_CODES: {
    OK: 200,
    CREATED: 201,
    BAD_REQUEST: 400,
    UNAUTHORIZED: 401,
    FORBIDDEN: 403,
    NOT_FOUND: 404,
    INTERNAL_SERVER_ERROR: 500,
  },
} as const;

// UI Constants
export const UI_CONFIG = {
  // Colors
  COLORS: {
    PRIMARY: '#667eea',
    SECONDARY: '#764ba2',
    SUCCESS: '#155724',
    SUCCESS_BG: '#d4edda',
    SUCCESS_BORDER: '#c3e6cb',
    ERROR: '#721c24',
    ERROR_BG: '#f8d7da',
    ERROR_BORDER: '#f5c6cb',
    WARNING: '#856404',
    WARNING_BG: '#fff3cd',
    WARNING_BORDER: '#ffeeba',
    INFO: '#1565c0',
    INFO_BG: '#e3f2fd',
    INFO_BORDER: '#90caf9',
  },
  
  // Gradients
  GRADIENTS: {
    PRIMARY: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    CARD_SHADOW: '0 10px 30px rgba(0, 0, 0, 0.1)',
  },
  
  // Breakpoints
  BREAKPOINTS: {
    MOBILE: '768px',
    TABLET: '1024px',
    DESKTOP: '1200px',
  },
  
  // Spacing
  SPACING: {
    XS: '4px',
    SM: '8px',
    MD: '16px',
    LG: '24px',
    XL: '32px',
    XXL: '48px',
  },
  
  // Z-Index
  Z_INDEX: {
    MODAL: 1000,
    TOOLTIP: 1010,
    DROPDOWN: 1020,
    OVERLAY: 1030,
  },
} as const;

// Validation Constants
export const VALIDATION = {
  PASSWORD: {
    MIN_LENGTH: 6,
    MAX_LENGTH: 100,
  },
  EMAIL: {
    REGEX: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  },
  BOOK: {
    TITLE_MAX_LENGTH: 200,
    AUTHOR_MAX_LENGTH: 100,
    DESCRIPTION_MAX_LENGTH: 1000,
    MIN_PRICE: 0,
    MAX_PRICE: 9999.99,
  },
} as const;

// Messages
export const MESSAGES = {
  SUCCESS: {
    LOGIN: 'Login successful!',
    REGISTER: 'Registration successful!',
    PASSWORD_RESET_SENT: 'Password reset email sent! Check your inbox.',
    PASSWORD_RESET_SUCCESS: 'Password reset successfully! You can now log in.',
    EMAIL_VERIFIED: 'Email verified successfully! You can now log in.',
    VERIFICATION_EMAIL_SENT: 'Verification email sent successfully! Check your inbox.',
    BOOK_CREATED: 'Book created successfully!',
    BOOK_UPDATED: 'Book updated successfully!',
    BOOK_DELETED: 'Book deleted successfully!',
  },
  ERROR: {
    NETWORK: 'Network error - please check your connection',
    UNAUTHORIZED: 'Session expired. Please log in again.',
    GENERIC: 'An unexpected error occurred',
    INVALID_CREDENTIALS: 'Invalid email or password',
    EMAIL_EXISTS: 'Email already exists',
    PASSWORD_MISMATCH: 'Passwords do not match',
    INVALID_EMAIL: 'Please enter a valid email address',
    PASSWORD_TOO_SHORT: `Password must be at least ${VALIDATION.PASSWORD.MIN_LENGTH} characters`,
    REQUIRED_FIELD: 'This field is required',
    INVALID_RESET_LINK: 'Invalid reset link',
    RESET_LINK_EXPIRED: 'The password reset link has expired',
    VERIFICATION_FAILED: 'Email verification failed. The link may have expired.',
    VERIFICATION_EMAIL_FAILED: 'Failed to send verification email. Please try again.',
  },
  LOADING: {
    LOGGING_IN: 'Logging in...',
    REGISTERING: 'Registering...',
    SENDING: 'Sending...',
    RESETTING: 'Resetting...',
    VERIFYING: 'Verifying your email...',
    LOADING: 'Loading...',
    SAVING: 'Saving...',
    DELETING: 'Deleting...',
  },
} as const;

// Environment-specific configuration
export const ENV_CONFIG = {
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD,
  baseUrl: import.meta.env.VITE_API_BASE_URL || API_CONFIG.BASE_URL,
} as const;

// Utility function to get the API base URL
export const getApiBaseUrl = (): string => {
  return ENV_CONFIG.baseUrl;
};

// Utility function to build full API URL
export const buildApiUrl = (endpoint: string): string => {
  return `${getApiBaseUrl()}${endpoint}`;
};
