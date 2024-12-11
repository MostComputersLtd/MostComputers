using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;
using static MOSTComputers.Utils.ProductImageFileNameUtils.ProductImageFileNameUtils;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.SuccessfulInsertAbstractions;
using MOSTComputers.Services.DAL.Models.Requests.ProductStatuses;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductServiceTests : IntegrationTestBaseForNonWebProjectsWithDBReset
{
    public ProductServiceTests(
        IProductService productService,
        IProductCharacteristicService productCharacteristicService,
        IProductPropertyService productPropertyService,
        IProductStatusesService productStatusesService,
        IProductWorkStatusesService productWorkStatusesService,
        IProductImageFileManagementService productImageFileManagementService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _productService = productService;
        _productCharacteristicService = productCharacteristicService;
        _productPropertyService = productPropertyService;
        _productStatusesService = productStatusesService;
        _productWorkStatusesService = productWorkStatusesService;
        _productImageFileManagementService = productImageFileManagementService;
    }

    private readonly IProductService _productService;
    private readonly IProductCharacteristicService _productCharacteristicService;

#pragma warning disable IDE0052 // Remove unread private members
    private readonly IProductPropertyService _productPropertyService;
#pragma warning restore IDE0052 // Remove unread private members

    private readonly IProductStatusesService _productStatusesService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;

    private readonly IProductImageFileManagementService _productImageFileManagementService;

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();

        string[] filePaths = Directory.GetFiles(Startup.ImageDirectoryFullPath);

        foreach (string filePath in filePaths)
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void GetAllWithoutImagesAndProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<Product> allProducts = _productService.GetAllWithoutImagesAndProps();

        Product product1 = allProducts.Single(x => x.Id == productId1);
        Product product2 = allProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereSearchStringMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<Product> allProducts = _productService.GetAllWhereSearchStringMatches(validCreateRequest.SearchString!);

        Product product1 = allProducts.Single(x => x.Id == productId1);
        Product product2 = allProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereNameMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        IEnumerable<Product> allProducts = _productService.GetAllWhereNameMatches(validCreateRequest.Name!);

        Product product1 = allProducts.Single(x => x.Id == productId1);
        Product product2 = allProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
    }

    [Fact]
    public void GetAllWithoutImagesAndProps_ShouldOnlyGetTheDataThatWasSuccessfullyInserted()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        ProductCreateRequest invalidCreateRequest = GetValidProductCreateRequest();

        invalidCreateRequest.Name = "  ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(invalidCreateRequest);

        IEnumerable<Product> allProducts = _productService.GetAllWithoutImagesAndProps();

        Product product1 = allProducts.Single(x => x.Id == productId1);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.DoesNotContain(allProducts, x =>
            CompareProductAndRequestWithoutPropsOrImages(x, invalidCreateRequest));
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereSearchStringMatches_ShouldOnlyGetTheDataThatWasSuccessfullyInserted()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        ProductCreateRequest invalidCreateRequest = GetValidProductCreateRequest();

        invalidCreateRequest.Name = "  ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(invalidCreateRequest);

        Assert.True(insertResult2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        IEnumerable<Product> allProducts = _productService.GetAllWhereSearchStringMatches(validCreateRequest.SearchString!);

        Product product1 = allProducts.Single(x => x.Id == productId1);

        Assert.Equal(productId1, product1.Id);

        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.DoesNotContain(allProducts, x =>
            CompareProductAndRequestWithoutPropsOrImages(x, invalidCreateRequest));
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereNameMatches_ShouldOnlyGetTheDataThatWasSuccessfullyInserted()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        ProductCreateRequest invalidCreateRequest = GetValidProductCreateRequest();

        invalidCreateRequest.Name = "  ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(invalidCreateRequest);

        Assert.True(insertResult2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        IEnumerable<Product> allProducts = _productService.GetAllWhereNameMatches(validCreateRequest.Name!);

        Product product1 = allProducts.Single(x => x.Id == productId1);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.DoesNotContain(allProducts, x =>
            CompareProductAndRequestWithoutPropsOrImages(x, invalidCreateRequest));
    }

    [Fact]
    public void GetSelectionWithoutImagesAndProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        List<int> productIds = new() { productId1.Value, productId2.Value };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithoutImagesAndProps(productIds);

        Assert.Equal(2, insertedProducts.Count());

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
    }

    [Fact]
    public void GetSelectionWithoutImagesAndProps_ShouldOnlyGetProductsWithValidIds()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        List<int> productIds = new() { productId1.Value, productId2.Value, 0 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithoutImagesAndProps(productIds);

        Assert.Equal(2, insertedProducts.Count());

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
    }

    [Fact]
    public void GetSelectionWithFirstImage_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        List<int> productIds = new() { productId1.Value, productId2.Value };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithFirstImage(productIds);

        Assert.Equal(2, insertedProducts.Count());

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        List<CurrentProductImageCreateRequest> firstImageInRequest = new() { validCreateRequest.Images!.First() };

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product1.Images));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product2.Images));
    }

    [Fact]
    public void GetSelectionWithFirstImage_ShouldOnlyGetProductsWithValidIds()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        List<int> productIds = new() { productId1.Value, productId2.Value, 0 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithFirstImage(productIds);

        Assert.Equal(2, insertedProducts.Count());

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        List<CurrentProductImageCreateRequest> firstImageInRequest = new() { validCreateRequest.Images!.First() };

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product1.Images));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product2.Images));
    }

    [Fact]
    public void GetSelectionWithProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        List<int> productIds = new() { productId1.Value, productId2.Value };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithProps(productIds);

        Assert.Equal(2, insertedProducts.Count());

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product1.Properties));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product2.Properties));
    }

    [Fact]
    public void GetSelectionWithProps_ShouldOnlyGetProductsWithValidIds()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        int? productId1 = insertResult1.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        int? productId2 = insertResult2.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);
        
        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        List<int> productIds = new() { productId1.Value, productId2.Value, 0 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithProps(productIds);

        Assert.Equal(2, insertedProducts.Count());

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product1.Properties));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product2.Properties));
    }

    [Fact]
    public void GetFirstItemsBetweenStartAndEnd_ShouldSucceed_WhenInsertsAreValid()
    {
        List<int> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.DisplayOrder = i + 1;

            OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            int? productId = insertResult.Match<int?>(
                id => id,
                validationResult => null,
                unexpectedFailureResult => null);

            Assert.NotNull(productId);
            Assert.True(productId > 0);

            productIds.Add(productId.Value);
        }

        List<Product> allProductsRanged = _productService.GetAllWithoutImagesAndProps()
            .Skip(10)
            .Take(10)
            .ToList();

        ProductRangeSearchRequest rangeSearchRequest = new() { Start = 10, Length = 10 };

        List<Product> productsInRange = _productService.GetFirstItemsBetweenStartAndEnd(rangeSearchRequest)
            .ToList();

        Assert.True(productsInRange.Count >= 2);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }
    }

    [Fact]
    public void GetFirstInRangeWhereSearchStringMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        const string searchStringOfData = "hic opriwopirvoprjvopi og otioiok/df3243";

        List<int> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.SearchString = searchStringOfData;

            OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            int? productId = insertResult.Match<int?>(
                id => id,
                validationResult => null,
                unexpectedFailureResult => null);

            Assert.NotNull(productId);
            Assert.True(productId > 0);

            productIds.Add(productId.Value);
        }

        List<Product> allProductsRanged = _productService.GetAllWithoutImagesAndProps()
            .Where(x => x.SearchString == searchStringOfData)
            .Skip(10)
            .Take(10)
            .ToList();

        List<Product> productsInRange = _productService.GetFirstInRangeWhereSearchStringMatches(new() { Start = 10, Length = 10 }, searchStringOfData)
            .ToList();

        Assert.Equal(10, productsInRange.Count);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }
    }

    [Fact]
    public void GetFirstInRangeWhereNameMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        const string nameOfData = "hic opriwopirvoppi og /df3243";

        List<int> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.Name = nameOfData;

            OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            int? productId = insertResult.Match<int?>(
                id => id,
                validationResult => null,
                unexpectedFailureResult => null);

            Assert.NotNull(productId);
            Assert.True(productId > 0);

            productIds.Add(productId.Value);
        }

        List<Product> allProductsRanged = _productService.GetAllWithoutImagesAndProps()
            .Where(x => x.Name == nameOfData)
            .Skip(10)
            .Take(10)
            .ToList();

        List<Product> productsInRange = _productService.GetFirstInRangeWhereNameMatches(new() { Start = 10, Length = 10 }, nameOfData).ToList();

        Assert.Equal(10, productsInRange.Count);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }
    }

    [Fact]
    public void GetFirstInRangeWhereAllConditionsAreMet_ShouldSucceed_WhenInsertsAreValid_AndAllConditionsExceptForTheStatusesTableAndCategoryAreUsed()
    {
        const string nameOfData = "hic opriwopirvoppi og /df3243";
        const string searchStringOfData = "hic opriwopirvoprjsdsvopi og otioiok/df3243";
        const ProductStatusEnum statusOfData = ProductStatusEnum.Available;

        const string nameOfProductThatDoesntFollowCondition = "not the name in the condition";

        List<int> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.Name = nameOfData;
            validCreateRequest.SearchString = searchStringOfData;
            validCreateRequest.Status = statusOfData;

            OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            int? productId = insertResult.Match<int?>(
                id => id,
                validationResult => null,
                unexpectedFailureResult => null);

            Assert.NotNull(productId);

            productIds.Add(productId.Value);
        }

        ProductCreateRequest validCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        validCreateRequest2.Name = nameOfProductThatDoesntFollowCondition;
        validCreateRequest2.Status = statusOfData;

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResultForProductThatDoesntMatchConditions = _productService.Insert(validCreateRequest2);

        int? productIdForProductThatDoesntMatchConditions = insertResultForProductThatDoesntMatchConditions.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdForProductThatDoesntMatchConditions);
        Assert.True(productIdForProductThatDoesntMatchConditions > 0);

        productIds.Add(productIdForProductThatDoesntMatchConditions.Value);

        List<Product> allProductsRanged = _productService.GetAllWithoutImagesAndProps()
            .Where(x =>
            {
                return (x.Name == nameOfData
                    && x.SearchString == searchStringOfData
                    && x.Status == statusOfData);
            })
            .Skip(10)
            .Take(10)
            .ToList();

        ProductConditionalSearchRequest productConditionalSearchRequest = new()
        {
            NameSubstring = nameOfData,
            SearchStringSubstring = searchStringOfData,
            Status = statusOfData,
        };

        List<Product> productsInRange = _productService.GetFirstInRangeWhereAllConditionsAreMet(new() { Start = 10, Length = 10 }, productConditionalSearchRequest)
            .ToList();

        Assert.Equal(10, productsInRange.Count);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }
    }

    [Fact]
    public void GetFirstInRangeWhereAllConditionsAreMet_ShouldSucceed_WhenInsertsAreValid_AndAllConditionsExceptCategoryAreUsed()
    {
        const string nameOfData = "hic opriwopirvoppi og /df3243";
        const string searchStringOfData = "hic opri3rcrwopirvoprjvopi og iok/df3243";
        const ProductStatusEnum statusOfData = ProductStatusEnum.Available;
        const string nameOfProductThatDoesntFollowCondition = "not the name in the condition";
        const bool isProcessedForData = false;
        const bool needsToBeUpdatedForData = true;

        List<int> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.Name = nameOfData;
            validCreateRequest.SearchString = searchStringOfData;
            validCreateRequest.Status = statusOfData;

            OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            int? productId = insertResult.Match<int?>(
                id => id,
                validationResult => null,
                unexpectedFailureResult => null);

            Assert.NotNull(productId);
            Assert.True(productId > 0);

            ProductStatusesCreateRequest productStatusesCreateRequest = new()
            {
                ProductId = (int)productId,
                IsProcessed = isProcessedForData,
                NeedsToBeUpdated = needsToBeUpdatedForData
            };

            OneOf<Success, ValidationResult> productStatusesInsertResult = _productStatusesService.InsertIfItDoesntExist(productStatusesCreateRequest);

            Assert.True(productStatusesInsertResult.Match(
                success => true,
                unexpectedFailureResult => false));

            productIds.Add(productId.Value);
        }

        ProductCreateRequest validCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        validCreateRequest2.Name = nameOfProductThatDoesntFollowCondition;
        validCreateRequest2.Status = statusOfData;

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResultForProductThatDoesntMatchConditions = _productService.Insert(validCreateRequest2);

        int? productIdForProductThatDoesntMatchConditions = insertResultForProductThatDoesntMatchConditions.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productIdForProductThatDoesntMatchConditions);
        Assert.True(productIdForProductThatDoesntMatchConditions > 0);

        productIds.Add(productIdForProductThatDoesntMatchConditions.Value);

        List<Product> allProductsRanged = _productService.GetAllWithoutImagesAndProps()
            .Where(x =>
            {
                return (x.Name == nameOfData
                    && x.SearchString == searchStringOfData
                    && x.Status == statusOfData);
            })
            .Skip(10)
            .Take(10)
            .ToList();

        ProductConditionalSearchRequest productConditionalSearchRequest = new()
        {
            NameSubstring = nameOfData,
            SearchStringSubstring = searchStringOfData,
            Status = statusOfData,
            IsProcessed = isProcessedForData,
            NeedsToBeUpdated = needsToBeUpdatedForData
        };

        List<Product> productsInRange = _productService.GetFirstInRangeWhereAllConditionsAreMet(new() { Start = 10, Length = 10 }, productConditionalSearchRequest)
            .ToList();

        Assert.Equal(10, productsInRange.Count);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }
    }

    [Fact]
    public void GetByIdWithFirstImage_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        Product? insertedProduct = _productService.GetByIdWithFirstImage(productId.Value);

        Assert.NotNull(insertedProduct);

        List<CurrentProductImageCreateRequest> firstImageInRequest = new() { validCreateRequest.Images!.First() };

        Assert.Equal((int)productId, insertedProduct.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, insertedProduct.Images));
    }

    [Fact]
    public void GetByIdWithFirstImage_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        Product? insertedProduct = _productService.GetByIdWithFirstImage(0);

        Assert.Null(insertedProduct);
    }

    [Fact]
    public void GetByIdWithProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        Product? insertedProduct = _productService.GetByIdWithProps(productId.Value);

        Assert.NotNull(insertedProduct);

        Assert.Equal((int)productId, insertedProduct.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, insertedProduct.Properties));
    }

    [Fact]
    public void GetByIdWithProps_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        Product? insertedProduct = _productService.GetByIdWithProps(0);

        Assert.Null(insertedProduct);
    }

    [Fact]
    public void GetByIdWithImages_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        Product? insertedProduct = _productService.GetByIdWithImages(productId.Value);

        Assert.NotNull(insertedProduct);

        Assert.Equal((int)productId, insertedProduct.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(validCreateRequest.Images, insertedProduct.Images));
    }

    [Fact]
    public void GetByIdWithImages_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        Product? insertedProduct = _productService.GetByIdWithImages(0);

        Assert.Null(insertedProduct);
    }

    [Fact]
    public void GetProductFull_ShouldSucceedToGetWithSameId_WhenProductExists()
    {
        ProductCreateRequest validProductCreateRequest = ValidProductCreateRequest;

        int productId = InsertProductAndGetIdOrThrow(_productService, validProductCreateRequest);

        Product? insertedProduct = _productService.GetProductFull(productId);

        Assert.NotNull(insertedProduct);

        Assert.True(CompareProductAndRequestWithoutPropsOrImages(insertedProduct, validProductCreateRequest));

        Assert.True(ComparePropertiesInRequestAndProduct(validProductCreateRequest.Properties, insertedProduct.Properties));
        Assert.True(CompareImagesInRequestAndProduct(validProductCreateRequest.Images, insertedProduct.Images));
        Assert.True(CompareImageFileNamesInRequestAndProduct(validProductCreateRequest.ImageFileNames, insertedProduct.ImageFileNames));
    }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
    [Fact]
    public void GetProductFull_ShouldFailToGetWithSameId_WhenProductWithIdDoesntExist()
    {
        ProductCreateRequest validProductCreateRequest = ValidProductCreateRequest;

        int productId = InsertProductAndGetIdOrThrow(_productService, validProductCreateRequest);

        Product? insertedProduct = _productService.GetProductFull(0);

        Assert.Null(insertedProduct);
    }
