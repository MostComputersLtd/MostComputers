namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;

public enum InvoiceByStringSearchOptions
{
    ByExportUser = 1,
    ByCustomerName = 2,
    ByMPerson = 3,
    ByCustomerAddress = 4,
    ByInvoiceNumber = 5,
    ByRPerson = 6,
    ByBulstat = 7,
    ByRelatedInvNo = 8,
    ByCustomerBankNameAndId = 9,
    ByCustomerBankIBAN = 10,
    ByCustomerBankBIC = 11,
    ByPaymentStatusUserName = 12,
}