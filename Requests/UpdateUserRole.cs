using MasterThesisDotNetApp.Enums;
using MasterThesisDotNetApp.Models;

namespace MasterThesisDotNetApp.Requests;

public class UpdateUserRole
{
    public UserRole Role { get; set; }
}