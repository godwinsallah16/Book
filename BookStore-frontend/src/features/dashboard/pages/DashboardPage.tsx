import React, { useState } from 'react';
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../../../services';
import BookForm from '../../books/components/BookForm/BookForm';
import EnhancedBookList from '../components/EnhancedBookList';
import BookSearch from '../components/BookSearch';
import Favorites from '../components/Favorites';
import type { Book, BookFilters } from '../../../types/book.types';
import './DashboardPage.css';

interface DashboardPageProps {
  currentView: 'list' | 'favorites';
  onBookEdit: (book: Book) => void;
  onViewChange: (view: string) => void;
}

const DashboardPage: React.FC<DashboardPageProps> = ({ 
  currentView, 
  onBookEdit, 
  onViewChange 
}) => {
  const navigate = useNavigate();

  useEffect(() => {
    // Always fetch latest user status from backend
    const fetchUser = async () => {
      const updatedUser = await authService.fetchCurrentUser();
      if (updatedUser && !updatedUser.emailConfirmed) {
        navigate('/email-verification-required', { state: { userEmail: updatedUser.email } });
      }
    };
    fetchUser();
  }, [navigate]);
  const [searchFilters, setSearchFilters] = useState<BookFilters>({});
  const [showBookForm, setShowBookForm] = useState(false);

  const handleSearch = (filters: BookFilters) => {
    setSearchFilters(filters);
  };

  return (
    <div className="dashboard-page">
      <div className="dashboard-header">
        <h1>
          {currentView === 'favorites' ? 'My Favorites' : 'Book Library'}
        </h1>
        <div className="view-toggles">
          <button
            className={`view-toggle ${currentView === 'list' ? 'active' : ''}`}
            onClick={() => onViewChange('list')}
          >
            All Books
          </button>
          <button
            className={`view-toggle ${currentView === 'favorites' ? 'active' : ''}`}
            onClick={() => onViewChange('favorites')}
          >
            Favorites
          </button>
        </div>
        {/* Add Book button is only in one place in the header, not duplicated elsewhere */}
      </div>

      <div className="dashboard-content">
        {currentView === 'list' ? (
          <>
            <div className="search-section">
              <BookSearch onSearch={handleSearch} />
            </div>
            <div className="books-section">
              <EnhancedBookList 
                onBookEdit={onBookEdit}
                filters={searchFilters}
              />
            </div>
            {showBookForm && (
              <div className="modal-overlay">
                <div className="modal-content">
                  <BookForm
                    onCancel={() => setShowBookForm(false)}
                    onSuccess={() => {
                      setShowBookForm(false);
                      // Trigger EnhancedBookList to reload books
                      setSearchFilters({ ...searchFilters });
                    }}
                  />
                </div>
              </div>
            )}
          </>
        ) : (
          <div className="favorites-section">
            <Favorites />
          </div>
        )}
      </div>
    </div>
  );
};

export default DashboardPage;
