using HtmlAgilityPack;
using MOSTComputers.Services.PDF.Models.HomeOffers;
using MOSTComputers.Services.PDF.Services.Contracts;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using static MOSTComputers.Services.PDF.Utils.CurrencyBasicOperationUtils;
using static MOSTComputers.Services.PDF.Utils.HtmlBasicOperationUtils;

namespace MOSTComputers.Services.PDF.Services;

internal sealed class PdfHomeOfferFileGeneratorServiceWithHtmlTemplate : IPdfHomeOfferFileGeneratorService
{
    public PdfHomeOfferFileGeneratorServiceWithHtmlTemplate(
        string htmlTemplateFilePath,
        IBrowserProviderService browserProviderService)
    {
        _htmlTemplateFilePath = htmlTemplateFilePath;
        _browserProviderService = browserProviderService;
    }

    private readonly string _htmlTemplateFilePath;
    private readonly IBrowserProviderService _browserProviderService;

    private const string _homeOfferTemplateBodyElementId = "homeOfferTemplateBody";
    private const string _homeOfferTemplateBodyBackgroundDarkerClassName = "homeOfferTemplateBodyBackgroundDarker";
    private const string _homeOfferTemplateElementId = "homeOfferTemplateContainer_original";
    private const string _homeOfferTemplateOfferNumberElementId = "homeOfferTemplateOfferNumber";
    private const string _homeOfferTemplateOfferDateElementId = "homeOfferTemplateOfferDate";
    private const string _homeOfferObjectsTBodyElementId = "homeOfferObjectsTBody";
    private const string _homeOfferTemplateItemElementIdPrefix = "homeOfferTemplateItem";
    private const string _homeOfferTemplateItemIdElementIdPrefix = "homeOfferTemplateItemId";
    private const string _homeOfferTemplateItemNameElementIdPrefix = "homeOfferTemplateItemName";
    private const string _homeOfferTemplateItemQuantityElementIdPrefix = "homeOfferTemplateItemQuantity";
    private const string _homeOfferTemplateItemPricePerUnitElementIdPrefix = "homeOfferTemplateItemPricePerUnit";
    private const string _homeOfferTemplateItemTotalPriceElementIdPrefix = "homeOfferTemplateItemTotalPrice";
    private const string _homeOfferTemplateTotalNetPriceItemElementId = "homeOfferTemplateTotalNetPriceItem";
    private const string _homeOfferTemplateTotalNetPriceElementId = "homeOfferTemplateTotalNetPrice";
    private const string _homeOfferTemplateTotalVatPriceElementId = "homeOfferTemplateTotalVatPrice";
    private const string _homeOfferTemplateTotalPriceElementId = "homeOfferTemplateTotalPrice";
    private const string _homeOfferTemplateAdditionalInfoElementId = "homeOfferTemplateAdditionalInfo";

    private static readonly MarginOptions _defaultChromePdfMargins = new()
    {
        Left = "0.4in",
        Right = "0.39in",
        Top = "0.4in",
        Bottom = "0.39in"
    };

    public async Task CreateHomeOfferPdfAndSaveAsync(HomeOfferData homeOfferData, string destinationFilePath)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(homeOfferData);

