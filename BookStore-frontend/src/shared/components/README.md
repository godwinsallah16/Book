# Shared Components

Reusable components used across the BookStore application.

## 📁 Structure

```
components/
├── ui/              # Generic UI components
│   ├── Button/      # Button component with variants
│   ├── Input/       # Form input components
│   ├── Card/        # Card layout component
│   ├── Modal/       # Modal dialog component
│   ├── Badge/       # Status/count badge
│   ├── LoadingSpinner/ # Loading indicator
│   ├── README.md    # UI components docs
│   └── index.ts     # UI exports
├── layout/          # Layout components
│   ├── Navigation/  # Main navigation
│   ├── NotFound/    # 404 page
│   ├── README.md    # Layout components docs
│   └── index.ts     # Layout exports
├── README.md        # This documentation
└── index.ts         # All components export
```

## 🎨 UI Components

### **Button**
Reusable button component with multiple variants:
- Primary, Secondary, Outline, Ghost, Danger
- Small, Medium, Large sizes
- Loading and disabled states

### **Input**
Form input components:
- Text inputs with validation
- Password inputs with visibility toggle
- Consistent styling and error states

### **Card**
Layout card component:
- Consistent padding and borders
- Header, body, footer sections
- Responsive design

### **Modal**
Modal dialog component:
- Overlay backdrop
- Close on escape/outside click
- Customizable content

### **Badge**
Status and count badges:
- Different color variants
- Small, compact display
- Used for notifications, counts

### **LoadingSpinner**
Loading indicator:
- Consistent loading animation
- Multiple sizes
- Overlay and inline variants

## 🏗️ Layout Components

### **Navigation**
Main application navigation:
- User authentication state
- Navigation menu
- Responsive design
- Shopping cart integration

### **NotFound**
404 error page component:
- Friendly error message
- Navigation back to main areas
- Consistent with app design

## 🔧 Usage Examples

```typescript
// UI Components
import { Button, Input, Card } from './ui';

// Layout Components
import { Navigation, NotFound } from './layout';

// All components
import { Button, Navigation } from './shared/components';

// Example usage
<Button variant="primary" size="lg" onClick={handleClick}>
  Save Changes
</Button>

<Card>
  <Card.Header>Title</Card.Header>
  <Card.Body>Content</Card.Body>
</Card>
```

## 🎯 Design Principles

1. **Generic**: No business logic, pure UI
2. **Reusable**: Used across all features
3. **Consistent**: Follow design system
4. **Accessible**: ARIA labels and keyboard navigation
5. **Responsive**: Work on all screen sizes

## ✅ Benefits

- **Consistency**: Uniform UI across the app
- **Maintainability**: Single source of truth
- **Development Speed**: Pre-built components
- **Quality**: Well-tested, accessible components
