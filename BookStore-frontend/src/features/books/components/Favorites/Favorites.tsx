import { useState, useEffect } from 'react';
import { favoritesService, authService } from '../../../../services';
import { useCart } from '../../../../hooks';
import { API_CONFIG } from '../../../../utils';
import type { Favorite } from '../../../../types';
import './Favorites.css';

export function Favorites() {
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { addToCart } = useCart();

  useEffect(() => {
    // Only fetch favorites if user is authenticated
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    if (token && authService.isAuthenticated()) {
      fetchFavorites();
    } else {
      setIsLoading(false);
      setError('Please log in to view your favorites');
    }
  }, []);

  const fetchFavorites = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await favoritesService.getFavorites();
      setFavorites(data);
    } catch (err) {
      console.error('Error fetching favorites:', err);
      setError('Failed to load favorites');
    } finally {
      setIsLoading(false);
    }
  };

  const handleRemoveFromFavorites = async (bookId: number) => {
    try {
      await favoritesService.removeFromFavorites(bookId);
      setFavorites(prev => prev.filter(fav => fav.bookId !== bookId));
    } catch (err) {
      console.error('Error removing from favorites:', err);
      setError('Failed to remove from favorites');
    }
  };

  const handleAddToCart = async (bookId: number) => {
    try {
      await addToCart(bookId, 1);
      // Show success message or update UI
    } catch (err) {
      console.error('Error adding to cart:', err);
      setError('Failed to add to cart');
    }
  };

  if (isLoading) {
    return (
      <div className="favorites-container">
        <div className="favorites-header">
          <h2>My Favorites</h2>
        </div>
        <div className="favorites-loading">
          <div className="spinner"></div>
          <p>Loading favorites...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="favorites-container">
        <div className="favorites-header">
          <h2>My Favorites</h2>
        </div>
        <div className="favorites-error">
          <p>{error}</p>
          <button onClick={fetchFavorites}>Try Again</button>
        </div>
      </div>
    );
  }

  return (
    <div className="favorites-container">
      <div className="favorites-header">
        <h2>My Favorites</h2>
        <p>{favorites.length} book(s) in your favorites</p>
      </div>

      {favorites.length === 0 ? (
        <div className="empty-favorites">
          <h3>No favorites yet</h3>
          <p>Start browsing books and add them to your favorites!</p>
        </div>
      ) : (
        <div className="favorites-grid">
          {favorites.map((favorite) => (
            <div key={favorite.id} className="favorite-item">
              <div className="favorite-image">
                {favorite.book.imageUrl ? (
                  <img src={favorite.book.imageUrl} alt={favorite.book.title} />
                ) : (
                  <div className="no-image">No Image</div>
                )}
              </div>
              
              <div className="favorite-details">
                <h3>{favorite.book.title}</h3>
                <p className="favorite-author">{favorite.book.author}</p>
                <p className="favorite-category">{favorite.book.category}</p>
                <p className="favorite-price">${favorite.book.price.toFixed(2)}</p>
              </div>

              <div className="favorite-actions">
                <button
                  className="add-to-cart-btn"
                  onClick={() => handleAddToCart(favorite.bookId)}
                >
                  Add to Cart
                </button>
                <button
                  className="remove-favorite-btn"
                  onClick={() => handleRemoveFromFavorites(favorite.bookId)}
                >
                  Remove
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
