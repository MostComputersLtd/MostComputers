using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductImages;
public class ProductImageFileNameInfoViewPartialModel
{
    public required ModalData ModalData { get; init; }
    public required int ProductId { get; init; }
    public string? ProductName { get; init; }
    public List<ProductImageFileData>? ImageFileNameInfos { get; init; }
}