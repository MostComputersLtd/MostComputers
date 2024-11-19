using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using System.Numerics;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Models.Requests.Promotion;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

internal static class CommonTestElements
{
    internal const int UseRequiredValuePlaceholder = -1366748;

    internal static readonly byte[] LocalTestImageData = LoadLocalTestImageData();

    private static byte[] LoadLocalTestImageData()
    {
        byte[] pictureData = File.ReadAllBytes(Startup.TestingImageFileFullPath);

        return pictureData;
    }

    internal static byte[] GetImageDataEncoded(byte[] imageData)
    {
        using Image image = Image.Load(imageData);

        using MemoryStream memoryStream = new();

        image.Save(memoryStream, new PngEncoder());

        return memoryStream.ToArray();
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
        PriceListDescription = "dddddddd",
        PartNumber1 = "DF FKD@$ 343432 wdwfc",
        PartNumber2 = "123123/DD",
        SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

        Properties = new()
        {
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
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
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
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
        ProductCreateRequest productCreateRequest = new()
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

            Properties = new(),
            Images = new List<CurrentProductImageCreateRequest>()
            {
                new()
                {
                    ImageData = LocalTestImageData,
                    ImageContentType = "image/png",
                    HtmlData = null,
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

        if (categoryId == 7)
        {
            productCreateRequest.Properties = new()
            {
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            };
        }

        return productCreateRequest;
    }

    internal static ProductUpdateRequest GetValidProductUpdateRequest(int id, int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        ProductUpdateRequest productUpdateRequest = new()
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

            Properties = new(),

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

            CategoryId = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };

        if (categoryId == 7)
        {
            productUpdateRequest.Properties = new()
            {
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 404, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            };
        }

        return productUpdateRequest;
    }

    internal static ProductUpdateRequest GetValidProductUpdateRequestWithNoImages(
        int id, int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        ProductUpdateRequest productUpdateRequest = new()
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

            Properties = new(),

            Images = new List<CurrentProductImageUpdateRequest>()
            {
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoUpdateRequest>()
            {
            },

            CategoryId = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };

        if (categoryId == 7)
        {
            productUpdateRequest.Properties = new()
            {
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 404, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyUpdateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            };
        }

        return productUpdateRequest;
    }

    internal static ProductFullUpdateRequest GetValidProductFullUpdateRequestWithNoImages(
        int id, int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        ProductFullUpdateRequest productFullUpdateRequest = new()
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

            PropertyUpsertRequests = new(),
            ImageAndFileNameUpsertRequests = new(),

            CategoryId = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
        };

        if (categoryId == 7)
        {
            productFullUpdateRequest.PropertyUpsertRequests = new()
            {
                new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.AtTheTop },
            };
        }

        return productFullUpdateRequest;
    }

    internal static ProductCreateRequest GetValidProductCreateRequestUsingRandomData(int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        int? additionalWarantyTermMonths = _randomWarranties[Random.Shared.Next(0, _randomWarranties.Count)];

        ProductCreateRequest productCreateRequest = new()
        {
            Name = RandomString(Random.Shared.Next(10, 29)),
            AdditionalWarrantyTermMonths = additionalWarantyTermMonths,
            AdditionalWarrantyPrice = (additionalWarantyTermMonths is not null) ? 3.00M : 0.00M,
            StandardWarrantyPrice = "0.00",
            StandardWarrantyTermMonths = _randomWarranties[Random.Shared.Next(0, _randomWarranties.Count)],
            DisplayOrder = Random.Shared.Next(1_000, 50_000_000),
            Status = (ProductStatusEnum)Random.Shared.Next(0, 3),
            PlShow = Random.Shared.Next(0, 2),
            Price1 = Random.Shared.NextInt64(100, 900_000) / 100,
            DisplayPrice = Random.Shared.NextInt64(100, 900_000) / 100,
            Price3 = Random.Shared.NextInt64(100, 900_000) / 100,
            Currency = _randomCurrencies[Random.Shared.Next(0, _randomCurrencies.Count)],
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

            Properties = new(),

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

        if (categoryId == 7)
        {
            productCreateRequest.Properties = new()
            {
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            };
        }

        return productCreateRequest;
    }

    private static readonly List<int?> _randomWarranties = new() { null, 12, 18, 36, 45, 72, 144 };

    private static readonly List<CurrencyEnum> _randomCurrencies = new() { CurrencyEnum.USD, CurrencyEnum.BGN, CurrencyEnum.EUR };

    public static string RandomString(int length)
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(Enumerable.Repeat(allowedChars, length)
        .Select(s => s[Random.Shared.Next(s.Length)])
        .ToArray());
    }

    internal static ProductCreateRequest GetValidProductCreateRequestWithNoImages(
        int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        ProductCreateRequest productCreateRequest = new()
        {
            CategoryId = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
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

            Properties = new(),

            Images = new List<CurrentProductImageCreateRequest>()
            {
            },
            ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
            {
            },
        };

        if (categoryId == 7)
        {
            productCreateRequest.Properties = new()
            {
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            };
        }

        return productCreateRequest;
    }

