using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
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
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
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

    private readonly List<uint> _categoriesToDeleteIds = new();
    private readonly List<uint> _productCharacteristicsToDeleteIds = new();

    private OneOf<uint, ValidationResult, UnexpectedFailureResult> InsertCategoryAndAddIdToDelete(ServiceCategoryCreateRequest categoryCreateRequest)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _categoryService.Insert(categoryCreateRequest);

        result.Switch(
            id =>
            {
                _categoriesToDeleteIds.Add(id);
            },
            validationResult => { },
            unexpectedFailureResult => { });

        return result;
    }

    private OneOf<uint, ValidationResult, UnexpectedFailureResult> InsertProductCharacteristicAndAddIdToDelete(ProductCharacteristicCreateRequest characteristicCreateRequest)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _productCharacteristicService.Insert(characteristicCreateRequest);

        result.Switch(
            id =>
            {
                _productCharacteristicsToDeleteIds.Add(id);
            },
            validationResult => { },
            unexpectedFailureResult => { });

        return result;
    }

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();

        _categoryService.DeleteRangeCategories(_categoriesToDeleteIds.ToArray());
        _productCharacteristicService.DeleteRangeCharacteristics(_productCharacteristicsToDeleteIds.ToArray());
    }

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

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        uint? characteristicId1 = characteristicInsertResult1.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        uint? characteristicId2 = characteristicInsertResult2.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.NotNull(characteristicId2);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        IEnumerable<ProductProperty> propsInProduct = _productPropertyService.GetAllInProduct(productId.Value);

        Assert.True(propsInProduct.Count() >= 2);

        Assert.Contains(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId1
        && x.Characteristic == characteristicName1
        && x.Value == propertyCreateRequest1.Value
        && x.DisplayOrder == propertyCreateRequest1.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest1.XmlPlacement);

        Assert.Contains(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest2.XmlPlacement);
    }

    [Fact]
    public void GetAllInProduct_ShouldFail_WhenProductExistsAndInsertsAreValid_ButCharacteristicDoesNotExist()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, 0, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        IEnumerable<ProductProperty> propsInProduct = _productPropertyService.GetAllInProduct(productId.Value);

        Assert.NotEmpty(propsInProduct);

        Assert.Contains(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId
        && x.Characteristic == characteristicName1
        && x.Value == propertyCreateRequest1.Value
        && x.DisplayOrder == propertyCreateRequest1.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest1.XmlPlacement);

        Assert.DoesNotContain(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == 0
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest2.XmlPlacement);
    }

    [Fact]
    public void GetAllInProduct_ShouldFail_WhenCharacteristicExistsAndInsertsAreValid_ButProductDoesNotExist()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";
        const int invalidProductId = 0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        uint? characteristicId1 = characteristicInsertResult1.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        uint? characteristicId2 = characteristicInsertResult2.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.NotNull(characteristicId2);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById(invalidProductId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById(invalidProductId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        IEnumerable<ProductProperty> propsInProduct = _productPropertyService.GetAllInProduct(invalidProductId);

        Assert.Empty(propsInProduct);
    }

    [Fact]
    public void GetAllInProduct_ShouldFail_WhenCharacteristicAndProductExist_ButInsertsAreInvalid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string invalidPropertyValue1 = " ";
        const string invalidPropertyValue2 = "";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        ProductCreateRequest productCreateRequest = GetValidProductCreateRequest((int)categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(productCreateRequest);

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        uint? characteristicId1 = characteristicInsertResult1.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        uint? characteristicId2 = characteristicInsertResult2.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.NotNull(characteristicId2);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, invalidPropertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, invalidPropertyValue2);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest1);
        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest2);

        IEnumerable<ProductProperty> propsInProduct = _productPropertyService.GetAllInProduct(productId.Value);

        if (productCreateRequest.Properties is not null)
        {
            Assert.Equal(productCreateRequest.Properties.Count, propsInProduct.Count());
        }

        Assert.DoesNotContain(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId1
        && x.Characteristic == characteristicName1
        && x.Value == invalidPropertyCreateRequest1.Value
        && x.DisplayOrder == invalidPropertyCreateRequest1.DisplayOrder
        && x.XmlPlacement == invalidPropertyCreateRequest1.XmlPlacement);

        Assert.DoesNotContain(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == invalidPropertyCreateRequest2.Value
        && x.DisplayOrder == invalidPropertyCreateRequest2.DisplayOrder
        && x.XmlPlacement == invalidPropertyCreateRequest2.XmlPlacement);
    }

    [Fact]
    public void GetByNameAndProductId_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertIsValid()
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string propertyValue = "VAL1";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

        Assert.NotNull(property);

        Assert.Equal((int)productId, property.ProductId);
        Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
        Assert.Equal(characteristicName, property.Characteristic);
        Assert.Equal(propertyCreateRequest.Value, property.Value);
        Assert.Equal(propertyCreateRequest.DisplayOrder, property.DisplayOrder);
        Assert.Equal(propertyCreateRequest.XmlPlacement, property.XmlPlacement);
    }

    [Fact]
    public void GetByNameAndProductId_ShouldFail_WhenProductExists_ButCharacteristicDoesNotExist()
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string propertyValue = "VAL1";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById((int)productId, 0, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

        Assert.Null(property);
    }

    [Fact]
    public void GetByNameAndProductId_ShouldFail_WhenCharacteristicExists_ButProductDoesNotExist()
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string propertyValue = "VAL1";
        const int invalidProductId = 0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById(invalidProductId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, invalidProductId);

        Assert.Null(property);
    }

    [Fact]
    public void GetByNameAndProductId_ShouldFail_WhenCharacteristicAndProductExist_ButInsertIsInvalid()
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string invalidPropertyValue = "";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, invalidPropertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

        Assert.Null(property);
    }

    [Theory]
    [MemberData(nameof(InsertWithCharacteristicId_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void InsertWithCharacteristicId_ShouldSucceedOrFail_InAnExpectedManner(ProductPropertyByCharacteristicIdCreateRequest createRequest, bool expected)
    {
        const string characteristicName = "NAMEOFCHAR1";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

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
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

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

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

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
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

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

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest createRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(createRequest);

        Assert.True(propertyInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

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
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

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

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        bool success = _productPropertyService.Delete(productId.Value, characteristicId.Value);

        Assert.True(success);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

        Assert.Null(property);
    }

    [Fact]
    public void Delete_ShouldFail_WhenCharacteristicExists_ButProductDoesNotExist()
    {
        const string characteristicName = "NAMEOFCHAR1_DELETED";
        const string propertyValue = "VAL1";
        const int invalidProductId = 0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById(invalidProductId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        bool success = _productPropertyService.Delete(invalidProductId, characteristicId.Value);

        Assert.False(success);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, invalidProductId);

        Assert.Null(property);
    }

    [Fact]
    public void Delete_ShouldFail_WhenProductExists_ButCharacteristicDoesNotExist()
    {
        const string characteristicName = "NAMEOFCHAR1_DELETED";
        const string propertyValue = "VAL1";
        const int invalidCharacteristicId = 0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById((int)productId, invalidCharacteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        bool success = _productPropertyService.Delete(productId.Value, invalidCharacteristicId);

        Assert.False(success);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

        Assert.Null(property);
    }

    [Fact]
    public void Delete_ShouldFail_WhenCharacteristicAndProductExist_ButInsertIsInvalid()
    {
        const string characteristicName = "NAMEOFCHAR1_DELETED";
        const string invalidPropertyValue = "";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        uint? characteristicId = characteristicInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, invalidPropertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        bool success = _productPropertyService.Delete(productId.Value, characteristicId.Value);

        Assert.False(success);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, productId.Value);

        Assert.Null(property);
    }

    [Fact]
    public void DeleteAllForProduct_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertsAreValid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        uint? characteristicId1 = characteristicInsertResult1.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        uint? characteristicId2 = characteristicInsertResult2.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.NotNull(characteristicId2);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        bool success = _productPropertyService.DeleteAllForProduct(productId.Value);

        IEnumerable<ProductProperty> characteristicsInCategory = _productPropertyService.GetAllInProduct(productId.Value);

        Assert.Empty(characteristicsInCategory);
    }

    [Fact]
    public void DeleteAllForCharacteristic_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertsAreValid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        uint? categoryId = categoryInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        uint? productId = productInsertResult.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        uint? characteristicId1 = characteristicInsertResult1.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        uint? characteristicId2 = characteristicInsertResult2.Match<uint?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.NotNull(characteristicId2);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        bool success = _productPropertyService.DeleteAllForCharacteristic(characteristicId1.Value);

        IEnumerable<ProductProperty> characteristicsInCategory = _productPropertyService.GetAllInProduct(productId.Value);


        Assert.NotEmpty(characteristicsInCategory);

        Assert.Contains(characteristicsInCategory, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.DisplayOrder
        && x.XmlPlacement == propertyCreateRequest2.XmlPlacement);
    }
}