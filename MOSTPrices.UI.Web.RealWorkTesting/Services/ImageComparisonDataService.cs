using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts.ExternalXmlImport;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ImagesAndImageFilesComparison;
using OneOf;
using FluentValidation.Results;

using static MOSTComputers.Services.ProductImageFileManagement.Utils.ProductImageFileManagementUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;
using static MOSTComputers.Utils.OneOf.OneOfExtensions;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services;

public class ImageComparisonDataService : IImageComparisonDataService
{
    public ImageComparisonDataService(
        IProductService productService,
        IProductImageService productImageService,
        IXmlImportProductImageService xmlImportProductImageService,
        IXmlImportProductImageFileNameInfoService xmlImportProductImageFileNameInfoService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        _productService = productService;
        _productImageService = productImageService;
        _xmlImportProductImageService = xmlImportProductImageService;
        _xmlImportProductImageFileNameInfoService = xmlImportProductImageFileNameInfoService;
        _productImageFileManagementService = productImageFileManagementService;
    }

    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IXmlImportProductImageService _xmlImportProductImageService;
    private readonly IXmlImportProductImageFileNameInfoService _xmlImportProductImageFileNameInfoService;
    private readonly IProductImageFileManagementService _productImageFileManagementService;

    public async Task<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException>>
        GetImagesForCategoryAsync(int categoryId, ImageComparisonDataSourceEnum dataSourceEnum)
    {
        IEnumerable<Product> productsInCategory = _productService.GetAllInCategoryWithoutImagesAndProps(categoryId);

        if (!productsInCategory.Any()) return new();

        return dataSourceEnum switch
        {
            ImageComparisonDataSourceEnum.ImageFiles => (await GetImageComparisonDataFromFilesAsync(productsInCategory))
                .Map<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException>(),
            ImageComparisonDataSourceEnum.FirstImages => GetImageComparisonDataFromFirstImages(productsInCategory),
            ImageComparisonDataSourceEnum.AllImages => GetImageComparisonDataFromAllImages(productsInCategory),
            ImageComparisonDataSourceEnum.FirstImagesForTesting => GetImageComparisonDataFromFirstImagesForTesting(productsInCategory),
            ImageComparisonDataSourceEnum.AllImagesForTesting => GetImageComparisonDataFromAllImagesForTesting(productsInCategory),
            _ => new NotImplementedException()
        };
    }

    public async Task<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException>>
        GetImagesForProductAsync(int productId, ImageComparisonDataSourceEnum dataSource)
    {
        if (dataSource == ImageComparisonDataSourceEnum.ImageFiles)
        {
            OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult> getImageComparisonDataFromFilesResult
                = await GetImageComparisonDataFromFilesAsync(productId);

            return getImageComparisonDataFromFilesResult
                .Map<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException>();
        }

        Product? product = _productService.GetProductFull(productId);

        if (product is null)
        {
            return new List<ImageComparisonData>();
        }

        else if (dataSource == ImageComparisonDataSourceEnum.FirstImages)
        {
            ImageComparisonData? firstImageComparisonData = GetImageComparisonDataFromFirstImages(product);

            if (firstImageComparisonData is null) return new List<ImageComparisonData>();

            return new List<ImageComparisonData>() { firstImageComparisonData };
        }
        else if (dataSource == ImageComparisonDataSourceEnum.AllImages)
        {
            return GetImageComparisonDataFromAllImages(product);
        }
        else if (dataSource == ImageComparisonDataSourceEnum.FirstImagesForTesting)
        {
            ImageComparisonData? firstImageComparisonData = GetImageComparisonDataFromFirstImagesForTesting(product);

            if (firstImageComparisonData is null) return new List<ImageComparisonData>();

            return new List<ImageComparisonData>() { firstImageComparisonData };
        }
        else if (dataSource == ImageComparisonDataSourceEnum.AllImagesForTesting)
        {
            return GetImageComparisonDataFromAllImagesForTesting(product);
        }

        return new NotImplementedException();
    }

