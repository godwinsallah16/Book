import React from 'react';
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
    <form onSubmit={handleSubmit} style={{ display: 'flex', gap: '1rem', alignItems: 'center', flexWrap: 'wrap' }}>
      <input
        type="text"
        placeholder="Title or keyword"
        value={search}
        onChange={e => setSearch(e.target.value)}
      />
      <input
        type="text"
        placeholder="Author"
        value={author}
        onChange={e => setAuthor(e.target.value)}
      />
      <input
        type="text"
        placeholder="Category"
        value={category}
        onChange={e => setCategory(e.target.value)}
      />
      <button type="submit">Search</button>
    </form>
  );
};

export default BookSearch;
