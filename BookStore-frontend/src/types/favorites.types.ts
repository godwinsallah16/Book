export interface Favorite {
  id: number;
  bookId: number;
  book: {
    id: number;
    title: string;
    author: string;
    price: number;
    imageUrl?: string;
    category: string;
  };
  createdAt: string;
}

export interface AddToFavoritesRequest {
  bookId: number;
}
