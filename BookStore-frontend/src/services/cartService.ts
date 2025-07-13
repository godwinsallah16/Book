import type { CartItem, AddToCartRequest, UpdateCartItemRequest, CartSummary } from '../types';
import { apiClient } from '../utils/httpClient';
import { API_CONFIG } from '../utils/constants';

class CartService {
  async getCart(): Promise<CartItem[]> {
    try {
      const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
      if (!token) {
        return []; // Return empty array if not authenticated
      }
      
      const response = await apiClient.get<CartItem[]>(API_CONFIG.ENDPOINTS.CART.BASE);
      return response.data || [];
    } catch (error) {
      console.error('Error fetching cart:', error);
      return []; // Return empty array on error
    }
  }

  async addToCart(request: AddToCartRequest): Promise<CartItem> {
    try {
      const response = await apiClient.post<CartItem>(
        API_CONFIG.ENDPOINTS.CART.BASE,
        request
      );
      return response.data;
    } catch (error) {
      console.error('Error adding to cart:', error);
      throw error;
    }
  }

  async updateCartItem(request: UpdateCartItemRequest): Promise<CartItem> {
    try {
      const response = await apiClient.put<CartItem>(
        API_CONFIG.ENDPOINTS.CART.BASE,
        request
      );
      return response.data;
    } catch (error) {
      console.error('Error updating cart item:', error);
      throw error;
    }
  }

  async removeFromCart(bookId: number): Promise<void> {
    try {
      await apiClient.delete(API_CONFIG.ENDPOINTS.CART.BY_ID(bookId));
    } catch (error) {
      console.error('Error removing from cart:', error);
      throw error;
    }
  }

  async clearCart(): Promise<void> {
    try {
      await apiClient.delete(API_CONFIG.ENDPOINTS.CART.BASE);
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
