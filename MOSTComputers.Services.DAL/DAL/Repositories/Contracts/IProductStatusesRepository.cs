using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
public interface IProductStatusesRepository
{
    bool DeleteByProductId(int productId);
    IEnumerable<ProductStatuses> GetAll();
    ProductStatuses? GetByProductId(int productId);
    IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds);
    OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest);
    bool Update(ProductStatusesUpdateRequest updateRequest);
}