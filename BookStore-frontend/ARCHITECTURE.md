# Frontend Architecture Guide

## 📁 Folder Structure

The frontend follows a **feature-based architecture** with clear separation of concerns:

```
src/
├── 📁 features/              # Feature-based modules
│   ├── 📁 auth/              # Authentication feature
│   │   ├── 📁 components/    # Auth-specific components
│   │   ├── 📁 pages/         # Auth pages (Login, Register, etc.)
│   │   └── index.ts          # Feature exports
│   ├── 📁 books/             # Books management feature
│   │   ├── 📁 components/    # Book-related components
│   │   ├── 📁 pages/         # Book pages (BookForm, etc.)
│   │   └── index.ts          # Feature exports
│   ├── 📁 cart/              # Shopping cart feature
│   │   ├── 📁 components/    # Cart components
│   │   └── index.ts          # Feature exports
│   ├── 📁 orders/            # Order management feature
│   │   ├── 📁 components/    # Order components (Checkout, etc.)
│   │   └── index.ts          # Feature exports
│   ├── 📁 dashboard/         # Dashboard feature
│   │   ├── DashboardPage.tsx # Main dashboard page
│   │   └── index.ts          # Feature exports
│   └── index.ts              # Main features export
├── 📁 shared/                # Shared/reusable code
│   ├── 📁 components/        # Shared components
│   │   ├── 📁 ui/            # Generic UI components (Button, Input, etc.)
│   │   ├── 📁 layout/        # Layout components (Navigation, etc.)
│   │   └── index.ts          # Shared components exports
│   ├── 📁 assets/            # Static assets
│   └── index.ts              # Shared exports
├── 📁 context/               # React Context providers
├── 📁 hooks/                 # Custom React hooks
├── 📁 services/              # API service layer
├── 📁 types/                 # TypeScript type definitions
├── 📁 utils/                 # Utility functions
├── App.tsx                   # Main application component
└── main.tsx                  # Application entry point
```

## 🏗️ Architecture Principles

### 1. **Feature-Based Organization**
- Each feature is self-contained with its own components, pages, and logic
- Features can be easily added, removed, or modified
- Clear boundaries between different business domains

### 2. **Separation of Concerns**
- **Features**: Business logic and feature-specific components
- **Shared**: Reusable components and utilities
- **Services**: API communication layer
- **Context**: Global state management
- **Types**: TypeScript definitions

### 3. **Import Hierarchy**
```typescript
// ✅ Good: Features can import from shared
import { Button } from '../../shared/components/ui';

// ✅ Good: Features can import from services/utils
import { authService } from '../../services/authService';

// ❌ Avoid: Features importing from other features
import { BookList } from '../books/components'; // Avoid this

// ✅ Better: Use shared components or lift to shared
import { BookList } from '../../shared/components';
```

## 📦 Feature Structure

Each feature follows a consistent structure:

```
features/[feature-name]/
├── components/           # Feature-specific components
│   ├── ComponentA/
│   │   ├── ComponentA.tsx
│   │   ├── ComponentA.css
│   │   └── index.ts
│   └── index.ts         # Export all components
├── pages/               # Feature pages
│   ├── PageA.tsx
│   └── index.ts
├── hooks/               # Feature-specific hooks (optional)
├── types/               # Feature-specific types (optional)
└── index.ts            # Main feature export
```

## 🔄 Import/Export Patterns

### Feature Exports (`features/[feature]/index.ts`)
```typescript
// Export pages
export { default as LoginPage } from './pages/LoginPage';

// Export components
export * from './components';
```

### Main Features Export (`features/index.ts`)
```typescript
// Page exports
export { LoginPage, RegisterPage } from './auth';
export { BookFormPage } from './books';

// Component exports
export * from './auth/components';
export * from './orders/components';
```

### Component Usage in App.tsx
```typescript
import { 
  LoginPage,
  RegisterPage,
  ForgotPassword,
  Checkout
} from './features';

import { 
  Navigation,
  NotFound
} from './shared/components';
```

## 🎯 Component Categories

### 1. **UI Components** (`shared/components/ui/`)
Generic, reusable UI components:
- Button, Input, Card, Modal, Badge, LoadingSpinner
- No business logic
- Highly reusable
- Well-documented props

### 2. **Layout Components** (`shared/components/layout/`)
Layout and navigation components:
- Navigation, Header, Footer, NotFound
- App-wide layout logic
- Navigation state management

### 3. **Feature Components** (`features/[feature]/components/`)
Business logic components:
- Feature-specific functionality
- Can use shared UI components
- Contain business rules and API calls

### 4. **Page Components** (`features/[feature]/pages/`)
Route-level components:
- Compose feature components
- Handle route-specific logic
- Connect to contexts and services

## 🔧 Best Practices

### 1. **Component Organization**
```typescript
// ComponentName/
├── ComponentName.tsx     # Main component
├── ComponentName.css     # Styles
├── ComponentName.test.tsx # Tests (optional)
└── index.ts             # Export
```

### 2. **Index Files**
Always use index files for clean imports:
```typescript
// features/auth/components/index.ts
export { Login } from './Login';
export { Register } from './Register';
```

### 3. **Type Definitions**
Keep types close to usage:
```typescript
// For feature-specific types
features/auth/types/auth.types.ts

// For shared types
types/shared.types.ts
```

### 4. **Barrel Exports**
Use barrel exports for clean imports:
```typescript
// Instead of
import { LoginPage } from './features/auth/pages/LoginPage';
import { RegisterPage } from './features/auth/pages/RegisterPage';

// Use
import { LoginPage, RegisterPage } from './features';
```

## 🚀 Benefits

### 1. **Scalability**
- Easy to add new features
- Clear separation of concerns
- Modular architecture

### 2. **Maintainability**
- Feature isolation
- Predictable file locations
- Consistent patterns

### 3. **Developer Experience**
- Clear import paths
- Easy navigation
- Logical grouping

### 4. **Team Collaboration**
- Clear ownership boundaries
- Reduced merge conflicts
- Consistent code organization

## 📚 Migration Guide

When adding new features:

1. **Create feature directory**: `features/new-feature/`
2. **Add components**: `features/new-feature/components/`
3. **Add pages**: `features/new-feature/pages/`
4. **Create exports**: `features/new-feature/index.ts`
5. **Update main export**: `features/index.ts`
6. **Import in App.tsx**: `import { NewComponent } from './features'`

This architecture ensures the codebase remains organized, scalable, and maintainable as the application grows.
