# Context Organization

This directory contains React Context providers following a consistent structure.

## 📁 Structure

Each context provider is organized in its own folder:

```
context/
├── BookContext/
│   ├── BookContext.tsx    # Context implementation
│   └── index.ts          # Exports
├── CartContext/
│   ├── CartContext.tsx    # Context implementation
│   └── index.ts          # Exports
└── index.ts              # Main context exports
```

## 📋 Naming Convention

- **Folder Name**: `[ContextName]Context/`
- **File Name**: `[ContextName]Context.tsx`
- **Provider Name**: `[ContextName]Provider`
- **Context Name**: `[ContextName]Context`

## 🏗️ Structure Pattern

Each context folder should contain:

1. **`[Context]Context.tsx`** - Main implementation file with:
   - State interface
   - Action types (if using reducer)
   - Context interface
   - Context creation
   - Provider component
   - Custom hook (optional)

2. **`index.ts`** - Export file with:
   - Provider export
   - Context export
   - Type exports

## 📦 Usage

Import contexts from the main context index:

```typescript
import { BookProvider, CartProvider } from './context';
```

Or from specific context:

```typescript
import { BookProvider } from './context/BookContext';
import { CartProvider } from './context/CartContext';
```

## ✅ Benefits

- **Consistency**: All contexts follow the same structure
- **Maintainability**: Easy to find and modify context files
- **Scalability**: Easy to add new contexts
- **Clean Imports**: Barrel exports provide clean import paths
