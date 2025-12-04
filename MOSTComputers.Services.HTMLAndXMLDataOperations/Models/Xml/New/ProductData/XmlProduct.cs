using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using System.Xml;
using System.Xml.Serialization;

using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

[Serializable]
public class XmlProduct : IXmlAsyncSerializable
{
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [XmlElement(ElementName = "name")]
    public string Name { get; set; }

    [XmlIgnore]
    public ProductStatus Status { get; set; }

    [XmlElement(ElementName = "product_status")]
    public string StatusString
    {
        get => GetBGStatusStringFromStatusEnum(Status) ?? string.Empty;
        set => Status = GetStatusEnumFromBGStatusString(value) ?? ProductStatus.Unavailable;
    }

    [XmlElement(ElementName = "general_description")]
    public string GeneralDescription { get; set; }

    [XmlArray("promotions")]
    [XmlArrayItem("promo")]
    public List<XmlPromotion>? Promotions { get; set; } = null;

    [XmlElement("promoGroup")]
    public XmlGroupPromotion? GroupPromotion { get; set; } = null;

    public bool ShouldSerializePromotions()
    {
        return Promotions is not null && Promotions.Count > 0;
    }

    public bool ShouldSerializeGroupPromotion()
    {
        return GroupPromotion is not null;
    }

    [XmlElement(ElementName = "price")]
    public decimal Price { get; set; }

    [XmlElement(ElementName = "currency")]
    public string CurrencyCode { get; set; }

    [XmlElement(ElementName = "category")]
    public XmlCategory Category { get; set; }

    [XmlElement(ElementName = "subcategory")]
    public XmlSubCategory? SubCategory { get; set; }
    public bool ShouldSerializeSubCategory()
    {
        return SubCategory is not null;
    }

    [XmlElement(ElementName = "manufacturer")]
    public XmlManufacturer Manufacturer { get; set; }

    [XmlElement(ElementName = "warrantyInMonths")]
    public long? WarrantyInMonths { get; set; }

    public bool ShouldSerializeWarrantyInMonths()
    {
        return WarrantyInMonths > 0;
    }

    [XmlElement(ElementName = "EAN")]
    public string? PartNumber1 { get; set; }
    public bool ShouldSerializePartNumber1()
    {
        return PartNumber1 is not null;
    }

    [XmlElement(ElementName = "PartNumber")]
    public string? PartNumber2 { get; set; }
    public bool ShouldSerializePartNumber2()
    {
        return PartNumber2 is not null;
    }

    [XmlElement(ElementName = "vendorUrl")]
    public string? VendorUrl { get; set; }
    public bool ShouldSerializeVendorUrl()
    {
        return !string.IsNullOrEmpty(VendorUrl);
    }

    [XmlElement(ElementName = "searchstring")]
    public string SearchString { get; set; }

    [XmlArray(ElementName = "searchStringParts")]
    [XmlArrayItem(ElementName = "description")]
    public List<XmlSearchStringPartInfo>? SearchStringParts { get; set; }
    public bool ShouldSerializeSearchStringParts()
    {
        return SearchStringParts is not null && SearchStringParts.Count > 0;
    }

    [XmlArray(ElementName = "gallery")]
    [XmlArrayItem(ElementName = "pictureUrl")]
    public List<XmlProductImage> Images { get; set; }

    [XmlArray(ElementName = "downloads")]
    [XmlArrayItem(ElementName = "file")]
    public List<XmlFile> Downloads { get; set; }
    public bool ShouldSerializeDownloads()
    {
        return Downloads is not null && Downloads.Count > 0;
    }

    [XmlArray(ElementName = "properties")]
    [XmlArrayItem(ElementName = "property")]
    public List<XmlProductProperty> Properties { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public async Task WriteXmlAsync(XmlWriter writer, string rootElementName)
    {
        await writer.WriteStartElementAsync(null, rootElementName, null);

        await writer.WriteAttributeStringAsync(null, "id", null, Id.ToString());

        await writer.WriteElementStringAsync(null, "name", null, Name);

        await writer.WriteElementStringAsync(null, "product_status", null, StatusString);

        await writer.WriteElementStringAsync(null, "general_description", null, GeneralDescription);

        if (ShouldSerializePromotions())
        {
            await writer.WriteStartElementAsync(null, "promotions", null);

            foreach (XmlPromotion promotion in Promotions!)
            {
                await promotion.WriteXmlAsync(writer, "promo");
            }

            await writer.WriteEndElementAsync();
        }

        if (ShouldSerializeGroupPromotion())
        {
            await GroupPromotion!.WriteXmlAsync(writer, "promoGroup");
        }

        await writer.WriteElementStringAsync(null, "price", null, Price.ToString());

        await writer.WriteElementStringAsync(null, "currency", null, CurrencyCode);

        await Category.WriteXmlAsync(writer, "category");

        if (ShouldSerializeSubCategory())
        {
            await SubCategory!.WriteXmlAsync(writer, "subcategory");
        }

        await Manufacturer.WriteXmlAsync(writer, "manufacturer");

        if (ShouldSerializeWarrantyInMonths())
        {
            await writer.WriteElementStringAsync(null, "warrantyInMonths", null, WarrantyInMonths!.Value.ToString());
        }

        if (ShouldSerializePartNumber1())
        {
            await writer.WriteElementStringAsync(null, "EAN", null, PartNumber1!);
        }

        if (ShouldSerializePartNumber2())
        {
            await writer.WriteElementStringAsync(null, "PartNumber", null, PartNumber2!);
        }

        if (!string.IsNullOrEmpty(VendorUrl))
        {
            await writer.WriteElementStringAsync(null, "vendorUrl", null, VendorUrl);
        }

        await writer.WriteElementStringAsync(null, "searchstring", null, SearchString);

        if (ShouldSerializeSearchStringParts())
        {
            await writer.WriteStartElementAsync(null, "searchStringParts", null);

            foreach (XmlSearchStringPartInfo partInfo in SearchStringParts!)
            {
                await partInfo.WriteXmlAsync(writer, "description");
            }

            await writer.WriteEndElementAsync();
        }

        await writer.WriteStartElementAsync(null, "gallery", null);

        foreach (XmlProductImage image in Images)
        {
            await image.WriteXmlAsync(writer, "pictureUrl");
        }

        await writer.WriteEndElementAsync();

        if (ShouldSerializeDownloads())
        {
            await writer.WriteStartElementAsync(null, "downloads", null);

            foreach (XmlFile file in Downloads!)
            {
                await file.WriteXmlAsync(writer, "file");
            }

            await writer.WriteEndElementAsync();
        }

        await writer.WriteStartElementAsync(null, "properties", null);

        foreach (XmlProductProperty property in Properties)
        {
            await property.WriteXmlAsync(writer, "property");
        }

        await writer.WriteEndElementAsync();

        await writer.WriteFullEndElementAsync();
    }
}