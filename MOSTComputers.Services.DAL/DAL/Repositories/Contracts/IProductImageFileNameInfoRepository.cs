using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts
{
    internal interface IProductImageFileNameInfoRepository
    {
        bool DeleteAllForProductId(uint productId);
        bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder);
        IEnumerable<ProductImageFileNameInfo> GetAll();
        IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId);
        OneOf<Success, ValidationResult> Insert(ProductImageFileNameInfoCreateRequest createRequest, IValidator<ProductImageFileNameInfoCreateRequest>? validator = null);
        OneOf<Success, ValidationResult> Update(ProductImageFileNameInfoUpdateRequest updateRequest, IValidator<ProductImageFileNameInfoUpdateRequest>? validator = null);
    }
}