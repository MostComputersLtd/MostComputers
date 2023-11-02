using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Promotions;
using MOSTComputers.Services.DAL.Models.Responses;
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
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(PromotionCreateRequest createRequest, IValidator<PromotionCreateRequest>? validator = null);
    OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(PromotionUpdateRequest updateRequest, IValidator<PromotionUpdateRequest>? validator = null);
    bool Delete(uint id);
    bool DeleteAllByProductId(uint productId);
}