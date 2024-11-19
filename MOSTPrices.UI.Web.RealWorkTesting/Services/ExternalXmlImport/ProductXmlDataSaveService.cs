using MOSTComputers.Models.FileManagement.Models;
using OneOf.Types;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.ProductImageFileManagement.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Utils.OneOf;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts.ExternalXmlImport;

using static MOSTComputers.Services.ProductImageFileManagement.Utils.ProductImageFileManagementUtils;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductMappingUtils;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductProperty;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.ExternalXmlImport;

internal sealed class ProductXmlDataSaveService : IProductXmlDataSaveService
{
    public ProductXmlDataSaveService(
        IHttpClientFactory httpClientFactory,
        IProductImageFileManagementService productImageFileManagementService,
        IProductImageService productImageService,
        IXmlImportProductPropertyService xmlImportProductPropertyService,
        IXmlImportProductImageFileNameInfoService xmlImportProductImageFileNameInfoService,
        IXmlImportProductImageService xmlImportProductImageService,
        IXmlProductToProductMappingService xmlProductToProductMappingService,
        ITransactionExecuteService transactionExecuteService)
    {
        _httpClientFactory = httpClientFactory;
        _productImageFileManagementService = productImageFileManagementService;
        _productImageService = productImageService;
        _xmlImportProductPropertyService = xmlImportProductPropertyService;
        _xmlImportProductImageFileNameInfoService = xmlImportProductImageFileNameInfoService;
        _xmlImportProductImageService = xmlImportProductImageService;
        _xmlProductToProductMappingService = xmlProductToProductMappingService;
        _transactionExecuteService = transactionExecuteService;
    }

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProductImageFileManagementService _productImageFileManagementService;
    private readonly IProductImageService _productImageService;
    private readonly IXmlImportProductPropertyService _xmlImportProductPropertyService;
    private readonly IXmlImportProductImageFileNameInfoService _xmlImportProductImageFileNameInfoService;
    private readonly IXmlImportProductImageService _xmlImportProductImageService;
    private readonly IXmlProductToProductMappingService _xmlProductToProductMappingService;
    private readonly ITransactionExecuteService _transactionExecuteService;

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertProductPropertiesFromXmlPropertiesBasedOnRelationData(
        IEnumerable<XmlProduct> xmlProducts, List<ProductCharacteristicAndExternalXmlDataRelation> characteristicAndPropertyRelations)
    {
        return _transactionExecuteService.ExecuteActionInTransactionAndCommitWithCondition(
            () => UpsertProductPropertiesFromXmlPropertiesBasedOnRelationDataInternal(xmlProducts, characteristicAndPropertyRelations),
            result => result.Match(
                success => true,
                validationResult => false,
                unexpectedFailureResult => false),
            transactionOptions: new() { Timeout = TimeSpan.FromMinutes(10) });
    }

