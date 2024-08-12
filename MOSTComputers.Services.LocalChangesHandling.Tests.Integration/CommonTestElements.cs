using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.LocalChangesHandling.Tests.Integration;

internal static class CommonTestElements
{
    internal const string tableChangeNameOfProductsTable = "MOSTPRices";
    internal const string tableChangeNameOfFirstImagesTable = "Images";
    internal const string tableChangeNameOfAllImagesTable = "ImagesAll";
    internal const string tableNameOfProductCharacteristicsTable = "ProductKeyword";
    internal const string tableNameOfCategoriesTable = "Categories";
    internal const string tableNameOfManifacturersTable = "Manufacturer";
    internal const string tableNameOfPromotionsTable = "Promotions";

    internal static string GetFullTableName(string tableName)
    {
        return "dbo." + tableName;
    }

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
        PromotionId = null,
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

        CategoryId = 7,
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
        PromotionId = null,
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

        CategoryId = 7,
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
            PromotionId = null,
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
                    ImageContentType = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },

                new()
                {
                    ImageData = LocalTestImageData,
                    ImageContentType = "image/png",
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

            CategoryId = categoryId,
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
                    ImageContentType = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },

                new()
                {
                    ImageData = LocalTestImageData,
                    ImageContentType = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoUpdateRequest>()
            {
                new() { FileName = "1 - 2", ImageNumber = 1, NewDisplayOrder = 2 },
                new() { FileName = "2 - 1", ImageNumber = 2, NewDisplayOrder = 1 },
                new() { FileName = "2 - 3", ImageNumber = 2, NewDisplayOrder = 3 },
                new() { FileName = "1 - 3", ImageNumber = 1, NewDisplayOrder = 3 },
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
            PromotionId = null,
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
                    ImageContentType = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },

                new()
                {
                    ImageData = LocalTestImageData,
                    ImageContentType = "image/png",
                    HtmlData = "<data></data>",
                    DateModified = DateTime.Now,
                },
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
            {
                new() { FileName = "20143.png", DisplayOrder = 1 },
                new() { FileName = "20144.png", DisplayOrder = 2 }
            },

            CategoryId = categoryId,
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
            PromotionId = null,
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

            CategoryId = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };
    }
}