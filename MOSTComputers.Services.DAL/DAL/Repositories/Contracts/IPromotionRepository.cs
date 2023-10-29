using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Promotions;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
internal interface IPromotionRepository
{
    bool Delete(uint id);
    bool DeleteAllByProductId(uint productId);
    Promotion? GetActiveForProduct(uint productId);
    IEnumerable<Promotion> GetAll();
    IEnumerable<Promotion> GetAllActive();
    IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<uint> productIds);
    IEnumerable<Promotion> GetAllForProduct(uint productId);
    IEnumerable<Promotion> GetAllForSelectionOfProducts(List<uint> productIds);
    OneOf<Success, ValidationResult> Insert(PromotionCreateRequest createRequest, IValidator<PromotionCreateRequest>? validator = null);
    OneOf<Success, ValidationResult> Update(PromotionUpdateRequest updateRequest, IValidator<PromotionUpdateRequest>? validator = null);
}