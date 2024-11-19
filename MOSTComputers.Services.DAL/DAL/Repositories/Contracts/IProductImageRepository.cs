using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductImageRepository
{
    bool DeleteAllWithSameProductIdInAllImages(int productId);
    bool DeleteInAllImagesAndImageFilePathInfosById(int id);
    bool DeleteInAllImagesById(int id);
    bool DeleteInFirstImagesByProductId(int id);
    IEnumerable<ProductImage> GetAllFirstImagesForAllProducts();
    IEnumerable<ProductImage> GetAllInProduct(int productId);
    ProductImage? GetByIdInAllImages(int id);
    ProductImage? GetByProductIdInFirstImages(int productId);
    IEnumerable<ProductImage> GetFirstImagesForSelectionOfProducts(List<int> productIds);
    OneOf<int, UnexpectedFailureResult> InsertInAllImages(ProductImageCreateRequest createRequest);
    OneOf<int, UnexpectedFailureResult> InsertInAllImagesAndImageFileNameInfos(ProductImageCreateRequest createRequest, int? displayOrder = null);
    OneOf<Success, UnexpectedFailureResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest);
    OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInAllImagesById(int imageId, string htmlData);
    OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInFirstImagesByProductId(int productId, string htmlData);
    OneOf<bool, UnexpectedFailureResult> UpdateHtmlDataInFirstAndAllImagesByProductId(int productId, string htmlData);
    OneOf<Success, UnexpectedFailureResult> UpdateInAllImages(ProductImageUpdateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> UpdateInFirstImages(ProductFirstImageUpdateRequest createRequest);
}