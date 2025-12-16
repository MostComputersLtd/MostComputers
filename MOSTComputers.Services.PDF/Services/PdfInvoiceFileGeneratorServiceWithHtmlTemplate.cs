using HtmlAgilityPack;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using MOSTComputers.Services.PDF.Models.Invoices;
using MOSTComputers.Services.PDF.Services.Contracts;

using static MOSTComputers.Services.PDF.Utils.CurrencyBasicOperationUtils;
using static MOSTComputers.Services.PDF.Utils.CurrencyToWordsConversionUtils;
using static MOSTComputers.Services.PDF.Utils.HtmlBasicOperationUtils;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Models.Product.Models;
using OneOf;
using OneOf.Types;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.PDF.Services;
internal class PdfInvoiceFileGeneratorServiceWithHtmlTemplate : IPdfInvoiceFileGeneratorService
{
    public PdfInvoiceFileGeneratorServiceWithHtmlTemplate(
        string htmlTemplateFilePath,
        IBrowserProviderService browserProviderService)
    {
        _htmlTemplateFilePath = htmlTemplateFilePath;
        _browserProviderService = browserProviderService;
    }

    //private const decimal _levaToEuroConversionRate = 1.95583M;

    private readonly string _htmlTemplateFilePath;
    private readonly IBrowserProviderService _browserProviderService;

    private const string _invoiceTemplateBodyElementId = "invoiceTemplateBody";

    private const string _invoiceTemplateBodyBackgroundDarkerClassName = "invoiceTemplateBodyBackgroundDarker";

    private const string _invoiceTemplateElementId = "invoiceTemplateContainer_original";
    private const string _invoiceTemplateConstrainedToPageClassName = "invoiceTemplateConstrainedToPage";
    private const string _invoiceTemplateWithBorderClassName = "invoiceTemplateWithBorder";

    private const string _invoiceTemplateLeftFirmAndAddressElementId = "invoiceTemplateLeftFirmAndAddress";
    private const string _invoiceTemplateLeftFirmElementId = "invoiceTemplateLeftFirmId";
    private const string _invoiceTemplateLeftFirmVatElementId = "invoiceTemplateLeftFirmVatId";
    private const string _invoiceTemplateLeftFirmMRPersonElementId = "invoiceTemplateLeftFirmMRPerson";

    private const string _invoiceTemplateRightFirmAndAddressElementId = "invoiceTemplateRightFirmAndAddress";
    private const string _invoiceTemplateRightFirmIdElementId = "invoiceTemplateRightFirmId";
    private const string _invoiceTemplateRightFirmVatIdElementId = "invoiceTemplateRightFirmVatId";
    private const string _invoiceTemplateRightFirmMRPersonElementId = "invoiceTemplateRightFirmMRPerson";

    private const string _invoiceTemplateInvoiceIdElementId = "invoiceTemplateInvoiceId";

    private const string _invoiceTemplateDateOfIssueElementId = "invoiceTemplateDateOfIssue";
    private const string _invoiceTemplateDateOfTaxEventElementId = "invoiceTemplateDateOfTaxEvent";
    private const string _invoiceTemplateDueDateElementId = "invoiceTemplateDueDate";

    private const string _invoiceTemplateEntriesTBodyElementId = "invoiceTemplateEntriesTBody";

    private const string _invoiceTemplateEntryElementIdPrefix = "invoiceTemplateEntry";
    private const string _invoiceTemplateEntryNumberElementIdPrefix = "invoiceTemplateEntryNumber";
    private const string _invoiceTemplateEntryNameElementIdPrefix = "invoiceTemplateEntryName";
    private const string _invoiceTemplateEntryQuantityElementIdPrefix = "invoiceTemplateEntryQuantity";
    private const string _invoiceTemplateEntryUnitOfMeasureElementIdPrefix = "invoiceTemplateEntryUnitOfMeasure";
    private const string _invoiceTemplateEntryPricePerUnitElementIdPrefix = "invoiceTemplateEntryPricePerUnit";
    private const string _invoiceTemplateEntryTotalPriceElementIdPrefix = "invoiceTemplateEntryTotalPrice";
    private const string _invoiceTemplateEntryCurrencyElementIdPrefix = "invoiceTemplateEntryCurrency";

    private const string _invoiceTemplateSupplierBankIdElementId = "invoiceTemplateSupplierBankId";
    private const string _invoiceTemplateSupplierIbanElementId = "invoiceTemplateSupplierIban";

    private const string _invoiceTemplateTotalPriceInInvoiceElementId = "invoiceTemplateTotalPriceInInvoice";
    private const string _invoiceTemplateTaxBaseElementId = "invoiceTemplateTaxBase";

