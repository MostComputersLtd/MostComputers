using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Utils;

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
        IEnumerable<SelectListItem> productXmlStatusSelectListItems,
        string? productFullWithXmlPopupContentId = null,
        string? productFullHtmlBasedPopupContentId = null,
        string? productChangesPopupContentId = null,
        string? productImagesPopupContentId = null,
        string? productFirstImagePopupContentId = null)
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

        ProductFullWithXmlPopupContentId = productFullWithXmlPopupContentId;
        ProductFullHtmlBasedPopupContentId = productFullHtmlBasedPopupContentId;

        ProductChangesPopupContentId = productChangesPopupContentId;
        ProductImagesPopupContentId = productImagesPopupContentId;
        ProductFirstImagePopupContentId = productFirstImagePopupContentId;
    }

    public IndexProductTableRowPartialModel(
        ProductDisplayData productData,
        string htmlElementId,
        int tableIndex,
        IEnumerable<Category> allPossibleCategories,
        IEnumerable<Manifacturer> allPossibleManifacturers,
        string? productFullWithXmlPopupContentId = null,
        string? productFullHtmlBasedPopupContentId = null,
        string? productChangesPopupContentId = null,
        string? productImagesPopupContentId = null,
        string? productFirstImagePopupContentId = null)
    {
        ProductDisplayData = productData;
        HtmlElementId = htmlElementId;
        TableIndex = tableIndex;

        CategorySelectListItems = ProductSelectListItemUtils.GetCategorySelectListItems(productData, allPossibleCategories);
        ManifacturerSelectListItems = ProductSelectListItemUtils.GetManifacturerSelectListItems(productData, allPossibleManifacturers);
        StatusSelectListItems = ProductSelectListItemUtils.GetStatusSelectListItems(productData);
        CurrencySelectListItems = ProductSelectListItemUtils.GetCurrencySelectListItems(productData);
        ProductNewStatusSelectListItems = ProductSelectListItemUtils.GetProductNewStatusSelectListItems(productData);
        ProductXmlStatusSelectListItems = ProductSelectListItemUtils.GetProductXmlStatusSelectListItems(productData);

        ProductFullWithXmlPopupContentId = productFullWithXmlPopupContentId;
        ProductFullHtmlBasedPopupContentId = productFullHtmlBasedPopupContentId;

        ProductChangesPopupContentId = productChangesPopupContentId;
        ProductImagesPopupContentId = productImagesPopupContentId;
        ProductFirstImagePopupContentId = productFirstImagePopupContentId;
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

    public string? ProductFullWithXmlPopupContentId { get; }
    public string? ProductFullHtmlBasedPopupContentId { get; }
    public string? ProductChangesPopupContentId { get; }
    public string? ProductImagesPopupContentId { get; }
    public string? ProductFirstImagePopupContentId { get; }
    
}