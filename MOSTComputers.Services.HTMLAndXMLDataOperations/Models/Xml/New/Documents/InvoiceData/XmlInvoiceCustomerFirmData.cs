using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;

public sealed class XmlInvoiceCustomerFirmData : IXmlAsyncSerializable
{
    public int? CustomerBID { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerAddress { get; init; }
    public string? Bulstat { get; init; }
    public string? MPerson { get; init; }
    public string? RPerson { get; init; }
    public string? BankNameAndId { get; init; }
    public string? BankBIC { get; init; }
    public string? BankIBAN { get; init; }

    public bool ShouldDisplayCustomerBID()
    {
        return CustomerBID.HasValue;
    }

    public bool ShouldDisplayCustomerName()
    {
        return !string.IsNullOrEmpty(CustomerName);
    }

    public bool ShouldDisplayCustomerAddress()
    {
        return !string.IsNullOrEmpty(CustomerAddress);
    }

    public bool ShouldDisplayBulstat()
    {
        return !string.IsNullOrEmpty(Bulstat);
    }

    public bool ShouldDisplayMPerson()
    {
        return !string.IsNullOrEmpty(MPerson);
    }

    public bool ShouldDisplayRPerson()
    {
        return !string.IsNullOrEmpty(RPerson);
    }

    public bool ShouldDisplayBankNameAndId()
    {
        return !string.IsNullOrEmpty(BankNameAndId);
    }

    public bool ShouldDisplayBankBIC()
    {
        return !string.IsNullOrEmpty(BankBIC);
    }

    public bool ShouldDisplayBankIBAN()
    {
        return !string.IsNullOrEmpty(BankIBAN);
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldDisplayCustomerBID())
        {
            await writer.WriteElementStringAsync(null, "BID", null, CustomerBID!.Value.ToString());
        }

        if (ShouldDisplayCustomerName())
        {
            await writer.WriteElementStringAsync(null, "name", null, CustomerName!);
        }

        if (ShouldDisplayCustomerAddress())
        {
            await writer.WriteElementStringAsync(null, "address", null, CustomerAddress!);
        }

        if (ShouldDisplayBulstat())
        {
            await writer.WriteElementStringAsync(null, "bulstat", null, Bulstat!);
        }

        if (ShouldDisplayMPerson())
        {
            await writer.WriteElementStringAsync(null, "mPerson", null, MPerson!);
        }

        if (ShouldDisplayRPerson())
        {
            await writer.WriteElementStringAsync(null, "rPerson", null, RPerson!);
        }

        if (ShouldDisplayBankNameAndId())
        {
            await writer.WriteElementStringAsync(null, "bankNameAndId", null, BankNameAndId!);
        }

        if (ShouldDisplayBankBIC())
        {
            await writer.WriteElementStringAsync(null, "bankBIC", null, BankBIC!);
        }

        if (ShouldDisplayBankIBAN())
        {
            await writer.WriteElementStringAsync(null, "bankIBAN", null, BankIBAN!);
        }

        await writer.WriteEndElementAsync();
    }
}
