using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductImageFileManagement.Services;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using OneOf;

using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductFirstImageDisplayPopupPartialModel
{
    public ProductFirstImageDisplayPopupPartialModel(
        ProductImagePopupUsageEnum productImagePopupUsage,
        ProductDisplayData productDisplayData,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductImageFileManagementService productImageFileManagementService,
        string? notificationBoxId = null)
    {
        ProductImagePopupUsage = productImagePopupUsage;
        ProductDisplayData = productDisplayData;
        NotificationBoxId = notificationBoxId;

        ImageToDisplay = GetImageToDisplay(
            productImagePopupUsage, productDisplayData, productImageService, productImageFileNameInfoService, productImageFileManagementService);
    }

    public ProductImagePopupUsageEnum ProductImagePopupUsage { get; }
    public ProductDisplayData ProductDisplayData { get; }
    public string? NotificationBoxId { get; }
    public ProductImage? ImageToDisplay { get; }

    private static ProductImage? GetImageToDisplay(
        ProductImagePopupUsageEnum productImagePopupUsage,
        ProductDisplayData productDisplayData,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        if (productImagePopupUsage == ProductImagePopupUsageEnum.ImagesInDatabase)
        {
            return productImageService.GetFirstImageForProduct(productDisplayData.Id);
        }
        else if (productImagePopupUsage == ProductImagePopupUsageEnum.ImagesInFiles)
        {
            IEnumerable<ProductImageFileNameInfo> imageFileNames = productImageFileNameInfoService.GetAllInProduct(productDisplayData.Id);

            ProductImageFileNameInfo? firstImageFileNameInfo = imageFileNames.FirstOrDefault(fileNameInfo => fileNameInfo.DisplayOrder == 1);

            firstImageFileNameInfo ??= imageFileNames.FirstOrDefault();

            if (firstImageFileNameInfo?.FileName is null) return null;

            OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> readImageFileResult
                = productImageFileManagementService.GetImage(firstImageFileNameInfo.FileName);

            return readImageFileResult.Match<ProductImage?>(
                imageData =>
                {
                    string fileExtension = Path.GetExtension(firstImageFileNameInfo.FileName);

                    ProductImage productImageFromData = new()
                    {
                        ProductId = productDisplayData.Id,
                        ImageData = imageData,
                        ImageContentType = GetImageContentTypeFromFileExtension(fileExtension),
                    };

                    return productImageFromData;
                },
                fileDoesntExistResult => null,
                notSupportedFileTypeResult => null);
        }

        return productDisplayData.ImagesAndImageFileInfos?.FirstOrDefault()?.ProductImage;
    }
}