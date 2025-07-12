import axios from 'axios';
import type { Favorite, AddToFavoritesRequest } from '../types/favorites.types';
import { API_CONFIG } from '../utils/constants';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api';

class FavoritesService {
  private baseURL = `${API_BASE_URL}/favorites`;

  private getAuthHeaders() {
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }

  async getFavorites(): Promise<Favorite[]> {
    try {
      const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
      if (!token) {
        return []; // Return empty array if not authenticated
      }
      
      const response = await axios.get<Favorite[]>(this.baseURL, {
        headers: this.getAuthHeaders()
      });
      return response.data || [];
    } catch (error) {
      console.error('Error fetching favorites:', error);
      return []; // Return empty array on error
    }
  }

  async addToFavorites(request: AddToFavoritesRequest): Promise<void> {
    try {
      await axios.post(this.baseURL, request, {
        headers: this.getAuthHeaders()
      });
    } catch (error) {
      console.error('Error adding to favorites:', error);
      throw error;
    }
  }

  async removeFromFavorites(bookId: number): Promise<void> {
    try {
      await axios.delete(`${this.baseURL}/${bookId}`, {
        headers: this.getAuthHeaders()
      });
    } catch (error) {
      console.error('Error removing from favorites:', error);
      throw error;
    }
  }

  async isFavorite(bookId: number): Promise<boolean> {
    try {
      const response = await axios.get<boolean>(`${this.baseURL}/${bookId}`, {
        headers: this.getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error('Error checking favorite status:', error);
      return false;
    }
  }
}

export const favoritesService = new FavoritesService();
