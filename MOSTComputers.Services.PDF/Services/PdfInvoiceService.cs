using MOSTComputers.Services.PDF.Models;
using MOSTComputers.Services.PDF.Services.Contracts;
using OneOf;
using OneOf.Types;
using Spire.Pdf;
using Spire.Pdf.Fields;
using Spire.Pdf.Widget;

namespace MOSTComputers.Services.PDF.Services;
internal class PdfInvoiceService : IPdfInvoiceService
{
    public PdfInvoiceService(PdfBasicOperationsService pdfBasicOperationsService, string invoiceTempateFilePath)
    {
        _pdfBasicOperationsService = pdfBasicOperationsService;
        _invoiceTempateFilePath = invoiceTempateFilePath;
    }

    internal const string HeaderTextFieldId = "text_1ecqf";
    internal const string HeaderAddressFieldId = "text_2mjlo";

    internal const string InvoiceIdFieldId = "text_4zswp";
    internal const string TopDateFieldId = "text_53cnqj";

    internal const string TopLocationFieldId = "text_5rrqf";
    internal const string InvoiceOriginFieldId = "text_6etap";

    internal const string RecipientNameFieldId = "text_8rfyk";
    internal const string RecipientAddressFieldId = "text_9wghb";
    internal const string RecipientMRPersonFullNameFieldId = "text_10krfv";
    internal const string RecipientVatIdFieldId = "text_11ypdj";
    internal const string RecipientFirmOrPersonIdFieldId = "text_19hjpy";
    internal const string RecipientBankIdFieldId = "text_20vlmp";
    internal const string RecipientIbanFieldId = "text_55uznf";

    internal const string SupplierNameFieldId = "text_13nxtu";
    internal const string SupplierAddressFieldId = "text_14badc";
    internal const string SupplierMRPersonFullNameFieldId = "text_15fogs";
    internal const string SupplierVatIdFieldId = "text_16twzb";
    internal const string SupplierFirmOrPersonIdFieldId = "text_17kzox";
    internal const string SupplierBankIdFieldId = "text_18wnce";
    internal const string SupplierIbanFieldId = "text_56njji";

    internal const string PriceWithoutTaxesFieldId = "text_33elmo";
    internal const string VatPriceFieldId = "text_35uqeg";
    internal const string TotalPriceFieldId = "text_37qkpv";

    internal const string BottomDateFieldId = "text_41vzne";
    internal const string BottomRecipientFieldId = "text_44djtt";

    internal const string TypeOfPaymentFieldId = "text_47knsi";
    internal const string EndDateFieldId = "text_49gaqf";
    internal const string AuthorFieldId = "text_51zmnf";

    internal readonly PdfBasicOperationsService _pdfBasicOperationsService;
    private readonly string _invoiceTempateFilePath;

    public PdfDocument CreateInvoicePdf(InvoiceData invoiceData, string destinationFilePath)
    {
        PdfDocument pdfDocument = new();

        pdfDocument.LoadFromFile(_invoiceTempateFilePath);

        string dateAsString = invoiceData.Date.Date.ToString();
        string finalDateAsString = invoiceData.DueDate.Date.ToString();

        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, HeaderTextFieldId, x => x.Text = invoiceData.FirmInHeaderName);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, HeaderAddressFieldId, x => x.Text = invoiceData.FirmInHeaderAddress);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, InvoiceIdFieldId, x => x.Text = invoiceData.InvoiceId);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, TopDateFieldId, x => x.Text = dateAsString);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, TopLocationFieldId, x => x.Text = invoiceData.Location);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, InvoiceOriginFieldId, x => x.Text = invoiceData.InvoiceOriginText);

        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, RecipientNameFieldId, x => x.Text = invoiceData.RecipientData.FirmName);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, RecipientAddressFieldId, x => x.Text = invoiceData.RecipientData.FirmAddress);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, RecipientMRPersonFullNameFieldId, x => x.Text = invoiceData.RecipientData.MRPersonFullName);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, RecipientVatIdFieldId, x => x.Text = invoiceData.RecipientData.VatId);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, RecipientFirmOrPersonIdFieldId, x => x.Text = invoiceData.RecipientData.FirmOrPersonId);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, RecipientBankIdFieldId, x => x.Text = invoiceData.RecipientData.BankId ?? string.Empty);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, RecipientIbanFieldId, x => x.Text = invoiceData.RecipientData.Iban ?? string.Empty);

        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, SupplierNameFieldId, x => x.Text = invoiceData.SupplierData.FirmName);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, SupplierAddressFieldId, x => x.Text = invoiceData.SupplierData.FirmAddress);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, SupplierMRPersonFullNameFieldId, x => x.Text = invoiceData.SupplierData.MRPersonFullName);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, SupplierVatIdFieldId, x => x.Text = invoiceData.SupplierData.VatId);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, SupplierFirmOrPersonIdFieldId, x => x.Text = invoiceData.SupplierData.FirmOrPersonId);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, SupplierBankIdFieldId, x => x.Text = invoiceData.SupplierData.BankId ?? string.Empty);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, SupplierIbanFieldId, x => x.Text = invoiceData.SupplierData.Iban ?? string.Empty);

        double priceWithoutTaxes = 0;
        double priceIncreaseFromTaxes = 0;

        foreach (PurchaseInvoiceData purchaseInvoiceData in invoiceData.Purchases)
        {
            double totalPriceOfPurchase = purchaseInvoiceData.PricePerUnit * purchaseInvoiceData.Quantity;

            priceWithoutTaxes += totalPriceOfPurchase;

            priceIncreaseFromTaxes += totalPriceOfPurchase * invoiceData.VatPercentageFraction;
        }

        double fullPrice = priceWithoutTaxes + priceIncreaseFromTaxes;

        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, PriceWithoutTaxesFieldId, x => x.Text = priceWithoutTaxes.ToString("F2"));
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, VatPriceFieldId, x => x.Text = priceIncreaseFromTaxes.ToString("F2"));
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, TotalPriceFieldId, x => x.Text = fullPrice.ToString("F2"));

        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, BottomDateFieldId, x => x.Text = dateAsString);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, BottomRecipientFieldId, x => x.Text = invoiceData.RecipientFullName);

        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, TypeOfPaymentFieldId, x => x.Text = invoiceData.TypeOfPayment);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, EndDateFieldId, x => x.Text = finalDateAsString);
        ChangeFieldValue<PdfTextBoxFieldWidget>(pdfDocument, AuthorFieldId, x => x.Text = invoiceData.AuthorFullName);

        pdfDocument.SaveToFile(destinationFilePath);

        return pdfDocument;
    }

    private bool ChangeFieldValue<TField>(PdfDocument pdfDocument, string fieldName, Action<TField> changeValueAction)
        where TField : PdfField
    {
        OneOf<TField, NotFound> getFieldResult = _pdfBasicOperationsService.GetFieldIfExists<TField>(pdfDocument, fieldName);

        return getFieldResult.Match(
            field =>
            {
                changeValueAction(field);

                return true;
            },
            notFound => false);
    }
}