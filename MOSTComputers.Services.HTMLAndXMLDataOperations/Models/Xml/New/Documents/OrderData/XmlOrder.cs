using System.Xml;
using MOSTComputers.Models.Common;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.OrderData;

public sealed class XmlOrder : IXmlAsyncSerializable
{
    private const string _dateFormat = "MMM dd yyyy T:HH:mm:ss";

    public required int Id { get; init; }
	public byte? Status { get; init; }
	//public bool? AutoReply { get; init; }
	public int? QuoteId { get; init; }
	public int? UserId { get; init; }
	public DateTime? OrderDate { get; init; }
	public string? OrderName { get; init; }
	//public DateTime? ReadTime { get; init; }
	//public int? ReadUserId { get; init; }
	public int? BusinessId { get; init; }
	public int? DealId { get; init; }
	//public int? MOST3DealId { get; init; }
	//public bool? IsConfiguration { get; init; }
	//public decimal? CfgProfit { get; init; }
	public Currency? Currency { get; init; }
	//public int? OriginalInternetOrderId { get; init; }
	//public int? RepliedInternetOrderId { get; init; }
	public string? Info { get; init; }

    public List<XmlOrderItem> Items { get; init; } = new();

    public bool ShouldDisplayStatus()
    {
        return Status.HasValue;
    }

    public bool ShouldDisplayQuoteId()
    {
        return QuoteId.HasValue;
    }

    public bool ShouldDisplayUserId()
    {
        return UserId.HasValue;
    }

    public bool ShouldDisplayOrderDate()
    {
        return OrderDate.HasValue;
    }

    public bool ShouldDisplayOrderName()
    {
        return !string.IsNullOrWhiteSpace(OrderName);
    }

    public bool ShouldDisplayBusinessId()
    {
        return BusinessId.HasValue;
    }

    public bool ShouldDisplayDealId()
    {
        return DealId.HasValue;
    }

    public bool ShouldDisplayCurrency()
    {
        return Currency.HasValue;
    }

    public bool ShouldDisplayInfo()
    {
        return !string.IsNullOrEmpty(Info);
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        await writer.WriteAttributeStringAsync(null, "id", null, Id.ToString());

        if (ShouldDisplayStatus())
        {
            await writer.WriteElementStringAsync(null, "status", null, Status!.Value.ToString());
        }

        if (ShouldDisplayQuoteId())
        {
            await writer.WriteElementStringAsync(null, "quoteId", null, QuoteId!.Value.ToString());
        }

        if (ShouldDisplayUserId())
        {
            await writer.WriteElementStringAsync(null, "userId", null, UserId!.Value.ToString());
        }

        if (ShouldDisplayOrderDate())
        {
            await writer.WriteElementStringAsync(null, "date", null, OrderDate!.Value.ToString(_dateFormat));
        }

        if (ShouldDisplayOrderName())
        {
            await writer.WriteElementStringAsync(null, "name", null, OrderName!);
        }

        if (ShouldDisplayBusinessId())
        {
            await writer.WriteElementStringAsync(null, "customerId", null, BusinessId!.Value.ToString());
        }

        if (ShouldDisplayDealId())
        {
            await writer.WriteElementStringAsync(null, "dealId", null, DealId!.Value.ToString());
        }

        if (ShouldDisplayCurrency())
        {
            await writer.WriteElementStringAsync(null, "currency", null, GetStringFromCurrencyInOrder(Currency!)!);
        }

        if (ShouldDisplayInfo())
        {
            await writer.WriteElementStringAsync(null, "info", null, Info!);
        }

        if (Items?.Count > 0)
        {
            await writer.WriteStartElementAsync(null, "items", null);

            foreach (XmlOrderItem item in Items)
            {
                await item.WriteXmlAsync(writer, "item");
            }

            await writer.WriteEndElementAsync();
        }
    }

    private static string? GetStringFromCurrencyInOrder(Currency? currencyEnum)
    {
        if (currencyEnum is null) return null;

        return currencyEnum switch
        {
            MOSTComputers.Models.Common.Currency.BGN => "BGN",
            MOSTComputers.Models.Common.Currency.EUR => "EUR",
            MOSTComputers.Models.Common.Currency.USD => "USD",
            _ => "BGN"
        };
    }
}
