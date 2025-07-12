import { createContext, useReducer, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { CartItem } from '../types/cart.types';
import { cartService } from '../services/cartService';
import { authService } from '../services/authService';
import { API_CONFIG } from '../utils/constants';

interface CartState {
  items: CartItem[];
  totalItems: number;
  totalPrice: number;
  isLoading: boolean;
  error: string | null;
}

type CartAction =
  | { type: 'SET_LOADING'; payload: boolean }
  | { type: 'SET_ERROR'; payload: string | null }
  | { type: 'SET_CART'; payload: CartItem[] }
  | { type: 'ADD_ITEM'; payload: CartItem }
  | { type: 'UPDATE_ITEM'; payload: CartItem }
  | { type: 'REMOVE_ITEM'; payload: number }
  | { type: 'CLEAR_CART' };

const initialState: CartState = {
  items: [],
  totalItems: 0,
  totalPrice: 0,
  isLoading: false,
  error: null
};

function cartReducer(state: CartState, action: CartAction): CartState {
  switch (action.type) {
    case 'SET_LOADING':
      return { ...state, isLoading: action.payload };
    case 'SET_ERROR':
      return { ...state, error: action.payload, isLoading: false };
    case 'SET_CART': {
      const items = action.payload || [];
      const totalItems = items.reduce((sum, item) => sum + item.quantity, 0);
      const totalPrice = items.reduce((sum, item) => sum + (item.book.price * item.quantity), 0);
      return { ...state, items, totalItems, totalPrice, isLoading: false, error: null };
    }
    case 'ADD_ITEM': {
      const newItem = action.payload;
      const existingItemIndex = state.items.findIndex(item => item.bookId === newItem.bookId);
      let newItems;
      
      if (existingItemIndex >= 0) {
        newItems = [...state.items];
        newItems[existingItemIndex] = newItem;
      } else {
        newItems = [...state.items, newItem];
      }
      
      const totalItems = newItems.reduce((sum, item) => sum + item.quantity, 0);
      const totalPrice = newItems.reduce((sum, item) => sum + (item.book.price * item.quantity), 0);
      return { ...state, items: newItems, totalItems, totalPrice, isLoading: false, error: null };
    }
    case 'UPDATE_ITEM': {
      const updatedItem = action.payload;
      const newItems = state.items.map(item =>
        item.bookId === updatedItem.bookId ? updatedItem : item
      );
      const totalItems = newItems.reduce((sum, item) => sum + item.quantity, 0);
      const totalPrice = newItems.reduce((sum, item) => sum + (item.book.price * item.quantity), 0);
      return { ...state, items: newItems, totalItems, totalPrice, isLoading: false, error: null };
    }
    case 'REMOVE_ITEM': {
      const newItems = state.items.filter(item => item.bookId !== action.payload);
      const totalItems = newItems.reduce((sum, item) => sum + item.quantity, 0);
      const totalPrice = newItems.reduce((sum, item) => sum + (item.book.price * item.quantity), 0);
      return { ...state, items: newItems, totalItems, totalPrice, isLoading: false, error: null };
    }
    case 'CLEAR_CART':
      return { ...state, items: [], totalItems: 0, totalPrice: 0, isLoading: false, error: null };
    default:
      return state;
  }
}

interface CartContextType {
  state: CartState;
  fetchCart: () => Promise<void>;
  addToCart: (bookId: number, quantity: number) => Promise<void>;
  updateCartItem: (bookId: number, quantity: number) => Promise<void>;
  removeFromCart: (bookId: number) => Promise<void>;
  clearCart: () => Promise<void>;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export { CartContext };

export function CartProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(cartReducer, initialState);

  const fetchCart = async () => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });
      const items = await cartService.getCart();
      dispatch({ type: 'SET_CART', payload: items || [] });
    } catch (err) {
      console.error('Error fetching cart:', err);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to fetch cart' });
      dispatch({ type: 'SET_CART', payload: [] }); // Set empty cart on error
    }
  };

  const addToCart = async (bookId: number, quantity: number) => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });
      const item = await cartService.addToCart({ bookId, quantity });
      dispatch({ type: 'ADD_ITEM', payload: item });
    } catch (err) {
      console.error('Error adding to cart:', err);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to add item to cart' });
    }
  };

  const updateCartItem = async (bookId: number, quantity: number) => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });
      const item = await cartService.updateCartItem({ bookId, quantity });
      dispatch({ type: 'UPDATE_ITEM', payload: item });
    } catch (err) {
      console.error('Error updating cart item:', err);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to update cart item' });
    }
  };

  const removeFromCart = async (bookId: number) => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });
      await cartService.removeFromCart(bookId);
      dispatch({ type: 'REMOVE_ITEM', payload: bookId });
    } catch (err) {
      console.error('Error removing from cart:', err);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to remove item from cart' });
    }
  };

  const clearCart = async () => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });
      await cartService.clearCart();
      dispatch({ type: 'CLEAR_CART' });
    } catch (err) {
      console.error('Error clearing cart:', err);
      dispatch({ type: 'SET_ERROR', payload: 'Failed to clear cart' });
    }
  };

  useEffect(() => {
    // Only fetch cart if user is authenticated
    const token = localStorage.getItem(API_CONFIG.STORAGE_KEYS.AUTH_TOKEN);
    if (token && authService.isAuthenticated()) {
      fetchCart();
    }
  }, []);

  return (
    <CartContext.Provider
      value={{
        state,
        fetchCart,
        addToCart,
        updateCartItem,
        removeFromCart,
        clearCart
      }}
    >
      {children}
    </CartContext.Provider>
  );
}
