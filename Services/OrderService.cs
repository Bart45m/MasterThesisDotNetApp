using MasterThesisDotNetApp.Data;
using MasterThesisDotNetApp.Models;
using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;
using MasterThesisDotNetApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace MasterThesisDotNetApp.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResponse?> CreateOrderAsync(int userId, CreateOrder request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            return null;
        }

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        if (products.Count != productIds.Count)
        {
            return null;
        }

        decimal totalPrice = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var product = products[item.ProductId];
            totalPrice += product.Price * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        var order = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            TotalPrice = totalPrice,
            OrderItems = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return await GetOrderResponseByIdAsync(order.Id);
    }

    public async Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Product != null ? oi.Product.Price : 0
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(int orderId, int userId, string userRole)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return null;
        }

        // BOLA / Baseline: dostęp dla właściciela lub konta ADMIN / MANAGER
        if (order.UserId != userId && userRole != "ADMIN" && userRole != "MANAGER")
        {
            return null;
        }

        return await GetOrderResponseByIdAsync(orderId);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatus request)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return false;
        }

        order.Status = request.Status;
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<OrderResponse?> GetOrderResponseByIdAsync(int orderId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Product != null ? oi.Product.Price : 0
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}