using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts.ExternalXmlImport;

public interface IProductXmlProvidingService
{
    Task<OneOf<string, NotFound>> GetProductXmlAsync();
}