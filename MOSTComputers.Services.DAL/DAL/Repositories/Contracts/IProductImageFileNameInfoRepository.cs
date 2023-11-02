using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IProductImageFileNameInfoRepository
{
    IEnumerable<ProductImageFileNameInfo> GetAll();
    IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductImageFileNameInfoUpdateRequest updateRequest);
    bool DeleteAllForProductId(uint productId);
    bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder);
}