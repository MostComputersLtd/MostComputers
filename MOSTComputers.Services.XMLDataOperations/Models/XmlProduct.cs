using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.XMLDataOperations.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

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
        get => ProductStatusMapping.GetBGStatusStringFromStatusEnum(Status);
        set => Status = ProductStatusMapping.GetStatusEnumFromBGStatusString(value) ?? ProductStatusEnum.Unavailable;
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

    [XmlArray(ElementName = "gallery")]
    [XmlArrayItem(ElementName = "picture")]
    public List<XmlShopItemImage> ShopItemImages { get; set; }

    [XmlArray(ElementName = "properties")]
    [XmlArrayItem(ElementName = "property")]
    public List<XmlProductProperty> xmlProductProperties { get; set; }
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