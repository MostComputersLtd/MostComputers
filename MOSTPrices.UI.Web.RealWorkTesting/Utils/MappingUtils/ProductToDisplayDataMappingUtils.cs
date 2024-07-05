using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils;

public static class ProductToDisplayDataMappingUtils
{
    public static ProductDisplayData MapToProductDisplayData(Product product, ProductWorkStatuses? productWorkStatuses = null)
    {
        return new ProductDisplayData()
        {
            Id = product.Id,
            Name = product.Name,
            AdditionalWarrantyPrice = product.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = product.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = product.StandardWarrantyPrice,
            StandardWarrantyTermMonths = product.StandardWarrantyTermMonths,
            DisplayOrder = product.DisplayOrder,
            Status = product.Status,
            PlShow = product.PlShow,
            Price1 = 0,
            DisplayPrice = product.Price,
            Price3 = 0,
            Currency = product.Currency,
            RowGuid = product.RowGuid,
            PromotionId = product.Promotionid,
            PromRid = product.PromRid,
            PromotionPictureId = product.PromotionPictureId,
            PromotionExpireDate = product.PromotionExpireDate,
            AlertPictureId = product.AlertPictureId,
            AlertExpireDate = product.AlertExpireDate,
            PriceListDescription = product.PriceListDescription,
            PartNumber1 = product.PartNumber1,
            PartNumber2 = product.PartNumber2,
            SearchString = product.SearchString,
            Category = product.Category,
            Manifacturer = product.Manifacturer,
            SubCategoryId = product.SubCategoryId,
            ImagesAndImageFileInfos = GetImageDictionaryFromImagesAndImageFileInfos(product.Images, product.ImageFileNames?.ToList()),
            Properties = product.Properties?.ToList(),

            CategoryId = product.CategoryID,
            ManifacturerId = product.ManifacturerId,

            ProductWorkStatusesId = productWorkStatuses?.Id ?? -1,
            ProductNewStatus = productWorkStatuses?.ProductNewStatus,
            ProductXmlStatus = productWorkStatuses?.ProductXmlStatus,
            ReadyForImageInsert = productWorkStatuses?.ReadyForImageInsert,
        };
    }

    public static Product MapToProduct(ProductDisplayData productDisplayData)
    {
        return new Product()
        {
            Id = productDisplayData.Id,
            Name = productDisplayData.Name,
            AdditionalWarrantyPrice = productDisplayData.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productDisplayData.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productDisplayData.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productDisplayData.StandardWarrantyTermMonths,
            DisplayOrder = productDisplayData.DisplayOrder,
            Status = productDisplayData.Status,
            PlShow = productDisplayData.PlShow,
            Price = productDisplayData.DisplayPrice,
            Currency = productDisplayData.Currency,
            RowGuid = productDisplayData.RowGuid,
            Promotionid = productDisplayData.PromotionId,
            PromRid = productDisplayData.PromRid,
            PromotionPictureId = productDisplayData.PromotionPictureId,
            PromotionExpireDate = productDisplayData.PromotionExpireDate,
            AlertPictureId = productDisplayData.AlertPictureId,
            AlertExpireDate = productDisplayData.AlertExpireDate,
            PriceListDescription = productDisplayData.PriceListDescription,
            PartNumber1 = productDisplayData.PartNumber1,
            PartNumber2 = productDisplayData.PartNumber2,
            SearchString = productDisplayData.SearchString,
            Category = productDisplayData.Category,
            Manifacturer = productDisplayData.Manifacturer,
            SubCategoryId = productDisplayData.SubCategoryId,
            Images = productDisplayData.ImagesAndImageFileInfos?
                .Select(x => x.Item1)
                .Where(x => x != null)!
                .ToList<ProductImage>(),
            ImageFileNames = productDisplayData.ImagesAndImageFileInfos?
                .Select(x => x.Item2)
                .Where(x => x != null)!
                .ToList<ProductImageFileNameInfo>(),
            Properties = productDisplayData.Properties?.ToList() ?? new(),

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryID = productDisplayData.CategoryId,
        };
    }

