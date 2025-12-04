using Microsoft.Extensions.Options;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.UI.Web.Blazor.Models.Configuration;
using MOSTComputers.UI.Web.Blazor.Services.ExternalXmlImport.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.Blazor.Services.ExternalXmlImport;
public class ProductXmlProvidingService : IProductXmlProvidingService
{
    public ProductXmlProvidingService(IOptions<LegacyPricelistSiteOptions> legacyPricelistSiteOptions, IServiceScopeFactory serviceScopeFactory)
    {
        _legacyPricelistSiteOptions = legacyPricelistSiteOptions;
        _serviceScopeFactory = serviceScopeFactory;
    }

    private readonly IOptions<LegacyPricelistSiteOptions> _legacyPricelistSiteOptions;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private string? _productXml = null;
    private LegacyXmlObjectData? _xmlObjectData = null;

    public async Task<OneOf<string, NotFound>> GetProductXmlAsync(bool updateCachedProductXml = false)
    {
        if (_productXml is not null && !updateCachedProductXml)
        {
            return _productXml;
        }

        OneOf<string, NotFound> getProductXmlResult = await GetNewProductXmlAsync();

        return getProductXmlResult.Match<OneOf<string, NotFound>>(
            productXml =>
            {
                SetProductXml(productXml);

                return productXml;
            },
            notFound => notFound);
    }

    public async Task<OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound>> GetProductXmlParsedAsync(bool updateCachedProductXml = false)
    {
        if (_xmlObjectData is not null && !updateCachedProductXml)
        {
            return _xmlObjectData;
        }

        OneOf<string, NotFound> getProductXmlResult = await GetProductXmlAsync(updateCachedProductXml);

        return getProductXmlResult.Match(
            productXml =>
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                ILegacyProductXmlService productDeserializeService
                    = scope.ServiceProvider.GetRequiredService<ILegacyProductXmlService>();

                OneOf<LegacyXmlObjectData?, InvalidXmlResult> deserializeProductXmlResult
                    = productDeserializeService.TryDeserializeProductsXml(productXml);

                return deserializeProductXmlResult.Match<OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound>>(
                    xmlObjectData =>
                    {
                        if (xmlObjectData is not null)
                        {
                            _xmlObjectData = xmlObjectData;

                            return xmlObjectData;
                        }

                        return new InvalidXmlResult();
                    },
                    invalidXmlResult => invalidXmlResult);
            },
            notFound => new NotFound()
        );
    }

    private async Task<OneOf<string, NotFound>> GetNewProductXmlAsync()
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        string? xmlDataPath = _legacyPricelistSiteOptions.Value.LegacyProductsXmlEndpointPath;

        if (xmlDataPath is null) return new NotFound();

        IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

        HttpClient client = httpClientFactory.CreateClient();

        return await client.GetStringAsync(xmlDataPath);
    }

    private void SetProductXml(string productXml)
    {
        _productXml = productXml;

        _xmlObjectData = null;
    }
}