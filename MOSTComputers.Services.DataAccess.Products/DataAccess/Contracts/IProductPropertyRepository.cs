using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
public interface IProductPropertyRepository
{
    Task<List<ProductProperty>> GetAllAsync();
    Task<List<IGrouping<int, ProductProperty>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductPropertiesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductProperty>> GetAllInProductAsync(int productId);
    Task<int> GetCountOfAllInProductAsync(int productId);
    Task<ProductProperty?> GetByProductAndCharacteristicIdAsync(int productId, int characteristicId);
    Task<ProductProperty?> GetByNameAndProductIdAsync(string name, int productId);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ProductPropertyCreateRequest createRequest);
    Task<OneOf<Success, UnexpectedFailureResult>> UpdateAsync(ProductPropertyUpdateRequest updateRequest);
    Task<OneOf<Success, UnexpectedFailureResult>> UpsertAsync(ProductPropertyUpsertRequest upsertRequest);
    Task<bool> DeleteAllForCharacteristicAsync(int characteristicId);
    Task<bool> DeleteAllForProductAsync(int productId);
    Task<bool> DeleteAsync(int productId, int characteristicId);
}