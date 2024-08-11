using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using System.Transactions;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.LocalChangesHandling.Tests.Integration.CommonTestElements;
using MOSTComputers.Models.Product.Models.ProductStatuses;

namespace MOSTComputers.Services.LocalChangesHandling.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductChangesServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductChangesServiceTests(
        IProductChangesService productChangesService,
        ILocalChangesService localChangesService,
        IProductService productService,
        IProductStatusesService productStatusesService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _productChangesService = productChangesService;
        _localChangesService = localChangesService;
        _productService = productService;
        _productStatusesService = productStatusesService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
    }

    private const int _useRequiredIdValue = -1877;

    private readonly IProductChangesService _productChangesService;
    private readonly ILocalChangesService _localChangesService;
    private readonly IProductService _productService;
    private readonly IProductStatusesService _productStatusesService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public void HandleInsert_ShouldSucceed_WhenConnectionToDBIsSuccessful_AndDataIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        LocalChangeData? changeDataForProduct =
            _localChangesService.GetByTableNameAndElementIdAndOperationType(tableChangeNameOfProductsTable, (int)productIdFromInsertResult, ChangeOperationTypeEnum.Create);

        Assert.NotNull(changeDataForProduct);

        Assert.True(changeDataForProduct.TableName == tableChangeNameOfProductsTable
            && changeDataForProduct.TableElementId == productIdFromInsertResult
            && changeDataForProduct.OperationType == ChangeOperationTypeEnum.Create);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleInsertResult = _productChangesService.HandleInsert(changeDataForProduct);

        Assert.True(handleInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductStatuses? productStatuses = _productStatusesService.GetByProductId(productIdFromInsertResult.Value);

        Assert.NotNull(productStatuses);

        Assert.True(productStatuses.ProductId == productIdFromInsertResult.Value
            && productStatuses.IsProcessed == true
            && productStatuses.NeedsToBeUpdated == false);

        LocalChangeData? changeDataForProductHandleInsert
            = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Create);

        Assert.Null(changeDataForProductHandleInsert);
    }

    [Theory]
    [MemberData(nameof(HandleInsert_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsNotValid_Data))]
    public void HandleInsert_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsNotValid(LocalChangeData invalidChangeDataForProduct)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleInsertResult = _productChangesService.HandleInsert(invalidChangeDataForProduct);

        Assert.True(handleInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        //Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));
    }

    public static TheoryData<LocalChangeData> HandleInsert_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsNotValid_Data => new()
    {
        new LocalChangeData()
        {
            TableName = tableChangeNameOfProductsTable,
            TableElementId = -20,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = tableChangeNameOfProductsTable,
            TableElementId = 0,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "      ",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "not product table name dfk;sdfkflkjjej",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },
    };

    [Fact]
    public void HandleInsert_ShouldFail_WhenConnectionToDBIsSuccessful_ButProductIsAlreadyDeleted()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        bool isProductDeleted = _productService.Delete(productIdFromInsertResult.Value);

        Assert.True(isProductDeleted);

        LocalChangeData? changeDataForProduct
            = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Create);

        Assert.Null(changeDataForProduct);

        LocalChangeData simulatedInsertDataForProduct = new()
        {
            OperationType = ChangeOperationTypeEnum.Create,
            TableElementId = (int)productIdFromInsertResult,
            TableName = tableChangeNameOfProductsTable,
            Id = 100000,
            TimeStamp = DateTime.Now,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleInsertResult = _productChangesService.HandleInsert(simulatedInsertDataForProduct);

        Assert.True(handleInsertResult.Match(
            success => false,
            validationResult => false,
            unexpectedFailureResult => true));

        //Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));
    }

    //public static TheoryData<LocalChangeData> HandleInsert_ShouldFailAndRollback_WhenConnectionToDBIsSuccessful_ButAnOperationFails_Data => new()
    //{
    //    new LocalChangeData()
    //    {
    //        TableName = tableNameOfProductsTable,
    //        TableElementId = -20,
    //        OperationType = ChangeOperationTypeEnum.Create
    //    },

    //    new LocalChangeData()
    //    {
    //        TableName = tableNameOfProductsTable,
    //        TableElementId = 0,
    //        OperationType = ChangeOperationTypeEnum.Create
    //    }
    //};

    [Fact]
    public void HandleInsert_ShouldFailAndRollBack_WhenConnectionToDBIsSuccessful_ButTransactionTimesOut()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        LocalChangeData? changeDataForProduct
            = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Create);

        Assert.NotNull(changeDataForProduct);

        Assert.True(changeDataForProduct.TableName == tableChangeNameOfProductsTable
            && changeDataForProduct.TableElementId == productIdFromInsertResult
            && changeDataForProduct.OperationType == ChangeOperationTypeEnum.Create);

        OneOf<Success, ValidationResult, UnexpectedFailureResult>? handleInsertResult = null;

        try
        {
            using TransactionScope outerScope = new(TransactionScopeOption.Required, new TransactionOptions() { Timeout = new TimeSpan(0, 0, 0, 0, 5) });

            handleInsertResult = _productChangesService.HandleInsert(changeDataForProduct);

            outerScope.Complete();
        }
        catch(TransactionAbortedException transactionAbortedEx)
        {
            Assert.True(transactionAbortedEx.InnerException is TimeoutException);
        }

        Assert.NotNull(handleInsertResult);

        Assert.True(handleInsertResult.Value.Match(
            success => false,
            validationResult => false,
            unexpectedFailureResult => true));

        //Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));
    }

    [Fact]
    public void HandleUpdate_ShouldSucceed_WhenConnectionToDBIsSuccessful_AndDataIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        ProductUpdateRequest productUpdateRequest = GetValidProductUpdateRequestWithNoImages(productIdFromInsertResult.Value);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        Assert.True(productUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        LocalChangeData? changeDataForProduct
            = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Update);

        Assert.NotNull(changeDataForProduct);

        Assert.True(changeDataForProduct.TableName == tableChangeNameOfProductsTable
            && changeDataForProduct.TableElementId == productIdFromInsertResult
            && changeDataForProduct.OperationType == ChangeOperationTypeEnum.Update);

        Product? productBeforeUpdate = GetProductFull(productIdFromInsertResult.Value);

        Assert.NotNull(productBeforeUpdate);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleUpdateResult = _productChangesService.HandleUpdate(changeDataForProduct);

        Assert.True(handleUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        Product? productAfterUpdate = GetProductFull(productIdFromInsertResult.Value);

        Assert.NotNull(productAfterUpdate);

        ProductStatuses? productStatusAfterUpdate = _productStatusesService.GetByProductId(productIdFromInsertResult.Value);

        Assert.NotNull(productStatusAfterUpdate);

        Assert.Equal(productIdFromInsertResult.Value, productStatusAfterUpdate.ProductId);
        Assert.Equal(DetermineWhetherProductIsProcessedOrNot(productAfterUpdate), productStatusAfterUpdate.IsProcessed);
        Assert.Equal(DetermineWhetherUpdateIsImportant(productBeforeUpdate, productAfterUpdate), productStatusAfterUpdate.NeedsToBeUpdated);
    }

    [Theory]
    [MemberData(nameof(HandleUpdate_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsInvalid_Data))]
    public void HandleUpdate_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsInvalid(LocalChangeData invalidChangeData)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        ProductUpdateRequest productUpdateRequest = GetValidProductUpdateRequestWithNoImages(productIdFromInsertResult.Value);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        Assert.True(productUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        if (invalidChangeData.TableElementId == _useRequiredIdValue)
        {
            invalidChangeData.TableElementId = (int)productIdFromInsertResult;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleUpdateResult = _productChangesService.HandleUpdate(invalidChangeData);

        Assert.True(handleUpdateResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Product? productAfterUpdate = GetProductFull(productIdFromInsertResult.Value);

        Assert.NotNull(productAfterUpdate);

        ProductStatuses? productStatusAfterUpdate = _productStatusesService.GetByProductId((int)productIdFromInsertResult.Value);

        if (productStatusAfterUpdate is not null)
        {
            Assert.Equal((int)productIdFromInsertResult, productStatusAfterUpdate.ProductId);
            Assert.Equal(DetermineWhetherProductIsProcessedOrNot(productAfterUpdate), productStatusAfterUpdate.IsProcessed);
            Assert.False(productStatusAfterUpdate.NeedsToBeUpdated);
        }
    }

    public static TheoryData<LocalChangeData> HandleUpdate_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsInvalid_Data => new()
    {
        new LocalChangeData()
        {
            TableName = tableChangeNameOfProductsTable,
            TableElementId = -20,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = tableChangeNameOfProductsTable,
            TableElementId = 0,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "      ",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "not product table name dfk;sdfkflkjjej",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        }
    };

    [Fact]
    public void HandleUpdate_ShouldFail_WhenConnectionToDBIsSuccessful_ButProductIsAlreadyDeleted()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        ProductUpdateRequest productUpdateRequest = GetValidProductUpdateRequestWithNoImages(productIdFromInsertResult.Value);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        Assert.True(productUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        bool productDeleteSuccess = _productService.Delete(productIdFromInsertResult.Value);

        Assert.True(productDeleteSuccess);

        LocalChangeData? changeDataForProduct
            = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Update);

        Assert.Null(changeDataForProduct);

        LocalChangeData simulatedUpdateDataForProduct = new()
        {
            OperationType = ChangeOperationTypeEnum.Update,
            TableElementId = (int)productIdFromInsertResult,
            TableName = tableChangeNameOfProductsTable,
            Id = 100000,
            TimeStamp = DateTime.Now,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleUpdateResult = _productChangesService.HandleUpdate(simulatedUpdateDataForProduct);

        Assert.True(handleUpdateResult.Match(
            success => false,
            validationResult => false,
            unexpectedFailureResult => true));

        //Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));
    }

    [Fact]
    public void HandleUpdate_ShouldFail_WhenConnectionToDBIsSuccessful_ButTransactionTimesOut()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        ProductUpdateRequest productUpdateRequest = GetValidProductUpdateRequestWithNoImages(productIdFromInsertResult.Value);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        Assert.True(productUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        LocalChangeData? changeDataForProduct
             = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                 tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Update);

        Assert.NotNull(changeDataForProduct);

        Assert.True(changeDataForProduct.TableName == tableChangeNameOfProductsTable
            && changeDataForProduct.TableElementId == productIdFromInsertResult
            && changeDataForProduct.OperationType == ChangeOperationTypeEnum.Update);

        OneOf<Success, ValidationResult, UnexpectedFailureResult>? handleUpdateResult = null;

        using (TransactionScope outerScope = new(TransactionScopeOption.Required, new TransactionOptions() { Timeout = TimeSpan.FromMilliseconds(5) }))
        {
            handleUpdateResult = _productChangesService.HandleUpdate(changeDataForProduct);

            outerScope.Complete();
        }

        Assert.NotNull(handleUpdateResult);

        Assert.True(handleUpdateResult.Value.Match(
            success => false,
            validationResult => false,
            unexpectedFailureResult => true));

        //Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));
    }

    [Fact]
    public void HandleDelete_ShouldSucceed_WhenConnectionToDBIsSuccessful_AndDataIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        bool isProductDeleted = _productService.Delete(productIdFromInsertResult.Value);

        Assert.True(isProductDeleted);

        LocalChangeData? changeDataForProduct =
            _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Delete);

        Assert.NotNull(changeDataForProduct);

        Assert.True(changeDataForProduct.TableName == tableChangeNameOfProductsTable
            && changeDataForProduct.TableElementId == productIdFromInsertResult
            && changeDataForProduct.OperationType == ChangeOperationTypeEnum.Delete);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleDeleteResult = _productChangesService.HandleDelete(changeDataForProduct);

        Assert.True(handleDeleteResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));

        LocalChangeData? changeDataForProductHandleDelete
            = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, (int)productIdFromInsertResult, ChangeOperationTypeEnum.Create);

        Assert.Null(changeDataForProductHandleDelete);
    }

    [Theory]
    [MemberData(nameof(HandleDelete_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsInvalid_Data))]
    public void HandleDelete_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsInvalid(LocalChangeData invalidChangeDataForProduct)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        bool isProductDeleted = _productService.Delete(productIdFromInsertResult.Value);

        Assert.True(isProductDeleted);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> handleDeleteResult = _productChangesService.HandleDelete(invalidChangeDataForProduct);

        Assert.True(handleDeleteResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));

        LocalChangeData? changeDataForProductHandleDelete
            = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Create);

        Assert.Null(changeDataForProductHandleDelete);
    }

    public static TheoryData<LocalChangeData> HandleDelete_ShouldFail_WhenConnectionToDBIsSuccessful_ButDataIsInvalid_Data => new()
    {
        new LocalChangeData()
        {
            TableName = tableChangeNameOfProductsTable,
            TableElementId = -20,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = tableChangeNameOfProductsTable,
            TableElementId = 0,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "      ",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },

        new LocalChangeData()
        {
            TableName = "not product table name dfk;sdfkflkjjej",
            TableElementId = _useRequiredIdValue,
            OperationType = ChangeOperationTypeEnum.Create
        },
    };

    [Fact]
    public void HandleDelete_ShouldFail_WhenConnectionToDBIsSuccessful_ButTransactionTimesOut()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productIdFromInsertResult = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdFromInsertResult);
        Assert.True(productIdFromInsertResult > 0);

        bool productUpdateResult = _productService.Delete(productIdFromInsertResult.Value);

        Assert.True(productUpdateResult);

        LocalChangeData? changeDataForProduct
             = _localChangesService.GetByTableNameAndElementIdAndOperationType(
                 tableChangeNameOfProductsTable, productIdFromInsertResult.Value, ChangeOperationTypeEnum.Delete);

        Assert.NotNull(changeDataForProduct);

        Assert.True(changeDataForProduct.TableName == tableChangeNameOfProductsTable
            && changeDataForProduct.TableElementId == productIdFromInsertResult
            && changeDataForProduct.OperationType == ChangeOperationTypeEnum.Delete);

        OneOf<Success, ValidationResult, UnexpectedFailureResult>? handleDeleteResult = null;

        using (TransactionScope outerScope = new(TransactionScopeOption.Required, new TransactionOptions() { Timeout = new TimeSpan(0, 0, 0, 0, 5) }))
        {
            handleDeleteResult = _productChangesService.HandleDelete(changeDataForProduct);

            outerScope.Complete();
        }

        Assert.NotNull(handleDeleteResult);

        Assert.True(handleDeleteResult.Value.Match(
            success => false,
            validationResult => false,
            unexpectedFailureResult => true));

        Assert.True(AreAllPartsOfAProductDeleted(productIdFromInsertResult.Value));
    }

    private bool AreAllPartsOfAProductDeleted(int productId)
    {
        Product? product = _productService.GetByIdWithImages(productId);

        if (product is not null) return false;

        IEnumerable<ProductImage> productImages = _productImageService.GetAllInProduct(productId);

        if (productImages.Any()) return false;

        ProductImage? productFirstImage = _productImageService.GetFirstImageForProduct(productId);

        if (productFirstImage is not null) return false;

        IEnumerable<ProductImageFileNameInfo> productImageFileNameInfos = _productImageFileNameInfoService.GetAllInProduct(productId);

        if (productImageFileNameInfos.Any()) return false;

        IEnumerable<ProductProperty> productProperties = _productPropertyService.GetAllInProduct(productId);

        if (productProperties.Any()) return false;

        ProductStatuses? productStatuses = _productStatusesService.GetByProductId(productId);

        if (productStatuses is not null) return false;

        return true;
    }

    private static bool DetermineWhetherProductIsProcessedOrNot(Product productFull)
    {
        if (productFull is null)
        {
            return false;
        }

        if (productFull.Properties is not null
            && productFull.Properties.Count > 0)
        {
            return true;
        }

        if (productFull.Images is not null
            && productFull.Images.Count > 0)
        {
            return true;
        }

        return false;
    }

    private Product? GetProductFull(int productId)
    {
        Product? product = _productService.GetByIdWithProps(productId);

        if (product is null) return null;

        if (product.Images is null
            || product.Images.Count == 0)
        {
            product.Images = _productImageService.GetAllInProduct((int)productId)
                .ToList();
        }

        if (product.ImageFileNames is null
            || product.ImageFileNames.Count == 0)
        {
            product.ImageFileNames = _productImageFileNameInfoService.GetAllInProduct((int)productId)
                .ToList();
        }

        return product;
    }

    private bool DetermineWhetherUpdateIsImportant(Product productBeforeUpdate, Product productAfterUpdate)
    {
        if (productBeforeUpdate.SearchString != productAfterUpdate.SearchString
            || productBeforeUpdate.Status != productAfterUpdate.Status
            || productBeforeUpdate.CategoryID != productAfterUpdate.CategoryID
            || productBeforeUpdate.ManifacturerId != productAfterUpdate.ManifacturerId
            || productBeforeUpdate.Promotionid != productAfterUpdate.Promotionid
            || productBeforeUpdate.PromotionPictureId != productAfterUpdate.PromotionPictureId)
        {
            return true;
        }

        if (productBeforeUpdate.Properties is null
            || productBeforeUpdate.Properties.Count <= 0)
        {
            productBeforeUpdate.Properties = _productPropertyService.GetAllInProduct(productBeforeUpdate.Id)
                .ToList();
        }

        if (productAfterUpdate.Properties is null
            || productAfterUpdate.Properties.Count <= 0)
        {
            productAfterUpdate.Properties = _productPropertyService.GetAllInProduct(productAfterUpdate.Id)
                .ToList();
        }

        if (!CompareDataInPropertyCollections(productBeforeUpdate.Properties, productAfterUpdate.Properties))
        {
            return true;
        }

        return false;
    }

    private static bool CompareDataInPropertyCollections(List<ProductProperty> productProperties1, List<ProductProperty> productProperties2)
    {
        if (productProperties1.Count != productProperties2.Count)
        {
            return false;
        }

        productProperties1 = productProperties1
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        productProperties2 = productProperties2
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        for (int i = 0; i < productProperties1.Count; i++)
        {
            ProductProperty prop1 = productProperties1[i];
            ProductProperty prop2 = productProperties2[i];

            if (prop1.ProductCharacteristicId != prop2.ProductCharacteristicId
                || prop1.DisplayOrder != prop2.DisplayOrder
                || prop1.XmlPlacement != prop2.XmlPlacement) return false;
        }

        return true;
    }
}