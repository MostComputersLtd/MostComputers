using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Services.Mapping;
using OneOf;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using System.Xml.Serialization;

namespace MOSTComputers.Services.XMLDataOperations.Services;

public sealed class ProductDeserializeService : IProductDeserializeService
{
    public ProductDeserializeService(
        XmlSerializerFactory xmlSerializerFactory,
        IProductToXmlProductMappingService productToXmlProductMappingService)
    {
        _xmlSerializer = xmlSerializerFactory.CreateSerializer(typeof(XmlObjectData));
        _productToXmlProductMappingService = productToXmlProductMappingService;
    }

    private readonly XmlSerializer _xmlSerializer;
    private readonly IProductToXmlProductMappingService _productToXmlProductMappingService;

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
            if (invalidOperationEx.InnerException is null
                || invalidOperationEx.InnerException is not XmlException)
            {
                return new InvalidXmlResult() { Text = "Something went wrong" };
            }

            return new InvalidXmlResult() { Text = invalidOperationEx.InnerException?.Message };
        }
    }

    public string SerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes)
    {
        using StringWriter writer = new();

        _xmlSerializer.Serialize(writer, xmlObjectData);

        string xml = writer.ToString();

        if (!shouldRemoveXmlSerializeEscapes) return xml;

        string unescapedXml = TransformToOriginalAfterSerialization(xml);

        return unescapedXml;
    }

    public string SerializeProductsXml(Product product, bool shouldRemoveXmlSerializeEscapes)
    {
        using StringWriter writer = new();

        _xmlSerializer.Serialize(writer, _productToXmlProductMappingService.MapToXmlProduct(product));

        string xml = writer.ToString();

        if (!shouldRemoveXmlSerializeEscapes) return xml;

        string unescapedXml = TransformToOriginalAfterSerialization(xml);

        return unescapedXml;
    }

    public OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes)
    {
        try
        {
            return SerializeProductsXml(xmlObjectData, shouldRemoveXmlSerializeEscapes);
        }
        catch (InvalidOperationException invalidOperationEx)
        {
            if (invalidOperationEx.InnerException is null
                || invalidOperationEx.InnerException is not XmlException)
            {
                return new InvalidXmlResult() { Text = "Something went wrong" };
            }

            return new InvalidXmlResult() { Text = invalidOperationEx.InnerException?.Message };
        }
    }

    public OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(Product product, bool shouldRemoveXmlSerializeEscapes)
    {
        try
        {
            return SerializeProductsXml(product, shouldRemoveXmlSerializeEscapes);
        }
        catch (InvalidOperationException invalidOperationEx)
        {
            if (invalidOperationEx.InnerException is null
                || invalidOperationEx.InnerException is not XmlException)
            {
                return new InvalidXmlResult() { Text = "Something went wrong" };
            }

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

    private static string TransformToOriginalAfterSerialization(string xml)
    {
        return xml.Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&apos;", "'")
            .Replace("&quot;", "\"")
            .Replace("&amp;", "&");
    }
}