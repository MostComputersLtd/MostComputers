using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IFailedPropertyNameOfProductService
{
    bool Delete(uint productId, string propertyName);
    bool DeleteAllForProduct(uint productId);
    bool DeleteAllForSelectionOfProducts(IEnumerable<uint> productIds);
    IEnumerable<FailedPropertyNameOfProduct> GetAll();
    IEnumerable<FailedPropertyNameOfProduct> GetAllForProduct(uint productId);
    IEnumerable<FailedPropertyNameOfProduct> GetAllForSelectionOfProducts(IEnumerable<uint> productIds);
    OneOf<bool, ValidationResult> Insert(FailedPropertyNameOfProductCreateRequest createRequest);
    OneOf<bool, ValidationResult> MultiInsert(FailedPropertyNameOfProductMultiCreateRequest createRequest);
    OneOf<bool, ValidationResult> Update(FailedPropertyNameOfProductUpdateRequest updateRequest);
}