using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductPropertyServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductPropertyServiceTests(
        IProductPropertyService productPropertyService,
        IProductCharacteristicService productCharacteristicService,
        IProductService productService,
        ICategoryService categoryService)
        : base(Startup.ConnectionString)
    {
        _productPropertyService = productPropertyService;
        _productCharacteristicService = productCharacteristicService;
        _productService = productService;
        _categoryService = categoryService;
    }

    private const int _useRequiredValue = -100;

    private const string _useRequiredNameForUpdateValue = "Use required name for update";

    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    private static ProductPropertyByCharacteristicIdCreateRequest GetValidCreateRequestById(int productId, int characteristicId, string value = "VAL") =>
    new()
    {
        ProductId = productId,
        ProductCharacteristicId = characteristicId,
        Value = value,
        DisplayOrder = 12,
        XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
    };

    private static ProductPropertyByCharacteristicNameCreateRequest GetValidCreateRequestByName(int productId, string characteristicName, string value = "VAL") =>
    new()
    {
        ProductId = productId,
        ProductCharacteristicName = characteristicName,
        Value = value,
        DisplayOrder = 12,
        XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
    };


    [Fact]
    public void GetAllInProduct_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertsAreValid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(characteristicCreateRequest2);

        Assert.True(characteristicInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        Assert.True(characteristicInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId1 = characteristicInsertResult1.AsT0;
        uint characteristicId2 = characteristicInsertResult2.AsT0;

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        IEnumerable<ProductProperty> characteristicsInCategory = _productPropertyService.GetAllInProduct(productId);

        Assert.True(characteristicsInCategory.Count() >= 2);

        Assert.Contains(characteristicsInCategory, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId1
        && x.Characteristic == characteristicName1
        && x.Value == propertyCreateRequest1.Value
        && x.DisplayOrder == propertyCreateRequest1.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest1.XmlPlacement);

        Assert.Contains(characteristicsInCategory, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest2.XmlPlacement);

        // Deterministic delete
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId1, characteristicId2);
    }

    [Fact]
    public void GetByNameAndProductId_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertIsValid()
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string propertyValue = "VAL1";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(characteristicCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId = characteristicInsertResult.AsT0;

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId);

        Assert.NotNull(property);

        Assert.Equal((int)productId, property.ProductId);
        Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
        Assert.Equal(characteristicName, property.Characteristic);
        Assert.Equal(propertyCreateRequest.Value, property.Value);
        Assert.Equal(propertyCreateRequest.DisplayOrder, property.DisplayOrder);
        Assert.Equal(propertyCreateRequest.XmlPlacement, property.XmlPlacement);

        // Deterministic delete
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId);
    }

    [Theory]
    [MemberData(nameof(InsertWithCharacteristicId_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertWithCharacteristicId_ShouldSucceedOrFail_InAnExpectedManner(ProductPropertyByCharacteristicIdCreateRequest createRequest, bool expected)
    {
        const string characteristicName = "NAMEOFCHAR1";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(characteristicCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId = characteristicInsertResult.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId;
        }

        if (createRequest.ProductCharacteristicId == _useRequiredValue)
        {
            createRequest.ProductCharacteristicId = (int)characteristicId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(createRequest);

        Assert.Equal(expected, propertyInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId);

        Assert.Equal(expected, property is not null);

        if (expected)
        {
            Assert.NotNull(property);

            Assert.Equal((int)productId, property.ProductId);
            Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
            Assert.Equal(characteristicName, property.Characteristic);
            Assert.Equal(createRequest.Value, property.Value);
            Assert.Equal(createRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(createRequest.XmlPlacement, property.XmlPlacement);
        }

        // Deterministic delete
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId);
    }

    public static readonly List<object[]> InsertWithCharacteristicId_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            GetValidCreateRequestById(_useRequiredValue, _useRequiredValue, "VAL_UPDATED"),
            true
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = "",
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = string.Empty,
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = "VAL_UPD",
                DisplayOrder = 0,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = "VAL_UPD",
                DisplayOrder = -12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },
    };

    [Theory]
    [MemberData(nameof(InsertWithCharacteristicName_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertWithCharacteristicName_ShouldSucceedOrFail_InAnExpectedManner(ProductPropertyByCharacteristicNameCreateRequest createRequest, bool expected)
    {
        const string characteristicName = "NAMEOFCHAR1";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(characteristicCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId = characteristicInsertResult.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId;
        }

        if (createRequest.ProductCharacteristicName == _useRequiredNameForUpdateValue)
        {
            createRequest.ProductCharacteristicName = characteristicName;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicName(createRequest);

        Assert.Equal(expected, propertyInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId);

        Assert.Equal(expected, property is not null);

        if (expected)
        {
            Assert.NotNull(property);

            Assert.Equal((int)productId, property.ProductId);
            Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
            Assert.Equal(characteristicName, property.Characteristic);
            Assert.Equal(createRequest.Value, property.Value);
            Assert.Equal(createRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(createRequest.XmlPlacement, property.XmlPlacement);
        }

        // Deterministic delete
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId);
    }

    public static readonly List<object[]> InsertWithCharacteristicName_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            GetValidCreateRequestByName(_useRequiredValue, _useRequiredNameForUpdateValue, "VAL_UPDATED"),
            true
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicName = "Memory size",
                Value = "VAL_UPD",
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicName = string.Empty,
                Value = "VAL_UPD",
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicName = "       ",
                Value = "VAL_UPD",
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = string.Empty,
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "     ",
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "VAL_UPD",
                DisplayOrder = 0,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "VAL_UPD",
                DisplayOrder = -12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },
    };

    [Theory]
    [MemberData(nameof(Update_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Update_ShouldSucceedOrFail_InAnExpectedManner(ProductPropertyUpdateRequest updateRequest, bool expected)
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string propertyValue = "VAL_INSERTED";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(characteristicCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId = characteristicInsertResult.AsT0;

        ProductPropertyByCharacteristicIdCreateRequest createRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(createRequest);

        Assert.True(propertyInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        if (updateRequest.ProductId == _useRequiredValue)
        {
            updateRequest.ProductId = (int)productId;
        }

        if (updateRequest.ProductCharacteristicId == _useRequiredValue)
        {
            updateRequest.ProductCharacteristicId = (int)characteristicId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyUpdateResult = _productPropertyService.Update(updateRequest);

        Assert.Equal(expected, propertyUpdateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId);

        Assert.NotNull(property);

        Assert.Equal((int)productId, property.ProductId);
        Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
        Assert.Equal(characteristicName, property.Characteristic);

        if (expected)
        {
            Assert.Equal(updateRequest.Value, property.Value);
            Assert.Equal(updateRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(updateRequest.XmlPlacement, property.XmlPlacement);
        }
        else
        {
            Assert.Equal(createRequest.Value, property.Value);
            Assert.Equal(createRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(createRequest.XmlPlacement, property.XmlPlacement);
        }

        // Deterministic delete
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId);
    }

    public static readonly List<object[]> Update_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = "VAL_UPDATED",
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        new object[2]
        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = "",
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = string.Empty,
                DisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = "VAL_UPD",
                DisplayOrder = 0,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        new object[2]
        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = _useRequiredValue,
                ProductCharacteristicId = _useRequiredValue,
                Value = "VAL_UPD",
                DisplayOrder = -12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },
    };

    [Fact]
    public void Delete_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertIsValid()
    {
        const string characteristicName = "NAMEOFCHAR1_DELETED";
        const string propertyValue = "VAL1";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = _productCharacteristicService.Insert(characteristicCreateRequest);

        Assert.True(characteristicInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId = characteristicInsertResult.AsT0;

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        bool success = _productPropertyService.Delete(productId, characteristicId);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId);

        Assert.Null(property);

        // Deterministic delete (in case delete in test fails)
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId);
    }

    [Fact]
    public void DeleteAllForProduct_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertsAreValid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(characteristicCreateRequest2);

        Assert.True(characteristicInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        Assert.True(characteristicInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId1 = characteristicInsertResult1.AsT0;
        uint characteristicId2 = characteristicInsertResult2.AsT0;

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        bool success = _productPropertyService.DeleteAllForProduct(productId);

        IEnumerable<ProductProperty> characteristicsInCategory = _productPropertyService.GetAllInProduct(productId);

        Assert.Empty(characteristicsInCategory);

        // Deterministic delete (in case delete in test fails)
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId1, characteristicId2);
    }

    [Fact]
    public void DeleteAllForCharacteristic_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertsAreValid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = _categoryService.Insert(ValidCategoryCreateRequest);

        Assert.True(categoryInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint categoryId = categoryInsertResult.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = _productCharacteristicService.Insert(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = _productCharacteristicService.Insert(characteristicCreateRequest2);

        Assert.True(characteristicInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        Assert.True(characteristicInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint characteristicId1 = characteristicInsertResult1.AsT0;
        uint characteristicId2 = characteristicInsertResult2.AsT0;

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        bool success = _productPropertyService.DeleteAllForCharacteristic(characteristicId1);

        IEnumerable<ProductProperty> characteristicsInCategory = _productPropertyService.GetAllInProduct(productId);


        Assert.NotEmpty(characteristicsInCategory);

        Assert.Contains(characteristicsInCategory, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest2.XmlPlacement);

        // Deterministic delete (in case delete in test fails)
        _productService.DeleteProducts(productId);
        _categoryService.DeleteRangeCategories(categoryId);
        _productCharacteristicService.DeleteRangeCharacteristics(characteristicId1, characteristicId2);
    }
}