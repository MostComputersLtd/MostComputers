using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Products.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileData;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.SuccessfulInsertAbstractions;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;
[Collection(DefaultTestCollection.Name)]
public sealed class ProductImageFileNameInfoServiceTests : IntegrationTestBaseForNonWebProjectsWithDBReset
{
    public ProductImageFileNameInfoServiceTests(
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductService productService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productService = productService;
    }

    private const string _useRequiredValuePlaceholder = "Use required value..,''][;[l];l;,;[///,,,";
    private const string _invalidFileName = "fileNameWithoutExtension";

    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductService _productService;

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    private static ServiceProductImageFileNameInfoCreateRequest GetCreateRequest(int productId, string fileName = "20183.png", int displayOrder = 1, bool active = false) =>
    new()
    {
        FileName = fileName,
        DisplayOrder = displayOrder,
        ProductId = productId,
        Active = active
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

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAll();

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

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct((int)productId);

        Assert.True(productImageFileNames.Count() >= 2);

        Assert.Contains(productImageFileNames,
            x =>
            x.ProductId == createRequest1.ProductId
            && x.Id > 0
            && x.DisplayOrder == createRequest1.DisplayOrder
            && x.FileName == createRequest1.FileName);

        Assert.Contains(productImageFileNames,
            x =>
            x.ProductId == createRequest2.ProductId
            && x.Id > 0
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

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        if (ValidProductCreateRequest.ImageFileNames is not null)
        {
            Assert.True(productImageFileNames.Count() == ValidProductCreateRequest.ImageFileNames.Count);
        }
    }

    [Fact]
    public void GetByProductIdAndImageNumber_ShouldSucceed_WhenRecordExists()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequest);

        int? lastImageNumber = ValidProductCreateRequest.ImageFileNames?.Count;

        CurrentProductImageFileNameInfoCreateRequest? lastFileNameInfoCreateRequest = ValidProductCreateRequest.ImageFileNames?.LastOrDefault();

        if (lastFileNameInfoCreateRequest is null)
        {
            ServiceProductImageFileNameInfoCreateRequest createRequest = GetCreateRequest(productId);

            OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(createRequest);

            Assert.True(imageFileNameInfoInsertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            lastFileNameInfoCreateRequest = new()
            {
                DisplayOrder = createRequest.DisplayOrder,
                FileName = createRequest.FileName,
                Active = createRequest.Active ?? false
            };

            lastImageNumber = 1;
        }

        ProductImageFileData? fileNameInfo = _productImageFileNameInfoService.GetByProductIdAndImageNumber(productId, lastImageNumber!.Value);

        Assert.NotNull(fileNameInfo);

        Assert.Equal(productId, fileNameInfo.ProductId);
        Assert.Equal(lastImageNumber, fileNameInfo.Id);
        Assert.Equal(lastFileNameInfoCreateRequest.FileName, fileNameInfo.FileName);
        Assert.Equal(lastFileNameInfoCreateRequest.Active, fileNameInfo.Active);
    }

    [Fact]
    public void GetByProductIdAndImageNumber_ShouldFail_WhenRecordDoesntExist()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductImageFileData? fileNameInfo = _productImageFileNameInfoService.GetByProductIdAndImageNumber(productId, 1);

