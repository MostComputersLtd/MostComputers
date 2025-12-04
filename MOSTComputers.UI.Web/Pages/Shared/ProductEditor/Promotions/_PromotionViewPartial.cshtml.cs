using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.Promotions;
public class PromotionViewPartialModel
{
    public required ModalData ModalData { get; init; }
    public required Product Product { get; init; }
    public required Promotion Promotion { get; init; }
}