using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models;
using System.Text;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using System.Numerics;

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

    internal static readonly ProductCreateRequest ValidProductCreateRequestWithNoImages = new()
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
            
        },

        CategoryID = 7,
        ManifacturerId = 12,
        SubCategoryId = null,
    };

    internal static ProductCreateRequest GetValidProductCreateRequest(int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        return new()
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
                new()
                {
                    ImageData = LocalTestImageData,
                    ImageFileExtension = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },

                new()
                {
                    ImageData = LocalTestImageData,
                    ImageFileExtension = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
            {
                new() { FileName = "1.png", DisplayOrder = 1 },
                new() { FileName = "2.png", DisplayOrder = 2 },
                new() { FileName = "3.png", DisplayOrder = 3 },
                new() { FileName = "4.png", DisplayOrder = 4 }
            },

            CategoryID = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };
    }

    internal static ProductUpdateRequest GetValidProductUpdateRequest(int id, int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        return new()
        {
            Id = id,
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
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            },
            Images = new List<CurrentProductImageUpdateRequest>()
            {
                new()
                {
                    ImageData = LocalTestImageData,
                    ImageFileExtension = "image/png",
                    XML = "<data></data>",
                    DateModified = DateTime.Now,
                },

                new()
                {
                    ImageData = LocalTestImageData,
                    ImageFileExtension = "image/png",
                    XML = "<data></data>",
                    DateModified = DateTime.Now,
                },
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoUpdateRequest>()
            {
                new() { FileName = "1 - 2", DisplayOrder = 1, NewDisplayOrder = 2 },
                new() { FileName = "2 - 1", DisplayOrder = 2, NewDisplayOrder = 1 },
                new() { FileName = "2 - 3", DisplayOrder = 2, NewDisplayOrder = 3 },
                new() { FileName = "1 - 3", DisplayOrder = 1, NewDisplayOrder = 3 },
            },

            CategoryID = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };
    }

    internal static ProductUpdateRequest GetValidProductUpdateRequestWithNoImages(int id, int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        return new()
        {
            Id = id,
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
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            },
            Images = new List<CurrentProductImageUpdateRequest>()
            {
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoUpdateRequest>()
            {
            },

            CategoryID = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };
    }

    internal static ProductCreateRequest GetValidProductCreateRequestUsingRandomData(int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        int? additionalWarantyTermMonths = _randomWarranties[Random.Shared.Next(0, _randomWarranties.Count - 1)];

        return new()
        {
            Name = RandomString(Random.Shared.Next(10, 29)),
            AdditionalWarrantyTermMonths = additionalWarantyTermMonths,
            AdditionalWarrantyPrice = (additionalWarantyTermMonths is not null) ? 3.00M : 0.00M,
            StandardWarrantyPrice = "0.00",
            StandardWarrantyTermMonths = _randomWarranties[Random.Shared.Next(0, _randomWarranties.Count - 1)],
            DisplayOrder = Random.Shared.Next(1_000, 50_000_000),
            Status = (ProductStatusEnum)Random.Shared.Next(0, 2),
            PlShow = Random.Shared.Next(0, 1),
            Price1 = Random.Shared.NextInt64(100, 900_000) / 100,
            DisplayPrice = Random.Shared.NextInt64(100, 900_000) / 100,
            Price3 = Random.Shared.NextInt64(100, 900_000) / 100,
            Currency = _randomCurrencies[Random.Shared.Next(0, _randomCurrencies.Count - 1)],
            RowGuid = Guid.NewGuid(),
            Promotionid = null,
            PromRid = null,
            PromotionPictureId = null,
            PromotionExpireDate = null,
            AlertPictureId = null,
            AlertExpireDate = null,
            PriceListDescription = null,
            PartNumber1 = RandomString(Random.Shared.Next(1, 15)),
            PartNumber2 = RandomString(Random.Shared.Next(1, 10)),
            SearchString = RandomString(Random.Shared.Next(10, 35)),

            Properties = new()
            {
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            },
            Images = new List<CurrentProductImageCreateRequest>()
            {
                new()
                {
                    ImageData = LocalTestImageData,
                    ImageFileExtension = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },

                new()
                {
                    ImageData = LocalTestImageData,
                    ImageFileExtension = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
            {
                new() { FileName = "20143.png", DisplayOrder = 1 },
                new() { FileName = "20144.png", DisplayOrder = 2 }
            },

            CategoryID = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };
    }

    private static readonly List<int?> _randomWarranties = new() { null, 12, 18, 36, 45, 72, 144 };

    private static readonly List<CurrencyEnum> _randomCurrencies = new() { CurrencyEnum.USD, CurrencyEnum.BGN, CurrencyEnum.EUR };

    public static string RandomString(int length)
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(allowedChars, length)
        .Select(s => s[Random.Shared.Next(s.Length - 1)])
        .ToArray());
    }

    internal static readonly byte[] LocalTestImageData = LoadLocalTestImageData();

    private static byte[] LoadLocalTestImageData()
    {
        string pictureFile = File.ReadAllText("C:/Users/Dani/source/repos/MOSTComputers/MOSTComputers.Services.ProductRegister.Tests.Integration/Images/RND_PC_IMG.png");

        string dataToGetRidOf = "data:image/png;base64,";

        return Encoding.ASCII.GetBytes(pictureFile.Replace(dataToGetRidOf, string.Empty));
    }

    internal static ProductCreateRequest GetValidProductCreateRequestWithNoImages(int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        return new()
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
            },

            CategoryID = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };
    }

    internal static readonly ServiceCategoryCreateRequest ValidCategoryCreateRequest = new()
    {
        Description = "description",
        DisplayOrder = 13123,
        ProductsUpdateCounter = 0,
        ParentCategoryId = null
    };

    internal static ProductCharacteristicCreateRequest GetValidCharacteristicCreateRequest(
        int categoryId,
        string name = "Name",
        ProductCharacteristicTypeEnum type = ProductCharacteristicTypeEnum.ProductCharacteristic)
    {
        return new()
        {
            CategoryId = categoryId,
            Name = name,
            Meaning = "Name of the object",
            PKUserId = 91,
            KWPrCh = type,
            DisplayOrder = 12,
            Active = true
        };
    }

    internal static ServiceProductImageCreateRequest GetCreateRequestWithImageData(int productId)
    {
        return new()
        {
            ImageData = LocalTestImageData,
            ImageFileExtension = "image/png",
            ProductId = productId,
            HtmlData = "<data></data>",
        };
    }

    internal static ServiceProductFirstImageCreateRequest GetFirstImageCreateRequestWithImageData(int productId)
    {
        return new()
        {
            ImageData = LocalTestImageData,
            ImageFileExtension = "image/png",
            ProductId = productId,
            XML = "<data></data>",
        };
    }

    internal static ServicePromotionCreateRequest GetValidPromotionCreateRequest(int productId)
    {
        return new()
        {
            Name = "2023-Q3",
            Source = 1,
            Type = 2,
            Status = 3,
            SPOID = null,
            DiscountUSD = 4.99M,
            DiscountEUR = 4.99M,
            Active = false,
            StartDate = DateTime.Today.AddDays(-5),
            ExpirationDate = DateTime.Today.AddDays(7),
            MinimumQuantity = 2,
            MaximumQuantity = 120,
            QuantityIncrement = 123,

            RequiredProductIds = null,

            ExpQuantity = null,
            SoldQuantity = 1782,
            Consignation = 1,
            Points = 2,
            TimeStamp = null,

            ProductId = productId,
            CampaignId = null,
            RegistrationId = null,
            PromotionVisualizationId = null,
        };
    }

    internal static bool ComparaNumberLists<T>(List<T>? numberList1, List<T>? numberList2, bool needToOrderItems = false)
       where T : INumber<T>
    {
        if (numberList1 is null && numberList2 is null) return true;

        if (numberList1 is null || numberList2 is null) return false;

        if (numberList1.Count != numberList2.Count) return false;

        if (needToOrderItems)
        {
            numberList1 = numberList1.Order().ToList();
            numberList2 = numberList2.Order().ToList();
        }

        for (int i = 0; i < numberList1.Count; i++)
        {
            T number1 = numberList1[i];
            T number2 = numberList2[i];

            if (number1 != number2) return false;
        }

        return true;
    }

    internal static bool DeleteProducts(this IProductService productService, params uint[] productIds)
    {
        foreach (var productId in productIds)
        {
            bool success = productService.Delete(productId);

            if (!success) return false;
        }

        return true;
    }

    internal static bool DeleteRangeCategories(this ICategoryService categoryService, params uint[] ids)
    {
        foreach (uint id in ids)
        {
            bool success = categoryService.Delete(id);

            if (!success) return false;
        }

        return true;
    }

    internal static bool DeleteRangeCharacteristics(this IProductCharacteristicService productCharacteristicService, params uint[] ids)
    {
        foreach (uint id in ids)
        {
            bool success = productCharacteristicService.Delete(id);

            if (!success) return false;
        }

        return true;
    }

    internal static bool DeleteRangeProductStatusesByProductIds(this IProductStatusesService productStatusesService, params uint[] productIds)
    {
        foreach (var productId in productIds)
        {
            bool success = productStatusesService.DeleteByProductId(productId);

            if (!success) return false;
        }

        return true;
    }

    internal static bool CompareDataInByteArrays(byte[]? bytes1, byte[]? bytes2)
    {
        if (bytes1 is null && bytes2 is null) return true;

        if (bytes1 is null || bytes2 is null) return false;

        if (bytes1.Length != bytes2.Length) return false;

        for (int i = 0; i < bytes1.Length; i++)
        {
            byte item1 = bytes1[i];
            byte item2 = bytes2[i];

            if (item1 != item2) return false;
        }

        return true;
    }
}