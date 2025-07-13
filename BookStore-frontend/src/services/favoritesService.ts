import type { Favorite, AddToFavoritesRequest } from '../types';
import { apiClient } from '../utils/httpClient';
import { API_CONFIG } from '../utils/constants';

class FavoritesService {
  async getFavorites(): Promise<Favorite[]> {
    try {
      const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
      if (!token) {
        return []; // Return empty array if not authenticated
      }
      
      const response = await apiClient.get<Favorite[]>(API_CONFIG.ENDPOINTS.FAVORITES.BASE);
      return response.data || [];
    } catch (error) {
      console.error('Error fetching favorites:', error);
      return []; // Return empty array on error
    }
  }

  async addToFavorites(request: AddToFavoritesRequest): Promise<void> {
    try {
      await apiClient.post(API_CONFIG.ENDPOINTS.FAVORITES.BASE, request);
    } catch (error) {
      console.error('Error adding to favorites:', error);
      throw error;
    }
  }

  async removeFromFavorites(bookId: number): Promise<void> {
    try {
      await apiClient.delete(API_CONFIG.ENDPOINTS.FAVORITES.BY_ID(bookId));
    } catch (error) {
      console.error('Error removing from favorites:', error);
      throw error;
    }
  }

  async isFavorite(bookId: number): Promise<boolean> {
    try {
      const response = await apiClient.get<boolean>(API_CONFIG.ENDPOINTS.FAVORITES.BY_ID(bookId));
      return response.data;
    } catch (error) {
      console.error('Error checking favorite status:', error);
      return false;
    }
  }
}

export const favoritesService = new FavoritesService();
