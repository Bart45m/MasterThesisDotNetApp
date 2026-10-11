using MasterThesisDotNetApp.Models;
using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;

namespace MasterThesisDotNetApp.Services;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<User?> GetUserByIdAsync(int userId);
    Task<string?> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
}