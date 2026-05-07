using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using static MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData.ProductDataViewComponent;

namespace MOSTComputers.UI.Web.Client.Pages;

public class ProductDataModel : PageModel
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ProductDataModel(IProductService productService,
        IHttpContextAccessor httpContextAccessor)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        _productService = productService;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IProductService _productService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductDataExistingData ExistingData { get; set; }

    public async Task<IActionResult> OnGetAsync(
        [FromRoute] int? productId = null,
        [FromQuery(Name = "mid")] int? productIdProvidedFromLegacySite = null)
    {
        int? actualProductId = productId;

        if (productId == null)
        {
            if (_httpContextAccessor.HttpContext?.Request.Path.ToString() != "/preview_most.php"
                || productIdProvidedFromLegacySite == null)
            {
                return NotFound();
            }

            actualProductId = productIdProvidedFromLegacySite;
        }

        Product? product = await _productService.GetByIdAsync(actualProductId!.Value);

        ExistingData = new()
        {
            Product = product,
        };

        return Page();
    }
}