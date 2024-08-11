using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductImageFileNameInfoServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductImageFileNameInfoServiceTests(
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductService productService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productService = productService;
    }

    private const int _useRequiredValue = -100;

    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductService _productService;

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    private static ServiceProductImageFileNameInfoCreateRequest GetCreateRequest(int productId, string fileName = "20183.png", uint displayOrder = 1) =>
    new()
    {
        FileName = fileName,
        DisplayOrder = (int)displayOrder,
        ProductId = productId
    };

    [Fact]
    public void GetAll_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, displayOrder: 3);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, displayOrder: 4);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult1 = _productImageFileNameInfoService.Insert(createRequest1);

        Assert.True(imageFileNameInfoInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult2 = _productImageFileNameInfoService.Insert(createRequest2);

        Assert.True(imageFileNameInfoInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAll();

        Assert.True(productImageFileNames.Count() >= 2);

        Assert.Contains(productImageFileNames,
            x =>
            x.ProductId == createRequest1.ProductId
            && x.DisplayOrder == createRequest1.DisplayOrder
            && x.FileName == createRequest1.FileName);

        Assert.Contains(productImageFileNames,
            x =>
            x.ProductId == createRequest2.ProductId
            && x.DisplayOrder == createRequest2.DisplayOrder
            && x.FileName == createRequest2.FileName);
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

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, displayOrder: 3);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, displayOrder: 4);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult1 = _productImageFileNameInfoService.Insert(createRequest1);

        Assert.True(imageFileNameInfoInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult2 = _productImageFileNameInfoService.Insert(createRequest2);

        Assert.True(imageFileNameInfoInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct((int)productId);

        Assert.True(productImageFileNames.Count() >= 2);

        Assert.Contains(productImageFileNames,
            x =>
            x.ProductId == createRequest1.ProductId
            && x.ImageNumber > 0
            && x.DisplayOrder == createRequest1.DisplayOrder
            && x.FileName == createRequest1.FileName);

        Assert.Contains(productImageFileNames,
            x =>
            x.ProductId == createRequest2.ProductId
            && x.ImageNumber > 0
            && x.DisplayOrder == createRequest2.DisplayOrder
            && x.FileName == createRequest2.FileName);
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

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, "   ", displayOrder: 3);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, "", displayOrder: 4);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult1 = _productImageFileNameInfoService.Insert(createRequest1);

        Assert.True(imageFileNameInfoInsertResult1.Match(
            _ => false,
            _ => true,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult2 = _productImageFileNameInfoService.Insert(createRequest2);

        Assert.True(imageFileNameInfoInsertResult2.Match(
            _ => false,
            _ => true,
            _ => false));

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        if (ValidProductCreateRequest.ImageFileNames is not null)
        {
            Assert.True(productImageFileNames.Count() == ValidProductCreateRequest.ImageFileNames.Count);
        }
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductImageFileNameInfoCreateRequest createRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageFileNameInfoService.Insert(createRequest);

        Assert.Equal(expected, insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        if (expected)
        {
            Assert.True(productImageFileNames.Any());

            Assert.Contains(productImageFileNames,
                x =>
                x.ProductId == createRequest.ProductId
                && x.ImageNumber == (ValidProductCreateRequest.ImageFileNames?.Count ?? 1)
                && x.DisplayOrder == createRequest.DisplayOrder
                && x.FileName == createRequest.FileName);
        }
        else
        {
            Assert.DoesNotContain(productImageFileNames,
                x =>
                x.ProductId == createRequest.ProductId
                && x.DisplayOrder == createRequest.DisplayOrder
                && x.FileName == createRequest.FileName);
        }
    }

    public static List<object[]> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            GetCreateRequest(_useRequiredValue, displayOrder: 3),
            true
        },

        new object[2]
        {
            GetCreateRequest(_useRequiredValue, displayOrder: 1),
            true
        },

        new object[2]
        {
            GetCreateRequest(_useRequiredValue, displayOrder: 0),
            false
        },

        new object[2]
        {
            GetCreateRequest(_useRequiredValue, fileName: string.Empty, displayOrder: 1),
            false
        },

        new object[2]
        {
            GetCreateRequest(_useRequiredValue, fileName: "     ", displayOrder: 1),
            false
        },
    };

    [Theory]
    [MemberData(nameof(Update_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Update_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductImageFileNameInfoByImageNumberUpdateRequest updateRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageFileNameInfoCreateRequest createRequest = GetCreateRequest(productId, displayOrder: 3);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageFileNameInfoService.Insert(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImageFileNameInfo? fileNameInfo = _productImageFileNameInfoService.GetAllInProduct((int)productId)
            .FirstOrDefault(x => x.DisplayOrder == createRequest.DisplayOrder);

        Assert.NotNull(fileNameInfo);

        if (updateRequest.ProductId == _useRequiredValue)
        {
            updateRequest.ProductId = productId;
        }

        if (updateRequest.ImageNumber == _useRequiredValue)
        {
            updateRequest.ImageNumber = fileNameInfo.ImageNumber;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageFileNameInfoService.Update(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        Assert.True(productImageFileNames.Any());

        if (expected)
        {
            ProductImageFileNameInfo? productImageFileNameInfo = productImageFileNames.FirstOrDefault(x =>
                x.ProductId == updateRequest.ProductId
                && x.ImageNumber == updateRequest.ImageNumber
                && x.FileName == updateRequest.FileName);

            Assert.NotNull(productImageFileNameInfo);

            int? maxDisplayOrder = productImageFileNames.Max(x => x.DisplayOrder);

            Assert.True(productImageFileNameInfo.DisplayOrder == maxDisplayOrder
                || (productImageFileNameInfo.DisplayOrder > 0 && productImageFileNameInfo.DisplayOrder < maxDisplayOrder));
        }
        else
        {
            Assert.Contains(productImageFileNames,
                x =>
                x.ProductId == createRequest.ProductId
                && x.DisplayOrder == createRequest.DisplayOrder
                && x.FileName == createRequest.FileName);
        }
    }

    public static List<object[]> Update_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ServiceProductImageFileNameInfoByImageNumberUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageNumber = _useRequiredValue,
                NewDisplayOrder = 1000,
                FileName = "12342.png"
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageFileNameInfoByImageNumberUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageNumber = _useRequiredValue,
                NewDisplayOrder = 2,
                FileName = "12342.png"
            },
            true
        },

        new object[2]
        {
            new ServiceProductImageFileNameInfoByImageNumberUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageNumber = _useRequiredValue,
                NewDisplayOrder = 0,
                FileName = "12342.png"
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageFileNameInfoByImageNumberUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageNumber = 0,
                NewDisplayOrder = 3,
                FileName = "12342.png"
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageFileNameInfoByImageNumberUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageNumber = _useRequiredValue,
                NewDisplayOrder = 1000,
                FileName = string.Empty,
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageFileNameInfoByImageNumberUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ImageNumber = 3,
                NewDisplayOrder = 1000,
                FileName = "veryyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy long imaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaage name",
            },
            false
        },

        new object[2]
        {
            new ServiceProductImageFileNameInfoByImageNumberUpdateRequest()
            {
                ProductId = 0,
                ImageNumber = 3,
                NewDisplayOrder = 1000,
                FileName = "12342.png"
            },
            false
        },
    };

    [Fact]
    public void DeleteAllForProductId_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, displayOrder: 3);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, displayOrder: 4);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult1 = _productImageFileNameInfoService.Insert(createRequest1);

        Assert.True(imageFileNameInfoInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult2 = _productImageFileNameInfoService.Insert(createRequest2);

        Assert.True(imageFileNameInfoInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        bool deleteSuccess = _productImageFileNameInfoService.DeleteAllForProductId(productId);

        Assert.True(deleteSuccess);

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        Assert.Empty(productImageFileNames);
    }

    [Fact]
    public void DeleteAllForProductId_ShouldFail_WhenInsertsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, "", displayOrder: 3);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, "", displayOrder: 4);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult1 = _productImageFileNameInfoService.Insert(createRequest1);

        Assert.True(imageFileNameInfoInsertResult1.Match(
            _ => false,
            _ => true,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult2 = _productImageFileNameInfoService.Insert(createRequest2);

        Assert.True(imageFileNameInfoInsertResult2.Match(
            _ => false,
            _ => true,
            _ => false));

        bool deleteSuccess = _productImageFileNameInfoService.DeleteAllForProductId(productId);

        Assert.False(deleteSuccess);

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        if (ValidProductCreateRequestWithNoImages.ImageFileNames is not null)
        {
            Assert.Equal(ValidProductCreateRequestWithNoImages.ImageFileNames.Count, productImageFileNames.Count());
        }
    }

    [Fact]
    public void DeleteByProductIdAndDisplayOrder_ShouldSucceed_AndUpdateDisplayOrdersToBeInOrder_WhenInsertsAndDisplayOrderAreValid()
    {
        const string firstFileName1 = "11111.png";
        const string firstFileName2 = "12222.png";

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, firstFileName1, displayOrder: 3);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, firstFileName2, displayOrder: 4);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult1 = _productImageFileNameInfoService.Insert(createRequest1);

        Assert.True(imageFileNameInfoInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult2 = _productImageFileNameInfoService.Insert(createRequest2);

        Assert.True(imageFileNameInfoInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        bool deleteSuccess = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(productId, 3);

        Assert.True(deleteSuccess);

        List<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId).ToList();

        Assert.DoesNotContain(productImageFileNames, x =>
        x.ProductId == productId
        && x.FileName == createRequest1.FileName
        && x.DisplayOrder == createRequest1.DisplayOrder);

        for (int i = 0; i < productImageFileNames.Count; i++)
        {
            Assert.Equal(i + 1, productImageFileNames[i].DisplayOrder);
        }
    }

    [Fact]
    public void DeleteByProductIdAndDisplayOrder_ShouldFail_WhenProductIdIsInvalid()
    {
        const string firstFileName1 = "11111.png";
        const string firstFileName2 = "12222.png";

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, firstFileName1, displayOrder: 3);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, firstFileName2, displayOrder: 4);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult1 = _productImageFileNameInfoService.Insert(createRequest1);

        Assert.True(imageFileNameInfoInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult2 = _productImageFileNameInfoService.Insert(createRequest2);

        Assert.True(imageFileNameInfoInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        bool deleteSuccess = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(0, 3);

        Assert.False(deleteSuccess);

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        Assert.Contains(productImageFileNames, x =>
        x.ProductId == productId
        && x.FileName == createRequest1.FileName
        && x.DisplayOrder == createRequest1.DisplayOrder);
    }
}