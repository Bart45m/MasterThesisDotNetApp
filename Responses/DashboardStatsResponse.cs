namespace MasterThesisDotNetApp.Responses;

public class DashboardStatsResponse
{
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public Dictionary<string, int> OrdersByStatus { get; set; } = new();
}