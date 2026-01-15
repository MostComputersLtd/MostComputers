using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Services.DataAccess.Documents.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;
using MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;
using MOSTComputers.Services.PDF.Services.Contracts;
using MOSTComputers.UI.Web.Controllers.Documents;
using MOSTComputers.UI.Web.Models.Documents.Invoices;
using MOSTComputers.UI.Web.Models.Documents.WarrantyCards;
using MOSTComputers.UI.Web.Pages.Shared.Documents;

namespace MOSTComputers.UI.Web.Pages;

[Authorize]
public sealed class DocumentsModel : PageModel
{
    public DocumentsModel(
        IInvoiceRepository invoiceRepository,
        IWarrantyCardRepository warrantyCardRepository)
    {
        _invoiceRepository = invoiceRepository;
        _warrantyCardRepository = warrantyCardRepository;
    }

    private const string _invoiceTablePartialPath = "Documents/_InvoiceTablePartial";
    private const string _warrantyCardTablePartialPath = "Documents/_WarrantyCardTablePartial";

    private const string _pdfDisplayPartialPath = "Documents/_PdfDisplayPartial";

    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IWarrantyCardRepository _warrantyCardRepository;

    public void OnGet()
    {
    }

    public IStatusCodeActionResult OnGetGetInvoicePdfFromInvoiceNumberAsync(string invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            return BadRequest("The invoice number cannot be null or empty.");
        }

        string? invoicePdfUrl = $"/{PdfInvoiceDataController.ControllerRoute}/{invoiceNumber}";

        PdfDisplayPartialModel pdfDisplayPartialModel = new()
        {
            SourceUrl = invoicePdfUrl,
        };

