using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Utils.OneOf;

using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;
using static MOSTComputers.Services.ProductRegister.Utils.ValidationUtils;
using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductImageFileNameInfoConstraints;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages;

internal sealed class ProductImageFileService : IProductImageFileService
{
    public ProductImageFileService(
        IProductRepository productRepository,
        IProductImageFileDataRepository imageFileNameInfoRepository,
        IProductImageRepository imageRepository,
        IProductImageFileManagementService productImageFileManagementService,
        IProductWorkStatusesWorkflowService productWorkStatusesWorkflowService,
        ITransactionExecuteService transactionExecuteService,

        IValidator<ProductImageFileCreateRequest>? createRequestValidator = null,
        IValidator<ProductImageFileUpdateRequest>? updateRequestValidator = null,
        IValidator<ProductImageFileChangeRequest>? updateFileRequestValidator = null,
        IValidator<ProductImageFileRenameRequest>? renameFileRequestValidator = null)
    {
        _productRepository = productRepository;
        _imageFileNameInfoRepository = imageFileNameInfoRepository;
        _imageRepository = imageRepository;
        _productImageFileManagementService = productImageFileManagementService;
        _productWorkStatusesWorkflowService = productWorkStatusesWorkflowService;
        _transactionExecuteService = transactionExecuteService;

        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _updateFileRequestValidator = updateFileRequestValidator;
        _renameFileRequestValidator = renameFileRequestValidator;
    }

    private const string _invalidFileTypeErrorMessage = "This file type is unsupported";
    private const string _imageFileDoesNotExistErrorMessage = "Image file does not exist";
    private const string _imageDoesNotExistErrorMessage = "Image does not exist";
    private const string _imageIsInDifferentProductErrorMessage = "Image does not belong to required product";
    private const string _imageAlreadyHasAFileErrorMessage = "Image is already has a file";
    private static readonly string _fileNameTooLongErrorMessage = $"File name cannot be longer than {FileNameMaxLength} characters";

    private readonly IProductRepository _productRepository;
    private readonly IProductImageFileDataRepository _imageFileNameInfoRepository;
    private readonly IProductImageRepository _imageRepository;
    private readonly IProductImageFileManagementService _productImageFileManagementService;
    private readonly IProductWorkStatusesWorkflowService _productWorkStatusesWorkflowService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    private readonly IValidator<ProductImageFileCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductImageFileUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<ProductImageFileChangeRequest>? _updateFileRequestValidator;
    private readonly IValidator<ProductImageFileRenameRequest>? _renameFileRequestValidator;

    public async Task<List<ProductImageFileData>> GetAllAsync()
    {
        return await _imageFileNameInfoRepository.GetAllAsync();
    }
    
