using MasterThesisDotNetApp.Data;
using MasterThesisDotNetApp.Models;
using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;
using Microsoft.EntityFrameworkCore;

namespace MasterThesisDotNetApp.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductResponse>> GetProductsAsync(
        string? search,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (inStock.HasValue && inStock.Value)
        {
            query = query.Where(p => p.Quantity > 0);
        }

        return await query
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                FilePath = p.FilePath,
                IsHidden = p.IsHidden
            })
            .ToListAsync();
    }

    public async Task<ProductResponse> CreateProductAsync(ProductCreateRequest request)
    {
        string? filePath = null;

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
                filePath = Path.Combine("uploads", fileName);

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

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
            FilePath = filePath,
            IsHidden = false
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            FilePath = product.FilePath,
            IsHidden = product.IsHidden
        };
    }
}