    public static ProductDisplayData CloneProductDisplayData(ProductDisplayData productDisplayData)
    {
        return new ProductDisplayData()
        {
            Id = productDisplayData.Id,
            Name = productDisplayData.Name,
            AdditionalWarrantyPrice = productDisplayData.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productDisplayData.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productDisplayData.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productDisplayData.StandardWarrantyTermMonths,
            DisplayOrder = productDisplayData.DisplayOrder,
            Status = productDisplayData.Status,
            PlShow = productDisplayData.PlShow,
            Price1 = productDisplayData.Price1,
            DisplayPrice = productDisplayData.DisplayPrice,
            Price3 = productDisplayData.Price3,
            Currency = productDisplayData.Currency,
            RowGuid = productDisplayData.RowGuid,
            PromotionId = productDisplayData.PromotionId,
            PromRid = productDisplayData.PromRid,
            PromotionPictureId = productDisplayData.PromotionPictureId,
            PromotionExpireDate = productDisplayData.PromotionExpireDate,
            AlertPictureId = productDisplayData.AlertPictureId,
            AlertExpireDate = productDisplayData.AlertExpireDate,
            PriceListDescription = productDisplayData.PriceListDescription,
            PartNumber1 = productDisplayData.PartNumber1,
            PartNumber2 = productDisplayData.PartNumber2,
            SearchString = productDisplayData.SearchString,
            Category = productDisplayData.Category,
            Manifacturer = productDisplayData.Manifacturer,
            SubCategoryId = productDisplayData.SubCategoryId,

            ImagesAndImageFileInfos = productDisplayData.ImagesAndImageFileInfos?
            .Select(x =>
            {
                bool imageIsNull = x.Item1 is not null;
                bool imageFileNameIsNull = x.Item2 is not null;

                return new Tuple<ProductImage?, ProductImageFileNameInfo?>(
                    imageIsNull ?
                        new ProductImage()
                        {
                            Id = x.Item1!.Id,
                            ProductId = x.Item1.ProductId,
                            ImageData = x.Item1.ImageData,
                            ImageFileExtension = x.Item1.ImageFileExtension,
                            HtmlData = x.Item1.HtmlData,
                            DateModified = x.Item1.DateModified,
                        }
                    : null,
                    imageFileNameIsNull ?
                        new ProductImageFileNameInfo()
                        {
                            ProductId = x.Item2!.ProductId,
                            ImageNumber = x.Item2.ImageNumber,
                            DisplayOrder = x.Item2.DisplayOrder,
                            Active = x.Item2.Active,
                            FileName = x.Item2.FileName,
                        }
                    : null);
            })?
            .ToList(),

            Properties = productDisplayData.Properties?
                 .Select(x => new ProductProperty()
                 {
                     ProductId = x.ProductId,
                     ProductCharacteristicId = x.ProductCharacteristicId,
                     DisplayOrder = x.DisplayOrder,
                     Value = x.Value,
                     Characteristic = x.Characteristic,
                     XmlPlacement = x.XmlPlacement,
                 }).ToList(),

            ManifacturerId = productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,

            ProductWorkStatusesId = productDisplayData.Id,
            ProductNewStatus = productDisplayData.ProductNewStatus,
            ProductXmlStatus = productDisplayData.ProductXmlStatus,
            ReadyForImageInsert = productDisplayData.ReadyForImageInsert,
        };
    }

