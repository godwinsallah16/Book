import React, { useState, useEffect } from 'react';
import { useBooks } from '../../../../hooks';
import type { Book, CreateBookRequest } from '../../../../types/book.types';
import { BOOK_CATEGORIES } from '../../../../types/book.categories';
import './BookForm.css';

interface BookFormProps {
  book?: Book;
  onCancel: () => void;
  onSuccess: () => void;
}

const BookForm: React.FC<BookFormProps> = ({ book, onCancel, onSuccess }) => {
  const { createBook, updateBook, state } = useBooks();
  const { loading, error } = state;

  const [formData, setFormData] = useState<CreateBookRequest>({
    title: '',
    author: '',
    isbn: '',
    publicationYear: new Date().getFullYear(),
    publisher: '',
    category: '',
    price: 0,
    stockQuantity: 0,
    description: '',
    imageUrl: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});



  useEffect(() => {
    if (book) {
      setFormData({
        title: book.title,
        author: book.author,
        isbn: book.isbn,
        publicationYear: book.publicationYear,
        publisher: book.publisher,
        category: book.category,
        price: book.price,
        stockQuantity: book.stockQuantity,
        description: book.description || '',
        imageUrl: book.imageUrl || '',
      });
    }
  }, [book]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev: CreateBookRequest) => ({
      ...prev,
      [name]: name === 'price' || name === 'stockQuantity' || name === 'publicationYear' 
        ? Number(value) 
        : value
    }));
    
    // Clear error when user starts typing
    if (errors[name]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[name];
        return newErrors;
      });
    }
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.title.trim()) newErrors.title = 'Title is required';
    if (!formData.author.trim()) newErrors.author = 'Author is required';
    if (!formData.isbn.trim()) newErrors.isbn = 'ISBN is required';
    if (!formData.publisher.trim()) newErrors.publisher = 'Publisher is required';
    if (!formData.category.trim()) newErrors.category = 'Category is required';
    if (formData.price <= 0) newErrors.price = 'Price must be greater than 0';
    if (formData.stockQuantity < 0) newErrors.stockQuantity = 'Stock quantity cannot be negative';
    if (formData.publicationYear < 1000 || formData.publicationYear > new Date().getFullYear()) {
      newErrors.publicationYear = 'Please enter a valid publication year';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    try {
      if (book) {
        await updateBook(book.id, { ...formData, id: book.id });
      } else {
        await createBook(formData);
      }
      onSuccess();
    } catch (error) {
      console.error('Error saving book:', error);
    }
  };

  return (
    <div className="book-form-container">
      <form onSubmit={handleSubmit} className="book-form">
        <h2>{book ? 'Edit Book' : 'Add New Book'}</h2>
        
        {error && <div className="error-message">{error}</div>}

        <div className="form-group">
          <label htmlFor="title">Title *</label>
          <input
            type="text"
            id="title"
            name="title"
            value={formData.title}
            onChange={handleChange}
            className={errors.title ? 'error' : ''}
            required
          />
          {errors.title && <span className="error-text">{errors.title}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="author">Author *</label>
          <input
            type="text"
            id="author"
            name="author"
            value={formData.author}
            onChange={handleChange}
            className={errors.author ? 'error' : ''}
            required
          />
          {errors.author && <span className="error-text">{errors.author}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="isbn">ISBN *</label>
          <input
            type="text"
            id="isbn"
            name="isbn"
            value={formData.isbn}
            onChange={handleChange}
            className={errors.isbn ? 'error' : ''}
            required
          />
          {errors.isbn && <span className="error-text">{errors.isbn}</span>}
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="publicationYear">Publication Year *</label>
            <input
              type="number"
              id="publicationYear"
              name="publicationYear"
              value={formData.publicationYear}
              onChange={handleChange}
              className={errors.publicationYear ? 'error' : ''}
              min="1000"
              max={new Date().getFullYear()}
              required
            />
            {errors.publicationYear && <span className="error-text">{errors.publicationYear}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="publisher">Publisher *</label>
            <input
              type="text"
              id="publisher"
              name="publisher"
              value={formData.publisher}
              onChange={handleChange}
              className={errors.publisher ? 'error' : ''}
              required
            />
            {errors.publisher && <span className="error-text">{errors.publisher}</span>}
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="category">Category *</label>
          <select
            id="category"
            name="category"
            value={formData.category}
            onChange={handleChange}
            className={errors.category ? 'error' : ''}
            required
            aria-label="Select book category"
          >
            <option value="">Select a category</option>
            {BOOK_CATEGORIES.map(cat => (
              <optgroup key={cat.name} label={cat.name}>
                <option value={cat.name}>{cat.name}</option>
                {cat.subgenres && cat.subgenres.map(sub => (
                  <option key={cat.name + '-' + sub} value={sub}>{cat.name} - {sub}</option>
                ))}
              </optgroup>
            ))}
          </select>
          {errors.category && <span className="error-text">{errors.category}</span>}
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="price">Price ($) *</label>
            <input
              type="number"
              id="price"
              name="price"
              value={formData.price}
              onChange={handleChange}
              className={errors.price ? 'error' : ''}
              min="0"
              step="0.01"
              required
            />
            {errors.price && <span className="error-text">{errors.price}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="stockQuantity">Stock Quantity *</label>
            <input
              type="number"
              id="stockQuantity"
              name="stockQuantity"
              value={formData.stockQuantity}
              onChange={handleChange}
              className={errors.stockQuantity ? 'error' : ''}
              min="0"
              required
            />
            {errors.stockQuantity && <span className="error-text">{errors.stockQuantity}</span>}
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="imageUrl">Image URL</label>
          <input
            type="url"
            id="imageUrl"
            name="imageUrl"
            value={formData.imageUrl}
            onChange={handleChange}
            placeholder="https://example.com/image.jpg"
          />
        </div>

        <div className="form-group">
          <label htmlFor="description">Description</label>
          <textarea
            id="description"
            name="description"
            value={formData.description}
            onChange={handleChange}
            rows={4}
            placeholder="Enter book description..."
          />
        </div>

        <div className="form-actions">
          <button type="button" onClick={onCancel} className="btn btn-secondary">
            Cancel
          </button>
          <button type="submit" disabled={loading} className="btn btn-primary">
            {loading ? 'Saving...' : (book ? 'Update Book' : 'Add Book')}
          </button>
        </div>
      </form>
    </div>
  );
};

export default BookForm;
