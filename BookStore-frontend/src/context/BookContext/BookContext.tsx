import React, { createContext, useReducer, useCallback } from 'react';
import type { Book, BookFilters, CreateBookRequest, UpdateBookRequest } from '../../types';
import { bookService } from '../../services/bookService';
import type { AxiosError } from 'axios';

interface BookState {
  books: Book[];
  selectedBook: Book | null;
  filters: BookFilters;
  loading: boolean;
  error: string | null;
  categories: string[];
  authors: string[];
}

type BookAction =
  | { type: 'SET_LOADING'; payload: boolean }
  | { type: 'SET_ERROR'; payload: string | null }
  | { type: 'SET_BOOKS'; payload: Book[] }
  | { type: 'SET_SELECTED_BOOK'; payload: Book | null }
  | { type: 'SET_FILTERS'; payload: BookFilters }
  | { type: 'ADD_BOOK'; payload: Book }
  | { type: 'UPDATE_BOOK'; payload: Book }
  | { type: 'DELETE_BOOK'; payload: number }
  | { type: 'SET_CATEGORIES'; payload: string[] }
  | { type: 'SET_AUTHORS'; payload: string[] };

const initialState: BookState = {
  books: [],
  selectedBook: null,
  filters: {},
  loading: false,
  error: null,
  categories: [],
  authors: [],
};

function bookReducer(state: BookState, action: BookAction): BookState {
  switch (action.type) {
    case 'SET_LOADING':
      return { ...state, loading: action.payload };
    case 'SET_ERROR':
      return { ...state, error: action.payload, loading: false };
    case 'SET_BOOKS':
      return { ...state, books: action.payload, loading: false, error: null };
    case 'SET_SELECTED_BOOK':
      return { ...state, selectedBook: action.payload };
    case 'SET_FILTERS':
      return { ...state, filters: action.payload };
    case 'ADD_BOOK':
      return { ...state, books: [...state.books, action.payload] };
    case 'UPDATE_BOOK':
      return {
        ...state,
        books: state.books.map(book =>
          book.id === action.payload.id ? action.payload : book
        ),
        selectedBook: state.selectedBook?.id === action.payload.id ? action.payload : state.selectedBook,
      };
    case 'DELETE_BOOK':
      return {
        ...state,
        books: state.books.filter(book => book.id !== action.payload),
        selectedBook: state.selectedBook?.id === action.payload ? null : state.selectedBook,
      };
    case 'SET_CATEGORIES':
      return { ...state, categories: action.payload };
    case 'SET_AUTHORS':
      return { ...state, authors: action.payload };
    default:
      return state;
  }
}

interface BookContextType {
  state: BookState;
  fetchBooks: (filters?: BookFilters) => Promise<void>;
  fetchBook: (id: number) => Promise<void>;
  createBook: (book: CreateBookRequest) => Promise<void>;
  updateBook: (id: number, book: UpdateBookRequest) => Promise<void>;
  deleteBook: (id: number) => Promise<void>;
  setFilters: (filters: BookFilters) => void;
  setSelectedBook: (book: Book | null) => void;
  fetchCategories: () => Promise<void>;
  fetchAuthors: () => Promise<void>;
}

const BookContext = createContext<BookContextType | undefined>(undefined);

export { BookContext };

interface BookProviderProps {
  children: React.ReactNode;
}

export const BookProvider: React.FC<BookProviderProps> = ({ children }) => {
  const [state, dispatch] = useReducer(bookReducer, initialState);

  const fetchBooks = useCallback(async (filters?: BookFilters) => {
    dispatch({ type: 'SET_LOADING', payload: true });
    try {
      const paginated = await bookService.getBooks(filters);
      dispatch({ type: 'SET_BOOKS', payload: paginated.data });
      if (filters) {
        dispatch({ type: 'SET_FILTERS', payload: filters });
      }
    } catch (error) {
      console.error('Error fetching books:', error);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to fetch books' });
    }
  }, []);

  const fetchBook = useCallback(async (id: number) => {
    dispatch({ type: 'SET_LOADING', payload: true });
    try {
      const book = await bookService.getBook(id);
      dispatch({ type: 'SET_SELECTED_BOOK', payload: book });
      dispatch({ type: 'SET_LOADING', payload: false });
    } catch (error) {
      console.error('Error fetching book:', error);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to fetch book' });
    }
  }, []);

  const createBook = useCallback(async (book: CreateBookRequest) => {
    dispatch({ type: 'SET_LOADING', payload: true });
    try {
      const newBook = await bookService.createBook(book);
      dispatch({ type: 'ADD_BOOK', payload: newBook });
      dispatch({ type: 'SET_LOADING', payload: false });
    } catch (error) {
      console.error('Error creating book:', error);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to create book' });
    }
  }, []);

  const updateBook = useCallback(async (id: number, book: UpdateBookRequest) => {
    dispatch({ type: 'SET_LOADING', payload: true });
    try {
      const updatedBook = await bookService.updateBook(id, book);
      dispatch({ type: 'UPDATE_BOOK', payload: updatedBook });
      dispatch({ type: 'SET_LOADING', payload: false });
    } catch (error) {
      console.error('Error updating book:', error);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to update book' });
    }
  }, []);

  const deleteBook = useCallback(async (id: number) => {
    dispatch({ type: 'SET_LOADING', payload: true });
    try {
      await bookService.deleteBook(id);
      dispatch({ type: 'DELETE_BOOK', payload: id });
      dispatch({ type: 'SET_LOADING', payload: false });
      
      // No navigation here. UI should handle navigation after delete if needed.
    } catch (error) {
      console.error('Error deleting book:', error);
      // Even if API returns 404, the book might still be deleted
      // Check if the error is 404 and handle accordingly
      if (error && typeof error === 'object' && 'response' in error) {
        const axiosError = error as AxiosError;
        if (axiosError.response?.status === 404) {
          // Book was deleted but API returned 404, still remove from state
          dispatch({ type: 'DELETE_BOOK', payload: id });
          dispatch({ type: 'SET_LOADING', payload: false });
          
          // No navigation here. UI should handle navigation after delete if needed.
          return;
        }
      }
      dispatch({ type: 'SET_ERROR', payload: 'Failed to delete book' });
    }
  }, []);

  const setFilters = useCallback((filters: BookFilters) => {
    dispatch({ type: 'SET_FILTERS', payload: filters });
  }, []);

  const setSelectedBook = useCallback((book: Book | null) => {
    dispatch({ type: 'SET_SELECTED_BOOK', payload: book });
  }, []);

  const fetchCategories = useCallback(async () => {
    try {
      const categories = await bookService.getCategories();
      dispatch({ type: 'SET_CATEGORIES', payload: categories });
    } catch (error) {
      console.error('Error fetching categories:', error);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to fetch categories' });
    }
  }, []);

  const fetchAuthors = useCallback(async () => {
    try {
      const authors = await bookService.getAuthors();
      dispatch({ type: 'SET_AUTHORS', payload: authors });
    } catch (error) {
      console.error('Error fetching authors:', error);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to fetch authors' });
    }
  }, []);

  const value: BookContextType = {
    state,
    fetchBooks,
    fetchBook,
    createBook,
    updateBook,
    deleteBook,
    setFilters,
    setSelectedBook,
    fetchCategories,
    fetchAuthors,
  };

  return <BookContext.Provider value={value}>{children}</BookContext.Provider>;
};
