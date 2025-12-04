using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FirstImage;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
internal interface IProductImageAndFileService
{
    Task<List<IGrouping<int, ProductImageData>>> GetAllWithoutFileDataAsync();
    Task<List<IGrouping<int, ProductImage>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<IGrouping<int, ProductImageData>>> GetAllInProductsWithoutFileDataAsync(IEnumerable<int> productIds);
    Task<List<ProductImagesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductImage>> GetAllInProductAsync(int productId);
    Task<List<ProductImageData>> GetAllInProductWithoutFileDataAsync(int productId);
    Task<int> GetCountOfAllInProductAsync(int productId);
    Task<List<ProductImage>> GetAllFirstImagesForAllProductsAsync();
    Task<ProductImage?> GetByIdInAllImagesAsync(int id);
    Task<ProductImageData?> GetByIdInAllImagesWithoutFileDataAsync(int id);
    Task<bool> DoesProductImageExistAsync(int imageId);
    Task<List<ProductImage>> GetFirstImagesForSelectionOfProductsAsync(List<int> productIds);
    Task<List<ProductFirstImageExistsForProductData>> DoProductsHaveImagesInFirstImagesAsync(List<int> productIds);
    Task<ProductImage?> GetByProductIdInFirstImagesAsync(int productId);
    Task<bool> DoesProductHaveImageInFirstImagesAsync(int productId);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> InsertInAllImagesAsync(ServiceProductImageCreateRequest createRequest);
    Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInAllImagesWithFileAsync(ProductImageWithFileCreateRequest productImageWithFileCreateRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertInFirstImagesAsync(ServiceProductFirstImageCreateRequest createRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInAllImagesAsync(ServiceProductImageUpdateRequest updateRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateInFirstImagesAsync(ServiceProductFirstImageUpdateRequest updateRequest);
    Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInAllImagesByIdAsync(int imageId, string htmlData);
    Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(int productId, string htmlData);
    Task<OneOf<bool, ValidationResult, UnexpectedFailureResult>> UpdateHtmlDataInFirstImagesByProductIdAsync(int productId, string htmlData);
    Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateInAllImagesWithFileAsync(ProductImageWithFileUpdateRequest productImageWithFileUpdateRequest);
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertInAllImagesAsync(ProductImageUpsertRequest productImageUpsertRequest);
    Task<OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertInAllImagesWithFileAsync(ProductImageWithFileUpsertRequest productImageWithFileUpsertRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertInFirstImagesAsync(ServiceProductFirstImageUpsertRequest upsertRequest);
    Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesForProductAsync(int productId, List<ProductImageForProductUpsertRequest> imageUpsertRequests);
    Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpsertFirstAndAllImagesWithFilesForProductAsync(int productId, List<ProductImageWithFileForProductUpsertRequest> imageAndFileNameUpsertRequests, string deleteUserName);
    Task<bool> DeleteAllImagesForProductAsync(int productId);
    Task<bool> DeleteInAllImagesByIdAsync(int id);
    Task<OneOf<Success, NotFound, FileDoesntExistResult, UnexpectedFailureResult>> DeleteInAllImagesByIdWithFileAsync(int id, string deleteUserName);
    Task<bool> DeleteInFirstImagesByProductIdAsync(int productId);
}