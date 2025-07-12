import { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { BookProvider } from './context/BookContext';
import { CartProvider } from './context/CartContext';
import { BookForm, BookSearch, Login, Register, ForgotPassword, ResetPassword, VerifyEmail, EnhancedBookList, Navigation, Favorites, NotFound, Checkout, OrderConfirmation, Orders } from './components';
import { authService } from './services/authService';
import type { Book, BookFilters } from './types/book.types';
import './App.css';

function App() {
  const [currentView, setCurrentView] = useState<'list' | 'add' | 'edit' | 'favorites'>('list');
  const [selectedBook, setSelectedBook] = useState<Book | null>(null);
  const [searchFilters, setSearchFilters] = useState<BookFilters>({});
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<{ userId: string; email: string; firstName: string; lastName: string } | null>(null);

  useEffect(() => {
    // Check if user is already authenticated
    const authenticated = authService.isAuthenticated();
    setIsAuthenticated(authenticated);
    if (authenticated) {
      setUser(authService.getCurrentUser());
    }
  }, []);

  const handleViewChange = (view: string) => {
    if (view === 'list' || view === 'add' || view === 'edit' || view === 'favorites') {
      setCurrentView(view);
    }
  };

  const handleEditBook = (book: Book) => {
    setSelectedBook(book);
    setCurrentView('edit');
  };

  const handleSearch = (filters: BookFilters) => {
    setSearchFilters(filters);
  };

  const handleFormSuccess = () => {
    setCurrentView('list');
    setSelectedBook(null);
  };

  const handleFormCancel = () => {
    setCurrentView('list');
    setSelectedBook(null);
  };

  const handleLogin = () => {
    setIsAuthenticated(true);
    setUser(authService.getCurrentUser());
  };

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setUser(null);
  };

  // Layout component for authenticated users
  const AuthenticatedLayout = ({ children }: { children: React.ReactNode }) => (
    <CartProvider>
      <BookProvider>
        <div className="App">
          <Navigation 
            user={user}
            onLogout={handleLogout}
            currentView={currentView}
            onViewChange={handleViewChange}
          />
          <main className="main-content">
            {children}
          </main>
        </div>
      </BookProvider>
    </CartProvider>
  );

  // Books page component
  const BooksPage = () => (
    <>
      <BookSearch onSearch={handleSearch} />
      <EnhancedBookList 
        onBookEdit={handleEditBook}
        filters={searchFilters}
      />
    </>
  );

  // Add/Edit Book page component
  const BookFormPage = () => (
    <BookForm 
      book={selectedBook || undefined}
      onSuccess={handleFormSuccess}
      onCancel={handleFormCancel}
    />
  );

  return (
    <Router>
      <div className="App">
        <Routes>
          {/* Public Routes */}
          <Route 
            path="/login" 
            element={
              !isAuthenticated ? (
                <Login onLogin={handleLogin} />
              ) : (
                <Navigate to="/books" replace />
              )
            } 
          />
          <Route 
            path="/register" 
            element={
              !isAuthenticated ? (
                <Register />
              ) : (
                <Navigate to="/books" replace />
              )
            } 
          />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/verify-email" element={<VerifyEmail />} />
          
          {/* Protected Routes */}
          <Route 
            path="/books" 
            element={
              isAuthenticated ? (
                <AuthenticatedLayout>
                  <BooksPage />
                </AuthenticatedLayout>
              ) : (
                <Navigate to="/login" replace />
              )
            } 
          />
          
          <Route 
            path="/add-book" 
            element={
              isAuthenticated ? (
                <AuthenticatedLayout>
                  <BookFormPage />
                </AuthenticatedLayout>
              ) : (
                <Navigate to="/login" replace />
              )
            } 
          />
          
          <Route 
            path="/favorites" 
            element={
              isAuthenticated ? (
                <AuthenticatedLayout>
                  <Favorites />
                </AuthenticatedLayout>
              ) : (
                <Navigate to="/login" replace />
              )
            } 
          />
          
          <Route 
            path="/checkout" 
            element={
              isAuthenticated ? (
                <AuthenticatedLayout>
                  <Checkout />
                </AuthenticatedLayout>
              ) : (
                <Navigate to="/login" replace />
              )
            } 
          />
          
          <Route 
            path="/orders" 
            element={
              isAuthenticated ? (
                <AuthenticatedLayout>
                  <Orders />
                </AuthenticatedLayout>
              ) : (
                <Navigate to="/login" replace />
              )
            } 
          />
          
          <Route 
            path="/order-confirmation/:orderId" 
            element={
              isAuthenticated ? (
                <AuthenticatedLayout>
                  <OrderConfirmation />
                </AuthenticatedLayout>
              ) : (
                <Navigate to="/login" replace />
              )
            } 
          />
          
          {/* Legacy route - redirect to books */}
          <Route 
            path="/dashboard" 
            element={
              <Navigate to="/books" replace />
            } 
          />
          
          {/* Default Route */}
          <Route 
            path="/" 
            element={
              <Navigate to={isAuthenticated ? "/books" : "/login"} replace />
            } 
          />
          
          {/* 404 Route - must be last */}
          <Route 
            path="*" 
            element={
              <NotFound 
                message="The page you're looking for doesn't exist."
                redirectTo={isAuthenticated ? "/books" : "/login"}
              />
            } 
          />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
