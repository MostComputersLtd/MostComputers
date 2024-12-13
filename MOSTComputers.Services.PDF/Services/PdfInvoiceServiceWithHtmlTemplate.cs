using HtmlAgilityPack;
using MOSTComputers.Services.PDF.Models;
using MOSTComputers.Services.PDF.Services.Contracts;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using static MOSTComputers.Services.PDF.Utils.CurrencyToWordsConversionUtils;

namespace MOSTComputers.Services.PDF.Services;
internal class PdfInvoiceServiceWithHtmlTemplate : IPdfInvoiceService
{
    public PdfInvoiceServiceWithHtmlTemplate(string htmlTemplateFilePath)
    {
        _htmlTemplateFilePath = htmlTemplateFilePath;
    }

    private readonly string _htmlTemplateFilePath;
    
    private const string _invoiceTemplateBodyElementId = "invoiceTemplateBody";

    private const string _invoiceTemplateBodyBackgroundDarkerClassName = "invoiceTemplateBodyBackgroundDarker";

    private const string _invoiceTemplateElementId = "invoiceTemplateContainer_original";
    private const string _invoiceTemplateConstrainedToPageClassName = "invoiceTemplateConstrainedToPage";

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

    private const string _invoiceTemplateTotalPriceElementId = "pamount_total";

    private const string _invoiceTemplateTotalPriceInWordsElementId = "invoiceTemplateTotalPriceInWords";
    private const string _invoiceTemplatePaymentMethodElementId = "invoiceTemplatePaymentMethod";

    private const string _invoiceTemplateLeftFirmMRPersonSignatureInitialsElementId = "invoiceTemplateLeftFirmMRPersonSignatureInitials";
    private const string _invoiceTemplateInvoiceCreatorElementId = "invoiceTemplateInvoiceCreator";
    private const string _invoiceTemplateLeagalTextElementId = "invoiceTemplateLeagalText";

