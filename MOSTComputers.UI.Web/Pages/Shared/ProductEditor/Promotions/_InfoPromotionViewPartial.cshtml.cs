using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.Promotions;
public sealed class InfoPromotionViewPartialModel
{
    public required ModalData ModalData { get; init; }
    public required Product Product { get; init; }
}