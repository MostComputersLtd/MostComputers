using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.PDF.Models.Invoices;
using MOSTComputers.Services.PDF.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.PDF.Services;
internal sealed class PdfInvoiceDataService : IPdfInvoiceDataService
{
    public PdfInvoiceDataService(
        IInvoiceRepository invoiceRepository,
        ICurrencyVATService currencyVATService,
        IExchangeRateService exchangeRateService,
        ICurrencyConversionService currencyConversionService)
    {
        _invoiceRepository = invoiceRepository;
        _currencyVATService = currencyVATService;
        _exchangeRateService = exchangeRateService;
        _currencyConversionService = currencyConversionService;
    }

    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ICurrencyVATService _currencyVATService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ICurrencyConversionService _currencyConversionService;

    public async Task<InvoiceData?> GetInvoiceDataByIdAsync(int invoiceId)
    {
        if (invoiceId < 0) return null;

        Invoice? invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);

        if (invoice is null) return null;

        return await GetPdfInvoiceDataFromInvoiceAsync(invoice);
    }

    public async Task<InvoiceData?> GetInvoiceDataByNumberAsync(string invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber)) return null;

        Invoice? invoice = await _invoiceRepository.GetInvoiceByNumberAsync(invoiceNumber);

        if (invoice is null) return null;

        return await GetPdfInvoiceDataFromInvoiceAsync(invoice);
    }

    public async Task<InvoiceData> GetPdfInvoiceDataFromInvoiceAsync(Invoice invoice)
    {
        string? invoiceNumberAsString = invoice.InvoiceNumber?.Replace("C", string.Empty);

        bool invoiceNumberParseSuccess = int.TryParse(invoiceNumberAsString, out int invoiceNumber);

        decimal vatPercentageFraction = (invoice.VatPercent is not null) ? (decimal)invoice.VatPercent / 100 : 0M;

        ExchangeRate? levaToEuroExchangeRateObject = await _exchangeRateService.GetForCurrenciesAsync(Currency.BGN, Currency.EUR);

        decimal levaToEuroExchangeRate = levaToEuroExchangeRateObject?.Rate ?? 1.95583M;

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
            Purchases = invoice.InvoiceItems is not null ? GetPdfInvoicePurchaseDataFromInvoice(invoice.InvoiceItems, vatPercentageFraction, levaToEuroExchangeRate) : new(),

            VatPercentageFraction = vatPercentageFraction,
            LevaToEuroExchangeRate = levaToEuroExchangeRate,
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

    private List<PurchaseInvoiceData> GetPdfInvoicePurchaseDataFromInvoice(IEnumerable<InvoiceItem> invoiceItems, decimal vatPercentage, decimal levaToEuroExchangeRate)
    {
        List<PurchaseInvoiceData> output = new();

        foreach (InvoiceItem invoiceItem in invoiceItems)
        {
            output.Add(GetPdfInvoicePurchaseDataFromInvoiceData(invoiceItem, vatPercentage, levaToEuroExchangeRate));
        }

        return output;
    }

    private PurchaseInvoiceData GetPdfInvoicePurchaseDataFromInvoiceData(
        InvoiceItem invoiceItem, decimal vatPercentage, decimal levaToEuroExchangeRate)
    {
        decimal? netPrice = null;
        decimal? vatPrice = null;
        decimal? totalPrice = null;
        decimal? totalPriceInEuro = null;

        if (invoiceItem.PriceInLeva is not null
            && invoiceItem.Quantity is not null)
        {
            netPrice = invoiceItem.PriceInLeva.Value * invoiceItem.Quantity.Value;

            vatPrice = _currencyVATService.CalculateVAT(invoiceItem.PriceInLeva.Value, invoiceItem.Quantity.Value, vatPercentage);

            totalPrice = netPrice + vatPrice;

            totalPriceInEuro = _currencyConversionService.ChangeCurrency(totalPrice.Value, levaToEuroExchangeRate);
        }

        return new()
        {
            ProductName = invoiceItem.Name,
            Quantity = invoiceItem.Quantity,
            Currency = "лв",
            UnitOfMeasurement = "брой",
            PricePerUnit = invoiceItem.PriceInLeva,
            LineNetPrice = netPrice,
            LineVatPrice = vatPrice,
            LineTotalPrice = totalPrice,
            LineTotalPriceEuro = totalPriceInEuro,
        };
    }
}