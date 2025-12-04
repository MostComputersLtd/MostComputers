using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;
using MOSTComputers.Utils.OneOf;

using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Files.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Files.PromotionFiles;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles;
internal sealed class PromotionFileService : IPromotionFileService
{
    public PromotionFileService(
        IPromotionFileInfoRepository promotionFileInfoService,
        IPromotionProductFileInfoRepository promotionProductFileInfoRepository,
        IPromotionFileManagementService promotionFileManagementService,
        ITransactionExecuteService transactionExecuteService,
        IValidator<CreatePromotionFileRequest>? createRequestValidator = null,
        IValidator<UpdatePromotionFileRequest>? updateRequestValidator = null)
    {
        _promotionFileInfoRepository = promotionFileInfoService;
        _promotionProductFileInfoRepository = promotionProductFileInfoRepository;
        _promotionFileManagementService = promotionFileManagementService;
        _transactionExecuteService = transactionExecuteService;

        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private const string _idDoesNotExistErrorMessage = "Id does not correspond to any known promotion file id";

    private readonly IPromotionFileInfoRepository _promotionFileInfoRepository;
    private readonly IPromotionProductFileInfoRepository _promotionProductFileInfoRepository;
    private readonly IPromotionFileManagementService _promotionFileManagementService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    private readonly IValidator<CreatePromotionFileRequest>? _createRequestValidator;
    private readonly IValidator<UpdatePromotionFileRequest>? _updateRequestValidator;

    public async Task<List<PromotionFileInfo>> GetAllAsync()
    {
        return await _promotionFileInfoRepository.GetAllAsync();
    }

    public async Task<List<PromotionFileInfo>> GetAllByActivityAsync(bool active = true)
    {
        return await _promotionFileInfoRepository.GetAllByActivityAsync(active);
    }

    public async Task<PromotionFileInfo?> GetByIdAsync(int id)
    {
        return await _promotionFileInfoRepository.GetByIdAsync(id);
    }

    public async Task<Stream?> GetFileDataByIdAsync(int fileInfoId)
    {
        PromotionFileInfo? fileInfo = await _promotionFileInfoRepository.GetByIdAsync(fileInfoId);

        if (fileInfo is null)
        {
            return null;
        }

        return _promotionFileManagementService.GetFileStream(fileInfo.FileName);
    }

    public async Task<List<PromotionFileInfo>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _promotionFileInfoRepository.GetByIdsAsync(ids);
    }

    public async Task<OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertAsync(
        CreatePromotionFileRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        OneOf<int, FileAlreadyExistsResult, UnexpectedFailureResult> insertResult
            = await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                x => InsertInternalAsync(x),
                result => result.Match(
                    id => true,
                    fileAlreadyExistsResult => false,
                    unexpectedFailureResult => false),
                createRequest);