        Assert.Null(fileNameInfo);
    }

    [Fact]
    public void GetByFileName_ShouldSucceed_WhenRecordExists()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequest);

        CurrentProductImageFileNameInfoCreateRequest? lastFileNameInfoCreateRequest = ValidProductCreateRequest.ImageFileNames?.LastOrDefault();

        if (lastFileNameInfoCreateRequest is null)
        {
            ServiceProductImageFileNameInfoCreateRequest createRequest = GetCreateRequest(productId);

            OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(createRequest);

            Assert.True(imageFileNameInfoInsertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            lastFileNameInfoCreateRequest = new()
            {
                DisplayOrder = createRequest.DisplayOrder,
                FileName = createRequest.FileName,
                Active = createRequest.Active ?? false
            };
        }

        ProductImageFileData? fileNameInfo = _productImageFileNameInfoService.GetByFileName(lastFileNameInfoCreateRequest.FileName!);

        Assert.NotNull(fileNameInfo);

        Assert.Equal(productId, fileNameInfo.ProductId);
        Assert.Equal(ValidProductCreateRequest.ImageFileNames?.Count ?? 1, fileNameInfo.Id);
        Assert.Equal(lastFileNameInfoCreateRequest.FileName, fileNameInfo.FileName);
        Assert.Equal(lastFileNameInfoCreateRequest.Active, fileNameInfo.Active);
    }

    [Theory]
    [MemberData(nameof(GetByFileName_ShouldFail_WhenFileNameIsInvalid_Data))]
    public void GetByFileName_ShouldFail_WhenFileNameIsInvalid(string fileName)
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequest);

        CurrentProductImageFileNameInfoCreateRequest? lastFileNameInfoCreateRequest = ValidProductCreateRequest.ImageFileNames?.LastOrDefault();

        if (lastFileNameInfoCreateRequest is null)
        {
            ServiceProductImageFileNameInfoCreateRequest createRequest = GetCreateRequest(productId);

            OneOf<Success, ValidationResult, UnexpectedFailureResult> imageFileNameInfoInsertResult
                = _productImageFileNameInfoService.Insert(createRequest);

            Assert.True(imageFileNameInfoInsertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            lastFileNameInfoCreateRequest = new()
            {
                DisplayOrder = createRequest.DisplayOrder,
                FileName = createRequest.FileName,
                Active = createRequest.Active ?? false
            };
        }

        ProductImageFileData? fileNameInfo = _productImageFileNameInfoService.GetByFileName(fileName);

        Assert.Null(fileNameInfo);
    }

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static TheoryData<string> GetByFileName_ShouldFail_WhenFileNameIsInvalid_Data = new()
    {
        { string.Empty },
        { "     " },
        { _invalidFileName },
    };
