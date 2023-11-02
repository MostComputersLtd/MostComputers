using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImage;
using MOSTComputers.Services.DAL.Models.Responses;
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
    OneOf<Success, UnexpectedFailureResult> InsertInAllImages(ProductImageCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> InsertInFirstImages(ProductFirstImageCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> UpdateInAllImages(ProductImageUpdateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> UpdateInFirstImages(ProductFirstImageUpdateRequest createRequest);
}