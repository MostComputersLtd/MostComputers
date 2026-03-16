using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using static MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData.ProductDataViewComponent;

namespace MOSTComputers.UI.Web.Client.Pages;

public class ProductDataModel : PageModel
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ProductDataModel(IProductService productService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        _productService = productService;
    }

    private readonly IProductService _productService;

    public ProductDataExistingData ExistingData { get; set; }

    public async Task OnGetAsync([FromRoute] int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        ExistingData = new()
        {
            Product = product,
        };
    }
}