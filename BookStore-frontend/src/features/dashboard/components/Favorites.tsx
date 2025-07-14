import React, { useEffect, useState } from 'react';
import { favoritesService } from '../../../services/favoritesService';
import type { Favorite } from '../../../types';

const Favorites: React.FC = () => {
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchFavorites = async () => {
      setLoading(true);
      setError(null);
      try {
        const favs = await favoritesService.getFavorites();
        setFavorites(favs);
      } catch {
        setError('Failed to load favorites');
      } finally {
        setLoading(false);
      }
    };
    fetchFavorites();
  }, []);

  return (
    <div>
      <h2>Favorites</h2>
      {loading && <p>Loading...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {!loading && favorites.length === 0 && <p>No favorite books found.</p>}
      <div className="favorite-books-list">
        {favorites.map(fav => (
          <div key={fav.id} className="favorite-book-card">
            <img src={fav.book.imageUrl || '/default-book.png'} alt={fav.book.title} style={{ width: '80px', height: '120px', objectFit: 'cover', borderRadius: '8px' }} />
            <div>
              <h3>{fav.book.title}</h3>
              <p>by {fav.book.author}</p>
              <p>{fav.book.category}</p>
              <p>${fav.book.price.toFixed(2)}</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Favorites;
