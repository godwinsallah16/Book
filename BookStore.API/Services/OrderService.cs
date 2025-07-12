using Microsoft.EntityFrameworkCore;
using BookStore.API.Data;
using BookStore.API.Models;
using BookStore.API.DTOs;
using BookStore.API.Services.Interfaces;
using AutoMapper;

namespace BookStore.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, IMapper mapper, ILogger<OrderService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string userId)
        {
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    PaymentMethod = createOrderDto.PaymentMethod,
                    ShippingAddress = createOrderDto.ShippingAddress,
                    Notes = createOrderDto.Notes,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in createOrderDto.Items)
                {
                    var book = await _context.Books
                        .Where(b => b.Id == item.BookId && !b.IsDeleted)
                        .FirstOrDefaultAsync();

                    if (book == null)
                    {
                        throw new InvalidOperationException($"Book with ID {item.BookId} not found");
                    }

                    if (book.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for book '{book.Title}'. Available: {book.StockQuantity}, Requested: {item.Quantity}");
                    }

                    var orderItem = new OrderItem
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = book.Price,
                        TotalPrice = book.Price * item.Quantity
                    };

                    orderItems.Add(orderItem);
                    totalAmount += orderItem.TotalPrice;

                    // Update book stock
                    book.StockQuantity -= item.Quantity;
                }

                order.TotalAmount = totalAmount;
                order.OrderItems = orderItems;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return await GetOrderByIdAsync(order.Id, userId) ?? throw new InvalidOperationException("Failed to retrieve created order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                throw;
            }
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto paymentRequest, string userId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .FirstOrDefaultAsync(o => o.Id == paymentRequest.OrderId && o.UserId == userId);

                if (order == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        ErrorMessage = "Order not found"
                    };
                }

                if (order.Status != OrderStatus.Pending)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        ErrorMessage = "Order is not in pending status"
                    };
                }

                // Simulate payment processing
                var paymentResult = await SimulatePaymentProcessingAsync(paymentRequest, order);

                if (paymentResult.Success)
                {
                    order.Status = OrderStatus.Processing;
                    order.PaymentTransactionId = paymentResult.TransactionId;
                    order.CompletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    var orderDto = await GetOrderByIdAsync(order.Id, userId);
                    return new PaymentResponseDto
                    {
                        Success = true,
                        TransactionId = paymentResult.TransactionId,
                        Order = orderDto
                    };
                }
                else
                {
                    // Restore book stock on failed payment
                    foreach (var item in order.OrderItems)
                    {
                        var book = await _context.Books.FindAsync(item.BookId);
                        if (book != null)
                        {
                            book.StockQuantity += item.Quantity;
                        }
                    }
                    await _context.SaveChangesAsync();

                    return new PaymentResponseDto
                    {
                        Success = false,
                        ErrorMessage = paymentResult.ErrorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}", paymentRequest.OrderId);
                return new PaymentResponseDto
                {
                    Success = false,
                    ErrorMessage = "Payment processing failed"
                };
            }
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return orders.Select(MapToOrderDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, string userId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                return order != null ? MapToOrderDto(order) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId} for user {UserId}", orderId, userId);
                throw;
            }
        }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync(string userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                var completedOrders = orders.Where(o => o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Processing).ToList();
                var recentOrders = orders.Take(5).Select(MapToOrderDto).ToList();

                return new OrderSummaryDto
                {
                    TotalOrders = completedOrders.Count,
                    TotalSpent = completedOrders.Sum(o => o.TotalAmount),
                    RecentOrders = recentOrders
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order summary for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .Include(o => o.User)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                return orders.Select(MapToOrderDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                throw;
            }
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, OrderStatus status, string userId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order == null)
                {
                    return null;
                }

                order.Status = status;
                if (status == OrderStatus.Delivered && order.CompletedAt == null)
                {
                    order.CompletedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return MapToOrderDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for order {OrderId}", orderId);
                throw;
            }
        }

        private async Task<(bool Success, string? TransactionId, string? ErrorMessage)> SimulatePaymentProcessingAsync(PaymentRequestDto paymentRequest, Order order)
        {
            // Simulate payment processing delay
            await Task.Delay(1000);

            // Simulate success/failure (you can replace this with actual payment gateway integration)
            var random = new Random();
            var success = random.Next(1, 11) > 1; // 90% success rate for simulation

            if (success)
            {
                return (true, $"TXN_{DateTime.UtcNow:yyyyMMddHHmmss}_{random.Next(1000, 9999)}", null);
            }
            else
            {
                return (false, null, "Payment declined by bank");
            }
        }

        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                PaymentTransactionId = order.PaymentTransactionId,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    BookAuthor = oi.Book.Author,
                    BookImageUrl = oi.Book.ImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }
    }
}
