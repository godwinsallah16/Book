# Services

API service layer for the BookStore application.

## 📁 Structure

```
services/
├── authService.ts        # Authentication API calls
├── bookService.ts        # Book management API calls
├── cartService.ts        # Shopping cart API calls
├── favoritesService.ts   # Favorites management API calls
├── orderService.ts       # Order management API calls
├── README.md            # This documentation
└── index.ts             # Service exports
```

## 📋 Available Services

### 🔐 **authService**
- User authentication (login, register, logout)
- Password reset functionality
- Email verification
- JWT token management

### 📚 **bookService**
- Book CRUD operations
- Book search and filtering
- Book data management

### 🛒 **cartService**
- Shopping cart management
- Add/remove items from cart
- Cart synchronization with backend

### ⭐ **favoritesService**
- Favorite books management
- Add/remove favorites
- User favorites retrieval

### 📋 **orderService**
- Order creation and management
- Order history retrieval
- Order status tracking

## 🔧 Usage

Import services from the main services index:

```typescript
import { authService, bookService, cartService } from '../services';
```

Or import specific services:

```typescript
import { authService } from '../services/authService';
import { bookService } from '../services/bookService';
```

## 📡 API Integration

All services use the shared HTTP client from `utils/httpClient` and follow consistent patterns:

- **Error handling**: Standardized error responses
- **Authentication**: Automatic JWT token inclusion
- **Type safety**: Full TypeScript support
- **Async operations**: Promise-based API calls

## 🔒 Authentication

Services automatically handle authentication through:
- JWT token inclusion in requests
- Token refresh when needed
- Redirect to login on unauthorized access

## 🎯 Best Practices

1. **Use services in components**: Don't make direct API calls
2. **Handle errors gracefully**: Services provide consistent error handling
3. **Type safety**: All services are fully typed
4. **Async/await**: Use modern async patterns
