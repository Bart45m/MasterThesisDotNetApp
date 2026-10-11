namespace MasterThesisDotNetApp.Responses;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? FilePath { get; set; }
    public bool IsHidden { get; set; }
}