        return Partial(_pdfDisplayPartialPath, pdfDisplayPartialModel);
    }

    public IStatusCodeActionResult OnGetGetWarrantyCardPdfFromExportIdAsync(int warrantyCardExportId)
    {
        string? warrantyCardPdfUrl = $"/{PdfWarrantyCardDataWithoutPricesController.ControllerRoute}/{warrantyCardExportId}";

        PdfDisplayPartialModel pdfDisplayPartialModel = new()
        {
            SourceUrl = warrantyCardPdfUrl,
        };

        return Partial(_pdfDisplayPartialPath, pdfDisplayPartialModel);
    }

    public async Task<IStatusCodeActionResult> OnPostGetSearchedInvoicesAsync(
        [FromBody] Models.Documents.Invoices.InvoiceSearchRequest? invoiceSearchRequest = null)
    {
        if (invoiceSearchRequest is null) return BadRequest();

        if (int.TryParse(invoiceSearchRequest.Keyword, out int invoiceNumber))
        {
            string invoiceNumberToSearchFor = $"C{invoiceNumber}";

            invoiceSearchRequest.InvoiceNumber ??= invoiceNumberToSearchFor;
        }

        List<Invoice> invoices = await GetSearchedInvoicesAsync(invoiceSearchRequest);

        List<InvoiceDisplayData> invoiceDisplayDataList = new();

        foreach (Invoice invoice in invoices)
        {
            InvoiceDisplayData invoiceDisplayData = GetInvoiceDisplayDataFromInvoice(invoice);

            invoiceDisplayDataList.Add(invoiceDisplayData);
        }

        List<InvoiceDisplayData> orderedInvoices = invoiceDisplayDataList
            .OrderByDescending(x => x.InvoiceDate ?? DateTime.MinValue)
            .ToList();

        InvoiceTablePartialModel invoiceTableModel = new()
        {
            Invoices = orderedInvoices
        };

        return Partial(_invoiceTablePartialPath, invoiceTableModel);
    }

    public async Task<IStatusCodeActionResult> OnPostGetSearchedWarrantyCardsAsync(
        [FromBody] Models.Documents.WarrantyCards.WarrantyCardSearchRequest? warrantyCardSearchRequest = null)
    {
        if (warrantyCardSearchRequest is null) return BadRequest();

        List<WarrantyCard> warrantyCards = await GetSearchedWarrantyCardsAsync(warrantyCardSearchRequest);

        List<WarrantyCardDisplayData> warrantyCardDisplayDataList = new();

        foreach (WarrantyCard warrantyCard in warrantyCards)
        {
            WarrantyCardDisplayData warrantyCardDisplayData = GetWarrantyCardDisplayDataFromWarrantyCard(warrantyCard);

            warrantyCardDisplayDataList.Add(warrantyCardDisplayData);
        }

        List<WarrantyCardDisplayData> orderedWarranties = warrantyCardDisplayDataList
            .OrderByDescending(x => x.WarrantyCardDate ?? DateTime.MinValue)
            .ToList();

        WarrantyCardTablePartialModel warrantyCardTableModel = new()
        {
            WarrantyCards = orderedWarranties
        };

        return Partial(_warrantyCardTablePartialPath, warrantyCardTableModel);
    }

    private async Task<List<Invoice>> GetSearchedInvoicesAsync(Models.Documents.Invoices.InvoiceSearchRequest invoicesSearchRequest)
    {
        if (invoicesSearchRequest.InvoiceId is not null)
        {
            Invoice? invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoicesSearchRequest.InvoiceId.Value);

            if (invoice is null) return [];

            return [invoice];
        }
        else if (!string.IsNullOrWhiteSpace(invoicesSearchRequest.InvoiceNumber))
        {
            Invoice? invoice = await _invoiceRepository.GetInvoiceByNumberAsync(invoicesSearchRequest.InvoiceNumber);

            if (invoice is null) return [];

            return [invoice];
        }

        List<InvoiceByStringSearchRequest>? invoiceByStringSearchRequests = null;

        if (!string.IsNullOrWhiteSpace(invoicesSearchRequest.Keyword))
        {
            invoiceByStringSearchRequests = new()
            {
                new InvoiceByStringSearchRequest()
                {
                    Keyword = invoicesSearchRequest.Keyword,
                    SearchOption = InvoiceByStringSearchOptions.ByCustomerName
                },
            };
        };

        List<InvoiceByDateFilterRequest>? invoiceByDateSearchRequests = null;

        if (invoicesSearchRequest.FromDate is not null
            || invoicesSearchRequest.ToDate is not null)
        {
            invoiceByDateSearchRequests = new()
            {
                new InvoiceByDateFilterRequest()
                {
                    SearchOption = InvoiceByDateSearchOptions.ByInvoiceDate,
                    FromDate = invoicesSearchRequest.FromDate,
                    ToDate = invoicesSearchRequest.ToDate,
                }
            };
        }

        MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice.InvoiceSearchRequest invoiceSearchRequest = new()
        {
            InvoiceByStringSearchRequests = invoiceByStringSearchRequests,
            InvoiceByDateFilterRequests = invoiceByDateSearchRequests
        };

        List<Invoice>? invoices = await _invoiceRepository.GetAllMatchingAsync(invoiceSearchRequest);

        return invoices;
    }

    private async Task<List<WarrantyCard>> GetSearchedWarrantyCardsAsync(Models.Documents.WarrantyCards.WarrantyCardSearchRequest warrantyCardsSearchRequest)
    {
        List<WarrantyCardByStringSearchRequest>? warrantyCardByStringSearchRequests = null;

        if (!string.IsNullOrWhiteSpace(warrantyCardsSearchRequest.Keyword))
        {
            warrantyCardByStringSearchRequests = new()
            {
                new WarrantyCardByStringSearchRequest()
                {
                    Keyword = warrantyCardsSearchRequest.Keyword,
                    SearchOption = WarrantyCardByStringSearchOptions.ByCustomerName
                },
            };
        };

        List<WarrantyCardByDateFilterRequest>? warrantyCardByDateSearchRequests = null;

        if (warrantyCardsSearchRequest.FromDate is not null
            || warrantyCardsSearchRequest.ToDate is not null)
        {
            warrantyCardByDateSearchRequests = new()
            {
                new WarrantyCardByDateFilterRequest()
                {
                    SearchOption = WarrantyCardByDateSearchOptions.ByWarrantyCardDate,
                    FromDate = warrantyCardsSearchRequest.FromDate,
                    ToDate = warrantyCardsSearchRequest.ToDate,
                }
            };
        }

        MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard.WarrantyCardSearchRequest warrantyCardSearchRequest = new()
        {
            WarrantyCardByStringSearchRequests = warrantyCardByStringSearchRequests,
            WarrantyCardByDateFilterRequests = warrantyCardByDateSearchRequests,
        };

        List<WarrantyCard>? warrantyCards = await _warrantyCardRepository.GetAllMatchingAsync(warrantyCardSearchRequest);

        return warrantyCards;
    }

    private InvoiceDisplayData GetInvoiceDisplayDataFromInvoice(Invoice invoice)
    {
        decimal totalPriceWithoutVat = 0M;

        if (invoice.InvoiceItems is not null)
        {
            foreach (InvoiceItem item in invoice.InvoiceItems)
            {
                if (item.PriceInLeva is null || item.Quantity is null) continue;

                totalPriceWithoutVat += item.PriceInLeva.Value * item.Quantity.Value;
            }
        }

        float vatPercent = (invoice.VatPercent is not null) ? (float)invoice.VatPercent / 100 : 0F;

        decimal totalPriceChargedFromVat = totalPriceWithoutVat * (decimal)vatPercent;

        decimal totalPrice = totalPriceWithoutVat + totalPriceChargedFromVat;

        return new InvoiceDisplayData()
        {
            ExportId = invoice.ExportId,
            ExportDate = invoice.ExportDate,
            ExportUserId = invoice.ExportUserId,
            ExportUser = invoice.ExportUser,
            InvoiceId = invoice.InvoiceId,
            FirmId = invoice.FirmId,
            CustomerFirmBID = invoice.CustomerBID,
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
            RelatedInvNo = invoice.RelatedInvoiceNumber,
            IsVATRegistered = invoice.IsVATRegistered,
            PrintedNETAmount = invoice.PrintedNETAmount,
            DueDate = invoice.DueDate,
            CustomerBankNameAndId = invoice.BankNameAndId,
            CustomerBankIBAN = invoice.BankIBAN,
            CustomerBankBIC = invoice.CustomerBankBIC,
            PaymentStatus = invoice.PaymentStatus,
            PaymentStatusDate = invoice.PaymentStatusDate,
            PaymentStatusUserName = invoice.PaymentStatusUserName,

            InvoiceItems = invoice.InvoiceItems?
                .Select(item => GetInvoiceItemDisplayDataFromInvoiceItem(item))
                .ToList(),

            TotalPrice = totalPrice,
        };
    }

    private InvoiceItemDisplayData GetInvoiceItemDisplayDataFromInvoiceItem(InvoiceItem invoiceItem)
    {
        return new InvoiceItemDisplayData()
        {
            ExportedItemId = invoiceItem.ExportedItemId,
            ExportId = invoiceItem.ExportId,
            IEID = invoiceItem.IEID,
            InvoiceId = invoiceItem.InvoiceId,
            Name = invoiceItem.Name,
            PriceInLeva = invoiceItem.PriceInLeva,
            Quantity = invoiceItem.Quantity,
            DisplayOrder = invoiceItem.DisplayOrder,
        };
    }

    private WarrantyCardDisplayData GetWarrantyCardDisplayDataFromWarrantyCard(WarrantyCard warrantyCard)
    {
        return new()
        {
            ExportId = warrantyCard.ExportId,
            ExportDate = warrantyCard.ExportDate,
            ExportUserId = warrantyCard.ExportUserId,
            ExportUser = warrantyCard.ExportUser,
            OrderId = warrantyCard.OrderId,
            CustomerName = warrantyCard.CustomerName,
            CustomerBID = warrantyCard.CustomerBID,
            WarrantyCardDate = warrantyCard.WarrantyCardDate,
            WarrantyCardTerm = warrantyCard.WarrantyCardTerm,
            WarrantyCardItems = warrantyCard.WarrantyCardItems?
                .Select(item => GetWarrantyCardItemDisplayDataFromWarrantyCardItem(item))
                .ToList(),
        };
    }

    private static WarrantyCardItemDisplayData GetWarrantyCardItemDisplayDataFromWarrantyCardItem(WarrantyCardItem warrantyCardItem)
    {
        return new()
        {
            ExportedItemId = warrantyCardItem.ExportedItemId,
            ExportId = warrantyCardItem.ExportId,
            OrderId = warrantyCardItem.OrderId,
            ProductId = warrantyCardItem.ProductId,
            ProductName = warrantyCardItem.ProductName,
            SerialNumber = warrantyCardItem.SerialNumber,
            PriceInLeva = warrantyCardItem.PriceInLeva,
            Quantity = warrantyCardItem.Quantity,
            DisplayOrder = warrantyCardItem.DisplayOrder,
            WarrantyCardItemTermInMonths = warrantyCardItem.WarrantyCardItemTermInMonths,
        };
    }
}