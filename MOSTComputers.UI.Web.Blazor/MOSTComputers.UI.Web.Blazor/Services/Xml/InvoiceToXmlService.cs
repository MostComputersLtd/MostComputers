using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.Xml.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Services.Xml;

internal sealed class InvoiceToXmlService : IInvoiceToXmlService
{
    public InvoiceToXmlService(
        IInvoiceRepository invoiceRepository,
        IFirmDataRepository firmDataRepository,
        IInvoiceXmlService invoiceXmlService,
        ICurrencyVATService currencyVATService)
    {
        _invoiceRepository = invoiceRepository;
        _firmDataRepository = firmDataRepository;
        _invoiceXmlService = invoiceXmlService;
        _currencyVATService = currencyVATService;
    }

    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IFirmDataRepository _firmDataRepository;
    private readonly IInvoiceXmlService _invoiceXmlService;
    private readonly ICurrencyVATService _currencyVATService;

    public async Task GetXmlForInvoicesAsync(Stream outputStream, InvoiceSearchRequest invoiceSearchRequest)
    {
        List<Invoice> invoices = await _invoiceRepository.GetAllMatchingAsync(invoiceSearchRequest);

        List<FirmData> firmDatas = await _firmDataRepository.GetAllAsync();

        await GetXmlForInvoicesAsync(outputStream, invoices, firmDatas);
    }

    public async Task GetXmlForInvoicesAsync(Stream outputStream, IEnumerable<Invoice> invoices, IEnumerable<FirmData>? firmDatas = null)
    {
        InvoiceXmlFullData invoiceXmlFullData = new()
        {
            Invoices = new(),
        };

        foreach (Invoice invoice in invoices)
        {
            FirmData? firmData = firmDatas?.FirstOrDefault(x => x.Id == invoice.FirmId);

            XmlInvoice xmlInvoice = MapXmlInvoice(invoice, firmData);

            invoiceXmlFullData.Invoices.Add(xmlInvoice);
        }

        await _invoiceXmlService.TrySerializeXmlAsync(outputStream, invoiceXmlFullData);
    }

