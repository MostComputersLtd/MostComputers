using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class CategoryServiceTests : IntegrationTestBaseForNonWebProjects
{
    public CategoryServiceTests(ICategoryService categoryService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _categoryService = categoryService;
    }

    private readonly ICategoryService _categoryService;

    private const int _insertRequiredIdValue = -100;

    private readonly ServiceCategoryCreateRequest _invalidCategoryCreateRequest = new()
    {
        Description = "     ",
        DisplayOrder = 13123,
        ProductsUpdateCounter = 0,
        ParentCategoryId = null
    };

    private readonly List<int> _categoryIdsToDelete = new();

    private void ScheduleCategoriesForDeleteAfterTest(params int[] categoryIds)
    {
        _categoryIdsToDelete.AddRange(categoryIds);
    }

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();

        DeleteRange(_categoryIdsToDelete.ToArray());
    }

    [Fact]
    public void GetAll_ShouldSucceed_WithSuccessfullyCreatedObjects()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result1 = _categoryService.Insert(ValidCategoryCreateRequest);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result2 = _categoryService.Insert(ValidCategoryCreateRequest);

        int? id1 = result1.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        int? id2 = result2.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id1);
        Assert.True(id1 > 0);

        Assert.NotNull(id2);
        Assert.True(id2 > 0);

        IEnumerable<Category> categories = _categoryService.GetAll();

        Assert.True(categories.Count() >= 2);

        Assert.Contains(categories, x => x.Id == (int)id1);
        Assert.Contains(categories, x => x.Id == (int)id2);
    }

    [Fact]
    public void GetAll_ShouldFail_WithUnsuccessfullyCreatedObjects()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result1 = _categoryService.Insert(_invalidCategoryCreateRequest);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result2 = _categoryService.Insert(_invalidCategoryCreateRequest);

        Assert.True(result1.Match(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return false;
            },
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.True(result2.Match(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return false;
            },
            validationResult => true,
            unexpectedFailureResult => false));

        IEnumerable<Category> categories = _categoryService.GetAll();

        Assert.DoesNotContain(categories,
            x => x.Description == _invalidCategoryCreateRequest.Description
            && x.DisplayOrder == _invalidCategoryCreateRequest.DisplayOrder
            && x.ProductsUpdateCounter == _invalidCategoryCreateRequest.ProductsUpdateCounter
            && x.ParentCategoryId == _invalidCategoryCreateRequest.ParentCategoryId);
    }

    [Fact]
    public void GetById_ShouldSucceed_WithSuccessfullyCreatedObjects()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? id = createResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id);
        Assert.True(id > 0);

        Category? category = _categoryService.GetById(id.Value);

        Assert.NotNull(category);

        Assert.True(category.Description == ValidCategoryCreateRequest.Description);
        Assert.True(category.DisplayOrder == ValidCategoryCreateRequest.DisplayOrder);
        Assert.True(category.ProductsUpdateCounter == ValidCategoryCreateRequest.ProductsUpdateCounter);
        Assert.True(category.ParentCategoryId == ValidCategoryCreateRequest.ParentCategoryId);
        Assert.True(category.IsLeaf == false);

        //Assert.NotNull(category.RowGuid);

        Assert.True(category.RowGuid != Guid.Empty);
    }

    [Fact]
    public void GetById_ShouldFail_WithInvalidId()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(createResult.IsT0);

        int? id = createResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(id);
        Assert.True(id > 0);

        Category? category = _categoryService.GetById(id.Value + 1000);

        Assert.Null(category);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InExpectedManner(ServiceCategoryCreateRequest request, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(request);

        int? id = createResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            _ => null,
            _ => null);

        Assert.Equal(expected, id is not null);

        Category? category = null;

        if (id is not null)
        {
            Assert.True(id > 0);

            category = _categoryService.GetById(id.Value);
        }

        Assert.Equal(expected, category is not null);

        if (expected)
        {
            Assert.True(category!.Description == request.Description);
            Assert.True(category.DisplayOrder == request.DisplayOrder);
            Assert.True(category.ProductsUpdateCounter == request.ProductsUpdateCounter);
            Assert.True(category.ParentCategoryId == request.ParentCategoryId);
            Assert.True(category.IsLeaf == false);

            //Assert.NotNull(category.RowGuid);

            Assert.True(category.RowGuid != Guid.Empty);
        }
    }

    public static List<object[]> Insert_ShouldSucceedOrFail_InExpectedManner_Data => new()
    {
        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "description",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
                ParentCategoryId = null
            },
            true
        },

        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "description sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
                ParentCategoryId = null
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "description",
                DisplayOrder = -1,
                ProductsUpdateCounter = 0,
                ParentCategoryId = null
            },
            false
        },

         new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "description",
                DisplayOrder = 0,
                ProductsUpdateCounter = 0,
                ParentCategoryId = null
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "description",
                DisplayOrder = 13123,
                ProductsUpdateCounter = -1,
                ParentCategoryId = null
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "description",
                DisplayOrder = 0,
                ProductsUpdateCounter = -1,
                ParentCategoryId = null
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
                ParentCategoryId = null
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "         ",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
                ParentCategoryId = null
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryCreateRequest()
            {
                Description = "description",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
                ParentCategoryId = 0
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(Update_ShouldSucceedOrFail_InExpectedManner_Data))]
    public void Update_ShouldSucceedOrFail_InExpectedManner(ServiceCategoryUpdateRequest request, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? id = createResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            _ => null,
            _ => null);

        Assert.NotNull(id);
        Assert.True(id > 0);

        Category? categoryInserted = _categoryService.GetById(id.Value);

        Assert.NotNull(categoryInserted);

        if (request.Id == _insertRequiredIdValue) request.Id = categoryInserted.Id;

        _categoryService.Update(request);

        Category? updatedCategory = _categoryService.GetById(id.Value);

        Assert.NotNull(updatedCategory);

        if (expected)
        {
            Assert.True(updatedCategory!.Description == request.Description);
            Assert.True(updatedCategory.DisplayOrder == request.DisplayOrder);
            Assert.True(updatedCategory.ProductsUpdateCounter == request.ProductsUpdateCounter);

            //Assert.NotNull(category.RowGuid);

            Assert.True(updatedCategory.RowGuid != Guid.Empty);
        }
    }

    public static List<object[]> Update_ShouldSucceedOrFail_InExpectedManner_Data => new()
    {
        new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "description",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
            },
            true
        },

        new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "description sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "description",
                DisplayOrder = -1,
                ProductsUpdateCounter = 0,
            },
            false
        },

         new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "description",
                DisplayOrder = 0,
                ProductsUpdateCounter = 0,
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "description",
                DisplayOrder = 13123,
                ProductsUpdateCounter = -1,
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "description",
                DisplayOrder = 0,
                ProductsUpdateCounter = -1
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
            },
            false
        },

        new object[2]
        {
            new ServiceCategoryUpdateRequest()
            {
                Id = _insertRequiredIdValue,
                Description = "         ",
                DisplayOrder = 13123,
                ProductsUpdateCounter = 0,
            },
            false
        },
    };

    [Fact]
    public void Delete_ShouldSucceed_WhenIdIsValid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? id = createResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            _ => null,
            _ => null);

        Assert.NotNull(id);
        Assert.True(id > 0);

        Category? categoryInserted = _categoryService.GetById(id.Value);

        Assert.NotNull(categoryInserted);

        bool success = _categoryService.Delete(id.Value);

        Category? categoriesDelete = _categoryService.GetById(id.Value);

        Assert.Null(categoriesDelete);
        Assert.True(success);
    }

    [Fact]
    public void Delete_ShouldFail_WhenIdIsInvalid()
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(ValidCategoryCreateRequest);

        int? id = createResult.Match<int?>(
            id =>
            {
                ScheduleCategoriesForDeleteAfterTest(id);

                return id;
            },
            _ => null,
            _ => null);

        Assert.NotNull(id);
        Assert.True(id > 0);

        Category? categoryInserted = _categoryService.GetById(id.Value);

        Assert.NotNull(categoryInserted);

        bool success = _categoryService.Delete(id.Value + 1000);

        Category? categoryToDelete = _categoryService.GetById(id.Value);

        Assert.NotNull(categoryToDelete);
        Assert.False(success);
    }

    private bool DeleteRange(params int[] ids)
    {
        foreach (int id in ids)
        {
            bool success = _categoryService.Delete(id);

            if (!success) return false;
        }

        return true;
    }
}