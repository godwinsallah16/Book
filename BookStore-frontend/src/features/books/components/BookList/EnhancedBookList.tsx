import { useEffect, useState } from 'react';
import { useBooks } from '../../../../hooks';
import { useCart } from '../../../../hooks/useCart';
import { favoritesService } from '../../../../services/favoritesService';
import { authService } from '../../../../services/authService';
import { API_CONFIG } from '../../../../utils/constants';
import type { Book, BookFilters } from '../../../../types';
import type { AxiosError } from 'axios';
import './EnhancedBookList.css';

interface EnhancedBookListProps {
  filters?: BookFilters;
  onBookSelect?: (book: Book) => void;
  onBookEdit?: (book: Book) => void;
  onBookDelete?: (book: Book) => void;
}

export function EnhancedBookList({ filters, onBookSelect, onBookEdit, onBookDelete }: EnhancedBookListProps) {
  const { state, fetchBooks, deleteBook } = useBooks();
  const { addToCart } = useCart();
  const { books, loading, error } = state;
  const [favorites, setFavorites] = useState<Set<number>>(new Set());
  const [loadingStates, setLoadingStates] = useState<Record<number, { cart: boolean; favorite: boolean }>>({});
  const currentUser = authService.getCurrentUser();

  // Check if current user can edit/delete a book
  const canUserEditBook = (book: Book): boolean => {
    if (!currentUser) return false;
    
    // Check if user owns the book
    return book.userId === currentUser.userId;
  };

  useEffect(() => {
    fetchBooks(filters);
    loadFavorites();
  }, [fetchBooks, filters]);

  const loadFavorites = async () => {
    try {
      const userFavorites = await favoritesService.getFavorites();
      if (userFavorites && Array.isArray(userFavorites)) {
        setFavorites(new Set(userFavorites.map(fav => fav.bookId)));
      } else {
        setFavorites(new Set());
      }
    } catch (error) {
      console.error('Failed to load favorites:', error);
      setFavorites(new Set()); // Reset to empty set on error
    }
  };

  const setBookLoading = (bookId: number, type: 'cart' | 'favorite', isLoading: boolean) => {
    setLoadingStates(prev => ({
      ...prev,
      [bookId]: {
        ...prev[bookId],
        [type]: isLoading
      }
    }));
  };

  const handleAddToCart = async (book: Book) => {
    if (book.stockQuantity <= 0) return;
    
    // Check if user is authenticated
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    if (!token || !authService.isAuthenticated()) {
      alert('Please log in to add items to your cart');
      return;
    }
    
    setBookLoading(book.id, 'cart', true);
    try {
      await addToCart(book.id, 1);
      // Show success message
    } catch (error) {
      console.error('Failed to add to cart:', error);
      alert('Failed to add book to cart');
    } finally {
      setBookLoading(book.id, 'cart', false);
    }
  };

  const handleToggleFavorite = async (book: Book) => {
    // Check if user is authenticated
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    if (!token || !authService.isAuthenticated()) {
      alert('Please log in to add items to your favorites');
      return;
    }
    setBookLoading(book.id, 'favorite', true);
    try {
      if (favorites.has(book.id)) {
        await favoritesService.removeFromFavorites(book.id);
      } else {
        await favoritesService.addToFavorites({ bookId: book.id });
      }
      await loadFavorites(); // Always refresh favorites after change
    } catch (error) {
      console.error('Failed to toggle favorite:', error);
      alert('Failed to update favorites');
    } finally {
      setBookLoading(book.id, 'favorite', false);
    }
  };

  const handleDelete = async (book: Book) => {
    if (window.confirm(`Are you sure you want to delete "${book.title}"?`)) {
      try {
        await deleteBook(book.id);
        if (onBookDelete) {
          onBookDelete(book);
        }
      } catch (error) {
        console.error('Error deleting book:', error);
        // Check if it's a 404 error (book was deleted but API returned 404)
        if (error && typeof error === 'object' && 'response' in error) {
          const axiosError = error as AxiosError;
          if (axiosError.response?.status === 404) {
            // Book was likely deleted successfully despite 404
            if (onBookDelete) {
              onBookDelete(book);
            }
            return;
          }
        }
        // Show error message for other types of errors
        alert('Failed to delete book. Please try again.');
      }
    }
  };

  if (loading) {
    return (
      <div className="enhanced-book-list">
        <div className="loading">
          <div className="spinner"></div>
          <p>Loading books...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="enhanced-book-list">
        <div className="error">{error}</div>
      </div>
    );
  }

  if ((books?.length ?? 0) === 0) {
    return (
      <div className="enhanced-book-list">
        <div className="empty">No books found</div>
      </div>
    );
  }

  return (
    <div className="enhanced-book-list">
      <div className="books-header">
        <h2>Books</h2>
        <p>{books?.length ?? 0} book(s) found</p>
      </div>
      
      <div className="books-grid">
        {books?.map((book: Book) => (
          <div key={book.id} className="enhanced-book-card">
            <div className="book-image">
              {book.imageUrl ? (
                <img src={book.imageUrl} alt={book.title} />
              ) : (
                <div className="book-placeholder">No Image</div>
              )}
              <button
                className={`favorite-btn ${favorites.has(book.id) ? 'active' : ''}`}
                onClick={() => handleToggleFavorite(book)}
                disabled={loadingStates[book.id]?.favorite}
                title={favorites.has(book.id) ? 'Remove from favorites' : 'Add to favorites'}
              >
                {loadingStates[book.id]?.favorite ? (
                  <div className="btn-spinner"></div>
                ) : (
                  <svg
                    width="20"
                    height="20"
                    viewBox="0 0 24 24"
                    fill={favorites.has(book.id) ? 'currentColor' : 'none'}
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"
                      stroke="currentColor"
                      strokeWidth="2"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    />
                  </svg>
                )}
              </button>
            </div>
            <div className="book-info">
              <h3 className="book-title">{book.title}</h3>
              <p className="book-author">by {book.author}</p>
              <p className="book-category">{book.category}</p>
              <p className="book-price">${book.price.toFixed(2)}</p>
              <p className={`book-stock ${book.stockQuantity === 0 ? 'out-of-stock' : ''}`}>{book.stockQuantity > 0 ? `${book.stockQuantity} in stock` : 'Out of stock'}</p>
              {book.description && <p className="book-description">{book.description}</p>}
              <div className="book-actions">
                <button
                  className="btn btn-primary add-to-cart"
                  onClick={() => handleAddToCart(book)}
                  disabled={book.stockQuantity <= 0 || loadingStates[book.id]?.cart}
                  title={book.stockQuantity <= 0 ? 'Out of stock' : 'Add to cart'}
                >
                  {loadingStates[book.id]?.cart ? (
                    <div className="btn-spinner"></div>
                  ) : (
                    <>
                      <svg
                        width="16"
                        height="16"
                        viewBox="0 0 24 24"
                        fill="none"
                        xmlns="http://www.w3.org/2000/svg"
                      >
                        <path
                          d="M3 3H5L5.4 5M7 13H17L21 5H5.4M7 13L5.4 5M7 13L4.7 15.3C4.3 15.7 4.6 16.5 5.1 16.5H17M17 13V16.5M9 19.5C9.8 19.5 10.5 20.2 10.5 21S9.8 22.5 9 22.5 7.5 21.8 7.5 21 8.2 19.5 9 19.5ZM20 19.5C20.8 19.5 21.5 20.2 21.5 21S20.8 22.5 20 22.5 18.5 21.8 18.5 21 19.2 19.5 20 19.5Z"
                          stroke="currentColor"
                          strokeWidth="2"
                          strokeLinecap="round"
                          strokeLinejoin="round"
                        />
                      </svg>
                      Add to Cart
                    </>
                  )}
                </button>
                <div className="book-admin-actions">
                  {onBookSelect && (
                    <button
                      onClick={() => onBookSelect(book)}
                      className="btn btn-secondary"
                      title="View details"
                    >
                      View
                    </button>
                  )}
                  {onBookEdit && canUserEditBook(book) && (
                    <button
                      onClick={() => onBookEdit(book)}
                      className="btn btn-secondary"
                      title="Edit book"
                    >
                      Edit
                    </button>
                  )}
                  {onBookDelete && canUserEditBook(book) && (
                    <button
                      onClick={() => handleDelete(book)}
                      className="btn btn-danger"
                      title="Delete book"
                    >
                      Delete
                    </button>
                  )}
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
