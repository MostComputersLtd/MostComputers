using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductPopups;

public class ProductSearchStringDisplayPopupPartialModel
{
    public required Product Product { get; set; }
    public required IEnumerable<ProductCharacteristic> CharacteristicsAndSearchStringAbbreviationsForProduct { get; set; }
    public required ISearchStringOriginService SearchStringOriginService { get; set; }

    public List<Tuple<string, List<SearchStringPartOriginData>?>>? GetSearchStringPartsAndDataAboutTheirOrigin()
    {
        return SearchStringOriginService.GetSearchStringPartsAndDataAboutTheirOrigin(Product);
    }
}
