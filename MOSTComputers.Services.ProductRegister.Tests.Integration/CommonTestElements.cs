using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

internal static class CommonTestElements
{
    internal static readonly ProductCreateRequest ValidProductCreateRequest = new()
    {
        Name = "Product name",
        AdditionalWarrantyPrice = 3.00M,
        AdditionalWarrantyTermMonths = 36,
        StandardWarrantyPrice = "0.00",
        StandardWarrantyTermMonths = 36,
        DisplayOrder = 12324,
        Status = ProductStatusEnum.Call,
        PlShow = 0,
        Price1 = 123.4M,
        DisplayPrice = 123.99M,
        Price3 = 122.5M,
        Currency = CurrencyEnum.EUR,
        RowGuid = Guid.NewGuid(),
        Promotionid = null,
        PromRid = null,
        PromotionPictureId = null,
        PromotionExpireDate = null,
        AlertPictureId = null,
        AlertExpireDate = null,
        PriceListDescription = null,
        PartNumber1 = "DF FKD@$ 343432 wdwfc",
        PartNumber2 = "123123/DD",
        SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

        Properties = new()
        {
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
        },
        Images = new List<CurrentProductImageCreateRequest>()
        {
        },
        ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
        {
            new() { FileName = "20143.png", DisplayOrder = 1 },
            new() { FileName = "20144.png", DisplayOrder = 2 }
        },

        CategoryID = 7,
        ManifacturerId = 12,
        SubCategoryId = null,
    };

    internal static readonly ServiceCategoryCreateRequest ValidCategoryCreateRequest = new()
    {
        Description = "description",
        DisplayOrder = 13123,
        ProductsUpdateCounter = 0,
        ParentCategoryId = null
    };
}