        return insertResult.Map<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
    }

    private async Task<OneOf<int, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertInternalAsync(
        CreatePromotionFileRequest createRequest)
    {
        DateTime createDate = DateTime.Now;

        PromotionFileInfoCreateRequest promotionFileInfoCreateRequest = new()
        {
            Name = createRequest.Name,
            Active = createRequest.Active,
            FileName = createRequest.FileName,
            ValidFrom = createRequest.ValidFrom,
            ValidTo = createRequest.ValidTo,
            Description = createRequest.Description,
            RelatedProductsString = createRequest.RelatedProductsString,
            CreateUserName = createRequest.CreateUserName,
            CreateDate = createDate,
            LastUpdateUserName = createRequest.CreateUserName,
            LastUpdateDate = createDate
        };

        OneOf<int, UnexpectedFailureResult> createPromotionFileInfoResult
            = await _promotionFileInfoRepository.InsertAsync(promotionFileInfoCreateRequest);

        return await createPromotionFileInfoResult.MatchAsync(
            async id =>
            {
                OneOf<Success, FileAlreadyExistsResult> addFileResult
                    = await _promotionFileManagementService.AddFileAsync(createRequest.FileName, createRequest.FileData);

                return addFileResult.Match<OneOf<int, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                    success => id,
                    fileAlreadyExistsResult => fileAlreadyExistsResult);
            },
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateAsync(
        UpdatePromotionFileRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            x => UpdateInternalAsync(x),
            result => result.Match(
                id => true,
                validationResult => false,
                fileDoesntExistResult => false,
                fileAlreadyExistsResult => false,
                unexpectedFailureResult => false),
            updateRequest);
    }

    private async Task<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateInternalAsync(
        UpdatePromotionFileRequest updateRequest)
    {
        PromotionFileInfo? currentPromotionFileInfo = await _promotionFileInfoRepository.GetByIdAsync(updateRequest.Id);

        if (currentPromotionFileInfo is null)
        {
            ValidationResult invalidIdValidationResult = new();

            ValidationFailure invalidIdValidationFailure = new(nameof(UpdatePromotionFileRequest.Id), _idDoesNotExistErrorMessage);

            invalidIdValidationResult.Errors.Add(invalidIdValidationFailure);

            return invalidIdValidationResult;
        }

        string newFileName = currentPromotionFileInfo.FileName;

        if (updateRequest.NewFileData is not null
            && currentPromotionFileInfo.FileName != updateRequest.NewFileData.FileName)
        {
            newFileName = updateRequest.NewFileData.FileName;
        }

        DateTime updateDate = DateTime.Now;

        PromotionFileInfoUpdateRequest promotionFileInfoUpdateRequest = new()
        {
            Id = updateRequest.Id,
            Name = updateRequest.Name,
            Active = updateRequest.Active,
            FileName = newFileName,
            ValidFrom = updateRequest.ValidFrom,
            ValidTo = updateRequest.ValidTo,
            Description = updateRequest.Description,
            RelatedProductsString = updateRequest.RelatedProductsString,
            LastUpdateUserName = updateRequest.UpdateUserName,
            LastUpdateDate = updateDate,
        };

        OneOf<Success, UnexpectedFailureResult> updatePromotionFileInfoResult
            = await _promotionFileInfoRepository.UpdateAsync(promotionFileInfoUpdateRequest);

        return await updatePromotionFileInfoResult.MatchAsync(
            async success =>
            {
                if (updateRequest.NewFileData is null) return new Success();

                string currentFileExtension = Path.GetExtension(currentPromotionFileInfo.FileName);
                string newFileExtension = Path.GetExtension(updateRequest.NewFileData.FileName);

                if (currentFileExtension == newFileExtension)
                {
                    OneOf<Success, FileDoesntExistResult> updateFileResult = await _promotionFileManagementService.UpdateFileAsync(
                        currentPromotionFileInfo.FileName, updateRequest.NewFileData.Data);

                    return updateFileResult.Map<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
                }

                OneOf<Success, FileDoesntExistResult> deleteFileResult
                    = _promotionFileManagementService.DeleteFile(currentPromotionFileInfo.FileName);

                OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> resultFromFileDelete
                    = deleteFileResult.Match<OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                        success => success,
                        fileDoesntExistResult => fileDoesntExistResult);

                if (!resultFromFileDelete.IsT0) return resultFromFileDelete;

                OneOf<Success, FileAlreadyExistsResult> addNewFileResult
                    = await _promotionFileManagementService.AddFileAsync(newFileName, updateRequest.NewFileData.Data);

                return addNewFileResult.Map<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
            },
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult>> DeleteAsync(
        int promotionFileInfoId)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            x => DeleteInternalAsync(x),
            result => result.Match(
                success => true,
                notFound => false,
                promotionFileHasRelationsResult => false,
                fileDoesntExistResult => false),
            promotionFileInfoId);
    }

    private async Task<OneOf<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult>> DeleteInternalAsync(int promotionFileInfoId)
    {
        PromotionFileInfo? promotionFileInfoToDelete = await _promotionFileInfoRepository.GetByIdAsync(promotionFileInfoId);

        if (promotionFileInfoToDelete is null) return new NotFound();

        bool doesPromotionFileHaveRelatedProducts = await _promotionProductFileInfoRepository.DoesExistForPromotionFileAsync(promotionFileInfoId);

        if (doesPromotionFileHaveRelatedProducts)
        {
            return new PromotionFileHasRelationsResult()
            {
                PromotionFileId = promotionFileInfoId,
            };
        }

        bool deletePromotionFileInfoResult = await _promotionFileInfoRepository.DeleteAsync(promotionFileInfoId);

        if (!deletePromotionFileInfoResult) return new NotFound();

        OneOf<Success, FileDoesntExistResult> deleteFileResult = _promotionFileManagementService.DeleteFile(promotionFileInfoToDelete.FileName);

        return deleteFileResult.Map<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult>();
    }
}