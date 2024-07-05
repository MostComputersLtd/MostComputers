using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Services;

internal sealed class ProductService : IProductService
{
    public ProductService(
        IProductRepository productRepository,
        IValidator<ProductCreateRequest>? createRequestValidator = null,
        IValidator<ProductUpdateRequest>? updateRequestValidator = null)
    {
        _productRepository = productRepository;
        _createRequestValidator = createRequestValidator;
        _updateRequestValidator = updateRequestValidator;
    }

    private readonly IProductRepository _productRepository;
    private readonly IValidator<ProductCreateRequest>? _createRequestValidator;
    private readonly IValidator<ProductUpdateRequest>? _updateRequestValidator;

    public IEnumerable<Product> GetAllWithoutImagesAndProps()
    {
        return _productRepository.GetAll_WithManifacturerAndCategory();
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
        if (productRangeSearchRequest.Length == 0
            || string.IsNullOrWhiteSpace(subString)) return Enumerable.Empty<Product>();

        uint end = productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategory_WhereSearchStringMatchesAllSearchStringParts(productRangeSearchRequest.Start, end, subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereNameMatches(ProductRangeSearchRequest productRangeSearchRequest, string subString)
    {
        uint end = productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategory_WhereNameContainsSubstring(productRangeSearchRequest.Start, end, subString);
    }

    public IEnumerable<Product> GetFirstInRangeWhereAllConditionsAreMet(ProductRangeSearchRequest productRangeSearchRequest, ProductConditionalSearchRequest productConditionalSearchRequest)
    {
        uint end = productRangeSearchRequest.Start + productRangeSearchRequest.Length;

        return _productRepository.GetFirstInRange_WithManifacturerAndCategoryAndStatuses_WhereAllConditionsAreMet(productRangeSearchRequest.Start, end, productConditionalSearchRequest);
    }

    public IEnumerable<Product> GetSelectionWithoutImagesAndProps(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategory_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithFirstImage(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategoryAndFirstImage_ByIds(ids);
    }

    public IEnumerable<Product> GetSelectionWithProps(List<uint> ids)
    {
        return _productRepository.GetAll_WithManifacturerAndCategoryAndProperties_ByIds(ids);
    }

    public IEnumerable<Product> GetFirstItemsBetweenStartAndEnd(ProductRangeSearchRequest rangeSearchRequest)
    {
        uint end = rangeSearchRequest.Start + rangeSearchRequest.Length;

        if (rangeSearchRequest.Start == end) return Enumerable.Empty<Product>();

        return _productRepository.GetFirstBetweenStartAndEnd_WithCategoryAndManifacturer(rangeSearchRequest.Start, end);
    }

    public Product? GetByIdWithFirstImage(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndFirstImage(id);
    }

    public Product? GetByIdWithProps(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndProperties(id);
    }

    public Product? GetByIdWithImages(uint id)
    {
        return _productRepository.GetById_WithManifacturerAndCategoryAndImages(id);
    }

    public Product? GetProductWithHighestId()
    {
        return _productRepository.GetProductWithHighestId_WithManifacturerAndCategory();
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCreateRequest createRequest,
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

        OneOf<uint, UnexpectedFailureResult> result = _productRepository.Insert(createRequest);

        return result.Match<OneOf<uint, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductUpdateRequest updateRequest, IValidator<ProductUpdateRequest>? validator = null)
    {
        ValidationResult validationResult = ValidateTwoValidatorsDefault(updateRequest, validator, _updateRequestValidator);

        if (!validationResult.IsValid) return validationResult;

        if (updateRequest.Images is not null)
        {
            foreach (var item in updateRequest.Images)
            {
                item.DateModified = DateTime.Now;
            }
        }

        OneOf<Success, UnexpectedFailureResult> result = _productRepository.Update(updateRequest);

        return result.Match<OneOf<Success, ValidationResult, UnexpectedFailureResult>>(
            success => success, unexpectedFailure => unexpectedFailure);
    }

    public bool Delete(uint id)
    {
        return _productRepository.Delete(id);
    }
















    //private OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> SaveFirstProductInternal(Product _firstProduct)
    //{
    //    if (_firstProduct is null) return new UnexpectedFailureResult();

    //    uint productId = (uint)_firstProduct.Id;

    //    Product? oldFirstProduct = GetByIdWithImages(productId);

    //    if (oldFirstProduct is null)
    //    {
    //        ValidationResult productNullValResult = new();

    //        return new ArgumentNullException(nameof(_firstProduct));
    //    }

    //    if (oldFirstProduct.Properties is null
    //        || oldFirstProduct.Properties.Count <= 0)
    //    {
    //        oldFirstProduct.Properties = _productPropertyService.GetAllInProduct(productId)
    //            .ToList();
    //    }

    //    if (oldFirstProduct.ImageFileNames is null
    //        || oldFirstProduct.ImageFileNames.Count <= 0)
    //    {
    //        oldFirstProduct.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId)
    //            .ToList();
    //    }

    //    ProductUpdateRequest productUpdateRequest = new()
    //    {
    //        Id = _firstProduct.Id,
    //        Name = _firstProduct.Name,
    //        AdditionalWarrantyPrice = _firstProduct.AdditionalWarrantyPrice,
    //        AdditionalWarrantyTermMonths = _firstProduct.AdditionalWarrantyTermMonths,
    //        StandardWarrantyPrice = _firstProduct.StandardWarrantyPrice,
    //        StandardWarrantyTermMonths = _firstProduct.StandardWarrantyTermMonths,
    //        DisplayOrder = _firstProduct.DisplayOrder,
    //        Status = _firstProduct.Status,
    //        PlShow = _firstProduct.PlShow,
    //        DisplayPrice = _firstProduct.Price,
    //        Currency = _firstProduct.Currency,
    //        RowGuid = _firstProduct.RowGuid,
    //        Promotionid = _firstProduct.Promotionid,
    //        PromRid = _firstProduct.PromRid,
    //        PromotionPictureId = _firstProduct.PromotionPictureId,
    //        PromotionExpireDate = _firstProduct.PromotionExpireDate,
    //        AlertPictureId = _firstProduct.AlertPictureId,
    //        AlertExpireDate = _firstProduct.AlertExpireDate,
    //        PriceListDescription = _firstProduct.PriceListDescription,
    //        PartNumber1 = _firstProduct.PartNumber1,
    //        PartNumber2 = _firstProduct.PartNumber2,
    //        SearchString = _firstProduct.SearchString,
    //        Properties = new(),

    //        Images = new(),
    //        ImageFileNames = new(),
    //        CategoryID = _firstProduct.CategoryID,
    //        ManifacturerId = _firstProduct.ManifacturerId,
    //        SubCategoryId = _firstProduct.SubCategoryId,
    //    };

    //    OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> propertiesUpdateResult
    //        = UpdatePropertiesForFirstProduct(_firstProduct, oldFirstProduct, productId, productUpdateRequest);

    //    if (!propertiesUpdateResult.IsT0)
    //    {
    //        return propertiesUpdateResult;
    //    }

    //    OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> imagesUpdateResult =
    //        UpdateImagesInFirstProduct(_firstProduct, oldFirstProduct, productId, productUpdateRequest);

    //    if (!imagesUpdateResult.IsT0)
    //    {
    //        return imagesUpdateResult;
    //    }

    //    OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> imageFileNamesUpdateResult
    //        = UpdateImageFileNamesInFirstProduct(_firstProduct, oldFirstProduct, productId);

    //    if (!imageFileNamesUpdateResult.IsT0)
    //    {
    //        return imageFileNamesUpdateResult;
    //    }

    //    OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

    //    OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> resultFromUpdate
    //        = productUpdateResult.Match<OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult>>(
    //        success => success,
    //        validationResult => validationResult,
    //        unexpectedFailureResult => unexpectedFailureResult);

    //    if (!resultFromUpdate.IsT0)
    //    {
    //        return resultFromUpdate;
    //    }

    //    return new Success();
    //}

    //private OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> UpdatePropertiesForFirstProduct(
    //    Product _firstProduct,
    //    Product oldFirstProduct,
    //    uint productId,
    //    ProductUpdateRequest productUpdateRequestToAddPropsTo)
    //{
    //    if (_firstProduct is null) return new ArgumentNullException(nameof(_firstProduct));

    //    foreach (ProductProperty newProductProp in _firstProduct.Properties)
    //    {
    //        ProductProperty? oldProductPropForSameCharacteristic = oldFirstProduct.Properties
    //            .Find(prop => prop.ProductCharacteristicId == newProductProp.ProductCharacteristicId);

    //        if (oldProductPropForSameCharacteristic is null)
    //        {
    //            ProductPropertyByCharacteristicIdCreateRequest propCreateRequest = new()
    //            {
    //                ProductCharacteristicId = newProductProp.ProductCharacteristicId,
    //                ProductId = (int)productId,
    //                DisplayOrder = newProductProp.DisplayOrder,
    //                Value = newProductProp.Value,
    //                XmlPlacement = newProductProp.XmlPlacement,
    //            };

    //            OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult
    //                = _productPropertyService.InsertWithCharacteristicId(propCreateRequest);

    //            bool successFromPropertyInsert = propertyInsertResult.Match<bool>(
    //                success => true,
    //                validationResult => false,
    //                unexpectedFailureResult => false);

    //            if (!successFromPropertyInsert)
    //            {
    //                return propertyInsertResult.Match<OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult>>(
    //                    success => success,
    //                    validationResult => validationResult,
    //                    unexpectedFailureResult => unexpectedFailureResult);
    //            }

    //            continue;
    //        }

    //        if (oldProductPropForSameCharacteristic.Value == newProductProp.Value
    //            && oldProductPropForSameCharacteristic.XmlPlacement == newProductProp.XmlPlacement)
    //        {
    //            oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);

    //            continue;
    //        }

    //        CurrentProductPropertyUpdateRequest propUpdateRequest = new()
    //        {
    //            ProductCharacteristicId = newProductProp.ProductCharacteristicId,
    //            DisplayOrder = newProductProp.DisplayOrder,
    //            Value = newProductProp.Value,
    //            XmlPlacement = newProductProp.XmlPlacement,
    //        };

    //        productUpdateRequestToAddPropsTo.Properties.Add(propUpdateRequest);

    //        oldFirstProduct.Properties.Remove(oldProductPropForSameCharacteristic);
    //    }

    //    foreach (ProductProperty oldPropertyToDelete in oldFirstProduct.Properties)
    //    {
    //        if (oldPropertyToDelete.ProductCharacteristicId is null) return new UnexpectedFailureResult();

    //        bool imageFileNameDeleteSuccess = _productPropertyService.Delete(
    //            productId, (uint)oldPropertyToDelete.ProductCharacteristicId.Value);

    //        if (!imageFileNameDeleteSuccess) return new UnexpectedFailureResult();
    //    }

    //    return new Success();
    //}

    //private OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> UpdateImagesInFirstProduct(
    //    Product _firstProduct,
    //    Product oldFirstProduct,
    //    uint productId,
    //    ProductUpdateRequest productUpdateRequestToAddImagesTo)
    //{
    //    if (_firstProduct is null) return new ArgumentNullException(nameof(_firstProduct));

    //    if (_firstProduct.Images is not null)
    //    {
    //        foreach (ProductImage image in _firstProduct.Images)
    //        {
    //            ProductImage? imageInOldProduct = oldFirstProduct.Images?.Find(
    //                img => img.Id == image.Id);

    //            if (imageInOldProduct is null)
    //            {
    //                ServiceProductImageCreateRequest productImageCreateRequest = new()
    //                {
    //                    ProductId = (int)productId,
    //                    ImageData = image.ImageData,
    //                    ImageFileExtension = image.ImageFileExtension,
    //                    XML = image.XML,
    //                };

    //                OneOf<uint, ValidationResult, UnexpectedFailureResult> imageInsertResult
    //                    = _productImageService.InsertInAllImages(productImageCreateRequest);

    //                int imageId = -1;

    //                OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> outputFromImageInsert
    //                    = imageInsertResult.Match<OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult>>(
    //                    id =>
    //                    {
    //                        imageId = (int)id;

    //                        return new Success();
    //                    },
    //                    validationResult => validationResult,
    //                    unexpectedFailureResult => unexpectedFailureResult);

    //                if (!outputFromImageInsert.IsT0)
    //                {
    //                    return outputFromImageInsert;
    //                }

    //                int imageInOrderedListIndex = OrdersForImagesInFirstProduct.FindIndex(
    //                    x => x.productImage == image);

    //                if (imageInOrderedListIndex <= -1) return new UnexpectedFailureResult();

    //                (ProductImage productImage, int displayOrder) = OrdersForImagesInFirstProduct[imageInOrderedListIndex];

    //                productImage.Id = imageId;

    //                OrdersForImagesInFirstProduct[imageInOrderedListIndex] = new(productImage, displayOrder);

    //                continue;
    //            }

    //            if (CompareByteArrays(image.ImageData, imageInOldProduct.ImageData)
    //                && image.ImageFileExtension == imageInOldProduct.ImageFileExtension
    //                && image.XML == imageInOldProduct.XML)
    //            {
    //                oldFirstProduct.Images?.Remove(imageInOldProduct);

    //                continue;
    //            }

    //            CurrentProductImageUpdateRequest productImageUpdateRequest = new()
    //            {
    //                ImageData = image.ImageData,
    //                ImageFileExtension = image.ImageFileExtension,
    //                XML = image.XML,
    //            };

    //            productUpdateRequestToAddImagesTo.Images ??= new();

    //            productUpdateRequestToAddImagesTo.Images.Add(productImageUpdateRequest);

    //            oldFirstProduct.Images?.Remove(imageInOldProduct);
    //        }
    //    }

    //    if (oldFirstProduct.Images is not null
    //        && oldFirstProduct.Images.Count > 0)
    //    {
    //        foreach (ProductImage oldImageToBeRemoved in oldFirstProduct.Images)
    //        {
    //            bool imageDeleteResult = _productImageService.DeleteInAllImagesById((uint)oldImageToBeRemoved.Id);

    //            if (!imageDeleteResult) return new UnexpectedFailureResult();
    //        }
    //    }

    //    return new Success();
    //}

    //private OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> UpdateImageFileNamesInFirstProduct(
    //    Product _firstProduct,
    //    Product oldFirstProduct,
    //    uint productId)
    //{
    //    if (_firstProduct is null) return new UnexpectedFailureResult();

    //    List<ServiceProductImageFileNameInfoCreateRequest> imageFileNameInfoCreateRequests = new();

    //    if (oldFirstProduct.ImageFileNames is not null
    //        && oldFirstProduct.ImageFileNames.Count > 0)
    //    {
    //        if (_firstProduct.ImageFileNames is null
    //            || _firstProduct.ImageFileNames.Count <= 0)
    //        {
    //            foreach (ProductImageFileNameInfo oldProductImageFileName in oldFirstProduct.ImageFileNames)
    //            {
    //                if (oldProductImageFileName.DisplayOrder is null) return new UnexpectedFailureResult();

    //                bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
    //                    productId, oldProductImageFileName.DisplayOrder.Value);

    //                if (!imageFileNameDeleteResult) return new UnexpectedFailureResult();

    //                for (int k = 0; k < oldFirstProduct.ImageFileNames.Count; k++)
    //                {
    //                    ProductImageFileNameInfo oldImageFileName = oldFirstProduct.ImageFileNames[k];

    //                    oldImageFileName.DisplayOrder = k + 1;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            for (int i = 0; i < oldFirstProduct.ImageFileNames.Count; i++)
    //            {
    //                ProductImageFileNameInfo oldProductImageFileName = oldFirstProduct.ImageFileNames[i];

    //                if (oldProductImageFileName.DisplayOrder is null) return new UnexpectedFailureResult();

    //                ProductImageFileNameInfo? oldImageFileNameInCurrentProduct = _firstProduct.ImageFileNames.Find(
    //                    imgFileNameInfo => imgFileNameInfo.FileName == oldProductImageFileName.FileName);

    //                if (oldImageFileNameInCurrentProduct is not null) continue;

    //                bool imageFileNameDeleteResult = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(
    //                    productId, oldProductImageFileName.DisplayOrder.Value);

    //                if (!imageFileNameDeleteResult) return new UnexpectedFailureResult();

    //                oldFirstProduct.ImageFileNames.RemoveAt(i);

    //                i--;

    //                for (int k = 0; k < oldFirstProduct.ImageFileNames.Count; k++)
    //                {
    //                    ProductImageFileNameInfo oldImageFileName = oldFirstProduct.ImageFileNames[k];

    //                    oldImageFileName.DisplayOrder = k + 1;
    //                }
    //            }
    //        }
    //    }

    //    if (_firstProduct.ImageFileNames is not null)
    //    {
    //        int newImageFileNamesCount = 0;

    //        foreach (ProductImageFileNameInfo imageFileNameInfo in _firstProduct.ImageFileNames)
    //        {
    //            ProductImageFileNameInfo? imageFileNameOfOldProduct = oldFirstProduct.ImageFileNames?.Find(
    //                imgFileNameInfo => imgFileNameInfo.FileName == imageFileNameInfo.FileName);

    //            if (imageFileNameOfOldProduct is null)
    //            {
    //                ProductImage? imageRelatedToThatFileNameInfo = OrdersForImagesInFirstProduct.Find(
    //                    x => imageFileNameInfo.DisplayOrder == x.displayOrder)
    //                    .productImage;

    //                if (imageRelatedToThatFileNameInfo is null) return new UnexpectedFailureResult();

    //                ServiceProductImageFileNameInfoCreateRequest imageFileNameCreateRequest = new()
    //                {
    //                    ProductId = _firstProduct.Id,
    //                    DisplayOrder = imageFileNameInfo.DisplayOrder,
    //                    Active = imageFileNameInfo.Active,
    //                    FileName = $"{imageRelatedToThatFileNameInfo.Id}.{GetFileExtensionFromFileType(imageRelatedToThatFileNameInfo.ImageFileExtension ?? "*")}",
    //                };

    //                imageFileNameInfoCreateRequests.Add(imageFileNameCreateRequest);

    //                newImageFileNamesCount++;

    //                continue;
    //            }

    //            if (imageFileNameInfo.FileName is null
    //                || imageFileNameInfo.DisplayOrder is null) continue;

    //            int displayOrder = imageFileNameInfo.DisplayOrder.Value - newImageFileNamesCount;

    //            bool isFileNameInfoSameAsOldOne = displayOrder == imageFileNameOfOldProduct.DisplayOrder
    //                && imageFileNameInfo.Active == imageFileNameOfOldProduct.Active;

    //            if (isFileNameInfoSameAsOldOne) continue;

    //            ServiceProductImageFileNameInfoByFileNameUpdateRequest imageFileNameUpdateRequest = new()
    //            {
    //                ProductId = (int)productId,
    //                NewDisplayOrder = displayOrder,
    //                Active = imageFileNameInfo.Active,
    //                FileName = imageFileNameInfo.FileName,
    //                NewFileName = null,
    //            };

    //            OneOf<Success, ValidationResult, UnexpectedFailureResult> productImageFileNameUpdateResult
    //                = _productImageFileNameInfoService.UpdateByFileName(imageFileNameUpdateRequest);

    //            OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> outputFromImageFileNameUpdate
    //                = productImageFileNameUpdateResult.Match<OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult>>(
    //                    id => new Success(),
    //                    validationResult => validationResult,
    //                    unexpectedFailureResult => unexpectedFailureResult);

    //            if (!outputFromImageFileNameUpdate.IsT0)
    //            {
    //                return outputFromImageFileNameUpdate;
    //            }
    //        }
    //    }

    //    IEnumerable<ServiceProductImageFileNameInfoCreateRequest> orderedImageFileNameInfoCreateRequests = imageFileNameInfoCreateRequests
    //        .OrderBy(imageFileName => imageFileName.DisplayOrder);

    //    foreach (ServiceProductImageFileNameInfoCreateRequest fileNameInfoCreateRequest in orderedImageFileNameInfoCreateRequests)
    //    {
    //        OneOf<Success, ValidationResult, UnexpectedFailureResult> fileNameInfoInsertResult
    //            = _productImageFileNameInfoService.Insert(fileNameInfoCreateRequest);

    //        OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult> outputFromImageFileNameInsert
    //            = fileNameInfoInsertResult.Match<OneOf<Success, ArgumentNullException, ValidationResult, UnexpectedFailureResult>>(
    //            success => success,
    //            validationResult => validationResult,
    //            unexpectedFailureResult => unexpectedFailureResult);

    //        if (!outputFromImageFileNameInsert.IsT0)
    //        {
    //            return outputFromImageFileNameInsert;
    //        }
    //    }

    //    return new Success();
    //}

    //private static string GetFileExtensionFromFileType(string fileType)
    //{
    //    if (fileType.StartsWith("image/"))
    //    {
    //        return fileType[(fileType.IndexOf('/') + 1)..];
    //    }

    //    return fileType;
    //}

    //private static bool CompareByteArrays(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    //{
    //    return a.SequenceEqual(b);
    //}
}