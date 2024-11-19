using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IFailedPropertyNameOfProductService
{
    bool Delete(int productId, string propertyName);
    bool DeleteAllForProduct(int productId);
    bool DeleteAllForSelectionOfProducts(IEnumerable<int> productIds);
    IEnumerable<FailedPropertyNameOfProduct> GetAll();
    IEnumerable<FailedPropertyNameOfProduct> GetAllForProduct(int productId);
    IEnumerable<FailedPropertyNameOfProduct> GetAllForSelectionOfProducts(IEnumerable<int> productIds);
    OneOf<bool, ValidationResult> Insert(FailedPropertyNameOfProductCreateRequest createRequest);
    OneOf<bool, ValidationResult> MultiInsert(FailedPropertyNameOfProductMultiCreateRequest createRequest);
    OneOf<bool, ValidationResult> Update(FailedPropertyNameOfProductUpdateRequest updateRequest);
}