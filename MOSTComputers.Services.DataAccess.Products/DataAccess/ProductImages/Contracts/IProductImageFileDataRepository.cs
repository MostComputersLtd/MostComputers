using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;
public interface IProductImageFileDataRepository
{
    Task<List<ProductImageFileData>> GetAllAsync();
    Task<List<IGrouping<int, ProductImageFileData>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductImageFileData>> GetAllInProductAsync(int productId);
    Task<ProductImageFileData?> GetByIdAsync(int id);
    Task<ProductImageFileData?> GetByProductIdAndImageIdAsync(int productId, int imageId);
    Task<ProductImageFileData?> GetByFileNameAsync(string fileName);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertAsync(ProductImageFileNameInfoCreateRequest createRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateAsync(ProductImageFileNameInfoByIdUpdateRequest updateRequest);
    Task<bool> DeleteAllForProductIdAsync(int productId);
    Task<bool> DeleteAsync(int id);
}