# useBooks Hook

Custom React hook for book-related operations and state management.

## 📁 Structure

```
useBooks/
├── useBooks.ts       # Main hook implementation
└── index.ts          # Hook exports
```

## 🔧 Features

This hook provides functionality for:
- Book listing and pagination
- Book search and filtering
- Book CRUD operations
- Loading states
- Error handling

## 🔗 Usage

```typescript
import { useBooks } from '../hooks/useBooks';

function BookComponent() {
  const {
    books,
    loading,
    error,
    fetchBooks,
    searchBooks,
    createBook,
    updateBook,
    deleteBook
  } = useBooks();

  // Use the hook methods and state
}
```

## 📊 State Management

The hook manages:
- `books`: Array of book objects
- `loading`: Loading state indicator
- `error`: Error state and messages
- `pagination`: Pagination information

## 🎯 Methods

- `fetchBooks()`: Fetch all books
- `searchBooks(query)`: Search books by query
- `createBook(bookData)`: Create a new book
- `updateBook(id, bookData)`: Update existing book
- `deleteBook(id)`: Delete a book