    private async Task<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult>>
        GetImageComparisonDataFromFilesAsync(IEnumerable<Product> productsToGetDataFrom)
    {
        List<ImageComparisonData> output = new();

        foreach (Product product in productsToGetDataFrom)
        {
            IEnumerable<XmlImportProductImageFileNameInfo> imageFileNames = _xmlImportProductImageFileNameInfoService.GetAllInProduct(product.Id);

            foreach (XmlImportProductImageFileNameInfo imageFileNameData in imageFileNames)
            {
                string? fileName = imageFileNameData.FileName;

                if (string.IsNullOrEmpty(fileName)) continue;

                string? fileExtension = GetFileExtensionWithoutDot(fileName);

                if (string.IsNullOrWhiteSpace(fileExtension))
                {
                    return GetInvalidUrlValidationResult();
                }

                string? imageContentType = GetImageContentTypeFromFileExtension(fileExtension);

                if (string.IsNullOrWhiteSpace(imageContentType))
                {
                    return GetInvalidUrlValidationResult();
                }

                OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> getImageResult
                    = await _productImageFileManagementService.GetImageAsync(fileName);

                bool isGetImageSuccessful = getImageResult.Match(
                    imageData => true,
                    fileDoesntExistResult => false,
                    notSupportedFileTypeResult => false);

                if (!isGetImageSuccessful)
                {
                    return getImageResult.Match<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult>>(
                        imageData => output,
                        fileDoesntExistResult => fileDoesntExistResult,
                        notSupportedFileTypeResult => notSupportedFileTypeResult);
                }

                ImageComparisonData imageComparisonData = new()
                {
                    IdOrFileName = fileName,
                    ProductId = product.Id,
                    ImageContentType = imageContentType,
                    ImageData = getImageResult.AsT0,
                };

                output.Add(imageComparisonData);
            }
        }

        return output;
    }

    private async Task<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult>>
        GetImageComparisonDataFromFilesAsync(int productId)
    {
        List<ImageComparisonData>? output = new();

        IEnumerable<XmlImportProductImageFileNameInfo> imageFileNames = _xmlImportProductImageFileNameInfoService.GetAllInProduct(productId);

        foreach (XmlImportProductImageFileNameInfo imageFileNameData in imageFileNames)
        {
            string? fileName = imageFileNameData.FileName;

            if (string.IsNullOrEmpty(fileName)) continue;

            string? fileExtension = GetFileExtensionWithoutDot(fileName);

            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                return GetInvalidUrlValidationResult();
            }

            string? imageContentType = GetImageContentTypeFromFileExtension(fileExtension);

            if (string.IsNullOrWhiteSpace(imageContentType))
            {
                return GetInvalidUrlValidationResult();
            }

            OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> getImageResult
                = await _productImageFileManagementService.GetImageAsync(fileName);

            bool isGetImageSuccessful = getImageResult.Match(
                imageData => true,
                fileDoesntExistResult => false,
                notSupportedFileTypeResult => false);

            if (!isGetImageSuccessful)
            {
                return getImageResult.Match<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult>>(
                    imageData => output,
                    fileDoesntExistResult => fileDoesntExistResult,
                    notSupportedFileTypeResult => notSupportedFileTypeResult);
            }

            ImageComparisonData imageComparisonData = new()
            {
                IdOrFileName = fileName,
                ProductId = productId,
                ImageContentType = imageContentType,
                ImageData = getImageResult.AsT0,
            };

            output.Add(imageComparisonData);
        }

