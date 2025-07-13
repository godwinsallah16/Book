import React from 'react';
import './BookSearch.css';
import type { BookFilters } from '../../../types/book.types';

interface BookSearchProps {
  onSearch: (filters: BookFilters) => void;
}


import { useState } from 'react';

const BookSearch: React.FC<BookSearchProps> = ({ onSearch }) => {
  const [search, setSearch] = useState('');
  const [author, setAuthor] = useState('');
  const [category, setCategory] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch({
      search: search.trim() || undefined,
      author: author.trim() || undefined,
      category: category.trim() || undefined,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="book-search-form">
      <input
        type="text"
        placeholder="Title or keyword"
        value={search}
        onChange={e => setSearch(e.target.value)}
        className="book-search-input"
      />
      <input
        type="text"
        placeholder="Author"
        value={author}
        onChange={e => setAuthor(e.target.value)}
        className="book-search-input"
      />
      <input
        type="text"
        placeholder="Category"
        value={category}
        onChange={e => setCategory(e.target.value)}
        className="book-search-input"
      />
      <button type="submit" className="book-search-btn">Search</button>
    </form>
  );
};

export default BookSearch;
