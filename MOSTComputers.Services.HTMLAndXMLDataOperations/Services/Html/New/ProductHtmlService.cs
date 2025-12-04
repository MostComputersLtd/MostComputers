using OneOf;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Utils;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New;
internal sealed class ProductHtmlService : IProductHtmlService
{
    public ProductHtmlService(XmlSerializerFactory xmlSerializerFactory, string fullPathToProductXslFile)
    {
        _xmlSerializer = xmlSerializerFactory.CreateSerializer(typeof(HtmlProductsData));

        XslCompiledTransform xslCompiledTransform = new(enableDebug: false);

        xslCompiledTransform.Load(fullPathToProductXslFile);

        _xslCompiledTransform = xslCompiledTransform;
    }

    private readonly XslCompiledTransform _xslCompiledTransform;

    private readonly XmlSerializer _xmlSerializer;

    public OneOf<string, InvalidXmlResult> TryGetHtmlFromProducts(HtmlProductsData htmlProductsData)
    {
        OneOf<string, InvalidXmlResult> serializeProductResult = TrySerializeProductHtmlDataToXml(htmlProductsData);

        return serializeProductResult.Match<OneOf<string, InvalidXmlResult>>(
            xml => GenerateProductHtmlFromXml(xml),
            invalidXmlResult => invalidXmlResult);
    }

    private string GenerateProductHtmlFromXml(string xml)
    {
        XmlReader reader = XmlReader.Create(new StringReader(xml));

        StringWriter writer = new();

        _xslCompiledTransform.Transform(reader, null, writer);

        string htmlFromProduct = writer.ToString();

        return FormatGeneratedHtml(htmlFromProduct);
    }

    private OneOf<string, InvalidXmlResult> TrySerializeProductHtmlDataToXml(HtmlProductsData productsData)
    {
        try
        {
            using StringWriter writer = new();

            _xmlSerializer.Serialize(writer, productsData);

            return writer.ToString();
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

    private static string FormatGeneratedHtml(string html)
    {
        return html.Replace("•", "&bull;")
            .Replace("\r\n\t\t\t\t\t\t", string.Empty);
    }
}