#pragma warning restore CA2211 // Non-constant fields should not be visible

    [Fact]
    public void GetHighestImageNumber_ShouldSucceedToGetHighestImageNumber_WhenProductExistsAndHasFileNameInfos()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequest);

        int? highestImageNumber = _productImageFileNameInfoService.GetHighestImageNumber(productId);

        Assert.Equal(ValidProductCreateRequest.ImageFileNames?.Count, highestImageNumber);
    }

    [Fact]
    public void GetHighestImageNumber_ShouldFailToGetHighestImageNumber_WhenProductExistsButDoesntHaveFileNameInfos()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        int? highestImageNumber = _productImageFileNameInfoService.GetHighestImageNumber(productId);

        Assert.Null(highestImageNumber);
    }

    [Fact]
    public void GetHighestImageNumber_ShouldFailToGetHighestImageNumber_WhenProductDoesntExist()
    {
        int? highestImageNumber = _productImageFileNameInfoService.GetHighestImageNumber(0);

        Assert.Null(highestImageNumber);
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

        if (createRequest.ProductId == UseRequiredValuePlaceholder)
        {
            createRequest.ProductId = productId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageFileNameInfoService.Insert(createRequest);

        Assert.Equal(expected, insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        if (expected)
        {
            Assert.True(productImageFileNames.Any());

            Assert.Contains(productImageFileNames,
                x =>
                x.ProductId == createRequest.ProductId
                && x.Id == (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1
                && x.DisplayOrder == createRequest.DisplayOrder
                && x.FileName == createRequest.FileName
                && x.Active == createRequest.Active);
        }
        else
        {
            Assert.DoesNotContain(productImageFileNames,
                x =>
                x.ProductId == createRequest.ProductId
                && x.DisplayOrder == createRequest.DisplayOrder
                && x.FileName == createRequest.FileName
                && x.Active == createRequest.Active);
        }
    }

    public static TheoryData<ServiceProductImageFileNameInfoCreateRequest, bool> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        {
            GetCreateRequest(UseRequiredValuePlaceholder, displayOrder: 3),
            true
        },

        {
            GetCreateRequest(UseRequiredValuePlaceholder, displayOrder: 1),
            true
        },

        {
            GetCreateRequest(0, displayOrder: 1),
            false
        },

        {
            GetCreateRequest(UseRequiredValuePlaceholder, displayOrder: 0),
            false
        },

        {
            GetCreateRequest(UseRequiredValuePlaceholder, fileName: string.Empty, displayOrder: 1),
            false
        },

        {
            GetCreateRequest(UseRequiredValuePlaceholder, fileName: "     ", displayOrder: 1),
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateByImageNumber_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateByImageNumber_ShouldSucceedOrFail_InAnExpectedManner(ProductImageFileUpdateRequest updateRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        int lastDisplayOrder = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;

        ServiceProductImageFileNameInfoCreateRequest createRequest = GetCreateRequest(productId, displayOrder: lastDisplayOrder);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageFileNameInfoService.Insert(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImageFileData? fileNameInfo = _productImageFileNameInfoService.GetAllInProduct(productId)
            .FirstOrDefault(x => x.DisplayOrder == createRequest.DisplayOrder);

        Assert.NotNull(fileNameInfo);

        if (updateRequest.ProductId == UseRequiredValuePlaceholder)
        {
            updateRequest.ProductId = productId;
        }

        if (updateRequest.Id == UseRequiredValuePlaceholder)
        {
            updateRequest.Id = fileNameInfo.Id;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageFileNameInfoService.UpdateByImageNumber(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        Assert.True(productImageFileNames.Any());

        if (expected)
        {
            ProductImageFileData? productImageFileNameInfo = productImageFileNames.FirstOrDefault(x =>
                x.ProductId == updateRequest.ProductId
                && x.Id == updateRequest.Id
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

    public static TheoryData<ProductImageFileUpdateRequest, bool> UpdateByImageNumber_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = UseRequiredValuePlaceholder,
                NewDisplayOrder = 1000,
                FileName = "12342.png",
                Active = true,
                ShouldUpdateDisplayOrder = true,
            },
            true
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = UseRequiredValuePlaceholder,
                NewDisplayOrder = 1000,
                FileName = "12342.png",
                Active = true,
                ShouldUpdateDisplayOrder = false,
            },
            true
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = UseRequiredValuePlaceholder,
                NewDisplayOrder = 2,
                FileName = "12342.png",
                ShouldUpdateDisplayOrder = true,
            },
            true
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = UseRequiredValuePlaceholder,
                NewDisplayOrder = 2,
                FileName = "12342.png",
                ShouldUpdateDisplayOrder = false,
            },
            true
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = UseRequiredValuePlaceholder,
                NewDisplayOrder = 0,
                FileName = "12342.png",
                ShouldUpdateDisplayOrder = false,
            },
            true
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = UseRequiredValuePlaceholder,
                NewDisplayOrder = 0,
                FileName = "12342.png",
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = 0,
                NewDisplayOrder = 3,
                FileName = "12342.png",
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = UseRequiredValuePlaceholder,
                NewDisplayOrder = 1000,
                FileName = string.Empty,
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                Id = 3,
                NewDisplayOrder = 1000,
                FileName = "veryyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy long " +
                "imaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaage filename",
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ProductImageFileUpdateRequest()
            {
                ProductId = 0,
                Id = 3,
                NewDisplayOrder = 1000,
                FileName = "12342.png",
                ShouldUpdateDisplayOrder = true,
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateByFileName_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateByFileName_ShouldSucceedOrFail_InAnExpectedManner(ServiceProductImageFileNameInfoByFileNameUpdateRequest updateRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        int lastDisplayOrder = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;

        ServiceProductImageFileNameInfoCreateRequest createRequest = GetCreateRequest(productId, displayOrder: lastDisplayOrder, fileName: "20184.png");

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertResult = _productImageFileNameInfoService.Insert(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductImageFileData? fileNameInfo = _productImageFileNameInfoService.GetAllInProduct(productId)
            .FirstOrDefault(x => x.DisplayOrder == createRequest.DisplayOrder);

        Assert.NotNull(fileNameInfo);
        Assert.NotNull(fileNameInfo.FileName);

        if (updateRequest.ProductId == UseRequiredValuePlaceholder)
        {
            updateRequest.ProductId = productId;
        }

        if (updateRequest.FileName == _useRequiredValuePlaceholder)
        {
            updateRequest.FileName = fileNameInfo.FileName;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productImageFileNameInfoService.UpdateByFileName(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        Assert.True(productImageFileNames.Any());

        if (expected)
        {
            int? maxDisplayOrder = productImageFileNames.Max(x => x.DisplayOrder);

            int? expectedDisplayOrder = (updateRequest.ShouldUpdateDisplayOrder) ? updateRequest.NewDisplayOrder : createRequest.DisplayOrder;

            if (expectedDisplayOrder is not null)
            {
                expectedDisplayOrder = Math.Min(expectedDisplayOrder.Value, maxDisplayOrder ?? 1);
            }

            ProductImageFileData? productImageFileNameInfo = productImageFileNames.FirstOrDefault(x =>
                x.ProductId == updateRequest.ProductId
                && x.DisplayOrder == expectedDisplayOrder
                && x.FileName == (updateRequest.NewFileName ?? createRequest.FileName)
                && x.Active == (updateRequest.Active ?? false));

            Assert.NotNull(productImageFileNameInfo);

            Assert.True(productImageFileNameInfo.DisplayOrder > 0 && productImageFileNameInfo.DisplayOrder <= maxDisplayOrder);
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

    public static TheoryData<ServiceProductImageFileNameInfoByFileNameUpdateRequest, bool> UpdateByFileName_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewDisplayOrder = 1000,
                NewFileName = "12342.png",
                Active = true,
                ShouldUpdateDisplayOrder = true,
            },
            true
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewDisplayOrder = 1000,
                NewFileName = "12342.png",
                Active = true,
                ShouldUpdateDisplayOrder = false,
            },
            true
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewDisplayOrder = 2,
                NewFileName = "12342.png",
                ShouldUpdateDisplayOrder = true,
            },
            true
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewDisplayOrder = 2,
                NewFileName = "12342.png",
                ShouldUpdateDisplayOrder = false,
            },
            true
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewDisplayOrder = 0,
                NewFileName = "12342.png",
                ShouldUpdateDisplayOrder = false,
            },
            true
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewDisplayOrder = 0,
                NewFileName = "12342.png",
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewFileName = string.Empty,
                NewDisplayOrder = 1000,
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = _useRequiredValuePlaceholder,
                NewDisplayOrder = 1000,
                NewFileName = "veryyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy long " +
                "imaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaage filename",
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = 0,
                FileName = _useRequiredValuePlaceholder,
                NewFileName = "12342.png",
                NewDisplayOrder = 1000,
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = string.Empty,
                NewFileName = "12342.png",
                NewDisplayOrder = 1000,
                ShouldUpdateDisplayOrder = true,
            },
            false
        },

        {
            new ServiceProductImageFileNameInfoByFileNameUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                FileName = "           ",
                NewFileName = "12342.png",
                NewDisplayOrder = 1000,
                ShouldUpdateDisplayOrder = true,
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

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

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

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        if (ValidProductCreateRequestWithNoImages.ImageFileNames is not null)
        {
            Assert.Equal(ValidProductCreateRequestWithNoImages.ImageFileNames.Count, productImageFileNames.Count());
        }
    }

    [Fact]
    public void DeleteByProductIdAndImageNumber_ShouldSucceed_AndUpdateDisplayOrdersToBeInOrder_WhenInsertsAndDisplayOrderAreValid()
    {
        const string firstFileName1 = "11111.png";
        const string firstFileName2 = "12222.png";

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        int lastImageNumber = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;
        int lastDisplayOrder = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, firstFileName1, displayOrder: lastDisplayOrder);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, firstFileName2, displayOrder: lastDisplayOrder + 1);

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

        bool deleteSuccess = _productImageFileNameInfoService.DeleteByProductIdAndImageNumber(productId, lastImageNumber);

        Assert.True(deleteSuccess);

        List<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId).ToList();

        Assert.Equal(lastImageNumber, productImageFileNames.Count);

        Assert.Equal(lastImageNumber, productImageFileNames.Max(x => x.Id));
        Assert.Equal(lastDisplayOrder, productImageFileNames.Max(x => x.DisplayOrder));

        Assert.DoesNotContain(productImageFileNames, x =>
            x.ProductId == productId
            && x.FileName == createRequest1.FileName
            && x.DisplayOrder == createRequest1.DisplayOrder);

        for (int i = 0; i < productImageFileNames.Count; i++)
        {
            int? displayOrder = productImageFileNames[i].DisplayOrder;

            if (displayOrder is null) continue;

            Assert.Equal(i + 1, displayOrder);
        }
    }

    [Fact]
    public void DeleteByProductIdAndImageNumber_ShouldFail_WhenProductIdIsInvalid()
    {
        const string firstFileName1 = "11111.png";
        const string firstFileName2 = "12222.png";

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        int productId = productInsertResult.AsT0;

        int lastImageNumber = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;
        int lastDisplayOrder = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, firstFileName1, displayOrder: lastDisplayOrder);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, firstFileName2, displayOrder: lastDisplayOrder + 1);

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

        bool deleteSuccess = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(0, lastDisplayOrder);

        Assert.False(deleteSuccess);

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        Assert.Equal(lastImageNumber + 1, productImageFileNames.Count());

        Assert.Equal(lastImageNumber + 1, productImageFileNames.Max(x => x.Id));
        Assert.Equal(lastDisplayOrder + 1, productImageFileNames.Max(x => x.DisplayOrder));

        Assert.Contains(productImageFileNames, x =>
        x.ProductId == productId
        && x.FileName == createRequest1.FileName
        && x.DisplayOrder == createRequest1.DisplayOrder);
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

        int lastDisplayOrder = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, firstFileName1, displayOrder: lastDisplayOrder);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, firstFileName2, displayOrder: lastDisplayOrder + 1);

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

        bool deleteSuccess = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(productId, lastDisplayOrder);

        Assert.True(deleteSuccess);

        List<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId).ToList();

        Assert.DoesNotContain(productImageFileNames, x =>
        x.ProductId == productId
        && x.FileName == createRequest1.FileName
        && x.DisplayOrder == createRequest1.DisplayOrder);

        for (int i = 0; i < productImageFileNames.Count; i++)
        {
            int? displayOrder = productImageFileNames[i].DisplayOrder;

            if (displayOrder is null) continue;

            Assert.Equal(i + 1, displayOrder);
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

        int lastDisplayOrder = (ValidProductCreateRequest.ImageFileNames?.Count ?? 0) + 1;

        ServiceProductImageFileNameInfoCreateRequest createRequest1 = GetCreateRequest(productId, firstFileName1, displayOrder: lastDisplayOrder);
        ServiceProductImageFileNameInfoCreateRequest createRequest2 = GetCreateRequest(productId, firstFileName2, displayOrder: lastDisplayOrder + 1);

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

        bool deleteSuccess = _productImageFileNameInfoService.DeleteByProductIdAndDisplayOrder(0, lastDisplayOrder);

        Assert.False(deleteSuccess);

        IEnumerable<ProductImageFileData> productImageFileNames = _productImageFileNameInfoService.GetAllInProduct(productId);

        Assert.Contains(productImageFileNames, x =>
        x.ProductId == productId
        && x.FileName == createRequest1.FileName
        && x.DisplayOrder == createRequest1.DisplayOrder);
    }
}