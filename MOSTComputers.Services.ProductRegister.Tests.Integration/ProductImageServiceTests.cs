using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductImageServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductImageServiceTests(
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductService productService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productService = productService;
    }

    private const int _useRequiredValue = -100;

    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductService _productService;

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    private static ServiceProductFirstImageCreateRequest GetInvalidFirstImageCreateRequest(int productId) => new()
    {
        ImageData = LocalTestImageData,
        ImageContentType = null,
        ProductId = productId,
        HtmlData = "<data></data>",
    };

    private static ServiceProductImageCreateRequest GetInvalidImageCreateRequest(int productId) => new()
    {
        ImageData = LocalTestImageData,
        ImageContentType = null,
        ProductId = productId,
        HtmlData = "<data></data>",
    };

    [Fact]
    public void GetAllFirstImagesForAllProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId2 = productInsertResult2.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = GetFirstImageCreateRequestWithImageData(productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllFirstImagesForAllProducts();

        Assert.NotEmpty(productFirstImages);

        Assert.Contains(productFirstImages, x =>
        x.Id == productId1
        && CompareDataInByteArrays(x.ImageData, createRequest1.ImageData)
        && x.ImageContentType == createRequest1.ImageContentType);
    }

    [Fact]
    public void GetAllFirstImagesForAllProducts_ShouldFail_WhenInsertsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId2 = productInsertResult2.AsT0;

        ServiceProductFirstImageCreateRequest invalidCreateRequest = GetInvalidFirstImageCreateRequest(productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(invalidCreateRequest);

        Assert.True(insertResult1.Match(
            _ => false,
            _ => true,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllFirstImagesForAllProducts();

        Assert.DoesNotContain(productFirstImages, x =>
        x.Id == productId1
        && CompareDataInByteArrays(x.ImageData, invalidCreateRequest.ImageData)
        && x.ImageContentType == invalidCreateRequest.ImageContentType);
    }

    [Fact]
    public void GetAllFirstImagesForSelectionOfProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId2 = productInsertResult2.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = GetFirstImageCreateRequestWithImageData(productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllFirstImagesForSelectionOfProducts(
            new List<int> { productId1, productId2 } );

        Assert.NotEmpty(productFirstImages);

        Assert.Contains(productFirstImages, x =>
        x.Id == productId1
        && CompareDataInByteArrays(x.ImageData, createRequest1.ImageData)
        && x.ImageContentType == createRequest1.ImageContentType);
    }

    [Fact]
    public void GetAllFirstImagesForSelectionOfProducts_ShouldFail_WhenInsertIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId2 = productInsertResult2.AsT0;

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(
            GetInvalidFirstImageCreateRequest(productId1));

        Assert.True(insertResult1.Match(
            _ => false,
            _ => true,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllFirstImagesForSelectionOfProducts(
            new List<int> { productId1, productId2 });

        Assert.Empty(productFirstImages);
    }

    [Fact]
    public void GetAllFirstImagesForSelectionOfProducts_ShouldFail_WhenProductIdsAreInvalidOrDontHaveRelatedImages()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId2 = productInsertResult2.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = GetFirstImageCreateRequestWithImageData(productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllFirstImagesForSelectionOfProducts(
            new List<int> { 0, productId2 });

        Assert.Empty(productFirstImages);
    }

    [Fact]
    public void GetAllForProduct_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData(productId);
        ServiceProductImageCreateRequest createRequest2 = GetCreateRequestWithImageData(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productImageService.InsertInAllImages(createRequest2);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(productId);

        Assert.Equal(2, productImages.Count());

        Assert.Contains(productImages, x =>
        x.ProductId == productId
        && CompareDataInByteArrays(x.ImageData, createRequest1.ImageData)
        && x.ImageContentType == createRequest1.ImageContentType);

        Assert.Contains(productImages, x =>
        x.ProductId == productId
        && CompareDataInByteArrays(x.ImageData, createRequest2.ImageData)
        && x.ImageContentType == createRequest2.ImageContentType);
    }

    [Fact]
    public void GetAllForProduct_ShouldFail_WhenInsertsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetInvalidImageCreateRequest(productId);
        ServiceProductImageCreateRequest createRequest2 = GetInvalidImageCreateRequest(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => false,
            _ => true,
            _ => false));

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productImageService.InsertInAllImages(createRequest2);

        Assert.True(insertResult1.Match(
            _ => false,
            _ => true,
            _ => false));

        IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(productId);

        Assert.Empty(productImages);
    }

    [Fact]
    public void GetByIdInAllImages_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageCreateRequest createRequest = GetCreateRequestWithImageData(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productImageService.InsertInAllImages(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int id = insertResult.AsT0;

        Assert.True(id > 0);

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

        Assert.NotNull(productImage);

        Assert.Equal(productId, productImage.ProductId);
        Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
        Assert.Equal(createRequest.ImageContentType, productImage.ImageContentType);
    }

    [Fact]
    public void GetByIdInAllImages_ShouldFail_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageCreateRequest createRequest = GetCreateRequestWithImageData(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productImageService.InsertInAllImages(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int id = insertResult.AsT0;

        Assert.True(id > 0);

        ProductImage? productImage = _productImageService.GetByIdInAllImages(999999999);

        Assert.Null(productImage);
    }

    [Fact]
    public void GetByProductIdInFirstImages_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductFirstImageCreateRequest createRequest = GetFirstImageCreateRequestWithImageData(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageService.InsertInFirstImages(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId);

        Assert.NotNull(productImage);

        Assert.Equal(productId, productImage.ProductId);
        Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
        Assert.Equal(createRequest.ImageContentType, productImage.ImageContentType);
    }

    [Fact]
    public void GetByProductIdInFirstImages_ShouldFail_WhenInsertIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductFirstImageCreateRequest invalidCreateRequest = GetInvalidFirstImageCreateRequest(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageService.InsertInFirstImages(invalidCreateRequest);

        Assert.True(insertResult.Match(
            _ => false,
            _ => true,
            _ => false));

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId);

        Assert.Null(productImage);
    }

    [Theory]
    [MemberData(nameof(InsertInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertInAllImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductImageCreateRequest createRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = productId1;
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest);

        Assert.Equal(expected, insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        if (expected)
        {
            int id = insertResult1.AsT0;

            Assert.True(id > 0);

            ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

            Assert.NotNull(productImage);

            Assert.Equal(productId1, productImage.ProductId);
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageContentType, productImage.ImageContentType);
        }
    }

    public static List<object[]> InsertInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            GetCreateRequestWithImageData(_useRequiredValue),
            true
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = "       ",
                HtmlData = null
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(InsertInAllImagesAndImageFileNameInfos_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertInAllImagesAndImageFileNameInfos_ShouldSucceedOrFail_InAnExpectedManner(
        ServiceProductImageCreateRequest createRequest,
        int? displayOrder,
        bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = productId1;
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1
            = _productImageService.InsertInAllImagesAndImageFileNameInfos(createRequest, displayOrder);

        Assert.Equal(expected, insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));


        if (expected)
        {
            int id = insertResult1.AsT0;

            Assert.True(id > 0);

            ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

            Assert.NotNull(productImage);

            Assert.Equal((int)productId1, productImage.ProductId);
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageContentType, productImage.ImageContentType);
        }
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public static List<object[]> InsertInAllImagesAndImageFileNameInfos_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[3]
        {
            GetCreateRequestWithImageData(_useRequiredValue),
            null,
            true
        },


        new object[3]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            null,
            true
        },

        new object[3]
        {
            GetCreateRequestWithImageData(_useRequiredValue),
            (uint?)1000,
            true
        },

        new object[3]
        {
            GetCreateRequestWithImageData(_useRequiredValue),
            (uint?)0,
            false
        },

        new object[3]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            null,
            false
        },

        new object[3]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            null,
            false
        },

        new object[3]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            null,
            false
        },

        new object[3]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = "       ",
                HtmlData = null
            },
            null,
            false
        },
    };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    [Theory]
    [MemberData(nameof(InsertInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertInFirstImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductFirstImageCreateRequest createRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = productId1;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest);

        Assert.Equal(expected, insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId1);

        Assert.Equal(expected, productImage is not null);

        if (expected)
        {
            Assert.NotNull(productImage);

            Assert.Equal(productId1, productImage.ProductId);
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageContentType, productImage.ImageContentType);
        }
    }

    public static List<object[]> InsertInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            GetFirstImageCreateRequestWithImageData(_useRequiredValue),
            true
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = "       ",
                HtmlData = null,
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateInAllImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductImageUpdateRequest updateRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        ServiceProductImageCreateRequest createRequest = GetCreateRequestWithImageData(productId1);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int id = insertResult1.AsT0;

        Assert.True(id > 0);

        if (updateRequest.Id == _useRequiredValue)
        {
            updateRequest.Id = id;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageService.UpdateInAllImages(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

        Assert.NotNull(productImage);

        Assert.Equal(productId1, productImage.ProductId);

        if (expected)
        {
            Assert.True(CompareDataInByteArrays(updateRequest.ImageData, productImage.ImageData));
            Assert.Equal(updateRequest.ImageContentType, productImage.ImageContentType);
        }
        else
        {
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageContentType, productImage.ImageContentType);
        }
    }

    public static List<object[]> UpdateInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = "image/png",
                HtmlData = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = "       ",
                HtmlData = null
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateInFirstImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductFirstImageUpdateRequest updateRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult1.AsT0;

        ServiceProductFirstImageCreateRequest createRequest = GetFirstImageCreateRequestWithImageData(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        if (updateRequest.ProductId == _useRequiredValue)
        {
            updateRequest.ProductId = productId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageService.UpdateInFirstImages(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId);

        Assert.NotNull(productImage);

        Assert.Equal(productId, productImage.ProductId);
        Assert.Equal(productId, productImage.Id);

        if (expected)
        {
            Assert.True(CompareDataInByteArrays(updateRequest.ImageData, productImage.ImageData));
            Assert.Equal(updateRequest.ImageContentType, productImage.ImageContentType);
        }
        else
        {
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageContentType, productImage.ImageContentType);
        }
    }

    public static List<object[]> UpdateInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = "image/png",
                HtmlData = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageContentType = "       ",
                HtmlData = null
            },
            false
        },
    };

    [Fact]
    public void DeleteInAllImagesById_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData(productId1);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int id = insertResult1.AsT0;

        Assert.True(id > 0);

        bool success = _productImageService.DeleteInAllImagesById(id);

        Assert.True(success);

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

        Assert.Null(productImage);
    }

    [Fact]
    public void DeleteInAllImagesById_ShouldFail_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        int? productId1 = productInsertResult1.Match<int?>(
             id => id,
             validationResult => null,
             unexpectedFailureResult => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData(productId1.Value);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);

        int? id = insertResult1.Match<int?>(
             id => id,
             validationResult => null,
             unexpectedFailureResult => null);

        Assert.NotNull(id);
        Assert.True(id > 0);

        bool success = _productImageService.DeleteInAllImagesById(999999999);

        Assert.False(success);

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id.Value);

        Assert.NotNull(productImage);
    }

    [Fact]
    public void DeleteInAllImagesAndImageFilePathInfosById_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId1 = productInsertResult1.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData(productId1);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImagesAndImageFileNameInfos(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int id = insertResult1.AsT0;

        bool success = _productImageService.DeleteInAllImagesAndImageFilePathInfosById(id);

        Assert.True(success);

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

        Assert.Null(productImage);
    }

    [Fact]
    public void DeleteInAllImagesAndImageFilePathInfosById_ShouldFail_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData(productId.Value);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImagesAndImageFileNameInfos(createRequest1);

        int? id = insertResult1.Match<int?>(
             id => id,
             validationResult => null,
             unexpectedFailureResult => null);

        Assert.NotNull(id);
        Assert.True(id > 0);

        bool success = _productImageService.DeleteInAllImagesById(999999999);

        Assert.False(success);

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id.Value);
        ProductImageFileNameInfo? productImageFileNameInfo = _productImageFileNameInfoService.GetAllInProduct(productId.Value)
            .FirstOrDefault(x => x.FileName?[x.FileName.IndexOf('.')..] == id.ToString());

        Assert.NotNull(productImage);
    }

    [Fact]
    public void DeleteAllImagesForProduct_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData(productId);
        ServiceProductImageCreateRequest createRequest2 = GetCreateRequestWithImageData(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productImageService.InsertInAllImages(createRequest2);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int id1 = insertResult1.AsT0;

        Assert.True(insertResult2.Match(
           _ => true,
           _ => false,
           _ => false));

        int id2 = insertResult2.AsT0;

        bool success = _productImageService.DeleteAllImagesForProduct(productId);

        Assert.True(success);

        IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(productId);

        Assert.Empty(productImages);
    }

    [Fact]
    public void DeleteAllImagesForProduct_ShouldFail_WhenInsertsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetInvalidImageCreateRequest(productId);
        ServiceProductImageCreateRequest createRequest2 = GetInvalidImageCreateRequest(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productImageService.InsertInAllImages(createRequest2);

        Assert.True(insertResult1.Match(
            _ => false,
            _ => true,
            _ => false));

        Assert.True(insertResult2.Match(
           _ => false,
           _ => true,
           _ => false));

        bool success = _productImageService.DeleteAllImagesForProduct(productId);

        Assert.False(success);

        IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(productId);

        Assert.Empty(productImages);
    }

    [Fact]
    public void DeleteInFirstImagesByProductId_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductFirstImageCreateRequest createRequest = GetFirstImageCreateRequestWithImageData(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageService.InsertInFirstImages(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        bool success = _productImageService.DeleteInFirstImagesByProductId(productId);

        Assert.True(success);

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId);

        Assert.Null(productImage);
    }

    [Fact]
    public void DeleteInFirstImagesByProductId_ShouldFail_WhenInsertIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductFirstImageCreateRequest createRequest = GetInvalidFirstImageCreateRequest(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageService.InsertInFirstImages(createRequest);

        Assert.True(insertResult.Match(
            _ => false,
            _ => true,
            _ => false));

        bool success = _productImageService.DeleteInFirstImagesByProductId(productId);

        Assert.False(success);

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId);

        Assert.Null(productImage);
    }

    [Fact]
    public void DeleteInFirstImagesByProductId_ShouldFail_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductFirstImageCreateRequest createRequest = GetFirstImageCreateRequestWithImageData(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageService.InsertInFirstImages(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        bool success = _productImageService.DeleteInFirstImagesByProductId(0);

        Assert.False(success);

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId);

        Assert.NotNull(productImage);
    }

#pragma warning disable IDE0051 // Remove unused private members
    private bool DeleteAllImagesInProduct(params int[] productIds)
#pragma warning restore IDE0051 // Remove unused private members
    {
        foreach (var productId in productIds)
        {
            bool success = _productImageService.DeleteAllImagesForProduct(productId);

            if (!success) return false;
        }

        return true;
    }
}