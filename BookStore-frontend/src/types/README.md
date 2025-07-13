# TypeScript Type Definitions

Type definitions for the BookStore application providing type safety across the entire codebase.

## 📁 Structure

```
types/
├── book.types.ts      # Book-related types
├── cart.types.ts      # Shopping cart types  
├── favorites.types.ts # User favorites types
├── order.types.ts     # Order and checkout types
├── user.types.ts      # User and authentication types
└── README.md          # This file
```

## 🏷 Available Types

### **Book Types** (`book.types.ts`)
Types related to books and book data.

```typescript
export interface Book {
  id: string;
  title: string;
  author: string;
  description: string;
  price: number;
  category: string;
  isbn: string;
  publishedDate: string;
  imageUrl?: string;
  stock: number;
  rating?: number;
  reviewCount?: number;
}

export interface BookSearchParams {
  query?: string;
  category?: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: 'title' | 'author' | 'price' | 'rating' | 'publishedDate';
  sortOrder?: 'asc' | 'desc';
  page?: number;
  limit?: number;
}

export interface BookCreateData {
  title: string;
  author: string;
  description: string;
  price: number;
  category: string;
  isbn: string;
  publishedDate: string;
  imageUrl?: string;
  stock: number;
}

export type BookUpdateData = Partial<BookCreateData>;
```

### **User Types** (`user.types.ts`)
Types for user authentication and profile management.

```typescript
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'User' | 'Admin';
  createdAt: string;
  updatedAt: string;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  user: User;
  token: string;
  refreshToken: string;
}

export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  address?: Address;
  preferences?: UserPreferences;
}

export interface Address {
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface UserPreferences {
  favoriteCategories: string[];
  emailNotifications: boolean;
  theme: 'light' | 'dark';
}
```

### **Cart Types** (`cart.types.ts`)
Types for shopping cart functionality.

```typescript
export interface CartItem {
  id: string;
  bookId: string;
  book: Book;
  quantity: number;
  price: number;
  addedAt: string;
}

export interface Cart {
  id: string;
  userId: string;
  items: CartItem[];
  totalItems: number;
  totalPrice: number;
  updatedAt: string;
}

export interface AddToCartData {
  bookId: string;
  quantity: number;
}

export interface UpdateCartItemData {
  quantity: number;
}

export interface CartSummary {
  itemCount: number;
  subtotal: number;
  tax: number;
  shipping: number;
  total: number;
}
```

### **Order Types** (`order.types.ts`)
Types for order processing and management.

```typescript
export interface Order {
  id: string;
  userId: string;
  orderNumber: string;
  status: OrderStatus;
  items: OrderItem[];
  shippingAddress: Address;
  billingAddress: Address;
  paymentMethod: PaymentMethod;
  subtotal: number;
  tax: number;
  shipping: number;
  total: number;
  createdAt: string;
  updatedAt: string;
}

export interface OrderItem {
  id: string;
  bookId: string;
  book: Book;
  quantity: number;
  price: number;
  total: number;
}

export type OrderStatus = 
  | 'Pending'
  | 'Processing'
  | 'Shipped'
  | 'Delivered'
  | 'Cancelled'
  | 'Refunded';

export interface PaymentMethod {
  type: 'CreditCard' | 'PayPal' | 'BankTransfer';
  cardLast4?: string;
  cardBrand?: string;
  expiryMonth?: number;
  expiryYear?: number;
}

export interface CreateOrderData {
  items: Array<{
    bookId: string;
    quantity: number;
  }>;
  shippingAddress: Address;
  billingAddress: Address;
  paymentMethod: PaymentMethod;
}

export interface OrderSearchParams {
  status?: OrderStatus;
  startDate?: string;
  endDate?: string;
  page?: number;
  limit?: number;
}
```

### **Favorites Types** (`favorites.types.ts`)
Types for user favorites functionality.

```typescript
export interface UserFavorite {
  id: string;
  userId: string;
  bookId: string;
  book: Book;
  addedAt: string;
}

export interface FavoritesList {
  favorites: UserFavorite[];
  totalCount: number;
}

export interface AddToFavoritesData {
  bookId: string;
}
```