    public static List<Tuple<ProductImage?, ProductImageFileNameInfo?>> GetImageDictionaryFromImagesAndImageFileInfos(
        List<ProductImage>? productImages, List<ProductImageFileNameInfo>? productImageFileNameInfos)
    {
        List<Tuple<ProductImage?, ProductImageFileNameInfo?>> output = new();

        if (productImages != null)
        {
            foreach (ProductImage image in productImages)
            {
                if (image.ImageFileExtension is null) continue;

                string? imageFileName = GetImageFileNameFromImageData(image.Id, image.ImageFileExtension);

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

                (string? imageIdOrTemporaryId, string? imageFileExtension) = GetImageDataFromImageFileName(imageFileNameInfo.FileName);

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

    public static List<Tuple<ProductImage?, ProductImageFileNameInfo?>> OrderImagesAndImageFileNameInfos(
        List<Tuple<ProductImage?, ProductImageFileNameInfo?>> imagesAndImageFileNameInfos)
    {
        Tuple<ProductImage?, ProductImageFileNameInfo?>[] output
            = new Tuple<ProductImage?, ProductImageFileNameInfo?>[imagesAndImageFileNameInfos.Count];

        for (int i = 0; i < imagesAndImageFileNameInfos.Count; i++)
        {
            Tuple<ProductImage?, ProductImageFileNameInfo?> tuple = imagesAndImageFileNameInfos[i];

            ProductImage? image = tuple.Item1;
            ProductImageFileNameInfo? imageFileNameInfo = tuple.Item2;

            if (image is null
                || imageFileNameInfo is null
                || imageFileNameInfo.DisplayOrder is null) continue;

            output[imageFileNameInfo.DisplayOrder.Value - 1] = tuple;

            imagesAndImageFileNameInfos.Remove(tuple);

            i--;
        }

        foreach (Tuple<ProductImage?, ProductImageFileNameInfo?> tuple in imagesAndImageFileNameInfos
            .OrderBy(x => x.Item1?.Id ?? int.MaxValue))
        {
            for (int i = 0; i < output.Length; i++)
            {
                Tuple<ProductImage?, ProductImageFileNameInfo?> outputTuple = output[i];

                if (outputTuple != null) continue;

                output[i] = tuple;

                break;
            }
        }

        return output.ToList();
    }

    public static OneOf<int, (int, int), False> GetImageIdOrTempIdFromImageFileName(string fileName)
    {
        int endIndexOfIdOfImageFromNameOfFileInfo = fileName.IndexOf('.');

        if (endIndexOfIdOfImageFromNameOfFileInfo < 0) return new False();

        string idOfImageAsString = fileName[..endIndexOfIdOfImageFromNameOfFileInfo];

        bool succeededGettingIdFromFileInfoName = int.TryParse(idOfImageAsString, out int idOfImage);

        if (succeededGettingIdFromFileInfoName) return idOfImage;

        int indexOfSeparatorOfTempId = idOfImageAsString.IndexOf('-');

        if (indexOfSeparatorOfTempId < 0) return new False();

        string productIdPartOfTempIdAsString = idOfImageAsString[..indexOfSeparatorOfTempId];

        bool isProductIdParsed = int.TryParse(productIdPartOfTempIdAsString, out int productId);

        if (!isProductIdParsed) return new False();

        string imageNumberPartOfTempIdAsString = idOfImageAsString[(indexOfSeparatorOfTempId + 1)..];

        bool isImageNumberParsed = int.TryParse(imageNumberPartOfTempIdAsString, out int imageNumber);

        if (!isImageNumberParsed) return new False();

        return (productId, imageNumber);
    }

    public static string? GetTemporaryIdFromFileNameInfoAndContentType(ProductImageFileNameInfo productImageFileNameInfo, string imageContentType)
    {
        if (productImageFileNameInfo.ProductId <= 0
            || productImageFileNameInfo.ImageNumber <= 0
            || string.IsNullOrWhiteSpace(imageContentType)) return null;

        string? fileExtensionFromContentType = GetImageFileExtensionFromContentType(imageContentType);

        if (fileExtensionFromContentType is null) return null;

        return $"{productImageFileNameInfo.ProductId}-{productImageFileNameInfo.ImageNumber}.{fileExtensionFromContentType}";
    }

    public static string? GetImageFileNameFromImageData(int imageId, string imageContentType)
    {
        if (imageId < 0) return null;

        string? fileExtension = GetImageFileExtensionFromContentType(imageContentType);

        if (fileExtension is null) return null;

        return $"{imageId}.{fileExtension}";
    }

    public static (string? imageIdOrTemporaryId, string? imageFileExtension) GetImageDataFromImageFileName(string imageFileName)
    {
        int indexOfDot = imageFileName.IndexOf('.');

        if (indexOfDot < 0) return (null, null);

        string imageIdOrTempId = imageFileName[..indexOfDot];

        string imageFileExtension = imageFileName[(indexOfDot + 1)..];

        return (imageIdOrTempId, imageFileExtension);
    }

    public static string? GetImageFileExtensionFromContentType(string imageContentType)
    {
        string? fileExtension = imageContentType switch
        {
            "image/jpeg" => "jpeg",
            "image/jpg" => "jpg",
            "image/png" => "png",
            _ => null
        };

        if (fileExtension is null)
        {
            int indexOfSlash = imageContentType.IndexOf('/');

            if (indexOfSlash < 0) return null;

            return imageContentType[(indexOfSlash + 1)..];
        }

        return fileExtension;
    }
}