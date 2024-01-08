using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductServiceTests(
        IProductService productService,
        IProductImageService productImageService,
        IProductImageFileNameInfoService productImageFileNameInfoService,
        IProductPropertyService productPropertyService,
        IProductStatusesService productStatusesService)
        : base(Startup.ConnectionString)
    {
        _productService = productService;
        _productImageService = productImageService;
        _productImageFileNameInfoService = productImageFileNameInfoService;
        _productPropertyService = productPropertyService;
        _productStatusesService = productStatusesService;
    }

    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileNameInfoService _productImageFileNameInfoService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductStatusesService _productStatusesService;
    private const int _useRequiredValue = -100;

    private static readonly ProductCreateRequest _validCreateRequest = new()
    {
        Name = "Nameee",
        AdditionalWarrantyPrice = 0.00M,
        AdditionalWarrantyTermMonths = 0,
        StandardWarrantyPrice = "3.00",
        StandardWarrantyTermMonths = 36,
        DisplayOrder = 13123,
        Status = ProductStatusEnum.Call,
        PlShow = 0,
        Price1 = 125.65M,
        DisplayPrice = 126.79M,
        Price3 = 128.99M,
        Currency = CurrencyEnum.EUR,
        RowGuid = Guid.NewGuid(),
        Promotionid = null,
        PromRid = null,
        PromotionPictureId = null,
        PromotionExpireDate = null,
        AlertPictureId = null,
        AlertExpireDate = null,
        PriceListDescription = null,
        PartNumber1 = "dsadasdasd",
        PartNumber2 = "12123",
        SearchString = "DC CDKC 33567",

        Properties = new()
        {
            new() { ProductCharacteristicId = 127, DisplayOrder = 12312, Value = "asdaddrer", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new() { ProductCharacteristicId = 129, DisplayOrder = 12332, Value = "asdaddrer", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
        },
        Images = new(),
        ImageFileNames = new(),

        CategoryID = 7,
        ManifacturerId = 12,
        SubCategoryId = null,
    };

    [Fact]
    public void GetAllWithoutImagesAndProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        IEnumerable<Product> allProducts = _productService.GetAllWithoutImagesAndProps();

        Product product1 = allProducts.Single(x => x.Id == productId1);
        Product product2 = allProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereSearchStringMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        IEnumerable<Product> allProducts = _productService.GetAllWhereSearchStringMatches(validCreateRequest.SearchString!);

        Product product1 = allProducts.Single(x => x.Id == productId1);
        Product product2 = allProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereNameMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        IEnumerable<Product> allProducts = _productService.GetAllWhereNameMatches(validCreateRequest.Name!);

        Product product1 = allProducts.Single(x => x.Id == productId1);
        Product product2 = allProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetAllWithoutImagesAndProps_ShouldOnlyGetTheDataThatWasSuccessfullyInserted()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        ProductCreateRequest invalidCreateRequest = GetValidProductCreateRequest();

        invalidCreateRequest.Name = "  ";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(invalidCreateRequest);

        Assert.True(insertResult2.Match(
            _ => false,
            _ => true,
            _ => false));

        IEnumerable<Product> allProducts = _productService.GetAllWithoutImagesAndProps();

        Product product1 = allProducts.Single(x => x.Id == productId1);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.DoesNotContain(allProducts, x =>
            CompareProductAndRequestWithoutPropsOrImages(x, invalidCreateRequest));

        // Deterministic delete
        _productService.DeleteProducts(productId1);
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereSearchStringMatches_ShouldOnlyGetTheDataThatWasSuccessfullyInserted()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        ProductCreateRequest invalidCreateRequest = GetValidProductCreateRequest();

        invalidCreateRequest.Name = "  ";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(invalidCreateRequest);

        Assert.True(insertResult2.Match(
            _ => false,
            _ => true,
            _ => false));

        IEnumerable<Product> allProducts = _productService.GetAllWhereSearchStringMatches(validCreateRequest.SearchString!);

        Product product1 = allProducts.Single(x => x.Id == productId1);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.DoesNotContain(allProducts, x =>
            CompareProductAndRequestWithoutPropsOrImages(x, invalidCreateRequest));

        // Deterministic delete
        _productService.DeleteProducts(productId1);
    }

    [Fact]
    public void GetWithoutImagesAndPropsWhereNameMatches_ShouldOnlyGetTheDataThatWasSuccessfullyInserted()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        ProductCreateRequest invalidCreateRequest = GetValidProductCreateRequest();

        invalidCreateRequest.Name = "  ";

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(invalidCreateRequest);

        Assert.True(insertResult2.Match(
            _ => false,
            _ => true,
            _ => false));

        IEnumerable<Product> allProducts = _productService.GetAllWhereNameMatches(validCreateRequest.Name!);

        Product product1 = allProducts.Single(x => x.Id == productId1);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.DoesNotContain(allProducts, x =>
            CompareProductAndRequestWithoutPropsOrImages(x, invalidCreateRequest));

        // Deterministic delete
        _productService.DeleteProducts(productId1);
    }

    [Fact]
    public void GetSelectionWithoutImagesAndProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        List<uint> productIds = new() { productId1, productId2 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithoutImagesAndProps(productIds);

        Assert.True(insertedProducts.Count() == 2);

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetSelectionWithoutImagesAndProps_ShouldOnlyGetProductsWithValidIds()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        List<uint> productIds = new() { productId1, productId2, 0 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithoutImagesAndProps(productIds);

        Assert.True(insertedProducts.Count() == 2);

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetSelectionWithFirstImage_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        List<uint> productIds = new() { productId1, productId2 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithFirstImage(productIds);

        Assert.True(insertedProducts.Count() == 2);

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        List<CurrentProductImageCreateRequest> firstImageInRequest = new() { validCreateRequest.Images!.First() };

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product1.Images));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product2.Images));

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetSelectionWithFirstImage_ShouldOnlyGetProductsWithValidIds()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        List<uint> productIds = new() { productId1, productId2, 0 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithFirstImage(productIds);

        Assert.True(insertedProducts.Count() == 2);

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        List<CurrentProductImageCreateRequest> firstImageInRequest = new() { validCreateRequest.Images!.First() };

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product1.Images));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, product2.Images));

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetSelectionWithProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        List<uint> productIds = new() { productId1, productId2 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithProps(productIds);

        Assert.True(insertedProducts.Count() == 2);

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product1.Properties));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product2.Properties));

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetSelectionWithProps_ShouldOnlyGetProductsWithValidIds()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult1 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = insertResult1.AsT0;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult2 = _productService.Insert(validCreateRequest);

        Assert.True(insertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = insertResult2.AsT0;

        List<uint> productIds = new() { productId1, productId2, 0 };

        IEnumerable<Product> insertedProducts = _productService.GetSelectionWithProps(productIds);

        Assert.True(insertedProducts.Count() == 2);

        Product product1 = insertedProducts.Single(x => x.Id == productId1);
        Product product2 = insertedProducts.Single(x => x.Id == productId2);

        Assert.Equal((int)productId1, product1.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product1, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product1.Properties));

        Assert.Equal((int)productId2, product2.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(product2, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, product2.Properties));

        // Deterministic delete
        _productService.DeleteProducts(productId1, productId2);
    }

    [Fact]
    public void GetFirstItemsBetweenStartAndEnd_ShouldSucceed_WhenInsertsAreValid()
    {
        List<uint> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.DisplayOrder = i + 1;

            OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            Assert.True(insertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            uint productId = insertResult.AsT0;

            productIds.Add(productId);
        }

        List<Product> allProductsRanged = _productService.GetAllWithoutImagesAndProps()
            .Skip(10)
            .Take(10)
            .ToList();

        List<Product> productsInRange = _productService.GetFirstItemsBetweenStartAndEnd(new() { Start = 10, Length = 10 }).ToList();

        Assert.True(productsInRange.Count >= 2);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }

        // Deterministic delete
        _productService.DeleteProducts(productIds.ToArray());
    }

    [Fact]
    public void GetFirstInRangeWhereSearchStringMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        const string searchStringOfData = "hic opriwopirvoprjvopi og otioiok/df3243";

        List<uint> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.SearchString = searchStringOfData;

            OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            Assert.True(insertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            uint productId = insertResult.AsT0;

            productIds.Add(productId);
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

        // Deterministic delete
        _productService.DeleteProducts(productIds.ToArray());
    }

    [Fact]
    public void GetFirstInRangeWhereNameMatches_ShouldSucceed_WhenInsertsAreValid()
    {
        const string nameOfData = "hic opriwopirvoppi og /df3243";

        List<uint> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.Name = nameOfData;

            OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            Assert.True(insertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            uint productId = insertResult.AsT0;

            productIds.Add(productId);
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

        // Deterministic delete
        _productService.DeleteProducts(productIds.ToArray());
    }

    [Fact]
    public void GetFirstInRangeWhereAllConditionsAreMet_ShouldSucceed_WhenInsertsAreValid_AndAllConditionsExceptForTheStatusesTableAndCategoryAreUsed()
    {
        const string nameOfData = "hic opriwopirvoppi og /df3243";
        const string searchStringOfData = "hic opriwopirvoprjsdsvopi og otioiok/df3243";
        const ProductStatusEnum statusOfData = ProductStatusEnum.Available;

        const string nameOfProductThatDoesntFollowCondition = "not the name in the condition";

        List<uint> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.Name = nameOfData;
            validCreateRequest.SearchString = searchStringOfData;
            validCreateRequest.Status = statusOfData;

            OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            Assert.True(insertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            uint productId = insertResult.AsT0;

            productIds.Add(productId);
        }

        ProductCreateRequest validCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        validCreateRequest2.Name = nameOfProductThatDoesntFollowCondition;
        validCreateRequest2.Status = statusOfData;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResultForProductThatDoesntMatchConditions = _productService.Insert(validCreateRequest2);

        Assert.True(insertResultForProductThatDoesntMatchConditions.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productIdForProductThatDoesntMatchConditions = insertResultForProductThatDoesntMatchConditions.AsT0;

        productIds.Add(productIdForProductThatDoesntMatchConditions);

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

        Assert.True(productsInRange.Count == 10);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }

        // Deterministic delete
        _productService.DeleteProducts(productIds.ToArray());
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

        List<uint> productIds = new();

        for (int i = 0; i < 20; i++)
        {
            ProductCreateRequest validCreateRequest = GetValidProductCreateRequestUsingRandomData();

            // Updating display order so that everything is ordered
            validCreateRequest.Name = nameOfData;
            validCreateRequest.SearchString = searchStringOfData;
            validCreateRequest.Status = statusOfData;

            OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

            Assert.True(insertResult.Match(
                _ => true,
                _ => false,
                _ => false));

            uint productId = insertResult.AsT0;

            ProductStatusesCreateRequest productStatusesCreateRequest = new()
            {
                ProductId = (int)productId,
                IsProcessed = isProcessedForData,
                NeedsToBeUpdated = needsToBeUpdatedForData
            };

            OneOf<Success, ValidationResult> productStatusesInsertResult = _productStatusesService.InsertIfItDoesntExist(productStatusesCreateRequest);

            Assert.True(productStatusesInsertResult.Match(
                _ => true,
                _ => false));

            productIds.Add(productId);
        }

        ProductCreateRequest validCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        validCreateRequest2.Name = nameOfProductThatDoesntFollowCondition;
        validCreateRequest2.Status = statusOfData;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResultForProductThatDoesntMatchConditions = _productService.Insert(validCreateRequest2);

        Assert.True(insertResultForProductThatDoesntMatchConditions.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productIdForProductThatDoesntMatchConditions = insertResultForProductThatDoesntMatchConditions.AsT0;

        productIds.Add(productIdForProductThatDoesntMatchConditions);

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

        Assert.True(productsInRange.Count == 10);

        Assert.Equal(allProductsRanged.Count, productsInRange.Count);

        for (int i = 0; i < allProductsRanged.Count; i++)
        {
            Product productInAll = allProductsRanged[i];
            Product productInRange = productsInRange[i];

            AssertProductIsEqualToProductWithoutPropsOrImages(productInRange, productInAll);
        }

        // Deterministic delete
        _productService.DeleteProducts(productIds.ToArray());
        _productStatusesService.DeleteRangeProductStatusesByProductIds(productIds.ToArray());
    }

    [Fact]
    public void GetByIdWithFirstImage_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        Product? insertedProduct = _productService.GetByIdWithFirstImage(productId);

        Assert.NotNull(insertedProduct);

        List<CurrentProductImageCreateRequest> firstImageInRequest = new() { validCreateRequest.Images!.First() };

        Assert.Equal((int)productId, insertedProduct.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(firstImageInRequest, insertedProduct.Images));

        // Deterministic delete
        _productService.DeleteProducts(productId);
    }

    [Fact]
    public void GetByIdWithFirstImage_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        Product? insertedProduct = _productService.GetByIdWithFirstImage(0);

        Assert.Null(insertedProduct);

        // Deterministic delete
        _productService.DeleteProducts(productId);
    }

    [Fact]
    public void GetByIdWithProps_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        Product? insertedProduct = _productService.GetByIdWithProps(productId);

        Assert.NotNull(insertedProduct);

        Assert.Equal((int)productId, insertedProduct.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, validCreateRequest);
        Assert.True(ComparePropertiesInRequestAndProduct(validCreateRequest.Properties, insertedProduct.Properties));

        // Deterministic delete
        _productService.DeleteProducts(productId);
    }

    [Fact]
    public void GetByIdWithProps_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        Product? insertedProduct = _productService.GetByIdWithProps(0);

        Assert.Null(insertedProduct);

        // Deterministic delete
        _productService.DeleteProducts(productId);
    }

    [Fact]
    public void GetByIdWithImages_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        Product? insertedProduct = _productService.GetByIdWithImages(productId);

        Assert.NotNull(insertedProduct);

        Assert.Equal((int)productId, insertedProduct.Id);
        AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, validCreateRequest);
        Assert.True(CompareImagesInRequestAndProduct(validCreateRequest.Images, insertedProduct.Images));

        // Deterministic delete
        _productService.DeleteProducts(productId);
    }

    [Fact]
    public void GetByIdWithImages_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        Product? insertedProduct = _productService.GetByIdWithImages(0);

        Assert.Null(insertedProduct);

        // Deterministic delete
        _productService.DeleteProducts(productId);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ProductCreateRequest createRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(createRequest);

        Assert.Equal(expected, insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        if (expected)
        {
            uint productId = insertResult.AsT0;

            Product? insertedProduct = _productService.GetByIdWithProps(productId);

            List<ProductImage> productImagesForProduct = _productImageService.GetAllInProduct(productId).ToList();
            List<ProductImageFileNameInfo> productImageFileNamesForProduct = _productImageFileNameInfoService.GetAllForProduct(productId).ToList();

            Assert.NotNull(insertedProduct);

            AssertProductIsEqualToRequestWithoutPropsOrImages(insertedProduct, createRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(createRequest.Properties, insertedProduct.Properties));
            Assert.True(CompareImagesInRequestAndProduct(createRequest.Images, productImagesForProduct));
            Assert.True(CompareImageFileNamesInRequestAndProduct(createRequest.ImageFileNames, productImageFileNamesForProduct));

            // Deterministic delete
            _productService.DeleteProducts(productId);
        }
    }

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static List<object[]> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            ValidProductCreateRequest,
            true
        },

        new object[2]
        {
            GetValidProductCreateRequestUsingRandomData(),
            true
        },

        new object[2]
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
                Promotionid = null,
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
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },
                Images = new List<CurrentProductImageCreateRequest>()
                {
                },
                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1 },
                    new() { FileName = "20144.png", DisplayOrder = 2 }
                },

                CategoryID = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },

        new object[2]
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
                Promotionid = null,
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
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                Images = new List<CurrentProductImageCreateRequest>()
                {
                },

                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1 },
                    new() { FileName = "20144.png", DisplayOrder = 2 }
                },

                CategoryID = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },

        new object[2]
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
                Promotionid = null,
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
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },

                Images = new List<CurrentProductImageCreateRequest>()
                {
                },

                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1 },
                    new() { FileName = "20144.png", DisplayOrder = 2 }
                },

                CategoryID = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },

        new object[2]
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
                Promotionid = null,
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
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 129, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 130, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                    new CurrentProductPropertyCreateRequest() { ProductCharacteristicId = 131, DisplayOrder = 13213, Value = "DDS256", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
                },
                Images = new List<CurrentProductImageCreateRequest>()
                {
                },
                ImageFileNames = new List<CurrentProductImageFileNameInfoCreateRequest>()
                {
                    new() { FileName = "20143.png", DisplayOrder = 1 },
                    new() { FileName = "20144.png", DisplayOrder = 2 }
                },

                CategoryID = 7,
                ManifacturerId = 12,
                SubCategoryId = null,
            },

            false
        },
    };

