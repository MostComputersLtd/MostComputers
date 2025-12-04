using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
public interface IProductWorkStatusesChangesUpsertService
{
    Task<OneOf<Success, UnexpectedFailureResult>> UpsertChangesIntoProductStatusesAsync(string upsertUserName);
}