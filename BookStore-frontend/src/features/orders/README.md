# Orders Feature

Handles order creation, confirmation, history, and management for the BookStore frontend.

## 📁 Structure

```
orders/
├── components/        # Order components
│   ├── Checkout/      # Checkout process
│   ├── Orders/        # Order history and listing
│   └── OrderConfirmation/ # Order confirmation
├── pages/             # Order pages (if needed)
└── index.ts           # Feature exports
```

## 🔧 Components

- **Checkout**: Complete checkout process
- **Orders**: Order history and management
- **OrderConfirmation**: Order confirmation display

## 📄 Pages

- Currently uses components directly in routes
- Future: Could have dedicated order management pages

## 🔗 Usage

```typescript
import { Checkout, Orders, OrderConfirmation } from './features/orders';
```

## 🎯 Features

- Process checkout
- View order history
- Order confirmation
- Order status tracking