    private const string _invoiceTemplateVatPercentElementId = "invoiceTemplateVatPercent";
    private const string _invoiceTemplateVatChargedElementId = "invoiceTemplateVatCharged";

    private const string _invoiceTemplateTotalPriceLevaElementId = "invoiceTemplateTotalPriceLeva";

    
    private const string _invoiceTemplateTotalPriceEuroConversionRateElementId = "invoiceTemplateTotalPriceEuroConversionRate";
    private const string _invoiceTemplateTotalPriceEuroElementId = "invoiceTemplateTotalPriceEuro";

    private const string _invoiceTemplateTotalPriceInWordsElementId = "invoiceTemplateTotalPriceInWords";
    private const string _invoiceTemplatePaymentMethodElementId = "invoiceTemplatePaymentMethod";

    private const string _invoiceTemplateLeftFirmMRPersonSignatureInitialsElementId = "invoiceTemplateLeftFirmMRPersonSignatureInitials";
    private const string _invoiceTemplateInvoiceCreatorElementId = "invoiceTemplateInvoiceCreator";
    private const string _invoiceTemplateLeagalTextElementId = "invoiceTemplateLeagalText";

    private static readonly MarginOptions _defaultChromePdfMargins = new() { Left = "0.4in", Right = "0.39in", Top = "0.4in", Bottom = "0.39in" };

    internal static readonly string[] _defaultChromeBrowserOptions = new[]
    {
        "--no-sandbox",
        "--disable-setuid-sandbox",
        "--disable-gpu",
        "--disable-extensions",
        "--disable-background-networking",
        "--disable-sync",
        "--disable-translate",
        "--disable-default-apps",
        "--mute-audio"
    };

    public async Task CreateInvoicePdfAndSaveAsync(InvoiceData invoiceData, string destinationFilePath)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(invoiceData);

