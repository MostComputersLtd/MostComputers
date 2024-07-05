using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductImageFileNameInfoRepository
{
    IEnumerable<ProductImageFileNameInfo> GetAll();
    IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(ProductImageFileNameInfoByImageNumberUpdateRequest updateRequest);
    bool DeleteAllForProductId(uint productId);
    bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(ProductImageFileNameInfoByFileNameUpdateRequest updateRequest);
    bool DeleteByProductIdAndImageNumber(uint productId, int imageNumber);
}