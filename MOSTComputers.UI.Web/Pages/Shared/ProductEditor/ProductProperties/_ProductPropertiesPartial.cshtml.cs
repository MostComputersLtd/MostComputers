using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductProperties;
public class ProductPropertiesPartialModel
{
    public required ModalData ModalData { get; init; }
    public Product? Product { get; init; }
    public List<ProductProperty>? ProductProperties { get; init; }
}