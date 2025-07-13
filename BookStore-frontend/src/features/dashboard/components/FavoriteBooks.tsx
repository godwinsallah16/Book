import React, { useEffect, useState } from 'react';
import BookCard from './BookCard';
import type { Book } from '../../../types/book.types';
import { authService } from '../../../services';
import bookService from '../../../services/bookService';
import './FavoriteBooks.css';

const FavoriteBooks: React.FC = () => {
  const currentUser = authService.getCurrentUser();
  const [books, setBooks] = useState<Book[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchFavorites = async () => {
      setLoading(true);
      setError(null);
      try {
        // Replace with your real favorite API endpoint
        const result = await bookService.getFavoriteBooks();
        setBooks(Array.isArray(result) ? result : []);
      } catch {
        setBooks([]);
        setError('Failed to load favorite books.');
      } finally {
        setLoading(false);
      }
    };
    fetchFavorites();
  }, []);

  const handleRemoveFavorite = async (book: Book) => {
    setLoading(true);
    try {
      await bookService.removeFavorite(book.id);
      setBooks((prev) => prev.filter((b) => b.id !== book.id));
    } catch {
      setError('Failed to remove favorite.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h2>Favorite Books</h2>
      {loading ? (
        <p>Loading favorites...</p>
      ) : error ? (
        <div className="error-container">{error}</div>
      ) : (
        <div className="book-list-grid">
          {books.length === 0 ? (
            <p>No favorite books found.</p>
          ) : (
            books.map((book) => (
              <BookCard
                key={book.id}
                book={book}
                isFavorite={true}
                onToggleFavorite={handleRemoveFavorite}
              />
            ))
          )}
        </div>
      )}
    </div>
  );
};

export default FavoriteBooks;
