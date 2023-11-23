using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductImageServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductImageServiceTests(
        IProductImageService productImageService,
        IProductService productService)
        : base(Startup.ConnectionString)
    {
        _productImageService = productImageService;
        _productService = productService;
    }

    private const int _useRequiredValue = -100;

    private readonly IProductImageService _productImageService;
    private readonly IProductService _productService;

    
    [Fact]
    public void GetAllFirstImagesForAllProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = productInsertResult2.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = GetFirstImageCreateRequestWithImageData((int)productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllFirstImagesForAllProducts();

        Assert.True(productFirstImages.Any());

        Assert.Contains(productFirstImages, x =>
        x.Id == productId1
        && CompareDataInByteArrays(x.ImageData, createRequest1.ImageData)
        && x.ImageFileExtension == createRequest1.ImageFileExtension);

        // Deterministic Delete
        _productService.DeleteProducts(productId1, productId2);
        DeleteFirstImagesInProduct(productId1, productId2);
    }

    [Fact]
    public void GetAllFirstImagesForSelectionOfProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = productInsertResult2.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = GetFirstImageCreateRequestWithImageData((int)productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllFirstImagesForSelectionOfProducts( new List<uint> { productId1, productId2 } );

        Assert.True(productFirstImages.Any());

        Assert.Contains(productFirstImages, x =>
        x.Id == productId1
        && CompareDataInByteArrays(x.ImageData, createRequest1.ImageData)
        && x.ImageFileExtension == createRequest1.ImageFileExtension);

        // Deterministic Delete
        _productService.DeleteProducts(productId1, productId2);
        DeleteFirstImagesInProduct(productId1, productId2);
    }

    [Fact]
    public void GetAllForProduct_ShouldSucceed_WhenInsertAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImage> productFirstImages = _productImageService.GetAllInProduct(productId1);

        Assert.True(productFirstImages.Any());

        Assert.Contains(productFirstImages, x =>
        x.ProductId == productId1
        && CompareDataInByteArrays(x.ImageData, createRequest1.ImageData)
        && x.ImageFileExtension == createRequest1.ImageFileExtension);

        // Deterministic Delete
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
    }

    [Fact]
    public void GetByIdInAllImages_ShouldSucceed_WhenInsertAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id = insertResult1.AsT0;

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

        Assert.NotNull(productImage);

        Assert.Equal((int)productId1, productImage.ProductId);
        Assert.True(CompareDataInByteArrays(createRequest1.ImageData, productImage.ImageData));
        Assert.Equal(createRequest1.ImageFileExtension, productImage.ImageFileExtension);

        // Deterministic Delete
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
    }

    [Fact]
    public void GetByProductIdInFirstImages_ShouldSucceed_WhenInsertAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = GetFirstImageCreateRequestWithImageData((int)productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId1);

        Assert.NotNull(productImage);

        Assert.Equal((int)productId1, productImage.ProductId);
        Assert.True(CompareDataInByteArrays(createRequest1.ImageData, productImage.ImageData));
        Assert.Equal(createRequest1.ImageFileExtension, productImage.ImageFileExtension);

        // Deterministic Delete
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
    }

    [Theory]
    [MemberData(nameof(InsertInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertInAllImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductImageCreateRequest createRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId1;
        }

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest);

        Assert.Equal(expected, insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));


        if (expected)
        {
            uint id = insertResult1.AsT0;

            ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

            Assert.NotNull(productImage);

            Assert.Equal((int)productId1, productImage.ProductId);
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageFileExtension, productImage.ImageFileExtension);
        }

        // Deterministic Delete
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
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
                ImageFileExtension = null,
                XML = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = "image/png",
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = "       ",
                XML = null
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(InsertInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertInFirstImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductFirstImageCreateRequest createRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId1;
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

            Assert.Equal((int)productId1, productImage.ProductId);
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageFileExtension, productImage.ImageFileExtension);
        }

        // Deterministic Delete
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
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
                ImageFileExtension = null,
                XML = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = "image/png",
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageCreateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = "       ",
                XML = null,
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateInAllImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductImageUpdateRequest updateRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        var createRequest = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id = insertResult1.AsT0;

        if (updateRequest.Id == _useRequiredValue)
        {
            updateRequest.Id = (int)id;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageService.UpdateInAllImages(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

        Assert.NotNull(productImage);

        Assert.Equal((int)productId1, productImage.ProductId);

        if (expected)
        {
            Assert.True(CompareDataInByteArrays(updateRequest.ImageData, productImage.ImageData));
            Assert.Equal(updateRequest.ImageFileExtension, productImage.ImageFileExtension);
        }
        else
        {
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageFileExtension, productImage.ImageFileExtension);
        }

        // Deterministic Delete
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
    }

    public static List<object[]> UpdateInAllImages_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = "image/png",
                XML = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = null,
                ImageFileExtension = null,
                XML = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = "image/png",
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageUpdateRequest()
            {
                Id = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = "       ",
                XML = null
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateInFirstImages_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductFirstImageUpdateRequest updateRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult1.AsT0;

        ServiceProductFirstImageCreateRequest createRequest = GetFirstImageCreateRequestWithImageData((int)productId);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        if (updateRequest.ProductId == _useRequiredValue)
        {
            updateRequest.ProductId = (int)productId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageService.UpdateInFirstImages(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId);

        Assert.NotNull(productImage);

        Assert.Equal((int)productId, productImage.ProductId);
        Assert.Equal((int)productId, productImage.Id);

        if (expected)
        {
            Assert.True(CompareDataInByteArrays(updateRequest.ImageData, productImage.ImageData));
            Assert.Equal(updateRequest.ImageFileExtension, productImage.ImageFileExtension);
        }
        else
        {
            Assert.True(CompareDataInByteArrays(createRequest.ImageData, productImage.ImageData));
            Assert.Equal(createRequest.ImageFileExtension, productImage.ImageFileExtension);
        }

        // Deterministic Delete
        _productService.DeleteProducts(productId);
        DeleteFirstImagesInProduct(productId);
    }

    public static List<object[]> UpdateInFirstImages_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = "image/png",
                XML = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = null,
                ImageFileExtension = null,
                XML = null
            },
            true
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = Array.Empty<byte>(),
                ImageFileExtension = "image/png",
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = null,
                XML = null
            },
            false
        },

        new object[2]
        {
            new ServiceProductFirstImageUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageData = LocalTestImageData,
                ImageFileExtension = "       ",
                XML = null
            },
            false
        },
    };

    [Fact]
    public void DeleteInAllImagesById_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id = insertResult1.AsT0;

        bool success = _productImageService.DeleteInAllImagesById(id);

        Assert.True(success);

        ProductImage? productImage = _productImageService.GetByIdInAllImages(id);

        Assert.Null(productImage);

        // Deterministic Delete (In case delete in test fails)
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
    }

    [Fact]
    public void DeleteAllImagesForProduct_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ServiceProductImageCreateRequest createRequest1 = GetCreateRequestWithImageData((int)productId1);
        ServiceProductImageCreateRequest createRequest2 = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInAllImages(createRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productImageService.InsertInAllImages(createRequest2);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id1 = insertResult1.AsT0;

        Assert.True(insertResult2.Match(
           _ => true,
           _ => false,
           _ => false));

        uint id2 = insertResult2.AsT0;

        bool success = _productImageService.DeleteAllImagesForProduct(productId1);

        Assert.True(success);

        IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(productId1);

        Assert.Empty(productImages);

        // Deterministic Delete (In case delete in test fails)
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
    }

    [Fact]
    public void DeleteInFirstImagesByProductId_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = GetFirstImageCreateRequestWithImageData((int)productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult1 = _productImageService.InsertInFirstImages(createRequest1);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        bool success = _productImageService.DeleteInFirstImagesByProductId(productId1);

        Assert.True(success);

        ProductImage? productImage = _productImageService.GetFirstImageForProduct(productId1);

        Assert.Null(productImage);

        // Deterministic Delete (In case delete in test fails)
        _productService.DeleteProducts(productId1);
        DeleteFirstImagesInProduct(productId1);
    }

    private bool DeleteFirstImagesInProduct(params uint[] productIds)
    {
        foreach (var productId in productIds)
        {
            bool success = _productImageService.DeleteInFirstImagesByProductId(productId);

            if (!success) return false;
        }

        return true;
    }
}