using MasterThesisDotNetApp.Data;
using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MasterThesisDotNetApp.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
        // Wymagane dla QuestPDF Community License
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateUserRoleAsync(int userId, UpdateUserRole request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.Role = request.Role;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DashboardStatsResponse> GetDashboardStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalProducts = await _context.Products.CountAsync();
        var totalOrders = await _context.Orders.CountAsync();

        var totalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice);

        var ordersByStatus = await _context.Orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

        return new DashboardStatsResponse
        {
            TotalUsers = totalUsers,
            TotalProducts = totalProducts,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            OrdersByStatus = ordersByStatus
        };
    }

    public async Task<byte[]> GeneratePdfReportAsync()
    {
        var stats = await GetDashboardStatsAsync();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("Raport Podsumowujący - Panel Administracyjny")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text($"Data wygenerowania: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                        column.Item().LineHorizontal(1);

                        column.Item().Text("Podsumowanie systemu:").Bold().FontSize(14);
                        column.Item().Text($"- Liczba użytkowników: {stats.TotalUsers}");
                        column.Item().Text($"- Liczba produktów: {stats.TotalProducts}");
                        column.Item().Text($"- Liczba zamówień: {stats.TotalOrders}");
                        column.Item().Text($"- Całkowity przychód: {stats.TotalRevenue:C2}");

                        column.Item().PaddingTop(10).Text("Zamówienia według statusu:").Bold().FontSize(14);

                        foreach (var kvp in stats.OrdersByStatus)
                        {
                            column.Item().Text($"  • {kvp.Key}: {kvp.Value}");
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Strona ");
                        x.CurrentPageNumber();
                    });
            });
        });

        return document.GeneratePdf();
    }
}