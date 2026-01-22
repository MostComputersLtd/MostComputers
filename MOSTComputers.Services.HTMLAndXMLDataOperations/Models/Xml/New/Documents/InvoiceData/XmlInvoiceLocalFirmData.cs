using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.InvoiceData;

public sealed class XmlInvoiceLocalFirmData : IXmlAsyncSerializable
{
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? MPerson { get; init; }
    public string? Bulstat { get; init; }
    public string? BankNameAndId { get; init; }
    public string? BankIBAN { get; init; }
    public string? VatId { get; init; }

    public bool ShouldDisplayName()
    {
        return !string.IsNullOrWhiteSpace(Name);
    }

    public bool ShouldDisplayAddress()
    {
        return !string.IsNullOrWhiteSpace(Address);
    }

    public bool ShouldDisplayMPerson()
    {
        return !string.IsNullOrWhiteSpace(MPerson);
    }

    public bool ShouldDisplayBulstat()
    {
        return !string.IsNullOrWhiteSpace(Bulstat);
    }

    public bool ShouldDisplayBankNameAndId()
    {
        return !string.IsNullOrWhiteSpace(BankNameAndId);
    }

    public bool ShouldDisplayBankIBAN()
    {
        return !string.IsNullOrWhiteSpace(BankIBAN);
    }

    public bool ShouldDisplayVatId()
    {
        return !string.IsNullOrWhiteSpace(VatId);
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldDisplayName())
        {
            await writer.WriteElementStringAsync(null, "name", null, Name!);
        }

        if (ShouldDisplayAddress())
        {
            await writer.WriteElementStringAsync(null, "address", null, Address!);
        }

        if (ShouldDisplayMPerson())
        {
            await writer.WriteElementStringAsync(null, "mPerson", null, MPerson!);
        }

        if (ShouldDisplayBulstat())
        {
            await writer.WriteElementStringAsync(null, "bulstat", null, Bulstat!);
        }

        if (ShouldDisplayBankNameAndId())
        {
            await writer.WriteElementStringAsync(null, "bankNameAndId", null, BankNameAndId!);
        }

        if (ShouldDisplayBankIBAN())
        {
            await writer.WriteElementStringAsync(null, "bankIBAN", null, BankIBAN!);
        }

        if (ShouldDisplayVatId())
        {
            await writer.WriteElementStringAsync(null, "vatId", null, VatId!);
        }

        await writer.WriteEndElementAsync();
    }
}
