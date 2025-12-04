using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Counters;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Counters.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services.ProductStatus;
internal sealed class ProductWorkStatusesChangesUpsertService : IProductWorkStatusesChangesUpsertService
{
    public ProductWorkStatusesChangesUpsertService(IProductWorkStatusesService productWorkStatusesService,
        IOriginalLocalChangesReadService originalLocalChangesReadService,
        ISystemCountersRepository systemCountersRepository,
        ITransactionExecuteService transactionExecuteService)
    {
        _productWorkStatusesService = productWorkStatusesService;
        _originalLocalChangesReadService = originalLocalChangesReadService;
        _systemCountersRepository = systemCountersRepository;
        _transactionExecuteService = transactionExecuteService;
    }

    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IOriginalLocalChangesReadService _originalLocalChangesReadService;
    private readonly ISystemCountersRepository _systemCountersRepository;
    private readonly ITransactionExecuteService _transactionExecuteService;

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpsertChangesIntoProductStatusesAsync(string upsertUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertChangesIntoProductStatusesInternalAsync(upsertUserName),
            result => result.Match(
                success => true,
                unexpectedFailureResult => false),
            transactionOptions: new()
            {
                Timeout = TimeSpan.FromMinutes(10),
            });
    }

    private async Task<OneOf<Success, UnexpectedFailureResult>> UpsertChangesIntoProductStatusesInternalAsync(string upsertUserName)
    {
        List<LocalChangeData> changes = await _originalLocalChangesReadService.GetAllForOperationTypeAsync(ChangeOperationType.Create);

        if (changes.Count <= 0) return new Success();

        SystemCounters? systemCounters = await _systemCountersRepository.GetSystemCountersAsync();

        if (systemCounters is null) return new UnexpectedFailureResult();

        List<LocalChangeData> newChanges = changes.Where(x => x.Id > systemCounters.OriginalChangesLastSearchedPK)
            .ToList();

        if (newChanges.Count <= 0) return new Success();

        foreach (LocalChangeData newChange in newChanges)
        {
            ServiceProductWorkStatusesUpsertRequest productStatusUpsertRequest = new()
            {
                ProductId = newChange.TableElementId,
                ProductNewStatus = ProductNewStatus.New,
                ProductXmlStatus = ProductXmlStatus.NotReady,
                ReadyForImageInsert = false,
                UpsertUserName = upsertUserName,
            };

            OneOf<int, ValidationResult, UnexpectedFailureResult> upsertStatusResult
                = await _productWorkStatusesService.UpsertByProductIdAsync(productStatusUpsertRequest);

            if (!upsertStatusResult.IsT0)
            {
                return upsertStatusResult.Match(
                    id => new UnexpectedFailureResult(),
                    validationResult => new UnexpectedFailureResult(),
                    unexpectedFailureResult => unexpectedFailureResult);
            }
        }

        LocalChangeData? lastChange = newChanges.MaxBy(x => x.Id);

        if (lastChange is null) return new Success();

        SystemCounters newCounters = new()
        {
            OriginalChangesLastSearchedPK = lastChange.Id,
        };

        return await _systemCountersRepository.UpdateAsync(newCounters);
    }
}