#pragma warning restore IDE0059 // Unnecessary assignment of a value

    [Fact]
    public void GetProductWithHighestId_ShouldSucceedToGetWithHighestId_WhenProductsExist()
    {
        ProductCreateRequest validProductCreateRequest = ValidProductCreateRequest;

        int _ = InsertProductAndGetIdOrThrow(_productService, validProductCreateRequest);

        Product? productWithHighestId = _productService.GetProductWithHighestId();

        Assert.NotNull(productWithHighestId);
         
        Assert.True(CompareProductAndRequestWithoutPropsOrImages(productWithHighestId, validProductCreateRequest));

        Assert.True(ComparePropertiesInRequestAndProduct(validProductCreateRequest.Properties, productWithHighestId.Properties));
        Assert.True(CompareImagesInRequestAndProduct(validProductCreateRequest.Images, productWithHighestId.Images));
        Assert.True(CompareImageFileNamesInRequestAndProduct(validProductCreateRequest.ImageFileNames, productWithHighestId.ImageFileNames));
    }

    [Fact]
    public void GetProductWithHighestId_ShouldFailToGetWithHighestId_WhenNoProductsExist()
    {
        Product? productWithHighestId = _productService.GetProductWithHighestId();

        Assert.Null(productWithHighestId);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ProductCreateRequest createRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(createRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.Equal(expected, productId is not null);

        if (expected)
        {
            Product? insertedProduct = _productService.GetProductFull(productId!.Value);

            Assert.NotNull(insertedProduct);

            AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, createRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(createRequest.Properties, insertedProduct.Properties));
            Assert.True(CompareImagesInRequestAndProduct(createRequest.Images, insertedProduct.Images));
            Assert.True(CompareImageFileNamesInRequestAndProduct(createRequest.ImageFileNames, insertedProduct.ImageFileNames));
        }
    }

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static TheoryData<ProductCreateRequest, bool> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            ValidProductCreateRequest,
            true
        },

        {
            GetValidProductCreateRequestUsingRandomData(),
            true
        },

        {
            new ProductCreateRequest()
            {
                Name = string.Empty,
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = null,
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },
                Images = new List<CurrentProductImageCreateRequest>()
                {
                },
                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1, Active = true },
                    new() { FileName = "20144.png", DisplayOrder = 2, Active = false }
                },

                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },

        {
            new ProductCreateRequest()
            {
                Name = "    ",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = null,
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                Images = new List<CurrentProductImageCreateRequest>()
                {
                },

                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1, Active = true },
                    new() { FileName = "20144.png", DisplayOrder = 2, Active = false }
                },

                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },

        {
            new ProductCreateRequest()
            {
                Name = "Product name",
                AdditionalWarrantyPrice = -1M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = null,
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                Images = new List<CurrentProductImageCreateRequest>()
                {
                },

                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1, Active = true },
                    new() { FileName = "20144.png", DisplayOrder = 2, Active = false }
                },

                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },

        {
            new ProductCreateRequest()
            {
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = -1,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = null,
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },
                Images = new List<CurrentProductImageCreateRequest>()
                {
                },
                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1, Active = true },
                    new() { FileName = "20144.png", DisplayOrder = 2, Active = true }
                },

                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },
    };

    [Theory]
    [MemberData(nameof(InsertWithImagesOnlyInDirectoryAsync_ShouldSucceedOrFail_WhenExpected_Data))]
    public async Task InsertWithImagesOnlyInDirectoryAsync_ShouldSucceedOrFail_WhenExpectedAsync(
        ProductCreateWithoutImagesInDatabaseRequest createRequest, bool expected)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> insertResult
            = await _productService.InsertWithImagesOnlyInDirectoryAsync(createRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null,
            directoryNotFoundResult => null,
            fileDoesntExistResult => null);

        Assert.Equal(expected, productId is not null);

        if (expected)
        {
            Product? insertedProduct = _productService.GetProductFull(productId!.Value);

            Assert.NotNull(insertedProduct);

            AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, createRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(createRequest.Properties, insertedProduct.Properties));

            Assert.True(insertedProduct.Images is null || insertedProduct.Images.Count == 0);

            Assert.True(CompareImageFileNamesAndImageFilesInRequestAndProduct(
                productId.Value, createRequest.ImageFileAndFileNameInfoUpsertRequests, insertedProduct.ImageFileNames));
        }
    }

    public static TheoryData<ProductCreateWithoutImagesInDatabaseRequest, bool> InsertWithImagesOnlyInDirectoryAsync_ShouldSucceedOrFail_WhenExpected_Data = new()
    {
        {
            GetValidProductCreateWithoutImagesInDatabaseRequest(),
            true
        },

        {
            new ProductCreateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = null,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = null,
                        Active = false,
                    },
                }
            },
            true
        },

        {
            GetValidProductCreateWithoutImagesInDatabaseRequest(categoryId: -2),
            false
        },

        {
            GetValidProductCreateWithoutImagesInDatabaseRequest(manifacturerId: 0),
            false
        },

        {
            new ProductCreateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = -1, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 3,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 2,
                        Active = false,
                    },
                }
            },
            false
        },

        {
            new ProductCreateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 2,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 3,
                        Active = false,
                    },
                }
            },
            false
        },

        {
            new ProductCreateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, Value = string.Empty, XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "    ", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 2,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 3,
                        Active = false,
                    },
                }
            },
            false
        },

        {
            new ProductCreateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 0,
                        Active = false,
                    },
                }
            },
            false
        },

        {
            new ProductCreateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                Properties = new()
                {
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = -1,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = 33782,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 2,
                        Active = false,
                    },
                }
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateProductAndUpdateImagesOnlyInDirectoryAsync_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public async Task UpdateProductAndUpdateImagesOnlyInDirectoryAsync_ShouldSucceedOrFail_InAnExpectedMannerAsync(
        ProductUpdateWithoutImagesInDatabaseRequest updateRequest, bool expected)
    {
        ProductCreateRequest validProductCreateRequest = ValidProductCreateRequest;

        int productId = InsertProductAndGetIdOrThrow(_productService, validProductCreateRequest);

        ProductWorkStatusesCreateRequest productStatusesCreateRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = ProductNewStatusEnum.ReadyForUse,
            ProductXmlStatus = ProductXmlStatusEnum.ReadyForUse,
            ReadyForImageInsert = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductWorkStatusesResult
            = _productWorkStatusesService.InsertIfItDoesntExist(productStatusesCreateRequest);

        if (updateRequest.Id == UseRequiredValuePlaceholder)
        {
            updateRequest.Id = productId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult, DirectoryNotFoundResult, FileDoesntExistResult> updateResult
            = await _productService.UpdateProductAndUpdateImagesOnlyInDirectoryAsync(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false,
            directoryNotFoundResult => false,
            fileDoesntExistResult => false));

        Product? updatedProduct = _productService.GetProductFull(productId);

        Assert.NotNull(updatedProduct);

        if (expected)
        {
            AssertProductIsEqualToRequestWithoutPropsOrImages(updatedProduct, updateRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(updateRequest.PropertyUpsertRequests, updatedProduct.Properties));

            Assert.True(CompareImagesInRequestAndProduct(validProductCreateRequest.Images, updatedProduct.Images));

            Assert.True(CompareImageFileNamesAndImageFilesInRequestAndProduct(
                productId, updateRequest.ImageFileAndFileNameInfoUpsertRequests, updatedProduct.ImageFileNames));
        }
        else
        {
            AssertProductIsEqualToRequestWithoutPropsOrImages(updatedProduct, validProductCreateRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(validProductCreateRequest.Properties, updatedProduct.Properties));
            Assert.True(CompareImagesInRequestAndProduct(validProductCreateRequest.Images, updatedProduct.Images));
            Assert.True(CompareImageFileNamesInRequestAndProduct(validProductCreateRequest.ImageFileNames, updatedProduct.ImageFileNames));
        }
    }

    public static TheoryData<ProductUpdateWithoutImagesInDatabaseRequest, bool> UpdateProductAndUpdateImagesOnlyInDirectoryAsync_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            GetValidProductUpdateWithoutImagesInDatabaseRequest(productId: UseRequiredValuePlaceholder),
            true
        },

        {
            new ProductUpdateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                PropertyUpsertRequests = new()
                {
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = null,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = null,
                        Active = false,
                    },
                }
            },
            true
        },

        {
            GetValidProductUpdateWithoutImagesInDatabaseRequest(productId: UseRequiredValuePlaceholder, categoryId: -2),
            false
        },

        {
            GetValidProductUpdateWithoutImagesInDatabaseRequest(productId: UseRequiredValuePlaceholder, manifacturerId: 0),
            false
        },

        {
            new ProductUpdateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                PropertyUpsertRequests = new()
                {
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = -1, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 3,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 2,
                        Active = false,
                    },
                }
            },
            false
        },

        {
            new ProductUpdateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                PropertyUpsertRequests = new()
                {
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 129, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 2,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 3,
                        Active = false,
                    },
                }
            },
            false
        },

         {
            new ProductUpdateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                PropertyUpsertRequests = new()
                {
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 404, Value = string.Empty, XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "    ", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 2,
                        Active = false,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 3,
                        Active = false,
                    },
                }
            },
            false
        },

        {
            new ProductUpdateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                PropertyUpsertRequests = new()
                {
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = null,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 0,
                        Active = false,
                    },
                }
            },
            false
        },

        {
            new ProductUpdateWithoutImagesInDatabaseRequest()
            {
                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = "dddddddd",
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",
                PropertyUpsertRequests = new()
                {
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                ImageFileAndFileNameInfoUpsertRequests = new()
                {
                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = -1,
                        CustomFileNameWithoutExtension = null,
                        DisplayOrder = 1,
                        Active = true,
                    },

                    new ImageFileAndFileNameInfoUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,
                        OldFileName = null,
                        RelatedImageId = 33782,
                        CustomFileNameWithoutExtension = "custom_file_name",
                        DisplayOrder = 2,
                        Active = false,
                    },
                }
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(UpdateProductFull_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public async Task UpdateProductFull_ShouldSucceedOrFail_InAnExpectedMannerAsync(ProductFullUpdateRequest updateRequest, bool expected)
    {
        ProductCreateRequest createRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(createRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        if (updateRequest.Id == UseRequiredValuePlaceholder)
        {
            updateRequest.Id = productId.Value;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult
            = await _productService.UpdateProductFullAsync(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            id => true,
            validationResult => false,
            unexpectedFailureResult => false));

        Product? updatedProduct = _productService.GetProductFull(productId.Value);

        Assert.NotNull(updatedProduct);

        if (expected)
        {
            AssertProductIsEqualToRequestWithoutPropsOrImages(updatedProduct, updateRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(updateRequest.PropertyUpsertRequests, updatedProduct.Properties));
            Assert.True(CompareImagesInRequestAndProduct(updateRequest.ImageAndFileNameUpsertRequests, updatedProduct.Images));
            Assert.True(CompareUpdateRequestDataWithActualUpdatedData(
                productId.Value, createRequest.ImageFileNames, updateRequest.ImageAndFileNameUpsertRequests, updatedProduct.ImageFileNames));
        }
        else
        {
            AssertProductIsEqualToRequestWithoutPropsOrImages(updatedProduct, createRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(createRequest.Properties, updatedProduct.Properties));
            Assert.True(CompareImagesInRequestAndProduct(createRequest.Images, updatedProduct.Images));
            Assert.True(CompareImageFileNamesInRequestAndProduct(createRequest.ImageFileNames, updatedProduct.ImageFileNames));
        }
    }

    public static TheoryData<ProductFullUpdateRequest, bool> UpdateProductFull_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        {
            new ProductFullUpdateRequest()
            {
                Id = UseRequiredValuePlaceholder,
                Name = "Product name",
                AdditionalWarrantyPrice = 3.00M,
                AdditionalWarrantyTermMonths = 36,
                StandardWarrantyPrice = "0.00",
                StandardWarrantyTermMonths = 36,
                DisplayOrder = 12324,
                Status = ProductStatusEnum.Call,
                PlShow = 0,
                Price1 = 123.4M,
                DisplayPrice = 123.99M,
                Price3 = 122.5M,
                Currency = CurrencyEnum.EUR,
                RowGuid = Guid.NewGuid(),
                PromotionId = null,
                PromRid = null,
                PromotionPictureId = null,
                PromotionExpireDate = null,
                AlertPictureId = null,
                AlertExpireDate = null,
                PriceListDescription = null,
                PartNumber1 = "DF FKD@$ 343432 wdwfc",
                PartNumber2 = "123123/DD",
                SearchString = "SKDJK DNKMWKE DS256 34563 SAMSON",

                PropertyUpsertRequests = new()
                {
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 404, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 405, CustomDisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new LocalProductPropertyUpsertRequest() { ProductCharacteristicId = 406, CustomDisplayOrder = -16, Value = "DDS256", XmlPlacement = XMLPlacementEnum.AtTheTop },
                },
                ImageAndFileNameUpsertRequests = new()
                {
                    new ImageAndImageFileNameUpsertRequest()
                    {
                        ImageContentType = "image/png",
                        ImageData = LocalTestImageData,

                        ProductImageUpsertRequest = new()
                        {
                            OriginalImageId = null,
                            HtmlData = null,
                        },

                        ProductImageFileNameInfoUpsertRequest = new()
                        {
                            OriginalImageNumber = null,
                            NewDisplayOrder = 1,
                            Active = true,
                        }
                    },
                },

                CategoryId = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },
            true
        },


    };
#pragma warning restore CA2211 // Non-constant fields should not be visible

    [Fact]
    public void Delete_ShouldSucceed_WhenInsertIsValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        bool? success = _productService.Delete(productId.Value);

        Product? insertedProduct = _productService.GetByIdWithFirstImage(productId.Value);

        Assert.Null(insertedProduct);
    }

    [Fact]
    public void Delete_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        int? productId = insertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        bool? success = _productService.Delete(0);

        Product? insertedProduct = _productService.GetByIdWithFirstImage(productId.Value);

        Assert.NotNull(insertedProduct);
    }

    private static void AssertProductIsEqualToRequestWithoutPropsOrImages(Product insertedProduct, ProductCreateRequest createRequest)
    {
        Assert.Equal(createRequest.Name, insertedProduct.Name);
        Assert.Equal(createRequest.AdditionalWarrantyPrice, insertedProduct.AdditionalWarrantyPrice);
        Assert.Equal(createRequest.AdditionalWarrantyTermMonths, insertedProduct.AdditionalWarrantyTermMonths);
        Assert.Equal(createRequest.StandardWarrantyPrice, insertedProduct.StandardWarrantyPrice);
        Assert.Equal(createRequest.StandardWarrantyTermMonths, insertedProduct.StandardWarrantyTermMonths);
        Assert.Equal(createRequest.DisplayOrder, insertedProduct.DisplayOrder);
        Assert.Equal(createRequest.Status, insertedProduct.Status);
        Assert.Equal(createRequest.PlShow, insertedProduct.PlShow);
        Assert.Equal(createRequest.DisplayPrice, insertedProduct.Price);
        Assert.Equal(createRequest.Currency, insertedProduct.Currency);
        Assert.Equal(createRequest.RowGuid, insertedProduct.RowGuid);
        Assert.Equal(createRequest.PromotionId, insertedProduct.PromotionId);
        Assert.Equal(createRequest.PromRid, insertedProduct.PromRid);
        Assert.Equal(createRequest.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(createRequest.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(createRequest.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(createRequest.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(createRequest.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(createRequest.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(createRequest.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(createRequest.SearchString, insertedProduct.SearchString);

        Assert.Equal(createRequest.CategoryId, insertedProduct.CategoryId);
        Assert.Equal(createRequest.ManifacturerId, insertedProduct.ManifacturerId);
        Assert.Equal(createRequest.SubCategoryId, insertedProduct.SubCategoryId);
    }

    private static void AssertProductIsEqualToRequestWithoutPropsOrImages(Product insertedProduct, ProductCreateWithoutImagesInDatabaseRequest createRequest)
    {
        Assert.Equal(createRequest.Name, insertedProduct.Name);
        Assert.Equal(createRequest.AdditionalWarrantyPrice, insertedProduct.AdditionalWarrantyPrice);
        Assert.Equal(createRequest.AdditionalWarrantyTermMonths, insertedProduct.AdditionalWarrantyTermMonths);
        Assert.Equal(createRequest.StandardWarrantyPrice, insertedProduct.StandardWarrantyPrice);
        Assert.Equal(createRequest.StandardWarrantyTermMonths, insertedProduct.StandardWarrantyTermMonths);
        Assert.Equal(createRequest.DisplayOrder, insertedProduct.DisplayOrder);
        Assert.Equal(createRequest.Status, insertedProduct.Status);
        Assert.Equal(createRequest.PlShow, insertedProduct.PlShow);
        Assert.Equal(createRequest.DisplayPrice, insertedProduct.Price);
        Assert.Equal(createRequest.Currency, insertedProduct.Currency);
        Assert.Equal(createRequest.RowGuid, insertedProduct.RowGuid);
        Assert.Equal(createRequest.PromotionId, insertedProduct.PromotionId);
        Assert.Equal(createRequest.PromRid, insertedProduct.PromRid);
        Assert.Equal(createRequest.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(createRequest.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(createRequest.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(createRequest.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(createRequest.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(createRequest.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(createRequest.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(createRequest.SearchString, insertedProduct.SearchString);

        Assert.Equal(createRequest.CategoryId, insertedProduct.CategoryId);
        Assert.Equal(createRequest.ManifacturerId, insertedProduct.ManifacturerId);
        Assert.Equal(createRequest.SubCategoryId, insertedProduct.SubCategoryId);
    }

    private static void AssertProductIsEqualToRequestWithoutPropsOrImages(Product insertedProduct, ProductUpdateWithoutImagesInDatabaseRequest updateRequest)
    {
        Assert.Equal(updateRequest.Name, insertedProduct.Name);
        Assert.Equal(updateRequest.AdditionalWarrantyPrice, insertedProduct.AdditionalWarrantyPrice);
        Assert.Equal(updateRequest.AdditionalWarrantyTermMonths, insertedProduct.AdditionalWarrantyTermMonths);
        Assert.Equal(updateRequest.StandardWarrantyPrice, insertedProduct.StandardWarrantyPrice);
        Assert.Equal(updateRequest.StandardWarrantyTermMonths, insertedProduct.StandardWarrantyTermMonths);
        Assert.Equal(updateRequest.DisplayOrder, insertedProduct.DisplayOrder);
        Assert.Equal(updateRequest.Status, insertedProduct.Status);
        Assert.Equal(updateRequest.PlShow, insertedProduct.PlShow);
        Assert.Equal(updateRequest.DisplayPrice, insertedProduct.Price);
        Assert.Equal(updateRequest.Currency, insertedProduct.Currency);
        Assert.Equal(updateRequest.RowGuid, insertedProduct.RowGuid);
        Assert.Equal(updateRequest.PromotionId, insertedProduct.PromotionId);
        Assert.Equal(updateRequest.PromRid, insertedProduct.PromRid);
        Assert.Equal(updateRequest.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(updateRequest.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(updateRequest.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(updateRequest.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(updateRequest.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(updateRequest.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(updateRequest.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(updateRequest.SearchString, insertedProduct.SearchString);

        Assert.Equal(updateRequest.CategoryId, insertedProduct.CategoryId);
        Assert.Equal(updateRequest.ManifacturerId, insertedProduct.ManifacturerId);
        Assert.Equal(updateRequest.SubCategoryId, insertedProduct.SubCategoryId);
    }

    private static void AssertProductIsEqualToRequestWithoutPropsOrImages(Product insertedProduct, ProductFullUpdateRequest updateRequest)
    {
        Assert.Equal(updateRequest.Name, insertedProduct.Name);
        Assert.Equal(updateRequest.AdditionalWarrantyPrice, insertedProduct.AdditionalWarrantyPrice);
        Assert.Equal(updateRequest.AdditionalWarrantyTermMonths, insertedProduct.AdditionalWarrantyTermMonths);
        Assert.Equal(updateRequest.StandardWarrantyPrice, insertedProduct.StandardWarrantyPrice);
        Assert.Equal(updateRequest.StandardWarrantyTermMonths, insertedProduct.StandardWarrantyTermMonths);
        Assert.Equal(updateRequest.DisplayOrder, insertedProduct.DisplayOrder);
        Assert.Equal(updateRequest.Status, insertedProduct.Status);
        Assert.Equal(updateRequest.PlShow, insertedProduct.PlShow);
        Assert.Equal(updateRequest.DisplayPrice, insertedProduct.Price);
        Assert.Equal(updateRequest.Currency, insertedProduct.Currency);
        Assert.Equal(updateRequest.RowGuid, insertedProduct.RowGuid);
        Assert.Equal(updateRequest.PromotionId, insertedProduct.PromotionId);
        Assert.Equal(updateRequest.PromRid, insertedProduct.PromRid);
        Assert.Equal(updateRequest.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(updateRequest.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(updateRequest.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(updateRequest.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(updateRequest.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(updateRequest.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(updateRequest.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(updateRequest.SearchString, insertedProduct.SearchString);

        Assert.Equal(updateRequest.CategoryId, insertedProduct.CategoryId);
        Assert.Equal(updateRequest.ManifacturerId, insertedProduct.ManifacturerId);
        Assert.Equal(updateRequest.SubCategoryId, insertedProduct.SubCategoryId);
    }

    private static void AssertProductIsEqualToProductWithoutPropsOrImages(Product insertedProduct, Product product)
    {
        Assert.Equal(product.Name, insertedProduct.Name);
        Assert.Equal(product.AdditionalWarrantyPrice, insertedProduct.AdditionalWarrantyPrice);
        Assert.Equal(product.AdditionalWarrantyTermMonths, insertedProduct.AdditionalWarrantyTermMonths);
        Assert.Equal(product.StandardWarrantyPrice, insertedProduct.StandardWarrantyPrice);
        Assert.Equal(product.StandardWarrantyTermMonths, insertedProduct.StandardWarrantyTermMonths);
        Assert.Equal(product.DisplayOrder, insertedProduct.DisplayOrder);
        Assert.Equal(product.Status, insertedProduct.Status);
        Assert.Equal(product.PlShow, insertedProduct.PlShow);
        Assert.Equal(product.Price, insertedProduct.Price);
        Assert.Equal(product.Currency, insertedProduct.Currency);
        Assert.Equal(product.RowGuid, insertedProduct.RowGuid);
        Assert.Equal(product.PromotionId, insertedProduct.PromotionId);
        Assert.Equal(product.PromRid, insertedProduct.PromRid);
        Assert.Equal(product.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(product.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(product.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(product.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(product.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(product.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(product.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(product.SearchString, insertedProduct.SearchString);

        Assert.Equal(product.CategoryId, insertedProduct.CategoryId);
        Assert.Equal(product.ManifacturerId, insertedProduct.ManifacturerId);
        Assert.Equal(product.SubCategoryId, insertedProduct.SubCategoryId);
    }

    private static bool CompareProductAndRequestWithoutPropsOrImages(Product insertedProduct, ProductCreateRequest createRequest)
    {
        return (createRequest.Name == insertedProduct.Name
            && createRequest.AdditionalWarrantyPrice == insertedProduct.AdditionalWarrantyPrice
            && createRequest.AdditionalWarrantyTermMonths == insertedProduct.AdditionalWarrantyTermMonths
            && createRequest.StandardWarrantyPrice == insertedProduct.StandardWarrantyPrice
            && createRequest.StandardWarrantyTermMonths == insertedProduct.StandardWarrantyTermMonths
            && createRequest.DisplayOrder == insertedProduct.DisplayOrder
            && createRequest.Status == insertedProduct.Status
            && createRequest.PlShow == insertedProduct.PlShow
            && createRequest.DisplayPrice == insertedProduct.Price
            && createRequest.Currency == insertedProduct.Currency
            && createRequest.RowGuid == insertedProduct.RowGuid
            && createRequest.PromotionId == insertedProduct.PromotionId
            && createRequest.PromRid == insertedProduct.PromRid
            && createRequest.PromotionPictureId == insertedProduct.PromotionPictureId
            && createRequest.PromotionExpireDate == insertedProduct.PromotionExpireDate
            && createRequest.AlertPictureId == insertedProduct.AlertPictureId
            && createRequest.AlertExpireDate == insertedProduct.AlertExpireDate
            && createRequest.PriceListDescription == insertedProduct.PriceListDescription
            && createRequest.PartNumber1 == insertedProduct.PartNumber1
            && createRequest.PartNumber2 == insertedProduct.PartNumber2
            && createRequest.SearchString == insertedProduct.SearchString

            && createRequest.CategoryId == insertedProduct.CategoryId
            && createRequest.ManifacturerId == insertedProduct.ManifacturerId
            && createRequest.SubCategoryId == insertedProduct.SubCategoryId);
    }

#pragma warning disable IDE0051 // Remove unused private members
    private static bool CompareProductAndRequestWithoutPropsOrImages(Product insertedProduct, ProductUpdateRequest updateRequest)
    {
        return (updateRequest.Name == insertedProduct.Name
        && updateRequest.AdditionalWarrantyPrice == insertedProduct.AdditionalWarrantyPrice
        && updateRequest.AdditionalWarrantyTermMonths == insertedProduct.AdditionalWarrantyTermMonths
        && updateRequest.StandardWarrantyPrice == insertedProduct.StandardWarrantyPrice
        && updateRequest.StandardWarrantyTermMonths == insertedProduct.StandardWarrantyTermMonths
        && updateRequest.DisplayOrder == insertedProduct.DisplayOrder
        && updateRequest.Status == insertedProduct.Status
        && updateRequest.PlShow == insertedProduct.PlShow
        && updateRequest.DisplayPrice == insertedProduct.Price
        && updateRequest.Currency == insertedProduct.Currency
        && updateRequest.RowGuid == insertedProduct.RowGuid
        && updateRequest.PromotionId == insertedProduct.PromotionId
        && updateRequest.PromRid == insertedProduct.PromRid
        && updateRequest.PromotionPictureId == insertedProduct.PromotionPictureId
        && updateRequest.PromotionExpireDate == insertedProduct.PromotionExpireDate
        && updateRequest.AlertPictureId == insertedProduct.AlertPictureId
        && updateRequest.AlertExpireDate == insertedProduct.AlertExpireDate
        && updateRequest.PriceListDescription == insertedProduct.PriceListDescription
        && updateRequest.PartNumber1 == insertedProduct.PartNumber1
        && updateRequest.PartNumber2 == insertedProduct.PartNumber2
        && updateRequest.SearchString == insertedProduct.SearchString

        && updateRequest.CategoryId == insertedProduct.CategoryId
        && updateRequest.ManifacturerId == insertedProduct.ManifacturerId
        && updateRequest.SubCategoryId == insertedProduct.SubCategoryId);
    }

    private static bool CompareProductAndProductWithoutPropsOrImages(Product insertedProduct, Product product)
    {
        return (product.Name == insertedProduct.Name
        && product.AdditionalWarrantyPrice == insertedProduct.AdditionalWarrantyPrice
        && product.AdditionalWarrantyTermMonths == insertedProduct.AdditionalWarrantyTermMonths
        && product.StandardWarrantyPrice == insertedProduct.StandardWarrantyPrice
        && product.StandardWarrantyTermMonths == insertedProduct.StandardWarrantyTermMonths
        && product.DisplayOrder == insertedProduct.DisplayOrder
        && product.Status == insertedProduct.Status
        && product.PlShow == insertedProduct.PlShow
        && product.Price == insertedProduct.Price
        && product.Currency == insertedProduct.Currency
        && product.RowGuid == insertedProduct.RowGuid
        && product.PromotionId == insertedProduct.PromotionId
        && product.PromRid == insertedProduct.PromRid
        && product.PromotionPictureId == insertedProduct.PromotionPictureId
        && product.PromotionExpireDate == insertedProduct.PromotionExpireDate
        && product.AlertPictureId == insertedProduct.AlertPictureId
        && product.AlertExpireDate == insertedProduct.AlertExpireDate
        && product.PriceListDescription == insertedProduct.PriceListDescription
        && product.PartNumber1 == insertedProduct.PartNumber1
        && product.PartNumber2 == insertedProduct.PartNumber2
        && product.SearchString == insertedProduct.SearchString

        && product.CategoryId == insertedProduct.CategoryId
        && product.ManifacturerId == insertedProduct.ManifacturerId
        && product.SubCategoryId == insertedProduct.SubCategoryId);
    }

#pragma warning restore IDE0051 // Remove unused private members
    private bool ComparePropertiesInRequestAndProduct(
        List<CurrentProductPropertyCreateRequest>? propsInRequest, List<ProductProperty>? propsInObject)
    {
        if (propsInRequest is null && propsInObject is null) return true;

        if (propsInRequest is null || propsInObject is null) return false;

        if (propsInRequest.Count != propsInObject.Count) return false;

        List<CurrentProductPropertyCreateRequest> orderedPropsInRequest = propsInRequest.OrderBy(x => x.ProductCharacteristicId).ToList();
        List<ProductProperty> orderedPropsInObject = propsInObject.OrderBy(x => x.ProductCharacteristicId).ToList();

        for (int i = 0; i < orderedPropsInRequest.Count; i++)
        {
            CurrentProductPropertyCreateRequest propInRequest = orderedPropsInRequest[i];
            ProductProperty propInObject = orderedPropsInObject[i];

            int? expectedDisplayOrder = propInRequest.CustomDisplayOrder;

            if (expectedDisplayOrder is null)
            {
                ProductCharacteristic? relatedCharacteristic = _productCharacteristicService.GetById(propInRequest.ProductCharacteristicId);

                expectedDisplayOrder = relatedCharacteristic?.DisplayOrder;
            }

            if (propInRequest.ProductCharacteristicId != propInObject.ProductCharacteristicId
                || propInRequest.Value != propInObject.Value
                || expectedDisplayOrder != propInObject.DisplayOrder
                || propInRequest.XmlPlacement != propInObject.XmlPlacement)
            {
                return false;
            }
        }

        return true;
    }

    private bool ComparePropertiesInRequestAndProduct(
        List<LocalProductPropertyUpsertRequest>? propsInRequest, List<ProductProperty>? propsInObject)
    {
        if (propsInRequest is null && propsInObject is null) return true;

        if (propsInRequest is null || propsInObject is null) return false;

        if (propsInRequest.Count != propsInObject.Count) return false;

        List<LocalProductPropertyUpsertRequest> orderedPropsInRequest = propsInRequest.OrderBy(x => x.ProductCharacteristicId).ToList();
        List<ProductProperty> orderedPropsInObject = propsInObject.OrderBy(x => x.ProductCharacteristicId).ToList();

        for (int i = 0; i < orderedPropsInRequest.Count; i++)
        {
            LocalProductPropertyUpsertRequest propInRequest = orderedPropsInRequest[i];
            ProductProperty propInObject = orderedPropsInObject[i];

            int? expectedDisplayOrder = propInRequest.CustomDisplayOrder;

            if (expectedDisplayOrder is null)
            {
                ProductCharacteristic? relatedCharacteristic = _productCharacteristicService.GetById(propInRequest.ProductCharacteristicId);

                expectedDisplayOrder = relatedCharacteristic?.DisplayOrder;
            }

            if (propInRequest.ProductCharacteristicId != propInObject.ProductCharacteristicId
                || propInRequest.Value != propInObject.Value
                || expectedDisplayOrder != propInObject.DisplayOrder
                || propInRequest.XmlPlacement != propInObject.XmlPlacement)
            {
                return false;
            }
        }

        return true;
    }

    private static bool CompareImagesInRequestAndProduct(List<CurrentProductImageCreateRequest>? imagesInRequest, List<ProductImage>? imagesInObject)
    {
        if (imagesInRequest is null && imagesInObject is null) return true;

        if (imagesInRequest is null || imagesInObject is null) return false;

        for (int i = 0; i < imagesInRequest.Count; i++)
        {
            CurrentProductImageCreateRequest imageInRequest = imagesInRequest[i];

            bool isMatched = false;

            for (int j = 0; j < imagesInObject.Count; j++)
            {
                ProductImage imageInObject = imagesInObject[j];

                if (CompareDataInByteArrays(imageInRequest.ImageData, imageInObject.ImageData)
                    && imageInRequest.ImageContentType == imageInObject.ImageContentType
                    && imageInRequest.HtmlData == imageInObject.HtmlData)
                {
                    isMatched = true;

                    break;
                }
            }

            if (!isMatched) return false;
        }

        return true;
    }

    private static bool CompareImagesInRequestAndProduct(List<ImageAndImageFileNameUpsertRequest>? imagesInRequest, List<ProductImage>? imagesInObject)
    {
        if (imagesInRequest is null && imagesInObject is null) return true;

        if (imagesInRequest is null || imagesInObject is null) return false;

        for (int i = 0; i < imagesInObject.Count; i++)
        {
            ProductImage imageInObject = imagesInObject[i];

            bool isMatched = false;

            for (int j = 0; j < imagesInRequest.Count; j++)
            {
                ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest = imagesInRequest[j];

                ProductImageUpsertRequest? imageInRequest = imageAndImageFileNameUpsertRequest.ProductImageUpsertRequest;

                if (imageInRequest == null) continue;

                if (CompareDataInByteArrays(imageAndImageFileNameUpsertRequest.ImageData, imageInObject.ImageData)
                    && imageAndImageFileNameUpsertRequest.ImageContentType == imageInObject.ImageContentType)
                {
                    isMatched = true;

                    break;
                }
            }

            if (!isMatched) return false;
        }

        return true;
    }

    private static bool CompareImageFileNamesInRequestAndProduct(List<CurrentProductImageFileNameInfoCreateRequest>? imageFileNamesInRequest, List<ProductImageFileNameInfo>? imageFileNamesInObject)
    {
        if (imageFileNamesInRequest is null && imageFileNamesInObject is null) return true;

        if (imageFileNamesInRequest is null || imageFileNamesInObject is null) return false;

        if (imageFileNamesInRequest.Count != imageFileNamesInObject.Count) return false;

        List<CurrentProductImageFileNameInfoCreateRequest> orderedImageFileNamesInRequest = imageFileNamesInRequest.OrderBy(x => x.DisplayOrder).ToList();
        List<ProductImageFileNameInfo> orderedImageFileNamesInObject = imageFileNamesInObject.OrderBy(x => x.DisplayOrder).ToList();

        for (int i = 0; i < orderedImageFileNamesInRequest.Count; i++)
        {
            CurrentProductImageFileNameInfoCreateRequest imageFileNameInRequest = orderedImageFileNamesInRequest[i];
            ProductImageFileNameInfo imageFileNameInObject = orderedImageFileNamesInObject[i];

            if (imageFileNameInRequest.FileName != imageFileNameInObject.FileName
                || imageFileNameInRequest.DisplayOrder != imageFileNameInObject.DisplayOrder
                || imageFileNameInRequest.Active != imageFileNameInObject.Active)
            {
                return false;
            }
        }

        return true;
    }

    private static bool CompareImageFileNamesAndImageFilesInRequestAndProduct(
        int productId,
        List<ImageFileAndFileNameInfoUpsertRequest>? imageFileAndFileNameUpsertRequests,
        List<ProductImageFileNameInfo>? imageFileNamesInObject)
    {
        if (imageFileAndFileNameUpsertRequests is null && imageFileNamesInObject is null) return true;

        if (imageFileAndFileNameUpsertRequests is null || imageFileNamesInObject is null) return false;

        if (imageFileAndFileNameUpsertRequests.Count != imageFileNamesInObject.Count) return false;

        List<ImageFileAndFileNameInfoUpsertRequest> orderedImageFileUpsertRequests
            = OrderImageFileAndFileNameInfoUpsertRequests(imageFileAndFileNameUpsertRequests);

        List<ProductImageFileNameInfo> orderedImageFileNamesInObject = OrderImageFileNameInfos(imageFileNamesInObject);

        for (int i = 0; i < orderedImageFileUpsertRequests.Count; i++)
        {
            ImageFileAndFileNameInfoUpsertRequest imageFileUpsertRequest = orderedImageFileUpsertRequests[i];
            ProductImageFileNameInfo imageFileNameInObject = orderedImageFileNamesInObject[i]!;

            if (!CompareFileNameData(productId, orderedImageFileUpsertRequests, i, imageFileNameInObject)
                || imageFileUpsertRequest.DisplayOrder != imageFileNameInObject.DisplayOrder
                || imageFileUpsertRequest.Active != imageFileNameInObject.Active)
            {
                return false;
            }

            if (imageFileNameInObject.FileName is not null)
            {
                bool isImageDataEqual = false;

                string filePath = Path.Combine(Startup.ImageDirectoryFullPath, imageFileNameInObject.FileName);

                if (File.Exists(filePath))
                {
                    byte[] imageData = File.ReadAllBytes(filePath);

                    byte[] imageDataEncoded = GetImageDataEncoded(imageFileUpsertRequest.ImageData);

                    isImageDataEqual = imageData.SequenceEqual(imageDataEncoded);
                }

                if (!isImageDataEqual) return false;
            }
        }

        return true;
    }

    private static bool CompareUpdateRequestDataWithActualUpdatedData(
        int productId,
        List<CurrentProductImageFileNameInfoCreateRequest>? createRequestData,
        List<ImageAndImageFileNameUpsertRequest>? upsertRequests,
        List<ProductImageFileNameInfo>? imageFileNamesInObject)
    {
        if (upsertRequests is null)
        {
            return CompareImageFileNamesInRequestAndProduct(createRequestData, imageFileNamesInObject);
        }

        bool areFileNamesInObjectEmpty = imageFileNamesInObject is null
            || imageFileNamesInObject.Count <= 0;

        if (areFileNamesInObjectEmpty)
        {
            return upsertRequests.Count == 0;
        }

        if (imageFileNamesInObject!.Count != upsertRequests.Count) return false;

        foreach (ProductImageFileNameInfo productImageFileNameInfo in imageFileNamesInObject)
        {
            ImageAndImageFileNameUpsertRequest? relatedUpsertRequest = GetRelatedUpsertRequest(productId, productImageFileNameInfo, upsertRequests);

            if (relatedUpsertRequest is not null)
            {
                ProductImageFileNameInfoUpsertRequest? fileNameInfoUpsertRequest = relatedUpsertRequest.ProductImageFileNameInfoUpsertRequest;

                if (fileNameInfoUpsertRequest is null) return false;

                return fileNameInfoUpsertRequest.NewDisplayOrder == productImageFileNameInfo.DisplayOrder
                    && fileNameInfoUpsertRequest.Active == productImageFileNameInfo.Active
                    && CompareFileNameData(productId, upsertRequests, upsertRequests.IndexOf(relatedUpsertRequest), productImageFileNameInfo);
            }
        }

        return false;
    }

    private static ImageAndImageFileNameUpsertRequest? GetRelatedUpsertRequest(
        int productId,
        ProductImageFileNameInfo productImageFileNameInfo,
        List<ImageAndImageFileNameUpsertRequest> upsertRequests)
    {
        ImageAndImageFileNameUpsertRequest? relatedUpsertRequest = upsertRequests.FirstOrDefault(
            x => productImageFileNameInfo.ImageNumber == x.ProductImageFileNameInfoUpsertRequest?.OriginalImageNumber);

        if (relatedUpsertRequest is not null) return relatedUpsertRequest;

        for (int i = 0; i < upsertRequests.Count; i++)
        {
            ImageAndImageFileNameUpsertRequest upsertRequest = upsertRequests[i];

            ProductImageFileNameInfoUpsertRequest? fileNameInfoUpsertRequest = upsertRequest.ProductImageFileNameInfoUpsertRequest;

            if (fileNameInfoUpsertRequest is null) continue;

            if (fileNameInfoUpsertRequest.NewDisplayOrder == productImageFileNameInfo.DisplayOrder
                && fileNameInfoUpsertRequest.Active == productImageFileNameInfo.Active
                && CompareFileNameData(productId, upsertRequests, i, productImageFileNameInfo))
            {
                return upsertRequest;
            }
        }

        return null;
    }

    private static bool CompareFileNameData(
        int productId,
        List<ImageFileAndFileNameInfoUpsertRequest> upsertRequests,
        int indexOfItem,
        ProductImageFileNameInfo productImageFileNameInfo)
    {
        ImageFileAndFileNameInfoUpsertRequest imageFileUpsertRequest = upsertRequests[indexOfItem];

        string? fileName = productImageFileNameInfo.FileName;

        if (fileName is null) return false;

        string? expectedFileExtension = GetImageFileExtensionFromContentType(imageFileUpsertRequest.ImageContentType);

        if (expectedFileExtension is null) return false;

        string expectedFileName;

        if (imageFileUpsertRequest.CustomFileNameWithoutExtension is not null)
        {
            expectedFileName = $"{imageFileUpsertRequest.CustomFileNameWithoutExtension}.{expectedFileExtension}";
        }
        else if (imageFileUpsertRequest?.RelatedImageId is not null)
        {
            expectedFileName = $"{imageFileUpsertRequest.RelatedImageId}.{expectedFileExtension}";
        }
        else
        {
            string? fileNameFromData = GetTemporaryFileNameWithoutExtension(productId, productImageFileNameInfo.ImageNumber);

            if (fileNameFromData is null) return false;

            expectedFileName = $"{fileNameFromData}.{expectedFileExtension}";
        }

        return expectedFileName == fileName;
    }

    private static bool CompareFileNameData(
        int productId,
        List<ImageAndImageFileNameUpsertRequest> upsertRequests,
        int indexOfItem,
        ProductImageFileNameInfo productImageFileNameInfo)
    {
        ImageAndImageFileNameUpsertRequest imageAndImageFileNameUpsertRequest = upsertRequests[indexOfItem];

        ProductImageUpsertRequest? imageUpsertRequest = imageAndImageFileNameUpsertRequest.ProductImageUpsertRequest;
        ProductImageFileNameInfoUpsertRequest? fileNameInfoUpsertRequest = imageAndImageFileNameUpsertRequest.ProductImageFileNameInfoUpsertRequest;

        string? fileName = productImageFileNameInfo.FileName;

        if (fileNameInfoUpsertRequest is null) return fileName is null;

        if (fileName is null) return false;

        string? expectedFileExtension = GetImageFileExtensionFromContentType(imageAndImageFileNameUpsertRequest.ImageContentType);

        if (expectedFileExtension is null) return false;

        string expectedFileName;

        if (fileNameInfoUpsertRequest.CustomFileNameWithoutExtension is not null)
        {
            expectedFileName = $"{fileNameInfoUpsertRequest.CustomFileNameWithoutExtension}.{expectedFileExtension}";
        }
        else if (imageUpsertRequest?.OriginalImageId is not null)
        {
            expectedFileName = $"{imageUpsertRequest.OriginalImageId}.{expectedFileExtension}";
        }
        else
        {
            int? expectedImageNumber = productImageFileNameInfo.ImageNumber;

            if (expectedImageNumber is null) return false;

            string? fileNameFromData = GetTemporaryFileNameWithoutExtension(productId, expectedImageNumber.Value);

            if (fileNameFromData is null) return false;

            expectedFileName = $"{fileNameFromData}.{expectedFileExtension}";
        }

        return expectedFileName == fileName;
    }

    private static List<ImageFileAndFileNameInfoUpsertRequest> OrderImageFileAndFileNameInfoUpsertRequests(
        List<ImageFileAndFileNameInfoUpsertRequest> imagesAndImageFileNameInfos)
    {
        imagesAndImageFileNameInfos = imagesAndImageFileNameInfos.ToList();

        ImageFileAndFileNameInfoUpsertRequest[] output
            = new ImageFileAndFileNameInfoUpsertRequest[imagesAndImageFileNameInfos.Count];

        for (int i = 0; i < imagesAndImageFileNameInfos.Count; i++)
        {
            ImageFileAndFileNameInfoUpsertRequest imageFileUpsertRequest = imagesAndImageFileNameInfos[i];

            if (imageFileUpsertRequest.DisplayOrder is null) continue;

            output[imageFileUpsertRequest.DisplayOrder.Value - 1] = imageFileUpsertRequest;

            imagesAndImageFileNameInfos.Remove(imageFileUpsertRequest);

            i--;
        }

        foreach (ImageFileAndFileNameInfoUpsertRequest imageFileUpsertRequest in imagesAndImageFileNameInfos
            .OrderBy(x => x.RelatedImageId ?? int.MaxValue))
        {
            for (int i = 0; i < output.Length; i++)
            {
                ImageFileAndFileNameInfoUpsertRequest outputImageFileUpsertRequest = output[i];

                if (outputImageFileUpsertRequest != null) continue;

                output[i] = imageFileUpsertRequest;

                break;
            }
        }

        return output.ToList();
    }

    private static List<ImageAndImageFileNameUpsertRequest> OrderImageAndImageFileNameUpsertRequests(
        List<ImageAndImageFileNameUpsertRequest> imagesAndImageFileNameInfos)
    {
        imagesAndImageFileNameInfos = imagesAndImageFileNameInfos.ToList();

        ImageAndImageFileNameUpsertRequest[] output
            = new ImageAndImageFileNameUpsertRequest[imagesAndImageFileNameInfos.Count];

        for (int i = 0; i < imagesAndImageFileNameInfos.Count; i++)
        {
            ImageAndImageFileNameUpsertRequest imageAndFileNameRelation = imagesAndImageFileNameInfos[i];

            ProductImageUpsertRequest? image = imageAndFileNameRelation.ProductImageUpsertRequest;
            ProductImageFileNameInfoUpsertRequest? imageFileNameInfo = imageAndFileNameRelation.ProductImageFileNameInfoUpsertRequest;

            if (image is null
                || imageFileNameInfo is null
                || imageFileNameInfo.NewDisplayOrder is null) continue;

            output[imageFileNameInfo.NewDisplayOrder.Value - 1] = imageAndFileNameRelation;

            imagesAndImageFileNameInfos.Remove(imageAndFileNameRelation);

            i--;
        }

        foreach (ImageAndImageFileNameUpsertRequest imageAndFileNameRelation in imagesAndImageFileNameInfos
            .OrderBy(x => x.ProductImageUpsertRequest?.OriginalImageId ?? int.MaxValue))
        {
            for (int i = 0; i < output.Length; i++)
            {
                ImageAndImageFileNameUpsertRequest outputRelation = output[i];

                if (outputRelation != null) continue;

                output[i] = imageAndFileNameRelation;

                break;
            }
        }

        return output.ToList();
    }

    private static List<ProductImageFileNameInfo> OrderImageFileNameInfos(
        List<ProductImageFileNameInfo> productImageFileNameInfos)
    {
        productImageFileNameInfos = new(productImageFileNameInfos);

        ProductImageFileNameInfo[] output
            = new ProductImageFileNameInfo[productImageFileNameInfos.Count];

        for (int i = 0; i < productImageFileNameInfos.Count; i++)
        {
            ProductImageFileNameInfo imageFileNameInfo = productImageFileNameInfos[i];

            if (imageFileNameInfo is null
                || imageFileNameInfo.DisplayOrder is null) continue;

            output[imageFileNameInfo.DisplayOrder.Value - 1] = imageFileNameInfo;

            productImageFileNameInfos.Remove(imageFileNameInfo);

            i--;
        }

        foreach (ProductImageFileNameInfo productImageFileNameInfo in productImageFileNameInfos
            .OrderBy(x => GetImageIdFromFileName(x.FileName) ?? int.MaxValue))
        {
            for (int i = 0; i < output.Length; i++)
            {
                ProductImageFileNameInfo outputFileNameInfo = output[i];

                if (outputFileNameInfo != null) continue;

                output[i] = productImageFileNameInfo;

                break;
            }
        }

        return output.ToList();
    }

    private static int? GetImageIdFromFileName(string? fileName)
    {
        if (fileName is null) return null;

        string imageIdPart = Path.GetFileNameWithoutExtension(fileName);

        bool isParseSuccessful = int.TryParse(imageIdPart, out int imageId);

        return isParseSuccessful ? imageId : null;
    }
}