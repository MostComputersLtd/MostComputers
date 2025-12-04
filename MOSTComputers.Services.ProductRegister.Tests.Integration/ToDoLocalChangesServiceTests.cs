using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.SuccessfulInsertAbstractions;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;
[Collection(DefaultTestCollection.Name)]
public sealed class ToDoLocalChangesServiceTests : IntegrationTestBaseForNonWebProjectsWithDBReset
{
    public ToDoLocalChangesServiceTests(
        IToDoLocalChangesService toDoLocalChangesService,
        IProductService productService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _toDoLocalChangesService = toDoLocalChangesService;
        _productService = productService;
    }

    private const string _productsTableName = "MOSTPRices";
    private const string _invalidTableName = "InvalidTableName";

    private readonly IToDoLocalChangesService _toDoLocalChangesService;
    private readonly IProductService _productService;
    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
    }

    /* IToDoLocalChangesService methods:
     
    IEnumerable<LocalChangeData> GetAll();
    IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum operationType);
    IEnumerable<LocalChangeData> GetAllForTable(string tableName);
    LocalChangeData? GetById(int id);
    LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationType);
    OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ToDoLocalChangeCreateRequest createRequest, IValidator<ToDoLocalChangeCreateRequest>? validator = null);
    bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds);
    bool DeleteRangeByIds(IEnumerable<int> ids);
    bool DeleteAllByTableNameAndElementId(string tableName, int elementId);
    bool DeleteById(int id);
    */

