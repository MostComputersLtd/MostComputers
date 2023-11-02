using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductImageFileNameInfoService
{
    IEnumerable<ProductImageFileNameInfo> GetAll();
    IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest, IValidator<ProductImageFileNameInfoCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductImageFileNameInfoUpdateRequest updateRequest, IValidator<ProductImageFileNameInfoUpdateRequest>? validator = null);
    bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder);
    bool DeleteAllForProductId(uint productId);
}