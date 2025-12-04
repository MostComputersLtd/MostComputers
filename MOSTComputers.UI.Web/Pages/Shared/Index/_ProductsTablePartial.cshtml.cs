using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.Index;

public sealed class ProductsTablePartialModel
{
    public IReadOnlyList<Product>? Products { get; init; }
    public required ModalData ProductFullDisplayPartialModalData { get; init; }
    public required ModalData ProductHtmlAndImagesPartialModelData { get; init; }
}