using Microsoft.AspNetCore.Mvc.Rendering;

namespace MOSTComputers.UI.Web.Pages.Shared;

public class ProductsAndStatusesDisplayPartialTableModel
{
    public required List<ProductAndStatuses> ProductsAndStatuses { get; set; }
    public required IEnumerable<SelectListItem> AllCategoriesSelectItems { get; set; }
    public string? CurrentSubStringForSearchStringSearches { get; set; }
}