using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductGeneratedXmlPopupPartialModel
{
    public required ModalData ModalData { get; init; }
    public required string XmlData { get; set; }
    public required Product Product { get; set; }
    public string? NotificationBoxId { get; set; } = null;
}
