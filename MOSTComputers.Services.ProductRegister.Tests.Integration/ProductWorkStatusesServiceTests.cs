using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.SuccessfulInsertAbstractions;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductWorkStatusesServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductWorkStatusesServiceTests(
        IProductWorkStatusesService productWorkStatusesService,
        IProductService productService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _productWorkStatusesService = productWorkStatusesService;
        _productService = productService;
    }

    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IProductService _productService;

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    /* IProductWorkStatusesService methods:
    
    IEnumerable<ProductWorkStatuses> GetAll();
    IEnumerable<ProductWorkStatuses> GetAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum);
    IEnumerable<ProductWorkStatuses> GetAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum);
    IEnumerable<ProductWorkStatuses> GetAllWithReadyForImageInsert(bool readyForImageInsert);
    ProductWorkStatuses? GetByProductId(int productId);
    ProductWorkStatuses? GetById(int productWorkStatusesId);
    OneOf<int, ValidationResult, UnexpectedFailureResult> InsertIfItDoesntExist(ProductWorkStatusesCreateRequest createRequest, IValidator<ProductWorkStatusesCreateRequest>? validator = null);
    OneOf<bool, ValidationResult> UpdateByProductId(ProductWorkStatusesUpdateByProductIdRequest updateRequest, IValidator<ProductWorkStatusesUpdateByProductIdRequest>? validator = null);
    OneOf<bool, ValidationResult> UpdateById(ProductWorkStatusesUpdateByIdRequest updateRequest, IValidator<ProductWorkStatusesUpdateByIdRequest>? validator = null);
    bool DeleteAll();
    bool DeleteAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum);
    bool DeleteAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum);
    bool DeleteAllWithReadyForImageInsert(bool readyForImageInsert);
    bool DeleteByProductId(int productId);
    bool DeleteById(int productWorkStatusesId);
    */

