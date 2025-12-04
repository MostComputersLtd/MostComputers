using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
using OneOf;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IProductWorkStatusesRepository
{
    Task<List<ProductWorkStatuses>> GetAllAsync();
    Task<List<ProductWorkStatuses>> GetAllForProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductWorkStatuses>> GetAllWithProductNewStatusAsync(ProductNewStatus productNewStatusEnum);
    Task<List<ProductWorkStatuses>> GetAllWithProductXmlStatusAsync(ProductXmlStatus productXmlStatusEnum);
    Task<List<ProductWorkStatuses>> GetAllWithReadyForImageInsertAsync(bool readyForImageInsert);
    Task<ProductWorkStatuses?> GetByIdAsync(int productWorkStatusesId);
    Task<ProductWorkStatuses?> GetByProductIdAsync(int productId);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertIfItDoesntExistAsync(ProductWorkStatusesCreateRequest createRequest);
    Task<bool> UpdateByIdAsync(ProductWorkStatusesUpdateByIdRequest updateRequest);
    Task<bool> UpdateByProductIdAsync(ProductWorkStatusesUpdateByProductIdRequest updateRequest);
    Task<OneOf<int, UnexpectedFailureResult>> UpsertByProductIdAsync(ProductWorkStatusesUpsertRequest upsertRequest);
    Task<bool> DeleteAllAsync();
    Task<bool> DeleteAllWithProductNewStatusAsync(ProductNewStatus productNewStatusEnum);
    Task<bool> DeleteAllWithProductXmlStatusAsync(ProductXmlStatus productXmlStatusEnum);
    Task<bool> DeleteAllWithReadyForImageInsertAsync(bool readyForImageInsert);
    Task<bool> DeleteByIdAsync(int productWorkStatusesId);
    Task<bool> DeleteByProductIdAsync(int productId);
}