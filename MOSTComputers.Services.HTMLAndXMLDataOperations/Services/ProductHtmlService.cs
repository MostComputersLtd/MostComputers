using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using OneOf;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Xsl;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations.Services;

internal sealed class ProductHtmlService : IProductHtmlService
{
    public ProductHtmlService(IProductDeserializeService productDeserializeService)
    {
        _productDeserializeService = productDeserializeService;
    }

    private const string _pathToProductXsl = "XSL/Product.xsl";

    private readonly IProductDeserializeService _productDeserializeService;

    public string GetHtmlFromProduct(Product product)
    {
        return RootProjectFolderProductHtmlMethodCaller.CallGetProductToHtmlMethod(this, product);
    }

    public OneOf<string, InvalidXmlResult> TryGetHtmlFromProduct(Product product)
    {
        return RootProjectFolderProductHtmlMethodCaller.CallTryGetProductToHtmlMethod(this, product);
    }

    internal string GetHtmlFromProductInternal(Product product, [CallerFilePath] string callerFilePath = "")
    {
        int indexOfFileNameInPath = callerFilePath.LastIndexOf('\\');

        string rootProjectPath = callerFilePath[..indexOfFileNameInPath];

        string xslFileFullPath = Path.Combine(rootProjectPath, _pathToProductXsl);

        xslFileFullPath = xslFileFullPath.Replace('\\', '/');

        string xml = _productDeserializeService.SerializeProductXml(product, true);

        XslCompiledTransform xslCompiledTransform = new(enableDebug: true);

        xslCompiledTransform.Load(xslFileFullPath);

        using XmlReader reader = XmlReader.Create(new StringReader(xml));

        using StringWriter writer = new();

        xslCompiledTransform.Transform(reader, null, writer);

        string htmlFromProduct = writer.ToString();

        return FormatGeneratedHtml(htmlFromProduct);
    }

    internal OneOf<string, InvalidXmlResult> TryGetHtmlFromProductInternal(Product product, [CallerFilePath] string callerFilePath = "")
    {
        int indexOfFileNameInPath = callerFilePath.LastIndexOf('\\');

        string rootProjectPath = callerFilePath[..indexOfFileNameInPath];

        string xslFileFullPath = Path.Combine(rootProjectPath, _pathToProductXsl);

        xslFileFullPath = xslFileFullPath.Replace('\\', '/');

        OneOf<string, InvalidXmlResult> serializeProductResult = _productDeserializeService.TrySerializeProductXml(product, true);

        return serializeProductResult.Match<OneOf<string, InvalidXmlResult>>(
            xml =>
            {
                XslCompiledTransform xslCompiledTransform = new(enableDebug: true);

                xslCompiledTransform.Load(xslFileFullPath);

                using XmlReader reader = XmlReader.Create(new StringReader(xml));

                using StringWriter writer = new();

                xslCompiledTransform.Transform(reader, null, writer);

                string htmlFromProduct = writer.ToString();

                return FormatGeneratedHtml(htmlFromProduct);
            },
            invalidXmlResult => invalidXmlResult);

    }

    private static string FormatGeneratedHtml(string html)
    {
        return html.Replace("•", "&bull;")
            .Replace("\r\n\t\t\t\t\t\t", "");
    }
}