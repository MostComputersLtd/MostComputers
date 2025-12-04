using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImage;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;
public interface IProductImageRepository
{
    Task<List<IGrouping<int, ProductImageData>>> GetAllWithoutFileDataAsync();
    Task<List<IGrouping<int, ProductImage>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductImage>> GetAllInProductAsync(int productId);
    Task<ProductImage?> GetByIdInAllImagesAsync(int id);
    Task<List<IGrouping<int, ProductImageData>>> GetAllInProductsWithoutFileDataAsync(IEnumerable<int> productIds);
    Task<List<ProductImageData>> GetAllInProductWithoutFileDataAsync(int productId);
    Task<ProductImageData?> GetByIdInAllImagesWithoutFileDataAsync(int id);
    Task<bool> DoesProductImageExistAsync(int imageId);
    Task<List<ProductImagesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds);
    Task<int> GetCountOfAllInProductAsync(int productId);
    Task<List<ProductImage>> GetAllFirstImagesForAllProductsAsync();
    Task<List<ProductImage>> GetFirstImagesForSelectionOfProductsAsync(IEnumerable<int> productIds);
    Task<ProductImage?> GetByProductIdInFirstImagesAsync(int productId);
    Task<List<ProductFirstImageExistsForProductData>> DoProductsHaveImagesInFirstImagesAsync(IEnumerable<int> productIds);
    Task<bool> DoesProductHaveImageInFirstImagesAsync(int productId);
    Task<OneOf<int, UnexpectedFailureResult>> InsertInAllImagesAsync(ProductImageCreateRequest createRequest);
    Task<OneOf<Success, UnexpectedFailureResult>> UpdateInAllImagesAsync(ProductImageUpdateRequest updateRequest);
    Task<OneOf<Success, UnexpectedFailureResult>> InsertInFirstImagesAsync(ProductFirstImageCreateRequest createRequest);
    Task<OneOf<Success, UnexpectedFailureResult>> UpdateInFirstImagesAsync(ProductFirstImageUpdateRequest updateRequest);
    Task<OneOf<bool, UnexpectedFailureResult>> UpdateHtmlDataInAllImagesByIdAsync(int imageId, string htmlData);
    Task<OneOf<bool, UnexpectedFailureResult>> UpdateHtmlDataInFirstAndAllImagesByProductIdAsync(int productId, string htmlData);
    Task<OneOf<bool, UnexpectedFailureResult>> UpdateHtmlDataInFirstImagesByProductIdAsync(int productId, string htmlData);
    Task<bool> DeleteAllInAllImagesByProductIdAsync(int productId);
    Task<bool> DeleteInAllImagesByIdAsync(int id);
    Task<bool> DeleteInFirstImagesByProductIdAsync(int id);
}