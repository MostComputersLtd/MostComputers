using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductStatuses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
//public interface IProductStatusesService
//{
//    bool DeleteByProductId(int productId);
//    IEnumerable<ProductStatuses> GetAll();
//    ProductStatuses? GetByProductId(int productId);
//    IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds);
//    Task<OneOf<Success, ValidationResult>> InsertIfItDoesntExistAsync(ProductStatusesCreateRequest createRequest);
//    OneOf<bool, ValidationResult> Update(ProductStatusesUpdateRequest updateRequest);
//}