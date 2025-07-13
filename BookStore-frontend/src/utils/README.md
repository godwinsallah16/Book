# Utilities

Utility functions and constants for the BookStore application.

## 📁 Structure

```
utils/
├── constants.ts      # Application constants
├── httpClient.ts     # HTTP client configuration
├── index.ts         # Main utilities exports
└── README.md        # This file
```

## 🛠 Available Utilities

### **Constants** (`constants.ts`)
Application-wide constants and configuration values.

**Exports:**
- `API_BASE_URL`: Base URL for API requests
- `APP_NAME`: Application name
- `VERSION`: Application version
- `ROUTES`: Route constants
- `STORAGE_KEYS`: LocalStorage key constants
- `VALIDATION`: Validation rules and messages

**Usage:**
```typescript
import { API_BASE_URL, ROUTES, STORAGE_KEYS } from '@/utils/constants';

const loginUrl = `${API_BASE_URL}/auth/login`;
const userToken = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
```

### **HTTP Client** (`httpClient.ts`)
Configured HTTP client with interceptors and error handling.

**Features:**
- Request/response interceptors
- Automatic token attachment
- Error handling and logging
- Request/response transformation
- Base URL configuration

**Exports:**
- `httpClient`: Configured axios instance
- `ApiError`: Custom error class
- `createApiRequest`: Request factory function

**Usage:**
```typescript
import { httpClient } from '@/utils/httpClient';

// GET request
const books = await httpClient.get('/books');

// POST request with auth
const newBook = await httpClient.post('/books', bookData);

// Error handling
try {
  const response = await httpClient.get('/protected-route');
} catch (error) {
  if (error instanceof ApiError) {
    console.error('API Error:', error.message, error.status);
  }
}
```

## 🔧 Utility Patterns

### **Error Handling**
```typescript
export class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
    public code?: string
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

export const handleApiError = (error: any): ApiError => {
  if (error.response) {
    return new ApiError(
      error.response.data.message || 'An error occurred',
      error.response.status,
      error.response.data.code
    );
  }
  return new ApiError('Network error', 0);
};
```

### **Request Interceptors**
```typescript
// Add auth token to requests
httpClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle response errors
httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized
      localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
      window.location.href = ROUTES.LOGIN;
    }
    return Promise.reject(handleApiError(error));
  }
);
```

### **Constants Organization**
```typescript
export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: '/auth/login',
    REGISTER: '/auth/register',
    REFRESH: '/auth/refresh',
    LOGOUT: '/auth/logout',
  },
  BOOKS: {
    LIST: '/books',
    DETAIL: (id: string) => `/books/${id}`,
    CREATE: '/books',
    UPDATE: (id: string) => `/books/${id}`,
    DELETE: (id: string) => `/books/${id}`,
  },
  CART: {
    GET: '/cart',
    ADD: '/cart/items',
    UPDATE: (id: string) => `/cart/items/${id}`,
    REMOVE: (id: string) => `/cart/items/${id}`,
    CLEAR: '/cart/clear',
  },
} as const;
```

## 🧪 Testing Utilities

Create test utilities for consistent testing:

```typescript
// test-utils.ts
import { render, RenderOptions } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from '@/context/AuthContext';

const AllTheProviders = ({ children }: { children: React.ReactNode }) => {
  return (
    <BrowserRouter>
      <AuthProvider>
        {children}
      </AuthProvider>
    </BrowserRouter>
  );
};

const customRender = (
  ui: React.ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => render(ui, { wrapper: AllTheProviders, ...options });

export * from '@testing-library/react';
export { customRender as render };
```

## 📦 Adding New Utilities

When adding new utilities:

1. **Create the utility file** with clear function names
2. **Add TypeScript types** for all parameters and returns
3. **Write unit tests** for complex logic
4. **Export from index.ts** for easy importing
5. **Document in this README** with examples

Example utility structure:
```typescript
// formatters.ts
export const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(amount);
};

export const formatDate = (date: Date | string): string => {
  return new Intl.DateTimeFormat('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  }).format(new Date(date));
};
```

## 🔍 Best Practices

1. **Pure Functions**: Keep utilities as pure functions when possible
2. **Single Responsibility**: Each utility should have one clear purpose
3. **Type Safety**: Use TypeScript for all utilities
4. **Error Handling**: Handle edge cases and provide meaningful errors
5. **Performance**: Consider memoization for expensive operations
6. **Testing**: Write unit tests for complex utilities
7. **Documentation**: Include JSDoc comments for complex functions
