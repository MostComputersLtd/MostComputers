using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Pages.Shared;

public class ProductImagesDisplayPopupModel
{
    public required Product Product { get; set; }
}
