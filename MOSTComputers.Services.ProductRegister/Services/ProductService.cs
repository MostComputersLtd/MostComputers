using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Contracts;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.Services;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using System.Transactions;
using MOSTComputers.Services.ProductRegister.Mapping;
using MOSTComputers.Models.FileManagement.Models;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;
using static MOSTComputers.Services.ProductRegister.StaticUtilities.ImageUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;
using static MOSTComputers.Services.ProductImageFileManagement.Utils.ProductImageFileManagementUtils;
using static MOSTComputers.Utils.OneOf.OneOfExtensions;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductService : IProductService
{
    public ProductService(
        IProductRepository productRepository,
        IProductPropertyService productPropertyService,
        IProductCharacteristicService productCharacteristicService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductWorkStatusesService productWorkStatusesService,
        IProductImageFileManagementService productImageFileManagementService,
        IProductHtmlService productHtmlService,
        ITransactionExecuteService transactionExecuteService,
        ProductMapper productMapper,
        IValidator<ProductCreateRequest>? createRequestValidator = null,
        IValidator<ProductCreateWithoutImagesInDatabaseRequest>? createWithoutImagesInDatabaseRequestValidator = null,
        IValidator<ProductUpdateWithoutImagesInDatabaseRequest>? updateWithoutImagesInDatabaseRequestValidator = null,
        IValidator<ProductUpdateRequest>? updateRequestValidator = null,
        IValidator<ProductFullUpdateRequest>? fullUpdateRequestValidator = null)
    {
        _productRepository = productRepository;
        _productPropertyService = productPropertyService;
        _productCharacteristicService = productCharacteristicService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productWorkStatusesService = productWorkStatusesService;
        _productImageFileManagementService = productImageFileManagementService;
        _productHtmlService = productHtmlService;
        _transactionExecuteService = transactionExecuteService;

        _productMapper = productMapper;

        _createRequestValidator = createRequestValidator;
        _createWithoutImagesInDatabaseRequestValidator = createWithoutImagesInDatabaseRequestValidator;
        _updateWithoutImagesInDatabaseRequestValidator = updateWithoutImagesInDatabaseRequestValidator;
        _updateRequestValidator = updateRequestValidator;
        _fullUpdateRequestValidator = fullUpdateRequestValidator;
    }

    private readonly IProductRepository _productRepository;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IProductImageFileManagementService _productImageFileManagementService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    private readonly ProductMapper _productMapper;

    private readonly IValidator<ProductCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductUpdateRequest>? _updateRequestValidator;
    private readonly IValidator<ProductCreateWithoutImagesInDatabaseRequest>? _createWithoutImagesInDatabaseRequestValidator;
    private readonly IValidator<ProductUpdateWithoutImagesInDatabaseRequest>? _updateWithoutImagesInDatabaseRequestValidator;
    private readonly IValidator<ProductFullUpdateRequest>? _fullUpdateRequestValidator;

    public IEnumerable<Product> GetAllWithoutImagesAndProps()
    {
        return _productRepository.GetAll_WithManifacturerAndCategory();
    }

    public IEnumerable<Product> GetAllInCategoryWithoutImagesAndProps(int categoryId)
    {
        return _productRepository.GetAllInCategory_WithManifacturerAndCategory(categoryId);
    }

    public IEnumerable<Product> GetAllWhereSearchStringMatches(string searchStringParts)
    {
        return _productRepository.GetAll_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(searchStringParts);
    }

    public IEnumerable<Product> GetAllWhereNameMatches(string subString)
    {
        return _productRepository.GetAll_WithManifacturerAndCategory_WhereSearchNameContainsSubstring(subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereSearchStringMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0
            || string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        uint end = (uint)productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(productRangeSearchRequest.Start, end, subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereNameMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0
            || string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        uint end = (uint)productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategory_WhereNameContainsSubstring(productRangeSearchRequest.Start, end, subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereAllConditionsAreMet(ProductRangeSearchRequest productRangeSearchRequest, ProductConditionalSearchRequest productConditionalSearchRequest)
    {
        if (productRangeSearchRequest.Start <= 0
            || productRangeSearchRequest.Length == 0) return Enumerable.Empty<Product>();

        uint end = (uint)productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategoryAndStatuses_WhereAllConditionsAreMet(productRangeSearchRequest.Start, end, productConditionalSearchRequest);
    }

    public IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _productRepository.GetAll_WithManifacturerAndCategory_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithFirstImage(List<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        return _productRepository.GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithProps(List<int> ids)
    {
        ids = RemoveValuesSmallerThanOne(ids);

        if (ids.Count > 1000)
        {
            List<Product> products = new();

            for (int i = 0; i < ids.Count; i += 1000)
            {
                int remainingIdsInThisIteration = Math.Min(1000, ids.Count - i);

                List<int> partOfIds = ids.GetRange(i, remainingIdsInThisIteration);

                IEnumerable<Product> partOfProducts = _productRepository.GetAll_WithManifacturerAndCategoryAndProperties_ByIds(partOfIds);

                products.AddRange(partOfProducts);
            }

            return products;
        }

        return _productRepository.GetAll_WithManifacturerAndCategoryAndProperties_ByIds(ids);
    }

    public IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(ProductRangeSearchRequest rangeSearchRequest)
    {
        if (rangeSearchRequest.Start <= 0
            || rangeSearchRequest.Length == 0) return Enumerable.Empty<Product>();

        uint end = (uint)rangeSearchRequest.Start + rangeSearchRequest.Length;

        if (rangeSearchRequest.Start == end) return Enumerable.Empty<Product>();

        return _productRepository.GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(rangeSearchRequest.Start, end);
    }

    public Product? GetByIdWithFirstImage(int id)
    {
        if (id <= 0) return null;

        return _productRepository.GetById_WithManifacturerAndCategoryAndFirstImage(id);
    }

    public Product? GetByIdWithProps(int id)
    {
        if (id <= 0) return null;

        return _productRepository.GetById_WithManifacturerAndCategoryAndProperties(id);
    }

    public Product? GetByIdWithImages(int id)
    {
        if (id <= 0) return null;

        return _productRepository.GetById_WithManifacturerAndCategoryAndImages(id);
    }

    public Product? GetProductWithHighestId()
    {
        Product? product = _productRepository.GetProductWithHighestId_WithManifacturerAndCategory();

        if (product is null) return null;

        if (product.Properties is null
            || product.Properties.Count <= 0)
        {
            product.Properties = _productPropertyService.GetAllInProduct(product.Id)
                .ToList();
        }

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count <= 0)
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(product.Id)
                .ToList();
        }

        if (product.Images is null
            || product.Images.Count <= 0)
        {
            product.Images = _productImageService.GetAllInProduct(product.Id)
                .ToList();
        }

        return product;
    }

    public Product? GetProductFull(int productId)
    {
        if (productId <= 0) return null;

        Product? product = _productRepository.GetById_WithManifacturerAndCategoryAndImages(productId);

        if (product is null) return null;

        if (product.Properties is null
            || product.Properties.Count <= 0)
        {
            product.Properties = _productPropertyService.GetAllInProduct(productId)
                .ToList();
        }

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count <= 0)
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
                .ToList();
        }

        return product;
    }

    public Product? GetProductFullWithHighestId()
    {
        Product? product = _productRepository.GetProductWithHighestId_WithManifacturerAndCategory();

        if (product is null) return null;

        if (product.Properties is null
            || product.Properties.Count <= 0)
        {
            product.Properties = _productPropertyService.GetAllInProduct(product.Id)
                .ToList();
        }

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count <= 0)
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(product.Id)
                .ToList();
        }

        if (product.Images is null
            || product.Images.Count <= 0)
        {
            product.Images = _productImageService.GetAllInProduct(product.Id)
                .ToList();
        }

        return product;
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest,
        IValidator<ProductCreateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(createRequest, validator, _createRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        if (createRequest.Images is not null)
        {
            foreach (var item in createRequest.Images)
            {
                item.DateModified = DateTime.Now;
            }
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _productRepository.Insert(createRequest);

        return result;
    }

    public async Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        InsertWithImagesOnlyInDirectoryAsync(ProductCreateWithoutImagesInDatabaseRequest productWithoutImagesInDBCreateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_createWithoutImagesInDatabaseRequestValidator, productWithoutImagesInDBCreateRequest);

        if (!validationResult.IsValid) return validationResult;

        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => InsertWithImagesOnlyInDirectoryInternalAsync(productWithoutImagesInDBCreateRequest),
            result => result.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false,
                directoryNotFoundResult => false,
                fileDoesntExistResult => false));
    }

    private async Task<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        InsertWithImagesOnlyInDirectoryInternalAsync(ProductCreateWithoutImagesInDatabaseRequest productWithoutImagesInDBCreateRequest)
    {
        ProductCreateRequest productCreateRequest = _productMapper.MapWithoutIncludingImages(productWithoutImagesInDBCreateRequest);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = Insert(productCreateRequest);

        int productId = -1;

        bool isProductInsertSuccessful = productInsertResult.Match(
            id =>
            {
                productId = id;

                return true;
            },
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isProductInsertSuccessful
            || productWithoutImagesInDBCreateRequest.ImageFileAndFileNameInfoUpsertRequests is null)
        {
            return productInsertResult.Map<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
        }

        List<ProductImageFileNameInfo> oldImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
            .ToList();

        ValidationResult oldFileNamesValidationResult = ValidateOldFileNames(
            productWithoutImagesInDBCreateRequest.ImageFileAndFileNameInfoUpsertRequests, oldImageFileNames);

        if (!oldFileNamesValidationResult.IsValid) return oldFileNamesValidationResult;

        foreach (ImageFileAndFileNameInfoUpsertRequest fileAndFileNameInfoUpsertRequest in productWithoutImagesInDBCreateRequest.ImageFileAndFileNameInfoUpsertRequests)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> upsertImageFileAndFileNameInfoResult
                = await UpsertImageFileAndImageFileNameInfoAsync(productId, fileAndFileNameInfoUpsertRequest);

            if (upsertImageFileAndFileNameInfoResult.Value is not Success)
            {
                return upsertImageFileAndFileNameInfoResult.Match<OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                    success => productId,
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult,
                    directoryNotFoundResult => directoryNotFoundResult,
                    fileDoesntExistResult => fileDoesntExistResult);
            }
        }

        return productId;
    }









    private static ValidationResult ValidateOldFileNames(IEnumerable<ImageFileAndFileNameInfoUpsertRequest> imageFileUpsertRequests,
        IEnumerable<ProductImageFileNameInfo> productImageFileNameInfos)
    {
        foreach (ImageFileAndFileNameInfoUpsertRequest fileAndFileNameInfoUpsertRequest in imageFileUpsertRequests)
        {
            if (fileAndFileNameInfoUpsertRequest.OldFileName is not null
                && productImageFileNameInfos.FirstOrDefault(x => x.FileName == fileAndFileNameInfoUpsertRequest.OldFileName) is null)
            {
                return GetInvalidOldFileNameValidationResult(fileAndFileNameInfoUpsertRequest.OldFileName);
            }
        }

        return new();
    }






    private OneOf<Success, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult> DeleteFilesAndFileNameInfosThatAreNotPresentInRequest(
        int productId,
        List<ImageFileAndFileNameInfoUpsertRequest> imageFileUpsertRequests)
    {
        List<ProductImageFileNameInfo> oldImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
            .OrderBy(x => x.ImageNumber)
            .ToList();

        ValidationResult oldFileNamesValidationResult = ValidateOldFileNames(
            imageFileUpsertRequests, oldImageFileNames);

        if (!oldFileNamesValidationResult.IsValid) return oldFileNamesValidationResult;

        if (oldImageFileNames.Count <= 0) return new Success();

        int lowestAlteredImageNumber = 0;

        for (int i = 0; i < oldImageFileNames.Count; i++)
        {
            ProductImageFileNameInfo imageFileNameInfo = oldImageFileNames[i];

            ImageFileAndFileNameInfoUpsertRequest? matchingUpsertRequest
                = imageFileUpsertRequests.FirstOrDefault(x => x.OldFileName == imageFileNameInfo.FileName);

            if (matchingUpsertRequest is not null) continue;

            bool isFileNameInfoDeleted = _productImageFileNameInfoService.DeleteByProductIdAndImageNumber(productId, imageFileNameInfo.ImageNumber);

            if (!isFileNameInfoDeleted) return new UnexpectedFailureResult();

            if (imageFileNameInfo.FileName is not null)
            {
                _productImageFileManagementService.DeleteFile(imageFileNameInfo.FileName);
            }

            if (imageFileNameInfo.ImageNumber < lowestAlteredImageNumber)
            {
                lowestAlteredImageNumber = imageFileNameInfo.ImageNumber;
            }

            oldImageFileNames.Remove(imageFileNameInfo);

            i--;

            ChangeAllFileNameInfosToMatchChangesInDelete(oldImageFileNames, imageFileNameInfo);
        }

        for (int i = 0; i < oldImageFileNames.Count; i++)
        {
            ProductImageFileNameInfo imageFileNameInfo = oldImageFileNames[i];

            if (imageFileNameInfo.FileName is null
                || !imageFileNameInfo.FileName.StartsWith($"{productId}-")
                || imageFileNameInfo.FileName.StartsWith($"{productId}-{imageFileNameInfo.ImageNumber}")) continue;

            string? newFileNameWithoutExtension = GetTemporaryFileNameWithoutExtension(productId, imageFileNameInfo.ImageNumber);

            string? fileNameExtension = Path.GetExtension(imageFileNameInfo.FileName);

            if (newFileNameWithoutExtension is null
                || fileNameExtension is null) return new UnexpectedFailureResult();

            string fullNewFileName = $"{newFileNameWithoutExtension}.{fileNameExtension}";

            ServiceProductImageFileNameInfoByImageNumberUpdateRequest fileNameInfoUpdateRequest = new()
            {
                ProductId = productId,
                ImageNumber = imageFileNameInfo.ImageNumber,
                ShouldUpdateDisplayOrder = false,
                FileName = fullNewFileName,
                Active = imageFileNameInfo.Active
            };

            OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoUpdateResult
                = _productImageFileNameInfoService.UpdateByImageNumber(fileNameInfoUpdateRequest);

            if (fileNameInfoUpdateResult.Value is not Success)
            {
                return fileNameInfoUpdateResult.Map<Success, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>();
            }

            string oldFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFileNameInfo.FileName);

            AllowedImageFileType? allowedImageFileType = GetAllowedImageFileTypeFromFileExtension(fileNameExtension);

            if (allowedImageFileType is null) return new UnexpectedFailureResult();

            OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> renameFileResult = _productImageFileManagementService.RenameImageFile(
                oldFileNameWithoutExtension, newFileNameWithoutExtension, allowedImageFileType);

            if (renameFileResult.Value is not Success)
            {
                return renameFileResult.Match<OneOf<Success, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult>>(
                    success => success,
                    fileDoesntExistResult => fileDoesntExistResult,
                    fileAlreadyExistsResult => new UnexpectedFailureResult());
            }
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> UpsertImageFileAndImageFileNameInfoAsync(
        int productId,
        ImageFileAndFileNameInfoUpsertRequest imageFileUpsertRequest)
    {
        string? fileExtension = GetImageFileExtensionFromContentType(imageFileUpsertRequest.ImageContentType);

        AllowedImageFileType? allowedImageFileType = GetAllowedImageFileTypeFromContentType(imageFileUpsertRequest.ImageContentType);

        if (fileExtension is null
            || allowedImageFileType is null)
        {
            return GetInvalidContentTypeValidationResult(imageFileUpsertRequest.ImageContentType, nameof(ImageFileAndFileNameInfoUpsertRequest.ImageContentType));
        }

        string? newFileNameWithoutExtension = imageFileUpsertRequest.CustomFileNameWithoutExtension ?? imageFileUpsertRequest.RelatedImageId?.ToString();

        if (imageFileUpsertRequest.OldFileName is null)
        {
            newFileNameWithoutExtension ??= GetFileNameFromMaxImageNumber(productId);

            if (newFileNameWithoutExtension is null) return new UnexpectedFailureResult();

            string fullNewFileName = $"{newFileNameWithoutExtension}.{fileExtension}";

            OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInsertResult
                = InsertImageFileNameInfo(productId, fullNewFileName, imageFileUpsertRequest.DisplayOrder, imageFileUpsertRequest.Active);

            if (imageFileNameInsertResult.Value is not Success)
            {
                return imageFileNameInsertResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
            }

            OneOf<Success, DirectoryNotFoundResult> addOrUpdateImageResult = await _productImageFileManagementService.AddOrUpdateImageAsync(
                newFileNameWithoutExtension, imageFileUpsertRequest.ImageData, allowedImageFileType);

            return addOrUpdateImageResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
        }

        ProductImageFileNameInfo? imageFileNameInfoForFile = _productImageFileNameInfoService.GetByFileName(imageFileUpsertRequest.OldFileName);

        if (imageFileNameInfoForFile?.ProductId != productId) return GetOldFileNameForOtherProductValidationResult(imageFileUpsertRequest.OldFileName);

        if (imageFileNameInfoForFile is not null)
        {
            newFileNameWithoutExtension ??= GetTemporaryFileNameWithoutExtension(productId, imageFileNameInfoForFile.ImageNumber);

            if (newFileNameWithoutExtension is null) return new UnexpectedFailureResult();

            string fullNewFileName = $"{newFileNameWithoutExtension}.{fileExtension}";

            OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoUpdateResult
                = UpdateFileNameInfoBasedOnImageFileUpsertRequest(productId, imageFileUpsertRequest, imageFileNameInfoForFile, fullNewFileName);
            
            if (fileNameInfoUpdateResult.Value is not Success)
            {
                return fileNameInfoUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
            }
        }
        else
        {
            newFileNameWithoutExtension ??= GetFileNameFromMaxImageNumber(productId);

            if (newFileNameWithoutExtension is null) return new UnexpectedFailureResult();

            string fullNewFileName = $"{newFileNameWithoutExtension}.{fileExtension}";

            OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInsertResult
                = InsertImageFileNameInfo(productId, fullNewFileName, imageFileUpsertRequest.DisplayOrder, imageFileUpsertRequest.Active);

            if (imageFileNameInsertResult.Value is not Success)
            {
                return imageFileNameInsertResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
            }
        }

        string oldFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFileUpsertRequest.OldFileName);

        return await AddOrUpdateAndRenameImageFileAsync(
            oldFileNameWithoutExtension,
            newFileNameWithoutExtension,
            imageFileUpsertRequest.ImageData,
            allowedImageFileType);
    }

    private string? GetFileNameFromMaxImageNumber(int productId)
    {
        int maxImageNumberOfProduct = _productImageFileNameInfoService.GetHighestImageNumber(productId) ?? 0;

        return GetTemporaryFileNameWithoutExtension(productId, maxImageNumberOfProduct + 1);
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertImageFileNameInfo(
        int productId,
        string fileName,
        int? displayOrder,
        bool active)
    {
        ServiceProductImageFileNameInfoCreateRequest imageFileNameInfoCreateRequest = new()
        {
            ProductId = productId,
            FileName = fileName,
            DisplayOrder = displayOrder,
            Active = active,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInsertResult
            = _productImageFileNameInfoService.Insert(imageFileNameInfoCreateRequest);

        return imageFileNameInsertResult;
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateFileNameInfoBasedOnImageFileUpsertRequest(
        int productId,
        ImageFileAndFileNameInfoUpsertRequest imageFileUpsertRequest,
        ProductImageFileNameInfo oldImageFileName,
        string fullNewFileName)
    {
        ServiceProductImageFileNameInfoByImageNumberUpdateRequest imageFileNameInfoUpdateRequest = new()
        {
            ProductId = productId,
            ImageNumber = oldImageFileName.ImageNumber,
            NewDisplayOrder = imageFileUpsertRequest.DisplayOrder,
            ShouldUpdateDisplayOrder = (imageFileUpsertRequest.DisplayOrder != oldImageFileName.DisplayOrder),
            FileName = fullNewFileName,
            Active = imageFileUpsertRequest.Active,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoUpdateResult
            = _productImageFileNameInfoService.UpdateByImageNumber(imageFileNameInfoUpdateRequest);

        return fileNameInfoUpdateResult;
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> AddOrUpdateAndRenameImageFileAsync(
        string oldFileNameWithoutExtension,
        string newFileNameWithoutExtension,
        byte[] imageData,
        AllowedImageFileType allowedImageFileType)
    {
        OneOf<Success, DirectoryNotFoundResult> addOrUpdateImageResult = await _productImageFileManagementService.AddOrUpdateImageAsync(
            oldFileNameWithoutExtension, imageData, allowedImageFileType);

        if (addOrUpdateImageResult.Value is not Success)
        {
            return addOrUpdateImageResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
        }

        if (oldFileNameWithoutExtension != newFileNameWithoutExtension)
        {
            OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> renameFileResult = _productImageFileManagementService.RenameImageFile(
                oldFileNameWithoutExtension, newFileNameWithoutExtension, allowedImageFileType);

            if (renameFileResult.Value is not Success)
            {
                return renameFileResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>(
                    success => success,
                    fileDoesntExistResult => fileDoesntExistResult,
                    fileAlreadyExistsResult => new UnexpectedFailureResult());
            }
        }

        return new Success();
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>>
        UpdateProductAndUpdateImagesOnlyInDirectoryAsync(ProductUpdateWithoutImagesInDatabaseRequest productUpdateWithoutImagesInDBRequest)
    {
        ValidationResult validationResult = ValidateDefault(_updateWithoutImagesInDatabaseRequestValidator, productUpdateWithoutImagesInDBRequest);

        if (!validationResult.IsValid) return validationResult;

        int productId = productUpdateWithoutImagesInDBRequest.Id;

        Product? oldProduct = GetProductFull(productId);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        ValidationResult updateReadyValidationResult = ValidateWhetherProductIsReadyForUpdate(oldProduct, productWorkStatuses);

        if (!updateReadyValidationResult.IsValid) return updateReadyValidationResult;

        ProductUpdateRequest productUpdateRequest = _productMapper.MapWithoutIncludingImagesAndProperties(productUpdateWithoutImagesInDBRequest);

        OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> saveProductResult
            = await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                () => UpdateProductWithoutUpdatingImagesInDBInternalAsync(productUpdateWithoutImagesInDBRequest, productUpdateRequest, oldProduct!),
                result => result.IsT0);

        return saveProductResult;
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>> UpdateProductWithoutUpdatingImagesInDBInternalAsync(
        ProductUpdateWithoutImagesInDatabaseRequest productUpdateWithoutImagesInDbRequest,
        ProductUpdateRequest productUpdateRequest,
        Product oldProduct)
    {
        if (productUpdateWithoutImagesInDbRequest.PropertyUpsertRequests is not null)
        {
            List<ProductCharacteristic>? productCharacteristics = null;

            List<int> productCharacteristicIds = productUpdateWithoutImagesInDbRequest.PropertyUpsertRequests.Select(property => property.ProductCharacteristicId)
                .ToList();

            productCharacteristics = _productCharacteristicService.GetSelectionByCharacteristicIds(productCharacteristicIds)
                .ToList();

            ValidationResult propertiesValidationResult = ValidateProperties(
                productUpdateWithoutImagesInDbRequest.PropertyUpsertRequests, productCharacteristicIds, productCharacteristics, productUpdateWithoutImagesInDbRequest.CategoryId);

            if (!propertiesValidationResult.IsValid) return propertiesValidationResult;

            OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyUpdateResult
                = UpdateProperties(productUpdateWithoutImagesInDbRequest.PropertyUpsertRequests, oldProduct, oldProduct.Id, productUpdateRequest, productCharacteristics);

            bool isPropertyUpdateSuccessful = propertyUpdateResult.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false);

            if (!isPropertyUpdateSuccessful)
            {
                return propertyUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
            }
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = UpdateSimple(productUpdateRequest);

        bool isProductUpdateSuccessful = productUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false);

        if (!isProductUpdateSuccessful)
        {
            return productUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult>();
        }

        if (productUpdateWithoutImagesInDbRequest.ImageFileAndFileNameInfoUpsertRequests is null) return new Success();

        foreach (ImageFileAndFileNameInfoUpsertRequest imageFileUpsertRequest in productUpdateWithoutImagesInDbRequest.ImageFileAndFileNameInfoUpsertRequests)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> upsertImageFileResult
                = await UpsertImageFileAndImageFileNameInfoAsync(productUpdateWithoutImagesInDbRequest.Id, imageFileUpsertRequest);

            if (upsertImageFileResult.Value is not Success) return upsertImageFileResult;
        }

        return new Success();
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateProductFullAsync(ProductFullUpdateRequest productFullUpdateRequest)
    {
        ValidationResult validationResult = ValidateDefault(_fullUpdateRequestValidator, productFullUpdateRequest);

        if (!validationResult.IsValid) return validationResult;

        try
        {
            return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                () => UpdateProductFullInternalAsync(productFullUpdateRequest),
                result => result.Match(
                    success => true,
                    validationResult => false,
                    unexpectedFailureResult => false));
        }
        catch (TransactionException)
        {
            return new UnexpectedFailureResult();
        }
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateProductFullInternalAsync(
        ProductFullUpdateRequest productFullUpdateRequest)
    {
        int productId = productFullUpdateRequest.Id;

        Product? oldProduct = GetProductFull(productFullUpdateRequest.Id);

        if (oldProduct is null)
        {
            List<ValidationFailure> validationFailures = new()
            {
                new(nameof(ProductFullUpdateRequest.Id), "Cannot update product that does not exist.")
            };

            return new ValidationResult(validationFailures);
        }

        List<ProductCharacteristic>? productCharacteristics = null;

        if (productFullUpdateRequest.PropertyUpsertRequests is not null)
        {
            List<int> productCharacteristicIds = productFullUpdateRequest.PropertyUpsertRequests.Select(property => property.ProductCharacteristicId)
                .ToList();

            productCharacteristics = _productCharacteristicService.GetSelectionByCharacteristicIds(productCharacteristicIds)
                .ToList();

            ValidationResult propertiesValidationResult = ValidateProperties(
                productFullUpdateRequest.PropertyUpsertRequests, productCharacteristicIds, productCharacteristics, productFullUpdateRequest.CategoryId);

            if (!propertiesValidationResult.IsValid) return propertiesValidationResult;
        }

        if (productFullUpdateRequest.ImageAndFileNameUpsertRequests is not null
            && productFullUpdateRequest.ImageAndFileNameUpsertRequests.Count > 0)
        {
            productFullUpdateRequest.ImageAndFileNameUpsertRequests = OrderImageAndImageFileNameUpsertRequests(productFullUpdateRequest.ImageAndFileNameUpsertRequests);
        }

        ProductUpdateRequest productUpdateRequest = _productMapper.MapWithoutIncludingImagesAndProperties(productFullUpdateRequest);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertiesUpdateResult = UpdateProperties(
            productFullUpdateRequest.PropertyUpsertRequests, oldProduct, productId, productUpdateRequest, productCharacteristics);

        if (!propertiesUpdateResult.IsT0)
        {
            return propertiesUpdateResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imagesUpdateResult = UpdateImages(
            productFullUpdateRequest, oldProduct, productId, productUpdateRequest);

        if (!imagesUpdateResult.IsT0)
        {
            return imagesUpdateResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNamesUpdateResult
            = await UpdateImageFileNamesAsync(productFullUpdateRequest, oldProduct, productId);

        if (!imageFileNamesUpdateResult.IsT0)
        {
            return imageFileNamesUpdateResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = UpdateSimple(productUpdateRequest);

        if (productUpdateResult.Value is not Success)
        {
            return productUpdateResult;
        }

        Product? updatedProduct = GetProductFull(productId);

        if (updatedProduct is null) return new UnexpectedFailureResult();

        return UpdateImagesHtmlData(updatedProduct);
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateProperties(
        List<LocalProductPropertyUpsertRequest>? propertyUpdateRequests,
        Product oldFirstProduct,
        int productId,
        ProductUpdateRequest productUpdateRequestToAddPropsTo,
        List<ProductCharacteristic>? productCharacteristics)
    {
        if (propertyUpdateRequests is not null
            && propertyUpdateRequests.Count > 0)
        {
            foreach (LocalProductPropertyUpsertRequest productPropUpsertRequest in propertyUpdateRequests)
            {
                ProductProperty? oldProductPropForSameCharacteristic = oldFirstProduct.Properties
                    .Find(prop => prop.ProductCharacteristicId == productPropUpsertRequest.ProductCharacteristicId);

                if (oldProductPropForSameCharacteristic is null)
                {
                    ProductPropertyByCharacteristicIdCreateRequest propCreateRequest = new()
                    {
                        ProductId = productId,
                        ProductCharacteristicId = productPropUpsertRequest.ProductCharacteristicId,
                        CustomDisplayOrder = productPropUpsertRequest.CustomDisplayOrder,
                        Value = productPropUpsertRequest.Value,
                        XmlPlacement = productPropUpsertRequest.XmlPlacement,
                    };

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult
                        = _productPropertyService.InsertWithCharacteristicId(propCreateRequest);

                    bool isPropertyInsertSuccessful = propertyInsertResult.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isPropertyInsertSuccessful)
                    {
                        return propertyInsertResult;
                    }

                    continue;
                }

                ProductCharacteristic? productCharacteristic = productCharacteristics?.FirstOrDefault(
                    characteristic => characteristic.Id == productPropUpsertRequest.ProductCharacteristicId);

                if (productCharacteristic is null) return new UnexpectedFailureResult();

                int? newDisplayOrder = (productPropUpsertRequest.CustomDisplayOrder ?? productCharacteristic.DisplayOrder);

                if (oldProductPropForSameCharacteristic.Value == productPropUpsertRequest.Value
                    && oldProductPropForSameCharacteristic.XmlPlacement == productPropUpsertRequest.XmlPlacement
                    && oldProductPropForSameCharacteristic.DisplayOrder == newDisplayOrder)
                {
                    oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);

                    continue;
                }

                CurrentProductPropertyUpdateRequest propUpdateRequest = new()
                {
                    ProductCharacteristicId = productPropUpsertRequest.ProductCharacteristicId,
                    CustomDisplayOrder = newDisplayOrder,
                    Value = productPropUpsertRequest.Value,
                    XmlPlacement = productPropUpsertRequest.XmlPlacement,
                };

                productUpdateRequestToAddPropsTo.Properties.Add(propUpdateRequest);

                oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);
            }
        }

        foreach (ProductProperty oldPropertyToDelete in oldFirstProduct.Properties)
        {
            if (oldPropertyToDelete.ProductCharacteristicId is null) return new UnexpectedFailureResult();

            bool imageFileNameDeleteSuccess = _productPropertyService.Delete(
                productId, oldPropertyToDelete.ProductCharacteristicId.Value);

            if (!imageFileNameDeleteSuccess) return new UnexpectedFailureResult();
        }

        return new Success();
    }

    private static ValidationResult ValidateProperties(
        List<LocalProductPropertyUpsertRequest> propertyUpsertRequests,
        List<int> matchingCharacteristicIds,
        IEnumerable<ProductCharacteristic> matchingCharacteristics,
        int? categoryIdOfProduct)
    {
        ValidationResult validationResult = new();

        if (categoryIdOfProduct is null)
        {
            if (propertyUpsertRequests.Count > 0)
            {
                validationResult.Errors.Add(new(
                    $"{nameof(ProductFullUpdateRequest.PropertyUpsertRequests)}",
                    "Product without category cannot have characteristics"));
            }

            return validationResult;
        }

        for (int i = 0; i < matchingCharacteristicIds.Count; i++)
        {
            int characteristicId = matchingCharacteristicIds[i];

            ProductCharacteristic? correspondingCharacteristic = matchingCharacteristics
                .FirstOrDefault(productCharacteristic => productCharacteristic.Id == characteristicId);

            if (correspondingCharacteristic is null)
            {
                validationResult.Errors.Add(new(
                    $"{nameof(ProductFullUpdateRequest.PropertyUpsertRequests)}[{i}].{nameof(LocalProductPropertyUpsertRequest.ProductCharacteristicId)}",
                    "Id does not correspond to any known characteristic id."));
            }
            else if (correspondingCharacteristic.CategoryId != categoryIdOfProduct)
            {
                validationResult.Errors.Add(new(
                    $"{nameof(ProductFullUpdateRequest.PropertyUpsertRequests)}[{i}].{nameof(LocalProductPropertyUpsertRequest.ProductCharacteristicId)}",
                    "Id corresponds to a characteristic of a different category."));
            }
        }

        return validationResult;
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateImages(
        ProductFullUpdateRequest productFullUpdateRequest,
        Product oldProduct,
        int productId,
        ProductUpdateRequest productUpdateRequestToAddImagesTo)
    //List<(ProductImage productImage, int displayOrder)> ordersForImagesInProduct)
    {
        if (productFullUpdateRequest.ImageAndFileNameUpsertRequests is not null)
        {
            ImageAndImageFileNameUpsertRequest? productFirstImageUpsertRequest = null;

            for (int i = 0; i < productFullUpdateRequest.ImageAndFileNameUpsertRequests.Count; i++)
            {
                ImageAndImageFileNameUpsertRequest imageAndFileNameInfoUpsertRequest = productFullUpdateRequest.ImageAndFileNameUpsertRequests[i];

                ProductImageUpsertRequest? image = imageAndFileNameInfoUpsertRequest.ProductImageUpsertRequest;

                if (image is null) continue;

                ProductImage? imageInOldProduct = oldProduct.Images?.Find(
                    img => img.Id == image.OriginalImageId);

                if (imageInOldProduct is null)
                {
                    ServiceProductImageCreateRequest productImageCreateRequest = new()
                    {
                        ProductId = productId,
                        ImageData = imageAndFileNameInfoUpsertRequest.ImageData,
                        ImageContentType = imageAndFileNameInfoUpsertRequest.ImageContentType,
                        HtmlData = image.HtmlData,
                    };

                    OneOf<int, ValidationResult, UnexpectedFailureResult> imageInsertResult
                        = _productImageService.InsertInAllImages(productImageCreateRequest);

                    int imageId = -1;

                    bool isImageInsertSuccessful = imageInsertResult.Match(
                        id =>
                        {
                            imageId = id;

                            return true;
                        },
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isImageInsertSuccessful)
                    {
                        return imageInsertResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                            id => new Success(),
                            validationResult => validationResult,
                            unexpectedFailureResult => unexpectedFailureResult);
                    }

                    image.OriginalImageId = imageId;

                    if (productFirstImageUpsertRequest is null
                        && imageAndFileNameInfoUpsertRequest.ProductImageUpsertRequest is not null)
                    {
                        productFirstImageUpsertRequest = imageAndFileNameInfoUpsertRequest;
                    }

                    //int imageInOrderedListIndex = ordersForImagesInProduct.FindIndex(
                    //    x => x.productImage == image);

                    //if (imageInOrderedListIndex < 0) return new UnexpectedFailureResult();

                    //if (ordersForImagesInProduct[imageInOrderedListIndex].displayOrder == 1)
                    //{
                    //    productFirstImage = image;
                    //}

                    //(ProductImage productImage, int displayOrder) = ordersForImagesInProduct[imageInOrderedListIndex];

                    //productImage.Id = imageId;

                    //ordersForImagesInProduct[imageInOrderedListIndex] = new(productImage, displayOrder);

                    continue;
                }

                if (productFirstImageUpsertRequest is null
                    && imageAndFileNameInfoUpsertRequest.ProductImageUpsertRequest is not null)
                {
                    productFirstImageUpsertRequest = imageAndFileNameInfoUpsertRequest;
                }

                if (CompareByteArrays(imageAndFileNameInfoUpsertRequest.ImageData, imageInOldProduct.ImageData)
                    && imageAndFileNameInfoUpsertRequest.ImageContentType == imageInOldProduct.ImageContentType
                    && image.HtmlData == imageInOldProduct.HtmlData)
                {
                    oldProduct.Images?.Remove(imageInOldProduct);

                    continue;
                }

                CurrentProductImageUpdateRequest productImageUpdateRequest = new()
                {
                    ImageData = imageAndFileNameInfoUpsertRequest.ImageData,
                    ImageContentType = imageAndFileNameInfoUpsertRequest.ImageContentType,
                    HtmlData = image.HtmlData,
                    DateModified = DateTime.Now,
                };

                productUpdateRequestToAddImagesTo.Images ??= new();

                productUpdateRequestToAddImagesTo.Images.Add(productImageUpdateRequest);

                oldProduct.Images?.Remove(imageInOldProduct);
            }

            ProductImage? oldProductFirstImage = _productImageService.GetFirstImageForProduct(productId);

            if (productFirstImageUpsertRequest is not null)
            {
                if (oldProductFirstImage is null)
                {
                    OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult = _productImageService.InsertInFirstImages(new()
                    {
                        ProductId = productId,
                        ImageData = productFirstImageUpsertRequest.ImageData,
                        ImageContentType = productFirstImageUpsertRequest.ImageContentType,
                        HtmlData = productFirstImageUpsertRequest.ProductImageUpsertRequest!.HtmlData,
                    });

                    bool isFirstImageInsertSuccessful = insertFirstImageResult.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isFirstImageInsertSuccessful) return insertFirstImageResult;
                }
                else
                {
                    OneOf<Success, ValidationResult, UnexpectedFailureResult> updateFirstImageResult = _productImageService.UpdateInFirstImages(new()
                    {
                        ProductId = productId,
                        ImageData = productFirstImageUpsertRequest.ImageData,
                        ImageContentType = productFirstImageUpsertRequest.ImageContentType,
                        HtmlData = productFirstImageUpsertRequest.ProductImageUpsertRequest!.HtmlData,
                    });

                    bool isFirstImageInsertSuccessful = updateFirstImageResult.Match(
                        success => true,
                        validationResult => false,
                        unexpectedFailureResult => false);

                    if (!isFirstImageInsertSuccessful) return updateFirstImageResult;
                }
            }
            else if (oldProductFirstImage is not null)
            {
                bool isFirstImageDeleted = _productImageService.DeleteInFirstImagesByProductId(productId);

                if (!isFirstImageDeleted) return new UnexpectedFailureResult();
            }
        }

        if (oldProduct.Images is not null
            && oldProduct.Images.Count > 0)
        {
            foreach (ProductImage oldImageToBeRemoved in oldProduct.Images)
            {
                bool imageDeleteResult = _productImageService.DeleteInAllImagesById(oldImageToBeRemoved.Id);

                if (!imageDeleteResult) return new UnexpectedFailureResult();
            }
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> UpdateImageFileNamesAsync(
        ProductFullUpdateRequest productFullUpdateRequest,
        Product oldProduct,
        int productId)
    {
        List<ServiceProductImageFileNameInfoCreateRequest> imageFileNameInfoCreateRequests = new();

        if (oldProduct.ImageFileNames is not null
            && oldProduct.ImageFileNames.Count > 0)
        {
            if (productFullUpdateRequest.ImageAndFileNameUpsertRequests is null
                || productFullUpdateRequest.ImageAndFileNameUpsertRequests.Count <= 0)
            {
                foreach (ProductImageFileNameInfo oldProductImageFileName in oldProduct.ImageFileNames)
                {
                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndImageNumber(
                        productId, oldProductImageFileName.ImageNumber);

                    if (!imageFileNameDeleteResult) return new UnexpectedFailureResult();

                    if (oldProductImageFileName.FileName is not null)
                    {
                        _productImageFileManagementService.DeleteFile(oldProductImageFileName.FileName);
                    }

                    ChangeAllFileNameInfosToMatchChangesInDelete(oldProduct.ImageFileNames, oldProductImageFileName);
                }
            }
            else
            {
                for (int i = 0; i < oldProduct.ImageFileNames.Count; i++)
                {
                    ProductImageFileNameInfo oldProductImageFileName = oldProduct.ImageFileNames[i];

                    ImageAndImageFileNameUpsertRequest? imageAndFileNameUpsertRequest = productFullUpdateRequest.ImageAndFileNameUpsertRequests.Find(
                        upsertRequest => oldProductImageFileName.ImageNumber == upsertRequest.ProductImageFileNameInfoUpsertRequest?.OriginalImageNumber);

                    if (imageAndFileNameUpsertRequest is not null) continue;

                    bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndImageNumber(
                        productId, oldProductImageFileName.ImageNumber);

                    if (!imageFileNameDeleteResult) return new UnexpectedFailureResult();

                    if (oldProductImageFileName.FileName is not null)
                    {
                        _productImageFileManagementService.DeleteFile(oldProductImageFileName.FileName);
                    }

                    oldProduct.ImageFileNames.RemoveAt(i);

                    i--;

                    ChangeAllFileNameInfosToMatchChangesInDelete(oldProduct.ImageFileNames, oldProductImageFileName);
                    ChangeAllFileNameInfosToMatchChangesInDelete(productFullUpdateRequest.ImageAndFileNameUpsertRequests, oldProductImageFileName);
                }
            }
        }

        if (productFullUpdateRequest.ImageAndFileNameUpsertRequests is not null
            && productFullUpdateRequest.ImageAndFileNameUpsertRequests.Count > 0)
        {
            int newImageFileNamesCount = 0;

            foreach (ImageAndImageFileNameUpsertRequest imageAndFileNameUpsertRequest in productFullUpdateRequest.ImageAndFileNameUpsertRequests)
            {
                ProductImageUpsertRequest? relatedImageUpsertRequest = imageAndFileNameUpsertRequest.ProductImageUpsertRequest;
                ProductImageFileNameInfoUpsertRequest? imageFileNameInfoUpsertRequest = imageAndFileNameUpsertRequest.ProductImageFileNameInfoUpsertRequest;

                if (imageFileNameInfoUpsertRequest is null) continue;

                ProductImageFileNameInfo? imageFileNameOfOldProduct = oldProduct.ImageFileNames?.Find(
                    imgFileNameInfo => imgFileNameInfo.ImageNumber == imageFileNameInfoUpsertRequest.OriginalImageNumber);

                string? newFileName = null;

                if (imageFileNameOfOldProduct is null)
                {
                    int highestImageNumber = productFullUpdateRequest.ImageAndFileNameUpsertRequests
                        .Max(x => x.ProductImageFileNameInfoUpsertRequest?.OriginalImageNumber) ?? newImageFileNamesCount;

                    OneOf<string, ValidationResult> localGetFileNameResult
                        = GetFileNameFromImageData(imageAndFileNameUpsertRequest, productId, highestImageNumber + 1);

                    if (localGetFileNameResult.IsT1) return localGetFileNameResult.AsT1;

                    newFileName = localGetFileNameResult.AsT0;

                    ServiceProductImageFileNameInfoCreateRequest imageFileNameCreateRequest = new()
                    {
                        ProductId = productFullUpdateRequest.Id,
                        DisplayOrder = imageFileNameInfoUpsertRequest.NewDisplayOrder,
                        Active = imageFileNameInfoUpsertRequest.Active,
                        FileName = newFileName,
                    };

                    imageFileNameInfoCreateRequests.Add(imageFileNameCreateRequest);

                    OneOf<Success, False, DirectoryNotFoundResult> addFileResult
                        = await AddOrUpdateFileIfDataAvailableAsync(imageAndFileNameUpsertRequest, newFileName);

                    if (addFileResult.Value is DirectoryNotFoundResult) return new UnexpectedFailureResult();

                    newImageFileNamesCount++;

                    continue;
                }

                OneOf<string, ValidationResult> getFileNameResult
                    = GetFileNameFromImageData(imageAndFileNameUpsertRequest, productId, imageFileNameOfOldProduct.ImageNumber);

                if (getFileNameResult.IsT1) return getFileNameResult.AsT1;

                newFileName = getFileNameResult.AsT0;

                if (imageFileNameOfOldProduct.FileName is not null)
                {
                    string oldFileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFileNameOfOldProduct.FileName);
                    string newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> renameResult
                        = RenameFileIfFileNameIsDifferent(imageAndFileNameUpsertRequest, oldFileNameWithoutExtension, newFileNameWithoutExtension);
                }

                int? displayOrder = null;

                if (imageFileNameInfoUpsertRequest.NewDisplayOrder is not null)
                {
                    displayOrder = imageFileNameInfoUpsertRequest.NewDisplayOrder.Value - newImageFileNamesCount;
                }

                bool isFileNameInfoSameAsOldOne = newFileName == imageFileNameOfOldProduct.FileName
                    && displayOrder == imageFileNameOfOldProduct.DisplayOrder
                    && imageFileNameInfoUpsertRequest.Active == imageFileNameOfOldProduct.Active;

                if (isFileNameInfoSameAsOldOne) continue;

                ServiceProductImageFileNameInfoByImageNumberUpdateRequest imageFileNameUpdateRequest = new()
                {
                    ProductId = productId,
                    ImageNumber = imageFileNameInfoUpsertRequest.OriginalImageNumber!.Value,
                    ShouldUpdateDisplayOrder = displayOrder != imageFileNameOfOldProduct.DisplayOrder,
                    NewDisplayOrder = displayOrder,
                    FileName = newFileName,
                    Active = imageFileNameInfoUpsertRequest.Active,
                };

                OneOf<Success, ValidationResult, UnexpectedFailureResult> productImageFileNameUpdateResult
                    = _productImageFileNameInfoService.UpdateByImageNumber(imageFileNameUpdateRequest);

                bool isImageFileNameUpdateSuccessful = productImageFileNameUpdateResult.Match(
                    id => true,
                    validationResult => false,
                    unexpectedFailureResult => false);

                if (!isImageFileNameUpdateSuccessful)
                {
                    return productImageFileNameUpdateResult;
                }
            }
        }

        IEnumerable<ServiceProductImageFileNameInfoCreateRequest> orderedImageFileNameInfoCreateRequests = imageFileNameInfoCreateRequests
            .OrderBy(imageFileName => imageFileName.DisplayOrder);

        foreach (ServiceProductImageFileNameInfoCreateRequest fileNameInfoCreateRequest in orderedImageFileNameInfoCreateRequests)
        {
            OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(fileNameInfoCreateRequest);

            bool isImageFileNameInsertSuccessful = fileNameInfoInsertResult.Match(
                    id => true,
                    validationResult => false,
                    unexpectedFailureResult => false);

            if (!isImageFileNameInsertSuccessful)
            {
                return fileNameInfoInsertResult;
            }
        }

        return new Success();
    }

    private static OneOf<string, ValidationResult> GetFileNameFromImageData(
        ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest, int productId, int newImageNumber)
    {
        string? customFileNameWithoutExtension = imageAndImageFileNameUpsertRequest.ProductImageFileNameInfoUpsertRequest?.CustomFileNameWithoutExtension;

        if (customFileNameWithoutExtension is not null)
        {
            string? fileExtension = GetImageFileExtensionFromContentType(imageAndImageFileNameUpsertRequest.ImageContentType);

            if (fileExtension is null)
            {
                return GetInvalidContentTypeValidationResult(imageAndImageFileNameUpsertRequest.ImageContentType, nameof(ImageAndImageFileNameUpsertRequest.ImageContentType));
            }

            return $"{customFileNameWithoutExtension}.{fileExtension}";
        }

        string? fileName;

        ProductImageUpsertRequest? relatedImageUpsertRequest = imageAndImageFileNameUpsertRequest.ProductImageUpsertRequest;

        if (relatedImageUpsertRequest?.OriginalImageId is null
            || relatedImageUpsertRequest.OriginalImageId <= 0)
        {
            string? temporaryIdFromContentType = GetTemporaryIdFromFileNameInfoAndContentType(
                productId, newImageNumber, imageAndImageFileNameUpsertRequest.ImageContentType);

            if (temporaryIdFromContentType is null)
            {
                return GetInvalidContentTypeValidationResult(imageAndImageFileNameUpsertRequest.ImageContentType, nameof(ImageAndImageFileNameUpsertRequest.ImageContentType));
            }

            fileName = temporaryIdFromContentType;
        }
        else
        {
            fileName = GetImageFileNameFromImageData(
                relatedImageUpsertRequest.OriginalImageId.Value, imageAndImageFileNameUpsertRequest.ImageContentType)!;
        }

        return fileName;
    }

    private async Task<OneOf<Success, False, DirectoryNotFoundResult>> AddOrUpdateFileIfDataAvailableAsync(
        ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest, string newFileName)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);
        byte[]? imageData = imageAndImageFileNameUpsertRequest.ImageData;

        if (imageData is null) return new False();

        AllowedImageFileType? allowedImageFileType = GetAllowedImageFileTypeFromContentType(imageAndImageFileNameUpsertRequest.ImageContentType);

        if (allowedImageFileType is null) return new False();

        OneOf<Success, DirectoryNotFoundResult> addFileResult
            = await _productImageFileManagementService.AddOrUpdateImageAsync(fileNameWithoutExtension, imageData, allowedImageFileType);

        return addFileResult.Map<Success, False, DirectoryNotFoundResult>();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> RenameFileIfFileNameIsDifferent(
        ImageAndImageFileNameUpsertRequest imageAndFileNameUpsertRequest, string oldFileNameWithoutExtension, string newFileNameWithoutExtension)
    {
        string? fileExtension = GetImageFileExtensionFromContentType(imageAndFileNameUpsertRequest.ImageContentType);

        if (fileExtension is null)
        {
            return GetInvalidContentTypeValidationResult(imageAndFileNameUpsertRequest.ImageContentType, nameof(ImageAndImageFileNameUpsertRequest.ImageContentType));
        }

        if (oldFileNameWithoutExtension != newFileNameWithoutExtension)
        {
            AllowedImageFileType? allowedImageFileTypeFromFileName = GetAllowedImageFileTypeFromFileExtension(fileExtension);

            if (allowedImageFileTypeFromFileName is null)
            {
                return GetInvalidContentTypeValidationResult(imageAndFileNameUpsertRequest.ImageContentType, nameof(ImageAndImageFileNameUpsertRequest.ImageContentType));
            }

            OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> renameResult = _productImageFileManagementService.RenameImageFile(
                oldFileNameWithoutExtension, newFileNameWithoutExtension, allowedImageFileTypeFromFileName);

            if (!renameResult.IsT0)
            {
                return renameResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                    success => success,
                    fileDoesntExistResult => new Success(),
                    fileAlreadyExistsResult => new UnexpectedFailureResult());
            }
        }

        return new Success();
    }

    private static ValidationResult ValidateWhetherProductIsReadyForUpdate(Product? oldProduct, ProductWorkStatuses? productWorkStatuses)
    {
        List<ValidationFailure> validationFailures = new();

        if (oldProduct is null)
        {
            validationFailures.Add(new(nameof(ProductUpdateWithoutImagesInDatabaseRequest.Id), "Cannot update product that does not exist."));
        }

        if (productWorkStatuses is null
            || productWorkStatuses.ProductNewStatus == ProductNewStatusEnum.New
            || productWorkStatuses.ProductXmlStatus == ProductXmlStatusEnum.NotReady)
        {
            validationFailures.Add(new(nameof(ProductUpdateWithoutImagesInDatabaseRequest), "Not ready for update"));
        }

        return new ValidationResult(validationFailures);
    }

    private static ValidationResult GetInvalidContentTypeValidationResult(string contentType, string propertyName)
    {
        ValidationResult invalidContentTypeValidationResult = new();

        invalidContentTypeValidationResult.Errors.Add(new(propertyName, $"Content type is invalid or not supported: {contentType}"));

        return invalidContentTypeValidationResult;
    }

    public static ValidationResult GetInvalidOldFileNameValidationResult(string fileName)
    {
        ValidationResult oldFileNameForOtherProductValidationResult = new();

        oldFileNameForOtherProductValidationResult.Errors.Add(new(nameof(ImageFileAndFileNameInfoUpsertRequest.OldFileName),
            $"Old file name does not match any file for this product: {fileName}"));

        return oldFileNameForOtherProductValidationResult;
    }
    
    public static ValidationResult GetOldFileNameForOtherProductValidationResult(string fileName)
    {
        ValidationResult oldFileNameForOtherProductValidationResult = new();

        oldFileNameForOtherProductValidationResult.Errors.Add(new(nameof(ImageFileAndFileNameInfoUpsertRequest.OldFileName),
            $"Old file name leads to an image file name for a different product: {fileName}"));

        return oldFileNameForOtherProductValidationResult;
    }

    private static void ChangeOldFileNameInfosToMatchChangesInInsert(
        List<ProductImageFileNameInfo> imageFileNames, ProductImageFileNameInfo insertedProductImageFileName)
    {
        for (int i = 0; i < imageFileNames.Count; i++)
        {
            ProductImageFileNameInfo imageFileName = imageFileNames[i];

            if (imageFileName.DisplayOrder >= insertedProductImageFileName.DisplayOrder)
            {
                imageFileName.DisplayOrder++;
            }
        }
    }

    private static void ChangeAllFileNameInfosToMatchChangesInUpdate(
        List<ProductImageFileNameInfo> imageFileNames, int oldDisplayOrder, int newDisplayOrder)
    {
        for (int i = 0; i < imageFileNames.Count; i++)
        {
            ProductImageFileNameInfo imageFileName = imageFileNames[i];

            if (imageFileName.DisplayOrder == oldDisplayOrder)
            {
                imageFileName.DisplayOrder = newDisplayOrder;
            }
            else if (oldDisplayOrder < newDisplayOrder
                && imageFileName.DisplayOrder > oldDisplayOrder
                && imageFileName.DisplayOrder <= newDisplayOrder)
            {
                imageFileName.DisplayOrder--;
            }
            else if (oldDisplayOrder > newDisplayOrder
                && imageFileName.DisplayOrder < oldDisplayOrder
                && imageFileName.DisplayOrder >= newDisplayOrder)
            {
                imageFileName.DisplayOrder++;
            }
        }
    }

    private static void ChangeAllFileNameInfosToMatchChangesInDelete(
        List<ProductImageFileNameInfo> imageFileNames, ProductImageFileNameInfo deletedProductImageFileName)
    {
        for (int i = 0; i < imageFileNames.Count; i++)
        {
            ProductImageFileNameInfo imageFileName = imageFileNames[i];

            if (imageFileName.ImageNumber > deletedProductImageFileName.ImageNumber)
            {
                imageFileName.ImageNumber--;
            }

            if (imageFileName.DisplayOrder > deletedProductImageFileName.DisplayOrder)
            {
                imageFileName.DisplayOrder--;
            }
        }
    }

    private static void ChangeAllFileNameInfosToMatchChangesInDelete(
        List<ImageAndImageFileNameUpsertRequest> upsertRequests, ProductImageFileNameInfo deletedProductImageFileName)
    {
        for (int i = 0; i < upsertRequests.Count; i++)
        {
            ProductImageFileNameInfoUpsertRequest? imageFileName = upsertRequests[i].ProductImageFileNameInfoUpsertRequest;

            if (imageFileName is null) continue;

            if (imageFileName.OriginalImageNumber is not null
                && imageFileName.OriginalImageNumber > deletedProductImageFileName.ImageNumber)
            {
                imageFileName.OriginalImageNumber--;
            }
        }
    }

    private OneOf<Success, ValidationResult, NotSupportedFileTypeResult, FileDoesntExistResult, FileAlreadyExistsResult> TryRenameFile(
        string fileName, string newFileName)
    {
        string? oldFileExtension = GetFileExtensionWithoutDot(fileName);

        string? fileExtension = GetFileExtensionWithoutDot(newFileName);

        if (oldFileExtension is null
            || fileExtension is null)
        {
            List<ValidationFailure> validationFailures = new()
            {
                new(nameof(ProductImageFileNameInfo.FileName), "Filename is invalid")
            };

            return new ValidationResult(validationFailures);
        }

        if (oldFileExtension != fileExtension)
        {
            List<ValidationFailure> validationFailures = new()
            {
                new(nameof(ProductImageFileNameInfo.FileName), "New and old filename must have the same extension")
            };

            return new ValidationResult(validationFailures);
        }

        AllowedImageFileType? allowedImageFileTypeFromFileName = GetAllowedImageFileTypeFromFileExtension(fileExtension);

        if (allowedImageFileTypeFromFileName is null) return new NotSupportedFileTypeResult() { FileExtension = fileExtension };

        OneOf<Success, FileDoesntExistResult, FileAlreadyExistsResult> renameResult = _productImageFileManagementService.RenameImageFile(
            Path.GetFileNameWithoutExtension(fileName), Path.GetFileNameWithoutExtension(newFileName), allowedImageFileTypeFromFileName);

        return renameResult.Map<Success, ValidationResult, NotSupportedFileTypeResult, FileDoesntExistResult, FileAlreadyExistsResult>();
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateImagesHtmlData(Product product)
    {
        OneOf<string, InvalidXmlResult> getProductHtmlResult = _productHtmlService.TryGetHtmlFromProduct(product);

        return getProductHtmlResult.Match(
            htmlData =>
            {
                OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesHtmlDataResult
                    = _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductId(product.Id, htmlData);

                return updateImagesHtmlDataResult.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
                    doAnyImagesExist => new Success(),
                    validationResult => validationResult,
                    unexpectedFailureResult => unexpectedFailureResult);
            },
            invalidXmlResult => new UnexpectedFailureResult());
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateSimple(ProductUpdateRequest updateRequest)
    {
        ValidationResult? validationResult = _updateRequestValidator?.Validate(updateRequest);

        if (validationResult is not null
            && !validationResult.IsValid) return validationResult;

        if (updateRequest.Images is not null)
        {
            foreach (CurrentProductImageUpdateRequest item in updateRequest.Images)
            {
                item.DateModified = DateTime.Now;
            }
        }

        OneOf<Success, UnexpectedFailureResult> result = _productRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(int id)
    {
        if (id <= 0) return false;

        return _productRepository.Delete(id);
    }
}