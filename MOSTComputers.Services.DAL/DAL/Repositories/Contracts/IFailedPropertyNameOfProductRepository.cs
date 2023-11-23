using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface IFailedPropertyNameOfProductRepository
{
    bool Delete(uint productId, string propertyName);
    bool DeleteAllForProduct(uint productId);
    bool DeleteAllForSelectionOfProducts(IEnumerable<uint> productIds);
    IEnumerable<FailedPropertyNameOfProduct> GetAll();
    IEnumerable<FailedPropertyNameOfProduct> GetAllForProduct(uint productId);
    IEnumerable<FailedPropertyNameOfProduct> GetAllForSelectionOfProducts(IEnumerable<uint> productIds);
    bool Insert(FailedPropertyNameOfProductCreateRequest createRequest);
    bool MultiInsert(FailedPropertyNameOfProductMultiCreateRequest createRequest);
    bool Update(FailedPropertyNameOfProductUpdateRequest updateRequest);
}