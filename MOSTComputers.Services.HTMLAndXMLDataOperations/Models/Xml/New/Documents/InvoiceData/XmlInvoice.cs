using MOSTComputers.Models.Product.MappingUtils;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;

public sealed class XmlInvoice : IXmlAsyncSerializable
{
    private const string _dateFormat = "MMM dd yyyy T:HH:mm:ss";

    public int ExportId { get; init; }
    public DateTime? ExportDate { get; init; }
    public int? ExportUserId { get; init; }
    public string? ExportUser { get; init; }
    public int InvoiceId { get; init; }
    //public int? FirmId { get; init; }
    public XmlInvoiceLocalFirmData? LocalFirmData { get; init; }
    public XmlInvoiceCustomerFirmData? CustomerFirmData { get; init; }
    public int? CustomerBID { get; init; }
    public string? InvoiceDirection { get; init; }
    public string? CustomerName { get; init; }
    public string? MPerson { get; init; }
    public string? CustomerAddress { get; init; }
    public DateTime? InvoiceDate { get; init; }
    public int? VatPercent { get; init; }
    public string? UserName { get; init; }
    public int? Status { get; init; }
    public string? InvoiceNumber { get; init; }
    public int? PayType { get; init; }
    public string? RPerson { get; init; }
    public DateTime? RDATE { get; init; }
    public string? Bulstat { get; init; }
    public int? PratkaId { get; init; }
    public int? InvType { get; init; }
    public string? InvBasis { get; init; }
    public string? RelatedInvoiceNumber { get; init; }
    public int? IsVATRegistered { get; init; }
    public decimal? PrintedNETAmount { get; init; }
    public DateTime? DueDate { get; init; }
    public string? BankNameAndId { get; init; }
    public string? BankIBAN { get; init; }
    public string? BankBIC { get; init; }
    public string? PaymentStatus { get; init; }
    public DateTime? PaymentStatusDate { get; init; }
    public string? PaymentStatusUserName { get; init; }
    public decimal? TotalPrice { get; init; }
    public decimal? TotalPriceWithVAT { get; init; }
    public Currency? InvoiceCurrency { get; init; }

    public List<XmlInvoiceItem>? InvoiceItems { get; init; }

    //public bool ShouldDisplayExportDate()
    //{
    //    return ExportDate.HasValue;
    //}

    //public bool ShouldDisplayExportUserId()
    //{
    //    return ExportUserId.HasValue;
    //}

    //public bool ShouldDisplayExportUser()
    //{
    //    return !string.IsNullOrEmpty(ExportUser);
    //}

    //public bool ShouldDisplayFirmId()
    //{
    //    return FirmId.HasValue;
    //}

    public bool ShouldDisplayCustomerBID()
    {
        return CustomerBID.HasValue;
    }

    public bool ShouldDisplayInvoiceDirection()
    {
        return !string.IsNullOrWhiteSpace(InvoiceDirection);
    }

    public bool ShouldDisplayLocalFirmData()
    {
        return LocalFirmData is not null;
    }

    public bool ShouldDisplayCustomerFirmData()
    {
        return CustomerFirmData is not null;
    }

    public bool ShouldDisplayCustomerName()
    {
        return !string.IsNullOrEmpty(CustomerName);
    }

    public bool ShouldDisplayMPerson()
    {
        return !string.IsNullOrEmpty(MPerson);
    }

    public bool ShouldDisplayCustomerAddress()
    {
        return !string.IsNullOrEmpty(CustomerAddress);
    }

    public bool ShouldDisplayInvoiceDate()
    {
        return InvoiceDate.HasValue;
    }

    //public bool ShouldDisplayVatPercent()
    //{
    //    return VatPercent.HasValue;
    //}

    //public bool ShouldDisplayUserName()
    //{
    //    return !string.IsNullOrEmpty(UserName);
    //}

    //public bool ShouldDisplayStatus()
    //{
    //    return Status.HasValue;
    //}

    public bool ShouldDisplayInvoiceNumber()
    {
        return !string.IsNullOrEmpty(InvoiceNumber);
    }

    //public bool ShouldDisplayPayType()
    //{
    //    return PayType.HasValue;
    //}

    public bool ShouldDisplayRPerson()
    {
        return !string.IsNullOrEmpty(RPerson);
    }

    public bool ShouldDisplayRDATE()
    {
        return RDATE.HasValue;
    }

    public bool ShouldDisplayBulstat()
    {
        return !string.IsNullOrEmpty(Bulstat);
    }