    private XmlInvoice MapXmlInvoice(Invoice invoice, FirmData? firmData = null)
    {
        string? invoiceNumber = invoice.InvoiceNumber;

        if (invoiceNumber is not null && invoiceNumber.StartsWith('C'))
        {
            invoiceNumber = invoiceNumber[1..];
        }

        string? invoiceDirectionString = GetInvoiceDirectionString(invoice.InvoiceDirection);

        XmlInvoiceLocalFirmData? xmlLocalFirmData = null;

        if (firmData is not null)
        {
            xmlLocalFirmData = new()
            {
                Name = firmData.Name,
                Address = firmData.Address,
                MPerson = firmData.MPerson,
                Bulstat = firmData.Bulstat,
                BankNameAndId = invoice.BankNameAndId,
                BankIBAN = invoice.BankIBAN,
                VatId = firmData.Bulstat,
            };
        }

        XmlInvoiceCustomerFirmData? xmlCustomerFirmData = new()
        {
            CustomerBID = invoice.CustomerBID,
            CustomerName = invoice.CustomerName,
            CustomerAddress = invoice.CustomerAddress,
            Bulstat = invoice.Bulstat,
            MPerson = invoice.MPerson,
            RPerson = invoice.RPerson,
            BankNameAndId = invoice.BankNameAndId,
            BankBIC = invoice.BankBIC,
            BankIBAN = invoice.BankIBAN,
        };

        decimal? vatPercentageFraction = (invoice.VatPercent is not null) ? (decimal)invoice.VatPercent / 100 : null;

        Currency? invoiceCurrency;

        if (invoice.InvoiceCurrency is not null && Enum.IsDefined(typeof(Currency), invoice.InvoiceCurrency))
        {
            invoiceCurrency = (Currency)invoice.InvoiceCurrency.Value;
        }
        else
        {
            invoiceCurrency = Currency.BGN;
        }

        List<XmlInvoiceItem>? xmlInvoiceItems = null;

        decimal? totalPrice = 0M;
        decimal? totalPriceWithVAT = 0M;

        if (invoice.InvoiceItems is not null)
        {
            xmlInvoiceItems = MapXmlInvoiceItems(invoice.InvoiceItems);

            foreach (XmlInvoiceItem xmlInvoiceItem in xmlInvoiceItems)
            {
                decimal totalItemPrice = 0M;
                decimal totalItemPriceWithVAT = xmlInvoiceItem.Price ?? 0M;

                if (vatPercentageFraction is not null
                    && xmlInvoiceItem.Price is not null
                    && xmlInvoiceItem.Quantity is not null)
                {
                    totalItemPrice = xmlInvoiceItem.Price.Value * xmlInvoiceItem.Quantity.Value;

                    decimal vatPrice = _currencyVATService.CalculateVAT(xmlInvoiceItem.Price.Value, xmlInvoiceItem.Quantity.Value, vatPercentageFraction.Value);

                    totalItemPriceWithVAT = totalItemPrice + vatPrice;
                }

                totalPrice += totalItemPrice;

                totalPriceWithVAT += totalItemPriceWithVAT;
            }
        }

        return new()
        {
            ExportId = invoice.ExportId,
            ExportDate = invoice.ExportDate,
            ExportUserId = invoice.ExportUserId,
            ExportUser = invoice.ExportUser,
            InvoiceId = invoice.InvoiceId,
            //FirmId = invoice.FirmId,
            LocalFirmData = xmlLocalFirmData,
            CustomerFirmData = xmlCustomerFirmData,
            CustomerBID = invoice.CustomerBID,
            InvoiceDirection = invoiceDirectionString,
            CustomerName = invoice.CustomerName,
            MPerson = invoice.MPerson,
            CustomerAddress = invoice.CustomerAddress,
            InvoiceDate = invoice.InvoiceDate,
            VatPercent = invoice.VatPercent,
            UserName = invoice.UserName,
            Status = invoice.Status,
            InvoiceNumber = invoiceNumber,
            PayType = invoice.PayType,
            RPerson = invoice.RPerson,
            RDATE = invoice.RDATE,
            Bulstat = invoice.Bulstat,
            PratkaId = invoice.PratkaId,
            InvType = invoice.InvType,
            InvBasis = invoice.InvBasis,
            RelatedInvoiceNumber = invoice.RelatedInvoiceNumber,
            IsVATRegistered = invoice.IsVATRegistered,
            PrintedNETAmount = invoice.PrintedNETAmount,
            DueDate = invoice.DueDate,
            BankNameAndId = invoice.BankNameAndId,
            BankIBAN = invoice.BankIBAN,
            BankBIC = invoice.BankBIC,
            PaymentStatus = invoice.PaymentStatus,
            PaymentStatusDate = invoice.PaymentStatusDate,
            PaymentStatusUserName = invoice.PaymentStatusUserName,
            InvoiceCurrency = invoiceCurrency,
            TotalPrice = totalPrice,
            TotalPriceWithVAT = totalPriceWithVAT,

            InvoiceItems = xmlInvoiceItems,
        };
    }

    private List<XmlInvoiceItem> MapXmlInvoiceItems(IEnumerable<InvoiceItem> invoiceItems)
    {
        List<XmlInvoiceItem> output = new();

        foreach (InvoiceItem item in invoiceItems)
        {
            output.Add(MapXmlInvoiceItem(item));
        }

        return output;
    }

#pragma warning disable CA1822 // Mark members as static
    private XmlInvoiceItem MapXmlInvoiceItem(InvoiceItem invoiceItem)
#pragma warning restore CA1822 // Mark members as static
    {
        return new()
        {
            ExportedItemId = invoiceItem.ExportedItemId,
            ExportId = invoiceItem.ExportId,
            IEID = invoiceItem.IEID,
            Name = invoiceItem.Name,
            Price = invoiceItem.PriceInLeva,
            Quantity = invoiceItem.Quantity,
            DisplayOrder = invoiceItem.DisplayOrder,
        };
    }

    private static string? GetInvoiceDirectionString(InvoiceDirection? invoiceDirection)
    {
        return invoiceDirection switch
        {
            InvoiceDirection.Debit => "ФАКТУРА",
            InvoiceDirection.Credit => "КРЕДИТНО ИЗВЕСТИЕ",
            _ => null,
        };
    }
}