using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.ProductRegister.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.Utils.ProductImageFileNameUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageAndFileNameRelationsUtils;

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
            PromotionId = product.PromotionId,
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

            CategoryId = product.CategoryId,
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
            Images = productDisplayData.ImagesAndImageFileInfos?
                .Select(x => x.ProductImage)
                .Where(x => x != null)!
                .ToList<ProductImage>(),
            ImageFileNames = productDisplayData.ImagesAndImageFileInfos?
                .Select(x => x.ProductImageFileNameInfo)
                .Where(x => x != null)!
                .ToList<ProductImageFileNameInfo>(),
            Properties = productDisplayData.Properties?.ToList() ?? new(),

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,
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
                bool imageIsNull = x.ProductImage is not null;
                bool imageFileNameIsNull = x.ProductImageFileNameInfo is not null;

                return new ImageAndImageFileNameRelation(
                    imageIsNull ?
                        new ProductImage()
                        {
                            Id = x.ProductImage!.Id,
                            ProductId = x.ProductImage.ProductId,
                            ImageData = x.ProductImage.ImageData,
                            ImageContentType = x.ProductImage.ImageContentType,
                            HtmlData = x.ProductImage.HtmlData,
                            DateModified = x.ProductImage.DateModified,
                        }
                    : null,
                    imageFileNameIsNull ?
                        new ProductImageFileNameInfo()
                        {
                            ProductId = x.ProductImageFileNameInfo!.ProductId,
                            ImageNumber = x.ProductImageFileNameInfo.ImageNumber,
                            DisplayOrder = x.ProductImageFileNameInfo.DisplayOrder,
                            Active = x.ProductImageFileNameInfo.Active,
                            FileName = x.ProductImageFileNameInfo.FileName,
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

    public static ProductUpdateWithoutImagesInDatabaseRequest MapToProductUpdateRequestWithoutImagesInDB(ProductDisplayData productDisplayData)
    {
        return new()
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

            ImagesAndFileNames = productDisplayData.ImagesAndImageFileInfos?.ToList(),

            Properties = productDisplayData.Properties?.ToList(),

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,
        };
    }

    public static ProductFullUpdateRequest MapToProductFullUpdateRequest(ProductDisplayData productDisplayData)
    {
        return new()
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

            ImagesAndFileNames = productDisplayData.ImagesAndImageFileInfos?.ToList(),

            Properties = productDisplayData.Properties?.ToList(),

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,
        };
    }

    public static ProductCreateWithoutImagesInDatabaseRequest MapToProductCreateRequestWithoutImagesInDB(ProductDisplayData productDisplayData)
    {
        return new()
        {
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

            ImagesAndFileNames = productDisplayData.ImagesAndImageFileInfos?.ToList(),

            Properties = productDisplayData.Properties?.ToList(),

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,
        };
    }
}