## 🔧 Type Patterns

### **API Response Types**
```typescript
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  pagination: {
    page: number;
    limit: number;
    total: number;
    totalPages: number;
    hasNext: boolean;
    hasPrev: boolean;
  };
}

export interface ErrorResponse {
  error: string;
  message: string;
  statusCode: number;
  timestamp: string;
}
```

### **Form Types**
```typescript
export interface FormFieldError {
  field: string;
  message: string;
}

export interface FormState<T> {
  data: T;
  errors: FormFieldError[];
  isSubmitting: boolean;
  isValid: boolean;
}

export interface ValidationRule {
  required?: boolean;
  minLength?: number;
  maxLength?: number;
  pattern?: RegExp;
  custom?: (value: any) => string | null;
}
```

### **Component Props Types**
```typescript
export interface BaseComponentProps {
  className?: string;
  children?: React.ReactNode;
  testId?: string;
}

export interface ButtonProps extends BaseComponentProps {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  loading?: boolean;
  disabled?: boolean;
  onClick?: () => void;
}

export interface InputProps extends BaseComponentProps {
  type?: string;
  placeholder?: string;
  value?: string;
  error?: string;
  required?: boolean;
  onChange?: (value: string) => void;
}
```

## 🔍 Type Utilities

### **Common Utility Types**
```typescript
// Create optional version of required fields
export type Optional<T, K extends keyof T> = Omit<T, K> & Partial<Pick<T, K>>;

// Create required version of optional fields
export type Required<T, K extends keyof T> = T & Required<Pick<T, K>>;

// Extract keys of specific type
export type KeysOfType<T, U> = {
  [K in keyof T]: T[K] extends U ? K : never;
}[keyof T];

// Create a type with only specified keys
export type PickByType<T, U> = Pick<T, KeysOfType<T, U>>;
```

### **API Type Helpers**
```typescript
// Extract data type from API response
export type ExtractApiData<T> = T extends ApiResponse<infer U> ? U : never;

// Create update type that makes all fields optional except id
export type UpdateType<T> = Partial<Omit<T, 'id'>> & Pick<T, 'id'>;

// Create create type that omits generated fields
export type CreateType<T> = Omit<T, 'id' | 'createdAt' | 'updatedAt'>;
```

## 📝 Best Practices

### **Type Naming Conventions**
- Use PascalCase for interfaces and types
- Use descriptive names that clearly indicate purpose
- Suffix with appropriate descriptor (Data, Response, Params, etc.)
- Group related types in the same file

### **Interface vs Type**
- Use `interface` for object shapes that might be extended
- Use `type` for unions, primitives, and computed types
- Prefer `interface` for component props and API responses

### **Generics Usage**
```typescript
// Good: Generic API response
export interface ApiResponse<T> {
  data: T;
  message: string;
}

// Good: Generic form state
export interface FormState<T> {
  data: T;
  errors: Record<keyof T, string>;
}
```

### **Documentation**
```typescript
/**
 * Represents a book in the bookstore
 * @interface Book
 */
export interface Book {
  /** Unique identifier for the book */
  id: string;
  /** Title of the book */
  title: string;
  /** Author of the book */
  author: string;
  /** Book description */
  description: string;
  /** Price in USD */
  price: number;
}
```

## 🧪 Type Testing

Use type-only imports for testing:

```typescript
import type { Book, BookSearchParams } from '@/types/book.types';

// Type assertions for testing
const mockBook: Book = {
  id: '1',
  title: 'Test Book',
  author: 'Test Author',
  // ... other required fields
};

// Utility for creating test data
export const createMockBook = (overrides: Partial<Book> = {}): Book => ({
  id: '1',
  title: 'Test Book',
  author: 'Test Author',
  description: 'Test description',
  price: 29.99,
  category: 'Fiction',
  isbn: '1234567890',
  publishedDate: '2023-01-01',
  stock: 10,
  ...overrides,
});
```
