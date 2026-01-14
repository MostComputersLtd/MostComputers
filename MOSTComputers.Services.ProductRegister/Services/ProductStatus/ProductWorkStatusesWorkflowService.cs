using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus;
internal sealed class ProductWorkStatusesWorkflowService : IProductWorkStatusesWorkflowService
{
    public ProductWorkStatusesWorkflowService(IProductWorkStatusesService productWorkStatusesService)
    {
        _productWorkStatusesService = productWorkStatusesService;
    }

    private readonly IProductWorkStatusesService _productWorkStatusesService;

    public async Task<OneOf<int?, ValidationResult, UnexpectedFailureResult>> UpsertProductNewStatusToGivenStatusIfItsNewAsync(
        int productId, ProductNewStatus newStatus, string upsertUserName)
    {
        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductNewStatusToWorkInProgressResult
            = await UpsertProductNewStatusIfConditionIsMetAsync(
                productId,
                newStatus,
                productWorkStatuses => (productWorkStatuses is null) || (productWorkStatuses.ProductNewStatus == ProductNewStatus.New),
                upsertUserName);

        return upsertProductNewStatusToWorkInProgressResult;
    }

    public async Task<OneOf<int?, ValidationResult, UnexpectedFailureResult>> UpsertProductNewStatusIfConditionIsMetAsync(
       int productId, ProductNewStatus productNewStatusEnum, Predicate<ProductWorkStatuses?> condition, string upsertUserName)
    {
        ProductWorkStatuses? productWorkStatus = await _productWorkStatusesService.GetByProductIdAsync(productId);

        if (!condition(productWorkStatus)) return productWorkStatus?.Id;

        OneOf<int, ValidationResult, UnexpectedFailureResult> upsertProductNewStatus
            = await UpsertProductNewStatusInternalAsync(productId, productNewStatusEnum, upsertUserName, productWorkStatus);

        return upsertProductNewStatus.Match<OneOf<int?, ValidationResult, UnexpectedFailureResult>>(
            id => id,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertProductNewStatusAsync(
        int productId, ProductNewStatus productNewStatusEnum, string upsertUserName)
    {
        ProductWorkStatuses? productWorkStatus = await _productWorkStatusesService.GetByProductIdAsync(productId);

        return await UpsertProductNewStatusInternalAsync(productId, productNewStatusEnum, upsertUserName, productWorkStatus);
    }

    private async Task<OneOf<int, ValidationResult, UnexpectedFailureResult>> UpsertProductNewStatusInternalAsync(
        int productId,
        ProductNewStatus productNewStatusEnum,
        string upsertUserName,
        ProductWorkStatuses? productWorkStatus)
    {
        if (productWorkStatus is null)
        {
            ServiceProductWorkStatusesCreateRequest createRequest = new()
            {
                ProductId = productId,
                ProductNewStatus = productNewStatusEnum,
                ProductXmlStatus = ProductXmlStatus.NotReady,
                ReadyForImageInsert = false,
                CreateUserName = upsertUserName,
            };

            return await _productWorkStatusesService.InsertIfItDoesntExistAsync(createRequest);
        }

        if (productWorkStatus.ProductNewStatus == productNewStatusEnum)
        {
            return productWorkStatus.Id;
        }

        ServiceProductWorkStatusesUpdateByProductIdRequest updateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = productNewStatusEnum,
            ProductXmlStatus = productWorkStatus.ProductXmlStatus,
            ReadyForImageInsert = productWorkStatus.ReadyForImageInsert,
            LastUpdateUserName = upsertUserName,
        };

        OneOf<bool, ValidationResult> updateResult = await _productWorkStatusesService.UpdateByProductIdAsync(updateRequest);

        return updateResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult>>(
            isSuccessful =>
            {
                if (isSuccessful) return productWorkStatus.Id;

                return new UnexpectedFailureResult();
            },
            validationResult => validationResult);
    }
}