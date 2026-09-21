using System.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.Documents.OrderData;

public sealed class XmlOrderItem : IXmlAsyncSerializable
{
	// public int OrderId { get; init; }
	public int? ProductId { get; init; }
	public int? Quantity { get; init; }
	public decimal? Price { get; init; }
	public decimal? AdditionalWarranty { get; init; }
	//public decimal? FDDC { get; init; }
	//public byte PriceGroup { get; init; }
	//public decimal? ProfitPercent { get; init; }
	//public int? PromotionPId { get; init; }
	//public int? PromotionRId { get; init; }
	public decimal? PromotionPAmount { get; init; }
	public decimal? PromotionRAmount { get; init; }
	//public string? STInfo { get; init; }
	//public string? STInfoW { get; init; }
	public string? ExternalInfo { get; init; }
	//public string? InternalInfo { get; init; }

    public bool ShouldDisplayProductId()
    {
        return ProductId.HasValue;
    }

    public bool ShouldDisplayQuantity()
    {
        return Quantity.HasValue;
    }

    public bool ShouldDisplayPrice()
    {
        return Price.HasValue;
    }

    public bool ShouldDisplayAdditionalWarranty()
    {
        return AdditionalWarranty.HasValue;
    }

    public bool ShouldDisplayPromotionPAmount()
    {
        return PromotionPAmount.HasValue;
    }

    public bool ShouldDisplayPromotionRAmount()
    {
        return PromotionRAmount.HasValue;
    }

    public bool ShouldDisplayExternalInfo()
    {
        return !string.IsNullOrEmpty(ExternalInfo);
    }

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        if (ShouldDisplayProductId())
        {
            await writer.WriteElementStringAsync(null, "productId", null, ProductId!.Value.ToString());
        }

        if (ShouldDisplayQuantity())
        {
            await writer.WriteElementStringAsync(null, "quantity", null, Quantity!.Value.ToString());
        }

        if (ShouldDisplayPrice())
        {
            await writer.WriteElementStringAsync(null, "pricePerItem", null, Price!.Value.ToString());
        }

        if (ShouldDisplayAdditionalWarranty())
        {
            await writer.WriteElementStringAsync(null, "addWrr", null, AdditionalWarranty!.Value.ToString());
        }

        if (ShouldDisplayPromotionPAmount())
        {
            await writer.WriteElementStringAsync(null, "promotionAmount", null, PromotionPAmount!.Value.ToString());
        }
        else if (ShouldDisplayPromotionRAmount())
        {
            await writer.WriteElementStringAsync(null, "promotionAmount", null, PromotionRAmount!.Value.ToString());
        }

        if (ShouldDisplayExternalInfo())
        {
            await writer.WriteElementStringAsync(null, "info", null, ExternalInfo!);
        }
    }
}
