# BookStore UI Component Library

This directory contains reusable UI components for the BookStore application built with React and TypeScript.

## Components

### Button
A flexible button component with multiple variants, sizes, and states.

**Props:**
- `variant`: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger' (default: 'primary')
- `size`: 'sm' | 'md' | 'lg' (default: 'md')
- `loading`: boolean (default: false)
- `leftIcon`: React.ReactNode
- `rightIcon`: React.ReactNode
- `fullWidth`: boolean (default: false)

**Example:**
```tsx
<Button variant="primary" size="lg" loading={loading}>
  Save Changes
</Button>
```

### Input
A customizable input field with labels, icons, and validation states.

**Props:**
- `label`: string
- `error`: string
- `hint`: string
- `leftIcon`: React.ReactNode
- `rightIcon`: React.ReactNode
- `variant`: 'default' | 'search' (default: 'default')
- `fullWidth`: boolean (default: false)

**Example:**
```tsx
<Input
  label="Email"
  type="email"
  placeholder="Enter your email"
  leftIcon={<EmailIcon />}
  error={errors.email}
  fullWidth
/>
```

### Card
A versatile container component with multiple styles and sections.

**Props:**
- `variant`: 'default' | 'elevated' | 'outlined' | 'filled' (default: 'default')
- `padding`: 'none' | 'sm' | 'md' | 'lg' (default: 'md')
- `clickable`: boolean (default: false)
- `header`: React.ReactNode
- `footer`: React.ReactNode

**Example:**
```tsx
<Card 
  variant="elevated" 
  padding="lg"
  header={<h2>Card Title</h2>}
  footer={<Button>Action</Button>}
>
  Card content goes here
</Card>
```

### Modal
A full-featured modal dialog with backdrop, keyboard support, and focus management.

**Props:**
- `isOpen`: boolean
- `onClose`: () => void
- `title`: string
- `size`: 'sm' | 'md' | 'lg' | 'xl' | 'full' (default: 'md')
- `closeOnBackdropClick`: boolean (default: true)
- `closeOnEscape`: boolean (default: true)
- `showCloseButton`: boolean (default: true)

**Example:**
```tsx
<Modal
  isOpen={isOpen}
  onClose={() => setIsOpen(false)}
  title="Confirm Action"
  size="md"
>
  <p>Are you sure you want to continue?</p>
  <div className="flex gap-2 justify-end">
    <Button variant="outline" onClick={() => setIsOpen(false)}>Cancel</Button>
    <Button variant="primary" onClick={handleConfirm}>Confirm</Button>
  </div>
</Modal>
```

### Badge
A small status indicator with multiple variants and sizes.

**Props:**
- `variant`: 'default' | 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'info' (default: 'default')
- `size`: 'sm' | 'md' | 'lg' (default: 'md')
- `dot`: boolean (default: false)

**Example:**
```tsx
<Badge variant="success" size="sm">Active</Badge>
<Badge variant="warning" dot />
```

## Design System

The components use CSS custom properties (CSS variables) for consistent theming:

### Colors
- Primary: `--color-primary` (#6366f1)
- Secondary: `--color-secondary` (#6b7280)
- Success: `--color-success` (#10b981)
- Warning: `--color-warning` (#f59e0b)
- Danger: `--color-danger` (#ef4444)
- Info: `--color-info` (#06b6d4)

### Spacing
- XS: `--spacing-xs` (0.25rem)
- SM: `--spacing-sm` (0.5rem)
- MD: `--spacing-md` (1rem)
- LG: `--spacing-lg` (1.5rem)
- XL: `--spacing-xl` (2rem)
- 2XL: `--spacing-2xl` (3rem)

### Border Radius
- SM: `--border-radius-sm` (0.25rem)
- MD: `--border-radius-md` (0.5rem)
- LG: `--border-radius-lg` (0.75rem)
- XL: `--border-radius-xl` (1rem)
- Full: `--border-radius-full` (9999px)

### Typography
- Font sizes from `--font-size-xs` (0.75rem) to `--font-size-4xl` (2.25rem)
- Font weights: normal (400), medium (500), semibold (600), bold (700)

## Usage

Import components from the UI directory:

```tsx
import { Button, Input, Card, Modal, Badge } from '../components/UI';
```

All components are built with accessibility in mind and include:
- Proper ARIA attributes
- Keyboard navigation support
- Focus management
- Screen reader compatibility

## Development

To see all components in action, visit the `/showcase` page (available when logged in) to explore the complete component library with examples and documentation.
