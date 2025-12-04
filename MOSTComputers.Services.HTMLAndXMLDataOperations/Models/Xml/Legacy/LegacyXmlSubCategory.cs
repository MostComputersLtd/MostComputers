using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
public class LegacyXmlSubCategory
{
    [XmlIgnore]
    public int? Id { get; set; }

    [XmlAttribute("id")]
    public string IdAsString
    {
        get
        {
            return Id?.ToString() ?? string.Empty;
        }
        set
        {
            bool isValidId = int.TryParse(value, out int id);

            Id = isValidId ? id : 0;
        }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [XmlText]
    public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}