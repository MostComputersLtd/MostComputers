using HtmlAgilityPack;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Globalization;
using MOSTComputers.Services.PDF.Models.WarrantyCards;
using MOSTComputers.Services.PDF.Services.Contracts;

using static MOSTComputers.Services.PDF.Utils.HtmlBasicOperationUtils;

namespace MOSTComputers.Services.PDF.Services;
internal sealed class PdfWarrantyCardWithoutPricesFileGeneratorServiceWithHtmlTemplate : IPdfWarrantyCardWithoutPricesFileGeneratorService
{
    public PdfWarrantyCardWithoutPricesFileGeneratorServiceWithHtmlTemplate(
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

    private const string _warrantyCardTemplateTopFirmNameElementId = "warrantyCardTemplateTopFirmName";
    private const string _warrantyCardTemplateCardPurposeTextElementId = "warrantyCardTemplateCardPurposeText";
    private const string _warrantyCardTemplateCardIdInTitleElementId = "warrantyCardTemplateCardIdInTitle";
    private const string _warrantyCardTemplateTopCustomerNameElementId = "warrantyCardTemplateTopCustomerName";
    private const string _warrantyCardTemplateTopDateElementId = "warrantyCardTemplateTopDate";

    private const string _warrantyCardTemplateItemListElementId = "warObjectsTBody";

    private const string _warrantyCardTemplateItemElementIdPrefix = "warrantyCardTemplateItem";
    private const string _warrantyCardTemplateItemNameElementIdPrefix = "warrantyCardTemplateItemName";
    private const string _warrantyCardTemplateItemSerialNumberElementIdPrefix = "warrantyCardTemplateItemSerialNumber";
    private const string _warrantyCardTemplateItemWarrantyCardTermElementIdPrefix = "warrantyCardTemplateItemWarrantyCardTerm";

    private const string _warrantyCardTemplateTermInMonthsElementId = "warrantyCardTemplateTermInMonths";
    private const string _warrantyCardTemplateWarrantyRepairServicesNumberElementId = "warrantyCardTemplateWarrantyRepairServicesNumber";
    private const string _warrantyCardTemplateBottomFirmNameElementId = "warrantyCardTemplateBottomFirmName";

    private static readonly MarginOptions _defaultWarrantyCardPdfMargins = new() { Left = "0.4in", Right = "0.39in", Top = "0.4in", Bottom = "0.39in" };

    public async Task CreateWarrantyCardPdfAndSaveAsync(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData, string destinationFilePath)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(warrantyCardWithoutPricesData);

        await ConvertHtmlToPDFAndSaveAsync(htmlDocument, warrantyCardWithoutPricesData, destinationFilePath);
    }

    public async Task<byte[]> CreateWarrantyCardPdfAsync(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(warrantyCardWithoutPricesData);

        return await ConvertHtmlToPDFAndGetDataAsync(htmlDocument, warrantyCardWithoutPricesData);
    }

    public async Task<Stream> CreateWarrantyCardPdfAndGetStreamAsync(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(warrantyCardWithoutPricesData);

        return await ConvertHtmlToPDFAndGetStreamAsync(htmlDocument, warrantyCardWithoutPricesData);
    }

