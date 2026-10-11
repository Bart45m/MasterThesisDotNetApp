using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;

namespace MasterThesisDotNetApp.Services;

public interface IAdminService
{
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<bool> UpdateUserRoleAsync(int userId, UpdateUserRole request);
    Task<DashboardStatsResponse> GetDashboardStatsAsync();
    Task<byte[]> GeneratePdfReportAsync();
}