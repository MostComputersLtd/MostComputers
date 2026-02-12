using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
internal interface IProductImageCrudService
{
    Task<List<IGrouping<int, ProductImageData>>> GetAllWithoutFileDataAsync();
    Task<List<ProductImage>> GetAllInProductAsync(int productId);
    Task<List<IGrouping<int, ProductImage>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<IGrouping<int, ProductImageData>>> GetAllInProductsWithoutFileDataAsync(IEnumerable<int> productIds);
    Task<List<ProductImageData>> GetAllInProductWithoutFileDataAsync(int productId);
    Task<ProductImage?> GetByIdInAllImagesAsync(int id);
    Task<ProductImageData?> GetByIdInAllImagesWithoutFileDataAsync(int id);
    Task<List<ProductImagesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds);
    Task<int> GetCountOfAllInProductAsync(int productId);
    Task<bool> DoesProductImageExistAsync(int imageId);
    Task<List<ProductImage>> GetAllFirstImagesForAllProductsAsync();
    Task<List<ProductImage>> GetFirstImagesForSelectionOfProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductFirstImageExistsForProductData>> DoProductsHaveImagesInFirstImagesAsync(IEnumerable<int> productIds);
    Task<ProductImage?> GetByProductIdInFirstImagesAsync(int productId);
    Task<bool> DoesProductHaveImageInFirstImagesAsync(int productId);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertInAllImagesAsync(ServiceProductImageCreateRequest createRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInAllImagesAsync(ServiceProductImageUpdateRequest updateRequest);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertInAllImagesAsync(ProductImageUpsertRequest productImageUpsertRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInFirstImagesAsync(ServiceProductFirstImageCreateRequest createRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInFirstImagesAsync(ServiceProductFirstImageUpdateRequest updateRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInFirstImagesAsync(ServiceProductFirstImageUpsertRequest upsertRequest);
    Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInAllImagesByIdAsync(int imageId, string htmlData);
    Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstImagesByProductIdAsync(int productId, string htmlData);
    Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(int productId, string htmlData);
    Task<bool> DeleteAllInAllImagesByProductIdAsync(int productId);
    Task<bool> DeleteInAllImagesByIdAsync(int id);
    Task<bool> DeleteInFirstImagesByProductIdAsync(int productId);
    int GetMinimumImagesAllInsertIdForLocalApplication();
    Task<List<ProductImageData>> GetAllFirstImagesWithoutFileDataForAllProductsAsync();
    Task<List<ProductImageData>> GetFirstImagesWithoutFileDataForSelectionOfProductsAsync(IEnumerable<int> productIds);
}