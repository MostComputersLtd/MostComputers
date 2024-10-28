using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Utils.ProductImageFileNameUtils;

namespace MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils;

internal static class ImageMappingUtils
{
    internal static ImageFileAndFileNameInfoUpsertRequest? GetFileUpsertRequest(
        ImageAndImageFileNameRelation imageAndImageFileNameRelation, bool useOldFileName, bool useImageId)
    {
        ProductImage? image = imageAndImageFileNameRelation.ProductImage;
        ProductImageFileNameInfo? fileNameInfo = imageAndImageFileNameRelation.ProductImageFileNameInfo;

        if (image?.ImageContentType is null
            || image.ImageData is null) return null;

        bool shouldUseImageId = (useImageId && image.Id > 0);

        ImageFileAndFileNameInfoUpsertRequest imageFileAndFileNameInfoUpsertRequest = new()
        {
            ImageContentType = image.ImageContentType,
            ImageData = image.ImageData,
            RelatedImageId = shouldUseImageId ? image.Id : null,
            DisplayOrder = fileNameInfo?.DisplayOrder,
            Active = fileNameInfo?.Active ?? false,
            OldFileName = useOldFileName ? fileNameInfo?.FileName : null,
        };

        return imageFileAndFileNameInfoUpsertRequest;
    }

}