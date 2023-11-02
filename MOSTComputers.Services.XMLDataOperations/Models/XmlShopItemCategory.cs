using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MOSTComputers.Services.XMLDataOperations.Models;

[Serializable]
public class XmlShopItemCategory
{
    [XmlAttribute("id")]
    public int Id { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [XmlText]
    public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [XmlIgnore]
    public int? ParentCategoryId { get; set; }
}