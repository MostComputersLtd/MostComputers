using MOSTComputers.Models.Product.Models;
using System.Xml.Serialization;
using static MOSTComputers.Models.Product.MappingUtils.ProductStatusMapping;

namespace MOSTComputers.Services.XMLDataOperations.Models;

[Serializable]
public class XmlProduct
{
    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlAttribute(AttributeName = "uid")]
    public int UId { get; set; }

    [XmlElement(ElementName = "code")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Code { get; set; }

    [XmlElement(ElementName = "name")]
    public string Name { get; set; }

    [XmlIgnore]
    public ProductStatusEnum Status { get; set; }

    [XmlElement(ElementName = "product_status")]
    public string StatusString
    {
        get => GetBGStatusStringFromStatusEnum(Status) ?? string.Empty;
        set => Status = GetStatusEnumFromBGStatusString(value) ?? ProductStatusEnum.Unavailable;
    }

    [XmlElement(ElementName = "price")]
    public decimal Price { get; set; }

    [XmlElement(ElementName = "currency")]
    public string CurrencyCode { get; set; }

    [XmlElement(ElementName = "searchstring")]
    public string SearchString { get; set; }

    [XmlElement(ElementName = "category")]
    public XmlShopItemCategory Category { get; set; }

    [XmlElement(ElementName = "manufacturer")]
    public XmlManifacturer Manifacturer { get; set; }

    [XmlElement(ElementName = "partnum")]
    public string PartNumbers { get; set; }

    [XmlElement(ElementName = "vendor_url", IsNullable = true)]
    public string? VendorUrl { get; set; }

    [XmlArray(ElementName = "gallery")]
    [XmlArrayItem(ElementName = "picture")]
    public List<XmlShopItemImage> ShopItemImages { get; set; }

    [XmlArray(ElementName = "properties")]
    [XmlArrayItem(ElementName = "property")]
    public List<XmlProductProperty> XmlProductProperties { get; set; }
}

[Serializable]
public class XmlProductProperty
{
    public XmlProductProperty()
    {
    }

    public XmlProductProperty(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public XmlProductProperty(string name, string order, string value)
    {
        Name = name;
        Order = order;
        Value = value;
    }

    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "order")]
    public string Order { get; set; }

    [XmlText]
    public string Value { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.