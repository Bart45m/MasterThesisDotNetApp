using MasterThesisDotNetApp.Enums;

namespace MasterThesisDotNetApp.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; }
    public MasterThesisDotNetApp.Enums.OrderStatus Status { get; set; } = MasterThesisDotNetApp.Enums.OrderStatus.Pending;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}