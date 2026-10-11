using MasterThesisDotNetApp.Models;
using MasterThesisDotNetApp.Enums;
namespace MasterThesisDotNetApp.Requests;

public class UpdateOrderStatus
{
    public OrderStatus Status { get; set; }
}