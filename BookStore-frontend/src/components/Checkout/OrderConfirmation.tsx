import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { orderService } from '../../services/orderService';
import { OrderStatusLabels, PaymentMethodLabels } from '../../types/order.types';
import type { OrderDto } from '../../types/order.types';
import './OrderConfirmation.css';

const OrderConfirmation: React.FC = () => {
  const { orderId } = useParams<{ orderId: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const [order, setOrder] = useState<OrderDto | null>(location.state?.order || null);
  const [loading, setLoading] = useState(!order);
  const [error, setError] = useState<string | null>(null);

  const loadOrder = useCallback(async () => {
    if (!orderId) return;
    
    try {
      setLoading(true);
      const orderData = await orderService.getOrderById(parseInt(orderId));
      setOrder(orderData);
    } catch (err) {
      setError('Failed to load order details');
      console.error('Error loading order:', err);
    } finally {
      setLoading(false);
    }
  }, [orderId]);

  useEffect(() => {
    if (!order && orderId) {
      loadOrder();
    }
  }, [orderId, order, loadOrder]);

  if (loading) {
    return (
      <div className="order-confirmation-container">
        <div className="loading">Loading order details...</div>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="order-confirmation-container">
        <div className="error-message">
          {error || 'Order not found'}
        </div>
        <button 
          className="back-button"
          onClick={() => navigate('/orders')}
        >
          View My Orders
        </button>
      </div>
    );
  }

  return (
    <div className="order-confirmation-container">
      <div className="confirmation-header">
        <div className="success-icon">✓</div>
        <h1>Order Confirmed!</h1>
        <p>Thank you for your purchase. Your order has been successfully placed.</p>
      </div>

      <div className="order-details">
        <div className="order-info">
          <h2>Order Information</h2>
          <div className="info-grid">
            <div className="info-item">
              <span className="label">Order ID:</span>
              <span className="value">#{order.id}</span>
            </div>
            <div className="info-item">
              <span className="label">Order Date:</span>
              <span className="value">{new Date(order.createdAt).toLocaleDateString()}</span>
            </div>
            <div className="info-item">
              <span className="label">Status:</span>
              <span className={`status ${order.status.toString().toLowerCase()}`}>
                {OrderStatusLabels[order.status]}
              </span>
            </div>
            <div className="info-item">
              <span className="label">Payment Method:</span>
              <span className="value">{PaymentMethodLabels[order.paymentMethod]}</span>
            </div>
            {order.paymentTransactionId && (
              <div className="info-item">
                <span className="label">Transaction ID:</span>
                <span className="value">{order.paymentTransactionId}</span>
              </div>
            )}
            <div className="info-item">
              <span className="label">Total Amount:</span>
              <span className="value total">${order.totalAmount.toFixed(2)}</span>
            </div>
          </div>
        </div>

        {order.shippingAddress && (
          <div className="shipping-info">
            <h2>Shipping Address</h2>
            <div className="address">
              {order.shippingAddress}
            </div>
          </div>
        )}

        <div className="order-items">
          <h2>Order Items</h2>
          <div className="items-list">
            {order.orderItems.map(item => (
              <div key={item.id} className="order-item">
                <div className="item-details">
                  <h3>{item.bookTitle}</h3>
                  <p>by {item.bookAuthor}</p>
                  <div className="item-meta">
                    <span>Quantity: {item.quantity}</span>
                    <span>Unit Price: ${item.unitPrice.toFixed(2)}</span>
                  </div>
                </div>
                <div className="item-total">
                  ${item.totalPrice.toFixed(2)}
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="order-summary">
          <div className="summary-row">
            <span className="label">Total Amount:</span>
            <span className="value">${order.totalAmount.toFixed(2)}</span>
          </div>
        </div>
      </div>

      <div className="action-buttons">
        <button 
          className="primary-button"
          onClick={() => navigate('/orders')}
        >
          View My Orders
        </button>
        <button 
          className="secondary-button"
          onClick={() => navigate('/books')}
        >
          Continue Shopping
        </button>
      </div>
    </div>
  );
};

export default OrderConfirmation;
