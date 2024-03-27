using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.XMLDataOperations.Models;
using static MOSTComputers.UI.Web.Pages.ProductCompareEditorModel;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductCompareEditor;

public class ProductFullEditorPartialModel
{
    public required Product? Product { get; set; }
    public required XmlProduct? XmlProduct { get; set; }
    public required List<Tuple<string, List<SearchStringPartOriginData>?>>? ProductSearchStringPartsAndDataAboutTheirOrigin { get; set; }
    public required List<ProductCharacteristic>? CharacteristicsRelatedToProduct { get; set; }
    public required string ElementIdAndNamePrefix { get; init; }
    public required string OtherElementIdAndNamePrefix { get; init; }
    public required string ImagesContainerId { get; init; }
    public required string OtherImagesContainerId { get; init; }

    private int _firstOrSecondProduct { get; set; }
    public required FirstOrSecondProductEnum FirstOrSecondProduct
    { init => _firstOrSecondProduct = (int)value; }

    public int ProductIndex => _firstOrSecondProduct;
}