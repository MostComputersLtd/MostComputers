using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductProperties;

public class ProductSinglePropertyDisplayPartialModel
{
    public ProductSinglePropertyDisplayPartialModel(
        int productId,
        ProductPropertyDisplayData productProperty,
        int tableRowIndex,
        bool isNew,
        IEnumerable<SelectListItem>? remainingCharacteristics,
        string? notificationBoxId = null)
    {
        ProductId = productId;
        ProductPropertyData = productProperty;
        TableRowIndex = tableRowIndex;
        IsNew = isNew;
        RemainingCharacteristics = remainingCharacteristics;
        NotificationBoxId = notificationBoxId;
    }

    public int ProductId { get; }
    public ProductPropertyDisplayData ProductPropertyData { get; }
    public int TableRowIndex { get; }
    public bool IsNew { get; }
    public IEnumerable<SelectListItem>? RemainingCharacteristics { get; }
    public string? NotificationBoxId { get; }
}