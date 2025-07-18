import React from 'react';
import type { Book } from '../../../types/book.types';
import './BookCard.css';

interface BookCardProps {
  book: Book;
  onEdit?: (book: Book) => void;
  onDelete?: (bookId: number) => void;
  onAddToCart?: (book: Book) => void;
  onToggleFavorite?: (book: Book) => void;
  isFavorite?: boolean;
  canEditOrDelete?: boolean;
}

const BookCard: React.FC<BookCardProps> = (props) => {
  const {
    book,
    onEdit,
    onDelete,
    onAddToCart,
    onToggleFavorite,
    isFavorite = false,
    canEditOrDelete = false,
  } = props;
  return (
    <div className="book-card vertical">
      <div className="book-card-image">
        <img src={book.imageUrl || '/default-book.png'} alt={book.title} />
        <div className="book-card-icons">
          <div style={{ width: '100%' }}>
            <button className={`favorite-btn${isFavorite ? ' active' : ''}`} onClick={() => onToggleFavorite?.(book)} title="Favorite">
              <span role="img" aria-label="favorite">❤️</span>
            </button>
          </div>
          <div style={{ width: '100%' }}>
            <button className="cart-btn" onClick={() => onAddToCart?.(book)} title="Add to Cart">
              <span role="img" aria-label="cart">🛒</span>
            </button>
          </div>
        </div>
      </div>
      <div className="book-card-details">
        <h3>{book.title}</h3>
        <p className="author">by {book.author}</p>
        <p className="price">${book.price.toFixed(2)}</p>
        <p className="category">{book.category}</p>
        <p className="desc">{book.description}</p>
        {canEditOrDelete && (
          <div className="actions">
            <button onClick={() => onEdit?.(book)} className="edit-btn">Edit</button>
            <button onClick={() => onDelete?.(book.id)} className="delete-btn">Delete</button>
          </div>
        )}
      </div>
    </div>
  );
};

export default BookCard;
