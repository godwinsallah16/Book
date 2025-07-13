import React, { useState } from 'react';
import { EnhancedBookList, BookSearch, Favorites } from '../../';
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
