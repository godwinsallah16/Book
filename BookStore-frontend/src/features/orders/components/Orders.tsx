import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import orderService from '../../../services/orderService';
import { OrderStatusLabels, PaymentMethodLabels, OrderStatus } from '../../../types/order.types';
import type { OrderDto } from '../../../types/order.types';
import './Orders.css';

const Orders: React.FC = () => {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [cancellingOrders, setCancellingOrders] = useState<Set<number>>(new Set());
  const navigate = useNavigate();

  useEffect(() => {
    loadOrders();
  }, []);

  const loadOrders = async () => {
    try {
      setLoading(true);
      const orderData = await orderService.getUserOrders();
      setOrders(orderData);
    } catch (err) {
      setError('Failed to load orders');
      console.error('Error loading orders:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleViewOrder = (orderId: number) => {
    navigate(`/order-confirmation/${orderId}`);
  };

  const handleCancelOrder = async (orderId: number) => {
    if (!window.confirm('Are you sure you want to cancel this order?')) {
      return;
    }

    try {
      setCancellingOrders(prev => new Set(prev).add(orderId));
      await orderService.cancelOrder(orderId);
      // Refresh the orders list
      await loadOrders();
    } catch (err) {
      console.error('Error cancelling order:', err);
      setError('Failed to cancel order. Please try again.');
    } finally {
      setCancellingOrders(prev => {
        const newSet = new Set(prev);
        newSet.delete(orderId);
        return newSet;
      });
    }
  };

  if (loading) {
    return (
      <div className="orders-container">
        <div className="loading">Loading your orders...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="orders-container">
        <div className="error-message">
          {error}
        </div>
        <button 
          className="retry-button"
          onClick={loadOrders}
        >
          Try Again
        </button>
      </div>
    );
  }

  return (
    <div className="orders-container">
      <div className="orders-header">
        <h1>My Orders</h1>
        <button 
          className="back-button"
          onClick={() => navigate('/books')}
        >
          Continue Shopping
        </button>
      </div>

      {orders.length === 0 ? (
        <div className="no-orders">
          <div className="no-orders-icon">📦</div>
          <h2>No orders yet</h2>
          <p>You haven't placed any orders yet. Start shopping to see your orders here!</p>
          <button 
            className="shop-button"
            onClick={() => navigate('/books')}
          >
            Start Shopping
          </button>
        </div>
      ) : (
        <div className="orders-list">
          {orders.map(order => (
            <div key={order.id} className="order-card">
              <div className="order-header">
                <div className="order-info">
                  <h3>Order #{order.id}</h3>
                  <p>Placed on {new Date(order.createdAt).toLocaleDateString()}</p>
                </div>
                <div className="order-status">
                  <span className={`status ${order.status.toString().toLowerCase()}`}>
                    {OrderStatusLabels[order.status]}
                  </span>
                </div>
              </div>

              <div className="order-details">
                <div className="order-items">
                  <h4>Items ({order.orderItems.length})</h4>
                  <div className="items-summary">
                    {order.orderItems.slice(0, 3).map(item => (
                      <div key={item.id} className="item-summary">
                        <span className="item-name">{item.bookTitle}</span>
                        <span className="item-quantity">x{item.quantity}</span>
                      </div>
                    ))}
                    {order.orderItems.length > 3 && (
                      <div className="more-items">
                        +{order.orderItems.length - 3} more items
                      </div>
                    )}
                  </div>
                </div>

                <div className="order-meta">
                  <div className="meta-item">
                    <span className="label">Payment:</span>
                    <span className="value">{PaymentMethodLabels[order.paymentMethod]}</span>
                  </div>
                  <div className="meta-item">
                    <span className="label">Total:</span>
                    <span className="value total">${order.totalAmount.toFixed(2)}</span>
                  </div>
                </div>
              </div>

              <div className="order-actions">
                <button 
                  className="view-button"
                  onClick={() => handleViewOrder(order.id)}
                >
                  View Details
                </button>
                {order.status === OrderStatus.Pending && (
                  <button 
                    className="cancel-button"
                    onClick={() => handleCancelOrder(order.id)}
                    disabled={cancellingOrders.has(order.id)}
                  >
                    {cancellingOrders.has(order.id) ? 'Cancelling...' : 'Cancel Order'}
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default Orders;
