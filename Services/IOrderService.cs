using MasterThesisDotNetApp.Models;
using System.Reflection;
using System.Security.Cryptography;

using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;

namespace MasterThesisDotNetApp.Services;

public interface IOrderService
{
    Task<OrderResponse?> CreateOrderAsync(int userId, CreateOrder request);
    Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(int userId);
    Task<OrderResponse?> GetOrderByIdAsync(int orderId, int userId, string userRole);
    Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatus request);
}
