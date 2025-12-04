using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

namespace MOSTComputers.Services.ProductRegister.Utils;
public static class ImageUtils
{
    internal static bool CompareByteArrays(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
    {
        return a.SequenceEqual(b);
    }

    internal static List<ProductImageWithFileForProductUpsertRequest> OrderProductImageWithFileUpsertRequests(
        List<ProductImageWithFileForProductUpsertRequest> imageWithFileUpsertRequests)
    {
        imageWithFileUpsertRequests = imageWithFileUpsertRequests.ToList();

        ProductImageWithFileForProductUpsertRequest[] output = new ProductImageWithFileForProductUpsertRequest[imageWithFileUpsertRequests.Count];

        for (int i = 0; i < imageWithFileUpsertRequests.Count; i++)
        {
            ProductImageWithFileForProductUpsertRequest imageWithFileUpsertRequest = imageWithFileUpsertRequests[i];

            if (imageWithFileUpsertRequest.FileUpsertRequest?.CustomDisplayOrder is null) continue;

            int indexFromDisplayOrder = imageWithFileUpsertRequest.FileUpsertRequest.CustomDisplayOrder.Value - 1;

            if (output[indexFromDisplayOrder] is not null) continue;

            output[indexFromDisplayOrder] = imageWithFileUpsertRequest;

            imageWithFileUpsertRequests.Remove(imageWithFileUpsertRequest);

            i--;
        }

        foreach (ProductImageWithFileForProductUpsertRequest imageWithFileUpsertRequest in imageWithFileUpsertRequests
            .OrderBy(x => x.ExistingImageId ?? int.MaxValue))
        {
            for (int i = 0; i < output.Length; i++)
            {
                ProductImageWithFileForProductUpsertRequest outputRelation = output[i];

                if (outputRelation != null) continue;

                output[i] = imageWithFileUpsertRequest;

                break;
            }
        }

        return output.ToList();
    }

    public static List<T> OrderImageRelatedItems<T>(
        List<T> imageRelatedItems,
        Func<T, int?> getCustomDisplayOrderFunc,
        Func<T, int?> getImageIdFunc)
    {
        imageRelatedItems = imageRelatedItems.ToList();

        T[] output = new T[imageRelatedItems.Count];

        for (int i = 0; i < imageRelatedItems.Count; i++)
        {
            T imageRelatedItem = imageRelatedItems[i];

            int? displayOrder = getCustomDisplayOrderFunc(imageRelatedItem);

            if (displayOrder is null) continue;

            int indexFromDisplayOrder = displayOrder.Value - 1;

            if (output[indexFromDisplayOrder] is not null) continue;

            output[indexFromDisplayOrder] = imageRelatedItem;

            imageRelatedItems.Remove(imageRelatedItem);

            i--;
        }

        IOrderedEnumerable<T> remainingImageRelatedItemsOrdered = imageRelatedItems
            .OrderBy(x => getImageIdFunc(x) ?? int.MaxValue);

        foreach (T imageRelatedItem in remainingImageRelatedItemsOrdered)
        {
            for (int i = 0; i < output.Length; i++)
            {
                T itemAtOutputLocation = output[i];

                if (itemAtOutputLocation != null) continue;

                output[i] = imageRelatedItem;

                break;
            }
        }

        return output.ToList();
    }
}