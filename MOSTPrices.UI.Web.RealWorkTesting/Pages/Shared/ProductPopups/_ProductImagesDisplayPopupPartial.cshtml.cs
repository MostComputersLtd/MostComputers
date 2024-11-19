using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.Utils.ProductImageFileNameUtils;
using OneOf;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageAndFileNameRelationsUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductImagesDisplayPopupPartialModel
{
    public ProductImagesDisplayPopupPartialModel(
        ProductImagePopupUsageEnum productImagePopupUsageEnum,
        ProductDisplayData productDisplayData,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductImageFileManagementService productImageFileManagementService,
        string popupContainerId,
        string? notificationBoxId = null)
    {
        ProductImagePopupUsageEnum = productImagePopupUsageEnum;
        ProductData = productDisplayData;
        PopupContainerId = popupContainerId;
        NotificationBoxId = notificationBoxId;

        ImagesToDisplay = GetImagesToDisplayFromData(
            productImagePopupUsageEnum,
            productDisplayData,
            productImageService,
            productImageFileNameInfoService,
            productImageFileManagementService);

        if (productDisplayData?.ImagesAndImageFileInfos is not null)
        {
            productDisplayData.ImagesAndImageFileInfos = OrderImagesAndImageFileNameInfos(productDisplayData.ImagesAndImageFileInfos);
        }
    }

    public List<ImageAndImageFileNameRelation> ImagesToDisplay { get; set; }
    public ProductImagePopupUsageEnum ProductImagePopupUsageEnum { get; }
    public ProductDisplayData ProductData { get; set; }
    public string PopupContainerId { get; }
    public string? NotificationBoxId { get; }

    private static List<ImageAndImageFileNameRelation> GetImagesToDisplayFromData(
        ProductImagePopupUsageEnum productImagePopupUsageEnum,
        ProductDisplayData productDisplayData,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductImageFileManagementService productImageFileManagementService)
    {
        if (productImagePopupUsageEnum == ProductImagePopupUsageEnum.ImagesInDatabase)
        {
            IEnumerable<ProductImage> productImages = productImageService.GetAllInProduct(productDisplayData.Id);
            IEnumerable<ProductImageFileNameInfo> productImageFileNameInfos = productImageFileNameInfoService.GetAllInProduct(productDisplayData.Id);

            return GetImageRelationsFromImagesAndImageFileInfos(productImages.ToList(), productImageFileNameInfos.ToList());
        }
        else if (productImagePopupUsageEnum == ProductImagePopupUsageEnum.ImagesInFiles)
        {
            IEnumerable<ProductImageFileNameInfo> productImageFileNameInfos = productImageFileNameInfoService.GetAllInProduct(productDisplayData.Id);

            List<ImageAndImageFileNameRelation> output = new();

            foreach (ProductImageFileNameInfo imageFileNameInfo in productImageFileNameInfos)
            {
                if (imageFileNameInfo.FileName == null) continue;

                OneOf<byte[], FileDoesntExistResult, NotSupportedFileTypeResult> imageFileReadResult
                    = productImageFileManagementService.GetImage(imageFileNameInfo.FileName);

                ImageAndImageFileNameRelation imageAndFileNameRelation = imageFileReadResult.Match<ImageAndImageFileNameRelation>(
                    imageData =>
                    {
                        string fileExtension = Path.GetExtension(imageFileNameInfo.FileName);

                        ProductImage productImageFromData = new()
                        {
                            ProductId = productDisplayData.Id,
                            ImageData = imageData,
                            ImageContentType = GetImageContentTypeFromFileExtension(fileExtension),
                        };

                        return new(productImageFromData, imageFileNameInfo);
                    },
                    fileDoesntExistResult => new(null, imageFileNameInfo),
                    notSupportedFileTypeResult => new(null, imageFileNameInfo));

                output.Add(imageAndFileNameRelation);
            }

            return OrderImagesAndImageFileNameInfos(output);
        }

        if (productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0)
        {
            return new();
        }

        return OrderImagesAndImageFileNameInfos(productDisplayData.ImagesAndImageFileInfos);
    }
}