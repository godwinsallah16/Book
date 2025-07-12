import type { Book, CreateBookRequest, UpdateBookRequest, BookFilters } from '../../types/book.types';
import { apiClient } from '../../utils/httpClient';
import { API_CONFIG } from '../../utils/constants';

export const bookService = {
  // Get all books with optional filters
  async getBooks(filters?: BookFilters): Promise<Book[]> {
    try {
      const params = new URLSearchParams();
      
      if (filters?.search) params.append('search', filters.search);
      if (filters?.category) params.append('category', filters.category);
      if (filters?.author) params.append('author', filters.author);
      if (filters?.minPrice !== undefined) params.append('minPrice', filters.minPrice.toString());
      if (filters?.maxPrice !== undefined) params.append('maxPrice', filters.maxPrice.toString());
      if (filters?.inStock !== undefined) params.append('inStock', filters.inStock.toString());

      const response = await apiClient.get<Book[]>(`${API_CONFIG.ENDPOINTS.BOOKS.BASE}?${params.toString()}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching books:', error);
      throw error;
    }
  },

  // Get book by ID
  async getBook(id: number): Promise<Book> {
    try {
      const response = await apiClient.get<Book>(API_CONFIG.ENDPOINTS.BOOKS.BY_ID(id));
      return response.data;
    } catch (error) {
      console.error('Error fetching book:', error);
      throw error;
    }
  },

  // Create new book
  async createBook(book: CreateBookRequest): Promise<Book> {
    try {
      const response = await apiClient.post<Book>(API_CONFIG.ENDPOINTS.BOOKS.BASE, book);
      return response.data;
    } catch (error) {
      console.error('Error creating book:', error);
      throw error;
    }
  },

  // Update book
  async updateBook(id: number, book: UpdateBookRequest): Promise<Book> {
    try {
      const response = await apiClient.put<Book>(API_CONFIG.ENDPOINTS.BOOKS.BY_ID(id), book);
      return response.data;
    } catch (error) {
      console.error('Error updating book:', error);
      throw error;
    }
  },

  // Delete book
  async deleteBook(id: number): Promise<void> {
    try {
      await apiClient.delete(API_CONFIG.ENDPOINTS.BOOKS.BY_ID(id));
    } catch (error) {
      console.error('Error deleting book:', error);
      throw error;
    }
  },

  // Get unique categories
  async getCategories(): Promise<string[]> {
    try {
      const response = await apiClient.get<string[]>(API_CONFIG.ENDPOINTS.BOOKS.CATEGORIES);
      return response.data;
    } catch (error) {
      console.error('Error fetching categories:', error);
      throw error;
    }
  },

  // Get unique authors
  async getAuthors(): Promise<string[]> {
    try {
      const response = await apiClient.get<string[]>(API_CONFIG.ENDPOINTS.BOOKS.AUTHORS);
      return response.data;
    } catch (error) {
      console.error('Error fetching authors:', error);
      throw error;
    }
  }
};

export default bookService;
