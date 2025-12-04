using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
public interface IProductWorkStatusesWorkflowService
{
    Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertProductNewStatusAsync(int productId, ProductNewStatus productNewStatusEnum, string upsertUserName);
    Task<OneOf<int?, ValidationResult, UnexpectedFailureResult>> UpsertProductNewStatusIfConditionIsMetAsync(int productId, ProductNewStatus productNewStatusEnum, Predicate<ProductWorkStatuses?> condition, string upsertUserName);
    Task<OneOf<int?, ValidationResult, UnexpectedFailureResult>> UpsertProductNewStatusToGivenStatusIfItsNewAsync(int productId, ProductNewStatus newStatus, string upsertUserName);
}