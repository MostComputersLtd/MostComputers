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
        IInvoiceXmlService invoiceXmlService)
    {
        _invoiceRepository = invoiceRepository;
        _invoiceXmlService = invoiceXmlService;
    }

    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceXmlService _invoiceXmlService;

    public async Task GetXmlForInvoicesAsync(Stream outputStream, InvoiceSearchRequest invoiceSearchRequest)
    {
        List<Invoice> invoices = await _invoiceRepository.GetAllMatchingAsync(invoiceSearchRequest);

        await GetXmlForInvoicesAsync(outputStream, invoices);
    }

    public async Task GetXmlForInvoicesAsync(Stream outputStream, IEnumerable<Invoice> invoices)
    {
        InvoiceXmlFullData invoiceXmlFullData = new()
        {
            Invoices = new(),
        };

        foreach (Invoice invoice in invoices)
        {
            XmlInvoice xmlInvoice = MapXmlInvoice(invoice);

            invoiceXmlFullData.Invoices.Add(xmlInvoice);
        }

        await _invoiceXmlService.TrySerializeXmlAsync(outputStream, invoiceXmlFullData);
    }

    private static XmlInvoice MapXmlInvoice(Invoice invoice)
    {
        return new()
        {
            ExportId = invoice.ExportId,
            ExportDate = invoice.ExportDate,
            ExportUserId = invoice.ExportUserId,
            ExportUser = invoice.ExportUser,
            InvoiceId = invoice.InvoiceId,
            FirmId = invoice.FirmId,
            CustomerBID = invoice.CustomerBID,
            InvoiceDirection = invoice.InvoiceDirection,
            CustomerName = invoice.CustomerName,
            MPerson = invoice.MPerson,
            CustomerAddress = invoice.CustomerAddress,
            InvoiceDate = invoice.InvoiceDate,
            VatPercent = invoice.VatPercent,
            UserName = invoice.UserName,
            Status = invoice.Status,
            InvoiceNumber = invoice.InvoiceNumber,
            PayType = invoice.PayType,
            RPerson = invoice.RPerson,
            RDATE = invoice.RDATE,
            Bulstat = invoice.Bulstat,
            PratkaId = invoice.PratkaId,
            InvType = invoice.InvType,
            InvBasis = invoice.InvBasis,
            RelatedInvNo = invoice.RelatedInvNo,
            IsVATRegistered = invoice.IsVATRegistered,
            PrintedNETAmount = invoice.PrintedNETAmount,
            DueDate = invoice.DueDate,
            CustomerBankNameAndId = invoice.CustomerBankNameAndId,
            CustomerBankIBAN = invoice.CustomerBankIBAN,
            CustomerBankBIC = invoice.CustomerBankBIC,
            PaymentStatus = invoice.PaymentStatus,
            PaymentStatusDate = invoice.PaymentStatusDate,
            PaymentStatusUserName = invoice.PaymentStatusUserName,

            InvoiceItems = invoice.InvoiceItems is not null ? MapXmlInvoiceItems(invoice.InvoiceItems) : null,
        };
    }

    private static List<XmlInvoiceItem> MapXmlInvoiceItems(IEnumerable<InvoiceItem> invoiceItems)
    {
        List<XmlInvoiceItem> output = new();

        foreach (InvoiceItem item in invoiceItems)
        {
            output.Add(MapXmlInvoiceItem(item));
        }

        return output;
    }

    private static XmlInvoiceItem MapXmlInvoiceItem(InvoiceItem invoiceItem)
    {
        return new()
        {
            ExportedItemId = invoiceItem.ExportedItemId,
            ExportId = invoiceItem.ExportId,
            IEID = invoiceItem.IEID,
            Name = invoiceItem.Name,
            PriceInLeva = invoiceItem.PriceInLeva,
            Quantity = invoiceItem.Quantity,
            DisplayOrder = invoiceItem.DisplayOrder,
        };
    }
}