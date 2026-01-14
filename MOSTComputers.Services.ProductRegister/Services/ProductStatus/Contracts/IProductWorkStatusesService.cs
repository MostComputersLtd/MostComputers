using FluentValidation.Results;
using OneOf;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductWorkStatuses;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
public interface IProductWorkStatusesService
{
    Task<List<ProductWorkStatuses>> GetAllAsync();
    Task<List<ProductWorkStatuses>> GetAllForProductsAsync(IEnumerable<int> productIds);
    Task<ProductWorkStatuses?> GetByIdAsync(int productWorkStatusesId);
    Task<ProductWorkStatuses?> GetByProductIdAsync(int productId);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertIfItDoesntExistAsync(ServiceProductWorkStatusesCreateRequest createRequest);
    Task<OneOf<ProductWorkStatusesCreateManyWithSameDataResponse, ValidationResult>> InsertAllIfTheyDontExistAsync(ServiceProductWorkStatusesCreateManyWithSameDataRequest createRequest);
    Task<OneOf<bool, ValidationResult>> UpdateByIdAsync(ServiceProductWorkStatusesUpdateByIdRequest updateRequest);
    Task<OneOf<bool, ValidationResult>> UpdateByProductIdAsync(ServiceProductWorkStatusesUpdateByProductIdRequest updateRequest);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertByProductIdAsync(ServiceProductWorkStatusesUpsertRequest upsertRequest);
    Task<bool> DeleteAllAsync();
    Task<bool> DeleteByIdAsync(int productWorkStatusesId);
    Task<bool> DeleteByProductIdAsync(int productId);
}