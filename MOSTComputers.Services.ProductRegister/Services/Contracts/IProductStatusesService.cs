using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.DAL.Models.Requests.ProductStatuses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductStatusesService
{
    bool DeleteByProductId(int productId);
    IEnumerable<ProductStatuses> GetAll();
    ProductStatuses? GetByProductId(int productId);
    IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds);
    OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest);
    OneOf<bool, ValidationResult> Update(ProductStatusesUpdateRequest updateRequest);
}