    private HtmlDocument CreateHtmlDocumentFromTemplateWithData(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData)
    {
        HtmlDocument htmlDocument = new();

        htmlDocument.Load(_htmlTemplateFilePath);

        string warrantyCardDateAsString = (warrantyCardWithoutPricesData.WarrantyCardDate is not null)
            ? warrantyCardWithoutPricesData.WarrantyCardDate.Value.ToString("d-MMM-yyyy", new CultureInfo("en-US"))
            : string.Empty;

        string warrantyTermMonthsString = (warrantyCardWithoutPricesData.WarrantyCardTermInMonths is not null)
            ? warrantyCardWithoutPricesData.WarrantyCardTermInMonths.Value.ToString()
            : string.Empty;

        string warrantyTermInMonthsDisplayString = (warrantyTermMonthsString != string.Empty) ? $"{warrantyTermMonthsString} м." : string.Empty;

        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateTopFirmNameElementId, "MOST COMPUTERS");
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateCardPurposeTextElementId, "Гаранционна карта");
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateCardIdInTitleElementId, warrantyCardWithoutPricesData.OrderId.ToString());
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateTopCustomerNameElementId, warrantyCardWithoutPricesData.CustomerName ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateTopDateElementId, warrantyCardDateAsString);

        AddItemsToList(warrantyCardWithoutPricesData, htmlDocument);

        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateTermInMonthsElementId, warrantyTermInMonthsDisplayString);
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateWarrantyRepairServicesNumberElementId, "02 91823");
        ChangeHtmlElementInnerHtml(htmlDocument, _warrantyCardTemplateBottomFirmNameElementId, "Мост Компютърс");

        ChangeHtmlElement(htmlDocument, _warrantyCardTemplateBodyElementId, x => x.RemoveClass(_warrantyCardTemplateBodyBackgroundDarkerClassName));

        return htmlDocument;
    }

    private static void AddItemsToList(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData, HtmlDocument document)
    {
        string originalItemsListItemElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemElementIdPrefix, 0);

        string originalWarrantyCardTemplateItemNameElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemNameElementIdPrefix, 0);
        string originalWarrantyCardTemplateItemSerialNumberElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemSerialNumberElementIdPrefix, 0);
        string originalWarrantyCardTemplateItemCardTermElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemWarrantyCardTermElementIdPrefix, 0);

        HtmlNode itemsTBody = document.GetElementbyId(_warrantyCardTemplateItemListElementId);

        HtmlNode originalListItem = document.GetElementbyId(originalItemsListItemElementId);

        originalListItem.Remove();

        if (warrantyCardWithoutPricesData.WarrantyCardItems is null) return;

        for (int i = 0; i < warrantyCardWithoutPricesData.WarrantyCardItems.Count; i++)
        {
            WarrantyCardItemWithoutPricesData warrantyCardItemData = warrantyCardWithoutPricesData.WarrantyCardItems[i];

            HtmlNode purchasesListItemCopy = originalListItem.CloneNode(true);

            string newWarrantyCardTemplateItemNameElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemNameElementIdPrefix, i);
            string newWarrantyCardTemplateItemSerialNumberElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemSerialNumberElementIdPrefix, i);
            string newWarrantyCardTemplateItemCardTermElementId = GetIdForNodeInListFromIndex(_warrantyCardTemplateItemWarrantyCardTermElementIdPrefix, i);

            string warrantyTermMonthsString = (warrantyCardItemData.WarrantyCardItemTermInMonths is not null)
                ? warrantyCardItemData.WarrantyCardItemTermInMonths.Value.ToString()
                : string.Empty;

            string warrantyTermInMonthsDisplayString = (warrantyTermMonthsString != string.Empty) ? $"{warrantyTermMonthsString} м." : string.Empty;

            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemNameElementId, warrantyCardItemData.ProductName ?? string.Empty);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemSerialNumberElementId, warrantyCardItemData.SerialNumber ?? string.Empty);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalWarrantyCardTemplateItemCardTermElementId, warrantyTermMonthsString);

            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemNameElementId, x => x.Id = newWarrantyCardTemplateItemNameElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemSerialNumberElementId, x => x.Id = newWarrantyCardTemplateItemSerialNumberElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalWarrantyCardTemplateItemCardTermElementId, x => x.Id = newWarrantyCardTemplateItemCardTermElementId);

            itemsTBody.AppendChild(purchasesListItemCopy);
        }
    }

    private async Task ConvertHtmlToPDFAndSaveAsync(HtmlDocument document,
        WarrantyCardWithoutPricesData warrantyCardWithoutPricesData, string destinationFilePath)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        await page.PdfAsync(destinationFilePath, GetPdfOptions(warrantyCardWithoutPricesData, _defaultWarrantyCardPdfMargins));
    }

    private async Task<byte[]> ConvertHtmlToPDFAndGetDataAsync(HtmlDocument document, WarrantyCardWithoutPricesData warrantyCardWithoutPricesData)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        return await page.PdfDataAsync(GetPdfOptions(warrantyCardWithoutPricesData, _defaultWarrantyCardPdfMargins));
    }

    private async Task<Stream> ConvertHtmlToPDFAndGetStreamAsync(HtmlDocument document, WarrantyCardWithoutPricesData warrantyCardWithoutPricesData)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        return await page.PdfStreamAsync(GetPdfOptions(warrantyCardWithoutPricesData, _defaultWarrantyCardPdfMargins));
    }

    private static PdfOptions GetPdfOptions(WarrantyCardWithoutPricesData warrantyCardWithoutPricesData, MarginOptions pdfMargins)
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
                        Гаранционна карта № {warrantyCardWithoutPricesData.OrderId}: Стр. <span class='pageNumber'></span> / <span class='totalPages'></span>
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
}