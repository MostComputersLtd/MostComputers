using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using OneOf.Types;
using System.Runtime.CompilerServices;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductCharacteristicServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductCharacteristicServiceTests(
        IProductCharacteristicService productCharacteristicService, 
        ICategoryService categoryService)
        : base(Startup.ConnectionString)
    {
        _productCharacteristicService = productCharacteristicService;
        _categoryService = categoryService;
    }

    private const int _useRequiredIdValue = -100;

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly ICategoryService _categoryService;

    private static ProductCharacteristicCreateRequest GetValidCreateRequest(int categoryId, string name = "Name") =>
    new ()
    {
        CategoryId = categoryId,
        Name = name,
        Meaning = "Name of the object",
        PKUserId = 91,
        KWPrCh = 0,
        DisplayOrder = 12,
        Active = 1
    };

    private readonly ServiceCategoryCreateRequest _validCategoryCreateRequest = new()
    {
        Description = "description",
        DisplayOrder = 13123,
        ProductsUpdateCounter = 0,
        ParentCategoryId = null
    };

    [Fact]
    public void GetAllByCategoryId_ShouldSucceed_WhenCategoryExists()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(_validCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCreateRequest((int)categoryId, "First");
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCreateRequest((int)categoryId, "Second");

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        Assert.True(characteristicInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id1 = characteristicInsertResult2.AsT0;

        Assert.True(characteristicInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id2 = characteristicInsertResult2.AsT0;

        IEnumerable<ProductCharacteristic> characteristicsInCategory = _productCharacteristicService.GetAllByCategoryId(categoryId);

        Assert.True(characteristicsInCategory.Count() >= 2);

        Assert.Contains(characteristicsInCategory, x => x.Id == id1);
        Assert.Contains(characteristicsInCategory, x => x.Id == id2);

        // Deterministic delete
        DeleteRangeCategories(categoryId);
        DeleteRange(id1, id2);
    }

    [Fact]
    public void GetByCategoryIdAndName_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest = "NAME";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(_validCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId = characteristicInsertResult.AsT0;

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, nameOfCreateRequest);

        Assert.NotNull(characteristic);

        Assert.Equal(characteristicId, (uint)characteristic.Id);
        Assert.Equal(validCreateRequest.CategoryId, characteristic.CategoryId);
        Assert.Equal(validCreateRequest.Name, characteristic.Name);
        Assert.Equal(validCreateRequest.Meaning, characteristic.Meaning);
        Assert.Equal(validCreateRequest.LastUpdate, characteristic.LastUpdate);
        Assert.Equal(validCreateRequest.Active, characteristic.Active);
        Assert.Equal(validCreateRequest.DisplayOrder, characteristic.DisplayOrder);
        Assert.Equal(validCreateRequest.PKUserId, characteristic.PKUserId);
        Assert.Equal(validCreateRequest.KWPrCh, characteristic.KWPrCh);

        // Deterministic delete
        DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ProductCharacteristicCreateRequest createRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(_validCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        if (createRequest.CategoryId == _useRequiredIdValue)
        {
            createRequest.CategoryId = (int)categoryId;
        }

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(createRequest);

        bool success = characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false);

        Assert.Equal(expected, success);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, createRequest.Name);

        if (expected)
        {
            uint characteristicId = characteristicInsertResult.AsT0;

            Assert.Equal(expected, characteristic is not null);

            Assert.Equal(characteristicId, (uint)characteristic!.Id);
            Assert.Equal(createRequest.CategoryId, characteristic.CategoryId);
            Assert.Equal(createRequest.Name, characteristic.Name);
            Assert.Equal(createRequest.Meaning, characteristic.Meaning);
            Assert.Equal(createRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(createRequest.Active, characteristic.Active);
            Assert.Equal(createRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(createRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(createRequest.KWPrCh, characteristic.KWPrCh);

            // Deterministic delete
            DeleteRange(characteristicId);
        }
        else
        {
            Assert.Null(characteristic);
        }

        // Deterministic delete
        DeleteRangeCategories(categoryId);
    }

    public static List<object[]> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            GetValidCreateRequest(_useRequiredIdValue),
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
                Active = 1
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
                Active = 1
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
                Active = 1
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
                Active = 1
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
                Active = 1
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateById_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateById_ShouldSucceedOrFail_InAnExpectedManner(ProductCharacteristicByIdUpdateRequest updateRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(_validCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCreateRequest((int)categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        bool successInsert = characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false);

        Assert.True(successInsert);

        uint characteristicId = characteristicInsertResult.AsT0;

        if (updateRequest.Id == _useRequiredIdValue)
        {
            updateRequest.Id = (int)characteristicId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> characteristicUpdateResult = _productCharacteristicService.UpdateById(updateRequest);

        bool successUpdate = characteristicUpdateResult.Match(
            _ => true,
            _ => false,
            _ => false);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, updateRequest.Name);

        Assert.NotNull(characteristic);
        Assert.Equal(expected, successUpdate);

        if (expected)
        {
            Assert.Equal(characteristicId, (uint)characteristic!.Id);
            Assert.Equal(updateRequest.Id, characteristic.Id);
            Assert.Equal(updateRequest.Name, characteristic.Name);
            Assert.Equal(updateRequest.Meaning, characteristic.Meaning);
            Assert.Equal(updateRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(updateRequest.Active, characteristic.Active);
            Assert.Equal(updateRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(updateRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(updateRequest.KWPrCh, characteristic.KWPrCh);
        }

        // Deterministic delete
        DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId);
    }

    public static List<object[]> UpdateById_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            GetValidCreateRequest(_useRequiredIdValue),
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
                Active = 1
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
                Active = 1
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
                Active = 1
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
                Active = 1
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
                KWPrCh = 0,
                DisplayOrder = 0,
                Active = 1
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
                KWPrCh = -12,
                DisplayOrder = 12,
                Active = 1
            },
            false
        },
    };

    [Fact]
    public void Delete_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest = "NAME_DELETE";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(_validCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId = characteristicInsertResult.AsT0;

        bool success = _productCharacteristicService.Delete(characteristicId);

        ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, nameOfCreateRequest);

        Assert.Null(characteristic);

        // Deterministic delete (in case delete in test fails)
        DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId);
    }

    [Fact]
    public void DeleteAllForCategory_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest = "NAME_DELETE";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(_validCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCreateRequest((int)categoryId, nameOfCreateRequest);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest);

        Assert.True(characteristicInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id1 = characteristicInsertResult1.AsT0;

        Assert.True(characteristicInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id2 = characteristicInsertResult2.AsT0;

        bool success = _productCharacteristicService.DeleteAllForCategory(categoryId);

        IEnumerable<ProductCharacteristic> characteristics = _productCharacteristicService.GetAllByCategoryId(categoryId);

        Assert.Empty(characteristics);

        // Deterministic delete (in case delete in test fails)
        DeleteRangeCategories(categoryId);
        DeleteRange(id1, id2);
    }

    private bool DeleteRangeCategories(params uint[] ids)
    {
        foreach (uint id in ids)
        {
            bool success = _categoryService.Delete(id);

            if (!success) return false;
        }

        return true;
    }

    private bool DeleteRange(params uint[] ids)
    {
        foreach (uint id in ids)
        {
            bool success = _productCharacteristicService.Delete(id);

            if (!success) return false;
        }

        return true;
    }
}