using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;
public sealed class XmlInvoice : IXmlAsyncSerializable
{
    public int ExportId { get; init; }
    public DateTime? ExportDate { get; init; }
    public int? ExportUserId { get; init; }
    public string? ExportUser { get; init; }
    public int InvoiceId { get; init; }
    public int? FirmId { get; init; }
    public int? CustomerBID { get; init; }
    public int? InvoiceDirection { get; init; }
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
    public string? RelatedInvNo { get; init; }
    public int? IsVATRegistered { get; init; }
    public decimal? PrintedNETAmount { get; init; }
    public DateTime? DueDate { get; init; }
    public string? CustomerBankNameAndId { get; init; }
    public string? CustomerBankIBAN { get; init; }
    public string? CustomerBankBIC { get; init; }
    public string? PaymentStatus { get; init; }
    public DateTime? PaymentStatusDate { get; init; }
    public string? PaymentStatusUserName { get; init; }

    public List<XmlInvoiceItem>? InvoiceItems { get; init; }

    public bool ShouldDisplayExportDate()
    {
        return ExportDate.HasValue;
    }

    public bool ShouldDisplayExportUserId()
    {
        return ExportUserId.HasValue;
    }

    public bool ShouldDisplayExportUser()
    {
        return !string.IsNullOrEmpty(ExportUser);
    }

    public bool ShouldDisplayFirmId()
    {
        return FirmId.HasValue;
    }

    public bool ShouldDisplayCustomerBID()
    {
        return CustomerBID.HasValue;
    }

