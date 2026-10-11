using MasterThesisDotNetApp.Data;
using MasterThesisDotNetApp.Models;
using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;
using Microsoft.EntityFrameworkCore;

namespace MasterThesisDotNetApp.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;

    public ReviewService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReviewResponse>> GetReviewsByProductIdAsync(int productId)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ProductId == productId)
            .Include(r => r.User)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                UserEmail = r.User != null ? r.User.Email : string.Empty,
                Rating = r.Rating,
                Comment = r.Comment,
                AttachmentUrl = r.AttachmentUrl
            })
            .ToListAsync();
    }

    public async Task<ReviewResponse?> CreateReviewAsync(int userId, CreateReviewRequest request)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId);
        if (!productExists)
        {
            return null;
        }

        string? attachmentUrl = null;

        if (request.File != null && request.File.Length > 0)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = request.File.FileName;
                var fullPath = Path.Combine(uploadsFolder, fileName);
                attachmentUrl = Path.Combine("uploads", fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File save error: {ex.Message}");
            }
        }

        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            AttachmentUrl = attachmentUrl
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);

        return new ReviewResponse
        {
            Id = review.Id,
            ProductId = review.ProductId,
            UserId = review.UserId,
            UserEmail = user?.Email ?? string.Empty,
            Rating = review.Rating,
            Comment = review.Comment,
            AttachmentUrl = review.AttachmentUrl
        };
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, int userId, string userRole)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            return false;
        }

        // Sprawdzenie uprawnień: Właściciel opinii LUB Admin / Manager
        if (review.UserId != userId && userRole != "ADMIN" && userRole != "MANAGER")
        {
            return false;
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }
}