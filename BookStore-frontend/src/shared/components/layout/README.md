# Layout Components

Layout components for the BookStore application that handle page structure and navigation.

## 📐 Available Components

### **Navigation** (`Navigation/`)
Main navigation component with user authentication state.

**Features:**
- Responsive navigation bar
- User authentication display
- Shopping cart badge
- Mobile menu support

**Props:**
- `user`: User object or null
- `cartItemCount`: number
- `onLogout`: () => void

**Usage:**
```typescript
<Navigation 
  user={currentUser} 
  cartItemCount={cartItems.length}
  onLogout={handleLogout}
/>
```

### **Header** (`Header/`)
Page header component with title and breadcrumbs.

**Props:**
- `title`: string
- `breadcrumbs`: BreadcrumbItem[]
- `actions`: React.ReactNode (optional)

**Usage:**
```typescript
<Header 
  title="Book Details"
  breadcrumbs={[
    { label: 'Home', href: '/' },
    { label: 'Books', href: '/books' },
    { label: book.title }
  ]}
  actions={<Button>Edit Book</Button>}
/>
```

### **Footer** (`Footer/`)
Application footer with links and copyright.

**Features:**
- Company information
- Quick links
- Social media links
- Copyright notice

**Usage:**
```typescript
<Footer />
```

### **Sidebar** (`Sidebar/`)
Sidebar navigation for dashboard and admin pages.

**Props:**
- `isOpen`: boolean
- `onClose`: () => void
- `items`: SidebarItem[]

**Usage:**
```typescript
<Sidebar 
  isOpen={sidebarOpen}
  onClose={() => setSidebarOpen(false)}
  items={navigationItems}
/>
```

### **Container** (`Container/`)
Page container with consistent padding and max-width.

**Props:**
- `maxWidth`: 'sm' | 'md' | 'lg' | 'xl' | 'full'
- `padding`: 'none' | 'sm' | 'md' | 'lg'

**Usage:**
```typescript
<Container maxWidth="lg" padding="md">
  <h1>Page Content</h1>
</Container>
```

### **Section** (`Section/`)
Content section with optional background and spacing.

**Props:**
- `background`: 'default' | 'primary' | 'secondary'
- `spacing`: 'none' | 'sm' | 'md' | 'lg'

**Usage:**
```typescript
<Section background="primary" spacing="lg">
  <h2>Featured Books</h2>
  <BookGrid books={featuredBooks} />
</Section>
```

## 🎯 Layout Patterns

### **Page Layout**
Standard page structure:
```typescript
<Navigation user={user} cartItemCount={cartCount} />
<Header title="Page Title" breadcrumbs={breadcrumbs} />
<Container maxWidth="lg">
  <main>
    {/* Page content */}
  </main>
</Container>
<Footer />
```

### **Dashboard Layout**
Admin/user dashboard structure:
```typescript
<Navigation user={user} />
<div className="dashboard-layout">
  <Sidebar items={sidebarItems} />
  <main className="dashboard-main">
    <Header title="Dashboard" />
    <Container>
      {/* Dashboard content */}
    </Container>
  </main>
</div>
```

### **Modal Layout**
Modal content structure:
```typescript
<Modal isOpen={isOpen} onClose={onClose}>
  <Header title="Modal Title" />
  <Section spacing="md">
    {/* Modal content */}
  </Section>
  <footer className="modal-actions">
    <Button variant="secondary" onClick={onClose}>Cancel</Button>
    <Button variant="primary" onClick={onSave}>Save</Button>
  </footer>
</Modal>
```

## 📱 Responsive Design

Layout components are built with mobile-first responsive design:

- **Mobile**: Single column, collapsible navigation
- **Tablet**: Two column layouts, sidebar toggle
- **Desktop**: Multi-column layouts, persistent sidebar

## ♿ Accessibility

All layout components include:
- Semantic HTML structure
- ARIA landmarks and navigation
- Skip links for keyboard users
- Focus management
- Screen reader compatibility

## 🎨 Theming

Layout components support:
- CSS custom properties for colors
- Consistent spacing system
- Responsive breakpoints
- Dark/light theme switching
