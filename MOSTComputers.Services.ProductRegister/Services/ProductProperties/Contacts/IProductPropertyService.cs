using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
public interface IProductPropertyService
{
    Task<List<IGrouping<int, ProductProperty>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductPropertiesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductProperty>> GetAllInProductAsync(int productId);
    Task<int> GetCountOfAllInProductAsync(int productId);
    Task<ProductProperty?> GetByProductAndCharacteristicIdAsync(int productId, int characteristicId);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ServiceProductPropertyByCharacteristicIdCreateRequest createRequest, string createUserName);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateAsync(ProductPropertyUpdateRequest updateRequest, string updateUserName);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAsync(ProductPropertyUpdateRequest upsertRequest, string upsertUserName);
    Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> ChangePropertyCharacteristicIdAsync(int productId, int oldCharacteristicId, int newCharacteristicId, string changeUserName);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertAllProductPropertiesAsync(ProductPropertyUpsertAllForProductRequest productPropertyChangeAllForProductRequest, string upsertUserName);
    Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAsync(int productId, int characteristicId, string deleteUserName);
    Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllForProductAsync(int productId, string deleteUserName);
    Task<OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult>> DeleteAllForCharacteristicAsync(int characteristicId, string deleteUserName);
    Task<List<IGrouping<int, ProductProperty>>> GetAllAsync();
}