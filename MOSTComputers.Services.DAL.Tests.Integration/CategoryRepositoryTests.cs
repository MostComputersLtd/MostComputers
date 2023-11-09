using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class CategoryRepositoryTests : IntegrationTestBaseWithContainer<MOSTComputers.Services.ProductRegister.IEntryPoint, CustomWebApplicationFactory>
{
    public CategoryRepositoryTests(CustomWebApplicationFactory webApplicationFactory)
        : base(webApplicationFactory)
    {
        _categoryService = webApplicationFactory.Services.GetRequiredService<ICategoryService>();
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
        ServiceCategoryCreateRequest request = new()
        {
            Description = "description",
            DisplayOrder = 13123,
            ProductsUpdateCounter = 0,
            ParentCategoryId = null
        };

        _categoryService.Insert(request);
        _categoryService.Insert(request);

        IEnumerable<Category> categories = _categoryService.GetAll();

        Assert.True(categories.Count() == 2);
    }

    [Fact]
    public void GetById_ShouldSucceed_WithSuccessfullyCreatedObjects()
    {
        ServiceCategoryCreateRequest request = new()
        {
            Description = "description",
            DisplayOrder = 13123,
            ProductsUpdateCounter = 0,
            ParentCategoryId = null
        };

        _categoryService.Insert(request);

        IEnumerable<Category> categories = _categoryService.GetAll();

        Assert.True(categories.Count() == 1);

        int id = categories.First().Id;

        Category? category = _categoryService.GetById((uint)id);

        Assert.NotNull(category);

        Assert.True(category.Description == request.Description);
        Assert.True(category.DisplayOrder == request.DisplayOrder);
        Assert.True(category.ProductsUpdateCounter == request.ProductsUpdateCounter);
        Assert.True(category.ParentCategoryId == request.ParentCategoryId);
        Assert.True(category.IsLeaf == false);

        //Assert.NotNull(category.RowGuid);

        Assert.True(category.RowGuid != Guid.Empty);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InExpectedManner(ServiceCategoryCreateRequest request, bool expected)
    {
        _categoryService.Insert(request);

        IEnumerable<Category> categories = _categoryService.GetAll();

        Assert.Equal(categories.Count() == 1, expected);

        Category category = categories.First();

        Assert.Equal(category is not null, expected);

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
        _categoryService.Insert(_validCreateRequest);

        IEnumerable<Category> categoriesInsert = _categoryService.GetAll();

        Assert.Equal(categoriesInsert.Count() == 1, expected);

        Category categoryInserted = categoriesInsert.First();

        Assert.Equal(categoryInserted is not null, expected);

        if (request.Id == _insertRequiredIdValue) request.Id = categoryInserted!.Id;

        _categoryService.Update(request);

        IEnumerable<Category> categories = _categoryService.GetAll();

        Assert.Equal(categories.Count() == 1, expected);

        Category category = categories.First();

        Assert.Equal(category is not null, expected);

        if (expected)
        {
            Assert.True(category!.Description == request.Description);
            Assert.True(category.DisplayOrder == request.DisplayOrder);
            Assert.True(category.ProductsUpdateCounter == request.ProductsUpdateCounter);

            //Assert.NotNull(category.RowGuid);

            Assert.True(category.RowGuid != Guid.Empty);
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
        _categoryService.Insert(_validCreateRequest);

        IEnumerable<Category> categoriesInsert = _categoryService.GetAll();

        Assert.Single(categoriesInsert);

        Category categoryInserted = categoriesInsert.First();

        Assert.NotNull(categoryInserted);

        uint id = (uint)categoryInserted.Id;

        _categoryService.Delete(id);

        IEnumerable<Category> categoriesDelete = _categoryService.GetAll();

        Assert.Empty(categoriesDelete);
    }
}