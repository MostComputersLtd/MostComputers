
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.Index;

public class ProductHtmlAndImagesPartialModel
{
    public required ModalData ModalData { get; init; }
    public required Product Product { get; init; }
    public List<ProductImageData>? ProductImages { get; init; }
    public List<ProductImageFileData>? ProductImageFileNames { get; init; }
    public string? ProductHtml { get; init; }
}