    public async Task<List<IGrouping<int, ProductImageFileData>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return await _imageFileNameInfoRepository.GetAllInProductsAsync(productIds);
    }

    public async Task<List<ProductImageFileData>> GetAllInProductAsync(int productId)
    {
        if (productId <= 0) return new();

        return await _imageFileNameInfoRepository.GetAllInProductAsync(productId);
    }

    public async Task<ProductImageFileData?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _imageFileNameInfoRepository.GetByIdAsync(id);
    }

    public async Task<ProductImageFileData?> GetByProductIdAndImageIdAsync(int productId, int imageId)
    {
        if (productId <= 0 || imageId <= 0) return null;

        return await _imageFileNameInfoRepository.GetByProductIdAndImageIdAsync(productId, imageId);
    }

    public async Task<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertFileAsync(ProductImageFileCreateRequest createRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createRequestValidator, createRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => InsertFileInternalAsync(createRequest),
            result => result.Match(
                id => true,
                validationResult => false,
                fileSaveFailureResult => false,
                fileAlreadyExistsResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>> InsertFileInternalAsync(
        ProductImageFileCreateRequest createRequest)
    {
        Product? product = await _productRepository.GetByIdAsync(createRequest.ProductId);

        if (product is null)
        {
            ValidationResult productExistsValidationResult = new();

            productExistsValidationResult.Errors.Add(new(nameof(ProductImageFileCreateRequest.ProductId),
                "Id does not correspond to any known product"));

            return productExistsValidationResult;
        }

        if (createRequest.ImageId is not null)
        {
            ValidationResult imageValidationResult = await ValidateImageIdForFileAsync(createRequest.ImageId.Value, createRequest.ProductId, null);

            if (!imageValidationResult.IsValid) return imageValidationResult;
        }

        DateTime createDate = DateTime.Now;

        ProductImageFileNameInfoCreateRequest createRequestInternal = new()
        {
            ProductId = createRequest.ProductId,
            ImageId = createRequest.ImageId,
            FileName = createRequest.FileData?.FileName,
            CustomDisplayOrder = createRequest.CustomDisplayOrder,
            Active = createRequest.Active ?? false,
            CreateUserName = createRequest.CreateUserName,
            CreateDate = createDate,
            LastUpdateUserName = createRequest.CreateUserName,
            LastUpdateDate = createDate,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult = await _imageFileNameInfoRepository.InsertAsync(createRequestInternal);

        if (!imageFileNameInfoInsertResult.IsT0)
        {
            return imageFileNameInfoInsertResult.Map<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
        }

        int newImageFileId = imageFileNameInfoInsertResult.AsT0;

        if (createRequest.FileData is null)
        {
            return newImageFileId;
        }

        OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileAlreadyExistsResult> fileCreateResult
            = await _productImageFileManagementService.AddImageAsync(createRequest.FileData.FileName, createRequest.FileData.Data);

        if (!fileCreateResult.IsT0)
        {
            return fileCreateResult.Match<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => newImageFileId,
                fileSaveFailureResult => fileSaveFailureResult,
                directoryNotFoundResult => new UnexpectedFailureResult(),
                fileAlreadyExistsResult => fileAlreadyExistsResult);
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                createRequest.ProductId, ProductNewStatus.WorkInProgress, createRequest.CreateUserName);
        
        return upsertProductStatusResult.Match<OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
            statusId => newImageFileId,
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateFileAsync(
        ProductImageFileUpdateRequest updateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateRequestValidator, updateRequest);

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpdateFileInternalAsync(updateRequest),
            result => result.Match(
                success => true,
                validationResult => false,
                fileSaveFailureResult => false,
                fileDoesntExistResult => false,
                fileAlreadyExistsResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> UpdateFileInternalAsync(
        ProductImageFileUpdateRequest updateRequest)
    {
        ProductImageFileData? fileNameInfo = await GetByIdAsync(updateRequest.Id);

        if (fileNameInfo is null)
        {
            ValidationFailure validationFailure = new(nameof(ProductImageFileUpdateRequest.Id), _imageFileDoesNotExistErrorMessage);

            return CreateValidationResultFromErrors(validationFailure);
        }

        DateTime updateDate = DateTime.Now;

        int? newImageId = fileNameInfo.ImageId;

        if (updateRequest.UpdateImageIdRequest.IsT0)
        {
            newImageId = updateRequest.UpdateImageIdRequest.AsT0;
        }

        string? newFileName = fileNameInfo.FileName;

        if (updateRequest.UpdateFileDataRequest is not null)
        {
            newFileName = updateRequest.UpdateFileDataRequest?.FileData?.FileName;
        }

        ProductImageFileNameInfoByIdUpdateRequest updateRequestInternal = new()
        {
            Id = updateRequest.Id,
            ImageId = newImageId,
            ShouldUpdateDisplayOrder = updateRequest.NewDisplayOrder is not null,
            NewDisplayOrder = updateRequest.NewDisplayOrder,
            FileName = newFileName,
            Active = updateRequest.Active ?? false,
            LastUpdateUserName = updateRequest.UpdateUserName,
            LastUpdateDate = updateDate,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateImageFileNameInfoResult = await _imageFileNameInfoRepository.UpdateAsync(updateRequestInternal);

        if (!updateImageFileNameInfoResult.IsT0)
        {
            return updateImageFileNameInfoResult.Map<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
        }

        if (updateRequest.UpdateFileDataRequest is null) return new Success();

        if (fileNameInfo.FileName is not null)
        {
            OneOf<Success, FileDoesntExistResult> deleteOldFileResult = _productImageFileManagementService.DeleteFile(fileNameInfo.FileName);

            if (!deleteOldFileResult.IsT0)
            {
                return deleteOldFileResult.Map<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
            }
        }

        FileData? fileData = updateRequest.UpdateFileDataRequest.FileData;

        if (fileData is null) return new Success();

        OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileAlreadyExistsResult> addNewImageFileResult
           = await _productImageFileManagementService.AddImageAsync(fileData.FileName, fileData.Data);

        if (!addNewImageFileResult.IsT0)
        {
            return addNewImageFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => success,
                fileSaveFailureResult => fileSaveFailureResult,
                directoryNotFoundResult => new UnexpectedFailureResult(),
                fileAlreadyExistsResult => fileAlreadyExistsResult);
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                fileNameInfo.ProductId, ProductNewStatus.WorkInProgress, updateRequest.UpdateUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
            statusId => new Success(),
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> ChangeFileAsync(
        ProductImageFileChangeRequest fileChangeRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateFileRequestValidator, fileChangeRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => ChangeFileInternalAsync(fileChangeRequest),
            result => result.Match(
                success => true,
                validationResult => false,
                fileSaveFailureResult => false,
                fileDoesntExistResult => false,
                fileAlreadyExistsResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> ChangeFileInternalAsync(
        ProductImageFileChangeRequest fileChangeRequest)
    {
        ProductImageFileData? productImageFileNameInfo = await GetByIdAsync(fileChangeRequest.Id);

        if (productImageFileNameInfo is null)
        {
            ValidationFailure fileDoesNotExistError = new(nameof(ProductImageFileChangeRequest.Id), "File does not exist");

            return CreateValidationResultFromErrors(fileDoesNotExistError);
        }

        if (productImageFileNameInfo.FileName is not null)
        {
            OneOf<Success, FileDoesntExistResult> deleteOldFileResult = _productImageFileManagementService.DeleteFile(productImageFileNameInfo.FileName);

            if (!deleteOldFileResult.IsT0) return deleteOldFileResult.AsT0;
        }

        FileData newFileData = fileChangeRequest.NewFileData;

        OneOf<Success, FileSaveFailureResult, DirectoryNotFoundResult, FileAlreadyExistsResult> addNewImageFileResult
            = await _productImageFileManagementService.AddImageAsync(newFileData.FileName, newFileData.Data);

        if (!addNewImageFileResult.IsT0)
        {
            return addNewImageFileResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => success,
                fileSaveFailureResult => fileSaveFailureResult,
                directoryNotFoundResult => new UnexpectedFailureResult(),
                fileAlreadyExistsResult => fileAlreadyExistsResult);
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateFileNameInfoResult
            = await UpdateFileNameInfoWithNewFileNameAsync(productImageFileNameInfo, fileChangeRequest.UpdateUserName, newFileData.FileName);

        if (!updateFileNameInfoResult.IsT0)
        {
            return updateFileNameInfoResult.Map<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>();
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productImageFileNameInfo.ProductId, ProductNewStatus.WorkInProgress, fileChangeRequest.UpdateUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
            statusId => new Success(),
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    public async Task<OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> RenameFileAsync(
        ProductImageFileRenameRequest renameRequest)
    {
        ValidationResult validationResult = ValidateDefault(_renameFileRequestValidator, renameRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => RenameFileInternalAsync(renameRequest),
            result => result.Match(
                success => true,
                validationResult => false,
                notFound => false,
                fileDoesntExistResult => false,
                fileAlreadyExistsResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>> RenameFileInternalAsync(
        ProductImageFileRenameRequest renameRequest)
    {
        ProductImageFileData? productImageFileNameInfo = await GetByIdAsync(renameRequest.Id);

        if (productImageFileNameInfo is null) return new NotFound();

        if (productImageFileNameInfo.FileName is null)
        {
            return new FileDoesntExistResult()
            {
                FileName = string.Empty,
            };
        }

        string fileExtension = Path.GetExtension(productImageFileNameInfo.FileName);

        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return new UnexpectedFailureResult();
        }

        string newPath = $"{renameRequest.NewFileNameWithoutExtension}{fileExtension}";

        if (newPath.Length > FileNameMaxLength)
        {
            ValidationResult fileNameTooLongValidationResult = new();

            ValidationFailure fileNameTooLongValidationFailure = new(
                nameof(ProductImageFileRenameRequest.NewFileNameWithoutExtension),
                _fileNameTooLongErrorMessage);

            fileNameTooLongValidationResult.Errors.Add(fileNameTooLongValidationFailure);

            return fileNameTooLongValidationResult;
        }

        OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> fileRenameResult
            = _productImageFileManagementService.RenameImageFile(productImageFileNameInfo.FileName, renameRequest.NewFileNameWithoutExtension);

        if (!fileRenameResult.IsT0)
        {
            return fileRenameResult.Match<OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => success,
                fileDoesntExistResult => fileDoesntExistResult,
                fileAlreadyExistsResult => fileAlreadyExistsResult);
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateFileNameResult
            = await UpdateFileNameInfoWithNewFileNameAsync(productImageFileNameInfo, renameRequest.UpdateUserName, renameRequest.NewFileNameWithoutExtension);

        if (!updateFileNameResult.IsT0)
        {
            return updateFileNameResult.Match<OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
                success => success,
                validationResult => validationResult,
                unexpectedFailureResult => unexpectedFailureResult);
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productImageFileNameInfo.ProductId, ProductNewStatus.WorkInProgress, renameRequest.UpdateUserName);

        return upsertProductStatusResult.Match<OneOf<Success, ValidationResult, NotFound, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult>>(
            statusId => new Success(),
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateFileNameInfoWithNewFileNameAsync(
        ProductImageFileData productImageFileNameInfo, string updateUserName, string? newFileName)
    {
        DateTime updateDate = DateTime.Now;

        ProductImageFileNameInfoByIdUpdateRequest updateRequestInternal = new()
        {
            Id = productImageFileNameInfo.Id,
            ImageId = productImageFileNameInfo.ImageId,
            FileName = newFileName,
            Active = productImageFileNameInfo.Active,
            NewDisplayOrder = null,
            ShouldUpdateDisplayOrder = false,
            LastUpdateUserName = updateUserName,
            LastUpdateDate = updateDate,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateFileNameInfoResult
            = await _imageFileNameInfoRepository.UpdateAsync(updateRequestInternal);

        return updateFileNameInfoResult;
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteAllFilesForProductAsync(
        int productId, string deleteUserName)
    {
        if (productId <= 0) return new NotFound();

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteAllFilesForProductIdInternalAsync(productId, deleteUserName),
            result => result.Match(
                success => true,
                notFound => true,
                fileDoesntExistResult => false,
                validationResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteAllFilesForProductIdInternalAsync(
        int productId, string deleteUserName)
    {
        List<ProductImageFileData> fileNameInfosForProduct = await GetAllInProductAsync(productId);

        if (fileNameInfosForProduct.Count <= 0) return new NotFound();

        foreach (ProductImageFileData fileNameInfo in fileNameInfosForProduct)
        {
            OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult> deleteProductImageFileResult
                = await DeleteFileInternalAsync(fileNameInfo, deleteUserName);

            if (!deleteProductImageFileResult.IsT0)
            {
                return deleteProductImageFileResult;
            }
        }

        return new Success();
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileAsync(
        int id, string deleteUserName)
    {
        if (id <= 0) return new NotFound();

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteFileInternalAsync(id, deleteUserName),
            result => result.Match(
                success => true,
                notFound => true,
                fileDoesntExistResult => false,
                validationResult => false,
                unexpectedFailureResult => false));
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileAsync(
        ProductImageFileData fileNameInfo, string deleteUserName)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteFileInternalAsync(fileNameInfo, deleteUserName),
            result => result.Match(
                success => true,
                notFound => true,
                fileDoesntExistResult => false,
                validationResult => false,
                unexpectedFailureResult => false));
    }

    public async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileByProductIdAndImageIdAsync(
        int productId, int imageId, string deleteUserName)
    {
        if (productId < 0
            || imageId < 1)
        {
            return new NotFound();
        }

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => DeleteFileByImageIdInternalAsync(productId, imageId, deleteUserName),
            result => result.Match(
                success => true,
                notFound => true,
                fileDoesntExistResult => false,
                validationResult => false,
                unexpectedFailureResult => false));
    }

    private async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileByImageIdInternalAsync(
        int productId, int imageId, string deleteUserName)
    {
        ProductImageFileData? fileNameInfo = await GetByProductIdAndImageIdAsync(productId, imageId);

        if (fileNameInfo is null)
        {
            return new NotFound();
        }

        return await DeleteFileInternalAsync(fileNameInfo, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileInternalAsync(
        int id, string deleteUserName)
    {
        ProductImageFileData? fileNameInfo = await GetByIdAsync(id);

        if (fileNameInfo is null)
        {
            return new NotFound();
        }

        return await DeleteFileInternalAsync(fileNameInfo, deleteUserName);
    }

    private async Task<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>> DeleteFileInternalAsync(
        ProductImageFileData fileNameInfo, string deleteUserName)
    {
        bool isProductImageFileNameDeleted = await _imageFileNameInfoRepository.DeleteAsync(fileNameInfo.Id);

        if (!isProductImageFileNameDeleted) return new NotFound();

        if (fileNameInfo.FileName is not null)
        {
            OneOf<Success, FileDoesntExistResult> deleteFileResult
                = _productImageFileManagementService.DeleteFile(fileNameInfo.FileName);

            if (!deleteFileResult.IsT0)
            {
                return deleteFileResult.Map<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>();
            }
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                fileNameInfo.ProductId, ProductNewStatus.WorkInProgress, deleteUserName);

        return upsertProductStatusResult.Match<OneOf<Success, NotFound, FileDoesntExistResult, ValidationResult, UnexpectedFailureResult>>(
            statusId => new Success(),
            validationResult => validationResult,
            unexpectedFailureResult => unexpectedFailureResult);
    }

    private async Task<ValidationResult> ValidateImageIdForFileAsync(
        int newImageId,
        int productId,
        int? fileInfoId,
        string imageIdPropertyName = nameof(ProductImageFileCreateRequest.ImageId))
    {
        ProductImage? image = await _imageRepository.GetByIdInAllImagesAsync(newImageId);

        if (image is null)
        {
            ValidationFailure imageDoesNotExistError = new(imageIdPropertyName, _imageDoesNotExistErrorMessage);

            return CreateValidationResultFromErrors(imageDoesNotExistError);
        }

        if (image.ProductId != productId)
        {
            ValidationFailure imageIsInDifferentProductError = new(imageIdPropertyName, _imageIsInDifferentProductErrorMessage);

            return CreateValidationResultFromErrors(imageIsInDifferentProductError);
        }

        List<ProductImageFileData> imageFiles = await GetAllInProductAsync(productId);

        foreach (ProductImageFileData imageFile in imageFiles)
        {
            if (imageFile.Id != fileInfoId
                && imageFile.ImageId == newImageId)
            {
                ValidationFailure imageAlreadyHasAFileError = new(imageIdPropertyName, _imageAlreadyHasAFileErrorMessage);

                return CreateValidationResultFromErrors(imageAlreadyHasAFileError);
            }
        }

        return new();
    }
}