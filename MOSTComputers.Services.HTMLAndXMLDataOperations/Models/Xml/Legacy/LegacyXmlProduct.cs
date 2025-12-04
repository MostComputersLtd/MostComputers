using MOSTComputers.Models.Product.Models;
using System.Xml.Serialization;
using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
[Serializable]
public class LegacyXmlProduct
{
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "uid")]
    public int UId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [XmlElement(ElementName = "code")]
    public string Code { get; set; }

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

    [XmlElement(ElementName = "haspromo")]
    public bool HasPromotion { get; set; }

    [XmlElement(ElementName = "general_description")]
    public string GeneralDescription { get; set; }

    [XmlElement(ElementName = "price")]
    public decimal Price { get; set; }

    [XmlElement(ElementName = "currency")]
    public string CurrencyCode { get; set; }

    [XmlElement(ElementName = "main_picture_url")]
    public string MainPictureUrl { get; set; }

    [XmlElement(ElementName = "searchstring")]
    public string SearchString { get; set; }

    [XmlElement(ElementName = "category")]
    public LegacyXmlCategory Category { get; set; }

    [XmlElement(ElementName = "subcategory")]
    public LegacyXmlSubCategory? SubCategory { get; set; }
    public bool ShouldSerializeSubCategory()
    {
        return SubCategory is not null;
    }

    [XmlElement(ElementName = "manufacturer")]
    public LegacyXmlManufacturer Manufacturer { get; set; }

    [XmlElement(ElementName = "partnum")]
    public string? PartNumbers { get; set; }

    [XmlElement(ElementName = "vendor_url", IsNullable = true)]
    public string? VendorUrl { get; set; }

    [XmlArray(ElementName = "searchStringParts")]
    [XmlArrayItem(ElementName = "description")]
    public List<LegacySearchStringPartInfo>? SearchStringParts { get; set; }
    public bool ShouldSerializeSearchStringParts()
    {
        return SearchStringParts is not null && SearchStringParts.Count > 0;
    }

    [XmlArray(ElementName = "gallery")]
    [XmlArrayItem(ElementName = "picture")]
    public List<LegacyXmlProductImage> Images { get; set; }

    [XmlArray(ElementName = "downloads")]
    [XmlArrayItem(ElementName = "file")]
    public List<LegacyXmlFile> Downloads { get; set; }

    [XmlArray(ElementName = "promotions")]
    [XmlArrayItem(ElementName = "promo")]
    public List<LegacyXmlPromotion>? Promotions { get; set; }

    [XmlArray(ElementName = "properties")]
    [XmlArrayItem(ElementName = "property")]
    public List<LegacyXmlProductProperty> Properties { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}