using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;

public interface IPromotionService
{
    Promotion? GetActiveForProduct(int productId);
    IEnumerable<Promotion> GetAll();
    IEnumerable<Promotion> GetAllActive();
    IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<int> productIds);
    IEnumerable<Promotion> GetAllForProduct(int productId);
    IEnumerable<Promotion> GetAllForSelectionOfProducts(List<int> productIds);
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ServicePromotionCreateRequest createRequest, IValidator<ServicePromotionCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServicePromotionUpdateRequest updateRequest, IValidator<ServicePromotionUpdateRequest>? validator = null);
    bool Delete(int id);
    bool DeleteAllByProductId(int productId);
}