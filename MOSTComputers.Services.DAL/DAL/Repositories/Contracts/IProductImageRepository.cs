using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductImageRepository
{
    bool DeleteAllWithSameProductIdInAllImages(uint productId);
    bool DeleteInAllImagesById(uint id);
    bool DeleteInFirstImagesByProductId(uint id);
    IEnumerable<ProductImage> GetAllFirstImagesForAllProducts();
    IEnumerable<ProductImage> GetAllInProduct(uint productId);
    ProductImage? GetByIdInAllImages(uint id);
    ProductImage? GetByProductIdInFirstImages(uint productId);
    IEnumerable<ProductImage> GetFirstImagesForSelectionOfProducts(List<uint> productIds);
    OneOf<uint, UnexpectedFailureResult> InsertInAllImages(ProductImageCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> UpdateInAllImages(ProductImageUpdateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> UpdateInFirstImages(ProductFirstImageUpdateRequest createRequest);
}