#pragma warning disable CA2211 // Non-constant fields should not be visible

    [Fact]
    public void GetAll_ShouldGetAll_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = 1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllAsync();

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);
    }

    [Fact]
    public void GetAll_ShouldGetOnlySuccessfullyInserted_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = -1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoLocalChangeInsertResult = _toDoLocalChangesService.InsertAsync(toDoLocalChangeCreateRequest2);

        bool hasToDoLocalChangeInsertFailedWithValidationResult = toDoLocalChangeInsertResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(hasToDoLocalChangeInsertFailedWithValidationResult);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllAsync();

        Assert.NotEmpty(toDoLocalChanges);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);
    }

    [Fact]
    public void GetAllForTable_ShouldGetAllWithSameTableName_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = 1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.All(toDoLocalChanges, x => Assert.Equal(_productsTableName, x.TableName));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);
    }

    [Fact]
    public void GetAllForTable_ShouldGetOnlySuccessfullyInsertedWithSameTableName_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = -1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoLocalChangeInsertResult = _toDoLocalChangesService.InsertAsync(toDoLocalChangeCreateRequest2);

        bool hasToDoLocalChangeInsertFailedWithValidationResult = toDoLocalChangeInsertResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(hasToDoLocalChangeInsertFailedWithValidationResult);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.NotEmpty(toDoLocalChanges);

        Assert.All(toDoLocalChanges, x => Assert.Equal(_productsTableName, x.TableName));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetAllWithSameOperation_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = 1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest3 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Update,
        };

        int? toDoLocalChangeId3 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest3);

        Assert.NotNull(toDoLocalChangeId3);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForOperationTypeAsync(ChangeOperationType.Create);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.All(toDoLocalChanges, x => Assert.Equal(ChangeOperationType.Create, x.OperationType));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetOnlySuccessfullyInsertedWithSameOperation_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = -1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoLocalChangeInsertResult = _toDoLocalChangesService.InsertAsync(toDoLocalChangeCreateRequest2);

        bool hasToDoLocalChangeInsertFailedWithValidationResult = toDoLocalChangeInsertResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(hasToDoLocalChangeInsertFailedWithValidationResult);


        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest3 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Update,
        };

        int? toDoLocalChangeId3 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest3);

        Assert.NotNull(toDoLocalChangeId3);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForOperationTypeAsync(ChangeOperationType.Create);

        Assert.NotEmpty(toDoLocalChanges);

        Assert.All(toDoLocalChanges, x => Assert.Equal(ChangeOperationType.Create, x.OperationType));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType);
    }

    [Theory]
    [MemberData(nameof(GetById_ShouldSucceedToGetWithSameId_WhenRecordExists_Data))]
    public void GetById_ShouldSucceedToGetWithSameId_WhenRecordExists(int id, bool expected)
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        if (id == UseRequiredValuePlaceholder)
        {
            id = toDoLocalChangeId1.Value;
        }

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetByIdAsync(id);

        Assert.Equal(expected, toDoLocalChangeData is not null);

        if (toDoLocalChangeData is not null)
        {
            Assert.Equal(toDoLocalChangeId1, toDoLocalChangeData.Id);
            Assert.Equal(productId1, toDoLocalChangeData.TableElementId);
            Assert.Equal(_productsTableName, toDoLocalChangeData.TableName);
            Assert.Equal(toDoLocalChangeCreateRequest1.OperationType, toDoLocalChangeData.OperationType);
        }
    }

    public static TheoryData<int, bool> GetById_ShouldSucceedToGetWithSameId_WhenRecordExists_Data = new()
    {
        { UseRequiredValuePlaceholder, true },
        { 0, false },
        { -1, false },
    };

    [Theory]
    [MemberData(nameof(GetByTableNameAndElementIdAndOperationType_ShouldSucceedToGetWithSameTableNameAndElementIdAndOperationType_WhenRecordExists_Data))]
    public void GetByTableNameAndElementIdAndOperationType_ShouldSucceedToGetWithSameTableNameAndElementIdAndOperationType_WhenRecordExists(
        int tableElementId, string tableName, ChangeOperationType changeOperationType, bool expected)
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        if (tableElementId == UseRequiredValuePlaceholder)
        {
            tableElementId = productId1;
        }

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetByTableNameAndElementIdAndOperationTypeAsync(
            tableName, tableElementId, changeOperationType);

        Assert.Equal(expected, toDoLocalChangeData is not null);

        if (toDoLocalChangeData is not null)
        {
            Assert.Equal(toDoLocalChangeId1, toDoLocalChangeData.Id);
            Assert.Equal(productId1, toDoLocalChangeData.TableElementId);
            Assert.Equal(_productsTableName, toDoLocalChangeData.TableName);
            Assert.Equal(toDoLocalChangeCreateRequest1.OperationType, toDoLocalChangeData.OperationType);
        }
    }

    public static TheoryData<int, string, ChangeOperationType, bool>
        GetByTableNameAndElementIdAndOperationType_ShouldSucceedToGetWithSameTableNameAndElementIdAndOperationType_WhenRecordExists_Data = new()
    {
        { UseRequiredValuePlaceholder, _productsTableName, ChangeOperationType.Create, true },
        { 0, _productsTableName, ChangeOperationType.Create, false },
        { -1, _productsTableName, ChangeOperationType.Create, false },
        { UseRequiredValuePlaceholder, string.Empty, ChangeOperationType.Create, false },
        { UseRequiredValuePlaceholder, "    ", ChangeOperationType.Create, false },
        { UseRequiredValuePlaceholder, _invalidTableName, ChangeOperationType.Create, false },
        { UseRequiredValuePlaceholder, _productsTableName, ChangeOperationType.Update, false },
        { UseRequiredValuePlaceholder, _productsTableName, ChangeOperationType.Delete, false },
    };

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedToInsert_WhenExpected_Data))]
    public void Insert_ShouldSucceedToInsert_WhenExpected(
        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest, bool expected)
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        if (toDoLocalChangeCreateRequest.TableElementId == UseRequiredValuePlaceholder)
        {
            toDoLocalChangeCreateRequest.TableElementId = productId1;
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoChangesInsertResult = _toDoLocalChangesService.InsertAsync(toDoLocalChangeCreateRequest);

        int? toDoLocalChangeId1 = toDoChangesInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.Equal(expected, toDoLocalChangeId1 is not null);

        if (toDoLocalChangeId1 is null) return;

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetByIdAsync(toDoLocalChangeId1.Value);

        Assert.Equal(expected, toDoLocalChangeData is not null);

        if (toDoLocalChangeData is not null)
        {
            Assert.Equal(toDoLocalChangeId1, toDoLocalChangeData.Id);
            Assert.Equal(toDoLocalChangeCreateRequest.TableElementId, toDoLocalChangeData.TableElementId);
            Assert.Equal(toDoLocalChangeCreateRequest.TableName, toDoLocalChangeData.TableName);
            Assert.Equal(toDoLocalChangeCreateRequest.OperationType, toDoLocalChangeData.OperationType);
        }
    }

    public static TheoryData<ServiceToDoLocalChangeCreateRequest, bool>
        Insert_ShouldSucceedToInsert_WhenExpected_Data = new()
    {
        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _productsTableName,
                OperationType = ChangeOperationType.Create,
            },
            true
        },

        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = 1,
                TableName = _productsTableName,
                OperationType = ChangeOperationType.Create,
            },
            true
        },

        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _productsTableName,
                OperationType = ChangeOperationType.Update,
            },
            true
        },

        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _productsTableName,
                OperationType = ChangeOperationType.Delete,
            },
            true
        },

        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = 0,
                TableName = _productsTableName,
                OperationType = ChangeOperationType.Create,
            },
            false
        },

        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = -1,
                TableName = _productsTableName,
                OperationType = ChangeOperationType.Create,
            },
            false
        },

        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = string.Empty,
                OperationType = ChangeOperationType.Create,
            },
            false
        },

        {
            new ServiceToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = "     ",
                OperationType = ChangeOperationType.Create,
            },
            false
        },
    };

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldDeleteAllWithSameTableNameAndElementId_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = productId2,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);

        List<int> ids = new() { productId1, productId2 };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByTableNameAndElementIdsAsync(_productsTableName, ids);

        Assert.True(isDeleteSuccessful);

        IEnumerable<LocalChangeData> toDoLocalChangesAfterDelete = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldFaliToDeleteWithSameTableNameAndElementId_WhenRecordsDontExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        List<int> ids = new() { productId1, productId2 };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByTableNameAndElementIdsAsync(_productsTableName, ids);

        Assert.False(isDeleteSuccessful);
    }

    [Fact]
    public void DeleteRangeByIds_ShouldDeleteAllWithSameId_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = productId2,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);

        List<int> ids = new() { toDoLocalChangeId1.Value, toDoLocalChangeId2.Value };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByIdsAsync(ids);

        Assert.True(isDeleteSuccessful);

        IEnumerable<LocalChangeData> toDoLocalChangesAfterDelete = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);
    }

    [Fact]
    public void DeleteRangeByIds_ShouldFaliToDeleteWithSameId_WhenRecordsDontExist()
    {
        List<int> ids = new() { 0, -1 };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByTableNameAndElementIdsAsync(_productsTableName, ids);

        Assert.False(isDeleteSuccessful);
    }

    [Fact]
    public void DeleteAllByTableNameAndElementId_ShouldDeleteAllWithSameTableNameAndElementId_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Update,

        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest3 = new()
        {
            TableElementId = productId2,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId3 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest3);

        Assert.NotNull(toDoLocalChangeId3);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 3);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType);

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteAllByTableNameAndElementIdAsync(_productsTableName, productId1);

        Assert.True(isDeleteSuccessful);

        IEnumerable<LocalChangeData> toDoLocalChangesAfterDelete = _toDoLocalChangesService.GetAllForTableAsync(_productsTableName);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType);


        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType);
    }

    [Fact]
    public void DeleteAllByTableNameAndElementId_ShouldFailToDeleteWithSameTableNameAndElementId_WhenRecordsDontExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteAllByTableNameAndElementIdAsync(_productsTableName, productId1);

        Assert.False(isDeleteSuccessful);
    }

    [Fact]
    public void DeleteById_ShouldDeletetWithSameId_WhenRecordExists()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetByIdAsync(toDoLocalChangeId1.Value);

        Assert.NotNull(toDoLocalChangeData);

        bool isDeleteByIdSuccessful = _toDoLocalChangesService.DeleteByIdAsync(toDoLocalChangeId1.Value);

        Assert.True(isDeleteByIdSuccessful);

        LocalChangeData? toDoLocalChangeDataAfterDelete = _toDoLocalChangesService.GetByIdAsync(toDoLocalChangeId1.Value);

        Assert.Null(toDoLocalChangeDataAfterDelete);
    }

    [Fact]
    public void DeleteById_ShouldFailToDeletetWithSameId_WhenRecordDoesntExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationType.Create,

        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetByIdAsync(toDoLocalChangeId1.Value);

        Assert.NotNull(toDoLocalChangeData);

        bool isDeleteByIdSuccessful = _toDoLocalChangesService.DeleteByIdAsync(0);

        Assert.False(isDeleteByIdSuccessful);

        LocalChangeData? toDoLocalChangeDataAfterDelete = _toDoLocalChangesService.GetByIdAsync(toDoLocalChangeId1.Value);

        Assert.NotNull(toDoLocalChangeDataAfterDelete);
    }
#pragma warning restore CA2211 // Non-constant fields should not be visible
}