using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.StaticUtils;
using OneOf;
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
        string transformedXml = ProductDeserializeXmlTransformUtils.TransformInputToWorkingXml(xml);

        using StringReader reader = new(transformedXml);

        object? dataAsObject = _xmlSerializer.Deserialize(reader);

        if (dataAsObject is null) return null;

        XmlObjectData data = (XmlObjectData)dataAsObject;

        ProductDeserializeXmlTransformUtils.TransformXmlDataBackToNormal(data);

        return data;
    }

    public OneOf<XmlObjectData?, InvalidXmlResult> TryDeserializeProductsXml(string xml)
    {
        try
        {
            string transformedXml = ProductDeserializeXmlTransformUtils.TransformInputToWorkingXml(xml);

            using StringReader reader = new(transformedXml);

            object? dataAsObject = _xmlSerializer.Deserialize(reader);

            if (dataAsObject is null) return null;

            XmlObjectData data = (XmlObjectData)dataAsObject;

            ProductDeserializeXmlTransformUtils.TransformXmlDataBackToNormal(data);

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

    public string SerializeProductsXml(XmlObjectData xmlObjectData, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false)
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

    public string SerializeProductsXml(Product product, bool shouldRemoveXmlSerializeEscapes, bool disableXsiAndXsdNamespaces = false)
    {
        using StringWriter writer = new();

        XmlObjectData xmlObjectData = new()
        {
            Products = new()
            {
                _productToXmlProductMappingService.MapToXmlProduct(product)
            }
        };

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

    public OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(
        XmlObjectData xmlObjectData,
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

    public OneOf<string?, InvalidXmlResult> TrySerializeProductsXml(
        Product product,
        bool shouldRemoveXmlSerializeEscapes,
        bool disableXsiAndXsdNamespaces = false)
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
}