        return output;
    }

    private List<ImageComparisonData> GetImageComparisonDataFromFirstImages(IEnumerable<Product> productsToGetDataFrom)
    {
        List<ImageComparisonData> output = new();

        foreach (Product product in productsToGetDataFrom)
        {
            ImageComparisonData? imageComparisonDataFromProduct = GetImageComparisonDataFromFirstImages(product);

            if (imageComparisonDataFromProduct is null) continue;

            output.Add(imageComparisonDataFromProduct);
        }

        return output;
    }

    private ImageComparisonData? GetImageComparisonDataFromFirstImages(Product product)
    {
        ProductImage? productFirstImage = _productImageService.GetFirstImageForProduct(product.Id);

        if (productFirstImage is null) return null;

        ImageComparisonData imageComparisonData = new()
        {
            IdOrFileName = productFirstImage.Id,
            ProductId = product.Id,
            ImageContentType = productFirstImage.ImageContentType,
            ImageData = productFirstImage.ImageData,
        };

        return imageComparisonData;
    }

    private List<ImageComparisonData> GetImageComparisonDataFromAllImages(IEnumerable<Product> productsToGetDataFrom)
    {
        List<ImageComparisonData> output = new();

        foreach (Product product in productsToGetDataFrom)
        {
            List<ImageComparisonData> imageComparisonDataFromProduct = GetImageComparisonDataFromAllImages(product);

            output.AddRange(imageComparisonDataFromProduct);
        }
        return output;
    }

    private List<ImageComparisonData> GetImageComparisonDataFromAllImages(Product product)
    {
        List<ImageComparisonData> output = new();

        IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(product.Id);

        foreach (ProductImage productImage in productImages)
        {
            ImageComparisonData imageComparisonData = new()
            {
                IdOrFileName = productImage.Id,
                ProductId = product.Id,
                ImageContentType = productImage.ImageContentType,
                ImageData = productImage.ImageData,
            };

            output.Add(imageComparisonData);
        }

        return output;
    }

    private List<ImageComparisonData> GetImageComparisonDataFromFirstImagesForTesting(IEnumerable<Product> productsToGetDataFrom)
    {
        List<ImageComparisonData> output = new();

        foreach (Product product in productsToGetDataFrom)
        {
            ImageComparisonData? imageComparisonDataForProduct = GetImageComparisonDataFromFirstImagesForTesting(product);

            if (imageComparisonDataForProduct is null) continue;

            output.Add(imageComparisonDataForProduct);
        }
        return output;
    }

    private ImageComparisonData? GetImageComparisonDataFromFirstImagesForTesting(Product product)
    {
        XmlImportProductImage? productFirstImage = _xmlImportProductImageService.GetFirstImageForProduct(product.Id);

        if (productFirstImage is null) return null;

        ImageComparisonData imageComparisonData = new()
        {
            IdOrFileName = productFirstImage.Id,
            ProductId = product.Id,
            ImageContentType = productFirstImage.ImageContentType,
            ImageData = productFirstImage.ImageData,
        };

        return imageComparisonData;
    }

    private List<ImageComparisonData> GetImageComparisonDataFromAllImagesForTesting(IEnumerable<Product> productsToGetDataFrom)
    {
        List<ImageComparisonData> output = new();

        foreach (Product product in productsToGetDataFrom)
        {
            List<ImageComparisonData> imageComparisonDataForProduct = GetImageComparisonDataFromAllImagesForTesting(product);

            output.AddRange(imageComparisonDataForProduct);
        }

        return output;
    }

    private List<ImageComparisonData> GetImageComparisonDataFromAllImagesForTesting(Product product)
    {
        List<ImageComparisonData> output = new();

        IEnumerable<XmlImportProductImage> productImages = _xmlImportProductImageService.GetAllInProduct(product.Id);

        foreach (XmlImportProductImage productImage in productImages)
        {
            ImageComparisonData imageComparisonData = new()
            {
                IdOrFileName = productImage.Id,
                ProductId = product.Id,
                ImageContentType = productImage.ImageContentType,
                ImageData = productImage.ImageData,
            };

            output.Add(imageComparisonData);
        }

        return output;
    }

    private static ValidationResult GetInvalidUrlValidationResult(
        string propertyName = nameof(XmlShopItemImage.PictureUrl))
    {
        ValidationResult invalidPathValidationResult = new();

        invalidPathValidationResult.Errors.Add(new(propertyName, "Url is invalid"));

        return invalidPathValidationResult;
    }
}