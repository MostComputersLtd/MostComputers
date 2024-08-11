using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IProductImageFileNameInfoService
{
    IEnumerable<ProductImageFileNameInfo> GetAll();
    IEnumerable<ProductImageFileNameInfo> GetAllInProduct(int productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ServiceProductImageFileNameInfoCreateRequest createRequest, IValidator<ServiceProductImageFileNameInfoCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServiceProductImageFileNameInfoByImageNumberUpdateRequest updateRequest, IValidator<ServiceProductImageFileNameInfoByImageNumberUpdateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(ServiceProductImageFileNameInfoByFileNameUpdateRequest updateRequest, IValidator<ServiceProductImageFileNameInfoByFileNameUpdateRequest>? validator = null);
    bool DeleteByProductIdAndDisplayOrder(int productId, int displayOrder);
    bool DeleteAllForProductId(int productId);
    bool DeleteByProductIdAndImageNumber(int productId, int imageNumber);
}