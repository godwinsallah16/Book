
import React, { useEffect, useState } from 'react';
import type { Book, BookFilters, PaginatedResponse } from '../../../types/book.types';
import { authService } from '../../../services';
import bookService from '../../../services/bookService';

interface EnhancedBookListProps {
  filters: BookFilters;
  onBookEdit: (book: Book) => void;
}




const EnhancedBookList: React.FC<EnhancedBookListProps> = ({ filters, onBookEdit }) => {
  const currentUser = authService.getCurrentUser();
  const isAdmin = currentUser?.roles?.includes('Admin');
  const [books, setBooks] = useState<Book[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchBooks = async () => {
    setLoading(true);
    setError(null);
    try {
      const result: PaginatedResponse<Book> = await bookService.getBooks(filters, page, pageSize);
      setBooks(result.data);
      setTotalPages(result.totalPages);
    } catch {
      setBooks([]);
      setTotalPages(1);
      setError('Failed to load books.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBooks();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filters, page, pageSize]);
  const handleDelete = async (bookId: number) => {
    if (!window.confirm('Are you sure you want to delete this book?')) return;
    setLoading(true);
    setError(null);
    try {
      await bookService.deleteBook(bookId);
      await fetchBooks();
    } catch {
      setError('Failed to delete book.');
    } finally {
      setLoading(false);
    }
  };

  const handlePageChange = (newPage: number) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setPage(newPage);
    }
  };

  return (
    <div>
      <h2>Book List</h2>
      {loading ? (
        <p>Loading books...</p>
      ) : error ? (
        <div className="error-container">{error}</div>
      ) : (
        <>
          {(books?.length ?? 0) === 0 ? (
            <p>No books found.</p>
          ) : (
            <ul>
              {books?.map((book) => {
                const canEditOrDelete = isAdmin || book.userId === currentUser?.userId;
                return (
                  <li key={book.id}>
                    <strong>{book.title}</strong> by {book.author}
                    {canEditOrDelete && (
                      <>
                        <button onClick={() => onBookEdit(book)}>Edit</button>
                        <button onClick={() => handleDelete(book.id)}>Delete</button>
                      </>
                    )}
                  </li>
                );
              })}
            </ul>
          )}
          <div className="pagination">
            <button onClick={() => handlePageChange(page - 1)} disabled={page === 1}>
              Previous
            </button>
            {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
              <button
                key={p}
                onClick={() => handlePageChange(p)}
                disabled={p === page}
              >
                {p}
              </button>
            ))}
            <button onClick={() => handlePageChange(page + 1)} disabled={page === totalPages}>
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default EnhancedBookList;
