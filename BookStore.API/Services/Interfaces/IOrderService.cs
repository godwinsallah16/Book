using BookStore.API.Models;
using BookStore.API.DTOs;

namespace BookStore.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string userId);
        Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto paymentRequest, string userId);
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId);
        Task<OrderSummaryDto> GetOrderSummaryAsync(string userId);
        Task<List<OrderDto>> GetAllOrdersAsync(); // Admin only
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus status, string userId);
    }
}
