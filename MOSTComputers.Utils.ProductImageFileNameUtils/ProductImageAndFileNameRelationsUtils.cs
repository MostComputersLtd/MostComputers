using MOSTComputers.Models.Product.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MOSTComputers.Utils.ProductImageFileNameUtils;
public static class ProductImageAndFileNameRelationsUtils
{
    public static List<ImageAndImageFileNameRelation> GetImageDictionaryFromImagesAndImageFileInfos(
        List<ProductImage>? productImages, List<ProductImageFileNameInfo>? productImageFileNameInfos)
    {
        List<ImageAndImageFileNameRelation> output = new();

        if (productImages != null)
        {
            foreach (ProductImage image in productImages)
            {
                if (image.ImageContentType is null) continue;

                string? imageFileName = ProductImageFileNameUtils.GetImageFileNameFromImageData(image.Id, image.ImageContentType);

                if (imageFileName is null) continue;

                ProductImageFileNameInfo? matchingProductImageFileNameInfo
                    = productImageFileNameInfos?.FirstOrDefault(imageFileInfo => imageFileName == imageFileInfo.FileName);

                output.Add(new(image, matchingProductImageFileNameInfo));

                if (matchingProductImageFileNameInfo is not null)
                {
                    productImageFileNameInfos!.Remove(matchingProductImageFileNameInfo);
                }
            }
        }

        if (productImageFileNameInfos != null)
        {
            foreach (ProductImageFileNameInfo imageFileNameInfo in productImageFileNameInfos)
            {
                if (imageFileNameInfo.FileName is null) continue;

                (string? imageIdOrTemporaryId, string? imageFileExtension) = ProductImageFileNameUtils.GetImageDataFromImageFileName(imageFileNameInfo.FileName);

                if (imageIdOrTemporaryId is null) continue;

                bool isImageIdParseSuccessful = int.TryParse(imageIdOrTemporaryId, out int imageId);

                if (!isImageIdParseSuccessful)
                {
                    output.Add(new(null, imageFileNameInfo));

                    continue;
                }

                ProductImage? matchingProductImage = productImages?.FirstOrDefault(image => image.Id == imageId);

                output.Add(new(matchingProductImage, imageFileNameInfo));
            }
        }

        return OrderImagesAndImageFileNameInfos(output);
    }

    public static List<ImageAndImageFileNameRelation> OrderImagesAndImageFileNameInfos(
        List<ImageAndImageFileNameRelation> imagesAndImageFileNameInfos)
    {
        imagesAndImageFileNameInfos = imagesAndImageFileNameInfos.ToList();

        ImageAndImageFileNameRelation[] output
            = new ImageAndImageFileNameRelation[imagesAndImageFileNameInfos.Count];

        for (int i = 0; i < imagesAndImageFileNameInfos.Count; i++)
        {
            ImageAndImageFileNameRelation imageAndFileNameRelation = imagesAndImageFileNameInfos[i];

            ProductImage? image = imageAndFileNameRelation.ProductImage;
            ProductImageFileNameInfo? imageFileNameInfo = imageAndFileNameRelation.ProductImageFileNameInfo;

            if (image is null
                || imageFileNameInfo is null
                || imageFileNameInfo.DisplayOrder is null) continue;

            output[imageFileNameInfo.DisplayOrder.Value - 1] = imageAndFileNameRelation;

            imagesAndImageFileNameInfos.Remove(imageAndFileNameRelation);

            i--;
        }

        foreach (ImageAndImageFileNameRelation imageAndFileNameRelation in imagesAndImageFileNameInfos
            .OrderBy(x => x.ProductImage?.Id ?? int.MaxValue))
        {
            for (int i = 0; i < output.Length; i++)
            {
                ImageAndImageFileNameRelation outputRelation = output[i];

                if (outputRelation != null) continue;

                output[i] = imageAndFileNameRelation;

                break;
            }
        }

        return output.ToList();
    }

    public static int GetLowestUnpopulatedDisplayOrder(List<ImageAndImageFileNameRelation>? imagesAndImageFileInfos)
    {
        if (imagesAndImageFileInfos is null
            || imagesAndImageFileInfos.Count <= 0) return 1;

        int currentDisplayOrder = 1;

        bool foundCurrentDisplayOrder = false;

        while (foundCurrentDisplayOrder)
        {
            foundCurrentDisplayOrder = false;

            foreach (ImageAndImageFileNameRelation tuple in imagesAndImageFileInfos)
            {
                if (tuple.ProductImageFileNameInfo?.DisplayOrder == currentDisplayOrder)
                {
                    foundCurrentDisplayOrder = true;

                    break;
                }
            }

            currentDisplayOrder++;
        }

        return currentDisplayOrder;
    }
}