#pragma warning disable CA2211 // Non-constant fields should not be visible

    [Fact]
    public void GetAll_ShouldGetAll_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.True(workStatuses.Count() >= 2);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);
    }

    [Fact]
    public void GetAll_ShouldGetOnlySuccesfullyInserted_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = -1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -2);

        Assert.Equal(-1, workStatusId2);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.NotEmpty(workStatuses);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);
    }

    [Fact]
    public void GetAllWithProductNewStatus_ShouldGetAllWithProductNewStatus_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAllWithProductNewStatus(ProductNewStatusEnum.New);

        Assert.True(workStatuses.Count() >= 2);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void GetAllWithProductNewStatus_ShouldGetOnlySuccessfullyInsertedWithProductNewStatus_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = -1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -2);

        Assert.Equal(-1, workStatusId2);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAllWithProductNewStatus(ProductNewStatusEnum.New);

        Assert.NotEmpty(workStatuses);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void GetAllWithProductXmlStatus_ShouldGetAllWithProductXmlStatus_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAllWithProductXmlStatus(ProductXmlStatusEnum.NotReady);

        Assert.True(workStatuses.Count() >= 2);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void GetAllWithProductXmlStatus_ShouldGetOnlySuccessfullyInsertedWithProductXmlStatus_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = -1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -2);

        Assert.Equal(-1, workStatusId2);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAllWithProductXmlStatus(ProductXmlStatusEnum.NotReady);

        Assert.NotEmpty(workStatuses);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void GetAllWithReadyForImageInsert_ShouldGetAllWithSameReadyForImageInsert_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAllWithReadyForImageInsert(false);

        Assert.True(workStatuses.Count() >= 2);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void GetAllWithReadyForImageInsert_ShouldGetOnlySuccessfullyInsertedWithSameReadyForImageInsert_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = -1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -2);

        Assert.Equal(-1, workStatusId2);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAllWithReadyForImageInsert(false);

        Assert.NotEmpty(workStatuses);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void GetByProductId_ShouldGetWithSameProductId_WhenRecordExists()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        Assert.NotNull(productWorkStatuses);

        Assert.Equal(workStatusId, productWorkStatuses.Id);
        Assert.Equal(productId, productWorkStatuses.ProductId);
        Assert.Equal(workStatusesCreateRequest.ProductNewStatus, productWorkStatuses.ProductNewStatus);
        Assert.Equal(workStatusesCreateRequest.ProductXmlStatus, productWorkStatuses.ProductXmlStatus);
        Assert.Equal(workStatusesCreateRequest.ReadyForImageInsert, productWorkStatuses.ReadyForImageInsert);
    }

    [Fact]
    public void GetByProductId_ShouldFailGetWithSameProductId_WhenRecordDoesntExist()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(-1);

        Assert.Null(productWorkStatuses);
    }

    [Fact]
    public void GetById_ShouldGetWithSameId_WhenRecordExists()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetById(workStatusId);

        Assert.NotNull(productWorkStatuses);

        Assert.Equal(workStatusId, productWorkStatuses.Id);
        Assert.Equal(productId, productWorkStatuses.ProductId);
        Assert.Equal(workStatusesCreateRequest.ProductNewStatus, productWorkStatuses.ProductNewStatus);
        Assert.Equal(workStatusesCreateRequest.ProductXmlStatus, productWorkStatuses.ProductXmlStatus);
        Assert.Equal(workStatusesCreateRequest.ReadyForImageInsert, productWorkStatuses.ReadyForImageInsert);
    }

    [Fact]
    public void GetById_ShouldFailGetWithSameId_WhenRecordDoesntExist()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetById(-1);

        Assert.Null(productWorkStatuses);
    }

    [Theory]
    [MemberData(nameof(InsertIfItDoesntExist_ShouldSucceedOrFailToInsert_WhenExpected_Data))]
    public void InsertIfItDoesntExist_ShouldSucceedOrFailToInsert_WhenExpected(ProductWorkStatusesCreateRequest workStatusesCreateRequest, bool expected)
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        if (workStatusesCreateRequest.ProductId == UseRequiredValuePlaceholder)
        {
            workStatusesCreateRequest.ProductId = productId;
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.Equal(expected, workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetById(workStatusId);

        Assert.Equal(expected, productWorkStatuses is not null);

        if (productWorkStatuses is not null)
        {
            Assert.Equal(workStatusId, productWorkStatuses.Id);
            Assert.Equal(productId, productWorkStatuses.ProductId);
            Assert.Equal(workStatusesCreateRequest.ProductNewStatus, productWorkStatuses.ProductNewStatus);
            Assert.Equal(workStatusesCreateRequest.ProductXmlStatus, productWorkStatuses.ProductXmlStatus);
            Assert.Equal(workStatusesCreateRequest.ReadyForImageInsert, productWorkStatuses.ReadyForImageInsert);
        }
    }

    public static TheoryData<ProductWorkStatusesCreateRequest, bool> InsertIfItDoesntExist_ShouldSucceedOrFailToInsert_WhenExpected_Data = new()
    {
        {
            new ProductWorkStatusesCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            true
        },

        {
            new ProductWorkStatusesCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
                ReadyForImageInsert = false,
            },
            true
        },

        {
            new ProductWorkStatusesCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesCreateRequest()
            {
                ProductId = 0,
                ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            false
        },

        {
            new ProductWorkStatusesCreateRequest()
            {
                ProductId = -1,
                ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            false
        },
    };

    [Fact]
    public void InsertIfItDoesntExist_ShouldFailToInsert_WhenProductDoesntExist()
    {
        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = 0,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        bool hasWorkStatusInsertFailedWithValidationResult = insertProductWorkStatusResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(hasWorkStatusInsertFailedWithValidationResult);
    }

    [Theory]
    [MemberData(nameof(UpdateByProductId_ShouldSucceedOrFailToUpdate_WhenExpected_Data))]
    public void UpdateByProductId_ShouldSucceedOrFailToUpdate_WhenExpected(
        ProductWorkStatusesUpdateByProductIdRequest workStatusesUpdateRequest, bool expected)
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        if (workStatusesUpdateRequest.ProductId == UseRequiredValuePlaceholder)
        {
            workStatusesUpdateRequest.ProductId = productId;
        }

        OneOf<bool, ValidationResult> workStatusUpdateResult = _productWorkStatusesService.UpdateByProductId(workStatusesUpdateRequest);

        bool isUpdateSuccessful = workStatusUpdateResult.Match(
            isSuccessful => isSuccessful,
            validationResult => false);

        Assert.Equal(expected, isUpdateSuccessful);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetById(workStatusId);

        Assert.NotNull(productWorkStatuses);

        if (expected)
        {
            Assert.Equal(workStatusId, productWorkStatuses.Id);
            Assert.Equal(productId, productWorkStatuses.ProductId);
            Assert.Equal(workStatusesUpdateRequest.ProductNewStatus, productWorkStatuses.ProductNewStatus);
            Assert.Equal(workStatusesUpdateRequest.ProductXmlStatus, productWorkStatuses.ProductXmlStatus);
            Assert.Equal(workStatusesUpdateRequest.ReadyForImageInsert, productWorkStatuses.ReadyForImageInsert);
        }
        else
        {
            Assert.Equal(workStatusId, productWorkStatuses.Id);
            Assert.Equal(productId, productWorkStatuses.ProductId);
            Assert.Equal(workStatusesCreateRequest.ProductNewStatus, productWorkStatuses.ProductNewStatus);
            Assert.Equal(workStatusesCreateRequest.ProductXmlStatus, productWorkStatuses.ProductXmlStatus);
            Assert.Equal(workStatusesCreateRequest.ReadyForImageInsert, productWorkStatuses.ReadyForImageInsert);
        }
    }

    public static TheoryData<ProductWorkStatusesUpdateByProductIdRequest, bool> UpdateByProductId_ShouldSucceedOrFailToUpdate_WhenExpected_Data = new()
    {
        {
            new ProductWorkStatusesUpdateByProductIdRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByProductIdRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
                ReadyForImageInsert = false,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByProductIdRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByProductIdRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByProductIdRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByProductIdRequest()
            {
                ProductId = 0,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            false
        },

        {
            new ProductWorkStatusesUpdateByProductIdRequest()
            {
                ProductId = -1,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateById_ShouldSucceedOrFailToUpdate_WhenExpected_Data))]
    public void UpdateById_ShouldSucceedOrFailToUpdate_WhenExpected(
        ProductWorkStatusesUpdateByIdRequest workStatusesUpdateRequest, bool expected)
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        if (workStatusesUpdateRequest.Id == UseRequiredValuePlaceholder)
        {
            workStatusesUpdateRequest.Id = workStatusId;
        }

        OneOf<bool, ValidationResult> workStatusUpdateResult = _productWorkStatusesService.UpdateById(workStatusesUpdateRequest);

        bool isUpdateSuccessful = workStatusUpdateResult.Match(
            isSuccessful => isSuccessful,
            validationResult => false);

        Assert.Equal(expected, isUpdateSuccessful);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetById(workStatusId);

        Assert.NotNull(productWorkStatuses);

        if (expected)
        {
            Assert.Equal(workStatusId, productWorkStatuses.Id);
            Assert.Equal(productId, productWorkStatuses.ProductId);
            Assert.Equal(workStatusesUpdateRequest.ProductNewStatus, productWorkStatuses.ProductNewStatus);
            Assert.Equal(workStatusesUpdateRequest.ProductXmlStatus, productWorkStatuses.ProductXmlStatus);
            Assert.Equal(workStatusesUpdateRequest.ReadyForImageInsert, productWorkStatuses.ReadyForImageInsert);
        }
        else
        {
            Assert.Equal(workStatusId, productWorkStatuses.Id);
            Assert.Equal(productId, productWorkStatuses.ProductId);
            Assert.Equal(workStatusesCreateRequest.ProductNewStatus, productWorkStatuses.ProductNewStatus);
            Assert.Equal(workStatusesCreateRequest.ProductXmlStatus, productWorkStatuses.ProductXmlStatus);
            Assert.Equal(workStatusesCreateRequest.ReadyForImageInsert, productWorkStatuses.ReadyForImageInsert);
        }
    }

    public static TheoryData<ProductWorkStatusesUpdateByIdRequest, bool> UpdateById_ShouldSucceedOrFailToUpdate_WhenExpected_Data = new()
    {
        {
            new ProductWorkStatusesUpdateByIdRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByIdRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
                ReadyForImageInsert = false,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByIdRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByIdRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByIdRequest()
            {
                Id = UseRequiredValuePlaceholder,
                ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = true,
            },
            true
        },

        {
            new ProductWorkStatusesUpdateByIdRequest()
            {
                Id = 0,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            false
        },

        {
            new ProductWorkStatusesUpdateByIdRequest()
            {
                Id = -1,
                ProductNewStatus = ProductNewStatusEnum.New,
                ProductXmlStatus = ProductXmlStatusEnum.NotReady,
                ReadyForImageInsert = false,
            },
            false
        },
    };

    [Fact]
    public void DeleteAll_ShouldDeleteAll_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> productWorkStatuses = _productWorkStatusesService.GetAll();

        Assert.True(productWorkStatuses.Count() >= 3);

        bool isDeleteAllSuccessful = _productWorkStatusesService.DeleteAll();

        Assert.True(isDeleteAllSuccessful);

        IEnumerable<ProductWorkStatuses> productWorkStatusesAfterDelete = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(productWorkStatusesAfterDelete, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(productWorkStatusesAfterDelete, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.DoesNotContain(productWorkStatusesAfterDelete, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteAll_ShouldFailToDelete_WhenNoRecordsExist()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = 0,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.Equal(-1, workStatusId);

        bool isDeleteAllSuccessful = _productWorkStatusesService.DeleteAll();

        Assert.False(isDeleteAllSuccessful);

        IEnumerable<ProductWorkStatuses> productWorkStatusesAfterDelete = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(productWorkStatusesAfterDelete, x =>
            x.Id == workStatusId
            && x.ProductId == productId
            && x.ProductNewStatus == workStatusesCreateRequest.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteAllWithProductNewStatus_ShouldDeleteAllWithSameProductNewStatus_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);

        bool isDeleteByProductNewStatusSuccessful = _productWorkStatusesService.DeleteAllWithProductNewStatus(ProductNewStatusEnum.New);

        IEnumerable<ProductWorkStatuses> workStatusesAfterUpdate = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatusesAfterUpdate, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteAllWithProductNewStatus_ShouldDeleteOnlySuccessfullyInsertedWithSameProductNewStatus_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = 0,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.Equal(-1, workStatusId2);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);

        bool isDeleteByProductNewStatusSuccessful = _productWorkStatusesService.DeleteAllWithProductNewStatus(ProductNewStatusEnum.New);

        IEnumerable<ProductWorkStatuses> workStatusesAfterUpdate = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatusesAfterUpdate, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteAllWithProductXmlStatus_ShouldDeleteAllWithSameProductXmlStatus_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);

        bool isDeleteByProductNewStatusSuccessful = _productWorkStatusesService.DeleteAllWithProductXmlStatus(ProductXmlStatusEnum.NotReady);

        IEnumerable<ProductWorkStatuses> workStatusesAfterUpdate = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatusesAfterUpdate, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteAllWithProductXmlStatus_ShouldDeleteOnlySuccessfullyInsertedWithSameProductXmlStatus_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = 0,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.Equal(-1, workStatusId2);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);

        bool isDeleteByProductNewStatusSuccessful = _productWorkStatusesService.DeleteAllWithProductXmlStatus(ProductXmlStatusEnum.NotReady);

        IEnumerable<ProductWorkStatuses> workStatusesAfterUpdate = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatusesAfterUpdate, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteAllWithReadyForImageInsert_ShouldDeleteAllWithSameReadyForImageInsert_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = productId2,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId2 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.WorkInProgress,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);

        bool isDeleteByProductNewStatusSuccessful = _productWorkStatusesService.DeleteAllWithReadyForImageInsert(false);

        IEnumerable<ProductWorkStatuses> workStatusesAfterUpdate = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatusesAfterUpdate, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteAllWithReadyForImageInsert_ShouldDeleteOnlySuccessfullyInsertedWithSameReadyForImageInsert_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId3 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest1 = new()
        {
            ProductId = productId1,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult1
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest1);

        int workStatusId1 = insertProductWorkStatusResult1.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId1 > 0);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest2 = new()
        {
            ProductId = 0,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult2
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest2);

        int workStatusId2 = insertProductWorkStatusResult2.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.Equal(-1, workStatusId2);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest3 = new()
        {
            ProductId = productId3,
            ProductNewStatus = ProductNewStatusEnum.WorkInProgress,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = true,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult3
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest3);

        int workStatusId3 = insertProductWorkStatusResult3.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId3 > 0);

        IEnumerable<ProductWorkStatuses> workStatuses = _productWorkStatusesService.GetAll();

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatuses, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatuses, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);

        bool isDeleteByProductNewStatusSuccessful = _productWorkStatusesService.DeleteAllWithReadyForImageInsert(false);

        IEnumerable<ProductWorkStatuses> workStatusesAfterUpdate = _productWorkStatusesService.GetAll();

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId1
            && x.ProductId == productId1
            && x.ProductNewStatus == workStatusesCreateRequest1.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest1.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest1.ReadyForImageInsert);

        Assert.DoesNotContain(workStatusesAfterUpdate, x =>
            x.Id == workStatusId2
            && x.ProductId == productId2
            && x.ProductNewStatus == workStatusesCreateRequest2.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest2.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest2.ReadyForImageInsert);

        Assert.Contains(workStatusesAfterUpdate, x =>
            x.Id == workStatusId3
            && x.ProductId == productId3
            && x.ProductNewStatus == workStatusesCreateRequest3.ProductNewStatus
            && x.ProductXmlStatus == workStatusesCreateRequest3.ProductXmlStatus
            && x.ReadyForImageInsert == workStatusesCreateRequest3.ReadyForImageInsert);
    }

    [Fact]
    public void DeleteByProductId_ShouldDeleteWithSameProductId_WhenRecordExists()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        Assert.NotNull(productWorkStatuses);

        bool isDeleteByProductIdSuccessful = _productWorkStatusesService.DeleteByProductId(productId);

        Assert.True(isDeleteByProductIdSuccessful);

        ProductWorkStatuses? productWorkStatusesAfterDelete = _productWorkStatusesService.GetByProductId(productId);

        Assert.Null(productWorkStatusesAfterDelete);
    }

    [Fact]
    public void DeleteByProductId_ShouldFailToDeleteWithSameProductId_WhenRecordDoesntExist()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetByProductId(productId);

        Assert.NotNull(productWorkStatuses);

        bool isDeleteByProductIdSuccessful = _productWorkStatusesService.DeleteByProductId(-1);

        Assert.False(isDeleteByProductIdSuccessful);

        ProductWorkStatuses? productWorkStatusesAfterDelete = _productWorkStatusesService.GetByProductId(productId);

        Assert.NotNull(productWorkStatusesAfterDelete);
    }

    [Fact]
    public void DeleteById_ShouldDeleteWithSameId_WhenRecordExists()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetById(workStatusId);

        Assert.NotNull(productWorkStatuses);

        bool isDeleteByProductIdSuccessful = _productWorkStatusesService.DeleteById(workStatusId);

        Assert.True(isDeleteByProductIdSuccessful);

        ProductWorkStatuses? productWorkStatusesAfterDelete = _productWorkStatusesService.GetById(workStatusId);

        Assert.Null(productWorkStatusesAfterDelete);
    }

    [Fact]
    public void DeleteById_ShouldFailToDeleteWithSameId_WhenRecordDoesntExist()
    {
        int productId = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ProductWorkStatusesCreateRequest workStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.New,
            ProductXmlStatus = ProductXmlStatusEnum.NotReady,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusResult
            = _productWorkStatusesService.InsertIfItDoesntExist(workStatusesCreateRequest);

        int workStatusId = insertProductWorkStatusResult.Match(
            id => id,
            validationResult => -1,
            unexpectedFailureResult => -1);

        Assert.True(workStatusId > 0);

        ProductWorkStatuses? productWorkStatuses = _productWorkStatusesService.GetById(workStatusId);

        Assert.NotNull(productWorkStatuses);

        bool isDeleteByProductIdSuccessful = _productWorkStatusesService.DeleteById(-1);

        Assert.False(isDeleteByProductIdSuccessful);

        ProductWorkStatuses? productWorkStatusesAfterDelete = _productWorkStatusesService.GetById(workStatusId);

        Assert.NotNull(productWorkStatusesAfterDelete);
    }

#pragma warning restore CA2211 // Non-constant fields should not be visible
}