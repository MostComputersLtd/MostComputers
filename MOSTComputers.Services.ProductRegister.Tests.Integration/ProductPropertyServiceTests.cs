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

    private const string _useRequiredNameForUpdateValue = "Use required name for update /.//,?.,";

    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    private readonly List<int> _categoriesToDeleteIds = new();
    private readonly List<int> _productCharacteristicsToDeleteIds = new();

    private OneOf<int, ValidationResult, UnexpectedFailureResult> InsertCategoryAndAddIdToDelete(ServiceCategoryCreateRequest categoryCreateRequest)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _categoryService.Insert(categoryCreateRequest);

        result.Switch(
            id =>
            {
                _categoriesToDeleteIds.Add(id);
            },
            validationResult => { },
            unexpectedFailureResult => { });

        return result;
    }

    private OneOf<int, ValidationResult, UnexpectedFailureResult> InsertProductCharacteristicAndAddIdToDelete(ProductCharacteristicCreateRequest characteristicCreateRequest)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _productCharacteristicService.Insert(characteristicCreateRequest);

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
        CustomDisplayOrder = 12,
        XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
    };

    private static ProductPropertyByCharacteristicNameCreateRequest GetValidCreateRequestByName(int productId, string characteristicName, string value = "VAL") =>
    new()
    {
        ProductId = productId,
        ProductCharacteristicName = characteristicName,
        Value = value,
        CustomDisplayOrder = 12,
        XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
    };


    [Fact]
    public void GetAllInProduct_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertsAreValid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest(categoryId.Value));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest(categoryId.Value, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest(categoryId.Value, characteristicName2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        int? characteristicId1 = characteristicInsertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        int? characteristicId2 = characteristicInsertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.True(characteristicId1 > 0);

        Assert.NotNull(characteristicId2);
        Assert.True(characteristicId2 > 0);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        var propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        var propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        IEnumerable<ProductProperty> propsInProduct = _productPropertyService.GetAllInProduct((int)productId.Value);

        Assert.True(propsInProduct.Count() >= 2);

        Assert.Contains(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId1
        && x.Characteristic == characteristicName1
        && x.Value == propertyCreateRequest1.Value
        && x.DisplayOrder == propertyCreateRequest1.CustomDisplayOrder
        && x.XmlPlacement == propertyCreateRequest1.XmlPlacement);

        Assert.Contains(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.CustomDisplayOrder
        && x.XmlPlacement == propertyCreateRequest2.XmlPlacement);
    }

    [Fact]
    public void GetAllInProduct_ShouldFail_WhenProductExistsAndInsertsAreValid_ButCharacteristicDoesNotExist()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest(categoryId.Value));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest(categoryId.Value, characteristicName1);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById(productId.Value, characteristicId.Value, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById(productId.Value, 0, propertyValue2);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        IEnumerable<ProductProperty> propsInProduct = _productPropertyService.GetAllInProduct((int)productId.Value);

        Assert.NotEmpty(propsInProduct);

        Assert.Contains(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId
        && x.Characteristic == characteristicName1
        && x.Value == propertyCreateRequest1.Value
        && x.DisplayOrder == propertyCreateRequest1.CustomDisplayOrder
        && x.XmlPlacement == propertyCreateRequest1.XmlPlacement);

        Assert.DoesNotContain(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == 0
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.CustomDisplayOrder
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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest(categoryId.Value, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest(categoryId.Value, characteristicName2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        int? characteristicId1 = characteristicInsertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        int? characteristicId2 = characteristicInsertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.True(characteristicId1 > 0);

        Assert.NotNull(characteristicId2);
        Assert.True(characteristicId2 > 0);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById(invalidProductId, characteristicId1.Value, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById(invalidProductId, characteristicId2.Value, propertyValue2);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCreateRequest productCreateRequest = GetValidProductCreateRequest(categoryId.Value);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(productCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        int? characteristicId1 = characteristicInsertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        int? characteristicId2 = characteristicInsertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.True(characteristicId1 > 0);

        Assert.NotNull(characteristicId2);
        Assert.True(characteristicId2 > 0);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, invalidPropertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, invalidPropertyValue2);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest1);
        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest2);

        IEnumerable<ProductProperty> propsInProduct = _productPropertyService.GetAllInProduct((int)productId.Value);

        if (productCreateRequest.Properties is not null)
        {
            Assert.Equal(productCreateRequest.Properties.Count, propsInProduct.Count());
        }

        Assert.DoesNotContain(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId1
        && x.Characteristic == characteristicName1
        && x.Value == invalidPropertyCreateRequest1.Value
        && x.DisplayOrder == invalidPropertyCreateRequest1.CustomDisplayOrder
        && x.XmlPlacement == invalidPropertyCreateRequest1.XmlPlacement);

        Assert.DoesNotContain(propsInProduct, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == invalidPropertyCreateRequest2.Value
        && x.DisplayOrder == invalidPropertyCreateRequest2.CustomDisplayOrder
        && x.XmlPlacement == invalidPropertyCreateRequest2.XmlPlacement);
    }

    [Fact]
    public void GetByNameAndProductId_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertIsValid()
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string propertyValue = "VAL1";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, (int)productId.Value);

        Assert.NotNull(property);

        Assert.Equal((int)productId, property.ProductId);
        Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
        Assert.Equal(characteristicName, property.Characteristic);
        Assert.Equal(propertyCreateRequest.Value, property.Value);
        Assert.Equal(propertyCreateRequest.CustomDisplayOrder, property.DisplayOrder);
        Assert.Equal(propertyCreateRequest.XmlPlacement, property.XmlPlacement);
    }

    [Fact]
    public void GetByNameAndProductId_ShouldFail_WhenProductExists_ButCharacteristicDoesNotExist()
    {
        const string characteristicName = "NAMEOFCHAR1";
        const string propertyValue = "VAL1";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        if (createRequest.ProductId == UseRequiredValuePlaceholder)
        {
            createRequest.ProductId = (int)productId;
        }

        if (createRequest.ProductCharacteristicId == UseRequiredValuePlaceholder)
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
            Assert.Equal(createRequest.CustomDisplayOrder ?? characteristicCreateRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(createRequest.XmlPlacement, property.XmlPlacement);
        }
    }

    public static readonly TheoryData<ProductPropertyByCharacteristicIdCreateRequest, bool> InsertWithCharacteristicId_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            GetValidCreateRequestById(UseRequiredValuePlaceholder, UseRequiredValuePlaceholder, "VAL_UPDATED"),
            true
        },

        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPDATED",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPDATED",
                CustomDisplayOrder = -12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "",
                CustomDisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "                ",
                CustomDisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "Longer than 200 characters: SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" +
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" +
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" +
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" +
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" +
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" +
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS",
                CustomDisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = 0,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPDATED",
                CustomDisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicIdCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = 0,
                Value = "VAL_UPDATED",
                CustomDisplayOrder = 12,
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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        if (createRequest.ProductId == UseRequiredValuePlaceholder)
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

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, (int)productId.Value);

        Assert.Equal(expected, property is not null);

        if (expected)
        {
            Assert.NotNull(property);

            Assert.Equal((int)productId, property.ProductId);
            Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
            Assert.Equal(characteristicName, property.Characteristic);
            Assert.Equal(createRequest.Value, property.Value);
            Assert.Equal(createRequest.CustomDisplayOrder ?? characteristicCreateRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(createRequest.XmlPlacement, property.XmlPlacement);
        }
    }

    public static readonly TheoryData<ProductPropertyByCharacteristicNameCreateRequest, bool> InsertWithCharacteristicName_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            GetValidCreateRequestByName(UseRequiredValuePlaceholder, _useRequiredNameForUpdateValue, "VAL_UPDATED"),
            true
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "VAL_UPD",
                CustomDisplayOrder = 0,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "VAL_UPD",
                CustomDisplayOrder = -12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "VAL_UPD",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = "Memory size",
                Value = "VAL_UPD",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },


        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = string.Empty,
                Value = "VAL_UPD",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = "       ",
                Value = "VAL_UPD",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = string.Empty,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "     ",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyByCharacteristicNameCreateRequest()
            {
                ProductId = 0,
                ProductCharacteristicName = _useRequiredNameForUpdateValue,
                Value = "     ",
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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductPropertyByCharacteristicIdCreateRequest createRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(createRequest);

        Assert.True(propertyInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        if (updateRequest.ProductId == UseRequiredValuePlaceholder)
        {
            updateRequest.ProductId = (int)productId;
        }

        if (updateRequest.ProductCharacteristicId == UseRequiredValuePlaceholder)
        {
            updateRequest.ProductCharacteristicId = (int)characteristicId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyUpdateResult = _productPropertyService.Update(updateRequest);

        Assert.Equal(expected, propertyUpdateResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, (int)productId.Value);

        Assert.NotNull(property);

        Assert.Equal((int)productId, property.ProductId);
        Assert.Equal((int)characteristicId, property.ProductCharacteristicId);
        Assert.Equal(characteristicName, property.Characteristic);

        if (expected)
        {
            Assert.Equal(updateRequest.Value, property.Value);
            Assert.Equal(updateRequest.CustomDisplayOrder ?? characteristicCreateRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(updateRequest.XmlPlacement, property.XmlPlacement);
        }
        else
        {
            Assert.Equal(createRequest.Value, property.Value);
            Assert.Equal(createRequest.CustomDisplayOrder ?? characteristicCreateRequest.DisplayOrder, property.DisplayOrder);
            Assert.Equal(createRequest.XmlPlacement, property.XmlPlacement);
        }
    }

    public static readonly TheoryData<ProductPropertyUpdateRequest, bool> Update_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPDATED",
                CustomDisplayOrder = 12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPD",
                CustomDisplayOrder = 0,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPD",
                CustomDisplayOrder = -12,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPD",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            true
        },

        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = 0,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "VAL_UPD",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = 0,
                Value = "VAL_UPD",
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = string.Empty,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            },

            false
        },

        {
            new ProductPropertyUpdateRequest()
            {
                ProductId = UseRequiredValuePlaceholder,
                ProductCharacteristicId = UseRequiredValuePlaceholder,
                Value = "      ",
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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        bool success = _productPropertyService.Delete((int)productId.Value, characteristicId.Value);

        Assert.True(success);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, (int)productId.Value);

        Assert.Null(property);
    }

    [Fact]
    public void Delete_ShouldFail_WhenCharacteristicExists_ButProductDoesNotExist()
    {
        const string characteristicName = "NAMEOFCHAR1_DELETED";
        const string propertyValue = "VAL1";
        const int invalidProductId = 0;

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById((int)productId, invalidCharacteristicId, propertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        bool success = _productPropertyService.Delete((int)productId.Value, invalidCharacteristicId);

        Assert.False(success);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, (int)productId.Value);

        Assert.Null(property);
    }

    [Fact]
    public void Delete_ShouldFail_WhenCharacteristicAndProductExist_ButInsertIsInvalid()
    {
        const string characteristicName = "NAMEOFCHAR1_DELETED";
        const string invalidPropertyValue = "";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest);

        int? characteristicId = characteristicInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId);
        Assert.True(characteristicId > 0);

        ProductPropertyByCharacteristicIdCreateRequest invalidPropertyCreateRequest = GetValidCreateRequestById((int)productId, (int)characteristicId, invalidPropertyValue);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult = _productPropertyService.InsertWithCharacteristicId(invalidPropertyCreateRequest);

        Assert.True(propertyInsertResult.Match(
            success => false,
            validationResult => true,
            unexpectedFailureResult => false));

        bool success = _productPropertyService.Delete((int)productId.Value, characteristicId.Value);

        Assert.False(success);

        ProductProperty? property = _productPropertyService.GetByNameAndProductId(characteristicName, (int)productId.Value);

        Assert.Null(property);
    }

    [Fact]
    public void DeleteAllForProduct_ShouldSucceed_WhenCharacteristicAndProductExistAndInsertsAreValid()
    {
        const string characteristicName1 = "NAMEOFCHAR1";
        const string characteristicName2 = "NAMEOFCHAR2";
        const string propertyValue1 = "VAL1";
        const string propertyValue2 = "VAL2";

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        int? characteristicId1 = characteristicInsertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        int? characteristicId2 = characteristicInsertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.True(characteristicId1 > 0);

        Assert.NotNull(characteristicId2);
        Assert.True(characteristicId2 > 0);

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

        OneOf<int, ValidationResult, UnexpectedFailureResult> categoryInsertResult = InsertCategoryAndAddIdToDelete(ValidCategoryCreateRequest);

        int? categoryId = categoryInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(categoryId);
        Assert.True(categoryId > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(GetValidProductCreateRequest((int)categoryId));

        int? productId = productInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ProductCharacteristicCreateRequest characteristicCreateRequest1 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName1);
        ProductCharacteristicCreateRequest characteristicCreateRequest2 = GetValidCharacteristicCreateRequest((int)categoryId, characteristicName2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult1 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> characteristicInsertResult2 = InsertProductCharacteristicAndAddIdToDelete(characteristicCreateRequest2);

        int? characteristicId1 = characteristicInsertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        int? characteristicId2 = characteristicInsertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(characteristicId1);
        Assert.True(characteristicId1 > 0);

        Assert.NotNull(characteristicId2);
        Assert.True(characteristicId2 > 0);

        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest1 = GetValidCreateRequestById((int)productId, (int)characteristicId1, propertyValue1);
        ProductPropertyByCharacteristicIdCreateRequest propertyCreateRequest2 = GetValidCreateRequestById((int)productId, (int)characteristicId2, propertyValue2);

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult1 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest1);
        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyInsertResult2 = _productPropertyService.InsertWithCharacteristicId(propertyCreateRequest2);

        bool success = _productPropertyService.DeleteAllForCharacteristic(characteristicId1.Value);

        IEnumerable<ProductProperty> characteristicsInCategory = _productPropertyService.GetAllInProduct((int)productId.Value);


        Assert.NotEmpty(characteristicsInCategory);

        Assert.Contains(characteristicsInCategory, x =>
        x.ProductId == productId
        && x.ProductCharacteristicId == characteristicId2
        && x.Characteristic == characteristicName2
        && x.Value == propertyCreateRequest2.Value
        && x.DisplayOrder == propertyCreateRequest2.CustomDisplayOrder
        && x.XmlPlacement == propertyCreateRequest2.XmlPlacement);
    }
}