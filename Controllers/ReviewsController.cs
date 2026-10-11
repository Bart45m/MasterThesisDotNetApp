using System.Security.Claims;
using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterThesisDotNetApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // 1. GET /api/reviews/product/{productId}
    [HttpGet("product/{productId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviewsByProductId(int productId)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
        return Ok(reviews);
    }

    // 2. POST /api/reviews
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateReview([FromForm] CreateReviewRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var review = await _reviewService.CreateReviewAsync(userId, request);
        if (review == null)
        {
            return BadRequest(new { message = "Product does not exist." });
        }

        return Ok(review);
    }

    // 3. DELETE /api/reviews/{id}
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _reviewService.DeleteReviewAsync(id, userId, userRole);
        if (!result)
        {
            return BadRequest(new { message = "Review not found or permission denied." });
        }

        return Ok(new { message = "Review deleted successfully." });
    }
}