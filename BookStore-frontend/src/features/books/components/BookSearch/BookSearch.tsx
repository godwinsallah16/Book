import { useState, useEffect } from 'react';
import { useBooks } from '../../../../hooks';
import { Input, Button, Card } from '../../../../shared/components/ui';
import type { BookFilters } from '../../../../types';
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
    setFilters((prev: BookFilters) => ({
      ...prev,
      [name]: value === '' ? undefined : 
              name === 'minPrice' || name === 'maxPrice' ? parseFloat(value) :
              name === 'inStock' ? value === 'true' : value
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
    <Card className="book-search" padding="lg">
      <form onSubmit={handleSubmit} className="search-form">
        <div className="search-row">
          <Input
            label="Search Books"
            name="search"
            value={filters.search || ''}
            onChange={handleInputChange}
            placeholder="Search by title, author, or ISBN..."
            variant="search"
            leftIcon={<span>🔍</span>}
            fullWidth
          />
        </div>

        <div className="search-row">
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
              {categories.map((cat: string) => (
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
              {authors.map((author: string) => (
                <option key={author} value={author}>{author}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="search-row">
          <Input
            label="Min Price ($)"
            type="number"
            name="minPrice"
            value={filters.minPrice?.toString() || ''}
            onChange={handleInputChange}
            placeholder="0.00"
            min={0}
            step={0.01}
            leftIcon={<span>💲</span>}
          />
          
          <Input
            label="Max Price ($)"
            type="number"
            name="maxPrice"
            value={filters.maxPrice?.toString() || ''}
            onChange={handleInputChange}
            placeholder="999.99"
            min={0}
            step={0.01}
            leftIcon={<span>💲</span>}
          />
          
          <div className="search-group">
            <label htmlFor="inStock">Availability</label>
            <select
              id="inStock"
              name="inStock"
              value={filters.inStock?.toString() || ''}
              onChange={handleInputChange}
              className="search-select"
            >
              <option value="">All Books</option>
              <option value="true">In Stock Only</option>
              <option value="false">Out of Stock</option>
            </select>
          </div>
        </div>

        <div className="search-actions">
          <Button type="submit" variant="primary" size="md">
            🔍 Search Books
          </Button>
          <Button type="button" variant="outline" size="md" onClick={handleClear}>
            🧹 Clear Filters
          </Button>
        </div>
      </form>
    </Card>
  );
};

export default BookSearch;
