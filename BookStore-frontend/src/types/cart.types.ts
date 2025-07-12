export interface CartItem {
  id: number;
  bookId: number;
  book: {
    id: number;
    title: string;
    author: string;
    price: number;
    imageUrl?: string;
    stockQuantity: number;
  };
  quantity: number;
  createdAt: string;
}

export interface AddToCartRequest {
  bookId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  bookId: number;
  quantity: number;
}

export interface CartSummary {
  totalItems: number;
  totalPrice: number;
  items: CartItem[];
}
