using System.Xml.Serialization;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Models;

[Serializable]
public class XmlManifacturer
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [XmlAttribute(AttributeName = "id")]
    public string IdAsString { get; set; }

    public int? Id
    {
        get
        {
            if (string.IsNullOrEmpty(IdAsString)) return null;

            bool parseSuccess = int.TryParse(IdAsString, out int output);

            return parseSuccess ? output : null;
        }
    }

    [XmlText]
    public string Name { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}