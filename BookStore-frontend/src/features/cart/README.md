# Cart Feature

Manages the shopping cart, including adding, removing, and updating items for the BookStore frontend.

## 📁 Structure

```
cart/
├── components/        # Cart components
│   ├── ShoppingCart/  # Main shopping cart
│   └── CartIcon/      # Cart icon with item count
├── pages/             # Cart pages (if needed)
└── index.ts           # Feature exports
```

## 🔧 Components

- **ShoppingCart**: Main shopping cart component with item management
- **CartIcon**: Cart icon with item count indicator

## 📄 Pages

- Currently uses components directly in other pages
- Future: Could have dedicated cart page

## 🔗 Usage

```typescript
import { ShoppingCart, CartIcon } from './features/cart';
```

## 🎯 Features

- Add items to cart
- Update item quantities
- Remove items from cart
- View total price
- Clear entire cart
