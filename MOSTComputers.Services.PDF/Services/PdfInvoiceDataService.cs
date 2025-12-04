using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.PDF.Models.Invoices;
using MOSTComputers.Services.PDF.Services.Contracts;

namespace MOSTComputers.Services.PDF.Services;
internal sealed class PdfInvoiceDataService : IPdfInvoiceDataService
{
    public PdfInvoiceDataService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    private readonly IInvoiceRepository _invoiceRepository;

    public async Task<InvoiceData?> GetInvoiceDataByIdAsync(int invoiceId)
    {
        if (invoiceId < 0) return null;

        Invoice? invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);

        if (invoice is null) return null;

        return GetPdfInvoiceDataFromInvoice(invoice);
    }

    public async Task<InvoiceData?> GetInvoiceDataByNumberAsync(string invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber)) return null;

        Invoice? invoice = await _invoiceRepository.GetInvoiceByNumberAsync(invoiceNumber);

        if (invoice is null) return null;

        return GetPdfInvoiceDataFromInvoice(invoice);
    }

    public InvoiceData GetPdfInvoiceDataFromInvoice(Invoice invoice)
    {
        string? invoiceNumberAsString = invoice.InvoiceNumber?.Replace("C", string.Empty);

        bool invoiceNumberParseSuccess = int.TryParse(invoiceNumberAsString, out int invoiceNumber);

        return new()
        {
            InvoiceNumber = invoiceNumberParseSuccess ? invoiceNumber : 0,
            Date = invoice.InvoiceDate,
            RDate = invoice.RDATE,
            DueDate = invoice.DueDate,
            FirmId = invoice.FirmId,

            RecipientData = new()
            {
                FirmOrPersonId = invoice.Bulstat,
                FirmName = invoice.CustomerName,
                FirmAddress = invoice.CustomerAddress,
                MRPersonFullName = invoice.MPerson,
                VatId = invoice.Bulstat,
                BankId = invoice.CustomerBankNameAndId,
                Iban = invoice.CustomerBankIBAN,
            },
            SupplierData = new()
            {
                FirmName = "МОСТ КОМПЮТЪРС",
                FirmAddress = "бул. Шипченски проход, бл. 240, вх. Г",
                MRPersonFullName = "Георги Петров",
                VatId = "BG201343443",
                FirmOrPersonId = "201343443",
                BankId = "ОББ АД AIC: UBBSBGSA",
                Iban = "BG23UBBS89247238478234",
            },
            Purchases = invoice.InvoiceItems is not null ? GetPdfInvoicePurchaseDataFromInvoice(invoice.InvoiceItems) : new(),

            VatPercentageFraction = (invoice.VatPercent is not null) ? (float)invoice.VatPercent / 100 : 0F,
            RecipientFullName = invoice.RPerson,
            TypeOfPayment = TEMP_GET_PAYTYPE_STRING(invoice.PayType),
            AuthorFullName = invoice.PaymentStatusUserName,

            InvoiceDirection = invoice.InvoiceDirection,
            PratkaId = invoice.PratkaId,
            InvType = invoice.InvType,
            InvBasis = invoice.InvBasis,
            RelatedInvNo = invoice.RelatedInvNo,
            IsVATRegistered = invoice.IsVATRegistered,
        };
    }

    private static string TEMP_GET_PAYTYPE_STRING(int? payType)
    {
        return payType switch
        {
            -1 => string.Empty,
            1 => string.Empty,
            2 => "по банков път",
            _ => string.Empty,
        };
    }

    private static List<PurchaseInvoiceData> GetPdfInvoicePurchaseDataFromInvoice(IEnumerable<InvoiceItem> invoiceItems)
    {
        List<PurchaseInvoiceData> output = new();

        foreach (InvoiceItem invoiceItem in invoiceItems)
        {
            output.Add(GetPdfInvoicePurchaseDataFromInvoice(invoiceItem));
        }

        return output;
    }

    private static PurchaseInvoiceData GetPdfInvoicePurchaseDataFromInvoice(InvoiceItem invoiceItem)
    {
        return new()
        {
            ProductName = invoiceItem.Name,
            PricePerUnit = invoiceItem.PriceInLeva,
            Quantity = invoiceItem.Quantity,
            Currency = "лв",
            UnitOfMeasurement = "брой",
        };
    }
}