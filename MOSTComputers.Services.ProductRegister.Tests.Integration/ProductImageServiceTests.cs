using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.SuccessfulInsertAbstractions;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductImageServiceTests : IntegrationTestBaseForNonWebProjectsWithDBReset
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

#pragma warning disable CA2211 // Non-constant fields should not be visible

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

        ServiceProductFirstImageCreateRequest createRequest1 = GetValidFirstImageCreateRequestWithImageData(productId1);

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

        ServiceProductFirstImageCreateRequest createRequest1 = GetValidFirstImageCreateRequestWithImageData(productId1);

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

        ServiceProductFirstImageCreateRequest createRequest1 = GetValidFirstImageCreateRequestWithImageData(productId1);

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

        ServiceProductImageCreateRequest createRequest1 = GetValidImageCreateRequestWithImageData(productId);
        ServiceProductImageCreateRequest createRequest2 = GetValidImageCreateRequestWithImageData(productId);

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

        ServiceProductImageCreateRequest createRequest = GetValidImageCreateRequestWithImageData(productId);

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

        ServiceProductImageCreateRequest createRequest = GetValidImageCreateRequestWithImageData(productId);

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

        ServiceProductFirstImageCreateRequest createRequest = GetValidFirstImageCreateRequestWithImageData(productId);

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

        if (createRequest.ProductId == UseRequiredValuePlaceholder)
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

    public static TheoryData<ServiceProductImageCreateRequest, bool> InsertInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            GetValidImageCreateRequestWithImageData(UseRequiredValuePlaceholder),
            true
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
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

        if (createRequest.ProductId == UseRequiredValuePlaceholder)
        {
            createRequest.ProductId = productId1.Value;
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

    public static TheoryData<ServiceProductImageCreateRequest, int?, bool> InsertInAllImagesAndImageFileNameInfos_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            GetValidImageCreateRequestWithImageData(UseRequiredValuePlaceholder),
            null,
            true
        },


        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            null,
            true
        },

        {
            GetValidImageCreateRequestWithImageData(UseRequiredValuePlaceholder),
            1000,
            true
        },

        {
            GetValidImageCreateRequestWithImageData(UseRequiredValuePlaceholder),
            0,
            false
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            null,
            false
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            null,
            false
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            null,
            false
        },

        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = "       ",
                HtmlData = null
            },
            null,
            false
        },
    };

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

        if (createRequest.ProductId == UseRequiredValuePlaceholder)
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

    public static TheoryData<ServiceProductFirstImageCreateRequest, bool> InsertInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            GetValidFirstImageCreateRequestWithImageData(UseRequiredValuePlaceholder),
            true
        },

        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
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

        ServiceProductImageCreateRequest createRequest = GetValidImageCreateRequestWithImageData(productId1);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        int id = insertResult1.AsT0;

        Assert.True(id > 0);

        if (updateRequest.Id == UseRequiredValuePlaceholder)
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

    public static TheoryData<ServiceProductImageUpdateRequest, bool> UpdateInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = "image/png",
                HtmlData = null
            },
            true
        },

        {
            new ServiceProductImageUpdateRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        {
            new ServiceProductImageUpdateRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductImageUpdateRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductImageUpdateRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductImageUpdateRequest()
            {
                Id = UseRequiredValuePlaceholder,
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

        ServiceProductFirstImageCreateRequest createRequest = GetValidFirstImageCreateRequestWithImageData(productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        if (updateRequest.ProductId == UseRequiredValuePlaceholder)
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

    public static TheoryData<ServiceProductFirstImageUpdateRequest, bool> UpdateInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = "image/png",
                HtmlData = null
            },
            true
        },

        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = null,
                ImageContentType = null,
                HtmlData = null
            },
            true
        },

        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = Array.Empty<byte>(),
                ImageContentType = "image/png",
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = null,
                HtmlData = null
            },
            false
        },

        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ImageData = LocalTestImageData,
                ImageContentType = "       ",
                HtmlData = null
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateHtmlDataInFirstAndAllImagesByProductId_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected_Data))]
    public void UpdateHtmlDataInFirstAndAllImagesByProductId_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected(
        int productIdToUse, string htmlData, bool expected)
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceProductImageCreateRequest imageCreateRequest = GetValidImageCreateRequestWithImageData(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertImageResult = _productImageService.InsertInAllImages(imageCreateRequest);

        int imageId = -1;

        bool isImageInsertSuccessful = insertImageResult.Match(
            id =>
            {
                imageId = id;

                return true;
            },
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isImageInsertSuccessful);
        Assert.True(imageId > 0);

        ServiceProductFirstImageCreateRequest firstImageCreateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult
            = _productImageService.InsertInFirstImages(firstImageCreateRequest);

        bool isFirstImageInsertSuccessful = insertFirstImageResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageInsertSuccessful);

        if (productIdToUse == UseRequiredValuePlaceholder)
        {
            productIdToUse = productId;
        }

        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesHtmlResult
            = _productImageService.UpdateHtmlDataInFirstAndAllImagesByProductId(productIdToUse, htmlData);

        Assert.Equal(expected, updateImagesHtmlResult.Match(
            isSuccessful => isSuccessful,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductImage? firstImage = _productImageService.GetFirstImageForProduct(productId);
        ProductImage? productImage = _productImageService.GetByIdInAllImages(imageId);

        Assert.NotNull(firstImage);
        Assert.NotNull(productImage);

        if (expected)
        {
            Assert.Equal(htmlData, firstImage.HtmlData);
            Assert.Equal(htmlData, productImage.HtmlData);
        }
        else
        {
            Assert.Equal(firstImageCreateRequest.HtmlData, firstImage.HtmlData);
            Assert.Equal(imageCreateRequest.HtmlData, productImage.HtmlData);
        }
    }

    public static TheoryData<int, string, bool> UpdateHtmlDataInFirstAndAllImagesByProductId_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected_Data = new()
    {
        { UseRequiredValuePlaceholder, "<data></data>", true },
        { 0, "<data></data>", false },
        { UseRequiredValuePlaceholder, string.Empty, false },
        { UseRequiredValuePlaceholder, "      ", false },
    };

    [Theory]
    [MemberData(nameof(UpdateHtmlDataInFirstImagesByProductId_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected_Data))]
    public void UpdateHtmlDataInFirstImagesByProductId_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected(
        int productIdToUse, string htmlData, bool expected)
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceProductImageCreateRequest imageCreateRequest = GetValidImageCreateRequestWithImageData(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertImageResult = _productImageService.InsertInAllImages(imageCreateRequest);

        int imageId = -1;

        bool isImageInsertSuccessful = insertImageResult.Match(
            id =>
            {
                imageId = id;

                return true;
            },
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isImageInsertSuccessful);
        Assert.True(imageId > 0);

        ServiceProductFirstImageCreateRequest firstImageCreateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult
            = _productImageService.InsertInFirstImages(firstImageCreateRequest);

        bool isFirstImageInsertSuccessful = insertFirstImageResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageInsertSuccessful);

        if (productIdToUse == UseRequiredValuePlaceholder)
        {
            productIdToUse = productId;
        }

        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesHtmlResult
            = _productImageService.UpdateHtmlDataInFirstImagesByProductId(productIdToUse, htmlData);

        Assert.Equal(expected, updateImagesHtmlResult.Match(
            isSuccessful => isSuccessful,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductImage? firstImage = _productImageService.GetFirstImageForProduct(productId);
        ProductImage? productImage = _productImageService.GetByIdInAllImages(imageId);

        Assert.NotNull(firstImage);
        Assert.NotNull(productImage);

        if (expected)
        {
            Assert.Equal(htmlData, firstImage.HtmlData);
            Assert.Equal(imageCreateRequest.HtmlData, productImage.HtmlData);
        }
        else
        {
            Assert.Equal(firstImageCreateRequest.HtmlData, firstImage.HtmlData);
            Assert.Equal(imageCreateRequest.HtmlData, productImage.HtmlData);
        }
    }

    public static TheoryData<int, string, bool> UpdateHtmlDataInFirstImagesByProductId_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected_Data = new()
    {
        { UseRequiredValuePlaceholder, "<data></data>", true },
        { 0, "<data></data>", false },
        { UseRequiredValuePlaceholder, string.Empty, false },
        { UseRequiredValuePlaceholder, "      ", false },
    };

    [Theory]
    [MemberData(nameof(UpdateHtmlDataInAllImagesById_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected_Data))]
    public void UpdateHtmlDataInAllImagesById_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected(
        int imageIdToUse, string htmlData, bool expected)
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceProductImageCreateRequest imageCreateRequest = GetValidImageCreateRequestWithImageData(productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertImageResult = _productImageService.InsertInAllImages(imageCreateRequest);

        int imageId = -1;

        bool isImageInsertSuccessful = insertImageResult.Match(
            id =>
            {
                imageId = id;

                return true;
            },
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isImageInsertSuccessful);
        Assert.True(imageId > 0);

        ServiceProductFirstImageCreateRequest firstImageCreateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult
            = _productImageService.InsertInFirstImages(firstImageCreateRequest);

        bool isFirstImageInsertSuccessful = insertFirstImageResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageInsertSuccessful);

        if (imageIdToUse == UseRequiredValuePlaceholder)
        {
            imageIdToUse = productId;
        }

        OneOf<bool, ValidationResult, UnexpectedFailureResult> updateImagesHtmlResult
            = _productImageService.UpdateHtmlDataInAllImagesById(imageIdToUse, htmlData);

        Assert.Equal(expected, updateImagesHtmlResult.Match(
            isSuccessful => isSuccessful,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductImage? firstImage = _productImageService.GetFirstImageForProduct(productId);
        ProductImage? productImage = _productImageService.GetByIdInAllImages(imageId);

        Assert.NotNull(firstImage);
        Assert.NotNull(productImage);

        Assert.Equal(firstImageCreateRequest.HtmlData, firstImage.HtmlData);

        if (expected)
        {
            Assert.Equal(htmlData, productImage.HtmlData);
        }
        else
        {
            Assert.Equal(imageCreateRequest.HtmlData, productImage.HtmlData);
        }
    }

    public static TheoryData<int, string, bool> UpdateHtmlDataInAllImagesById_ShouldSucceedToUpdateTheHtmlDataOfAllRelevantImages_WhenExpected_Data = new()
    {
        { UseRequiredValuePlaceholder, "<data></data>", true },
        { 0, "<data></data>", false },
        { UseRequiredValuePlaceholder, string.Empty, false },
        { UseRequiredValuePlaceholder, "      ", false },
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

        ServiceProductImageCreateRequest createRequest1 = GetValidImageCreateRequestWithImageData(productId1);

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

        ServiceProductImageCreateRequest createRequest1 = GetValidImageCreateRequestWithImageData(productId1.Value);

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

        ServiceProductImageCreateRequest createRequest1 = GetValidImageCreateRequestWithImageData(productId1);

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

        ServiceProductImageCreateRequest createRequest1 = GetValidImageCreateRequestWithImageData(productId.Value);

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

        ServiceProductImageCreateRequest createRequest1 = GetValidImageCreateRequestWithImageData(productId);
        ServiceProductImageCreateRequest createRequest2 = GetValidImageCreateRequestWithImageData(productId);

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

        ServiceProductFirstImageCreateRequest createRequest = GetValidFirstImageCreateRequestWithImageData(productId);

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

        ServiceProductFirstImageCreateRequest createRequest = GetValidFirstImageCreateRequestWithImageData(productId);

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

#pragma warning restore CA2211 // Non-constant fields should not be visible
}