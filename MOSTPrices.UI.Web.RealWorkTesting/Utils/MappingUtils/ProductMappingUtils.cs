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
        productCreateRequest.PromotionId = otherProductData.Promotionid;
        productCreateRequest.PromRid = otherProductData.PromRid;
        productCreateRequest.PromotionPictureId = otherProductData.PromotionPictureId;
        productCreateRequest.PromotionExpireDate = otherProductData.PromotionExpireDate;
        productCreateRequest.AlertPictureId = otherProductData.AlertPictureId;
        productCreateRequest.AlertExpireDate = otherProductData.AlertExpireDate;
        productCreateRequest.PriceListDescription = otherProductData.PriceListDescription;
        productCreateRequest.PartNumber1 = otherProductData.PartNumber1;
        productCreateRequest.PartNumber2 = otherProductData.PartNumber2;
        productCreateRequest.SearchString = otherProductData.SearchString;

        productCreateRequest.CategoryId = otherProductData.CategoryID;
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
            PromotionId = otherProductData.Promotionid,
            PromRid = otherProductData.PromRid,
            PromotionPictureId = otherProductData.PromotionPictureId,
            PromotionExpireDate = otherProductData.PromotionExpireDate,
            AlertPictureId = otherProductData.AlertPictureId,
            AlertExpireDate = otherProductData.AlertExpireDate,
            PriceListDescription = otherProductData.PriceListDescription,
            PartNumber1 = otherProductData.PartNumber1,
            PartNumber2 = otherProductData.PartNumber2,
            SearchString = otherProductData.SearchString,

            CategoryId = otherProductData.CategoryID,
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

            Images = product.Images?.ConvertAll(x => new CurrentProductImageCreateRequest()
            {
                ImageData = x.ImageData,
                ImageContentType = x.ImageContentType,
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

            CategoryId = product.CategoryID,
            ManifacturerId = product.ManifacturerId,
            SubCategoryId = product.SubCategoryId,
        };
    }

    internal static ProductUpdateRequest MapToUpdateRequestWithoutImagesAndProps(Product productToUpdate)
    {
        return new()
        {
            Id = productToUpdate.Id,
            Name = productToUpdate.Name,
            AdditionalWarrantyPrice = productToUpdate.AdditionalWarrantyPrice,
            AdditionalWarrantyTermMonths = productToUpdate.AdditionalWarrantyTermMonths,
            StandardWarrantyPrice = productToUpdate.StandardWarrantyPrice,
            StandardWarrantyTermMonths = productToUpdate.StandardWarrantyTermMonths,
            DisplayOrder = productToUpdate.DisplayOrder,
            Status = productToUpdate.Status,
            PlShow = productToUpdate.PlShow,
            DisplayPrice = productToUpdate.Price,
            Currency = productToUpdate.Currency,
            RowGuid = productToUpdate.RowGuid,
            Promotionid = productToUpdate.Promotionid,
            PromRid = productToUpdate.PromRid,
            PromotionPictureId = productToUpdate.PromotionPictureId,
            PromotionExpireDate = productToUpdate.PromotionExpireDate,
            AlertPictureId = productToUpdate.AlertPictureId,
            AlertExpireDate = productToUpdate.AlertExpireDate,
            PriceListDescription = productToUpdate.PriceListDescription,
            PartNumber1 = productToUpdate.PartNumber1,
            PartNumber2 = productToUpdate.PartNumber2,
            SearchString = productToUpdate.SearchString,

            Properties = new(),
            Images = new(),
            ImageFileNames = new(),

            CategoryID = productToUpdate.CategoryID,
            ManifacturerId = productToUpdate.ManifacturerId,
            SubCategoryId = productToUpdate.SubCategoryId,
        };
    }
}