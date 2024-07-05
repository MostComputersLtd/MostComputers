using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductStatusesService
{
    bool DeleteByProductId(uint productId);
    IEnumerable<ProductStatuses> GetAll();
    ProductStatuses? GetByProductId(uint productId);
    IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<uint> productIds);
    OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest);
    OneOf<bool, ValidationResult> Update(ProductStatusesUpdateRequest updateRequest);
}