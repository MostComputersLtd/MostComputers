using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.StaticUtilities;
using System.Diagnostics;

namespace MOSTComputers.UI.Web.Pages.Shared;

public class ProductImagesDisplayPopupModel
{
    public ProductImagesDisplayPopupModel(Product product)
    {
        Product = product;

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count <= 0
            || product.Images is null
            || product.Images.Count <= 0) return;

        foreach (ProductImageFileNameInfo imageFileNameInfo in product.ImageFileNames)
        {
            if (string.IsNullOrWhiteSpace(imageFileNameInfo.FileName)
                || imageFileNameInfo.DisplayOrder is null) continue;

            int? imageIdFromFileInfoName = ImageFileNameInfoToImageDataUtils.GetImageIdFromImageFileNameInfoName(imageFileNameInfo.FileName);

            int indexOfImageWithId = product.Images.FindIndex(image => image.Id == imageIdFromFileInfoName);

            if (indexOfImageWithId <= -1) continue;

            int newIndexOfImageWithId = imageFileNameInfo.DisplayOrder.Value - 1;

            ProductImage imageToMove = product.Images[indexOfImageWithId];

            product.Images.RemoveAt(indexOfImageWithId);

            if (newIndexOfImageWithId > indexOfImageWithId)
            {
                newIndexOfImageWithId--;
            }

            product.Images.Insert(newIndexOfImageWithId, imageToMove);
        }
    }

    public Product Product { get; set; }
}