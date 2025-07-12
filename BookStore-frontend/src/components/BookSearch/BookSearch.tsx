import { useState, useEffect } from 'react';
import { useBooks } from '../../hooks';
import type { BookFilters } from '../../types/book.types';
import './BookSearch.css';

interface BookSearchProps {
  onSearch: (filters: BookFilters) => void;
}

const BookSearch: React.FC<BookSearchProps> = ({ onSearch }) => {
  const { state, fetchCategories, fetchAuthors } = useBooks();
  const { categories, authors } = state;
  
  const [filters, setFilters] = useState<BookFilters>({
    search: '',
    category: '',
    author: '',
    minPrice: undefined,
    maxPrice: undefined,
    inStock: undefined,
  });

  useEffect(() => {
    fetchCategories();
    fetchAuthors();
  }, [fetchCategories, fetchAuthors]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFilters(prev => ({
      ...prev,
      [name]: value === '' ? undefined : (name === 'minPrice' || name === 'maxPrice') ? Number(value) : value
    }));
  };

  const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, checked } = e.target;
    setFilters(prev => ({
      ...prev,
      [name]: checked ? true : undefined
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch(filters);
  };

  const handleClear = () => {
    const clearedFilters = {
      search: '',
      category: '',
      author: '',
      minPrice: undefined,
      maxPrice: undefined,
      inStock: undefined,
    };
    setFilters(clearedFilters);
    onSearch(clearedFilters);
  };

  return (
    <div className="book-search">
      <form onSubmit={handleSubmit} className="search-form">
        <div className="search-row">
          <div className="search-group">
            <label htmlFor="search">Search Books</label>
            <input
              type="text"
              id="search"
              name="search"
              value={filters.search || ''}
              onChange={handleInputChange}
              placeholder="Search by title, author, or ISBN..."
              className="search-input"
            />
          </div>
          
          <div className="search-group">
            <label htmlFor="category">Category</label>
            <select
              id="category"
              name="category"
              value={filters.category || ''}
              onChange={handleInputChange}
              className="search-select"
            >
              <option value="">All Categories</option>
              {categories.map(cat => (
                <option key={cat} value={cat}>{cat}</option>
              ))}
            </select>
          </div>
          
          <div className="search-group">
            <label htmlFor="author">Author</label>
            <select
              id="author"
              name="author"
              value={filters.author || ''}
              onChange={handleInputChange}
              className="search-select"
            >
              <option value="">All Authors</option>
              {authors.map(author => (
                <option key={author} value={author}>{author}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="search-row">
          <div className="search-group">
            <label htmlFor="minPrice">Min Price ($)</label>
            <input
              type="number"
              id="minPrice"
              name="minPrice"
              value={filters.minPrice || ''}
              onChange={handleInputChange}
              placeholder="0.00"
              min="0"
              step="0.01"
              className="search-input"
            />
          </div>
          
          <div className="search-group">
            <label htmlFor="maxPrice">Max Price ($)</label>
            <input
              type="number"
              id="maxPrice"
              name="maxPrice"
              value={filters.maxPrice || ''}
              onChange={handleInputChange}
              placeholder="999.99"
              min="0"
              step="0.01"
              className="search-input"
            />
          </div>
          
          <div className="search-group checkbox-group">
            <label className="checkbox-label">
              <input
                type="checkbox"
                name="inStock"
                checked={filters.inStock || false}
                onChange={handleCheckboxChange}
                className="search-checkbox"
              />
              In Stock Only
            </label>
          </div>
        </div>

        <div className="search-actions">
          <button type="submit" className="btn btn-primary">
            🔍 Search
          </button>
          <button type="button" onClick={handleClear} className="btn btn-secondary">
            🗑️ Clear
          </button>
        </div>
      </form>
    </div>
  );
};

export default BookSearch;
