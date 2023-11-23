using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using OneOf.Types;
using System.Linq;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

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

    private const string _useRequiredNameForUpdateValue = "Use required name for update";

    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly ICategoryService _categoryService;
    
    [Fact]
    public void GetAllByCategoryId_ShouldSucceed_WhenCategoryExists()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, "First");
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, "Second");

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
        _categoryService.DeleteRangeCategories(categoryId);
        DeleteRange(id1, id2);
    }

    [Fact]
    public void GetAllForSelectionOfCategoryIds_ShouldSucceed_WhenCategoryExists()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult1 = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId1 = categoryInsertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult2 = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId2 = categoryInsertResult2.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId1, "First");
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId1, "Second");
        ProductCharacteristicCreateRequest validCreateRequest3 = GetValidCharacteristicCreateRequest((int)categoryId2, "Third");

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult3 = _productCharacteristicService.Insert(validCreateRequest3);

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

        Assert.True(characteristicInsertResult3.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id3 = characteristicInsertResult3.AsT0;

        IEnumerable<IGrouping<uint, ProductCharacteristic>> characteristicsInCategories = _productCharacteristicService.GetAllForSelectionOfCategoryIds(new List<uint>() { categoryId1, categoryId2 });

        Assert.True(characteristicsInCategories.Count() >= 3);

        List<ProductCharacteristic> group1 = characteristicsInCategories.Single(x => x.Key == categoryId1).ToList();
        List<ProductCharacteristic> group2 = characteristicsInCategories.Single(x => x.Key == categoryId2).ToList();

        Assert.Contains(group1, x => x.Id == id1);
        Assert.Contains(group1, x => x.Id == id2);
        Assert.Contains(group2, x => x.Id == id3);

        // Deterministic delete
        _categoryService.DeleteRangeCategories(categoryId1, categoryId2);
        DeleteRange(id1, id2, id3);
    }

    [Fact]
    public void GetByCategoryIdAndName_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest = "NAME";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest);

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
        _categoryService.DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId);
    }

    [Fact]
    public void GetSelectionByCategoryIdAndNames_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest1 = "NAME_1";
        const string nameOfCreateRequest2 = "NAME_2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest1);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);

        Assert.True(characteristicInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId1 = characteristicInsertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

        Assert.True(characteristicInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId2 = characteristicInsertResult2.AsT0;

        List<string> createdProductCharacteristicsNames = new()
        {
            nameOfCreateRequest1,
            nameOfCreateRequest2
        };

        IEnumerable<ProductCharacteristic> characteristics = _productCharacteristicService.GetSelectionByCategoryIdAndNames(categoryId, createdProductCharacteristicsNames);

        Assert.True(characteristics.Count() >= 2);

        ProductCharacteristic productCharacteristic1 = characteristics.First(x => x.Id == characteristicId1);

        ProductCharacteristic productCharacteristic2 = characteristics.First(x => x.Id == characteristicId2);

        Assert.Equal(characteristicId1, (uint)productCharacteristic1.Id);
        Assert.Equal(validCreateRequest1.CategoryId, productCharacteristic1.CategoryId);
        Assert.Equal(validCreateRequest1.Name, productCharacteristic1.Name);
        Assert.Equal(validCreateRequest1.Meaning, productCharacteristic1.Meaning);
        Assert.Equal(validCreateRequest1.LastUpdate, productCharacteristic1.LastUpdate);
        Assert.Equal(validCreateRequest1.Active, productCharacteristic1.Active);
        Assert.Equal(validCreateRequest1.DisplayOrder, productCharacteristic1.DisplayOrder);
        Assert.Equal(validCreateRequest1.PKUserId, productCharacteristic1.PKUserId);
        Assert.Equal(validCreateRequest1.KWPrCh, productCharacteristic1.KWPrCh);

        Assert.Equal(characteristicId2, (uint)productCharacteristic2.Id);
        Assert.Equal(validCreateRequest2.CategoryId, productCharacteristic2.CategoryId);
        Assert.Equal(validCreateRequest2.Name, productCharacteristic2.Name);
        Assert.Equal(validCreateRequest2.Meaning, productCharacteristic2.Meaning);
        Assert.Equal(validCreateRequest2.LastUpdate, productCharacteristic2.LastUpdate);
        Assert.Equal(validCreateRequest2.Active, productCharacteristic2.Active);
        Assert.Equal(validCreateRequest2.DisplayOrder, productCharacteristic2.DisplayOrder);
        Assert.Equal(validCreateRequest2.PKUserId, productCharacteristic2.PKUserId);
        Assert.Equal(validCreateRequest2.KWPrCh, productCharacteristic2.KWPrCh);

        // Deterministic delete
        _categoryService.DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId1, characteristicId2);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ProductCharacteristicCreateRequest createRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

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
        _categoryService.DeleteRangeCategories(categoryId);
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
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId);

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

        Assert.Equal(expected, successUpdate);

        if (expected)
        {
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, updateRequest.Name);

            Assert.NotNull(characteristic);

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
        else
        {
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, validCreateRequest.Name);

            Assert.NotNull(characteristic);

            Assert.Equal(characteristicId, (uint)characteristic!.Id);
            Assert.Equal(validCreateRequest.Name, characteristic.Name);
            Assert.Equal(validCreateRequest.Meaning, characteristic.Meaning);
            Assert.Equal(validCreateRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(validCreateRequest.Active, characteristic.Active);
            Assert.Equal(validCreateRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(validCreateRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(validCreateRequest.KWPrCh, characteristic.KWPrCh);
        }

        // Deterministic delete
        _categoryService.DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId);
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
                Active = 1
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
                Active = 1
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
                KWPrCh = -12,
                DisplayOrder = 12,
                Active = 1
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateByNameAndCategoryId_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void UpdateByNameAndCategoryId_ShouldSucceedOrFail_InAnExpectedManner(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(validCreateRequest);

        bool successInsert = characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false);

        Assert.True(successInsert);

        uint characteristicId = characteristicInsertResult.AsT0;

        if (updateRequest.CategoryId == _useRequiredIdValue)
        {
            updateRequest.CategoryId = (int)categoryId;
        }

        if (updateRequest.Name == _useRequiredNameForUpdateValue)
        {
            updateRequest.Name = validCreateRequest.Name;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> characteristicUpdateResult = _productCharacteristicService.UpdateByNameAndCategoryId(updateRequest);

        bool successUpdate = characteristicUpdateResult.Match(
            _ => true,
            _ => false,
            _ => false);

        Assert.Equal(expected, successUpdate);

        if (expected)
        {
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, updateRequest.Name);

            Assert.NotNull(characteristic);

            Assert.Equal(characteristicId, (uint)characteristic!.Id);
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
            ProductCharacteristic? characteristic = _productCharacteristicService.GetByCategoryIdAndName(categoryId, validCreateRequest.Name);

            Assert.NotNull(characteristic);

            Assert.Equal(characteristicId, (uint)characteristic!.Id);
            Assert.Equal(validCreateRequest.Name, characteristic.Name);
            Assert.Equal(validCreateRequest.Meaning, characteristic.Meaning);
            Assert.Equal(validCreateRequest.LastUpdate, characteristic.LastUpdate);
            Assert.Equal(validCreateRequest.Active, characteristic.Active);
            Assert.Equal(validCreateRequest.DisplayOrder, characteristic.DisplayOrder);
            Assert.Equal(validCreateRequest.PKUserId, characteristic.PKUserId);
            Assert.Equal(validCreateRequest.KWPrCh, characteristic.KWPrCh);
        }

        // Deterministic delete
        _categoryService.DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId);
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
                Active = 1
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
                Active = 1
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
                Active = 1
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
                Active = 1
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
                Active = 1
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
                Active = 1
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

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest);

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
        _categoryService.DeleteRangeCategories(categoryId);
        DeleteRange(characteristicId);
    }

    [Fact]
    public void DeleteAllForCategory_ShouldSucceed_WhenInsertIsValid()
    {
        const string nameOfCreateRequest1 = "NAME_DELETE_1";
        const string nameOfCreateRequest2 = "NAME_DELETE_2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        ProductCharacteristicCreateRequest validCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest1);
        ProductCharacteristicCreateRequest validCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, nameOfCreateRequest2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(validCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(validCreateRequest2);

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
        _categoryService.DeleteRangeCategories(categoryId);
        DeleteRange(id1, id2);
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