import React, { useEffect } from 'react';
import { useBooks } from '../../hooks/useBooks/useBooks';

const BookList: React.FC = () => {
  const { state, fetchBooks } = useBooks();

  useEffect(() => {
    fetchBooks();
  }, [fetchBooks]);

  if (state.loading) {
    return <div>Loading books...</div>;
  }

  if (state.error) {
    return <div style={{ color: 'red' }}>Error: {state.error}</div>;
  }

  if (!state.books.length) {
    return <div>No books found.</div>;
  }

  return (
    <ul>
      {state.books.map(book => (
        <li key={book.id}>
          <strong>{book.title}</strong> by {book.author}
        </li>
      ))}
    </ul>
  );
};

export default BookList;
