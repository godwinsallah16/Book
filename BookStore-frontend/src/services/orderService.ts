import { apiClient } from '../utils/httpClient';
import { API_CONFIG } from '../utils/constants';
import type {
  CreateOrderDto,
  OrderDto,
  PaymentRequestDto,
  PaymentResponseDto,
  OrderSummaryDto
} from '../types/order.types';
import { OrderStatus } from '../types/order.types';

export const orderService = {
  // Create new order
  async createOrder(createOrderDto: CreateOrderDto): Promise<OrderDto> {
    try {
      const response = await apiClient.post<OrderDto>(
        API_CONFIG.ENDPOINTS.ORDERS.BASE,
        createOrderDto
      );
      return response.data;
    } catch (error) {
      console.error('Create order error:', error);
      throw error;
    }
  },

  // Process payment
  async processPayment(paymentRequest: PaymentRequestDto): Promise<PaymentResponseDto> {
    try {
      const response = await apiClient.post<PaymentResponseDto>(
        API_CONFIG.ENDPOINTS.ORDERS.PAYMENT,
        paymentRequest
      );
      return response.data;
    } catch (error) {
      console.error('Process payment error:', error);
      throw error;
    }
  },

  // Get user orders
  async getUserOrders(): Promise<OrderDto[]> {
    try {
      const response = await apiClient.get<OrderDto[]>(
        API_CONFIG.ENDPOINTS.ORDERS.BASE
      );
      return response.data;
    } catch (error) {
      console.error('Get user orders error:', error);
      throw error;
    }
  },

  // Get order by ID
  async getOrderById(orderId: number): Promise<OrderDto> {
    try {
      const response = await apiClient.get<OrderDto>(
        API_CONFIG.ENDPOINTS.ORDERS.BY_ID(orderId)
      );
      return response.data;
    } catch (error) {
      console.error('Get order by ID error:', error);
      throw error;
    }
  },

  // Get order summary
  async getOrderSummary(): Promise<OrderSummaryDto> {
    try {
      const response = await apiClient.get<OrderSummaryDto>(
        API_CONFIG.ENDPOINTS.ORDERS.SUMMARY
      );
      return response.data;
    } catch (error) {
      console.error('Get order summary error:', error);
      throw error;
    }
  },

  // Update order status
  async updateOrderStatus(orderId: number, status: OrderStatus): Promise<OrderDto> {
    try {
      const response = await apiClient.put<OrderDto>(
        API_CONFIG.ENDPOINTS.ORDERS.STATUS(orderId),
        { status }
      );
      return response.data;
    } catch (error) {
      console.error('Update order status error:', error);
      throw error;
    }
  },

  // Cancel order (convenience method)
  async cancelOrder(orderId: number): Promise<OrderDto> {
    try {
      return await this.updateOrderStatus(orderId, OrderStatus.Cancelled);
    } catch (error) {
      console.error('Cancel order error:', error);
      throw error;
    }
  },

  // Get all orders (Admin only)
  async getAllOrders(): Promise<OrderDto[]> {
    try {
      const response = await apiClient.get<OrderDto[]>(
        API_CONFIG.ENDPOINTS.ORDERS.ALL
      );
      return response.data;
    } catch (error) {
      console.error('Get all orders error:', error);
      throw error;
    }
  }
};

export default orderService;