    public bool ShouldDisplayInvoiceDirection()
    {
        return InvoiceDirection.HasValue;
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

    public bool ShouldDisplayVatPercent()
    {
        return VatPercent.HasValue;
    }

    public bool ShouldDisplayUserName()
    {
        return !string.IsNullOrEmpty(UserName);
    }

    public bool ShouldDisplayStatus()
    {
        return Status.HasValue;
    }

    public bool ShouldDisplayInvoiceNumber()
    {
        return !string.IsNullOrEmpty(InvoiceNumber);
    }

    public bool ShouldDisplayPayType()
    {
        return PayType.HasValue;
    }

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

    public bool ShouldDisplayPratkaId()
    {
        return PratkaId.HasValue;
    }

    public bool ShouldDisplayInvType()
    {
        return InvType.HasValue;
    }

    public bool ShouldDisplayInvBasis()
    {
        return !string.IsNullOrEmpty(InvBasis);
    }

    public bool ShouldDisplayRelatedInvNo()
    {
        return !string.IsNullOrEmpty(RelatedInvNo);
    }

    public bool ShouldDisplayIsVATRegistered()
    {
        return IsVATRegistered.HasValue;
    }

    public bool ShouldDisplayPrintedNETAmount()
    {
        return PrintedNETAmount.HasValue;
    }

    public bool ShouldDisplayDueDate()
    {
        return DueDate.HasValue;
    }

    public bool ShouldDisplayCustomerBankNameAndId()
    {
        return !string.IsNullOrEmpty(CustomerBankNameAndId);
    }

    public bool ShouldDisplayCustomerBankIBAN()
    {
        return !string.IsNullOrEmpty(CustomerBankIBAN);
    }

    public bool ShouldDisplayCustomerBankBIC()
    {
        return !string.IsNullOrEmpty(CustomerBankBIC);
    }

    public bool ShouldDisplayPaymentStatus()
    {
        return !string.IsNullOrEmpty(PaymentStatus);
    }

    public bool ShouldDisplayPaymentStatusDate()
    {
        return PaymentStatusDate.HasValue;
    }

    public bool ShouldDisplayPaymentStatusUserName()
    {
        return !string.IsNullOrEmpty(PaymentStatusUserName);
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        await writer.WriteAttributeStringAsync(null, "id", null, InvoiceId.ToString());

        if (ShouldDisplayInvoiceNumber())
        {
            await writer.WriteAttributeStringAsync(null, "number", null, InvoiceNumber!);
        }

        if (ShouldDisplayPaymentStatus())
        {
            await writer.WriteElementStringAsync(null, "paymentStatus", null, PaymentStatus!);
        }

        if (ShouldDisplayPaymentStatusDate())
        {
            await writer.WriteElementStringAsync(null, "paymentStatusDate", null, PaymentStatusDate!.Value.ToString());
        }

        if (ShouldDisplayPaymentStatusUserName())
        {
            await writer.WriteElementStringAsync(null, "paymentStatusUserName", null, PaymentStatusUserName!);
        }

        if (ShouldDisplayInvoiceDate())
        {
            await writer.WriteElementStringAsync(null, "date", null, InvoiceDate!.Value.ToString());
        }

        if (ShouldDisplayExportDate())
        {
            await writer.WriteElementStringAsync(null, "exportDate", null, ExportDate!.Value.ToString());
        }

        if (ShouldDisplayExportUserId())
        {
            await writer.WriteElementStringAsync(null, "exportUserId", null, ExportUserId!.Value.ToString());
        }

        if (ShouldDisplayExportUser())
        {
            await writer.WriteElementStringAsync(null, "exportUser", null, ExportUser!);
        }

        if (ShouldDisplayFirmId())
        {
            await writer.WriteElementStringAsync(null, "firmId", null, FirmId!.Value.ToString());
        }

        if (ShouldDisplayCustomerBID())
        {
            await writer.WriteElementStringAsync(null, "customerBID", null, CustomerBID!.Value.ToString());
        }

        if (ShouldDisplayInvoiceDirection())
        {
            await writer.WriteElementStringAsync(null, "invoiceDirection", null, InvoiceDirection!.Value.ToString());
        }

        if (ShouldDisplayCustomerName())
        {
            await writer.WriteElementStringAsync(null, "customerName", null, CustomerName!);
        }

        if (ShouldDisplayMPerson())
        {
            await writer.WriteElementStringAsync(null, "mPerson", null, MPerson!);
        }

        if (ShouldDisplayCustomerAddress())
        {
            await writer.WriteElementStringAsync(null, "customerAddress", null, CustomerAddress!);
        }

        if (ShouldDisplayVatPercent())
        {
            await writer.WriteElementStringAsync(null, "vatPercent", null, VatPercent!.Value.ToString());
        }

        if (ShouldDisplayUserName())
        {
            await writer.WriteElementStringAsync(null, "userName", null, UserName!);
        }

        if (ShouldDisplayStatus())
        {
            await writer.WriteElementStringAsync(null, "status", null, Status!.Value.ToString());
        }

        if (ShouldDisplayPayType())
        {
            await writer.WriteElementStringAsync(null, "payType", null, PayType!.Value.ToString());
        }
        if (ShouldDisplayRPerson())
        {
            await writer.WriteElementStringAsync(null, "rPerson", null, RPerson!);
        }

        if (ShouldDisplayRDATE())
        {
            await writer.WriteElementStringAsync(null, "rDate", null, RDATE!.Value.ToString());
        }

        if (ShouldDisplayBulstat())
        {
            await writer.WriteElementStringAsync(null, "bulstat", null, Bulstat!);
        }

        if (ShouldDisplayPratkaId())
        {
            await writer.WriteElementStringAsync(null, "pratkaId", null, PratkaId!.Value.ToString());
        }

        if (ShouldDisplayInvType())
        {
            await writer.WriteElementStringAsync(null, "invType", null, InvType!.Value.ToString());
        }

        if (ShouldDisplayInvBasis())
        {
            await writer.WriteElementStringAsync(null, "invBasis", null, InvBasis!);
        }

        if (ShouldDisplayRelatedInvNo())
        {
            await writer.WriteElementStringAsync(null, "relatedInvNo", null, RelatedInvNo!);
        }

        if (ShouldDisplayIsVATRegistered())
        {
            await writer.WriteElementStringAsync(null, "isVATRegistered", null, IsVATRegistered!.Value.ToString());
        }

        if (ShouldDisplayPrintedNETAmount())
        {
            await writer.WriteElementStringAsync(null, "printedNETAmount", null, PrintedNETAmount!.Value.ToString());
        }

        if (ShouldDisplayDueDate())
        {
            await writer.WriteElementStringAsync(null, "dueDate", null, DueDate!.Value.ToString());
        }

        if (ShouldDisplayCustomerBankNameAndId())
        {
            await writer.WriteElementStringAsync(null, "customerBankNameAndId", null, CustomerBankNameAndId!);
        }

        if (ShouldDisplayCustomerBankIBAN())
        {
            await writer.WriteElementStringAsync(null, "customerBankIBAN", null, CustomerBankIBAN!);
        }

        if (ShouldDisplayCustomerBankBIC())
        {
            await writer.WriteElementStringAsync(null, "customerBankBIC", null, CustomerBankBIC!);
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
}