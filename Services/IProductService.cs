using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Responses;

namespace MasterThesisDotNetApp.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetProductsAsync(string? search, decimal? minPrice, decimal? maxPrice, bool? inStock);
    Task<ProductResponse> CreateProductAsync(ProductCreateRequest request);
}