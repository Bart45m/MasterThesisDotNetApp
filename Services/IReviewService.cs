using MasterThesisDotNetApp.Models;
using MasterThesisDotNetApp.Responses;
using MasterThesisDotNetApp.Requests;
using Microsoft.EntityFrameworkCore;

namespace MasterThesisDotNetApp.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewResponse>> GetReviewsByProductIdAsync(int productId);

    Task<ReviewResponse?> CreateReviewAsync(int userId, CreateReviewRequest request);

    Task<bool> DeleteReviewAsync(int reviewId, int userId, string userRole);
}