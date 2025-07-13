import React from 'react';
import { useNavigate } from 'react-router-dom';
import { BookForm } from '../components';
import type { Book } from '../../../types';
import './BookFormPage.css';

interface BookFormPageProps {
  book?: Book;
  onSuccess: () => void;
  onCancel: () => void;
}

const BookFormPage: React.FC<BookFormPageProps> = ({ book, onSuccess, onCancel }) => {
  const navigate = useNavigate();
  const handleSuccess = () => {
    if (onSuccess) onSuccess();
    navigate('/dashboard');
  };
  return (
    <div className="book-form-page">
      <div className="book-form-header">
        <h1>{book ? 'Edit Book' : 'Add New Book'}</h1>
        <p className="book-form-subtitle">
          {book ? 'Update book information' : 'Add a new book to the library'}
        </p>
      </div>
      <div className="book-form-container">
        <BookForm 
          book={book}
          onSuccess={handleSuccess}
          onCancel={onCancel}
        />
      </div>
    </div>
  );
};

export default BookFormPage;
