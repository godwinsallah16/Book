import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { cartService } from '../../../services/cartService';
import orderService from '../../../services/orderService';
import { PaymentMethod, PaymentMethodLabels } from '../../../types/order.types';
import type { CartItem } from '../../../types/cart.types';
import type { CreateOrderDto, PaymentRequestDto, CardDetailsDto } from '../../../types/order.types';
import './Checkout.css';

const Checkout: React.FC = () => {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>(PaymentMethod.CreditCard);
  const [shippingAddress, setShippingAddress] = useState('');
  const [cardDetails, setCardDetails] = useState<CardDetailsDto>({
    cardNumber: '',
    expiryMonth: '',
    expiryYear: '',
    cvv: '',
    cardHolderName: ''
  });
  const [total, setTotal] = useState(0);
  const navigate = useNavigate();

  useEffect(() => {
    loadCartItems();
  }, []);

  const loadCartItems = async () => {
    try {
      setLoading(true);
      const items = await cartService.getCart();
      setCartItems(items);
      const totalAmount = items.reduce((sum: number, item: CartItem) => sum + (item.book.price * item.quantity), 0);
      setTotal(totalAmount);
    } catch (err) {
      setError('Failed to load cart items');
      console.error('Error loading cart items:', err);
    } finally {
      setLoading(false);
    }
  };

  const handlePlaceOrder = async () => {
    if (cartItems.length === 0) {
      setError('Your cart is empty');
      return;
    }

    if (!shippingAddress.trim()) {
      setError('Please enter a shipping address');
      return;
    }

    if (paymentMethod === PaymentMethod.CreditCard) {
      if (!cardDetails.cardNumber || !cardDetails.expiryMonth || !cardDetails.expiryYear || 
          !cardDetails.cvv || !cardDetails.cardHolderName) {
        setError('Please fill in all card details');
        return;
      }
    }

    try {
      setLoading(true);
      setError(null);          // Create order
          const createOrderDto: CreateOrderDto = {
            items: cartItems.map(item => ({
              bookId: item.bookId,
              quantity: item.quantity
            })),
            paymentMethod,
            shippingAddress,
            notes: ''
          };

      const order = await orderService.createOrder(createOrderDto);

      // Process payment
      const paymentRequest: PaymentRequestDto = {
        orderId: order.id,
        paymentMethod,
        cardDetails: paymentMethod === PaymentMethod.CreditCard ? cardDetails : undefined
      };

      const paymentResult = await orderService.processPayment(paymentRequest);

      if (paymentResult.success) {
        // Clear cart after successful order
        await cartService.clearCart();
        
        // Navigate to order confirmation
        navigate(`/order-confirmation/${order.id}`, { 
          state: { order: paymentResult.order } 
        });
      } else {
        setError(paymentResult.errorMessage || 'Payment failed');
      }
    } catch (err) {
      setError('Failed to place order');
      console.error('Error placing order:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleCardInputChange = (field: keyof CardDetailsDto, value: string) => {
    setCardDetails(prev => ({
      ...prev,
      [field]: value
    }));
  };

  if (loading && cartItems.length === 0) {
    return (
      <div className="checkout-container">
        <div className="loading">Loading...</div>
      </div>
    );
  }

  return (
    <div className="checkout-container">
      <div className="checkout-header">
        <h1>Checkout</h1>
        <button 
          className="back-button"
          onClick={() => navigate('/cart')}
        >
          ← Back to Cart
        </button>
      </div>

      {error && (
        <div className="error-message">
          {error}
        </div>
      )}

      <div className="checkout-content">
        <div className="checkout-main">
          {/* Order Summary */}
          <div className="order-summary">
            <h2>Order Summary</h2>
            <div className="order-items">
              {cartItems.map(item => (
                <div key={item.id} className="order-item">
                  <div className="item-info">
                    <h3>{item.book.title}</h3>
                    <p>by {item.book.author}</p>
                    <p>Quantity: {item.quantity}</p>
                  </div>
                  <div className="item-price">
                    ${(item.book.price * item.quantity).toFixed(2)}
                  </div>
                </div>
              ))}
            </div>
            <div className="order-total">
              <strong>Total: ${total.toFixed(2)}</strong>
            </div>
          </div>

          {/* Shipping Address */}
          <div className="shipping-section">
            <h2>Shipping Address</h2>
            <textarea
              value={shippingAddress}
              onChange={(e) => setShippingAddress(e.target.value)}
              placeholder="Enter your complete shipping address..."
              rows={4}
              className="shipping-address"
            />
          </div>

          {/* Payment Method */}
          <div className="payment-section">
            <h2>Payment Method</h2>
            <div className="payment-methods">
              {Object.entries(PaymentMethodLabels).map(([key, label]) => (
                <label key={key} className="payment-method">
                  <input
                    type="radio"
                    name="paymentMethod"
                    value={key}
                    checked={paymentMethod === parseInt(key)}
                    onChange={(e) => setPaymentMethod(parseInt(e.target.value) as PaymentMethod)}
                  />
                  {label}
                </label>
              ))}
            </div>

            {/* Credit Card Details */}
            {paymentMethod === PaymentMethod.CreditCard && (
              <div className="card-details">
                <h3>Card Details</h3>
                <div className="card-form">
                  <input
                    type="text"
                    placeholder="Card Holder Name"
                    value={cardDetails.cardHolderName}
                    onChange={(e) => handleCardInputChange('cardHolderName', e.target.value)}
                  />
                  <input
                    type="text"
                    placeholder="Card Number"
                    value={cardDetails.cardNumber}
                    onChange={(e) => handleCardInputChange('cardNumber', e.target.value)}
                  />
                  <div className="expiry-cvv">
                    <input
                      type="text"
                      placeholder="MM"
                      value={cardDetails.expiryMonth}
                      onChange={(e) => handleCardInputChange('expiryMonth', e.target.value)}
                      maxLength={2}
                    />
                    <input
                      type="text"
                      placeholder="YY"
                      value={cardDetails.expiryYear}
                      onChange={(e) => handleCardInputChange('expiryYear', e.target.value)}
                      maxLength={2}
                    />
                    <input
                      type="text"
                      placeholder="CVV"
                      value={cardDetails.cvv}
                      onChange={(e) => handleCardInputChange('cvv', e.target.value)}
                      maxLength={3}
                    />
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Place Order Button */}
          <button 
            className="place-order-button"
            onClick={handlePlaceOrder}
            disabled={loading || cartItems.length === 0}
          >
            {loading ? 'Processing...' : `Place Order - $${total.toFixed(2)}`}
          </button>
        </div>
      </div>
    </div>
  );
};

export default Checkout;
