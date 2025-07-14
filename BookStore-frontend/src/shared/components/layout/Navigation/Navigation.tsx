import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import './Navigation.css';

export interface NavigationProps {
  user: { email: string; firstName: string; lastName: string } | null;
  onLogout: () => void;
  cartItemCount?: number;
}

export function Navigation({ user, onLogout, cartItemCount = 0 }: NavigationProps) {
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const location = useLocation();

  const handleLogout = () => {
    onLogout();
  };

  const toggleUserMenu = () => {
    setIsUserMenuOpen(!isUserMenuOpen);
  };

  const toggleMobileMenu = () => {
    setIsMobileMenuOpen(!isMobileMenuOpen);
  };

  const isActive = (path: string) => {
    return location.pathname === path || 
           (path === '/dashboard' && (location.pathname === '/dashboard' || location.pathname === '/books'));
  };

  const navItems = [
    { path: '/dashboard', label: 'Books', icon: '📚' },
    { path: '/add-book', label: 'Add Book', icon: '➕' },
    { path: '/favorites', label: 'Favorites', icon: '❤️' },
    { path: '/orders', label: 'Orders', icon: '📦' },
  ];

  return (
    <>
      <nav className="navigation">
        <div className="nav-container">
          {/* Logo/Brand */}
          <div className="nav-brand">
            <Link to="/dashboard" className="nav-logo">
              📚 BookStore
            </Link>
          </div>

          {/* Desktop Navigation */}
          <div className="nav-menu">
            {user && (
              <div className="nav-links">
                {navItems.map((item) => (
                  <Link
                    key={item.path}
                    to={item.path}
                    className={`nav-link ${isActive(item.path) ? 'nav-link--active' : ''}`}
                  >
                    <span className="nav-link__icon">{item.icon}</span>
                    <span className="nav-link__label">{item.label}</span>
                  </Link>
                ))}
              </div>
            )}
          </div>

          {/* Right Side Actions */}
          <div className="nav-actions">
            {user ? (
              <>
                {/* Cart Icon */}
                <Link
                  to="/cart"
                  className="nav-action-btn"
                  aria-label="Shopping cart"
                >
                  🛒 {cartItemCount > 0 && <span className="cart-badge">{cartItemCount}</span>}
                </Link>

                {/* User Menu */}
                <div className="user-menu">
                  <button
                    onClick={toggleUserMenu}
                    className="user-menu__trigger"
                    {...(isUserMenuOpen ? { 'aria-expanded': true } : { 'aria-expanded': false })}
                    aria-haspopup="true"
                  >
                    <div className="user-avatar">
                      {user.firstName.charAt(0).toUpperCase()}
                    </div>
                    <span className="user-name">
                      {user.firstName} {user.lastName}
                    </span>
                    <svg
                      className={`user-menu__chevron ${isUserMenuOpen ? 'user-menu__chevron--open' : ''}`}
                      width="20"
                      height="20"
                      viewBox="0 0 20 20"
                      fill="currentColor"
                    >
                      <path
                        fillRule="evenodd"
                        d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
                        clipRule="evenodd"
                      />
                    </svg>
                  </button>

                  {/* User Dropdown Menu */}
                  {isUserMenuOpen && (
                    <div className="user-menu__dropdown">
                      <div className="user-menu__header">
                        <div className="user-menu__email">{user.email}</div>
                      </div>
                      <div className="user-menu__divider" />
                      <Link
                        to="/profile"
                        className="user-menu__item"
                        onClick={() => setIsUserMenuOpen(false)}
                      >
                        👤 Profile
                      </Link>
                      <Link
                        to="/settings"
                        className="user-menu__item"
                        onClick={() => setIsUserMenuOpen(false)}
                      >
                        ⚙️ Settings
                      </Link>
                      <div className="user-menu__divider" />
                      <button
                        onClick={handleLogout}
                        className="user-menu__item user-menu__item--danger"
                      >
                        🚪 Logout
                      </button>
                    </div>
                  )}
                </div>
              </>
            ) : (
              <div className="auth-actions">
                <Link to="/login" className="nav-button nav-button--ghost">
                  Login
                </Link>
                <Link to="/register" className="nav-button nav-button--primary">
                  Sign Up
                </Link>
              </div>
            )}

            {/* Mobile Menu Toggle */}
            <button
              onClick={toggleMobileMenu}
              className="mobile-menu-toggle"
              aria-label="Toggle mobile menu"
            >
              <span className="hamburger">
                <span></span>
                <span></span>
                <span></span>
              </span>
            </button>
          </div>
        </div>

        {/* Mobile Menu */}
        {isMobileMenuOpen && (
          <div className="mobile-menu">
            {user && (
              <div className="mobile-nav-links">
                {navItems.map((item) => (
                  <Link
                    key={item.path}
                    to={item.path}
                    className={`mobile-nav-link ${isActive(item.path) ? 'mobile-nav-link--active' : ''}`}
                    onClick={() => setIsMobileMenuOpen(false)}
                  >
                    <span className="mobile-nav-link__icon">{item.icon}</span>
                    <span className="mobile-nav-link__label">{item.label}</span>
                  </Link>
                ))}
              </div>
            )}
          </div>
        )}
      </nav>
    </>
  );
}

export default Navigation;
