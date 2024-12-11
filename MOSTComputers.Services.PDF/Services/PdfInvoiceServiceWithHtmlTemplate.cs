using HtmlAgilityPack;
using MOSTComputers.Services.PDF.Models;
using MOSTComputers.Services.PDF.Services.Contracts;
using Spire.Pdf;
using System.Text;
using static MOSTComputers.Services.PDF.Utils.CurrencyToWordsConversionUtils;

namespace MOSTComputers.Services.PDF.Services;
internal class PdfInvoiceServiceWithHtmlTemplate : IPdfInvoiceService
{
    public PdfInvoiceServiceWithHtmlTemplate(string htmlTemplateFilePath)
    {
        _htmlTemplateFilePath = htmlTemplateFilePath;
    }

    private readonly string _htmlTemplateFilePath;

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

    public PdfDocument CreateInvoicePdf(InvoiceData invoiceData, string destinationFilePath)
    {
        HtmlDocument document = new();

        document.Load(_htmlTemplateFilePath);

        string invoiceDateAsString = invoiceData.Date.ToString("dd.MM.yyy");
        string invoiceDueDateAsString = invoiceData.DueDate.ToString("dd.MM.yyy");

        double totalPriceWithoutVat = GetTotalPriceOfPurchases(invoiceData.Purchases);

        string totalPriceWithoutVatString = GetCurrencyStringFromRoundedPrice(CeilingToTwoDecimals(totalPriceWithoutVat), "лв", true);

        double totalPriceChargedFromVat = totalPriceWithoutVat * invoiceData.VatPercentageFraction;

        string totalPriceChargedFromVatString = GetCurrencyStringFromRoundedPrice(CeilingToTwoDecimals(totalPriceChargedFromVat), "лв", true);

        double totalPrice = totalPriceWithoutVat + totalPriceChargedFromVat;

        string totalPriceString = GetCurrencyStringFromRoundedPrice(CeilingToTwoDecimals(totalPrice), "лв", true);

        string totalPriceInWords = ConvertPriceToWordsInLeva(totalPrice);

        double varPercentageAsPercent = invoiceData.VatPercentageFraction * 100;
        string varPercentageText = varPercentageAsPercent.ToString() + "%";

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeftFirmAndAddressElementId, invoiceData.RecipientData.FirmName + "<br>" + invoiceData.RecipientData.FirmAddress);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeftFirmElementId, invoiceData.RecipientData.FirmOrPersonId);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeftFirmVatElementId, invoiceData.RecipientData.VatId);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeftFirmMRPersonElementId, invoiceData.RecipientData.MRPersonFullName);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateRightFirmAndAddressElementId, invoiceData.SupplierData.FirmName + "<br>" + invoiceData.SupplierData.FirmAddress);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateRightFirmIdElementId, invoiceData.SupplierData.FirmOrPersonId);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateRightFirmVatIdElementId, invoiceData.SupplierData.VatId);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateRightFirmMRPersonElementId, invoiceData.SupplierData.MRPersonFullName);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateInvoiceIdElementId, invoiceData.InvoiceId);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateDateOfIssueElementId, invoiceDateAsString);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateDateOfTaxEventElementId, invoiceDateAsString);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateDueDateElementId, invoiceDueDateAsString);

        AddPurchasesToInvoiceList(invoiceData, document);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateSupplierBankIdElementId, invoiceData.SupplierData.BankId ?? string.Empty);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateSupplierIbanElementId, invoiceData.SupplierData.Iban ?? string.Empty);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateTotalPriceInInvoiceElementId, totalPriceWithoutVatString);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateTaxBaseElementId, totalPriceWithoutVatString);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateVatPercentElementId, varPercentageText);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateVatChargedElementId, totalPriceChargedFromVatString);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateTotalPriceElementId, totalPriceString);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateTotalPriceInWordsElementId, totalPriceInWords);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplatePaymentMethodElementId, invoiceData.TypeOfPayment);

        ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeftFirmMRPersonSignatureInitialsElementId, invoiceData.RecipientFullName);
        ChangeHtmlElementInnerHtml(document, _invoiceTemplateInvoiceCreatorElementId, invoiceData.AuthorFullName);

        //ChangeHtmlElementInnerHtml(document, _invoiceTemplateLeagalTextElementId, invoiceData.);

        document.Save(destinationFilePath, Encoding.UTF8);

        return new();
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

    private static bool ChangeHtmlElement(HtmlNode parentNode, string elementId, Action<HtmlNode> changeAction)
    {
        HtmlNode? element = parentNode.ChildNodes.FirstOrDefault(x => x.Id == elementId);

        if (element is null) return false;

        changeAction(element);

        return true;
    }
}