    //public bool ShouldDisplayPratkaId()
    //{
    //    return PratkaId.HasValue;
    //}

    //public bool ShouldDisplayInvType()
    //{
    //    return InvType.HasValue;
    //}

    //public bool ShouldDisplayInvBasis()
    //{
    //    return !string.IsNullOrEmpty(InvBasis);
    //}

    public bool ShouldDisplayRelatedInvNo()
    {
        return !string.IsNullOrEmpty(RelatedInvoiceNumber);
    }

    //public bool ShouldDisplayIsVATRegistered()
    //{
    //    return IsVATRegistered.HasValue;
    //}

    //public bool ShouldDisplayPrintedNETAmount()
    //{
    //    return PrintedNETAmount.HasValue;
    //}

    public bool ShouldDisplayDueDate()
    {
        return DueDate.HasValue;
    }

    public bool ShouldDisplayBankNameAndId()
    {
        return !string.IsNullOrEmpty(BankNameAndId);
    }

    public bool ShouldDisplayBankIBAN()
    {
        return !string.IsNullOrEmpty(BankIBAN);
    }

    public bool ShouldDisplayBankBIC()
    {
        return !string.IsNullOrEmpty(BankBIC);
    }

    public bool ShouldDisplayPaymentStatus()
    {
        return !string.IsNullOrEmpty(PaymentStatus);
    }

    //public bool ShouldDisplayPaymentStatusDate()
    //{
    //    return PaymentStatusDate.HasValue;
    //}

    //public bool ShouldDisplayPaymentStatusUserName()
    //{
    //    return !string.IsNullOrEmpty(PaymentStatusUserName);
    //}

    public bool ShouldDisplayTotalPrice()
    {
        return TotalPrice is not null;
    }

    public bool ShouldDisplayTotalPriceWithVAT()
    {
        return TotalPriceWithVAT is not null;
    }

