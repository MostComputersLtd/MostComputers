using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductProperties;

#pragma warning disable IDE1006 // Naming Styles
public class _ProductSinglePropertyDisplayPartialModel
#pragma warning restore IDE1006 // Naming Styles
{
    public _ProductSinglePropertyDisplayPartialModel(
        int productId,
        ProductProperty productProperty,
        int tableRowIndex,
        bool isNew,
        IEnumerable<SelectListItem>? remainingCharacteristics)
    {
        ProductId = productId;
        ProductProperty = productProperty;
        TableRowIndex = tableRowIndex;
        IsNew = isNew;
        RemainingCharacteristics = remainingCharacteristics;
    }

    public int ProductId { get; }
    public ProductProperty ProductProperty { get; }
    public int TableRowIndex { get; }
    public bool IsNew { get; }
    public IEnumerable<SelectListItem>? RemainingCharacteristics { get; }
}