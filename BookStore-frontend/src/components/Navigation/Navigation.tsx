import { useState } from 'react';
import { CartIcon } from '../ShoppingCart/CartIcon';
import { ShoppingCart } from '../ShoppingCart/ShoppingCart';
import { authService } from '../../services/authService';
import './Navigation.css';

interface NavigationProps {
  user: { email: string; firstName: string; lastName: string } | null;
  onLogout: () => void;
  currentView: string;
  onViewChange: (view: string) => void;
}

export function Navigation({ user, onLogout, currentView, onViewChange }: NavigationProps) {
  const [isCartOpen, setIsCartOpen] = useState(false);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);

  const handleLogout = () => {
    authService.logout();
    onLogout();
  };

  const toggleCart = () => {
    setIsCartOpen(!isCartOpen);
  };

  const toggleUserMenu = () => {
    setIsUserMenuOpen(!isUserMenuOpen);
  };

  return (
    <>
      <nav className="navbar">
        <div className="navbar-container">
          <div className="navbar-brand">
            <h1>BookStore</h1>
          </div>

          <div className="navbar-nav">
            <button
              className={`nav-link ${currentView === 'list' ? 'active' : ''}`}
              onClick={() => onViewChange('list')}
            >
              Books
            </button>
            <button
              className={`nav-link ${currentView === 'add' ? 'active' : ''}`}
              onClick={() => onViewChange('add')}
            >
              Add Book
            </button>
            <button
              className={`nav-link ${currentView === 'favorites' ? 'active' : ''}`}
              onClick={() => onViewChange('favorites')}
            >
              Favorites
            </button>
            <button
              className={`nav-link ${currentView === 'profile' ? 'active' : ''}`}
              onClick={() => onViewChange('profile')}
            >
              Profile
            </button>
          </div>

          <div className="navbar-actions">
            <CartIcon onClick={toggleCart} />
            
            <div className="user-menu">
              <button className="user-menu-toggle" onClick={toggleUserMenu}>
                <div className="user-avatar">
                  {user?.firstName?.[0]?.toUpperCase() || 'U'}
                </div>
                <span className="user-name">{user?.firstName || 'User'}</span>
                <svg
                  className={`dropdown-icon ${isUserMenuOpen ? 'open' : ''}`}
                  width="16"
                  height="16"
                  viewBox="0 0 16 16"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M4 6L8 10L12 6"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
              </button>
              
              {isUserMenuOpen && (
                <div className="user-dropdown">
                  <div className="user-info">
                    <p className="user-email">{user?.email}</p>
                    <p className="user-full-name">{user?.firstName} {user?.lastName}</p>
                  </div>
                  <hr />
                  <button
                    className="dropdown-item"
                    onClick={() => {
                      onViewChange('profile');
                      setIsUserMenuOpen(false);
                    }}
                  >
                    Profile Settings
                  </button>
                  <button
                    className="dropdown-item"
                    onClick={() => {
                      onViewChange('orders');
                      setIsUserMenuOpen(false);
                    }}
                  >
                    My Orders
                  </button>
                  <hr />
                  <button className="dropdown-item logout" onClick={handleLogout}>
                    Logout
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      </nav>

      <ShoppingCart isOpen={isCartOpen} onClose={() => setIsCartOpen(false)} />
    </>
  );
}
