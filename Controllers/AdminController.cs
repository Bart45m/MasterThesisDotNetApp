using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterThesisDotNetApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,MANAGER")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // 1. GET /api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    // 2. PATCH /api/admin/users/{id}/role
    [HttpPatch("users/{id:int}/role")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRole request)
    {
        var success = await _adminService.UpdateUserRoleAsync(id, request);
        if (!success)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(new { message = "User role updated successfully." });
    }

    // 3. GET /api/admin/stats (JSON)
    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return Ok(stats);
    }

    // 4. GET /api/admin/stats/pdf (Plik PDF)
    [HttpGet("stats/pdf")]
    public async Task<IActionResult> GetDashboardStatsPdf()
    {
        var pdfBytes = await _adminService.GeneratePdfReportAsync();
        var fileName = $"report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}