using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Tests.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class CategoryServiceTests : IntegrationTestBaseForNonWebProjects
{
    public CategoryServiceTests(ICategoryService categoryService)
        : base(Startup.ConnectionString)
    {
        _categoryService = categoryService;
    }

    private readonly ICategoryService _categoryService;

    private readonly ServiceCategoryCreateRequest _validCreateRequest = new()
    {
        Description = "description",
        DisplayOrder = 13123,
        ProductsUpdateCounter = 0,
        ParentCategoryId = null
    };

    private const int _insertRequiredIdValue = -100;

    [Fact]
    public void GetAll_ShouldSucceed_WithSuccessfullyCreatedObjects()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _categoryService.Insert(_validCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result2 = _categoryService.Insert(_validCreateRequest);

        Assert.True(result1.IsT0);
        Assert.True(result2.IsT0);

        IEnumerable<Category> categories = _categoryService.GetAll();

        Assert.True(categories.Count() >= 2);

        uint id1 = result1.AsT0;
        uint id2 = result2.AsT0;

        Assert.Contains(categories, x => x.Id == (int)id1);
        Assert.Contains(categories, x => x.Id == (int)id2);

        // Deterministic Delete
        DeleteRange(id1, id2);
    }

    [Fact]
    public void GetById_ShouldSucceed_WithSuccessfullyCreatedObjects()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(_validCreateRequest);

        Assert.True(createResult.IsT0);

        uint id = createResult.AsT0;

        Category? category = _categoryService.GetById(id);

        Assert.NotNull(category);

        Assert.True(category.Description == _validCreateRequest.Description);
        Assert.True(category.DisplayOrder == _validCreateRequest.DisplayOrder);
        Assert.True(category.ProductsUpdateCounter == _validCreateRequest.ProductsUpdateCounter);
        Assert.True(category.ParentCategoryId == _validCreateRequest.ParentCategoryId);
        Assert.True(category.IsLeaf == false);

        //Assert.NotNull(category.RowGuid);

        Assert.True(category.RowGuid != Guid.Empty);

        DeleteRange(id);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InExpectedManner(ServiceCategoryCreateRequest request, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(request);

        uint? id = createResult.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.Equal(expected, id is not null);

        Category? category = null;

        if (id is not null)
        {
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

        if (expected)
        {
            DeleteRange(id!.Value);
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
        OneOf<uint, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(_validCreateRequest);

        uint? id = createResult.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(id);

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

        DeleteRange(id.Value);
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
        OneOf<uint, ValidationResult, UnexpectedFailureResult> createResult = _categoryService.Insert(_validCreateRequest);

        uint? id = createResult.Match<uint?>(
            id => id,
            _ => null,
            _ => null);

        Assert.NotNull(id);

        Category? categoryInserted = _categoryService.GetById(id.Value);

        Assert.NotNull(categoryInserted);

        bool success = _categoryService.Delete(id.Value);

        Category? categoriesDelete = _categoryService.GetById(id.Value);

        Assert.Null(categoriesDelete);
        Assert.True(success);
    }

    private bool DeleteRange(params uint[] ids)
    {
        foreach (uint id in ids)
        {
            bool success = _categoryService.Delete(id);

            if (!success) return false;
        }

        return true;
    }
}