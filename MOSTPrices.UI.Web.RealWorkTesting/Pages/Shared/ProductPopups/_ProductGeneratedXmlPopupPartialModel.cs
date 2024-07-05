using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductGeneratedXmlPopupPartialModel
{
    public required string XmlData { get; set; }
    public required Product Product { get; set; }
}
