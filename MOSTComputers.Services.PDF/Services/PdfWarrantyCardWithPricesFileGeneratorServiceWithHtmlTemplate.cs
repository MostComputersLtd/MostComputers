using HtmlAgilityPack;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Globalization;
using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;

using static MOSTComputers.Services.PDF.Utils.CurrencyBasicOperationUtils;
using static MOSTComputers.Services.PDF.Utils.HtmlBasicOperationUtils;

namespace MOSTComputers.Services.PDF.Services;
internal sealed class PdfWarrantyCardWithPricesFileGeneratorServiceWithHtmlTemplate : IPdfWarrantyCardWithPricesFileGeneratorService
{
    public PdfWarrantyCardWithPricesFileGeneratorServiceWithHtmlTemplate(
        string htmlTemplateFilePath,
        IBrowserProviderService browserProviderService)
    {
        _htmlTemplateFilePath = htmlTemplateFilePath;
        _browserProviderService = browserProviderService;
    }

    private readonly string _htmlTemplateFilePath;
    private readonly IBrowserProviderService _browserProviderService;
    private const string _warrantyCardTemplateBodyElementId = "warrantyCardTemplateBody";

    private const string _warrantyCardTemplateBodyBackgroundDarkerClassName = "warrantyCardTemplateBodyBackgroundDarker";

    private const string _warrantyCardTemplateHeaderFirmNameElementId = "warrantyCardTemplateHeaderFirmName";
    private const string _warrantyCardTemplateCardIdInTitleElementId = "warrantyCardTemplateCardIdInTitle";

    private const string _warrantyCardTemplateDateOfWarrantyElementId = "warrantyCardTemplateDateOfWarranty";
    private const string _warrantyCardTemplateExportUserElementId = "warrantyCardTemplateExportUser";
    private const string _warrantyCardTemplateCustomerNameTopElementId = "warrantyCardTemplateCustomerNameTop";
    private const string _warrantyCardTemplateCustomerTypeElementId = "warrantyCardTemplateCustomerType";
    private const string _warrantyCardTemplateCardIdInDescriptionElementId = "warrantyCardTemplateCardIdInDescription";
    private const string _warrantyCardTemplateDateOfProductOrderElementId = "warrantyCardTemplateDateOfProductOrder";

    private const string _warrantyCardTemplateItemListElementId = "warObjectsTBody";

    private const string _warrantyCardTemplateItemElementIdPrefix = "warrantyCardTemplateItem";
    private const string _warrantyCardTemplateItemNameElementIdPrefix = "warrantyCardTemplateItemName";
    private const string _warrantyCardTemplateItemSerialNumberElementIdPrefix = "warrantyCardTemplateItemSerialNumber";
    private const string _warrantyCardTemplateItemWarrantyCardTermElementIdPrefix = "warrantyCardTemplateItemWarrantyCardTerm";
    private const string _warrantyCardTemplateItemQuantityElementIdPrefix = "warrantyCardTemplateItemQuantity";
    private const string _warrantyCardTemplateItemPricePerUnitElementIdPrefix = "warrantyCardTemplateItemPricePerUnit";
    private const string _warrantyCardTemplateItemTotalPriceElementIdPrefix = "warrantyCardTemplateItemTotalPrice";

    private const string _warrantyCardTemplateTotalPriceElementId = "warrantyCardTemplateTotalPrice";
    private const string _warrantyCardTemplateCustomerNameBottomElementId = "warrantyCardTemplateCustomerNameBottom";
    private const string _warrantyCardTemplateBottomDateElementId = "warrantyCardTemplateBottomDate";

    private static readonly MarginOptions _defaultWarrantyCardPdfMargins = new() { Left = "0.4in", Right = "0.39in", Top = "0.4in", Bottom = "0.39in" };

    public async Task CreateWarrantyCardPdfAndSaveAsync(WarrantyCardWithPricesData warrantyCardWithPricesData, string destinationFilePath)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(warrantyCardWithPricesData);

