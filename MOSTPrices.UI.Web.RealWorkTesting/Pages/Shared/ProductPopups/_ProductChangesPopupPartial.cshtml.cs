using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductChangesPopupPartialModel
{
    public ProductChangesPopupPartialModel(
        Product? product,
        List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringOriginData)
    {
        Product = product;
        ProductSearchStringPartsAndDataAboutTheirOrigin = searchStringOriginData;
    }

    public Product? Product { get; }
    public List<Tuple<string, List<SearchStringPartOriginData>?>>? ProductSearchStringPartsAndDataAboutTheirOrigin { get; }
}