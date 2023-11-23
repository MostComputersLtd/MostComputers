using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

public sealed class ProductImageFileNameInfoServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductImageFileNameInfoServiceTests(
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductService productService)
        : base(Startup.ConnectionString)
    {
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productService = productService;
    }

    private const int _useRequiredValue = -100;

    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductService _productService;

    private static ProductImageFileNameInfoCreateRequest GetCreateRequest(int productId, string fileName = "20183.png", uint displayOrder = 1) =>
    new()
    {
        FileName = fileName,
        DisplayOrder = (int)displayOrder,
        ProductId = productId
    };

    //private readonly ProductCreateRequest _validProductCreateRequest = new()
    //{
    //    Name = "Product name",
    //    AdditionalWarrantyPrice = 3.00M,
    //    AdditionalWarrantyTermMonths = 36,
    //    StandardWarrantyPrice = "0.00",
    //    StandardWarrantyTermMonths = 36,
    //    DisplayOrder = 12324,
    //    Status = ProductStatusEnum.Call,
    //    PlShow = 0,
    //    Price1 = 123.4M,
    //    DisplayPrice = 123.99M,
    //    Price3 = 122.5M,
    //    Currency = CurrencyEnum.EUR,
    //    RowGuid = Guid.NewGuid(),
    //    Promotionid = null,
    //    PromRid = null,
    //    PromotionPictureId = null,
    //    PromotionExpireDate = null,
    //    AlertPictureId = null,
    //    AlertExpireDate = null,
    //    PriceListDescription = null,
    //    PartNumber1 = "DF FKD@$ 343432 wdwfc",
    //    PartNumber2 = "123123/DD",
    //    SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

    //    Properties = new()
    //    {
    //        new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
    //        new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
    //        new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
    //    },
    //    Images = new List<CurrentProductImageCreateRequest>()
    //    {
    //    },
    //    ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
    //    {
    //        new() { FileName = "20143.png", DisplayOrder = 1 },
    //        new() { FileName = "20144.png", DisplayOrder = 2 }
    //    },

    //    CategoryID = 7,
    //    ManifacturerId = 12,
    //    SubCategoryId = null,
    //};

    [Fact]
    public void GetAll_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest((int)productId, displayOrder: 3);
        ProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest((int)productId, displayOrder: 4);

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

        // Deterministic Delete
        DeleteAllInProduct(productId);
        DeleteProduct(productId);
    }

    [Fact]
    public void GetAllForProduct_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest((int)productId, displayOrder: 3);
        ProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest((int)productId, displayOrder: 4);

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

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllForProduct(productId);

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

        // Deterministic Delete
        DeleteAllInProduct(productId);
        DeleteProduct(productId);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ProductImageFileNameInfoCreateRequest createRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageFileNameInfoService.Insert(createRequest);

        Assert.Equal(expected, insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllForProduct(productId);

        if (expected)
        {
            Assert.True(productImageFileNames.Any());

            Assert.Contains(productImageFileNames,
                x =>
                x.ProductId == createRequest.ProductId
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

        // Determinstic Delete
        DeleteProduct(productId);
        DeleteAllInProduct(productId);
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
    public void Update_ShouldSucceedOrFail_InAnExpectedManner(ProductImageFileNameInfoUpdateRequest updateRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductImageFileNameInfoCreateRequest createRequest = GetCreateRequest((int)productId, displayOrder: 3);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageFileNameInfoService.Insert(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        if (updateRequest.ProductId == _useRequiredValue)
        {
            updateRequest.ProductId = (int)productId;
        }

        if (updateRequest.DisplayOrder == _useRequiredValue)
        {
            updateRequest.DisplayOrder = createRequest.DisplayOrder;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageFileNameInfoService.Update(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllForProduct(productId);

        Assert.True(productImageFileNames.Any());

        if (expected)
        {
            ProductImageFileNameInfo? productImageFileNameInfo = productImageFileNames.FirstOrDefault(x =>
                x.ProductId == updateRequest.ProductId
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

        // Determinstic Delete
        DeleteProduct(productId);
        DeleteAllInProduct(productId);
    }

    public static List<object[]> Update_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ProductImageFileNameInfoUpdateRequest()
            {
                ProductId = _useRequiredValue,
                DisplayOrder = _useRequiredValue,
                NewDisplayOrder = 1000,
                FileName = "12342.png"
            },
            true
        },

        new object[2]
        {
            new ProductImageFileNameInfoUpdateRequest()
            {
                ProductId = _useRequiredValue,
                DisplayOrder = _useRequiredValue,
                NewDisplayOrder = 2,
                FileName = "12342.png"
            },
            true
        },

        new object[2]
        {
            new ProductImageFileNameInfoUpdateRequest()
            {
                ProductId = _useRequiredValue,
                DisplayOrder = _useRequiredValue,
                NewDisplayOrder = 0,
                FileName = "12342.png"
            },
            false
        },

        new object[2]
        {
            new ProductImageFileNameInfoUpdateRequest()
            {
                ProductId = _useRequiredValue,
                DisplayOrder = 0,
                NewDisplayOrder = 3,
                FileName = "12342.png"
            },
            false
        },

        new object[2]
        {
            new ProductImageFileNameInfoUpdateRequest()
            {
                ProductId = _useRequiredValue,
                DisplayOrder = _useRequiredValue,
                NewDisplayOrder = 1000,
                FileName = string.Empty,
            },
            false
        },

        new object[2]
        {
            new ProductImageFileNameInfoUpdateRequest()
            {
                ProductId = _useRequiredValue,
                DisplayOrder = 3,
                NewDisplayOrder = 1000,
                FileName = "veryyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy long imaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaage name",
            },
            false
        },

        new object[2]
        {
            new ProductImageFileNameInfoUpdateRequest()
            {
                ProductId = 0,
                DisplayOrder = 3,
                NewDisplayOrder = 1000,
                FileName = "12342.png"
            },
            false
        },
    };

    [Fact]
    public void DeleteAllForProductId_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest((int)productId, displayOrder: 3);
        ProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest((int)productId, displayOrder: 4);

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

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllForProduct(productId);

        Assert.Empty(productImageFileNames);

        // Deterministic Delete (in case delete in test fails)
        DeleteAllInProduct(productId);
        DeleteProduct(productId);
    }

    [Fact]
    public void DeleteByProductIdAndDisplayOrder_ShouldSucceed_WhenInsertsAndDisplayOrderAreValid()
    {
        const string firstFileName1 = "11111.png";
        const string firstFileName2 = "12222.png";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest((int)productId, firstFileName1, displayOrder: 3);
        ProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest((int)productId, firstFileName2, displayOrder: 4);

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

        IEnumerable<ProductImageFileNameInfo> productImageFileNames = _productImageFileNameInfoService.GetAllForProduct(productId);

        Assert.DoesNotContain(productImageFileNames, x =>
        x.ProductId == productId
        && x.FileName == createRequest1.FileName
        && x.DisplayOrder == createRequest1.DisplayOrder);

        // Deterministic Delete (in case delete in test fails)
        DeleteAllInProduct(productId);
        DeleteProduct(productId);
    }

    private bool DeleteAllInProduct(uint productId)
    {
        return _productImageFileNameInfoService.DeleteAllForProductId(productId);
    }

    private bool DeleteProduct(uint productId)
    {
        return _productService.Delete(productId);
    }
}