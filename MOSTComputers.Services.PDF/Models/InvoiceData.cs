using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.PDF.Models;
public class InvoiceData
{
    public required string InvoiceId { get; set; }
    public required DateTime Date { get; set; }
    public required DateTime DueDate { get; set; }
    public required string Location { get; set; }
    public required string FirmInHeaderName { get; set; }
    public required string FirmInHeaderAddress { get; set; }
    public required string InvoiceOriginText { get; set; }

    public required FirmInvoiceData RecipientData { get; set; }
    public required FirmInvoiceData SupplierData { get; set; }
    public required List<PurchaseInvoiceData> Purchases { get; set; }

    public required float VatPercentageFraction { get; set; }
    public required string RecipientFullName { get; set; }
    public required string TypeOfPayment { get; set; }
    public required string AuthorFullName { get; set; }
}