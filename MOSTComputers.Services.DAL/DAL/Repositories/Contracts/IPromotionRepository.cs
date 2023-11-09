using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Promotions;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IPromotionRepository
{
    IEnumerable<Promotion> GetAll();
    IEnumerable<Promotion> GetAllActive();
    IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<uint> productIds);
    IEnumerable<Promotion> GetAllForProduct(uint productId);
    IEnumerable<Promotion> GetAllForSelectionOfProducts(List<uint> productIds);
    Promotion? GetActiveForProduct(uint productId);
    OneOf<Success, UnexpectedFailureResult> Insert(PromotionCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> Update(PromotionUpdateRequest updateRequest);
    bool Delete(uint id);
    bool DeleteAllByProductId(uint productId);
}