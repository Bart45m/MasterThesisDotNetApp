namespace MasterThesisDotNetApp.Responses;

public class ReviewResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
}