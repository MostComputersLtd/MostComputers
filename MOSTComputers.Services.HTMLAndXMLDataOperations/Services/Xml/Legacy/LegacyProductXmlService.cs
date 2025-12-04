using OneOf;
using System.Xml;
using System.Xml.Serialization;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Utils;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy;
public sealed class LegacyProductXmlService : ILegacyProductXmlService
{
    public LegacyProductXmlService(XmlSerializerFactory xmlSerializerFactory)
    {
        _xmlSerializer = xmlSerializerFactory.CreateSerializer(typeof(LegacyXmlObjectData));
    }

    private readonly XmlSerializer _xmlSerializer;

    public LegacyXmlObjectData? DeserializeProductsXml(string xml)
    {
        //string transformedXml = ProductDeserializeXmlTransformUtils.TransformInputToWorkingXml(xml);

        //using StringReader reader = new(transformedXml);

        using StringReader reader = new(xml);

        object? dataAsObject = _xmlSerializer.Deserialize(reader);

        if (dataAsObject is null) return null;

        LegacyXmlObjectData data = (LegacyXmlObjectData)dataAsObject;

        //ProductDeserializeXmlTransformUtils.TransformXmlDataBackToNormal(data);

        return data;
    }

    public OneOf<LegacyXmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml)
    {
        try
        {
            //string transformedXml = ProductDeserializeXmlTransformUtils.TransformInputToWorkingXml(xml);

            //using StringReader reader = new(transformedXml);

            using StringReader reader = new(xml);

            object? dataAsObject = _xmlSerializer.Deserialize(reader);

            if (dataAsObject is null) return null;

            LegacyXmlObjectData data = (LegacyXmlObjectData)dataAsObject;

            //ProductDeserializeXmlTransformUtils.TransformXmlDataBackToNormal(data);

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

    public string SerializeProductsXml(LegacyXmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false)
    {
        using StringWriter writer = new();

        if (disableXsiAndXsdNamespaces)
        {
            XmlSerializerNamespaces xmlSerializerNamespaces = new();

            xmlSerializerNamespaces.Add("", null);

            _xmlSerializer.Serialize(writer, xmlObjectData, xmlSerializerNamespaces);
        }
        else
        {
            _xmlSerializer.Serialize(writer, xmlObjectData);
        }

        string xml = writer.ToString();

        if (!shouldRemoveXmlSerializeEscapes) return xml;

        string unescapedXml = ProductDeserializeXmlTransformUtils.TransformToOriginalAfterSerialization(xml);

        return unescapedXml;
    }

    public OneOf<string, InvalidXmlResult> TrySerializeProductsXml(
        LegacyXmlObjectData xmlObjectData,
        bool shouldRemoveXmlSerializeEscapes,
        bool disableXsiAndXsdNamespaces = false)
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
}