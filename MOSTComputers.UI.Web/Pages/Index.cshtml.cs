using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.UI.Web.Pages;
public class IndexModel : PageModel
{
    public IndexModel(IProductService productService, ILogger<IndexModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    private readonly IProductService _productService;
    private readonly ILogger<IndexModel> _logger;

    public IEnumerable<Product> Products { get; set; } = new List<Product>();

    public void OnGet()
    {
        IEnumerable<Product> products = _productService.GetSelectionWithoutImagesAndProps(new List<uint> { 67210, 67219, 68445, 54332 });

        Products = products;
    }

    internal static string GetDisplayNameFromManifactuerer(Manifacturer? manifacturer)
    {
        if (manifacturer is null) return string.Empty;

        return manifacturer.BGName ?? manifacturer.RealCompanyName ?? string.Empty;
    }
}