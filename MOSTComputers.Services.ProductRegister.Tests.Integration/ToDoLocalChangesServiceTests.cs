using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Models.Product.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.SuccessfulInsertAbstractions;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

public sealed class ToDoLocalChangesServiceTests : IntegrationTestBaseForNonWebProjects
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

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = 1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAll();

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);
    }

    [Fact]
    public void GetAll_ShouldGetOnlySuccessfullyInserted_WhenOnlySomeInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = -1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoLocalChangeInsertResult = _toDoLocalChangesService.Insert(toDoLocalChangeCreateRequest2);

        bool hasToDoLocalChangeInsertFailedWithValidationResult = toDoLocalChangeInsertResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(hasToDoLocalChangeInsertFailedWithValidationResult);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAll();

        Assert.NotEmpty(toDoLocalChanges);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);
    }

    [Fact]
    public void GetAllForTable_ShouldGetAllWithSameTableName_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = 1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.All(toDoLocalChanges, x => Assert.Equal(_productsTableName, x.TableName));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);
    }

    [Fact]
    public void GetAllForTable_ShouldGetOnlySuccessfullyInsertedWithSameTableName_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = -1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoLocalChangeInsertResult = _toDoLocalChangesService.Insert(toDoLocalChangeCreateRequest2);

        bool hasToDoLocalChangeInsertFailedWithValidationResult = toDoLocalChangeInsertResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(hasToDoLocalChangeInsertFailedWithValidationResult);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.NotEmpty(toDoLocalChanges);

        Assert.All(toDoLocalChanges, x => Assert.Equal(_productsTableName, x.TableName));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetAllWithSameOperation_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = 1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest3 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Update,
            TimeStamp = DateTime.Now
        };

        int? toDoLocalChangeId3 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest3);

        Assert.NotNull(toDoLocalChangeId3);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.All(toDoLocalChanges, x => Assert.Equal(ChangeOperationTypeEnum.Create, x.OperationType));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest3.TimeStamp);
    }

    [Fact]
    public void GetAllForOperationType_ShouldGetOnlySuccessfullyInsertedWithSameOperation_WhenAllInsertsAreValid()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = -1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoLocalChangeInsertResult = _toDoLocalChangesService.Insert(toDoLocalChangeCreateRequest2);

        bool hasToDoLocalChangeInsertFailedWithValidationResult = toDoLocalChangeInsertResult.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false);

        Assert.True(hasToDoLocalChangeInsertFailedWithValidationResult);


        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest3 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Update,
            TimeStamp = DateTime.Now
        };

        int? toDoLocalChangeId3 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest3);

        Assert.NotNull(toDoLocalChangeId3);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForOperationType(ChangeOperationTypeEnum.Create);

        Assert.NotEmpty(toDoLocalChanges);

        Assert.All(toDoLocalChanges, x => Assert.Equal(ChangeOperationTypeEnum.Create, x.OperationType));

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);

        Assert.DoesNotContain(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest3.TimeStamp);
    }

    [Theory]
    [MemberData(nameof(GetById_ShouldSucceedToGetWithSameId_WhenRecordExists_Data))]
    public void GetById_ShouldSucceedToGetWithSameId_WhenRecordExists(int id, bool expected)
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        if (id == UseRequiredValuePlaceholder)
        {
            id = toDoLocalChangeId1.Value;
        }

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetById(id);

        Assert.Equal(expected, toDoLocalChangeData is not null);

        if (toDoLocalChangeData is not null)
        {
            Assert.Equal(toDoLocalChangeId1, toDoLocalChangeData.Id);
            Assert.Equal(productId1, toDoLocalChangeData.TableElementId);
            Assert.Equal(_productsTableName, toDoLocalChangeData.TableName);
            Assert.Equal(toDoLocalChangeCreateRequest1.OperationType, toDoLocalChangeData.OperationType);
            Assert.Equal(toDoLocalChangeCreateRequest1.TimeStamp, toDoLocalChangeData.TimeStamp);
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
        int tableElementId, string tableName, ChangeOperationTypeEnum changeOperationType, bool expected)
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        if (tableElementId == UseRequiredValuePlaceholder)
        {
            tableElementId = productId1;
        }

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetByTableNameAndElementIdAndOperationType(
            tableName, tableElementId, changeOperationType);

        Assert.Equal(expected, toDoLocalChangeData is not null);

        if (toDoLocalChangeData is not null)
        {
            Assert.Equal(toDoLocalChangeId1, toDoLocalChangeData.Id);
            Assert.Equal(productId1, toDoLocalChangeData.TableElementId);
            Assert.Equal(_productsTableName, toDoLocalChangeData.TableName);
            Assert.Equal(toDoLocalChangeCreateRequest1.OperationType, toDoLocalChangeData.OperationType);
            Assert.Equal(toDoLocalChangeCreateRequest1.TimeStamp, toDoLocalChangeData.TimeStamp);
        }
    }

    public static TheoryData<int, string, ChangeOperationTypeEnum, bool>
        GetByTableNameAndElementIdAndOperationType_ShouldSucceedToGetWithSameTableNameAndElementIdAndOperationType_WhenRecordExists_Data = new()
    {
        { UseRequiredValuePlaceholder, _productsTableName, ChangeOperationTypeEnum.Create, true },
        { 0, _productsTableName, ChangeOperationTypeEnum.Create, false },
        { -1, _productsTableName, ChangeOperationTypeEnum.Create, false },
        { UseRequiredValuePlaceholder, string.Empty, ChangeOperationTypeEnum.Create, false },
        { UseRequiredValuePlaceholder, "    ", ChangeOperationTypeEnum.Create, false },
        { UseRequiredValuePlaceholder, _invalidTableName, ChangeOperationTypeEnum.Create, false },
        { UseRequiredValuePlaceholder, _productsTableName, ChangeOperationTypeEnum.Update, false },
        { UseRequiredValuePlaceholder, _productsTableName, ChangeOperationTypeEnum.Delete, false },
    };

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedToInsert_WhenExpected_Data))]
    public void Insert_ShouldSucceedToInsert_WhenExpected(
        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest, bool expected)
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        if (toDoLocalChangeCreateRequest.TableElementId == UseRequiredValuePlaceholder)
        {
            toDoLocalChangeCreateRequest.TableElementId = productId1;
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoChangesInsertResult = _toDoLocalChangesService.Insert(toDoLocalChangeCreateRequest);

        int? toDoLocalChangeId1 = toDoChangesInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.Equal(expected, toDoLocalChangeId1 is not null);

        if (toDoLocalChangeId1 is null) return;

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetById(toDoLocalChangeId1.Value);

        Assert.Equal(expected, toDoLocalChangeData is not null);

        if (toDoLocalChangeData is not null)
        {
            Assert.Equal(toDoLocalChangeId1, toDoLocalChangeData.Id);
            Assert.Equal(toDoLocalChangeCreateRequest.TableElementId, toDoLocalChangeData.TableElementId);
            Assert.Equal(toDoLocalChangeCreateRequest.TableName, toDoLocalChangeData.TableName);
            Assert.Equal(toDoLocalChangeCreateRequest.OperationType, toDoLocalChangeData.OperationType);
            Assert.Equal(toDoLocalChangeCreateRequest.TimeStamp, toDoLocalChangeData.TimeStamp);
        }
    }

    public static TheoryData<ToDoLocalChangeCreateRequest, bool>
        Insert_ShouldSucceedToInsert_WhenExpected_Data = new()
    {
        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _productsTableName,
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = DateTime.Now,
            },
            true
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = 1,
                TableName = _productsTableName,
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = DateTime.Now,
            },
            true
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _productsTableName,
                OperationType = ChangeOperationTypeEnum.Update,
                TimeStamp = DateTime.Now,
            },
            true
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _productsTableName,
                OperationType = ChangeOperationTypeEnum.Delete,
                TimeStamp = DateTime.Now,
            },
            true
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = 0,
                TableName = _productsTableName,
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = DateTime.Now,
            },
            false
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = -1,
                TableName = _productsTableName,
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = DateTime.Now,
            },
            false
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = string.Empty,
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = DateTime.Now,
            },
            false
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = "     ",
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = DateTime.Now,
            },
            false
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _invalidTableName,
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = DateTime.Now,
            },
            false
        },

        {
            new ToDoLocalChangeCreateRequest()
            {
                TableElementId = UseRequiredValuePlaceholder,
                TableName = _productsTableName,
                OperationType = ChangeOperationTypeEnum.Create,
                TimeStamp = new DateTime(0),
            },
            false
        },
    };

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldDeleteAllWithSameTableNameAndElementId_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = productId2,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);

        List<int> ids = new() { productId1, productId2 };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByTableNameAndElementIds(_productsTableName, ids);

        Assert.True(isDeleteSuccessful);

        IEnumerable<LocalChangeData> toDoLocalChangesAfterDelete = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);
    }

    [Fact]
    public void DeleteRangeByTableNameAndElementIds_ShouldFaliToDeleteWithSameTableNameAndElementId_WhenRecordsDontExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        List<int> ids = new() { productId1, productId2 };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByTableNameAndElementIds(_productsTableName, ids);

        Assert.False(isDeleteSuccessful);
    }

    [Fact]
    public void DeleteRangeByIds_ShouldDeleteAllWithSameId_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = productId2,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 2);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);

        List<int> ids = new() { toDoLocalChangeId1.Value, toDoLocalChangeId2.Value };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByIds(ids);

        Assert.True(isDeleteSuccessful);

        IEnumerable<LocalChangeData> toDoLocalChangesAfterDelete = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);
    }

    [Fact]
    public void DeleteRangeByIds_ShouldFaliToDeleteWithSameId_WhenRecordsDontExist()
    {
        List<int> ids = new() { 0, -1 };

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteRangeByTableNameAndElementIds(_productsTableName, ids);

        Assert.False(isDeleteSuccessful);
    }

    [Fact]
    public void DeleteAllByTableNameAndElementId_ShouldDeleteAllWithSameTableNameAndElementId_WhenRecordsExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);
        int productId2 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest2 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Update,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId2 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest2);

        Assert.NotNull(toDoLocalChangeId2);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest3 = new()
        {
            TableElementId = productId2,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId3 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest3);

        Assert.NotNull(toDoLocalChangeId3);

        IEnumerable<LocalChangeData> toDoLocalChanges = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.True(toDoLocalChanges.Count() >= 3);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);

        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest3.TimeStamp);

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteAllByTableNameAndElementId(_productsTableName, productId1);

        Assert.True(isDeleteSuccessful);

        IEnumerable<LocalChangeData> toDoLocalChangesAfterDelete = _toDoLocalChangesService.GetAllForTable(_productsTableName);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId1
            && x.TableElementId == toDoLocalChangeCreateRequest1.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest1.TableName
            && x.OperationType == toDoLocalChangeCreateRequest1.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest1.TimeStamp);

        Assert.DoesNotContain(toDoLocalChangesAfterDelete, x =>
            x.Id == toDoLocalChangeId2
            && x.TableElementId == toDoLocalChangeCreateRequest2.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest2.TableName
            && x.OperationType == toDoLocalChangeCreateRequest2.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest2.TimeStamp);


        Assert.Contains(toDoLocalChanges, x =>
            x.Id == toDoLocalChangeId3
            && x.TableElementId == toDoLocalChangeCreateRequest3.TableElementId
            && x.TableName == toDoLocalChangeCreateRequest3.TableName
            && x.OperationType == toDoLocalChangeCreateRequest3.OperationType
            && x.TimeStamp == toDoLocalChangeCreateRequest3.TimeStamp);
    }

    [Fact]
    public void DeleteAllByTableNameAndElementId_ShouldFailToDeleteWithSameTableNameAndElementId_WhenRecordsDontExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        bool isDeleteSuccessful = _toDoLocalChangesService.DeleteAllByTableNameAndElementId(_productsTableName, productId1);

        Assert.False(isDeleteSuccessful);
    }

    [Fact]
    public void DeleteById_ShouldDeletetWithSameId_WhenRecordExists()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetById(toDoLocalChangeId1.Value);

        Assert.NotNull(toDoLocalChangeData);

        bool isDeleteByIdSuccessful = _toDoLocalChangesService.DeleteById(toDoLocalChangeId1.Value);

        Assert.True(isDeleteByIdSuccessful);

        LocalChangeData? toDoLocalChangeDataAfterDelete = _toDoLocalChangesService.GetById(toDoLocalChangeId1.Value);

        Assert.Null(toDoLocalChangeDataAfterDelete);
    }

    [Fact]
    public void DeleteById_ShouldFailToDeletetWithSameId_WhenRecordDoesntExist()
    {
        int productId1 = InsertProductAndGetIdOrThrow(_productService, ValidProductCreateRequestWithNoImages);

        ToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest1 = new()
        {
            TableElementId = productId1,
            TableName = _productsTableName,
            OperationType = ChangeOperationTypeEnum.Create,
            TimeStamp = DateTime.Now,
        };

        int? toDoLocalChangeId1 = InsertToDoLocalChangeAndGetIdOrNull(_toDoLocalChangesService, toDoLocalChangeCreateRequest1);

        Assert.NotNull(toDoLocalChangeId1);

        LocalChangeData? toDoLocalChangeData = _toDoLocalChangesService.GetById(toDoLocalChangeId1.Value);

        Assert.NotNull(toDoLocalChangeData);

        bool isDeleteByIdSuccessful = _toDoLocalChangesService.DeleteById(0);

        Assert.False(isDeleteByIdSuccessful);

        LocalChangeData? toDoLocalChangeDataAfterDelete = _toDoLocalChangesService.GetById(toDoLocalChangeId1.Value);

        Assert.NotNull(toDoLocalChangeDataAfterDelete);
    }
#pragma warning restore CA2211 // Non-constant fields should not be visible
}