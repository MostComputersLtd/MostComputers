using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models;
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
        ModalData productFullWithXmlPopupContentId,
        ModalData productFullHtmlBasedPopupContentId,
        ModalData productChangesPopupContentId,
        ModalData productImagesPopupContentId,
        ModalData productFirstImagePopupContentId)
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

        ProductFullWithXmlPopupData = productFullWithXmlPopupContentId;
        ProductFullHtmlBasedPopupData = productFullHtmlBasedPopupContentId;

        ProductChangesPopupData = productChangesPopupContentId;
        ProductImagesPopupData = productImagesPopupContentId;
        ProductFirstImagePopupData = productFirstImagePopupContentId;
    }

    public IndexProductTableRowPartialModel(
        ProductDisplayData productData,
        string htmlElementId,
        int tableIndex,
        IEnumerable<Category> allPossibleCategories,
        IEnumerable<Manifacturer> allPossibleManifacturers,
        ModalData productFullWithXmlPopupContentId,
        ModalData productFullHtmlBasedPopupContentId,
        ModalData productChangesPopupContentId,
        ModalData productImagesPopupContentId,
        ModalData productFirstImagePopupContentId)
    {
        ProductDisplayData = productData;
        HtmlElementId = htmlElementId;
        TableIndex = tableIndex;

        CategorySelectListItems = SelectListItemUtils.GetCategorySelectListItems(productData, allPossibleCategories);
        ManifacturerSelectListItems = SelectListItemUtils.GetManifacturerSelectListItems(productData, allPossibleManifacturers);
        StatusSelectListItems = SelectListItemUtils.GetStatusSelectListItems(productData);
        CurrencySelectListItems = SelectListItemUtils.GetCurrencySelectListItems(productData);
        ProductNewStatusSelectListItems = SelectListItemUtils.GetProductNewStatusSelectListItems(productData);
        ProductXmlStatusSelectListItems = SelectListItemUtils.GetProductXmlStatusSelectListItems(productData);

        ProductFullWithXmlPopupData = productFullWithXmlPopupContentId;
        ProductFullHtmlBasedPopupData = productFullHtmlBasedPopupContentId;

        ProductChangesPopupData = productChangesPopupContentId;
        ProductImagesPopupData = productImagesPopupContentId;
        ProductFirstImagePopupData = productFirstImagePopupContentId;
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

    public ModalData ProductFullWithXmlPopupData { get; }
    public ModalData ProductFullHtmlBasedPopupData { get; }
    public ModalData ProductChangesPopupData { get; }
    public ModalData ProductImagesPopupData { get; }
    public ModalData ProductFirstImagePopupData { get; }
}