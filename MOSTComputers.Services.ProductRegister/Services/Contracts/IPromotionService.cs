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
    Promotion? GetActiveForProduct(uint productId);
    IEnumerable<Promotion> GetAll();
    IEnumerable<Promotion> GetAllActive();
    IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<uint> productIds);
    IEnumerable<Promotion> GetAllForProduct(uint productId);
    IEnumerable<Promotion> GetAllForSelectionOfProducts(List<uint> productIds);
    OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ServicePromotionCreateRequest createRequest, IValidator<ServicePromotionCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ServicePromotionUpdateRequest updateRequest, IValidator<ServicePromotionUpdateRequest>? validator = null);
    bool Delete(uint id);
    bool DeleteAllByProductId(uint productId);
}