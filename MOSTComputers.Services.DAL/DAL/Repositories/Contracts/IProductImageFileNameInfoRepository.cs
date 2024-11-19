using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductImageFileNameInfoRepository
{
    IEnumerable<ProductImageFileNameInfo> GetAll();
    IEnumerable<ProductImageFileNameInfo> GetAllInProduct(int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(ProductImageFileNameInfoByImageNumberUpdateRequest updateRequest);
    bool DeleteAllForProductId(int productId);
    bool DeleteByProductIdAndDisplayOrder(int productId, int displayOrder);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(ProductImageFileNameInfoByFileNameUpdateRequest updateRequest);
    bool DeleteByProductIdAndImageNumber(int productId, int imageNumber);
    ProductImageFileNameInfo? GetByProductIdAndImageNumber(int productId, int imageNumber);
    ProductImageFileNameInfo? GetByFileName(string fileName);
    int? GetHighestImageNumber(int productId);
}