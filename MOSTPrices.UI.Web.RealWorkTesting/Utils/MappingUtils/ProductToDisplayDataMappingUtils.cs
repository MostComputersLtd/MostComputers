using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using MOSTComputers.Utils.ProductImageFileNameUtils;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageAndFileNameRelationsUtils;

using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;

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
            ImagesAndImageFileInfos = GetImageRelationsFromImagesAndImageFileInfos(product.Images, product.ImageFileNames?.ToList()),
            Properties = product.Properties?
                .Select(property => MapToProductPropertyDisplayData(property))
                .ToList(),

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
            Properties = productDisplayData.Properties?
                .Select(propertyData => MapToProductProperty(propertyData, productDisplayData.Id))
                .ToList() ?? new(),

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
                 .Select(x => new ProductPropertyDisplayData()
                 {
                     ProductCharacteristicId = x.ProductCharacteristicId,
                     DisplayOrder = x.DisplayOrder,
                     CustomDisplayOrderOnInsertOrUpdate = x.CustomDisplayOrderOnInsertOrUpdate,
                     Value = x.Value,
                     Characteristic = x.Characteristic,
                     XmlPlacement = x.XmlPlacement,
                 }).ToList(),

            ManifacturerId = productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,

            ProductWorkStatusesId = productDisplayData.ProductWorkStatusesId,
            ProductNewStatus = productDisplayData.ProductNewStatus,
            ProductXmlStatus = productDisplayData.ProductXmlStatus,
            ReadyForImageInsert = productDisplayData.ReadyForImageInsert,
        };
    }

    public static ProductUpdateWithoutImagesInDatabaseRequest MapToProductUpdateRequestWithoutImagesInDB(
        ProductDisplayData productDisplayData, List<ImageFileAndFileNameInfoUpsertRequest> imageFileUpsertRequests)
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
            SubCategoryId = productDisplayData.SubCategoryId,

            ImageFileAndFileNameInfoUpsertRequests = imageFileUpsertRequests,

            PropertyUpsertRequests = productDisplayData.Properties?.Select(
                propertyData => new LocalProductPropertyUpsertRequest()
                {
                    ProductCharacteristicId = propertyData.ProductCharacteristicId ?? 0,
                    CustomDisplayOrder = propertyData.CustomDisplayOrderOnInsertOrUpdate,
                    Value = propertyData.Value,
                    XmlPlacement = propertyData.XmlPlacement,
                }).ToList(),

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,
        };
    }

    public static ProductFullUpdateRequest GetProductFullUpdateRequest(ProductDisplayData productDisplayData)
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
            SubCategoryId = productDisplayData.SubCategoryId,

            ImageAndFileNameUpsertRequests = productDisplayData.ImagesAndImageFileInfos?
                .Select(x => GetImageAndImageFileNameUpsertRequest(x))
                .ToList(),

            PropertyUpsertRequests = productDisplayData.Properties?.Select(
                propertyData => new LocalProductPropertyUpsertRequest()
                {
                    ProductCharacteristicId = propertyData.ProductCharacteristicId ?? 0,
                    CustomDisplayOrder = propertyData.CustomDisplayOrderOnInsertOrUpdate,
                    Value = propertyData.Value,
                    XmlPlacement = propertyData.XmlPlacement,
                }).ToList(),

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,
        };
    }

    public static ProductCreateWithoutImagesInDatabaseRequest MapToProductCreateRequestWithoutImagesInDB(
        ProductDisplayData productDisplayData, List<ImageFileAndFileNameInfoUpsertRequest> imageFileAndFileNameInfoUpsertRequests)
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
            SubCategoryId = productDisplayData.SubCategoryId,

            Properties = productDisplayData.Properties?.Select(
                propertyData => new CurrentProductPropertyCreateRequest()
                {
                    ProductCharacteristicId = propertyData.ProductCharacteristicId ?? 0,
                    CustomDisplayOrder = propertyData.CustomDisplayOrderOnInsertOrUpdate,
                    Value = propertyData.Value,
                    XmlPlacement = propertyData.XmlPlacement,
                }).ToList(),

            ImageFileAndFileNameInfoUpsertRequests = imageFileAndFileNameInfoUpsertRequests,

            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            CategoryId = productDisplayData.CategoryId,
        };
    }

    public static ImageAndImageFileNameUpsertRequest GetImageAndImageFileNameUpsertRequest(ImageAndImageFileNameRelation imageAndImageFileNameRelation)
    {
        ProductImage? productImage = imageAndImageFileNameRelation.ProductImage;
        ProductImageFileNameInfo? productImageFileNameInfo = imageAndImageFileNameRelation.ProductImageFileNameInfo;

        string? imageContentType = productImage?.ImageContentType;

        if (imageContentType is null)
        {
            string? fileExtension = Path.GetExtension(productImageFileNameInfo?.FileName);

            if (fileExtension is not null)
            {
                imageContentType = GetImageContentTypeFromFileExtension(fileExtension);
            }
        }


        ProductImageUpsertRequest? productImageUpsertRequest = null;
        
        if (productImage is not null)
        {
            productImageUpsertRequest = new()
            {
                OriginalImageId = (productImage.Id > 0) ? productImage.Id : null,
                HtmlData = productImage.HtmlData,
            };
        }

        ProductImageFileNameInfoUpsertRequest? productImageFileNameInfoUpsertRequest = null;

        if (productImageFileNameInfo is not null)
        {
            int? newDisplayOrder = null;

            if (productImageFileNameInfo.DisplayOrder is not null
                && productImageFileNameInfo.DisplayOrder > 0)
            {
                newDisplayOrder = productImageFileNameInfo.DisplayOrder;
            }

            productImageFileNameInfoUpsertRequest = new()
            {
                OriginalImageNumber = (productImageFileNameInfo.ImageNumber > 0) ? productImageFileNameInfo.ImageNumber : null,
                NewDisplayOrder = newDisplayOrder,
                Active = productImageFileNameInfo.Active,
            };
        }

        return new()
        {
            ImageContentType = imageContentType ?? string.Empty,
            ImageData = productImage?.ImageData,
            ProductImageUpsertRequest = productImageUpsertRequest,
            ProductImageFileNameInfoUpsertRequest = productImageFileNameInfoUpsertRequest,
        };
    }

    public static ProductPropertyDisplayData MapToProductPropertyDisplayData(ProductProperty productProperty)
    {
        return new()
        {
            ProductCharacteristicId = productProperty.ProductCharacteristicId,
            Characteristic = productProperty.Characteristic,
            DisplayOrder = productProperty.DisplayOrder,
            Value = productProperty.Value,
            XmlPlacement = productProperty.XmlPlacement,
        };
    }

    public static ProductProperty MapToProductProperty(ProductPropertyDisplayData productPropertyData, int productId)
    {
        return new()
        {
            ProductId = productId,
            ProductCharacteristicId = productPropertyData.ProductCharacteristicId,
            Characteristic = productPropertyData.Characteristic,
            DisplayOrder = productPropertyData.DisplayOrder,
            Value = productPropertyData.Value,
            XmlPlacement = productPropertyData.XmlPlacement,
        };
    }
}