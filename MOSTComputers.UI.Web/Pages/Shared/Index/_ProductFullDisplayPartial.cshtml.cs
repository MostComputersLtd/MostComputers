using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.Index;

public sealed class ProductFullDisplayPartialModel
{
    public required ModalData ModalData { get; init; }
    public required Product Product { get; init; }
    public List<ProductCharacteristic>? ProductCharacteristics { get; init; }
    public List<ProductProperty>? ProductProperties { get; init; }
    public List<ProductImageData>? ProductImages { get; init; }
    public List<SearchStringPartOriginData>? SearchStringParts { get; init; }
}