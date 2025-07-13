# Features

Feature-based architecture for the BookStore frontend application.

## 📁 Structure

Each feature is organized as a self-contained module with its own components, pages, and logic.

```
features/
├── auth/              # Authentication & user management
├── books/             # Book management & browsing
├── cart/              # Shopping cart functionality
├── dashboard/         # Main dashboard
├── orders/            # Order management & checkout
└── index.ts           # Main feature exports
```

## 🏗️ Feature Organization

Each feature follows this consistent structure:

```
[feature-name]/
├── components/        # Feature-specific components
├── pages/            # Feature pages
├── README.md         # Feature documentation
└── index.ts          # Feature exports
```

## 📦 Available Features

### 🔐 **Auth**
- Login, Registration, Password Reset
- Email Verification
- User Authentication

### 📚 **Books** 
- Book Listing & Search
- Book Creation & Editing
- Favorites Management

### 🛒 **Cart**
- Shopping Cart Management
- Add/Remove Items
- Cart Icon with Count

### 📊 **Dashboard**
- Main User Dashboard
- Quick Navigation
- Overview

### 📋 **Orders**
- Checkout Process
- Order History
- Order Confirmation

## 🔗 Usage

Import features from the main features index:

```typescript
import { 
  LoginPage, 
  RegisterPage,
  DashboardPage,
  BookFormPage,
  Checkout,
  Orders
} from './features';
```

Or import from specific features:

```typescript
import { LoginPage } from './features/auth';
import { BookList } from './features/books';
import { ShoppingCart } from './features/cart';
```

## ✅ Benefits

- **Modular**: Each feature is self-contained
- **Scalable**: Easy to add new features
- **Maintainable**: Clear separation of concerns
- **Reusable**: Components can be shared between features
- **Testable**: Features can be tested independently