#pragma warning restore CA2211 // Non-constant fields should not be visible

    [Theory]
    [MemberData(nameof(Update_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Update_ShouldSucceedOrFail_InAnExpectedManner(ProductUpdateRequest updateRequest, bool expected)
    {
        ProductCreateRequest createRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(createRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        if (updateRequest.Id == _useRequiredValue)
        {
            updateRequest.Id = (int)productId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _productService.Update(updateRequest);

        Assert.Equal(expected, updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        Product? updatedProduct = _productService.GetByIdWithProps(productId);

        List<ProductImage> productImagesForProduct = _productImageService.GetAllInProduct(productId).ToList();
        List<ProductImageFileNameInfo> productImageFileNamesForProduct = _productImageFileNameInfoService.GetAllForProduct(productId).ToList();

        Assert.NotNull(updatedProduct);

        if (expected)
        {
            AssertProductIsEqualToRequestWithoutPropsOrImages(updatedProduct, updateRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(updateRequest.Properties, updatedProduct.Properties));
            Assert.True(CompareImagesInRequestAndProduct(updateRequest.Images, productImagesForProduct));
            Assert.True(CompareUpdateRequestDataWithActualUpdatedData(createRequest.ImageFileNames, updateRequest.ImageFileNames, productImageFileNamesForProduct));
        }
        else
        {
            AssertProductIsEqualToRequestWithoutPropsOrImages(updatedProduct, createRequest);

            Assert.True(ComparePropertiesInRequestAndProduct(createRequest.Properties, updatedProduct.Properties));
            Assert.True(CompareImagesInRequestAndProduct(createRequest.Images, productImagesForProduct));
            Assert.True(CompareImageFileNamesInRequestAndProduct(createRequest.ImageFileNames, productImageFileNamesForProduct));
        }

        // Deterministic delete
        _productService.DeleteProducts(productId);
    }

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static List<object[]> Update_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            GetValidProductUpdateRequest(_useRequiredValue),
            true
        },

        
    };
#pragma warning restore CA2211 // Non-constant fields should not be visible

    [Fact]
    public void Delete_ShouldSucceed_WhenInsertIsValid()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        bool? success = _productService.Delete(productId);

        Product? insertedProduct = _productService.GetByIdWithFirstImage(productId);

        Assert.Null(insertedProduct);

        // Deterministic delete (in case delete in test fails)
        _productService.DeleteProducts(productId);
    }

    [Fact]
    public void Delete_ShouldFail_WhenIdDoesNotExist()
    {
        ProductCreateRequest validCreateRequest = GetValidProductCreateRequest();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = insertResult.AsT0;

        bool? success = _productService.Delete(0);

        Product? insertedProduct = _productService.GetByIdWithFirstImage(productId);

        Assert.NotNull(insertedProduct);

        // Deterministic delete
        _productService.DeleteProducts(productId);
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
        Assert.Equal(createRequest.Promotionid, insertedProduct.Promotionid);
        Assert.Equal(createRequest.PromRid, insertedProduct.PromRid);
        Assert.Equal(createRequest.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(createRequest.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(createRequest.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(createRequest.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(createRequest.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(createRequest.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(createRequest.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(createRequest.SearchString, insertedProduct.SearchString);

        Assert.Equal(createRequest.CategoryID, insertedProduct.CategoryID);
        Assert.Equal(createRequest.ManifacturerId, insertedProduct.ManifacturerId);
        Assert.Equal(createRequest.SubCategoryId, insertedProduct.SubCategoryId);
    }

    private static void AssertProductIsEqualToRequestWithoutPropsOrImages(Product insertedProduct, ProductUpdateRequest updateRequest)
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
        Assert.Equal(updateRequest.Promotionid, insertedProduct.Promotionid);
        Assert.Equal(updateRequest.PromRid, insertedProduct.PromRid);
        Assert.Equal(updateRequest.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(updateRequest.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(updateRequest.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(updateRequest.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(updateRequest.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(updateRequest.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(updateRequest.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(updateRequest.SearchString, insertedProduct.SearchString);

        Assert.Equal(updateRequest.CategoryID, insertedProduct.CategoryID);
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
        Assert.Equal(product.Promotionid, insertedProduct.Promotionid);
        Assert.Equal(product.PromRid, insertedProduct.PromRid);
        Assert.Equal(product.PromotionPictureId, insertedProduct.PromotionPictureId);
        Assert.Equal(product.PromotionExpireDate, insertedProduct.PromotionExpireDate);
        Assert.Equal(product.AlertPictureId, insertedProduct.AlertPictureId);
        Assert.Equal(product.AlertExpireDate, insertedProduct.AlertExpireDate);
        Assert.Equal(product.PriceListDescription, insertedProduct.PriceListDescription);
        Assert.Equal(product.PartNumber1, insertedProduct.PartNumber1);
        Assert.Equal(product.PartNumber2, insertedProduct.PartNumber2);
        Assert.Equal(product.SearchString, insertedProduct.SearchString);

        Assert.Equal(product.CategoryID, insertedProduct.CategoryID);
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
            && createRequest.Promotionid == insertedProduct.Promotionid
            && createRequest.PromRid == insertedProduct.PromRid
            && createRequest.PromotionPictureId == insertedProduct.PromotionPictureId
            && createRequest.PromotionExpireDate == insertedProduct.PromotionExpireDate
            && createRequest.AlertPictureId == insertedProduct.AlertPictureId
            && createRequest.AlertExpireDate == insertedProduct.AlertExpireDate
            && createRequest.PriceListDescription == insertedProduct.PriceListDescription
            && createRequest.PartNumber1 == insertedProduct.PartNumber1
            && createRequest.PartNumber2 == insertedProduct.PartNumber2
            && createRequest.SearchString == insertedProduct.SearchString

            && createRequest.CategoryID == insertedProduct.CategoryID
            && createRequest.ManifacturerId == insertedProduct.ManifacturerId
            && createRequest.SubCategoryId == insertedProduct.SubCategoryId);
    }

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
        && updateRequest.Promotionid == insertedProduct.Promotionid
        && updateRequest.PromRid == insertedProduct.PromRid
        && updateRequest.PromotionPictureId == insertedProduct.PromotionPictureId
        && updateRequest.PromotionExpireDate == insertedProduct.PromotionExpireDate
        && updateRequest.AlertPictureId == insertedProduct.AlertPictureId
        && updateRequest.AlertExpireDate == insertedProduct.AlertExpireDate
        && updateRequest.PriceListDescription == insertedProduct.PriceListDescription
        && updateRequest.PartNumber1 == insertedProduct.PartNumber1
        && updateRequest.PartNumber2 == insertedProduct.PartNumber2
        && updateRequest.SearchString == insertedProduct.SearchString

        && updateRequest.CategoryID == insertedProduct.CategoryID
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
        && product.Promotionid == insertedProduct.Promotionid
        && product.PromRid == insertedProduct.PromRid
        && product.PromotionPictureId == insertedProduct.PromotionPictureId
        && product.PromotionExpireDate == insertedProduct.PromotionExpireDate
        && product.AlertPictureId == insertedProduct.AlertPictureId
        && product.AlertExpireDate == insertedProduct.AlertExpireDate
        && product.PriceListDescription == insertedProduct.PriceListDescription
        && product.PartNumber1 == insertedProduct.PartNumber1
        && product.PartNumber2 == insertedProduct.PartNumber2
        && product.SearchString == insertedProduct.SearchString

        && product.CategoryID == insertedProduct.CategoryID
        && product.ManifacturerId == insertedProduct.ManifacturerId
        && product.SubCategoryId == insertedProduct.SubCategoryId);
    }

    private static bool ComparePropertiesInRequestAndProduct(List<CurrentProductPropertyCreateRequest>? propsInRequest, List<ProductProperty>? propsInObject)
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

            if (propInRequest.ProductCharacteristicId != propInObject.ProductCharacteristicId
                || propInRequest.Value != propInObject.Value
                || propInRequest.DisplayOrder != propInObject.DisplayOrder
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

        if (imagesInRequest.Count != imagesInObject.Count) return false;

        for (int i = 0; i < imagesInRequest.Count; i++)
        {
            CurrentProductImageCreateRequest imageInRequest = imagesInRequest[i];

            bool isMatched = false;

            for (int j = 0; j < imagesInObject.Count; j++)
            {
                ProductImage imageInObject = imagesInObject[j];

                if (CompareDataInByteArrays(imageInRequest.ImageData, imageInObject.ImageData)
                    && imageInRequest.ImageFileExtension == imageInObject.ImageFileExtension
                    && imageInRequest.XML == imageInObject.XML)
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
                || imageFileNameInRequest.DisplayOrder != imageFileNameInObject.DisplayOrder)
            {
                return false;
            }
        }

        return true;
    }

    private static bool ComparePropertiesInRequestAndProduct(List<CurrentProductPropertyUpdateRequest>? propsInRequest, List<ProductProperty>? propsInObject)
    {
        if (propsInRequest is null && propsInObject is null) return true;

        if (propsInRequest is null || propsInObject is null) return false;

        if (propsInRequest.Count != propsInObject.Count) return false;

        List<CurrentProductPropertyUpdateRequest> orderedPropsInRequest = propsInRequest.OrderBy(x => x.ProductCharacteristicId).ToList();
        List<ProductProperty> orderedPropsInObject = propsInObject.OrderBy(x => x.ProductCharacteristicId).ToList();

        for (int i = 0; i < orderedPropsInRequest.Count; i++)
        {
            CurrentProductPropertyUpdateRequest propInRequest = orderedPropsInRequest[i];
            ProductProperty propInObject = orderedPropsInObject[i];

            if (propInRequest.ProductCharacteristicId != propInObject.ProductCharacteristicId
                || propInRequest.Value != propInObject.Value
                || propInRequest.DisplayOrder != propInObject.DisplayOrder
                || propInRequest.XmlPlacement != propInObject.XmlPlacement)
            {
                return false;
            }
        }

        return true;
    }

    private static bool CompareImagesInRequestAndProduct(List<CurrentProductImageUpdateRequest>? imagesInRequest, List<ProductImage>? imagesInObject)
    {
        if (imagesInRequest is null && imagesInObject is null) return true;

        if (imagesInRequest is null || imagesInObject is null) return false;

        if (imagesInRequest.Count != imagesInObject.Count) return false;

        for (int i = 0; i < imagesInRequest.Count; i++)
        {
            CurrentProductImageUpdateRequest imageInRequest = imagesInRequest[i];

            bool isMatched = false;

            for (int j = 0; j < imagesInObject.Count; j++)
            {
                ProductImage imageInObject = imagesInObject[j];

                if (CompareDataInByteArrays(imageInRequest.ImageData, imageInObject.ImageData)
                    && imageInRequest.ImageFileExtension == imageInObject.ImageFileExtension
                    && imageInRequest.XML == imageInObject.XML)
                {
                    isMatched = true;

                    break;
                }
            }

            if (!isMatched) return false;
        }

        return true;
    }

    private static bool CompareUpdateRequestDataWithActualUpdatedData(List<CurrentProductImageFileNameInfoCreateRequest>? createRequestData, List<CurrentProductImageFileNameInfoUpdateRequest>? updateRequests, List<ProductImageFileNameInfo> imageFileNamesInObject)
    {
        if (updateRequests is null)
        {
            return CompareImageFileNamesInRequestAndProduct(createRequestData, imageFileNamesInObject);
        }

        if (createRequestData is null)
        {
            return imageFileNamesInObject.Count == 0;
        }

        int? maxDisplayOrder = createRequestData.Max(x => x.DisplayOrder);
        
        foreach (CurrentProductImageFileNameInfoUpdateRequest updateRequest in updateRequests)
        {
            if (updateRequest.NewDisplayOrder < 1)
            {
                updateRequest.NewDisplayOrder = 1;
            }
            else if (updateRequest.NewDisplayOrder > maxDisplayOrder)
            {
                updateRequest.NewDisplayOrder = maxDisplayOrder;
            }

            foreach (CurrentProductImageFileNameInfoCreateRequest createRequestInner in createRequestData)
            {
                if (createRequestInner.DisplayOrder == updateRequest.DisplayOrder)
                {
                    createRequestInner.DisplayOrder = updateRequest.NewDisplayOrder;
                    createRequestInner.FileName = updateRequest.FileName;
                }
                else if (updateRequest.DisplayOrder < updateRequest.NewDisplayOrder
                    && createRequestInner.DisplayOrder > updateRequest.DisplayOrder
                    && createRequestInner.DisplayOrder <= updateRequest.NewDisplayOrder)
                {
                    createRequestInner.DisplayOrder--;
                }
                else if (updateRequest.DisplayOrder > updateRequest.NewDisplayOrder
                    && createRequestInner.DisplayOrder < updateRequest.DisplayOrder
                    && createRequestInner.DisplayOrder >= updateRequest.NewDisplayOrder)
                {
                    createRequestInner.DisplayOrder++;
                }
            }
        }

        return CompareImageFileNamesInRequestAndProduct(createRequestData, imageFileNamesInObject);
    }
}