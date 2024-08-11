using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductCharacteristicServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductCharacteristicServiceTests(
        IProductCharacteristicService productCharacteristicService, 
        ICategoryService categoryService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _productCharacteristicService = productCharacteristicService;
        _categoryService = categoryService;
    }

    private const int _useRequiredIdValue = -100;

    private const string _useRequiredNameForUpdateValue = "Use required name for update";

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly ICategoryService _categoryService;

    private readonly List<int> _categoryIdsToDelete = new();
    private readonly List<int> _productCharacteristicIdsToDelete = new();

    private void ScheduleCategoriesForDeleteAfterTest(params int[] ids)
    {
        _categoryIdsToDelete.AddRange(ids);
    }

    private void ScheduleProductCharacteristicsForDeleteAfterTest(params int[] ids)
    {
        _productCharacteristicIdsToDelete.AddRange(ids);
    }

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();

        _categoryService.DeleteRangeCategories(_categoryIdsToDelete.ToArray());
        _productCharacteristicService.DeleteRangeCharacteristics(_productCharacteristicIdsToDelete.ToArray());
    }

    [Fact]
    public void GetAllByCategoryId_ShouldSucceed_WhenCategoryExists()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First");
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second");

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetAllByCategoryId((int)categoryId);

        Assert.True(characteristicsInCategory.Count() >= 2);

        Assert.Contains(characteristicsInCategory, x => x.Id == id1);
        Assert.Contains(characteristicsInCategory, x => x.Id == id2);
    }

    [Fact]
    public void GetAllByCategoryId_ShouldFail_WhenCategoryIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First");
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second");

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetAllByCategoryId(-111);

        Assert.Empty(characteristicsInCategory);
    }

    [Fact]
    public void GetCharacteristicsOnlyByCategoryId_ShouldSucceed_WhenCategoryExists()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second", ProductCharacteristicTypeEnum.ProductCharacteristic);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetCharacteristicsOnlyByCategoryId((int)categoryId);

        Assert.True(characteristicsInCategory.Count() >= 2);

        Assert.Contains(characteristicsInCategory, x => x.Id == id1);
        Assert.Contains(characteristicsInCategory, x => x.Id == id2);
    }

    [Fact]
    public void GetCharacteristicsOnlyByCategoryId_ShouldOnlyGetCharacteristics_WhenCategoryExists()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId, "Third", ProductCharacteristicTypeEnum.SearchStringAbbreviation);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetCharacteristicsOnlyByCategoryId((int)categoryId);

        Assert.Equal(2, characteristicsInCategory.Count());

        Assert.Contains(characteristicsInCategory, x => x.Id == id1);
        Assert.Contains(characteristicsInCategory, x => x.Id == id2);
        Assert.DoesNotContain(characteristicsInCategory, x => x.Id == id3);
    }

    [Fact]
    public void GetCharacteristicsOnlyByCategoryId_ShouldFail_WhenCategoryIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second", ProductCharacteristicTypeEnum.ProductCharacteristic);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetCharacteristicsOnlyByCategoryId(-111);

        Assert.Empty(characteristicsInCategory);
    }

    [Fact]
    public void GetSearchStringAbbreviationsOnlyByCategoryId_ShouldSucceed_WhenCategoryExists()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second", ProductCharacteristicTypeEnum.SearchStringAbbreviation);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetSearchStringAbbreviationsOnlyByCategoryId((int)categoryId);

        Assert.True(characteristicsInCategory.Count() >= 2);

        Assert.Contains(characteristicsInCategory, x => x.Id == id1);
        Assert.Contains(characteristicsInCategory, x => x.Id == id2);
    }

    [Fact]
    public void GetSearchStringAbbreviationsOnlyByCategoryId_ShouldOnlyGetSearchStringAbbreviations_WhenCategoryExists()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId, "Third", ProductCharacteristicTypeEnum.ProductCharacteristic);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetSearchStringAbbreviationsOnlyByCategoryId((int)categoryId);

        Assert.Equal(2, characteristicsInCategory.Count());

        Assert.Contains(characteristicsInCategory, x => x.Id == id1);
        Assert.Contains(characteristicsInCategory, x => x.Id == id2);
        Assert.DoesNotContain(characteristicsInCategory, x => x.Id == id3);
    }

    [Fact]
    public void GetSearchStringAbbreviationsOnlyByCategoryId_ShouldFail_WhenCategoryIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second", ProductCharacteristicTypeEnum.SearchStringAbbreviation);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetCharacteristicsOnlyByCategoryId(-111);

        Assert.Empty(characteristicsInCategory);
    }

    [Fact]
    public void GetAllForSelectionOfCategoryIds_ShouldSucceed_WhenCategoriesExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First");
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second");
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third");

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories
            = _productCharacteristicService.GetAllForSelectionOfCategoryIds(new List<int>() { (int)categoryId1, (int)categoryId2 });

        Assert.Equal(2, characteristicsInCategories.Count());

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();
        List<ProductCharacteristic> group2 = characteristicsInCategories.Single(x => x.Key == categoryId2).ToList();

        Assert.Contains(group1, x => x.Id == id1);
        Assert.Contains(group1, x => x.Id == id2);
        Assert.Contains(group2, x => x.Id == id3);
    }

    [Fact]
    public void GetAllForSelectionOfCategoryIds_ShouldFail_ForCategoriesThatDontExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First");
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second");
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third");

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        List<int> categoryIds = new() { (int)categoryId1, 0 };

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories = _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoryIds);

        Assert.Single(characteristicsInCategories);

        List<ProductCharacteristic> group1 = characteristicsInCategories
            .Single(x => x.Key == categoryId1)
            .ToList();

        Assert.DoesNotContain(characteristicsInCategories, x => x.Key == 0);

        Assert.Contains(group1, x => x.Id == id1);
        Assert.Contains(group1, x => x.Id == id2);
    }

    [Fact]
    public void GetCharacteristicsOnlyForSelectionOfCategoryIds_ShouldSucceed_WhenCategoriesExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third", ProductCharacteristicTypeEnum.ProductCharacteristic);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        List<int> categoryIds = new() { (int)categoryId1, (int)categoryId2 };

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories = _productCharacteristicService.GetCharacteristicsOnlyForSelectionOfCategoryIds(categoryIds);

        Assert.Equal(2, characteristicsInCategories.Count());

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();
        List<ProductCharacteristic> group2 = characteristicsInCategories.Single(x => x.Key == categoryId2).ToList();

        Assert.Contains(group1, x => x.Id == id1);
        Assert.Contains(group1, x => x.Id == id2);
        Assert.Contains(group2, x => x.Id == id3);
    }

    [Fact]
    public void GetCharacteristicsOnlyForSelectionOfCategoryIds_ShouldOnlyGetSearchStringAbbreviations_WhenCategoriesExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third", ProductCharacteristicTypeEnum.ProductCharacteristic);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        List<int> categoryIds = new() { (int)categoryId1, (int)categoryId2 };

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories = _productCharacteristicService.GetCharacteristicsOnlyForSelectionOfCategoryIds(categoryIds);

        Assert.Equal(2, characteristicsInCategories.Count());

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();
        List<ProductCharacteristic> group2 = characteristicsInCategories.Single(x => x.Key == categoryId2).ToList();

        Assert.Contains(group1, x => x.Id == id1);
        Assert.DoesNotContain(group1, x => x.Id == id2);
        Assert.Contains(group2, x => x.Id == id3);
    }

    [Fact]
    public void GetCharacteristicsOnlyForSelectionOfCategoryIds_ShouldFail_ForCategoriesThatDontExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third", ProductCharacteristicTypeEnum.ProductCharacteristic);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        List<int> categoryIds = new() { (int)categoryId1, 0 };

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories
            = _productCharacteristicService.GetCharacteristicsOnlyForSelectionOfCategoryIds(categoryIds);

        Assert.Single(characteristicsInCategories);

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();

        Assert.DoesNotContain(characteristicsInCategories, x => x.Key == 0);

        Assert.Contains(group1, x => x.Id == id1);
        Assert.Contains(group1, x => x.Id == id2);
    }

    [Fact]
    public void GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds_ShouldSucceed_WhenCategoriesExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third", ProductCharacteristicTypeEnum.SearchStringAbbreviation);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        List<int> categoryIds = new() { (int)categoryId1, (int)categoryId2 };

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories = _productCharacteristicService.GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds(categoryIds);

        Assert.True(characteristicsInCategories.Count() == 2);

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();
        List<ProductCharacteristic> group2 = characteristicsInCategories.Single(x => x.Key == categoryId2).ToList();

        Assert.Contains(group1, x => x.Id == id1);
        Assert.Contains(group1, x => x.Id == id2);
        Assert.Contains(group2, x => x.Id == id3);
    }

    [Fact]
    public void GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds_ShouldOnlyGetSearchStringAbbreviations_WhenCategoriesExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second", ProductCharacteristicTypeEnum.ProductCharacteristic);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third", ProductCharacteristicTypeEnum.SearchStringAbbreviation);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        List<int> categoryIds = new() { (int)categoryId1, (int)categoryId2 };

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories
            = _productCharacteristicService.GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds(categoryIds);

        Assert.Equal(2, characteristicsInCategories.Count());

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();
        List<ProductCharacteristic> group2 = characteristicsInCategories.Single(x => x.Key == categoryId2).ToList();

        Assert.Contains(group1, x => x.Id == id1);
        Assert.DoesNotContain(group1, x => x.Id == id2);
        Assert.Contains(group2, x => x.Id == id3);
    }

    [Fact]
    public void GetSearchStringAbbreviationsOnlyForSelectionOfCategoryIds_ShouldFail_ForCategoriesThatDontExist()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId1 = categoryInsertResult1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId2 = categoryInsertResult2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId1);
        Assert.True(categoryId1 > 0);

        Assert.NotNull(categoryId2);
        Assert.True(categoryId2 > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second", ProductCharacteristicTypeEnum.SearchStringAbbreviation);
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third", ProductCharacteristicTypeEnum.SearchStringAbbreviation);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);


        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id3 = characteristicInsertResult3.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        Assert.NotNull(id3);
        Assert.True(id3 > 0);

        List<int> categoryIds = new() { (int)categoryId1, 0 };

        IEnumerable<IGrouping<int, ProductCharacteristic>> characteristicsInCategories = _productCharacteristicService.GetAllForSelectionOfCategoryIds(categoryIds);

        Assert.Single(characteristicsInCategories);

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();

        Assert.DoesNotContain(characteristicsInCategories, x => x.Key == 0);

        Assert.Contains(group1, x => x.Id == id1);
        Assert.Contains(group1, x => x.Id == id2);
    }

    [Fact]
    public void GetByCategoryIdAndName_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest = "NAME";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, nameOfCreateRequest);

        Assert.NotNull(characteristic);

        Assert.Equal(characteristicId, characteristic.Id);
        Assert.Equal(validCreateRequest.CategoryId, characteristic.CategoryId);
        Assert.Equal(validCreateRequest.Name, characteristic.Name);
        Assert.Equal(validCreateRequest.Meaning, characteristic.Meaning);
        Assert.Equal(validCreateRequest.LastUpdate, characteristic.LastUpdate);
        Assert.Equal(validCreateRequest.Active, characteristic.Active);
        Assert.Equal(validCreateRequest.DisplayOrder, characteristic.DisplayOrder);
        Assert.Equal(validCreateRequest.PKUserId, characteristic.PKUserId);
        Assert.Equal(validCreateRequest.KWPrCh, characteristic.KWPrCh);
    }

    [Fact]
    public void GetByCategoryIdAndName_ShouldFail_WhenInsertIsInvalid()
    {
        const string invalidNameOfCharacteristc = "INV_Name";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest invalidCreateRequest = new()
        {
            CategoryId = (int)categoryId,
            Name = "        ",
            Meaning = "Name of the object",
            PKUserId = 91,
            KWPrCh = 0,
            DisplayOrder = 12,
            Active = true
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(invalidCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return false;
            },
            validationResult => true,
            unexpectedFailureResult => false));

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, invalidNameOfCharacteristc);

        Assert.Null(characteristic);
    }

    [Fact]
    public void GetByCategoryIdAndName_ShouldFail_WhenNameDoesNotMatch()
    {
        const string nameOfCreateRequest = "NAME";
        const string invalidNameOfCategory = "INV_Name";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, invalidNameOfCategory);

        Assert.Null(characteristic);
    }

    [Fact]
    public void GetByCategoryIdAndName_ShouldFail_WhenCategoryIdDoesNotMatch()
    {
        const string nameOfCreateRequest = "NAME";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId - 1, nameOfCreateRequest);

        Assert.Null(characteristic);
    }

    [Fact]
    public void GetSelectionByCategoryIdAndNames_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest1 = "NAME_1";
        const string nameOfCreateRequest2 = "NAME_2";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest1);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);

        int? characteristicId1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? characteristicId2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.True(characteristicId1 > 0);

        Assert.NotNull(characteristicId2);
        Assert.True(characteristicId2 > 0);

        List<string> createdProductCharacteristicsNames = new()
        {
            nameOfCreateRequest1,
            nameOfCreateRequest2
        };

        IEnumerable<ProductCharacteristic> characteristics = _productCharacteristicService.GetSelectionByCategoryIdAndNames(
            (int)categoryId, createdProductCharacteristicsNames);

        Assert.True(characteristics.Count() >= 2);

        ProductCharacteristic productCharacteristic1 = characteristics.First(x => x.Id == characteristicId1);

        ProductCharacteristic productCharacteristic2 = characteristics.First(x => x.Id == characteristicId2);

        Assert.Equal(characteristicId1, productCharacteristic1.Id);
        Assert.Equal(validCreateRequest1.CategoryId, productCharacteristic1.CategoryId);
        Assert.Equal(validCreateRequest1.Name, productCharacteristic1.Name);
        Assert.Equal(validCreateRequest1.Meaning, productCharacteristic1.Meaning);
        Assert.Equal(validCreateRequest1.LastUpdate, productCharacteristic1.LastUpdate);
        Assert.Equal(validCreateRequest1.Active, productCharacteristic1.Active);
        Assert.Equal(validCreateRequest1.DisplayOrder, productCharacteristic1.DisplayOrder);
        Assert.Equal(validCreateRequest1.PKUserId, productCharacteristic1.PKUserId);
        Assert.Equal(validCreateRequest1.KWPrCh, productCharacteristic1.KWPrCh);

        Assert.Equal(characteristicId2, productCharacteristic2.Id);
        Assert.Equal(validCreateRequest2.CategoryId, productCharacteristic2.CategoryId);
        Assert.Equal(validCreateRequest2.Name, productCharacteristic2.Name);
        Assert.Equal(validCreateRequest2.Meaning, productCharacteristic2.Meaning);
        Assert.Equal(validCreateRequest2.LastUpdate, productCharacteristic2.LastUpdate);
        Assert.Equal(validCreateRequest2.Active, productCharacteristic2.Active);
        Assert.Equal(validCreateRequest2.DisplayOrder, productCharacteristic2.DisplayOrder);
        Assert.Equal(validCreateRequest2.PKUserId, productCharacteristic2.PKUserId);
        Assert.Equal(validCreateRequest2.KWPrCh, productCharacteristic2.KWPrCh);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ProductCharacteristicCreateRequest createRequest, bool expected)
    {
        Assert.NotNull(createRequest.Name);

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        if (createRequest.CategoryId == _useRequiredIdValue)
        {
            createRequest.CategoryId = (int)categoryId;
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(createRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.Equal(expected, characteristicId is not null);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, createRequest.Name);

        if (expected)
        {
            Assert.NotNull(characteristicId);
            Assert.True(characteristicId > 0);

            Assert.Equal(expected, characteristic is not null);

            Assert.Equal(characteristicId, characteristic!.Id);
            Assert.Equal(createRequest.CategoryId, characteristic.CategoryId);
            Assert.Equal(createRequest.Name, characteristic.Name);
            Assert.Equal(createRequest.Meaning, characteristic.Meaning);
            Assert.Equal(createRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(createRequest.Active, characteristic.Active);
            Assert.Equal(createRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(createRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(createRequest.KWPrCh, characteristic.KWPrCh);
        }
        else
        {
            Assert.Null(characteristic);
        }
    }

    public static List<object[]> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            GetValidCharacteristicCreateRequest(_useRequiredIdValue),
            true
        },

        new object[2]
        {
            new ProductCharacteristicCreateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 0,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ProductCharacteristicCreateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = "",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicCreateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = "          ",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicCreateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicCreateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "        ",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateById_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateById_ShouldSucceedOrFail_InAnExpectedManner(ProductCharacteristicByIdUpdateRequest updateRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        if (updateRequest.Id == _useRequiredIdValue)
        {
            updateRequest.Id = (int)characteristicId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> characteristicUpdateResult = _productCharacteristicService.UpdateById(updateRequest);

        bool successUpdate = characteristicUpdateResult.Match(
            _ => true,
            _ => false,
            _ => false);

        Assert.Equal(expected, successUpdate);

        if (expected)
        {
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, updateRequest.Name);

            Assert.NotNull(characteristic);

            Assert.Equal(characteristicId, characteristic!.Id);
            Assert.Equal(updateRequest.Id, characteristic.Id);
            Assert.Equal(updateRequest.Name, characteristic.Name);
            Assert.Equal(updateRequest.Meaning, characteristic.Meaning);
            Assert.Equal(updateRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(updateRequest.Active, characteristic.Active);
            Assert.Equal(updateRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(updateRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(updateRequest.KWPrCh, characteristic.KWPrCh);
        }
        else
        {
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, validCreateRequest.Name);

            Assert.NotNull(characteristic);

            Assert.Equal(characteristicId, characteristic!.Id);
            Assert.Equal(validCreateRequest.Name, characteristic.Name);
            Assert.Equal(validCreateRequest.Meaning, characteristic.Meaning);
            Assert.Equal(validCreateRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(validCreateRequest.Active, characteristic.Active);
            Assert.Equal(validCreateRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(validCreateRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(validCreateRequest.KWPrCh, characteristic.KWPrCh);
        }
    }

    public static List<object[]> UpdateById_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ProductCharacteristicByIdUpdateRequest()
            {
                Id = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ProductCharacteristicByIdUpdateRequest()
            {
                Id = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 0,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ProductCharacteristicByIdUpdateRequest()
            {
                Id = _useRequiredIdValue,
                Name = "",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByIdUpdateRequest()
            {
                Id = _useRequiredIdValue,
                Name = "          ",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByIdUpdateRequest()
            {
                Id = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByIdUpdateRequest()
            {
                Id = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "        ",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByIdUpdateRequest()
            {
                Id = _useRequiredIdValue,
                Name = "NAME",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = null,
                DisplayOrder = 12,
                Active = true
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateByNameAndCategoryId_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateByNameAndCategoryId_ShouldSucceedOrFail_InAnExpectedManner(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        if (updateRequest.CategoryId == _useRequiredIdValue)
        {
            updateRequest.CategoryId = (int)categoryId;
        }

        if (updateRequest.Name == _useRequiredNameForUpdateValue)
        {
            updateRequest.Name = validCreateRequest.Name!;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> characteristicUpdateResult = _productCharacteristicService.UpdateByNameAndCategoryId(updateRequest);

        bool successUpdate = characteristicUpdateResult.Match(
            _ => true,
            _ => false,
            _ => false);

        Assert.Equal(expected, successUpdate);

        if (expected)
        {
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, updateRequest.Name);

            Assert.NotNull(characteristic);

            Assert.Equal(characteristicId, characteristic!.Id);
            Assert.Equal(updateRequest.CategoryId, characteristic.CategoryId);
            Assert.Equal(updateRequest.NewName, characteristic.Name);
            Assert.Equal(updateRequest.Meaning, characteristic.Meaning);
            Assert.Equal(updateRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(updateRequest.Active, characteristic.Active);
            Assert.Equal(updateRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(updateRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(updateRequest.KWPrCh, characteristic.KWPrCh);
        }
        else
        {
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, validCreateRequest.Name);

            Assert.NotNull(characteristic);

            Assert.Equal(characteristicId, characteristic!.Id);
            Assert.Equal(validCreateRequest.Name, characteristic.Name);
            Assert.Equal(validCreateRequest.Meaning, characteristic.Meaning);
            Assert.Equal(validCreateRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(validCreateRequest.Active, characteristic.Active);
            Assert.Equal(validCreateRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(validCreateRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(validCreateRequest.KWPrCh, characteristic.KWPrCh);
        }
    }

    public static List<object[]> UpdateByNameAndCategoryId_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ProductCharacteristicByNameAndCategoryIdUpdateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = _useRequiredNameForUpdateValue,
                NewName = "NAME",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ProductCharacteristicByNameAndCategoryIdUpdateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = _useRequiredNameForUpdateValue,
                NewName = "NAME",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 0,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ProductCharacteristicByNameAndCategoryIdUpdateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = _useRequiredNameForUpdateValue,
                NewName = "",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByNameAndCategoryIdUpdateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = _useRequiredNameForUpdateValue,
                NewName = "          ",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByNameAndCategoryIdUpdateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = _useRequiredNameForUpdateValue,
                NewName = "NAME",
                Meaning = "",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByNameAndCategoryIdUpdateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = _useRequiredNameForUpdateValue,
                NewName = "NAME",
                Meaning = "        ",
                PKUserId = 91,
                KWPrCh = 0,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ProductCharacteristicByNameAndCategoryIdUpdateRequest()
            {
                CategoryId = _useRequiredIdValue,
                Name = _useRequiredNameForUpdateValue,
                NewName = "NAME",
                Meaning = "Name of the object",
                PKUserId = 91,
                KWPrCh = null,
                DisplayOrder = 12,
                Active = true
            },
            false
        },
    };

    [Fact]
    public void Delete_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest = "NAME_DELETE";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        bool success = _productCharacteristicService.Delete(characteristicId.Value);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, nameOfCreateRequest);

        Assert.Null(characteristic);
    }

    [Fact]
    public void Delete_ShouldFail_WhenInsertIsInvalid()
    {
        const string nameOfCreateRequest = "NAME_DELETE";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        bool success = _productCharacteristicService.Delete(0);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName((int)categoryId, nameOfCreateRequest);

        Assert.NotNull(characteristic);
    }

    [Fact]
    public void DeleteAllForCategory_ShouldSucceed_WhenInsertsAreValid()
    {
        const string nameOfCreateRequest1 = "NAME_DELETE_1";
        const string nameOfCreateRequest2 = "NAME_DELETE_2";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest1);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        bool success = _productCharacteristicService.DeleteAllForCategory((int)categoryId);

        IEnumerable<ProductCharacteristic> characteristics = _productCharacteristicService.GetAllByCategoryId((int)categoryId);

        Assert.Empty(characteristics);
    }

    [Fact]
    public void DeleteAllForCategory_ShouldFail_WhenInsertsAreInvalid()
    {
        const string invalidNameOfCreateRequest1 = "   ";
        const string invalidNameOfCreateRequest2 = "";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, invalidNameOfCreateRequest1);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, invalidNameOfCreateRequest2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        Assert.True(characteristicInsertResult1.Match(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return false;
            },
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.True(characteristicInsertResult2.Match(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return false;
            },
            validationResult => true,
            unexpectedFailureResult => false));

        bool success = _productCharacteristicService.DeleteAllForCategory((int)categoryId);

        Assert.False(success);

        IEnumerable<ProductCharacteristic> characteristics = _productCharacteristicService.GetAllByCategoryId((int)categoryId);

        Assert.Empty(characteristics);
    }

    [Fact]
    public void DeleteAllForCategory_ShouldFail_WhenCategoryIdIsInvalid()
    {
        const string nameOfCreateRequest1 = "NAME_DELETE_1";
        const string nameOfCreateRequest2 = "NAME_DELETE_2";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest1);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        int? id1 = characteristicInsertResult1.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id2 = characteristicInsertResult2.Match<int?>(
            id =>
            {
                ScheduleProductCharacteristicsForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        bool success = _productCharacteristicService.DeleteAllForCategory(-111);

        Assert.False(success);

        IEnumerable<ProductCharacteristic> characteristics = _productCharacteristicService.GetAllByCategoryId((int)categoryId);

        Assert.Equal(2, characteristics.Count());
    }
}