        await ConvertHtmlToPDFAndSaveAsync(htmlDocument, warrantyCardWithPricesData, destinationFilePath);
    }

    public async Task<byte[]> CreateWarrantyCardPdfAsync(WarrantyCardWithPricesData warrantyCardWithPricesData)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(warrantyCardWithPricesData);

        return await ConvertHtmlToPDFAndGetDataAsync(htmlDocument, warrantyCardWithPricesData);
    }

    private HtmlDocument CreateHtmlDocumentFromTemplateWithData(WarrantyCardWithPricesData warrantyCardWithPricesData)
    {
        HtmlDocument htmlDocument = new();

        htmlDocument.Load(_htmlTemplateFilePath);

        string warrantyCardDateAsString = (warrantyCardWithPricesData.WarrantyCardDate is not null)
            ? warrantyCardWithPricesData.WarrantyCardDate.Value.ToString("d-MMM-yyyy", new CultureInfo("en-US"))
            : string.Empty;

        string todayDateAsString = DateTime.Today.ToString("dd.MM.yyyy");

        string warrantyTermMonthsString = (warrantyCardWithPricesData.WarrantyCardTermInMonths is not null)
            ? warrantyCardWithPricesData.WarrantyCardTermInMonths.Value.ToString() : string.Empty;

        string warrantyTermInMonthsDisplayString = (warrantyTermMonthsString != string.Empty) ? $"{warrantyTermMonthsString} м." : string.Empty;

        decimal totalPrice = (warrantyCardWithPricesData.WarrantyCardItems is not null)
            ? GetTotalPriceOfItems(warrantyCardWithPricesData.WarrantyCardItems)
            : 0;

        string totalPriceDisplayString = GetCurrencyStringFromPrice(totalPrice, "лв.", true);

        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateHeaderFirmNameElementId, "MOST COMPUTERS");
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateCardIdInTitleElementId, warrantyCardWithPricesData.OrderId.ToString());

        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateDateOfWarrantyElementId, warrantyCardDateAsString);
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateExportUserElementId, warrantyCardWithPricesData.ExportUser ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateCustomerNameTopElementId, warrantyCardWithPricesData.CustomerName ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateCustomerTypeElementId, "(КЛИЕНТ)");
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateCardIdInDescriptionElementId, warrantyCardWithPricesData.OrderId.ToString());
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateDateOfProductOrderElementId, warrantyCardDateAsString);

        AddItemsToList(warrantyCardWithPricesData, htmlDocument);

        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateTotalPriceElementId, totalPriceDisplayString);
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateCustomerNameBottomElementId, warrantyCardWithPricesData.CustomerName ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateBottomDateElementId, todayDateAsString);

        ChangeHtmlElement(htmlDocument, _warrantyCardTemplateBodyElementId, x => x.RemoveClass(_warrantyCardTemplateBodyBackgroundDarkerClassName));

        return htmlDocument;
    }

    private static void AddItemsToList(WarrantyCardWithPricesData warrantyCardWithPricesData, HtmlDocument document)
    {
        string originalItemsListItemElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemElementIdPrefix, 0);

        string originalWarrantyCardTemplateItemNameElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemNameElementIdPrefix, 0);
        string originalWarrantyCardTemplateItemSerialNumberElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemSerialNumberElementIdPrefix, 0);
        string originalWarrantyCardTemplateItemCardTermElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemWarrantyCardTermElementIdPrefix, 0);
        string originalWarrantyCardTemplateItemQuantityElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemQuantityElementIdPrefix, 0);
        string originalWarrantyCardTemplateItemPricePerUnitElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemPricePerUnitElementIdPrefix, 0);
        string originalWarrantyCardTemplateItemTotalPriceElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemTotalPriceElementIdPrefix, 0);

        HtmlNode itemsTBody = document.GetElementbyId(_warrantyCardTemplateItemListElementId);

        HtmlNode originalListItem = document.GetElementbyId(originalItemsListItemElementId);

        originalListItem.Remove();

        if (warrantyCardWithPricesData.WarrantyCardItems is null) return;

        for (int i = 0; i < warrantyCardWithPricesData.WarrantyCardItems.Count; i++)
        {
            WarrantyCardItemWithPricesData warrantyCardItemData = warrantyCardWithPricesData.WarrantyCardItems[i];

            HtmlNode purchasesListItemCopy = originalListItem.CloneNode(true);

            string newWarrantyCardTemplateItemNameElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemNameElementIdPrefix, i);
            string newWarrantyCardTemplateItemSerialNumberElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemSerialNumberElementIdPrefix, i);
            string newWarrantyCardTemplateItemCardTermElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemWarrantyCardTermElementIdPrefix, i);
            string newWarrantyCardTemplateItemQuantityElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemQuantityElementIdPrefix, i);
            string newWarrantyCardTemplateItemPricePerUnitElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemPricePerUnitElementIdPrefix, i);
            string newWarrantyCardTemplateItemTotalPriceElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemTotalPriceElementIdPrefix, i);

            string warrantyTermMonthsString = (warrantyCardItemData.WarrantyCardItemTermInMonths is not null)
                ? warrantyCardItemData.WarrantyCardItemTermInMonths.Value.ToString()
                : string.Empty;

            string warrantyTermInMonthsDisplayString = (warrantyTermMonthsString != string.Empty) ? $"{warrantyTermMonthsString} м." : string.Empty;

            int quantity = warrantyCardItemData.Quantity ?? 0;
            decimal pricePerUnit = warrantyCardItemData.PriceInLeva ?? 0;

            decimal totalPrice = quantity * pricePerUnit;

            string pricePerUnitString = Math.Round(pricePerUnit, 2).ToString("F2");
            string totalPriceString = RoundCurrency(totalPrice).ToString("F2");

            string pricePerUnitDisplayString = $"{pricePerUnitString} лв.";
            string totalPriceDisplayString = $"{totalPriceString} лв.";

            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemNameElementId, warrantyCardItemData.ProductName ?? string.Empty);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemSerialNumberElementId, warrantyCardItemData.SerialNumber ?? string.Empty);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemCardTermElementId, warrantyTermMonthsString);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemQuantityElementId, quantity.ToString());
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemPricePerUnitElementId, pricePerUnitDisplayString);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemTotalPriceElementId, totalPriceDisplayString);

            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemNameElementId, x => x.Id = newWarrantyCardTemplateItemNameElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemSerialNumberElementId, x => x.Id = newWarrantyCardTemplateItemSerialNumberElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemCardTermElementId, x => x.Id = newWarrantyCardTemplateItemCardTermElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemQuantityElementId, x => x.Id = newWarrantyCardTemplateItemQuantityElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemPricePerUnitElementId, x => x.Id = newWarrantyCardTemplateItemPricePerUnitElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemTotalPriceElementId, x => x.Id = newWarrantyCardTemplateItemTotalPriceElementId);

            itemsTBody.AppendChild(purchasesListItemCopy);
        }
    }

    private async Task ConvertHtmlToPDFAndSaveAsync(HtmlDocument document,
        WarrantyCardWithPricesData warrantyCardWithPricesData, string destinationFilePath)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        await page.PdfAsync(destinationFilePath, GetPdfOptions(warrantyCardWithPricesData, _defaultWarrantyCardPdfMargins));
    }

    private async Task<byte[]> ConvertHtmlToPDFAndGetDataAsync(HtmlDocument document, WarrantyCardWithPricesData warrantyCardWithPricesData)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        return await page.PdfDataAsync(GetPdfOptions(warrantyCardWithPricesData, _defaultWarrantyCardPdfMargins));
    }

    private static PdfOptions GetPdfOptions(WarrantyCardWithPricesData warrantyCardWithPricesData, MarginOptions pdfMargins)
    {
        return new()
        {
            Format = PaperFormat.A4,
            HeaderTemplate = "<div id='header-template'></div>",
            FooterTemplate =
                $"""
                <div id='footer-template' 
                    style='font-size:14px !important; width: 100%; height: 25px; align-content: end; margin-right: 0.4in; display: inline-flex; justify-content: flex-end;'>
                    <p>
                        Гаранционна карта № {warrantyCardWithPricesData.OrderId}: Стр. <span class='pageNumber'></span> / <span class='totalPages'></span>
                    </p>
                </div>
                """,
            DisplayHeaderFooter = true,
            OmitBackground = false,
            Scale = 0.89M,
            PrintBackground = true,
            MarginOptions = pdfMargins
        };
    }

    private static decimal GetTotalPriceOfItems(IEnumerable<WarrantyCardItemWithPricesData> warrantyCardItems)
    {
        decimal totalPrice = 0;

        foreach (WarrantyCardItemWithPricesData warrantyCardItem in warrantyCardItems)
        {
            int quantity = warrantyCardItem.Quantity ?? 0;

            decimal pricePerUnit = (warrantyCardItem.PriceInLeva is not null) ? RoundCurrency(warrantyCardItem.PriceInLeva.Value) : 0;

            totalPrice += quantity * pricePerUnit;
        }

        return totalPrice;
    }
}