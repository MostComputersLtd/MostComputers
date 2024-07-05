using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductFirstImageDisplayPopupPartialModel
{
    public ProductFirstImageDisplayPopupPartialModel(ProductDisplayData productData)
    {
        ProductData = productData;
    }

    public ProductDisplayData ProductData { get; }
}