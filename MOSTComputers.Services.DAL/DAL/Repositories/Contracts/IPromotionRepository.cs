using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

public interface IPromotionRepository
{
    IEnumerable<Promotion> GetAll();
    IEnumerable<Promotion> GetAllActive();
    IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<int> productIds);
    IEnumerable<Promotion> GetAllForProduct(int productId);
    IEnumerable<Promotion> GetAllForSelectionOfProducts(List<int> productIds);
    Promotion? GetActiveForProduct(int productId);
    OneOf<int, UnexpectedFailureResult> Insert(PromotionCreateRequest createRequest);
    OneOf<Success, UnexpectedFailureResult> Update(PromotionUpdateRequest updateRequest);
    bool Delete(int id);
    bool DeleteAllByProductId(int productId);
}