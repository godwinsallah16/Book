# BookStore Frontend

Modern React frontend application for the BookStore platform.

## 🚀 Quick Start

```bash
npm install
npm run dev
```

The application will be available at `http://localhost:5173`

## 📦 Built With

- **React 19** - UI library
- **TypeScript** - Type safety
- **Vite** - Fast build tool
- **React Router v7** - Client-side routing
- **Axios** - HTTP client
- **Context API** - State management

## 🏗️ Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── Auth/           # Authentication components
│   ├── BookForm/       # Book form components
│   ├── BookList/       # Book listing components
│   ├── Navigation/     # Navigation components
│   └── UI/             # Generic UI components
├── pages/              # Page components
│   ├── Auth/           # Authentication pages
│   ├── Books/          # Book-related pages
│   └── Dashboard/      # Dashboard pages
├── context/            # React Context providers
├── hooks/              # Custom React hooks
├── services/           # API service layer
├── types/              # TypeScript type definitions
├── utils/              # Utility functions
└── assets/             # Static assets
```

---

# Frontend Architecture Guide

## 📁 Folder Structure

The frontend follows a **feature-based architecture** with clear separation of concerns:

```
src/
├── features/              # Feature-based modules
│   ├── auth/              # Authentication feature
│   │   ├── components/    # Auth-specific components
│   │   ├── pages/         # Auth pages (Login, Register, etc.)
│   │   └── index.ts       # Feature exports
│   ├── books/             # Books management feature
│   │   ├── components/    # Book-related components
│   │   ├── pages/         # Book pages (BookForm, etc.)
│   │   └── index.ts       # Feature exports
│   ├── cart/              # Shopping cart feature
│   │   ├── components/    # Cart components
│   │   └── index.ts       # Feature exports
│   ├── orders/            # Order management feature
│   │   ├── components/    # Order components (Checkout, etc.)
│   │   └── index.ts       # Feature exports
│   ├── dashboard/         # Dashboard feature
│   │   ├── DashboardPage.tsx # Main dashboard page
│   │   └── index.ts       # Feature exports
│   └── index.ts           # Main features export
├── shared/                # Shared/reusable code
│   ├── components/        # Shared components
│   │   ├── ui/            # Generic UI components (Button, Input, etc.)
│   │   ├── layout/        # Layout components (Navigation, etc.)
│   │   └── index.ts       # Shared components exports
│   ├── assets/            # Static assets
│   └── index.ts           # Shared exports
├── context/               # React Context providers
├── hooks/                 # Custom React hooks
├── services/              # API service layer
├── types/                 # TypeScript type definitions
├── utils/                 # Utility functions
├── App.tsx                # Main application component
└── main.tsx               # Application entry point
```

## 🏗️ Architecture Principles

### 1. **Feature-Based Organization**
- Each feature is self-contained with its own components, pages, and logic
- Features can be easily added, removed, or modified
- Clear boundaries between different business domains

---

## 🛠️ Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run build:prod` - Build with production optimizations
- `npm run preview` - Preview production build locally
- `npm run lint` - Run ESLint
- `npm run lint:fix` - Fix ESLint issues
- `npm run type-check` - Run TypeScript type checking
- `npm run format` - Format code with Prettier
- `npm run format:check` - Check code formatting
- `npm run clean` - Clean build artifacts

## 🔧 Configuration

### Environment Variables

Create a `.env` file in the root directory:

```env
VITE_API_URL=http://localhost:5000/api
VITE_APP_NAME=BookStore
```

### Development

The app connects to the backend API running on `http://localhost:5000` by default.

### Production

For production builds, update the `VITE_API_URL` to point to your production API.

## 🎨 Code Style

This project uses ESLint and Prettier for code formatting and linting:

- ESLint configuration is in `eslint.config.js`
- Prettier configuration is in `.prettierrc`

## 🚀 Deployment

### Build for Production

```bash
npm run build:prod
```

The build artifacts will be stored in the `dist/` directory.

### Docker

```bash
docker build -t bookstore-frontend .
docker run -p 80:80 bookstore-frontend
```

### Static Hosting

The built app can be deployed to any static hosting service like:
- Vercel
- Netlify
- AWS S3 + CloudFront
- GitHub Pages

## 🧪 Testing

### Type Checking

```bash
npm run type-check
```

### Linting

```bash
npm run lint
```

## 📱 Features

- **Authentication** - Login, register, email verification
- **Book Management** - CRUD operations for books
- **Shopping Cart** - Add/remove books, checkout
- **Favorites** - Save favorite books
- **Order History** - View past orders
- **Responsive Design** - Works on all devices

## 🔗 API Integration

The frontend communicates with the BookStore.API through RESTful endpoints:

- Authentication: `/api/auth/*`
- Books: `/api/books/*`
- Cart: `/api/cart/*`
- Orders: `/api/orders/*`
- Favorites: `/api/favorites/*`

## 🤝 Contributing

1. Follow the existing code style
2. Use TypeScript types for all new code
3. Write descriptive commit messages
4. Test your changes thoroughly
5. Update documentation as needed
