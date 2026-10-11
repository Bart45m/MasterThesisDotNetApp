using System.Security.Claims;
using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterThesisDotNetApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // 1. GET /api/orders (Pobranie zamówień zalogowanego użytkownika)
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var orders = await _orderService.GetUserOrdersAsync(userId.Value);
        return Ok(orders);
    }

    // 2. GET /api/orders/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userId = GetCurrentUserId();
        var userRole = GetCurrentUserRole();
        if (userId == null) return Unauthorized();

        var order = await _orderService.GetOrderByIdAsync(id, userId.Value, userRole);
        if (order == null)
        {
            return NotFound(new { message = "Order not found or access denied." });
        }

        return Ok(order);
    }

    // 3. POST /api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrder request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var order = await _orderService.CreateOrderAsync(userId.Value, request);
        if (order == null)
        {
            return BadRequest(new { message = "Invalid items or products do not exist." });
        }

        return Ok(order);
    }

    // 4. PATCH /api/orders/{id}/status (Zmiana statusu - ograniczenie do ADMIN/MANAGER)
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatus request)
    {
        var success = await _orderService.UpdateOrderStatusAsync(id, request);
        if (!success)
        {
            return BadRequest(new { message = "Order not found or invalid status update." });
        }

        return Ok(new { message = "Order status updated successfully." });
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}