    public async Task CreateInvoicePdf(InvoiceData invoiceData, string destinationFilePath)
    {
        HtmlDocument htmlDocument = new();

        htmlDocument.Load(_htmlTemplateFilePath);

        string invoiceDateAsString = invoiceData.Date.ToString("dd.MM.yyy");
        string invoiceDueDateAsString = invoiceData.DueDate.ToString("dd.MM.yyy");

        double totalPriceWithoutVat = GetTotalPriceOfPurchases(invoiceData.Purchases);

        string totalPriceWithoutVatString = GetCurrencyStringFromRoundedPrice(CeilingToTwoDecimals(totalPriceWithoutVat), "лв", true);

        double totalPriceChargedFromVat = totalPriceWithoutVat * invoiceData.VatPercentageFraction;

        string totalPriceChargedFromVatString = GetCurrencyStringFromRoundedPrice(CeilingToTwoDecimals(totalPriceChargedFromVat), "лв", true);

        double totalPrice = CeilingToTwoDecimals(totalPriceWithoutVat + totalPriceChargedFromVat);

        string totalPriceString = GetCurrencyStringFromRoundedPrice(totalPrice, "лв", true);

        string totalPriceInWords = ConvertPriceToWordsInLeva(totalPrice);

        double varPercentageAsPercent = invoiceData.VatPercentageFraction * 100;
        string varPercentageText = varPercentageAsPercent.ToString() + "%";

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmAndAddressElementId, invoiceData.RecipientData.FirmName + "<br>" + invoiceData.RecipientData.FirmAddress);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmElementId, invoiceData.RecipientData.FirmOrPersonId);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmVatElementId, invoiceData.RecipientData.VatId);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmMRPersonElementId, invoiceData.RecipientData.MRPersonFullName);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmAndAddressElementId, invoiceData.SupplierData.FirmName + "<br>" + invoiceData.SupplierData.FirmAddress);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmIdElementId, invoiceData.SupplierData.FirmOrPersonId);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmVatIdElementId, invoiceData.SupplierData.VatId);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateRightFirmMRPersonElementId, invoiceData.SupplierData.MRPersonFullName);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateInvoiceIdElementId, invoiceData.InvoiceId);

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

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTotalPriceElementId, totalPriceString);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateTotalPriceInWordsElementId, totalPriceInWords);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplatePaymentMethodElementId, invoiceData.TypeOfPayment);

        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateLeftFirmMRPersonSignatureInitialsElementId, invoiceData.RecipientFullName);
        ChangeHtmlElementInnerHtml(htmlDocument, _invoiceTemplateInvoiceCreatorElementId, invoiceData.AuthorFullName);

        //ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeagalTextElementId, invoiceData.);

        ChangeHtmlElement(htmlDocument, _invoiceTemplateBodyElementId, x => x.RemoveClass(_invoiceTemplateBodyBackgroundDarkerClassName));
        ChangeHtmlElement(htmlDocument, _invoiceTemplateElementId, x => x.RemoveClass(_invoiceTemplateConstrainedToPageClassName));

        await ConvertHtmlToPDFAndSaveAsync(htmlDocument, invoiceData, destinationFilePath);
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

            double totalPrice = purchaseData.Quantity * purchaseData.PricePerUnit;

            string newInvoiceTemplateEntryNumberElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryNumberElementIdPrefix, i);
            string newInvoiceTemplateEntryNameElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryNameElementIdPrefix, i);
            string newInvoiceTemplateEntryQuantityElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryQuantityElementIdPrefix, i);
            string newInvoiceTemplateEntryUnitOfMeasureElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryUnitOfMeasureElementIdPrefix, i);
            string newInvoiceTemplateEntryPricePerUnitElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryPricePerUnitElementIdPrefix, i);
            string newInvoiceTemplateEntryTotalPriceElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryTotalPriceElementIdPrefix, i);
            string newInvoiceTemplateEntryCurrencyElementId = GetIdForNodeInListFromIndex(_invoiceTemplateEntryCurrencyElementIdPrefix, i);

            string pricePerUnitString = Math.Round(purchaseData.PricePerUnit, 2).ToString("F2");
            string totalPriceString = CeilingToTwoDecimals(totalPrice).ToString("F2");

            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryNumberElementId, purchaseNumber.ToString());
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryNameElementId, purchaseData.ProductName);
            ChangeHtmlElementInnerHtml(purchasesListItemCopy, originalInvoiceTemplateEntryQuantityElementId, purchaseData.Quantity.ToString());
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

    private static async Task ConvertHtmlToPDFAndSaveAsync(HtmlDocument document, InvoiceData invoiceData, string destinationFilePath)
    {
        BrowserFetcher browserFetcher = new();

        await browserFetcher.DownloadAsync();

        IBrowser browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        using var page = await browser.NewPageAsync();

        await page.SetContentAsync(document.DocumentNode.OuterHtml);

        MarginOptions pdfMargins = new() { Left = "0.4in", Right = "0.39in", Top = "0.4in", Bottom = "0.39in" };

        await page.PdfAsync(destinationFilePath, new()
        {
            Format = PaperFormat.A4,
            HeaderTemplate = "<div id='header-template'></div>",
            FooterTemplate = $"<div id='footer-template' style='font-size:14px !important; width: 100%; margin-right: {pdfMargins.Right}; display: inline-flex; justify-content: flex-end;'><p>Фактура № {invoiceData.InvoiceId}: Стр. <span class='pageNumber'></span> / <span class='totalPages'></span></p></div>",
            DisplayHeaderFooter = true,
            OmitBackground = false,
            Scale = 1.25M,
            PrintBackground = true,
            MarginOptions = pdfMargins
        });
    }

    private static string GetCurrencyStringFromRoundedPrice(double roundedPrice, string currency, bool addWhiteSpaceBetweenPriceAndCurrency)
    {
        string output = roundedPrice.ToString("F2");
            
        if (addWhiteSpaceBetweenPriceAndCurrency)
        {
            output += " ";
        }
        
        return output + currency;
    }

    private static double CeilingToTwoDecimals(double value)
    {
        double newVal = Math.Round(value + 0.005, 2);

        return newVal;
    }

    private static double GetTotalPriceOfPurchases(IEnumerable<PurchaseInvoiceData> purchaseInvoiceData)
    {
        double totalPrice = 0;

        foreach (PurchaseInvoiceData purchaseData in purchaseInvoiceData)
        {
            totalPrice += purchaseData.Quantity * purchaseData.PricePerUnit;
        }

        return totalPrice;
    }

    private static string GetIdForNodeInListFromIndex(string nodeIdPrefix, int index)
    {
        return nodeIdPrefix + "-" + index;
    }

    private static bool ChangeHtmlElementInnerHtml(HtmlDocument document, string elementId, string newInnerHtml)
    {
        HtmlNode? element = document.GetElementbyId(elementId);

        if (element is null) return false;

        element.InnerHtml = newInnerHtml;

        return true;
    }

    private static bool ChangeHtmlElementInnerHtml(HtmlNode parentNode, string elementId, string newInnerHtml)
    {
        HtmlNode? element = parentNode.ChildNodes.FirstOrDefault(x => x.Id == elementId);

        if (element is null) return false;

        element.InnerHtml = newInnerHtml;

        return true;
    }

    private static bool ChangeHtmlElement(HtmlDocument htmlDocument, string elementId, Action<HtmlNode> changeAction)
    {
        HtmlNode? element = htmlDocument.GetElementbyId(elementId);

        if (element is null) return false;

        changeAction(element);

        return true;
    }

    private static bool ChangeHtmlElement(HtmlNode parentNode, string elementId, Action<HtmlNode> changeAction)
    {
        HtmlNode? element = parentNode.ChildNodes.FirstOrDefault(x => x.Id == elementId);

        if (element is null) return false;

        changeAction(element);

        return true;
    }
}