    public static ProductCreateWithoutImagesInDatabaseRequest GetValidProductCreateWithoutImagesInDatabaseRequest(
        int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        ProductCreateWithoutImagesInDatabaseRequest productCreateWithoutImagesInDatabaseRequest = new()
        {
            CategoryId = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
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
            PriceListDescription = "dddddddd",
            PartNumber1 = "DF FKD@$ 343432 wdwfc",
            PartNumber2 = "123123/DD",
            SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

            Properties = new(),

            ImageFileAndFileNameInfoUpsertRequests = new()
            {
                new ImageFileAndFileNameInfoUpsertRequest()
                {
                    ImageContentType = "image/png",
                    ImageData = LocalTestImageData,
                    OldFileName = null,
                    RelatedImageId = null,
                    CustomFileNameWithoutExtension = null,
                    DisplayOrder = 1,
                    Active = true,
                },

                new ImageFileAndFileNameInfoUpsertRequest()
                {
                    ImageContentType = "image/png",
                    ImageData = LocalTestImageData,
                    OldFileName = null,
                    RelatedImageId = null,
                    CustomFileNameWithoutExtension = null,
                    DisplayOrder = 3,
                    Active = false,
                },

                new ImageFileAndFileNameInfoUpsertRequest()
                {
                    ImageContentType = "image/png",
                    ImageData = LocalTestImageData,
                    OldFileName = null,
                    RelatedImageId = null,
                    CustomFileNameWithoutExtension = "custom_file_name",
                    DisplayOrder = 2,
                    Active = false,
                },
            }
        };

        if (categoryId == 7)
        {
            productCreateWithoutImagesInDatabaseRequest.Properties = new()
            {
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            };
        }

        return productCreateWithoutImagesInDatabaseRequest;
    }

    public static ProductUpdateWithoutImagesInDatabaseRequest GetValidProductUpdateWithoutImagesInDatabaseRequest(
        int productId, int? categoryId = 7, short? manifacturerId = 12, int? subCategoryId = null)
    {
        ProductUpdateWithoutImagesInDatabaseRequest productUpdateWithoutImagesInDatabaseRequest = new()
        {
            Id = productId,
            CategoryId = categoryId,
            ManifacturerId = manifacturerId,
            SubCategoryId = subCategoryId,
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
            PriceListDescription = "dddddddd",
            PartNumber1 = "DF FKD@$ 343432 wdwfc",
            PartNumber2 = "123123/DD",
            SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

            PropertyUpsertRequests = new(),

            ImageFileAndFileNameInfoUpsertRequests = new()
            {
                new ImageFileAndFileNameInfoUpsertRequest()
                {
                    ImageContentType = "image/png",
                    ImageData = LocalTestImageData,
                    OldFileName = null,
                    RelatedImageId = null,
                    CustomFileNameWithoutExtension = null,
                    DisplayOrder = 1,
                    Active = true,
                },

                new ImageFileAndFileNameInfoUpsertRequest()
                {
                    ImageContentType = "image/png",
                    ImageData = LocalTestImageData,
                    OldFileName = null,
                    RelatedImageId = null,
                    CustomFileNameWithoutExtension = null,
                    DisplayOrder = 3,
                    Active = false,
                },

                new ImageFileAndFileNameInfoUpsertRequest()
                {
                    ImageContentType = "image/png",
                    ImageData = LocalTestImageData,
                    OldFileName = null,
                    RelatedImageId = null,
                    CustomFileNameWithoutExtension = "custom_file_name",
                    DisplayOrder = 2,
                    Active = false,
                },
            }
        };

        if (categoryId == 7)
        {
            productUpdateWithoutImagesInDatabaseRequest.PropertyUpsertRequests = new()
            {
                new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            };
        }

        return productUpdateWithoutImagesInDatabaseRequest;
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

    internal static ServiceProductImageCreateRequest GetValidImageCreateRequestWithImageData(int productId)
    {
        return new()
        {
            ImageData = LocalTestImageData,
            ImageContentType = "image/png",
            ProductId = productId,
            HtmlData = "<data></data>",
        };
    }

    internal static ServiceProductFirstImageCreateRequest GetValidFirstImageCreateRequestWithImageData(int productId)
    {
        return new()
        {
            ImageData = LocalTestImageData,
            ImageContentType = "image/png",
            ProductId = productId,
            HtmlData = "<data></data>",
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

    internal static bool DeleteProducts(this IProductService productService, params int[] productIds)
    {
        foreach (int productId in productIds)
        {
            bool success = productService.Delete(productId);

            if (!success) return false;
        }

        return true;
    }

    internal static bool DeleteRangeCategories(this ICategoryService categoryService, params int[] ids)
    {
        foreach (int id in ids)
        {
            bool success = categoryService.Delete(id);

            if (!success) return false;
        }

        return true;
    }

    internal static bool DeleteRangeCharacteristics(this IProductCharacteristicService productCharacteristicService, params int[] ids)
    {
        foreach (int id in ids)
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
            bool success = productStatusesService.DeleteByProductId((int)productId);

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