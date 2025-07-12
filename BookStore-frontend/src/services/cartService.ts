import axios from 'axios';
import type { CartItem, AddToCartRequest, UpdateCartItemRequest, CartSummary } from '../types/cart.types';
import { API_CONFIG } from '../utils/constants';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api';

class CartService {
  private baseURL = `${API_BASE_URL}/cart`;

  private getAuthHeaders() {
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }

  async getCart(): Promise<CartItem[]> {
    try {
      const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
      if (!token) {
        return []; // Return empty array if not authenticated
      }
      
      const response = await axios.get<CartItem[]>(this.baseURL, {
        headers: this.getAuthHeaders()
      });
      return response.data || [];
    } catch (error) {
      console.error('Error fetching cart:', error);
      return []; // Return empty array on error
    }
  }

  async addToCart(request: AddToCartRequest): Promise<CartItem> {
    try {
      const response = await axios.post<CartItem>(
        this.baseURL,
        request,
        {
          headers: this.getAuthHeaders()
        }
      );
      return response.data;
    } catch (error) {
      console.error('Error adding to cart:', error);
      throw error;
    }
  }

  async updateCartItem(request: UpdateCartItemRequest): Promise<CartItem> {
    try {
      const response = await axios.put<CartItem>(
        this.baseURL,
        request,
        {
          headers: this.getAuthHeaders()
        }
      );
      return response.data;
    } catch (error) {
      console.error('Error updating cart item:', error);
      throw error;
    }
  }

  async removeFromCart(bookId: number): Promise<void> {
    try {
      await axios.delete(`${this.baseURL}/${bookId}`, {
        headers: this.getAuthHeaders()
      });
    } catch (error) {
      console.error('Error removing from cart:', error);
      throw error;
    }
  }

  async clearCart(): Promise<void> {
    try {
      await axios.delete(this.baseURL, {
        headers: this.getAuthHeaders()
      });
    } catch (error) {
      console.error('Error clearing cart:', error);
      throw error;
    }
  }

  async getCartSummary(): Promise<CartSummary> {
    try {
      const items = await this.getCart();
      const totalItems = items.reduce((sum, item) => sum + item.quantity, 0);
      const totalPrice = items.reduce((sum, item) => sum + (item.book.price * item.quantity), 0);
      
      return {
        totalItems,
        totalPrice,
        items
      };
    } catch (error) {
      console.error('Error getting cart summary:', error);
      throw error;
    }
  }
}

export const cartService = new CartService();
