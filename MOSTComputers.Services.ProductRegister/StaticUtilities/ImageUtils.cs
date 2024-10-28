using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.StaticUtilities;
internal static class ImageUtils
{
    internal static bool CompareByteArrays(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        return a.SequenceEqual(b);
    }

    internal static List<ImageAndImageFileNameUpsertRequest> OrderImageAndImageFileNameUpsertRequests(
        List<ImageAndImageFileNameUpsertRequest> imagesAndImageFileNameInfos)
    {
        imagesAndImageFileNameInfos = imagesAndImageFileNameInfos.ToList();

        ImageAndImageFileNameUpsertRequest[] output = new ImageAndImageFileNameUpsertRequest[imagesAndImageFileNameInfos.Count];

        for (int i = 0; i < imagesAndImageFileNameInfos.Count; i++)
        {
            ImageAndImageFileNameUpsertRequest imageAndFileNameRelation = imagesAndImageFileNameInfos[i];

            ProductImageUpsertRequest? image = imageAndFileNameRelation.ProductImageUpsertRequest;
            ProductImageFileNameInfoUpsertRequest? imageFileNameInfo = imageAndFileNameRelation.ProductImageFileNameInfoUpsertRequest;

            if (image is null
                || imageFileNameInfo is null
                || imageFileNameInfo.NewDisplayOrder is null) continue;

            output[imageFileNameInfo.NewDisplayOrder.Value - 1] = imageAndFileNameRelation;

            imagesAndImageFileNameInfos.Remove(imageAndFileNameRelation);

            i--;
        }

        foreach (ImageAndImageFileNameUpsertRequest imageAndFileNameRelation in imagesAndImageFileNameInfos
            .OrderBy(x => x.ProductImageUpsertRequest?.OriginalImageId ?? int.MaxValue))
        {
            for (int i = 0; i < output.Length; i++)
            {
                ImageAndImageFileNameUpsertRequest outputRelation = output[i];

                if (outputRelation != null) continue;

                output[i] = imageAndFileNameRelation;

                break;
            }
        }

        return output.ToList();
    }
}