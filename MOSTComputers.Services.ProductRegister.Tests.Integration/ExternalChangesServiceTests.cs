using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.External;
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
public sealed class ExternalChangesServiceTests : IntegrationTestBaseForNonWebProjectsWithDBReset
{
    public ExternalChangesServiceTests(
        IExternalChangesService externalChangesService,
        IProductService productService,
        IProductImageService productImageService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _externalChangesService = externalChangesService;
        _productService = productService;
        _productImageService = productImageService;
    }

    private const string _firstImagesTableName = "Images";
    private const string _allImagesTableName = "ImagesAll";

    private readonly IExternalChangesService _externalChangesService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public void GetAll_ShouldGetAll_WhenAllOperationsAreValid()
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

        IEnumerable<ExternalChangeData> externalChanges = _externalChangesService.GetAll();

        Assert.True(externalChanges.Count() >= 2);

        Assert.Contains(externalChanges, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);

        Assert.Contains(externalChanges, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);
    }

    [Fact]
    public void GetAll_ShouldGetOnlySuccessfullyInserted_WhenOnlySomeOperationsAreValid()
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
            ImageData = null,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult
            = _productImageService.InsertInFirstImages(firstImageCreateRequest);

        bool isFirstImageInsertSuccessful = insertFirstImageResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.False(isFirstImageInsertSuccessful);

        IEnumerable<ExternalChangeData> externalChanges = _externalChangesService.GetAll();

        Assert.NotEmpty(externalChanges);

        Assert.Contains(externalChanges, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);

        Assert.DoesNotContain(externalChanges, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetAllWithSameOperation_WhenAllOperationsAreValid()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForOperation = _externalChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.True(externalChangesForOperation.Count() >= 2);

        Assert.All(externalChangesForOperation, x => Assert.Equal(ChangeOperationTypeEnum.Create, x.OperationType));

        Assert.Contains(externalChangesForOperation, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);

        Assert.Contains(externalChangesForOperation, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForOperation, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);
    }

    [Fact]
    public void GetAllForOperationType_ShouldOnlyGetSuccessfullyInsertedWithSameOperation_WhenOnlySomeOperationsAreValid()
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

        ServiceProductImageUpdateRequest imageUpdateRequest = new()
        {
            Id = imageId,
            ImageData = null,
            ImageContentType = imageCreateRequest.ImageContentType,
            HtmlData = imageCreateRequest.HtmlData,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> imageUpdateResult = _productImageService.UpdateInAllImages(imageUpdateRequest);

        bool isImageUpdateSuccessful = imageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.False(isImageUpdateSuccessful);

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForOperation = _externalChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Update);

        Assert.NotEmpty(externalChangesForOperation);

        Assert.All(externalChangesForOperation, x => Assert.Equal(ChangeOperationTypeEnum.Update, x.OperationType));

        Assert.Contains(externalChangesForOperation, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForOperation, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == imageId);
    }

    [Fact]
    public void GetAllForTable_ShouldGetAllWithSameTable_WhenAllOperationsAreValid()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable = _externalChangesService.GetAllForTable(_firstImagesTableName);

        Assert.True(externalChangesForFirstImageTable.Count() >= 2);

        Assert.All(externalChangesForFirstImageTable, x => Assert.Equal(_firstImagesTableName, x.TableName));

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForFirstImageTable, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);
    }

    [Fact]
    public void GetAllForTable_ShouldOnlyGetSuccessfullyInsertedWithSameTable_WhenOnlySomeOperationsAreValid()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = null,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateFailedWithValidationResult = firstImageUpdateResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateFailedWithValidationResult);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable = _externalChangesService.GetAllForTable(_firstImagesTableName);

        Assert.NotEmpty(externalChangesForFirstImageTable);

        Assert.All(externalChangesForFirstImageTable, x => Assert.Equal(_firstImagesTableName, x.TableName));

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForFirstImageTable, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);
    }

    [Fact]
    public void GetAllByTableNameAndElementId_ShouldGetAllWithSameTableNameAndElementId_WhenAllOperationsAreValid()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.True(externalChangesForFirstImageTable.Count() >= 2);

        Assert.All(externalChangesForFirstImageTable, x => Assert.Equal(_firstImagesTableName, x.TableName));

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForFirstImageTable, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);
    }

    [Fact]
    public void GetAllByTableNameAndElementId_ShouldGetOnlySuccessfullyInsertedWithSameTableNameAndElementId_WhenOnlySomeOperationsAreValid()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = null,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateFailedWithValidationResult = firstImageUpdateResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateFailedWithValidationResult);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.NotEmpty(externalChangesForFirstImageTable);

        Assert.All(externalChangesForFirstImageTable, x => Assert.Equal(_firstImagesTableName, x.TableName));

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForFirstImageTable, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);
    }

    [Fact]
    public void GetById_ShouldGetWithSameId_WhenRecordExists()
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

        IEnumerable<ExternalChangeData> allChangeDataForTable = _externalChangesService.GetAllForTable(_allImagesTableName);

        Assert.Single(allChangeDataForTable, x => x.TableElementId == imageId);

        ExternalChangeData externalChangeData = allChangeDataForTable.First(x =>
            x.TableElementId == imageId
            && x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create);

        ExternalChangeData? sameExternalChangeDataButUsingGetById = _externalChangesService.GetById(externalChangeData.Id);

        Assert.NotNull(sameExternalChangeDataButUsingGetById);

        Assert.Equal(externalChangeData.Id, sameExternalChangeDataButUsingGetById.Id);
        Assert.Equal(imageId, sameExternalChangeDataButUsingGetById.TableElementId);
        Assert.Equal(_allImagesTableName, sameExternalChangeDataButUsingGetById.TableName);
        Assert.Equal(ChangeOperationTypeEnum.Create, sameExternalChangeDataButUsingGetById.OperationType);
    }

    [Fact]
    public void GetById_ShouldFailToGetWithSameId_WhenRecordDoesntExists()
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

        IEnumerable<ExternalChangeData> allChangeDataForTable = _externalChangesService.GetAllForTable(_allImagesTableName);

        Assert.Single(allChangeDataForTable, x => x.TableElementId == imageId);

        ExternalChangeData externalChangeData = allChangeDataForTable.First(x =>
            x.TableElementId == imageId
            && x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create);

        ExternalChangeData? sameExternalChangeDataButUsingGetById = _externalChangesService.GetById(-1);

        Assert.Null(sameExternalChangeDataButUsingGetById);
    }

    [Fact]
    public void DeleteById_ShouldDeleteWithSameId_WhenRecordExists()
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

        IEnumerable<ExternalChangeData> allChangeDataForTable = _externalChangesService.GetAllForTable(_allImagesTableName);

        Assert.Single(allChangeDataForTable, x => x.TableElementId == imageId);

        ExternalChangeData externalChangeData = allChangeDataForTable.First(x =>
            x.TableElementId == imageId
            && x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create);

        bool isDeleteSuccessful = _externalChangesService.DeleteById(externalChangeData.Id);

        Assert.True(isDeleteSuccessful);

        ExternalChangeData? externalChangeDataAfterDelete = _externalChangesService.GetById(externalChangeData.Id);

        Assert.Null(externalChangeDataAfterDelete);
    }

    [Fact]
    public void DeleteById_ShouldFailDeleteWithSameId_WhenRecordDoesntExist()
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

        IEnumerable<ExternalChangeData> allChangeDataForTable = _externalChangesService.GetAllForTable(_allImagesTableName);

        Assert.Single(allChangeDataForTable, x => x.TableElementId == imageId);

        ExternalChangeData externalChangeData = allChangeDataForTable.First(x =>
            x.TableElementId == imageId
            && x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create);

        bool isDeleteSuccessful = _externalChangesService.DeleteById(-1);

        Assert.False(isDeleteSuccessful);

        ExternalChangeData? externalChangeDataAfterDelete = _externalChangesService.GetById(externalChangeData.Id);

        Assert.NotNull(externalChangeDataAfterDelete);
    }

    [Fact]
    public void DeleteByTableNameAndElementIdAndOperationType_ShouldDeleteWithSameTableNameAndElementIdAndOperationType_WhenRecordExists()
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

        IEnumerable<ExternalChangeData> allChangeDataForTable = _externalChangesService.GetAllForTable(_allImagesTableName);

        Assert.Single(allChangeDataForTable, x => x.TableElementId == imageId);

        ExternalChangeData externalChangeData = allChangeDataForTable.First(x =>
            x.TableElementId == imageId
            && x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create);

        bool isDeleteSuccessful = _externalChangesService.DeleteByTableNameAndElementIdAndOperationType(
            _allImagesTableName, imageId, ChangeOperationTypeEnum.Create);

        Assert.True(isDeleteSuccessful);

        ExternalChangeData? externalChangeDataAfterDelete = _externalChangesService.GetById(externalChangeData.Id);

        Assert.Null(externalChangeDataAfterDelete);
    }

    [Theory]
    [MemberData(nameof(DeleteByTableNameAndElementIdAndOperationType_ShouldOnlyDeleteWithSameTableNameAndElementIdAndOperationType_WhenRecordExists_Data))]
    public void DeleteByTableNameAndElementIdAndOperationType_ShouldOnlyDeleteWithSameTableNameAndElementIdAndOperationType_WhenRecordExists(
        string tableName, int elementId, ChangeOperationTypeEnum operationType, bool expected)
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

        IEnumerable<ExternalChangeData> allChangeDataForTable = _externalChangesService.GetAllForTable(_allImagesTableName);

        Assert.Single(allChangeDataForTable, x => x.TableElementId == imageId);

        ExternalChangeData externalChangeData = allChangeDataForTable.First(x =>
            x.TableElementId == imageId
            && x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create);

        if (elementId == UseRequiredValuePlaceholder)
        {
            elementId = imageId;
        }

        bool isDeleteSuccessful = _externalChangesService.DeleteByTableNameAndElementIdAndOperationType(tableName, elementId, operationType);

        Assert.Equal(expected, isDeleteSuccessful);

        ExternalChangeData? externalChangeDataAfterDelete = _externalChangesService.GetById(externalChangeData.Id);

        Assert.Equal(expected, externalChangeDataAfterDelete is null);
    }

    public static TheoryData<string, int, ChangeOperationTypeEnum, bool> DeleteByTableNameAndElementIdAndOperationType_ShouldOnlyDeleteWithSameTableNameAndElementIdAndOperationType_WhenRecordExists_Data => new()
    {
        { _allImagesTableName, UseRequiredValuePlaceholder, ChangeOperationTypeEnum.Create, true },
        { _firstImagesTableName, UseRequiredValuePlaceholder, ChangeOperationTypeEnum.Create, false },
        { string.Empty, UseRequiredValuePlaceholder, ChangeOperationTypeEnum.Create, false },
        { _allImagesTableName, -1, ChangeOperationTypeEnum.Create, false },
        { _allImagesTableName, UseRequiredValuePlaceholder, ChangeOperationTypeEnum.Update, false },
    };

    [Fact]
    public void DeleteAllByTableNameAndElementId_ShouldDeleteAllWithSameTableNameAndElementId_WhenAllOperationsAreValid()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.True(externalChangesForFirstImageTable.Count() >= 2);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        bool areAllRecordsWithSameTableNameAndElementIdDeleted
            = _externalChangesService.DeleteAllByTableNameAndElementId(_firstImagesTableName, productId);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTableAfterDelete
            = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterDelete, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterDelete, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        IEnumerable<ExternalChangeData> externalChangeDatasOfInsertInAllImagesOperation
            = _externalChangesService.GetAllByTableNameAndElementId(_allImagesTableName, imageId);

        Assert.Single(externalChangeDatasOfInsertInAllImagesOperation);
    }

    [Fact]
    public void DeleteAllByTableNameAndElementId_ShoulOnlydDeleteSuccessfullyInsertedWithSameTableNameAndElementId_WhenOnlySomeOperationsAreValid()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = null,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateFailedWithValidationResult = firstImageUpdateResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateFailedWithValidationResult);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable
            = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.NotEmpty(externalChangesForFirstImageTable);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        bool areAllRecordsWithSameTableNameAndElementIdDeleted
            = _externalChangesService.DeleteAllByTableNameAndElementId(_firstImagesTableName, productId);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTableAfterDelete
            = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterDelete, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        IEnumerable<ExternalChangeData> externalChangeDatasOfInsertInAllImagesOperation
            = _externalChangesService.GetAllByTableNameAndElementId(_allImagesTableName, imageId);

        Assert.Single(externalChangeDatasOfInsertInAllImagesOperation);
    }

    [Fact]
    public void DeleteRangeByIds_ShouldDeleteAllWithSameIds_WhenRecordsExist()
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

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable
            = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.Equal(2, externalChangesForFirstImageTable.Count());

        ExternalChangeData? externalChangeDataForFirstImageInsert = externalChangesForFirstImageTable.First(x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId);

        ExternalChangeData? externalChangeDataForFirstImageUpdate = externalChangesForFirstImageTable.First(x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId);

        Assert.NotNull(externalChangeDataForFirstImageInsert);
        Assert.NotNull(externalChangeDataForFirstImageUpdate);

        List<int> ids = new() { externalChangeDataForFirstImageInsert.Id, externalChangeDataForFirstImageUpdate.Id };

        bool isDeleteRangeByIdsSuccessful = _externalChangesService.DeleteRangeByIds(ids);

        Assert.True(isDeleteRangeByIdsSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTableAfterDelete
            = _externalChangesService.GetAllByTableNameAndElementId(_firstImagesTableName, productId);

        Assert.Empty(externalChangesForFirstImageTableAfterDelete);

        IEnumerable<ExternalChangeData> externalChangeDatasOfInsertInAllImagesOperation
          = _externalChangesService.GetAllByTableNameAndElementId(_allImagesTableName, imageId);

        Assert.Single(externalChangeDatasOfInsertInAllImagesOperation);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldDeleteAllWithSameTableNameAndElementIds_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceProductImageCreateRequest imageCreateRequest = GetValidImageCreateRequestWithImageData(productId1);

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

        ServiceProductFirstImageCreateRequest firstImageCreateRequest1 = new()
        {
            ProductId = productId1,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult1
            = _productImageService.InsertInFirstImages(firstImageCreateRequest1);

        bool isFirstImageInsert1Successful = insertFirstImageResult1.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageInsert1Successful);

        ServiceProductFirstImageCreateRequest firstImageCreateRequest2 = new()
        {
            ProductId = productId2,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult2
            = _productImageService.InsertInFirstImages(firstImageCreateRequest2);

        bool isFirstImageInsert2Successful = insertFirstImageResult2.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageInsert2Successful);

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId1,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable = _externalChangesService.GetAllForTable(_firstImagesTableName);

        Assert.True(externalChangesForFirstImageTable.Count() >= 3);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId1);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { productId1, productId2 };

        bool isDeleteByTableNameAndElementIdsSuccssful = _externalChangesService.DeleteRangeByTableNameAndElementIds(_firstImagesTableName, elementIds);

        Assert.True(isDeleteByTableNameAndElementIdsSuccssful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTableAfterUpdate = _externalChangesService.GetAllForTable(_firstImagesTableName);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterUpdate, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterUpdate, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId1);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterUpdate, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldDeleteOnlyThoseWithSameTableNameAndElementIds_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceProductImageCreateRequest imageCreateRequest = GetValidImageCreateRequestWithImageData(productId1);

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

        ServiceProductFirstImageCreateRequest firstImageCreateRequest1 = new()
        {
            ProductId = productId1,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult1
            = _productImageService.InsertInFirstImages(firstImageCreateRequest1);

        bool isFirstImageInsert1Successful = insertFirstImageResult1.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageInsert1Successful);

        ServiceProductFirstImageCreateRequest firstImageCreateRequest2 = new()
        {
            ProductId = productId2,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = imageCreateRequest.HtmlData,
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> insertFirstImageResult2
            = _productImageService.InsertInFirstImages(firstImageCreateRequest2);

        bool isFirstImageInsert2Successful = insertFirstImageResult2.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageInsert2Successful);

        ServiceProductFirstImageUpdateRequest firstImageUpdateRequest = new()
        {
            ProductId = productId1,
            ImageData = imageCreateRequest.ImageData,
            HtmlData = "<data></data>",
            ImageContentType = imageCreateRequest.ImageContentType,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> firstImageUpdateResult
            = _productImageService.UpdateInFirstImages(firstImageUpdateRequest);

        bool isFirstImageUpdateSuccessful = firstImageUpdateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isFirstImageUpdateSuccessful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTable = _externalChangesService.GetAllForTable(_firstImagesTableName);

        Assert.True(externalChangesForFirstImageTable.Count() >= 3);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId1);

        Assert.Contains(externalChangesForFirstImageTable, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { productId1 };

        bool isDeleteByTableNameAndElementIdsSuccssful = _externalChangesService.DeleteRangeByTableNameAndElementIds(_firstImagesTableName, elementIds);

        Assert.True(isDeleteByTableNameAndElementIdsSuccssful);

        IEnumerable<ExternalChangeData> externalChangesForFirstImageTableAfterUpdate = _externalChangesService.GetAllForTable(_firstImagesTableName);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterUpdate, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.DoesNotContain(externalChangesForFirstImageTableAfterUpdate, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Update
            && x.TableElementId == productId1);

        Assert.Contains(externalChangesForFirstImageTableAfterUpdate, x =>
            x.TableName == _firstImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);
    }
}