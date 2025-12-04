namespace MOSTComputers.UI.Web.Services.ExternalXmlImport.Contracts;

public interface IProductLegacyXmlReadService
{
    Task<string> GetProductXmlAsync(int uid);
}