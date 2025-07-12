export interface Book {
  id: number;
  title: string;
  author: string;
  isbn: string;
  publicationYear: number;
  publisher: string;
  category: string;
  price: number;
  stockQuantity: number;
  description?: string;
  imageUrl?: string;
  createdAt: string;
  updatedAt: string;
  userId?: string;
  uploaderName?: string;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  isbn: string;
  publicationYear: number;
  publisher: string;
  category: string;
  price: number;
  stockQuantity: number;
  description?: string;
  imageUrl?: string;
}

export interface UpdateBookRequest extends CreateBookRequest {
  id: number;
}

export interface BookFilters {
  search?: string;
  category?: string;
  author?: string;
  minPrice?: number;
  maxPrice?: number;
  inStock?: boolean;
}

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}
