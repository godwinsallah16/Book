# Shared

Shared/reusable code and components for the BookStore application.

## 📁 Structure

```
shared/
├── components/        # Shared components
│   ├── ui/           # Generic UI components
│   ├── layout/       # Layout components
│   ├── README.md     # Components documentation
│   └── index.ts      # Component exports
├── assets/           # Static assets
├── README.md         # This documentation
└── index.ts          # Main shared exports
```

## 📦 Contents

### 🎨 **Components**
Reusable components used across the application:

#### **UI Components** (`components/ui/`)
- **Button**: Reusable button with variants
- **Input**: Form input components
- **Card**: Card layout component
- **Modal**: Modal dialog component
- **Badge**: Status/count badge component
- **LoadingSpinner**: Loading indicator

#### **Layout Components** (`components/layout/`)
- **Navigation**: Main navigation component
- **NotFound**: 404 error page component

### 🖼️ **Assets** (`assets/`)
Static assets like images, icons, fonts (currently empty)

## 🔧 Usage

Import shared components:

```typescript
// UI Components
import { Button, Input, Card, Modal } from '../shared/components/ui';

// Layout Components
import { Navigation, NotFound } from '../shared/components/layout';

// All shared components
import { Button, Navigation } from '../shared/components';

// Or from main shared index
import { Button, Navigation } from '../shared';
```

## 🎯 Design Principles

### **UI Components**
- **Generic**: No business logic, pure UI
- **Reusable**: Can be used across all features
- **Customizable**: Props for styling and behavior
- **Consistent**: Follow design system patterns

### **Layout Components**
- **App-wide**: Used across the entire application
- **Navigation**: Handle global navigation state
- **Structure**: Provide consistent page layout

## ✅ Benefits

- **Consistency**: Shared components ensure UI consistency
- **Reusability**: Avoid code duplication
- **Maintainability**: Single source of truth for common components
- **Scalability**: Easy to add new shared components
