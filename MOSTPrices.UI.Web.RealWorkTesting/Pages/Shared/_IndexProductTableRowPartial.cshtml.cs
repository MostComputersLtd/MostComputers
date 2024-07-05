using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared;

public class IndexProductTableRowPartialModel
{
    public IndexProductTableRowPartialModel(
        ProductDisplayData product,
        string htmlElementId,
        int tableIndex,
        IEnumerable<SelectListItem> categorySelectListItems,
        IEnumerable<SelectListItem> manifacturerSelectListItems,
        IEnumerable<SelectListItem> statusSelectListItems,
        IEnumerable<SelectListItem> currencySelectListItems,
        IEnumerable<SelectListItem> productNewStatusSelectListItems,
        IEnumerable<SelectListItem> productXmlStatusSelectListItems)
    {
        ProductDisplayData = product;
        HtmlElementId = htmlElementId;
        TableIndex = tableIndex;
        CategorySelectListItems = categorySelectListItems;
        ManifacturerSelectListItems = manifacturerSelectListItems;
        StatusSelectListItems = statusSelectListItems;
        CurrencySelectListItems = currencySelectListItems;
        ProductNewStatusSelectListItems = productNewStatusSelectListItems;
        ProductXmlStatusSelectListItems = productXmlStatusSelectListItems;
    }

    public ProductDisplayData ProductDisplayData { get; }
    public string HtmlElementId { get; }
    public int TableIndex { get; }
    public IEnumerable<SelectListItem> CategorySelectListItems { get; }
    public IEnumerable<SelectListItem> ManifacturerSelectListItems { get; }
    public IEnumerable<SelectListItem> StatusSelectListItems { get; }
    public IEnumerable<SelectListItem> CurrencySelectListItems { get; }
    public IEnumerable<SelectListItem> ProductNewStatusSelectListItems { get; }
    public IEnumerable<SelectListItem> ProductXmlStatusSelectListItems { get; }
}