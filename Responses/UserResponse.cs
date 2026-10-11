using MasterThesisDotNetApp.Enums;
using MasterThesisDotNetApp.Models;

namespace MasterThesisDotNetApp.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}