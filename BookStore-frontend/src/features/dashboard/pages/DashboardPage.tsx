import React, { useState } from 'react';
import BookForm from '../../books/components/BookForm/BookForm';
import { authService } from '../../../services';
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
  const [searchFilters, setSearchFilters] = useState<BookFilters>({});
  const [showBookForm, setShowBookForm] = useState(false);
  const [formKey, setFormKey] = useState(0); // To reset form
  const currentUser = authService.getCurrentUser();

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
        {currentUser && currentView === 'list' && (
          <button
            className="add-book-btn"
            onClick={() => { setShowBookForm(true); setFormKey(formKey + 1); }}
            style={{ marginLeft: '1rem' }}
          >
            + Add Book
          </button>
        )}
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
                    key={formKey}
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
