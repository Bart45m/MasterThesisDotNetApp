using Microsoft.AspNetCore.Http;

namespace MasterThesisDotNetApp.Requests;

public class CreateReviewRequest
{
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
}