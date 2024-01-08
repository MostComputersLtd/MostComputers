using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Services;
using MOSTComputers.UI.Web.Services.Contracts;

namespace MOSTComputers.UI.Web.Pages.Shared;

public class ProductSearchStringDisplayPopupPartialModel
{
    public required Product Product { get; set; }
    public required IEnumerable<ProductCharacteristic> CharacteristicsAndSearchStringAbbreviationsForProduct { get; set; }
    public required ISearchStringOriginService SearchStringOriginService { get; set; }

    public Dictionary<string, List<SearchStringPartOriginData>?>? GetSearchStringPartsAndDataAboutTheirOrigin()
    {
        return SearchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(Product);
    }
}
