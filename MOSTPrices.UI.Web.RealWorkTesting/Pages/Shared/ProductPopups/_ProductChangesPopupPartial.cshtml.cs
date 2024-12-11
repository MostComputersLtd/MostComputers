using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductChangesPopupPartialModel
{
    public required ModalData ModalData { get; init; }
    public required ModalData SearchStringPopupModalData { get; init; }
    public Product? Product { get; init; }
    public List<Tuple<string, List<SearchStringPartOriginData>?>>? ProductSearchStringPartsAndDataAboutTheirOrigin { get; init; }
}