using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductIdentifiers.ProductGTINCode;

namespace MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers.Contracts;
public interface IProductGTINCodeService
{
    Task<List<IGrouping<int, ProductGTINCode>>> GetAllForProductsAsync(List<int> productIds);
    Task<List<ProductGTINCode>> GetAllForProductAsync(int productId);
    Task<ProductGTINCode?> GetByProductIdAndTypeAsync(int productId, ProductGTINCodeType productGTINCodeType);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceProductGTINCodeCreateRequest createRequest);
    Task<OneOf<Success, NotFound, ValidationResult>> UpdateAsync(ServiceProductGTINCodeUpdateRequest updateRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAsync(ServiceProductGTINCodeUpsertRequest upsertRequest);
    Task<OneOf<Success, NotFound>> DeleteAsync(int productId, ProductGTINCodeType productGTINCodeType);
}