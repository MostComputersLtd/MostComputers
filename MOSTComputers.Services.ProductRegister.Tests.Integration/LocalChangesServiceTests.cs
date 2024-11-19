using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class LocalChangesServiceTests : IntegrationTestBaseForNonWebProjects
{
    public LocalChangesServiceTests(
        ILocalChangesService localChangesService,
        IProductService productService,
        IProductImageService productImageService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _localChangesService = localChangesService;
        _productService = productService;
        _productImageService = productImageService;
    }

    private const string _productsTableChangeName = "MOSTPRices";

    private readonly ILocalChangesService _localChangesService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly ProductCreateRequest _invalidProductCreateRequest = new() { CategoryId = -100 };

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    [Fact]
    public void GetAll_ShouldGetAll_WhenAllInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId2 = productInsertResult2.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAll();

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);
    }

    [Fact]
    public void GetAll_ShouldGetOnlySuccessfullyInserted_WhenSomeInsertsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId3 = productInsertResult3.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        IEnumerable<LocalChangeData> data = _localChangesService.GetAll();

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3);
    }

    [Fact]
    public void GetAllForTable_ShouldGetAll_WhenAllInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId2 = productInsertResult2.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);
    }

    [Fact]
    public void GetAllForTable_ShouldGetOnlySuccessfullyInserted_WhenSomeInsertsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId3 = productInsertResult3.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x => 
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3);
    }

    [Fact]
    public void GetAllForTable_ShouldGetOnlyDataForItsGivenTable()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId3 = productInsertResult3.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.NotNull(productId1);
        Assert.NotNull(productId3);

        ServiceProductImageCreateRequest imageCreateRequest = GetValidImageCreateRequestWithImageData(productId1.Value);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productImageInsertResult = _productImageService.InsertInAllImages(imageCreateRequest);

        Assert.True(productImageInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetAll_WhenAllInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId2 = productInsertResult2.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetOnlySuccessfullyInserted_WhenSomeInsertsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId3 = productInsertResult3.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId3);
        Assert.True(productId3 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1
        );

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3
        );
    }

    [Fact]
    public async Task GetAllForOperationType_ShouldGetOnlyDataForItsGivenOperationTypeAsync()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId3 = productInsertResult3.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId3);
        Assert.True(productId3 > 0);

        ProductFullUpdateRequest productUpdateRequest = GetValidProductFullUpdateRequestWithNoImages(productId1.Value);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult
            = await _productService.UpdateProductFullAsync(productUpdateRequest);

        Assert.True(productUpdateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(3, data.Count());

        Assert.Contains(data, x => 
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1
        );

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3
        );
    }

    [Fact]
    public void GetById_ShouldGetItem_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableChangeName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        LocalChangeData? dataFromGetById = _localChangesService.GetById(dataFromGetAll.Id);

        Assert.NotNull(dataFromGetById);

        Assert.True(dataFromGetAll.Id == dataFromGetById.Id
            && dataFromGetAll.TableName == _productsTableChangeName
            && dataFromGetById.TableName == _productsTableChangeName
            && dataFromGetAll.TableElementId == dataFromGetById.TableElementId 
            && dataFromGetAll.TableElementId == productId1
            && dataFromGetAll.OperationType == dataFromGetById.OperationType
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TimeStamp == dataFromGetById.TimeStamp);
    }

    [Fact]
    public void GetById_ShouldFailToGetItem_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableChangeName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        LocalChangeData? dataFromGetById = _localChangesService.GetById(0);

        Assert.Null(dataFromGetById);
    }

    [Fact]
    public void GetByTableNameAndElementIdAndOperationType_ShouldGetItem_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        LocalChangeData? dataFromGetById = _localChangesService.GetByTableNameAndElementIdAndOperationType(
            _productsTableChangeName, productId1.Value, ChangeOperationTypeEnum.Create);

        Assert.NotNull(dataFromGetById);

        Assert.True(dataFromGetById.Id > 0
            && dataFromGetById.TableName == _productsTableChangeName
            && dataFromGetById.TableElementId == productId1
            && dataFromGetById.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetById.TimeStamp > new DateTime(0));
    }

    [Fact]
    public void GetByTableNameAndElementIdAndOperationType_ShouldFailToGetItem_WhenTableNameIsInvalid()
    {
        const string incorrectTableName = "asdalffsd;lff";

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        LocalChangeData? dataFromGetById = _localChangesService.GetByTableNameAndElementIdAndOperationType(
            incorrectTableName, productId1.Value, ChangeOperationTypeEnum.Create);

        Assert.Null(dataFromGetById);
    }

    [Fact]
    public void GetByTableNameAndElementIdAndOperationType_ShoulFailToGetItem_WhenElementIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        LocalChangeData? dataFromGetById = _localChangesService.GetByTableNameAndElementIdAndOperationType(
            _productsTableChangeName, 0, ChangeOperationTypeEnum.Create);

        Assert.Null(dataFromGetById);
    }

    [Fact]
    public void DeleteById_ShouldDeleteItem_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableChangeName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteById(dataFromGetAll.Id);

        Assert.True(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetById(dataFromGetAll.Id);

        Assert.Null(dataAfterDelete);
    }

    [Fact]
    public void DeleteById_ShouldFailToDeleteItem_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableChangeName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteById(0);

        Assert.False(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetById(dataFromGetAll.Id);

        Assert.NotNull(dataAfterDelete);
    }

    [Fact]
    public void DeleteRangeByIds_ShouldDeleteAllItems_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId2 = productInsertResult2.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x => 
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        IEnumerable<int> changeDataIds = data.Select(x => x.Id);

        bool deleteSuccess = _localChangesService.DeleteRangeByIds(changeDataIds);

        Assert.True(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Empty(dataAfterDelete);
    }

    [Fact]
    public void DeleteByTableNameAndElementId_ShouldDeleteItem_WhenInsertIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        int elementId = productId1.Value;

        LocalChangeData? data = _localChangesService.GetByTableNameAndElementIdAndOperationType(_productsTableChangeName, elementId, ChangeOperationTypeEnum.Create);

        Assert.NotNull(data);

        Assert.True(data.TableName == _productsTableChangeName
            && data.OperationType == ChangeOperationTypeEnum.Create
            && data.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteByTableNameAndElementId(_productsTableChangeName, elementId);

        Assert.True(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetByTableNameAndElementIdAndOperationType(_productsTableChangeName, elementId, ChangeOperationTypeEnum.Create);

        Assert.Null(dataAfterDelete);
    }

    [Fact]
    public void DeleteByTableNameAndElementId_ShouldFailToDeleteItem_WhenTableNameIsInvalid()
    {
        const string invalidTableName = "afproeworii;><sd";

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        int elementId = productId1.Value;

        LocalChangeData? data = _localChangesService.GetByTableNameAndElementIdAndOperationType(
            _productsTableChangeName, elementId, ChangeOperationTypeEnum.Create);

        Assert.NotNull(data);

        Assert.True(data.TableName == _productsTableChangeName
            && data.OperationType == ChangeOperationTypeEnum.Create
            && data.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteByTableNameAndElementId(invalidTableName, elementId);

        Assert.False(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetByTableNameAndElementIdAndOperationType(
            _productsTableChangeName, elementId, ChangeOperationTypeEnum.Create);

        Assert.NotNull(dataAfterDelete);
    }

    [Fact]
    public void DeleteByTableNameAndElementId_ShouldFailToDeleteItem_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        int elementId = productId1.Value;

        LocalChangeData? data = _localChangesService.GetByTableNameAndElementIdAndOperationType(
            _productsTableChangeName, elementId, ChangeOperationTypeEnum.Create);

        Assert.NotNull(data);

        Assert.True(data.TableName == _productsTableChangeName
            && data.OperationType == ChangeOperationTypeEnum.Create
            && data.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteByTableNameAndElementId(_productsTableChangeName, 0);

        Assert.False(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetByTableNameAndElementIdAndOperationType(
            _productsTableChangeName, elementId, ChangeOperationTypeEnum.Create);

        Assert.NotNull(dataAfterDelete);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldDeleteAllItems_WhenInsertsAreValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId2 = productInsertResult2.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x => x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x => x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { productId1.Value, productId2.Value };

        bool deleteSuccess = _localChangesService.DeleteRangeByTableNameAndElementIds(_productsTableChangeName, elementIds);

        Assert.True(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Empty(dataAfterDelete);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldFailToDeleteItems_WhenTableNameIsInvalid()
    {
        const string incorrectTableName = "adklfalkjkajkajkdsf";

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId2 = productInsertResult2.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x => x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x => x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { productId1.Value, productId2.Value };

        bool deleteSuccess = _localChangesService.DeleteRangeByTableNameAndElementIds(incorrectTableName, elementIds);

        Assert.False(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(2, dataAfterDelete.Count());
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldOnlyDeleteItemsWithValidIds_WhenSomeIdsAreInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        int? productId1 = productInsertResult1.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        int? productId2 = productInsertResult2.Match<int?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableChangeName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { productId1.Value, 0 };

        bool deleteSuccess = _localChangesService.DeleteRangeByTableNameAndElementIds(_productsTableChangeName, elementIds);

        Assert.True(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForTable(_productsTableChangeName);

        Assert.Single(dataAfterDelete);

        LocalChangeData singleDataRemaining = dataAfterDelete.First();

        LocalChangeData secondProductChangeData = data.First(x => x.TableElementId == (int)productId2);

        Assert.True(singleDataRemaining.Id == secondProductChangeData.Id
            && singleDataRemaining.TableName == _productsTableChangeName
            && singleDataRemaining.OperationType == ChangeOperationTypeEnum.Create
            && singleDataRemaining.TableElementId == productId2);
    }
}