using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Services.Contracts;
public interface ISearchStringOriginService
{
    Dictionary<string, List<SearchStringPartOriginData>?>? GetSearchStringPartsAndDataAboutTheirOrigin(Product product);
}