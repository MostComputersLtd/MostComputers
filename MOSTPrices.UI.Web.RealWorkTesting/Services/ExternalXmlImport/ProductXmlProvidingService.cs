using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts.ExternalXmlImport;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.ExternalXmlImport;

public class ProductXmlProvidingService : IProductXmlProvidingService
{
    public ProductXmlProvidingService(IServiceScopeFactory serviceScopeFactory)
    {
       
        _serviceScopeFactory = serviceScopeFactory;
    }

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private string? _productXml = null;

    public async Task<OneOf<string, NotFound>> GetProductXmlAsync()
    {
        if (_productXml is null)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string? xmlDataPath = configuration.GetRequiredSection("ExternalDataPaths").GetValue<string>("AllProductsXmlEndpointPath");

            if (xmlDataPath is null) return new NotFound();

            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            using HttpClient client = httpClientFactory.CreateClient();

            _productXml = await client.GetStringAsync(xmlDataPath);
        }

        return _productXml;
    }
}