        await ConvertHtmlToPDFAndSaveAsync(htmlDocument, homeOfferData, destinationFilePath);
    }

    public async Task<byte[]> CreateHomeOfferPdfAsync(HomeOfferData homeOfferData)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(homeOfferData);

        return await ConvertHtmlToPDFAndGetDataAsync(htmlDocument, homeOfferData);
    }

    public async Task<Stream> CreateHomeOfferPdfAndGetStreamAsync(HomeOfferData homeOfferData)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(homeOfferData);

        return await ConvertHtmlToPDFAndGetStreamAsync(htmlDocument, homeOfferData);
    }

    private HtmlDocument CreateHtmlDocumentFromTemplateWithData(HomeOfferData homeOfferData)
    {
        HtmlDocument htmlDocument = new();
        htmlDocument.Load(_htmlTemplateFilePath);

        string offerDateAsString = homeOfferData.Date is not null
            ? homeOfferData.Date.Value.ToString("dd.MM.yyyy")
            : string.Empty;

        string additionalInfo = homeOfferData.AdditionalInfo ?? string.Empty;

        ChangeHtmlElementInnerHtml(htmlDocument, _homeOfferTemplateOfferNumberElementId, homeOfferData.OfferNumber.ToString());
        ChangeHtmlElementInnerHtml(htmlDocument, _homeOfferTemplateOfferDateElementId, offerDateAsString);
        ChangeHtmlElementInnerHtml(htmlDocument, _homeOfferTemplateAdditionalInfoElementId, additionalInfo);

        decimal totalPriceWithoutVat = 0M;
        decimal totalPriceChargedFromVat = 0M;
        decimal totalPrice = 0M;

        foreach (HomeOfferItemData itemData in homeOfferData.Items)
        {
            totalPriceWithoutVat += itemData.LineNetPrice;
            totalPriceChargedFromVat += itemData.LineVatPrice;
            totalPrice += itemData.LineTotalPrice;
        }

        AddItemsAndSummaryToHomeOfferList(homeOfferData, htmlDocument);

        ChangeHtmlElementInnerHtml(htmlDocument, _homeOfferTemplateTotalNetPriceElementId, totalPriceWithoutVat.ToString("F2"));
        ChangeHtmlElementInnerHtml(htmlDocument, _homeOfferTemplateTotalVatPriceElementId, totalPriceChargedFromVat.ToString("F2"));
        ChangeHtmlElementInnerHtml(htmlDocument, _homeOfferTemplateTotalPriceElementId, totalPrice.ToString("F2"));

        ChangeHtmlElement(htmlDocument, _homeOfferTemplateBodyElementId, x =>
        {
            x.RemoveClass(_homeOfferTemplateBodyBackgroundDarkerClassName);
        });

        return htmlDocument;
    }

    private static void AddItemsAndSummaryToHomeOfferList(HomeOfferData homeOfferData, HtmlDocument document)
    {
        string originalHomeOfferTemplateItemElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemElementIdPrefix, 0);
        string originalHomeOfferTemplateItemIdElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemIdElementIdPrefix, 0);
        string originalHomeOfferTemplateItemNameElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemNameElementIdPrefix, 0);
        string originalHomeOfferTemplateItemQuantityElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemQuantityElementIdPrefix, 0);
        string originalHomeOfferTemplateItemPricePerUnitElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemPricePerUnitElementIdPrefix, 0);
        string originalHomeOfferTemplateItemTotalPriceElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemTotalPriceElementIdPrefix, 0);

        HtmlNode homeOfferObjectsTBody = document.GetElementbyId(_homeOfferObjectsTBodyElementId);
        HtmlNode originalHomeOfferTemplateItem = document.GetElementbyId(originalHomeOfferTemplateItemElementId);
        HtmlNode totalNetPriceSummaryItem = document.GetElementbyId(_homeOfferTemplateTotalNetPriceItemElementId);

        originalHomeOfferTemplateItem.Remove();

        for (int i = 0; i < homeOfferData.Items.Count; i++)
        {
            HomeOfferItemData itemData = homeOfferData.Items[i];
            HtmlNode homeOfferItemCopy = originalHomeOfferTemplateItem.CloneNode(true);

            string pricePerUnitString = RoundCurrency(itemData.PricePerUnit).ToString("F2");
            string totalPriceString = RoundCurrency(itemData.LineNetPrice).ToString("F2");

            string newHomeOfferTemplateItemElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemElementIdPrefix, i);
            string newHomeOfferTemplateItemIdElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemIdElementIdPrefix, i);
            string newHomeOfferTemplateItemNameElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemNameElementIdPrefix, i);
            string newHomeOfferTemplateItemQuantityElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemQuantityElementIdPrefix, i);
            string newHomeOfferTemplateItemPricePerUnitElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemPricePerUnitElementIdPrefix, i);
            string newHomeOfferTemplateItemTotalPriceElementId = GetIdForNodeInListFromIndex(_homeOfferTemplateItemTotalPriceElementIdPrefix, i);

            ChangeHtmlElementInnerHtml(homeOfferItemCopy, originalHomeOfferTemplateItemIdElementId, itemData.ProductId.ToString());
            ChangeHtmlElementInnerHtml(homeOfferItemCopy, originalHomeOfferTemplateItemNameElementId, itemData.ProductName ?? string.Empty);
            ChangeHtmlElementInnerHtml(homeOfferItemCopy, originalHomeOfferTemplateItemQuantityElementId, itemData.Quantity.ToString());
            ChangeHtmlElementInnerHtml(homeOfferItemCopy, originalHomeOfferTemplateItemPricePerUnitElementId, pricePerUnitString);
            ChangeHtmlElementInnerHtml(homeOfferItemCopy, originalHomeOfferTemplateItemTotalPriceElementId, totalPriceString);

            ChangeHtmlElement(homeOfferItemCopy, originalHomeOfferTemplateItemElementId, x => x.Id = newHomeOfferTemplateItemElementId);
            ChangeHtmlElement(homeOfferItemCopy, originalHomeOfferTemplateItemIdElementId, x => x.Id = newHomeOfferTemplateItemIdElementId);
            ChangeHtmlElement(homeOfferItemCopy, originalHomeOfferTemplateItemNameElementId, x => x.Id = newHomeOfferTemplateItemNameElementId);
            ChangeHtmlElement(homeOfferItemCopy, originalHomeOfferTemplateItemQuantityElementId, x => x.Id = newHomeOfferTemplateItemQuantityElementId);
            ChangeHtmlElement(homeOfferItemCopy, originalHomeOfferTemplateItemPricePerUnitElementId, x => x.Id = newHomeOfferTemplateItemPricePerUnitElementId);
            ChangeHtmlElement(homeOfferItemCopy, originalHomeOfferTemplateItemTotalPriceElementId, x => x.Id = newHomeOfferTemplateItemTotalPriceElementId);

            homeOfferObjectsTBody.InsertBefore(homeOfferItemCopy, totalNetPriceSummaryItem);
        }
    }

    private async Task ConvertHtmlToPDFAndSaveAsync(HtmlDocument document, HomeOfferData homeOfferData, string destinationFilePath)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using (IPage page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(document.DocumentNode.OuterHtml);
            await page.PdfAsync(destinationFilePath, GetPdfOptions(homeOfferData, _defaultChromePdfMargins));
        }
    }

    private async Task<byte[]> ConvertHtmlToPDFAndGetDataAsync(HtmlDocument document, HomeOfferData homeOfferData)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using (IPage page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(document.DocumentNode.OuterHtml);
            return await page.PdfDataAsync(GetPdfOptions(homeOfferData, _defaultChromePdfMargins));
        }
    }

    private async Task<Stream> ConvertHtmlToPDFAndGetStreamAsync(HtmlDocument document, HomeOfferData homeOfferData)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using (IPage page = await browser.NewPageAsync())
        {
            await page.SetContentAsync(document.DocumentNode.OuterHtml);
            return await page.PdfStreamAsync(GetPdfOptions(homeOfferData, _defaultChromePdfMargins));
        }
    }

    private static PdfOptions GetPdfOptions(HomeOfferData homeOfferData, MarginOptions pdfMargins)
    {
        return new()
        {
            Format = PaperFormat.A4,
            HeaderTemplate = "<div id='header-template'></div>",
            FooterTemplate =
                $"""
                <div id='footer-template' 
                    style='font-size:14px !important; width: 100%; height: 25px; align-content: end; margin-right: {pdfMargins.Right}; display: inline-flex; justify-content: flex-end;'>
                    <p>
                        Оферта № {homeOfferData.OfferNumber}: Стр. <span class='pageNumber'></span> / <span class='totalPages'></span>
                    </p>
                </div>
                """,
            DisplayHeaderFooter = true,
            OmitBackground = false,
            Scale = 0.64M,
            PrintBackground = true,
            MarginOptions = pdfMargins
        };
    }
}