        await ConvertHtmlToPDFAndSaveAsync(htmlDocument, invoiceData, destinationFilePath);
    }

    public async Task<byte[]> CreateInvoicePdfAsync(InvoiceData invoiceData)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(invoiceData);

        return await ConvertHtmlToPDFAndGetDataAsync(htmlDocument, invoiceData);
    }

    public async Task<Stream> CreateInvoicePdfAndGetStreamAsync(InvoiceData invoiceData)
    {
        HtmlDocument htmlDocument = CreateHtmlDocumentFromTemplateWithData(invoiceData);

        return await ConvertHtmlToPDFAndGetStreamAsync(htmlDocument, invoiceData);
    }

    private HtmlDocument CreateHtmlDocumentFromTemplateWithData(InvoiceData invoiceData)
    {
        HtmlDocument htmlDocument = new();

        htmlDocument.Load(_htmlTemplateFilePath);

        string invoiceDateAsString = (invoiceData.Date is not null) ? invoiceData.Date.Value.ToString("dd.MM.yyy") : string.Empty;
        string invoiceDueDateAsString = (invoiceData.DueDate is not null) ? invoiceData.DueDate.Value.ToString("dd.MM.yyy") : string.Empty;

        decimal totalPriceWithoutVat = 0M;
        decimal totalPriceChargedFromVat = 0M;
        decimal totalPriceInLeva = 0M;
        decimal totalPriceInEuro = 0M;

        foreach (PurchaseInvoiceData priceData in invoiceData.Purchases)
        {
            totalPriceWithoutVat += priceData.LineNetPrice ?? 0M;
            totalPriceChargedFromVat += priceData.LineVatPrice ?? 0M;
            totalPriceInLeva += priceData.LineTotalPrice ?? 0M;
            totalPriceInEuro += priceData.LineTotalPriceEuro ?? 0M;
        }

        //decimal totalPriceWithoutVat = GetTotalPriceOfPurchases(invoiceData.Purchases);

        //decimal totalPriceChargedFromVat = totalPriceWithoutVat * (decimal)invoiceData.VatPercentageFraction;

        //decimal totalPriceInLeva = RoundCurrency(totalPriceWithoutVat + totalPriceChargedFromVat);

        //decimal totalPriceInEuro = RoundCurrency(totalPriceInLeva / _levaToEuroConversionRate);

        decimal varPercentageAsPercent = invoiceData.VatPercentageFraction * 100;

        string totalPriceWithoutVatString = GetCurrencyStringFromPrice(RoundCurrency(totalPriceWithoutVat), "лв", true);

        string totalPriceChargedFromVatString = GetCurrencyStringFromPrice(RoundCurrency(totalPriceChargedFromVat), "лв", true);

        string totalPriceInLevaString = GetCurrencyStringFromPrice(totalPriceInLeva, "лв", true);

        string totalPriceInEuroConverstionRateString = invoiceData.LevaToEuroExchangeRate.ToString();

        string totalPriceInEuroString = GetCurrencyStringFromPrice(totalPriceInEuro, "€", true);

        string totalPriceInWords = ConvertPriceToWordsInLeva(totalPriceInLeva);

        string varPercentageText = varPercentageAsPercent.ToString() + "%";

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmAndAddressElementId, invoiceData.RecipientData.FirmName + "<br>" + invoiceData.RecipientData.FirmAddress);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmElementId, invoiceData.RecipientData.FirmOrPersonId ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmVatElementId, invoiceData.RecipientData.VatId ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmMRPersonElementId, invoiceData.RecipientData.MRPersonFullName ?? string.Empty);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmAndAddressElementId, invoiceData.SupplierData.FirmName + "<br>" + invoiceData.SupplierData.FirmAddress);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmIdElementId, invoiceData.SupplierData.FirmOrPersonId ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmVatIdElementId, invoiceData.SupplierData.VatId ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmMRPersonElementId, invoiceData.SupplierData.MRPersonFullName ?? string.Empty);
        
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateInvoiceIdElementId, invoiceData.InvoiceNumber.ToString().PadLeft(10, '0'));

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateDateOfIssueElementId, invoiceDateAsString);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateDateOfTaxEventElementId, invoiceDateAsString);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateDueDateElementId, invoiceDueDateAsString);

        AddPurchasesToInvoiceList(invoiceData, htmlDocument);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateSupplierBankIdElementId, invoiceData.SupplierData.BankId ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateSupplierIbanElementId, invoiceData.SupplierData.Iban ?? string.Empty);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTotalPriceInInvoiceElementId, totalPriceWithoutVatString);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTaxBaseElementId, totalPriceWithoutVatString);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateVatPercentElementId, varPercentageText);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateVatChargedElementId, totalPriceChargedFromVatString);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTotalPriceLevaElementId, totalPriceInLevaString);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTotalPriceEuroConversionRateElementId, totalPriceInEuroConverstionRateString);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTotalPriceEuroElementId, totalPriceInEuroString);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTotalPriceInWordsElementId, totalPriceInWords);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplatePaymentMethodElementId, invoiceData.TypeOfPayment);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmMRPersonSignatureInitialsElementId, invoiceData.RecipientFullName ?? string.Empty);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateInvoiceCreatorElementId, invoiceData.AuthorFullName ?? string.Empty);

        //ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeagalTextElementId, invoiceData.);

        ChangeHtmlElement(htmlDocument, _invoiceTemplateBodyElementId, x => x.RemoveClass(_invoiceTemplateBodyBackgroundDarkerClassName));
        ChangeHtmlElement(htmlDocument, _invoiceTemplateElementId, x =>
        {
            IEnumerable<HtmlAttribute> classAttributes = x.Attributes.AttributesWithName("class");

            foreach (HtmlAttribute classAttribute in classAttributes)
            {
                string newClassNames = classAttribute.Value.Replace(_invoiceTemplateConstrainedToPageClassName, string.Empty)
                    .Replace(_invoiceTemplateWithBorderClassName, string.Empty)
                    .Trim();

                x.SetAttributeValue(classAttribute.Name, newClassNames);
            }
        });

        return htmlDocument;
    }

    private static void AddPurchasesToInvoiceList(InvoiceData invoiceData, HtmlDocument document)
    {
        string originalPurchasesListItemElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryElementIdPrefix, 0);

        string originalInvoiceTemplateEntryNumberElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryNumberElementIdPrefix, 0);
        string originalInvoiceTemplateEntryNameElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryNameElementIdPrefix, 0);
        string originalInvoiceTemplateEntryQuantityElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryQuantityElementIdPrefix, 0);
        string originalInvoiceTemplateEntryUnitOfMeasureElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryUnitOfMeasureElementIdPrefix, 0);
        string originalInvoiceTemplateEntryPricePerUnitElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryPricePerUnitElementIdPrefix, 0);
        string originalInvoiceTemplateEntryTotalPriceElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryTotalPriceElementIdPrefix, 0);
        string originalInvoiceTemplateEntryCurrencyElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryCurrencyElementIdPrefix, 0);

        HtmlNode purchasesEntriesTBody = document.GetElementbyId(_invoiceTemplateEntriesTBodyElementId);

        HtmlNode originalPurchasesListItem = document.GetElementbyId(originalPurchasesListItemElementId);

        originalPurchasesListItem.Remove();

        for (int i = 0; i < invoiceData.Purchases.Count; i++)
        {
            PurchaseInvoiceData purchaseData = invoiceData.Purchases[i];

            HtmlNode purchasesListItemCopy = originalPurchasesListItem.CloneNode(true);

            int purchaseNumber = i + 1;

            int quantity = purchaseData.Quantity ?? 0;
            decimal pricePerUnit = purchaseData.PricePerUnit ?? 0;

            decimal totalPrice = quantity * pricePerUnit;

            string newInvoiceTemplateEntryNumberElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryNumberElementIdPrefix, i);
            string newInvoiceTemplateEntryNameElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryNameElementIdPrefix, i);
            string newInvoiceTemplateEntryQuantityElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryQuantityElementIdPrefix, i);
            string newInvoiceTemplateEntryUnitOfMeasureElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryUnitOfMeasureElementIdPrefix, i);
            string newInvoiceTemplateEntryPricePerUnitElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryPricePerUnitElementIdPrefix, i);
            string newInvoiceTemplateEntryTotalPriceElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryTotalPriceElementIdPrefix, i);
            string newInvoiceTemplateEntryCurrencyElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryCurrencyElementIdPrefix, i);

            string pricePerUnitString = Math.Round(pricePerUnit, 2).ToString("F2");
            string totalPriceString = RoundCurrency(totalPrice).ToString("F2");

            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryNumberElementId, purchaseNumber.ToString());
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryNameElementId, purchaseData.ProductName ?? string.Empty);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryQuantityElementId, quantity.ToString());
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryUnitOfMeasureElementId, purchaseData.UnitOfMeasurement);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryPricePerUnitElementId, pricePerUnitString);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryTotalPriceElementId, totalPriceString);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryCurrencyElementId, purchaseData.Currency);

            ChangeHtmlElement(purchasesListItemCopy, originalInvoiceTemplateEntryNumberElementId, x => x.Id = newInvoiceTemplateEntryNumberElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalInvoiceTemplateEntryNameElementId, x => x.Id = newInvoiceTemplateEntryNameElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalInvoiceTemplateEntryQuantityElementId, x => x.Id = newInvoiceTemplateEntryQuantityElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalInvoiceTemplateEntryUnitOfMeasureElementId, x => x.Id = newInvoiceTemplateEntryUnitOfMeasureElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalInvoiceTemplateEntryPricePerUnitElementId, x => x.Id = newInvoiceTemplateEntryPricePerUnitElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalInvoiceTemplateEntryTotalPriceElementId, x => x.Id = newInvoiceTemplateEntryTotalPriceElementId);
            ChangeHtmlElement(purchasesListItemCopy, originalInvoiceTemplateEntryCurrencyElementId, x => x.Id = newInvoiceTemplateEntryCurrencyElementId);

            purchasesEntriesTBody.AppendChild(purchasesListItemCopy);
        }
    }

    private async Task ConvertHtmlToPDFAndSaveAsync(HtmlDocument document, InvoiceData invoiceData, string destinationFilePath)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        await page.PdfAsync(destinationFilePath, GetPdfOptions(invoiceData, _defaultChromePdfMargins));
    }

    private async Task<byte[]> ConvertHtmlToPDFAndGetDataAsync(HtmlDocument document, InvoiceData invoiceData)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        return await page.PdfDataAsync(GetPdfOptions(invoiceData, _defaultChromePdfMargins));
    }

    private async Task<Stream> ConvertHtmlToPDFAndGetStreamAsync(HtmlDocument document, InvoiceData invoiceData)
    {
        IBrowser browser = await _browserProviderService.GetBrowserAsync();

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        return await page.PdfStreamAsync(GetPdfOptions(invoiceData, _defaultChromePdfMargins));
    }

    private static PdfOptions GetPdfOptions(InvoiceData invoiceData, MarginOptions pdfMargins)
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
                        Фактура № {invoiceData.InvoiceNumber}: Стр. <span class='pageNumber'></span> / <span class='totalPages'></span>
                    </p>
                </div>
                """,
            DisplayHeaderFooter = true,
            OmitBackground = false,
            Scale = 1.25M,
            PrintBackground = true,
            MarginOptions = pdfMargins
        };
    }

    //private static decimal GetTotalPriceOfPurchases(IEnumerable<PurchaseInvoiceData> purchaseInvoiceData)
    //{
    //    decimal totalPrice = 0;

    //    foreach (PurchaseInvoiceData purchaseData in purchaseInvoiceData)
    //    {
    //        int quantity = purchaseData.Quantity ?? 0;

    //        decimal pricePerUnit = (purchaseData.PricePerUnit is not null) ? RoundCurrency(purchaseData.PricePerUnit.Value) : 0;

    //        totalPrice += quantity * pricePerUnit;
    //    }

    //    return totalPrice;
    //}
}