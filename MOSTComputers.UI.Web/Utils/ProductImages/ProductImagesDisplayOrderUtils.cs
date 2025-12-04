using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New;

namespace MOSTComputers.UI.Web.Utils.ProductImages;

public static class ProductImagesDisplayOrderUtils
{
    public static List<TObjectWithDisplayOrder> OrderImagesAndImageFileNameInfos<TObjectWithDisplayOrder>(
        List<TObjectWithDisplayOrder> imageRelatedItems,
        Func<TObjectWithDisplayOrder, int?> getDisplayOrderFunc,
        Func<TObjectWithDisplayOrder, int?> getImageIdFunc)
    {
        imageRelatedItems = imageRelatedItems.ToList();

        TObjectWithDisplayOrder[] output
            = new TObjectWithDisplayOrder[imageRelatedItems.Count];

        for (int i = 0; i < imageRelatedItems.Count; i++)
        {
            TObjectWithDisplayOrder imageRelatedItem = imageRelatedItems[i];

            int? displayOrder = getDisplayOrderFunc(imageRelatedItem);

            if (displayOrder is not null)
            {
                displayOrder = int.Clamp(displayOrder.Value, 1, output.Length + 1);
            }    

            if (displayOrder is null
                || output[displayOrder.Value - 1] is not null)
            {
                continue;
            }

            output[displayOrder.Value - 1] = imageRelatedItem;

            imageRelatedItems.Remove(imageRelatedItem);

            i--;
        }

        IOrderedEnumerable<TObjectWithDisplayOrder> remainingItemsOrdered = imageRelatedItems
            .OrderBy(x => getImageIdFunc(x) ?? int.MaxValue);

        foreach (TObjectWithDisplayOrder imageAndFileNameRelation in remainingItemsOrdered)
        {
            for (int i = 0; i < output.Length; i++)
            {
                TObjectWithDisplayOrder outputRelation = output[i];

                if (outputRelation != null) continue;

                output[i] = imageAndFileNameRelation;

                break;
            }
        }

        return output.ToList();
    }
}