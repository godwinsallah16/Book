# useCart Hook

Custom React hook for shopping cart operations.

## 📁 Structure

```
useCart/
├── useCart.ts        # Main hook implementation
└── index.ts          # Hook exports
```

## 🔧 Features

This hook provides functionality for:
- Accessing cart context
- Cart state management
- Error handling for cart operations

## 🔗 Usage

```typescript
import { useCart } from '../hooks/useCart';

function CartComponent() {
  const {
    state,
    fetchCart,
    addToCart,
    updateCartItem,
    removeFromCart,
    clearCart
  } = useCart();

  // Access cart state and methods
  const { items, totalItems, totalPrice, isLoading, error } = state;
}
```

## 📊 Cart State

The hook provides access to:
- `items`: Array of cart items
- `totalItems`: Total number of items in cart
- `totalPrice`: Total price of all items
- `isLoading`: Loading state
- `error`: Error messages

## 🎯 Methods

- `fetchCart()`: Fetch current cart
- `addToCart(bookId, quantity)`: Add item to cart
- `updateCartItem(bookId, quantity)`: Update item quantity
- `removeFromCart(bookId)`: Remove item from cart
- `clearCart()`: Clear entire cart

## ⚠️ Requirements

This hook must be used within a `CartProvider` context.
