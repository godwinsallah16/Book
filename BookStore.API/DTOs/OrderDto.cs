using BookStore.API.Models;

namespace BookStore.API.DTOs
{
    public class CreateOrderDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateOrderItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string BookAuthor { get; set; } = null!;
        public string? BookImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class PaymentRequestDto
    {
        public int OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentToken { get; set; } // For Stripe, PayPal tokens
        public CardDetailsDto? CardDetails { get; set; }
    }

    public class CardDetailsDto
    {
        public string CardNumber { get; set; } = null!;
        public string ExpiryMonth { get; set; } = null!;
        public string ExpiryYear { get; set; } = null!;
        public string Cvv { get; set; } = null!;
        public string CardHolderName { get; set; } = null!;
    }

    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
        public OrderDto? Order { get; set; }
    }

    public class OrderSummaryDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public List<OrderDto> RecentOrders { get; set; } = new();
    }
}