    private OneOf<Success, ValidationResult, UnexpectedFailureResult> UpsertProductPropertiesFromXmlPropertiesBasedOnRelationDataInternal(
        IEnumerable<XmlProduct> xmlProducts, List<ProductCharacteristicAndExternalXmlDataRelation> characteristicAndPropertyRelations)
    {
        foreach (XmlProduct xmlProduct in xmlProducts)
        {
            int categoryId = xmlProduct.Category.Id;

            foreach (XmlProductProperty xmlProductProperty in xmlProduct.XmlProductProperties)
            {
                ProductCharacteristicAndExternalXmlDataRelation? matchingRelation = characteristicAndPropertyRelations
                    .FirstOrDefault(x =>
                    {
                        bool successfulParse = int.TryParse(xmlProductProperty.Order, out int xmlOrder);

                        int? actualXmlOrder = successfulParse ? xmlOrder : null;

                        return x.CategoryId == categoryId
                            && x.XmlName == xmlProductProperty.Name
                            && x.XmlDisplayOrder == actualXmlOrder;
                    });

                bool successfulParse = int.TryParse(xmlProductProperty.Order, out int xmlOrder);

                int? actualXmlOrder = successfulParse ? xmlOrder : null;

                XmlImportProductProperty? alreadyExistingProperty = GetExistingPropertyWithSameXmlData(xmlProduct.Id, xmlProductProperty.Name, actualXmlOrder);

                if (alreadyExistingProperty is null)
                {
                    XmlImportProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest = new()
                    {
                        ProductId = xmlProduct.Id,
                        ProductCharacteristicId = matchingRelation?.ProductCharacteristicId,
                        Value = xmlProductProperty.Value,
                        XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
                        XmlName = xmlProductProperty.Name,
                        XmlDisplayOrder = actualXmlOrder,
                    };

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> createPropertyResult
                        = _xmlImportProductPropertyService.InsertWithCharacteristicId(propertyCreateRequest);

                    if (createPropertyResult.Value is not Success)
                    {
                        return createPropertyResult;
                    }

                    continue;
                }

                XmlImportProductPropertyUpdateByXmlDataRequest propertyUpdateRequest = new()
                {
                    ProductId = xmlProduct.Id,
                    ProductCharacteristicId = matchingRelation?.ProductCharacteristicId,
                    Value = xmlProductProperty.Value,
                    XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
                    XmlName = xmlProductProperty.Name,
                    XmlDisplayOrder = actualXmlOrder,
                };

                OneOf<Success, ValidationResult, UnexpectedFailureResult> updatePropertyResult
                    = _xmlImportProductPropertyService.UpdateByXmlData(propertyUpdateRequest);

                if (updatePropertyResult.Value is not Success)
                {
                    return updatePropertyResult;
                }
            }
        }

        return new Success();
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>>
        UpsertImageFileNamesAndFilesFromXmlDataAsync(IEnumerable<XmlProduct> xmlProducts, bool insertFiles = false)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertImageFileNamesAndFilesFromXmlDataInternalAsync(xmlProducts, insertFiles),
            result => result.Match(
                success => true,
                validationResult => false,
                notSupportedFileTypeResult => false,
                unexpectedFailureResult => false,
                directoryNotFoundResult => false),
            transactionOptions: new() { Timeout = TimeSpan.FromMinutes(10) });
    }

    private async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>>
        UpsertImageFileNamesAndFilesFromXmlDataInternalAsync(IEnumerable<XmlProduct> xmlProducts, bool insertFiles)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient();

        foreach (XmlProduct xmlProduct in xmlProducts)
        {
            int productId = xmlProduct.Id;

            IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(productId);

            int imagesCount = productImages.Count();

            ProductImage? productFirstImage = _productImageService.GetFirstImageForProduct(productId);

            foreach (XmlShopItemImage imageFileData in xmlProduct.ShopItemImages.OrderBy(x => x.DisplayOrder ?? int.MaxValue))
            {
                int? displayOrder = (imageFileData.DisplayOrder > 0) ? imageFileData.DisplayOrder.Value : null;

                string? imageFileName = (string.IsNullOrWhiteSpace(imageFileData.PictureUrl)) ? null : Path.GetFileName(imageFileData.PictureUrl);

                XmlImportProductImageFileNameInfo? matchingFileNameInfo = GetMatchingFileNameInfo(productId, imageFileName);

                if (matchingFileNameInfo is null)
                {
                    XmlImportServiceProductImageFileNameInfoCreateRequest fileNameInfoCreateRequest = new()
                    {
                        ProductId = productId,
                        DisplayOrder = displayOrder,
                        FileName = imageFileName,
                        Active = false,
                        ImagesInImagesAllForProductCount = imagesCount,
                        IsProductFirstImageInImages = (productFirstImage is not null),
                    };

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInsertResult
                        = _xmlImportProductImageFileNameInfoService.Insert(fileNameInfoCreateRequest);

                    if (imageFileNameInsertResult.Value is not Success)
                    {
                        return imageFileNameInsertResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>();
                    }

                    if (insertFiles
                        && !string.IsNullOrWhiteSpace(imageFileName))
                    {
                        OneOf<Success, ValidationResult, NotSupportedFileTypeResult, DirectoryNotFoundResult> imageFileUpsertResult =
                            await UpsertImageFileAsync(httpClient, imageFileData, imageFileName);

                        if (!imageFileUpsertResult.IsT0)
                        {
                            return imageFileUpsertResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>();
                        }
                    }

                    continue;
                }

                XmlImportServiceProductImageFileNameInfoByImageNumberUpdateRequest fileNameInfoUpdateRequest = new()
                {
                    ProductId = productId,
                    FileName = imageFileName,
                    Active = false,
                    ImagesInImagesAllForProductCount = imagesCount,
                    IsProductFirstImageInImages = (productFirstImage is not null),
                    ImageNumber = matchingFileNameInfo.ImageNumber,
                    NewDisplayOrder = displayOrder,
                    ShouldUpdateDisplayOrder = (displayOrder != matchingFileNameInfo.DisplayOrder)
                };

                OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameUpdateResult
                    = _xmlImportProductImageFileNameInfoService.UpdateByImageNumber(fileNameInfoUpdateRequest);

                if (imageFileNameUpdateResult.Value is not Success)
                {
                    return imageFileNameUpdateResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>();
                }

                if (insertFiles
                    && !string.IsNullOrWhiteSpace(imageFileName))
                {
                    OneOf<Success, ValidationResult, NotSupportedFileTypeResult, DirectoryNotFoundResult> imageFileUpsertResult =
                           await UpsertImageFileAsync(httpClient, imageFileData, imageFileName);

                    if (!imageFileUpsertResult.IsT0)
                    {
                        return imageFileUpsertResult.Map<Success, ValidationResult, UnexpectedFailureResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>();
                    }
                }
            }
        }

        return new Success();
    }

    private async Task<OneOf<Success, ValidationResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>> UpsertImageFileAsync(
        HttpClient httpClient, XmlShopItemImage imageFileData, string imageFileName)
    {
        int imageFileNameExtensionStartIndex = imageFileName.IndexOf('.');

        if (imageFileNameExtensionStartIndex < 0)
        {
            return GetInvalidUrlValidationResult(nameof(XmlShopItemImage.PictureUrl));
        }

        string fileNameWithoutExtension = imageFileName[..imageFileNameExtensionStartIndex];

        string? fileExtension = GetFileExtensionWithoutDot(imageFileName);

        if (fileExtension is null) return GetInvalidUrlValidationResult(nameof(XmlShopItemImage.PictureUrl));

        AllowedImageFileType? imageFileType = GetAllowedImageFileTypeFromFileExtension(fileExtension);

        if (imageFileType is null) return new NotSupportedFileTypeResult() { FileExtension = fileExtension };

        byte[] imageData = await httpClient.GetByteArrayAsync(imageFileData.PictureUrl);

        OneOf<Success, DirectoryNotFoundResult> upsertImageFileResult
            = await _productImageFileManagementService.AddOrUpdateImageAsync(fileNameWithoutExtension, imageData, imageFileType);

        return upsertImageFileResult.Map<Success, ValidationResult, NotSupportedFileTypeResult, DirectoryNotFoundResult>();
    }

    private static ValidationResult GetInvalidUrlValidationResult(string propertyName)
    {
        ValidationResult invalidPathValidationResult = new();

        invalidPathValidationResult.Errors.Add(new(propertyName, "Url is invalid"));

        return invalidPathValidationResult;
    }

    public async Task<OneOf<Success, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>> UpsertTestingImagesFromXmlDataAsync(
        IEnumerable<XmlProduct> xmlProducts)
    {
        return await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => UpsertTestingImagesFromXmlDataInternalAsync(xmlProducts),
            result => result.Match(
                success => true,
                validationResult => false,
                invalidXmlResult => false,
                unexpectedFailureResult => false),
            transactionOptions: new() { Timeout = TimeSpan.FromMinutes(10) });
    }

    private async Task<OneOf<Success, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>> UpsertTestingImagesFromXmlDataInternalAsync(
        IEnumerable<XmlProduct> xmlProducts)
    {
        foreach (XmlProduct xmlProduct in xmlProducts)
        {
            OneOf<Product, ValidationResult, InvalidXmlResult> getProductFromXmlDataResult
                = await _xmlProductToProductMappingService.GetProductFromXmlDataAsync(xmlProduct);

            int productId = xmlProduct.Id;

            OneOf<Success, ValidationResult, InvalidXmlResult, UnexpectedFailureResult> result = getProductFromXmlDataResult.Match(
                product =>
                {
                    if (product.Images is null
                        || product.Images.Count <= 0)
                    {
                        return new Success();
                    }

                    IEnumerable<XmlImportProductImage> existingProductImages = _xmlImportProductImageService.GetAllInProduct(productId);

                    List<ImageAndImageFileNameUpsertRequest> imageUpsertRequests = new();

                    for (int i = 0; i < product.Images.Count; i++)
                    {
                        ProductImage image = product.Images[i];

                        ProductImageFileNameInfo? matchingFileNameInfo = product.ImageFileNames?.FirstOrDefault(fileNameInfo =>
                        {
                            if (fileNameInfo.FileName is null) return false;

                            int indexOfDotInFileName = fileNameInfo.FileName.IndexOf('.');

                            if (indexOfDotInFileName < 0) return false;

                            string idPartOfFileName = fileNameInfo.FileName[..indexOfDotInFileName];

                            bool parseSuccess = int.TryParse(idPartOfFileName, out int imageId);

                            if (!parseSuccess) return false;

                            return imageId == image.Id;
                        });

                        ImageAndImageFileNameUpsertRequest requestFromImage = GetImageAndImageFileNameUpsertRequest(image, matchingFileNameInfo);

                        imageUpsertRequests.Add(requestFromImage);
                    }

                    OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertImagesResult
                        = _xmlImportProductImageService.UpsertFirstAndAllImagesForProduct(productId, imageUpsertRequests, existingProductImages.ToList());

                    return upsertImagesResult.Map<Success, ValidationResult, InvalidXmlResult, UnexpectedFailureResult>();
                },
                validationResult => validationResult,
                invalidXmlResult => invalidXmlResult);

            if (!result.IsT0) return result;
        }

        return new Success();
    }

    private XmlImportProductProperty? GetExistingPropertyWithSameXmlData(
        int productId, string? xmlName, int? actualXmlOrder)
    {
        IEnumerable<XmlImportProductProperty> propertiesOfProduct = _xmlImportProductPropertyService.GetAllInProduct(productId);

        foreach (XmlImportProductProperty property in propertiesOfProduct)
        {
            if (property.XmlName == xmlName
                && property.XmlDisplayOrder == actualXmlOrder)
            {
                return property;
            }
        }

        return null;
    }

    private XmlImportProductImageFileNameInfo? GetMatchingFileNameInfo(int productId, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return null;

        IEnumerable<XmlImportProductImageFileNameInfo> imageFileNames = _xmlImportProductImageFileNameInfoService.GetAllInProduct(productId);

        foreach (XmlImportProductImageFileNameInfo imageFileNameInfo in imageFileNames)
        {
            if (imageFileNameInfo.FileName == fileName) return imageFileNameInfo;
        }

        return null;
    }
}