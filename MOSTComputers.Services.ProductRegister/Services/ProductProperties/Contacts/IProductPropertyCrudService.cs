using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
public interface IProductPropertyCrudService
{
    Task<List<ProductProperty>> GetAllAsync();
    Task<List<IGrouping<int, ProductProperty>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductProperty>> GetAllInProductAsync(int productId);
    Task<ProductProperty?> GetByProductAndCharacteristicIdAsync(int productId, int characteristicId);
    Task<List<ProductPropertiesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds);
    Task<int> GetCountOfAllInProductAsync(int productId);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceProductPropertyByCharacteristicIdCreateRequest createRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateAsync(ProductPropertyUpdateRequest updateRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAsync(ProductPropertyUpdateRequest upsertRequest);
    Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdAsync(int productId, int oldCharacteristicId, int newCharacteristicId);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesAsync(ProductPropertyUpsertAllForProductRequest productPropertyChangeAllForProductRequest);
    Task<bool> DeleteAsync(int productId, int characteristicId);
    Task<bool> DeleteAllForCharacteristicAsync(int characteristicId);
    Task<bool> DeleteAllForProductAsync(int productId);
}