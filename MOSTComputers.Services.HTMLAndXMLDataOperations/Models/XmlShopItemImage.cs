using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models;

[Serializable]
public class XmlShopItemImage
{
    [XmlIgnore]
    public short DisplayOrder { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [XmlElement(ElementName = "picture_url")]
    public string PictureUrl { get; set; }

    [XmlElement(ElementName = "thumbnail_url")]
    public string ThumbnailUrl { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.