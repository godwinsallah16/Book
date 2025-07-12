// Order Types
export interface CreateOrderDto {
  items: CreateOrderItemDto[];
  paymentMethod: PaymentMethod;
  shippingAddress?: string;
  notes?: string;
}

export interface CreateOrderItemDto {
  bookId: number;
  quantity: number;
}

export interface OrderDto {
  id: number;
  userId: string;
  totalAmount: number;
  status: OrderStatus;
  paymentMethod: PaymentMethod;
  paymentTransactionId?: string;
  createdAt: string;
  completedAt?: string;
  shippingAddress?: string;
  notes?: string;
  orderItems: OrderItemDto[];
}

export interface OrderItemDto {
  id: number;
  bookId: number;
  bookTitle: string;
  bookAuthor: string;
  bookImageUrl?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface PaymentRequestDto {
  orderId: number;
  paymentMethod: PaymentMethod;
  paymentToken?: string;
  cardDetails?: CardDetailsDto;
}

export interface CardDetailsDto {
  cardNumber: string;
  expiryMonth: string;
  expiryYear: string;
  cvv: string;
  cardHolderName: string;
}

export interface PaymentResponseDto {
  success: boolean;
  transactionId?: string;
  errorMessage?: string;
  order?: OrderDto;
}

export interface OrderSummaryDto {
  totalOrders: number;
  totalSpent: number;
  recentOrders: OrderDto[];
}

export const OrderStatus = {
  Pending: 0,
  Processing: 1,
  Shipped: 2,
  Delivered: 3,
  Cancelled: 4,
  Refunded: 5
} as const;

export type OrderStatus = typeof OrderStatus[keyof typeof OrderStatus];

export const PaymentMethod = {
  CreditCard: 0,
  PayPal: 1,
  Stripe: 2,
  BankTransfer: 3
} as const;

export type PaymentMethod = typeof PaymentMethod[keyof typeof PaymentMethod];

export const OrderStatusLabels: Record<OrderStatus, string> = {
  [OrderStatus.Pending]: 'Pending',
  [OrderStatus.Processing]: 'Processing',
  [OrderStatus.Shipped]: 'Shipped',
  [OrderStatus.Delivered]: 'Delivered',
  [OrderStatus.Cancelled]: 'Cancelled',
  [OrderStatus.Refunded]: 'Refunded'
};

export const PaymentMethodLabels: Record<PaymentMethod, string> = {
  [PaymentMethod.CreditCard]: 'Credit Card',
  [PaymentMethod.PayPal]: 'PayPal',
  [PaymentMethod.Stripe]: 'Stripe',
  [PaymentMethod.BankTransfer]: 'Bank Transfer'
};
