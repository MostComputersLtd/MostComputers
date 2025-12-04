using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.UI.Web.Models.ProductEditor;
public sealed class ProductEditorProductData
{
    public required Product Product { get; init; }
    public required LegacyXmlProduct? XmlProduct { get; init; }
    public ProductWorkStatuses? ProductStatuses { get; init; }
    public required int ProductPropertiesCount { get; init; }
    public required int DirectoryImagesCount { get; init; }
    public required List<ProductImageFileData>? FileNameData { get; init; }
    public required int LocalImagesCount { get; init; }
    public required int OriginalImagesCount { get; init; }
    public required bool OriginalFirstImageExists { get; init; }
    public List<Promotion>? ProductPromotions { get; init; }
    public List<SearchStringPartOriginData>? SearchStringParts { get; init; }
    public int? PromotionFilesCount { get; init; }
}