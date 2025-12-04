using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.UI.Web.Services.ExternalXmlImport.Contracts;

namespace MOSTComputers.UI.Web.Services.ExternalXmlImport;
internal sealed class ProductLegacyXmlReadService : IProductLegacyXmlReadService
{
    public ProductLegacyXmlReadService(string xmlDataPath, IHttpClientFactory httpClientFactory)
    {
        _xmlDataPath = xmlDataPath;
        _httpClientFactory = httpClientFactory;
    }

    private const string _dataPathSuffix = "?id=";

    private readonly string _xmlDataPath;
    private readonly IHttpClientFactory _httpClientFactory;

    public async Task<string> GetProductXmlAsync(int uid)
    {
        string productXmlDataPath = _xmlDataPath + _dataPathSuffix + uid.ToString();

        HttpClient httpClient = _httpClientFactory.CreateClient();

        string productXmlData = await httpClient.GetStringAsync(productXmlDataPath);

        return productXmlData;
    }
}