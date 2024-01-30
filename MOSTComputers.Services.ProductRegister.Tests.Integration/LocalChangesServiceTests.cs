using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
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
        : base(Startup.ConnectionString)
    {
        _localChangesService = localChangesService;
        _productService = productService;
        _productImageService = productImageService;
    }

    private const string _productsTableName = "MOSTPrices";
    private const string _firstImagesTableName = "Images";
    private const string _allImagesTableName = "ImagesAll";

    private readonly ILocalChangesService _localChangesService;
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly ProductCreateRequest _invalidProductCreateRequest = new() { CategoryID = -100 };

    [Fact]
    public void GetAll_ShouldGetAll_WhenAllInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId2 = productInsertResult2.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAll();

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);
    }

    [Fact]
    public void GetAll_ShouldGetOnlySuccessfullyInserted_WhenSomeInsertsAreInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId3 = productInsertResult3.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.IsT2);


        IEnumerable<LocalChangeData> data = _localChangesService.GetAll();

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3);
    }

    [Fact]
    public void GetAllForTable_ShouldGetAll_WhenAllInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId2 = productInsertResult2.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);
    }

    [Fact]
    public void GetAllForTable_ShouldGetOnlySuccessfullyInserted_WhenSomeInsertsAreInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId3 = productInsertResult3.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.IsT2);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x => 
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3);
    }

    [Fact]
    public void GetAllForTable_ShouldGetOnlyDataForItsGivenTable()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId3 = productInsertResult3.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.IsT2);

        Assert.NotNull(productId1);
        Assert.NotNull(productId3);

        ServiceProductImageCreateRequest imageCreateRequest = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productImageInsertResult = _productImageService.InsertInAllImages(imageCreateRequest);

        Assert.True(productImageInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetAll_WhenAllInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId2 = productInsertResult2.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.NotNull(productId2);

        ServiceProductImageCreateRequest imageCreateRequest = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productImageInsertResult = _productImageService.InsertInAllImages(imageCreateRequest);

        uint? imageId = productImageInsertResult.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(imageId);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Equal(3, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        Assert.Contains(data, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetOnlySuccessfullyInserted_WhenSomeInsertsAreInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId3 = productInsertResult3.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.IsT2);

        Assert.NotNull(productId1);
        Assert.NotNull(productId3);

        ServiceProductImageCreateRequest imageCreateRequest = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productImageInsertResult = _productImageService.InsertInAllImages(imageCreateRequest);

        uint? imageId = productImageInsertResult.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(imageId);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Equal(3, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1
        );

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3
        );

        Assert.Contains(data, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId
        );
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetOnlyDataForItsGivenOperationType()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(_invalidProductCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult3 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId3 = productInsertResult3.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.True(productInsertResult2.IsT2);

        Assert.NotNull(productId1);
        Assert.NotNull(productId3);

        ServiceProductImageCreateRequest imageCreateRequest = GetCreateRequestWithImageData((int)productId1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productImageInsertResult = _productImageService.InsertInAllImages(imageCreateRequest);

        uint? imageId = productImageInsertResult.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(imageId);

        ProductUpdateRequest productUpdateRequest = GetValidProductUpdateRequestWithNoImages((int)productId1);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> productUpdateResult = _productService.Update(productUpdateRequest);

        Assert.True(productUpdateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Equal(3, data.Count());

        Assert.Contains(data, x => 
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1
        );

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId3
        );

        Assert.Contains(data, x =>
            x.TableName == _allImagesTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == imageId
        );
    }

    [Fact]
    public void GetById_ShouldGetItem_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        LocalChangeData? dataFromGetById = _localChangesService.GetById((uint)dataFromGetAll.Id);

        Assert.NotNull(dataFromGetById);

        Assert.True(dataFromGetAll.Id == dataFromGetById.Id
            && dataFromGetAll.TableName == _productsTableName
            && dataFromGetById.TableName == _productsTableName
            && dataFromGetAll.TableElementId == dataFromGetById.TableElementId 
            && dataFromGetAll.TableElementId == productId1
            && dataFromGetAll.OperationType == dataFromGetById.OperationType
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TimeStamp == dataFromGetById.TimeStamp);
    }

    [Fact]
    public void GetById_ShouldFailToGetItem_WhenIdIsInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        LocalChangeData? dataFromGetById = _localChangesService.GetById(0);

        Assert.Null(dataFromGetById);
    }

    [Fact]
    public void GetByTableNameAndElementId_ShouldGetItem_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        LocalChangeData? dataFromGetById = _localChangesService.GetByTableNameAndElementId(_productsTableName, (int)productId1);

        Assert.NotNull(dataFromGetById);

        Assert.True(dataFromGetById.Id > 0
            && dataFromGetById.TableName == _productsTableName
            && dataFromGetById.TableElementId == productId1
            && dataFromGetById.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetById.TimeStamp > new DateTime(0, 0, 0));
    }

    [Fact]
    public void GetByTableNameAndElementId_ShouldFailToGetItem_WhenTableNameIsInvalid()
    {
        const string incorrectTableName = "asdalffsd;lff";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        LocalChangeData? dataFromGetById = _localChangesService.GetByTableNameAndElementId(incorrectTableName, (int)productId1);

        Assert.Null(dataFromGetById);
    }

    [Fact]
    public void GetByTableNameAndElementId_ShoulFailToGetItem_WhenElementIdIsInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        LocalChangeData? dataFromGetById = _localChangesService.GetByTableNameAndElementId(_productsTableName, 0);

        Assert.Null(dataFromGetById);
    }

    [Fact]
    public void DeleteById_ShouldDeleteItem_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteById((uint)dataFromGetAll.Id);

        Assert.True(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetById((uint)dataFromGetAll.Id);

        Assert.Null(dataAfterDelete);
    }

    [Fact]
    public void DeleteById_ShouldFailToDeleteItem_WhenIdIsInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Single(data);

        LocalChangeData dataFromGetAll = data.First();

        Assert.True(dataFromGetAll.TableName == _productsTableName
            && dataFromGetAll.OperationType == ChangeOperationTypeEnum.Create
            && dataFromGetAll.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteById(0);

        Assert.False(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetById((uint)dataFromGetAll.Id);

        Assert.NotNull(dataAfterDelete);
    }

    [Fact]
    public void DeleteRangeByIds_ShouldDeleteAllItems_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId2 = productInsertResult2.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.NotNull(productId2);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x => 
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        IEnumerable<uint> changeDataIds = data.Select(x => (uint)x.Id);

        bool deleteSuccess = _localChangesService.DeleteRangeByIds(changeDataIds);

        Assert.True(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.Empty(dataAfterDelete);
    }

    [Fact]
    public void DeleteByTableNameAndElementId_ShouldDeleteItem_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        int elementId = (int)productId1;

        LocalChangeData? data = _localChangesService.GetByTableNameAndElementId(_productsTableName, elementId);

        Assert.NotNull(data);

        Assert.True(data.TableName == _productsTableName
            && data.OperationType == ChangeOperationTypeEnum.Create
            && data.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteByTableNameAndElementId(_productsTableName, elementId);

        Assert.True(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetByTableNameAndElementId(_productsTableName, elementId);

        Assert.Null(dataAfterDelete);
    }

    [Fact]
    public void DeleteByTableNameAndElementId_ShouldFailToDeleteItem_WhenTableNameIsInvalid()
    {
        const string invalidTableName = "afproeworii;><sd";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        int elementId = (int)productId1;

        LocalChangeData? data = _localChangesService.GetByTableNameAndElementId(_productsTableName, elementId);

        Assert.NotNull(data);

        Assert.True(data.TableName == _productsTableName
            && data.OperationType == ChangeOperationTypeEnum.Create
            && data.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteByTableNameAndElementId(invalidTableName, elementId);

        Assert.False(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetByTableNameAndElementId(_productsTableName, elementId);

        Assert.NotNull(dataAfterDelete);
    }

    [Fact]
    public void DeleteByTableNameAndElementId_ShouldFailToDeleteItem_WhenIdIsInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);

        int elementId = (int)productId1;

        LocalChangeData? data = _localChangesService.GetByTableNameAndElementId(_productsTableName, elementId);

        Assert.NotNull(data);

        Assert.True(data.TableName == _productsTableName
            && data.OperationType == ChangeOperationTypeEnum.Create
            && data.TableElementId == productId1);

        bool deleteSuccess = _localChangesService.DeleteByTableNameAndElementId(_productsTableName, 0);

        Assert.False(deleteSuccess);

        LocalChangeData? dataAfterDelete = _localChangesService.GetByTableNameAndElementId(_productsTableName, elementId);

        Assert.NotNull(dataAfterDelete);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldDeleteAllItems_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId2 = productInsertResult2.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.NotNull(productId2);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x => x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x => x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { (int)productId1.Value, (int)productId2.Value };

        bool deleteSuccess = _localChangesService.DeleteRangeByTableNameAndElementIds(_productsTableName, elementIds);

        Assert.True(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Empty(dataAfterDelete);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldFailToDeleteItems_WhenTableNameIsInvalid()
    {
        const string incorrectTableName = "adklfalkjkajkajkdsf";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId2 = productInsertResult2.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.NotNull(productId2);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x => x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x => x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { (int)productId1.Value, (int)productId2.Value };

        bool deleteSuccess = _localChangesService.DeleteRangeByTableNameAndElementIds(incorrectTableName, elementIds);

        Assert.True(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Empty(dataAfterDelete);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldOnlyDeleteItemsWithValidIds_WhenSomeIdsAreInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(ValidProductCreateRequestWithNoImages);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(ValidProductCreateRequestWithNoImages);

        uint? productId1 = productInsertResult1.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        uint? productId2 = productInsertResult2.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(productId1);
        Assert.NotNull(productId2);

        IEnumerable<LocalChangeData> data = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Equal(2, data.Count());

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId1);

        Assert.Contains(data, x =>
            x.TableName == _productsTableName
            && x.OperationType == ChangeOperationTypeEnum.Create
            && x.TableElementId == productId2);

        List<int> elementIds = new() { (int)productId1.Value, 0 };

        bool deleteSuccess = _localChangesService.DeleteRangeByTableNameAndElementIds(_productsTableName, elementIds);

        Assert.True(deleteSuccess);

        IEnumerable<LocalChangeData> dataAfterDelete = _localChangesService.GetAllForTable(_productsTableName);

        Assert.Single(dataAfterDelete);

        LocalChangeData singleDataRemaining = dataAfterDelete.First();

        LocalChangeData secondProductChangeData = data.First(x => x.TableElementId == (int)productId2);

        Assert.True(singleDataRemaining.Id == secondProductChangeData.Id
            && singleDataRemaining.TableName == _productsTableName
            && singleDataRemaining.OperationType == ChangeOperationTypeEnum.Create
            && singleDataRemaining.TableElementId == productId2);
    }
}