    public bool ShouldDisplayInvoiceCurrency()
    {
        return InvoiceCurrency is not null;
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        //await writer.WriteAttributeStringAsync(null, "id", null, InvoiceId.ToString());

        if (ShouldDisplayInvoiceNumber())
        {
            await writer.WriteAttributeStringAsync(null, "number", null, InvoiceNumber!);
        }

        //if (ShouldDisplayPaymentStatusDate())
        //{
        //    await writer.WriteElementStringAsync(null, "paymentStatusDate", null, PaymentStatusDate!.Value.ToString());
        //}

        //if (ShouldDisplayPaymentStatusUserName())
        //{
        //    await writer.WriteElementStringAsync(null, "paymentStatusUserName", null, PaymentStatusUserName!);
        //}

        //if (ShouldDisplayCustomerBID())
        //{
        //    await writer.WriteElementStringAsync(null, "customerBID", null, CustomerBID!.Value.ToString());
        //}

        if (ShouldDisplayInvoiceDirection())
        {
            await writer.WriteElementStringAsync(null, "type", null, InvoiceDirection!);
        }

        if (ShouldDisplayInvoiceDate())
        {
            await writer.WriteElementStringAsync(null, "date", null, InvoiceDate!.Value.ToString(_dateFormat));
        }

        if (ShouldDisplayRDATE())
        {
            await writer.WriteElementStringAsync(null, "rDate", null, RDATE!.Value.ToString(_dateFormat));
        }

        if (ShouldDisplayDueDate())
        {
        await writer.WriteElementStringAsync(null, "dueDate", null, DueDate!.Value.ToString(_dateFormat));
        }

        //if (ShouldDisplayExportDate())
        //{
        //    await writer.WriteElementStringAsync(null, "exportDate", null, ExportDate!.Value.ToString());
        //}

        //if (ShouldDisplayExportUserId())
        //{
        //    await writer.WriteElementStringAsync(null, "exportUserId", null, ExportUserId!.Value.ToString());
        //}

        //if (ShouldDisplayExportUser())
        //{
        //    await writer.WriteElementStringAsync(null, "exportUser", null, ExportUser!);
        //}

        //if (ShouldDisplayFirmId())
        //{
        //    await writer.WriteElementStringAsync(null, "firmId", null, FirmId!.Value.ToString());
        //}

        if (ShouldDisplayRelatedInvNo())
        {
            await writer.WriteElementStringAsync(null, "relatedInvoiceNumber", null, RelatedInvoiceNumber!);
        }

        if (ShouldDisplayLocalFirmData())
        {
            await LocalFirmData!.WriteXmlAsync(writer, "supplier");
        }

        if (ShouldDisplayCustomerFirmData())
        {
            await CustomerFirmData!.WriteXmlAsync(writer, "customer");
        }

        //if (ShouldDisplayCustomerName())
        //{
        //    await writer.WriteElementStringAsync(null, "customerName", null, CustomerName!);
        //}

        //if (ShouldDisplayMPerson())
        //{
        //    await writer.WriteElementStringAsync(null, "mPerson", null, MPerson!);
        //}

        //if (ShouldDisplayCustomerAddress())
        //{
        //    await writer.WriteElementStringAsync(null, "customerAddress", null, CustomerAddress!);
        //}

        //if (ShouldDisplayVatPercent())
        //{
        //    await writer.WriteElementStringAsync(null, "vatPercent", null, VatPercent!.Value.ToString());
        //}

        //if (ShouldDisplayUserName())
        //{
        //    await writer.WriteElementStringAsync(null, "userName", null, UserName!);
        //}

        //if (ShouldDisplayStatus())
        //{
        //    await writer.WriteElementStringAsync(null, "status", null, Status!.Value.ToString());
        //}

        //if (ShouldDisplayPayType())
        //{
        //    await writer.WriteElementStringAsync(null, "payType", null, PayType!.Value.ToString());
        //}

        //if (ShouldDisplayRPerson())
        //{
        //    await writer.WriteElementStringAsync(null, "rPerson", null, RPerson!);
        //}

        //if (ShouldDisplayBulstat())
        //{
        //    await writer.WriteElementStringAsync(null, "bulstat", null, Bulstat!);
        //}

        //if (ShouldDisplayPratkaId())
        //{
        //    await writer.WriteElementStringAsync(null, "pratkaId", null, PratkaId!.Value.ToString());
        //}

        //if (ShouldDisplayInvType())
        //{
        //    await writer.WriteElementStringAsync(null, "invType", null, InvType!.Value.ToString());
        //}

        //if (ShouldDisplayInvBasis())
        //{
        //    await writer.WriteElementStringAsync(null, "invBasis", null, InvBasis!);
        //}

        //if (ShouldDisplayIsVATRegistered())
        //{
        //    await writer.WriteElementStringAsync(null, "isVATRegistered", null, IsVATRegistered!.Value.ToString());
        //}

        //if (ShouldDisplayPrintedNETAmount())
        //{
        //    await writer.WriteElementStringAsync(null, "printedNETAmount", null, PrintedNETAmount!.Value.ToString());
        //}

        if (ShouldDisplayPaymentStatus())
        {
            await writer.WriteElementStringAsync(null, "paymentStatus", null, PaymentStatus!);
        }

        //if (ShouldDisplayBankNameAndId())
        //{
        //    await writer.WriteElementStringAsync(null, "customerBankNameAndId", null, BankNameAndId!);
        //}

        //if (ShouldDisplayBankBIC())
        //{
        //    await writer.WriteElementStringAsync(null, "customerBankBIC", null, BankBIC!);
        //}

        //if (ShouldDisplayBankIBAN())
        //{
        //    await writer.WriteElementStringAsync(null, "customerBankIBAN", null, BankIBAN!);
        //}

        if (ShouldDisplayTotalPrice())
        {
            await writer.WriteElementStringAsync(null, "totalAmount", null, TotalPrice!.Value.ToString("F2"));
        }

        if (ShouldDisplayTotalPriceWithVAT())
        {
            await writer.WriteElementStringAsync(null, "totalAmountWithVAT", null, TotalPriceWithVAT!.Value.ToString("F2"));
        }

        if (ShouldDisplayInvoiceCurrency())
        {
            await writer.WriteElementStringAsync(null, "currency", null, GetStringFromCurrencyInInvoice(InvoiceCurrency)!);
        }

        if (InvoiceItems?.Count > 0)
        {
            await writer.WriteStartElementAsync(null, "items", null);

            foreach (XmlInvoiceItem invoiceItem in InvoiceItems)
            {
                await invoiceItem.WriteXmlAsync(writer, "item");
            }

            await writer.WriteEndElementAsync();
        }

        await writer.WriteEndElementAsync();
    }

    private static string? GetStringFromCurrencyInInvoice(Currency? currencyEnum)
    {
        if (currencyEnum is null) return null;

        return currencyEnum switch
        {
            Currency.BGN => "BGN",
            Currency.EUR => "EUR",
            Currency.USD => "USD",
            _ => "BGN"
        };
    }
}