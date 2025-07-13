import React, { useEffect } from 'react';
import { useBooks } from '../../../../hooks';
import type { Book, BookFilters } from '../../../../types';
import './BookList.css';

interface BookListProps {
  filters?: BookFilters;
  onBookSelect?: (book: Book) => void;
  onBookEdit?: (book: Book) => void;
  onBookDelete?: (book: Book) => void;
}

const BookList: React.FC<BookListProps> = ({ filters, onBookSelect, onBookEdit, onBookDelete }) => {
  const { state, fetchBooks, deleteBook } = useBooks();
  const { books, loading, error } = state;

  useEffect(() => {
    fetchBooks(filters);
  }, [fetchBooks, filters]);

  const handleDelete = async (book: Book) => {
    if (window.confirm(`Are you sure you want to delete "${book.title}"?`)) {
      try {
        await deleteBook(book.id);
        if (onBookDelete) {
          onBookDelete(book);
        }
      } catch (error) {
        console.error('Failed to delete book:', error);
      }
    }
  };

  if (loading) {
    return <div className="loading">Loading books...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  if ((books?.length ?? 0) === 0) {
    return <div className="empty">No books found</div>;
  }

  return (
    <div className="book-list">
      <h2>Books</h2>
      <div className="books-grid">
        {books?.map((book: Book) => (
          <div key={book.id} className="book-card">
            <div className="book-image">
              {book.imageUrl ? (
                <img src={book.imageUrl} alt={book.title} />
              ) : (
                <div className="book-placeholder">No Image</div>
              )}
            </div>
            <div className="book-info">
              <h3 className="book-title">{book.title}</h3>
              <p className="book-author">by {book.author}</p>
              <p className="book-category">{book.category}</p>
              <p className="book-price">${book.price.toFixed(2)}</p>
              <p className="book-stock">
                {book.stockQuantity > 0 ? `${book.stockQuantity} in stock` : 'Out of stock'}
              </p>
              {book.description && (
                <p className="book-description">{book.description}</p>
              )}
              <div className="book-actions">
                {onBookSelect && (
                  <button 
                    onClick={() => onBookSelect(book)}
                    className="btn btn-primary"
                  >
                    View
                  </button>
                )}
                {onBookEdit && (
                  <button 
                    onClick={() => onBookEdit(book)}
                    className="btn btn-secondary"
                  >
                    Edit
                  </button>
                )}
                {onBookDelete && (
                  <button 
                    onClick={() => handleDelete(book)}
                    className="btn btn-danger"
                  >
                    Delete
                  </button>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default BookList;
