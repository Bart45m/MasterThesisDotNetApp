using MasterThesisDotNetApp.Requests;
using MasterThesisDotNetApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterThesisDotNetApp.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET /api/products?search=string&minPrice=decimal&maxPrice=decimal&inStock=boolean
    // Dostęp publiczny (anonimowy)
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock)
    {
        var products = await _productService.GetProductsAsync(search, minPrice, maxPrice, inStock);
        return Ok(products);
    }

    // POST /api/products (Multipart Form)
    // Wymagany token JWT z rolą ADMIN lub MANAGER
    [HttpPost]
    [Authorize(Roles = "ADMIN,MANAGER")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateProduct([FromForm] ProductCreateRequest request)
    {
        var product = await _productService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
    }
}