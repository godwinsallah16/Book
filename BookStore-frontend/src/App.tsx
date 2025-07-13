import { useState, useEffect } from 'react';
import { HashRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { BookProvider, CartProvider } from './context';
import { 
  LoginPage, 
  RegisterPage, 
  EmailVerificationRequired,
  DashboardPage,
  BookFormPage,
  ForgotPassword,
  ResetPassword,
  VerifyEmailPage,
  Checkout,
  OrderConfirmation,
  Orders
} from './features';
import { 
  Navigation, 
  NotFound
} from './shared/components';
import { authService } from './services/authService';
import type { Book } from './types/book.types';
import './App.css';

function App() {
  const [currentView, setCurrentView] = useState<'list' | 'favorites'>('list');
  const [selectedBook, setSelectedBook] = useState<Book | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isEmailVerified, setIsEmailVerified] = useState(false);
  const [user, setUser] = useState<{ 
    userId: string; 
    email: string; 
    firstName: string; 
    lastName: string; 
    emailConfirmed?: boolean 
  } | null>(null);

  useEffect(() => {
    // Check if user is already authenticated
    const authenticated = authService.isAuthenticated();
    setIsAuthenticated(authenticated);
    if (authenticated) {
      const currentUser = authService.getCurrentUser();
      setUser(currentUser);
      setIsEmailVerified(authService.isEmailVerified());
    }
  }, []);

  const handleViewChange = (view: string) => {
    if (view === 'list' || view === 'favorites') {
      setCurrentView(view);
    }
  };

  const handleEditBook = (book: Book) => {
    setSelectedBook(book);
  };

  const handleFormSuccess = () => {
    setSelectedBook(null);
  };

  const handleFormCancel = () => {
    setSelectedBook(null);
  };

  const handleLogin = () => {
    setIsAuthenticated(true);
    const currentUser = authService.getCurrentUser();
    setUser(currentUser);
    setIsEmailVerified(authService.isEmailVerified());
  };

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setUser(null);
    setIsEmailVerified(false);
  };

  // Layout component for authenticated users
  const AuthenticatedLayout = ({ children }: { children: React.ReactNode }) => (
    <CartProvider>
      <BookProvider>
        <div className="App">
          <Navigation 
            user={user}
            onLogout={handleLogout}
          />
          <main className="main-content">
            {children}
          </main>
        </div>
      </BookProvider>
    </CartProvider>
  );

  // Protected Route Component that checks both authentication and email verification
  const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    if (!isAuthenticated) {
      return <Navigate to="/login" replace />;
    }
    
    if (!isEmailVerified) {
      return <EmailVerificationRequired />;
    }
    
    return <AuthenticatedLayout>{children}</AuthenticatedLayout>;
  };

  return (
    <Router>
      <div className="App">
        <Routes>
          {/* Public Routes */}
          <Route 
            path="/login" 
            element={
              !isAuthenticated ? (
                <LoginPage onLogin={handleLogin} />
              ) : (
                <Navigate to="/dashboard" replace />
              )
            } 
          />
          <Route 
            path="/register" 
            element={
              !isAuthenticated ? (
                <RegisterPage />
              ) : (
                <Navigate to="/dashboard" replace />
              )
            } 
          />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/verify-email" element={<VerifyEmailPage />} />
          
          {/* Email Verification Required Route */}
          <Route 
            path="/email-verification-required" 
            element={
              isAuthenticated && !isEmailVerified ? (
                <EmailVerificationRequired />
              ) : (
                <Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />
              )
            } 
          />
          
          {/* Protected Routes */}
          <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <DashboardPage 
                  currentView={currentView}
                  onBookEdit={handleEditBook}
                  onViewChange={handleViewChange}
                />
              </ProtectedRoute>
            } 
          />
          
          <Route 
            path="/books" 
            element={
              <Navigate to="/dashboard" replace />
            } 
          />
          
          <Route 
            path="/add-book" 
            element={
              <ProtectedRoute>
                <BookFormPage 
                  onSuccess={handleFormSuccess}
                  onCancel={handleFormCancel}
                />
              </ProtectedRoute>
            } 
          />
          
          <Route 
            path="/edit-book" 
            element={
              <ProtectedRoute>
                <BookFormPage 
                  book={selectedBook || undefined}
                  onSuccess={handleFormSuccess}
                  onCancel={handleFormCancel}
                />
              </ProtectedRoute>
            } 
          />
          
          <Route 
            path="/favorites" 
            element={
              <ProtectedRoute>
                <DashboardPage 
                  currentView="favorites"
                  onBookEdit={handleEditBook}
                  onViewChange={handleViewChange}
                />
              </ProtectedRoute>
            } 
          />
          
          <Route 
            path="/checkout" 
            element={
              <ProtectedRoute>
                <Checkout />
              </ProtectedRoute>
            } 
          />
          
          <Route 
            path="/orders" 
            element={
              <ProtectedRoute>
                <Orders />
              </ProtectedRoute>
            } 
          />
          
          <Route 
            path="/order-confirmation/:orderId" 
            element={
              <ProtectedRoute>
                <OrderConfirmation />
              </ProtectedRoute>
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
                message="The page you're looking for doesn't exist."
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
