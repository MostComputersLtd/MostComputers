using MOSTComputers.Services.XMLDataOperations.Models;
using OneOf;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.XMLDataOperations.Services;

public sealed class ProductDeserializeService
{
    public ProductDeserializeService(XmlSerializerFactory xmlSerializerFactory)
    {
        _xmlSerializer = xmlSerializerFactory.CreateSerializer(typeof(XmlObjectData));
    }

    private readonly XmlSerializer _xmlSerializer;

    public XmlObjectData? DeserializeProductsXml(string xml)
    {
        string transformedXml = TransformInputToWorkingXml(xml);

        using StringReader reader = new(transformedXml);

        object? dataAsObject = _xmlSerializer.Deserialize(reader);

        if (dataAsObject is null) return null;

        XmlObjectData data = (XmlObjectData)dataAsObject;

        TransformXmlDataBackToNormal(data);

        return data;
    }

    public OneOf<XmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml)
    {
        try
        {
            string transformedXml = TransformInputToWorkingXml(xml);

            using StringReader reader = new(transformedXml);

            object? dataAsObject = _xmlSerializer.Deserialize(reader);

            if (dataAsObject is null) return null;

            XmlObjectData data = (XmlObjectData)dataAsObject;

            TransformXmlDataBackToNormal(data);

            return data;
        }
        catch (InvalidOperationException invalidOperationEx)
        {
            return new InvalidXmlResult() { Text = invalidOperationEx.InnerException?.Message };
        }

    }

    private static string TransformInputToWorkingXml(string xml)
    {
        int currentIndex = 0;

        while (currentIndex != -1)
        {
            int startIndexRaw = xml.IndexOf("<searchstring>", currentIndex);
            int endIndexRaw = xml.IndexOf("</searchstring>", currentIndex);

            if (startIndexRaw == -1 || endIndexRaw == -1) break;

            int searchStringStartIndex = startIndexRaw + 14;
            int searchStringEndIndex = endIndexRaw - 1;

            for (int i = searchStringStartIndex; i < searchStringEndIndex; i++)
            {
                if (xml[i] == '<') xml = xml[..(i)] + "|smallerSign|" + xml[(i + 1)..];
            }

            currentIndex = searchStringEndIndex + 19;
        }

        return xml;
    }

    private static bool TransformXmlDataBackToNormal(XmlObjectData xmlData)
    {
        foreach (XmlProduct item in xmlData.Products)
        {
            item.SearchString = item.SearchString.Replace("|smallerSign|", "<");
        }

        return true;
    }
}