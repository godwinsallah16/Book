import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../../../hooks/useCart';
import type { CartItem } from '../../../types';
import './ShoppingCart.css';

interface ShoppingCartProps {
  isOpen: boolean;
  onClose: () => void;
}

export function ShoppingCart({ isOpen, onClose }: ShoppingCartProps) {
  const { state, updateCartItem, removeFromCart, clearCart } = useCart();
  const [isCheckingOut, setIsCheckingOut] = useState(false);
  const navigate = useNavigate();

  const handleQuantityChange = async (bookId: number, newQuantity: number) => {
    if (newQuantity <= 0) {
      await removeFromCart(bookId);
    } else {
      await updateCartItem(bookId, newQuantity);
    }
  };

  const handleCheckout = async () => {
    if ((state.items?.length ?? 0) === 0) {
      alert('Your cart is empty');
      return;
    }

    setIsCheckingOut(true);
    try {
      // Navigate to checkout page
      onClose(); // Close the cart first
      navigate('/checkout');
    } catch (err) {
      console.error('Navigation failed:', err);
      alert('Failed to navigate to checkout. Please try again.');
    } finally {
      setIsCheckingOut(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="cart-overlay">
      <div className="cart-sidebar">
        <div className="cart-header">
          <h2>Shopping Cart</h2>
          <button className="close-button" onClick={onClose}>
            ×
          </button>
        </div>

        {state.isLoading && (
          <div className="cart-loading">
            <div className="spinner"></div>
            <p>Loading cart...</p>
          </div>
        )}

        {state.error && (
          <div className="cart-error">
            <p>{state.error}</p>
          </div>
        )}

        <div className="cart-content">
          {(state.items?.length ?? 0) === 0 ? (
            <div className="empty-cart">
              <p>Your cart is empty</p>
              <button className="continue-shopping-btn" onClick={onClose}>
                Continue Shopping
              </button>
            </div>
          ) : (
            <>
              <div className="cart-items">
                {state.items?.map((item: CartItem) => (
                  <div key={item.id} className="cart-item">
                    <div className="item-image">
                      {item.book.imageUrl ? (
                        <img src={item.book.imageUrl} alt={item.book.title} />
                      ) : (
                        <div className="no-image">No Image</div>
                      )}
                    </div>
                    <div className="item-details">
                      <h3>{item.book.title}</h3>
                      <p className="item-author">{item.book.author}</p>
                      <p className="item-price">${item.book.price.toFixed(2)}</p>
                    </div>
                    <div className="quantity-controls">
                      <button
                        onClick={() => handleQuantityChange(item.bookId, item.quantity - 1)}
                        disabled={state.isLoading}
                      >
                        -
                      </button>
                      <span className="quantity">{item.quantity}</span>
                      <button
                        onClick={() => handleQuantityChange(item.bookId, item.quantity + 1)}
                        disabled={state.isLoading || item.quantity >= item.book.stockQuantity}
                      >
                        +
                      </button>
                    </div>
                    <div className="item-total">
                      ${(item.book.price * item.quantity).toFixed(2)}
                    </div>
                    <button
                      className="remove-item-btn"
                      onClick={() => removeFromCart(item.bookId)}
                      disabled={state.isLoading}
                    >
                      Remove
                    </button>
                  </div>
                ))}
              </div>

              <div className="cart-footer">
                <div className="cart-summary">
                  <div className="summary-line">
                    <span>Total Items: {state.totalItems}</span>
                  </div>
                  <div className="summary-line total">
                    <span>Total: ${state.totalPrice.toFixed(2)}</span>
                  </div>
                </div>

                <div className="cart-actions">
                  <button
                    className="clear-cart-btn"
                    onClick={clearCart}
                    disabled={state.isLoading || isCheckingOut}
                  >
                    Clear Cart
                  </button>
                  <button
                    className="checkout-btn"
                    onClick={handleCheckout}
                    disabled={state.isLoading || isCheckingOut}
                  >
                    {isCheckingOut ? 'Processing...' : 'Checkout'}
                  </button>
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
