using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;

namespace MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils;

internal static class ProductMappingUtils
{
    internal static (ProductUpdateRequest, ProductWorkStatuses?) MapProductDisplayDataToUpdateRequest(ProductDisplayData productDisplayData)
    {
        ProductUpdateRequest productUpdateRequest = new()
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

            CategoryID = productDisplayData.CategoryId,
            ManifacturerId = (short?)productDisplayData.ManifacturerId,
            SubCategoryId = productDisplayData.SubCategoryId,
        };

        if (productDisplayData.ProductWorkStatusesId <= 0) return (productUpdateRequest, null);

        return (productUpdateRequest,
        new ProductWorkStatuses()
        {
            Id = productDisplayData.ProductWorkStatusesId ?? -1,
            ProductId = productDisplayData.Id,
            ProductNewStatus = productDisplayData.ProductNewStatus ?? ProductNewStatusEnum.New,
            ProductXmlStatus = productDisplayData.ProductXmlStatus ?? ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = productDisplayData.ReadyForImageInsert ?? false,
        });
    }

    internal static void MapProductToCreateRequest(ProductCreateRequest productCreateRequest, Product otherProductData)
    {
        productCreateRequest.Name = otherProductData.Name;
        productCreateRequest.AdditionalWarrantyPrice = otherProductData.AdditionalWarrantyPrice;
        productCreateRequest.AdditionalWarrantyTermMonths = otherProductData.AdditionalWarrantyTermMonths;
        productCreateRequest.StandardWarrantyPrice = otherProductData.StandardWarrantyPrice;
        productCreateRequest.StandardWarrantyTermMonths = otherProductData.StandardWarrantyTermMonths;
        productCreateRequest.DisplayOrder = otherProductData.DisplayOrder;
        productCreateRequest.Status = otherProductData.Status;
        productCreateRequest.PlShow = otherProductData.PlShow;
        productCreateRequest.DisplayPrice = otherProductData.Price;
        productCreateRequest.Currency = otherProductData.Currency;
        productCreateRequest.RowGuid = otherProductData.RowGuid;
        productCreateRequest.Promotionid = otherProductData.Promotionid;
        productCreateRequest.PromRid = otherProductData.PromRid;
        productCreateRequest.PromotionPictureId = otherProductData.PromotionPictureId;
        productCreateRequest.PromotionExpireDate = otherProductData.PromotionExpireDate;
        productCreateRequest.AlertPictureId = otherProductData.AlertPictureId;
        productCreateRequest.AlertExpireDate = otherProductData.AlertExpireDate;
        productCreateRequest.PriceListDescription = otherProductData.PriceListDescription;
        productCreateRequest.PartNumber1 = otherProductData.PartNumber1;
        productCreateRequest.PartNumber2 = otherProductData.PartNumber2;
        productCreateRequest.SearchString = otherProductData.SearchString;

        productCreateRequest.CategoryID = otherProductData.CategoryID;
        productCreateRequest.ManifacturerId = otherProductData.ManifacturerId;
        productCreateRequest.SubCategoryId = otherProductData.SubCategoryId;
    }

    internal static ProductCreateRequest MapProductToCreateRequest(Product otherProductData)
    {
        return new()
        {
            Name = otherProductData.Name,
            AdditionalWarrantyPrice = otherProductData.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = otherProductData.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = otherProductData.StandardWarrantyPrice,
            StandardWarrantyTermMonths = otherProductData.StandardWarrantyTermMonths,
            DisplayOrder = otherProductData.DisplayOrder,
            Status = otherProductData.Status,
            PlShow = otherProductData.PlShow,
            DisplayPrice = otherProductData.Price,
            Currency = otherProductData.Currency,
            RowGuid = otherProductData.RowGuid,
            Promotionid = otherProductData.Promotionid,
            PromRid = otherProductData.PromRid,
            PromotionPictureId = otherProductData.PromotionPictureId,
            PromotionExpireDate = otherProductData.PromotionExpireDate,
            AlertPictureId = otherProductData.AlertPictureId,
            AlertExpireDate = otherProductData.AlertExpireDate,
            PriceListDescription = otherProductData.PriceListDescription,
            PartNumber1 = otherProductData.PartNumber1,
            PartNumber2 = otherProductData.PartNumber2,
            SearchString = otherProductData.SearchString,

            CategoryID = otherProductData.CategoryID,
            ManifacturerId = otherProductData.ManifacturerId,
            SubCategoryId = otherProductData.SubCategoryId,
        };
    }

    internal static ProductCreateRequest MapProductToCreateRequestWithImagesAndProps(Product product)
    {
        return new()
        {
            Name = product.Name,
            AdditionalWarrantyPrice = product.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = product.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = product.StandardWarrantyPrice,
            StandardWarrantyTermMonths = product.StandardWarrantyTermMonths,
            DisplayOrder = product.DisplayOrder,
            Status = product.Status,
            PlShow = product.PlShow,
            DisplayPrice = product.Price,
            Currency = product.Currency,
            RowGuid = product.RowGuid,
            Promotionid = product.Promotionid,
            PromRid = product.PromRid,
            PromotionPictureId = product.PromotionPictureId,
            PromotionExpireDate = product.PromotionExpireDate,
            AlertPictureId = product.AlertPictureId,
            AlertExpireDate = product.AlertExpireDate,
            PriceListDescription = product.PriceListDescription,
            PartNumber1 = product.PartNumber1,
            PartNumber2 = product.PartNumber2,
            SearchString = product.SearchString,

            Images = product.Images?.ConvertAll(x => new CurrentProductImageCreateRequest()
            {
                ImageData = x.ImageData,
                ImageFileExtension = x.ImageFileExtension,
                DateModified = x.DateModified,
                HtmlData = x.HtmlData,
            }),

            ImageFileNames = product.ImageFileNames?.ConvertAll(x => new CurrentProductImageFileNameInfoCreateRequest()
            {
                FileName = x.FileName,
                Active = x.Active,
                DisplayOrder = x.DisplayOrder,
            }),

            Properties = product.Properties?.ConvertAll(x => new CurrentProductPropertyCreateRequest()
            {
                ProductCharacteristicId = x.ProductCharacteristicId,
                Value = x.Value,
                DisplayOrder = x.DisplayOrder,
                XmlPlacement = x.XmlPlacement,
            }),

            CategoryID = product.CategoryID,
            ManifacturerId = product.ManifacturerId,
            SubCategoryId = product.SubCategoryId,
        };
    }
}