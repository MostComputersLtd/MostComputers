using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface IFailedPropertyNameOfProductRepository
{
    bool Delete(int productId, string propertyName);
    bool DeleteAllForProduct(int productId);
    bool DeleteAllForSelectionOfProducts(IEnumerable<int> productIds);
    IEnumerable<FailedPropertyNameOfProduct> GetAll();
    IEnumerable<FailedPropertyNameOfProduct> GetAllForProduct(int productId);
    IEnumerable<FailedPropertyNameOfProduct> GetAllForSelectionOfProducts(IEnumerable<int> productIds);
    bool Insert(FailedPropertyNameOfProductCreateRequest createRequest);
    bool MultiInsert(FailedPropertyNameOfProductMultiCreateRequest createRequest);
    bool Update(FailedPropertyNameOfProductUpdateRequest updateRequest);
}