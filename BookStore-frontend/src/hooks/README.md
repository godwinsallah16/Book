# Hooks

Custom React hooks for the BookStore application.

## 📁 Structure

```
hooks/
├── useBooks/         # Book-related operations
│   ├── useBooks.ts   # Hook implementation
│   ├── README.md     # Documentation
│   └── index.ts      # Exports
├── useCart/          # Cart operations
│   ├── useCart.ts    # Hook implementation
│   ├── README.md     # Documentation
│   └── index.ts      # Exports
├── README.md         # This file
└── index.ts          # Main hooks exports
```

## 🪝 Available Hooks

### 📚 **useBooks**
- Book listing and pagination
- Book search and filtering
- Book CRUD operations
- Loading and error states

### 🛒 **useCart**
- Shopping cart state access
- Cart operations (add, update, remove)
- Must be used within CartProvider

## 🔗 Usage

Import hooks from the main hooks index:

```typescript
import { useBooks, useCart } from './hooks';
```

Or import specific hooks:

```typescript
import { useBooks } from './hooks/useBooks';
import { useCart } from './hooks/useCart';
```

## 🏗️ Hook Pattern

Each hook follows this structure:

```typescript
// Custom hook
export function useFeature() {
  // State management
  // Side effects
  // Methods
  
  return {
    // State
    // Methods
  };
}
```

## ✅ Benefits

- **Reusable Logic**: Share stateful logic between components
- **Clean Components**: Keep components focused on rendering
- **Testable**: Hooks can be tested independently
- **Type Safe**: Full TypeScript support
- **Consistent**: Standardized patterns across the app

## 📋 Best Practices

1. **Naming**: Use `use` prefix for all custom hooks
2. **Single Responsibility**: Each hook should have one clear purpose
3. **Error Handling**: Include proper error states
4. **Loading States**: Provide loading indicators
5. **Documentation**: Document parameters and return values
