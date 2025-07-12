import { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { BookProvider } from './context/BookContext';
import { CartProvider } from './context/CartContext';
import { BookForm, BookSearch, Login, Register, ForgotPassword, ResetPassword, VerifyEmail, EnhancedBookList, Navigation, Favorites, NotFound } from './components';
import { authService } from './services/authService';
import type { Book, BookFilters } from './types/book.types';
import './App.css';

function App() {
  const [currentView, setCurrentView] = useState<'list' | 'add' | 'edit' | 'favorites'>('list');
  const [selectedBook, setSelectedBook] = useState<Book | null>(null);
  const [searchFilters, setSearchFilters] = useState<BookFilters>({});
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<{ email: string; firstName: string; lastName: string } | null>(null);

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

  const handleDeleteBook = async (book: Book) => {
    // This callback is now handled inside BookList component
    console.log('Book deleted:', book.title);
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

  // Registration no longer automatically logs in users
  // Users must verify email first

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setUser(null);
  };

  // Main App Component for authenticated users
  const MainApp = () => (
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
            {currentView === 'list' && (
              <>
                <BookSearch onSearch={handleSearch} />
                <EnhancedBookList 
                  onBookEdit={handleEditBook}
                  onBookDelete={handleDeleteBook}
                  filters={searchFilters}
                />
              </>
            )}
            
            {currentView === 'favorites' && (
              <Favorites />
            )}
            
            {(currentView === 'add' || currentView === 'edit') && (
              <BookForm 
                book={selectedBook || undefined}
                onSuccess={handleFormSuccess}
                onCancel={handleFormCancel}
              />
            )}
          </main>
        </div>
      </BookProvider>
    </CartProvider>
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
                <Navigate to="/dashboard" replace />
              )
            } 
          />
          <Route 
            path="/register" 
            element={
              !isAuthenticated ? (
                <Register />
              ) : (
                <Navigate to="/dashboard" replace />
              )
            } 
          />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/verify-email" element={<VerifyEmail />} />
          
          {/* Protected Routes */}
          <Route 
            path="/dashboard" 
            element={
              isAuthenticated ? (
                <MainApp />
              ) : (
                <Navigate to="/login" replace />
              )
            } 
          />
          
          {/* Default Route */}
          <Route 
            path="/" 
            element={
              <Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />
            } 
          />
          
          {/* 404 Route - must be last */}
          <Route 
            path="*" 
            element={
              <NotFound 
                message="The page you're looking for doesn't exist. You might have been redirected here after deleting a book or accessing an invalid URL."
                redirectTo={isAuthenticated ? "/dashboard" : "/login"}
              />
            } 
          />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
