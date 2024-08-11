using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class PromotionServiceTests : IntegrationTestBaseForNonWebProjects
{
    public PromotionServiceTests(
        IPromotionService promotionService,
        IProductService productService)
        : base(Startup.ConnectionString, Startup.RespawnerOptionsToIgnoreTablesThatShouldntBeWiped)
    {
        _promotionService = promotionService;
        _productService = productService;
    }

    private readonly IPromotionService _promotionService;
    private readonly IProductService _productService;

    private const int _useRequiredValue = -100;

    private readonly List<int> _promotionsToRemoveProductIds = new();

    private void SchedulePromotionsForDeleteByProductIdAfterTest(params int[] ids)
    {
        _promotionsToRemoveProductIds.AddRange(ids);
    }

    public override async Task DisposeAsync()
    {
        await ResetDatabaseAsync();

        DeleteRangeByProductIds(_promotionsToRemoveProductIds.ToArray());
    }

    [Fact]
    public void GetAll_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        IEnumerable<Promotion> promotions = _promotionService.GetAll();

        Assert.NotEmpty(promotions);

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest, x));
    }

    [Fact]
    public void GetAll_ShouldFail_WhenInsertsAreInvalid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest invalidCreateRequest = GetValidPromotionCreateRequest((int)productId);

        invalidCreateRequest.Name = "    ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(invalidCreateRequest);

        Assert.True(result.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAll();

        Assert.DoesNotContain(promotions, x =>
            ComparePromotionAndCreateRequest(invalidCreateRequest, x));
    }

    [Fact]
    public void GetAllActive_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = true;

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        Assert.True(result.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllActive();

        Assert.NotEmpty(promotions);

        Assert.Contains(promotions, x =>
                ComparePromotionAndCreateRequest(validCreateRequest, x));
    }

    [Fact]
    public void GetAllActive_ShouldFail_WhenInsertsAreInvalid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest invalidCreateRequest = GetValidPromotionCreateRequest((int)productId);

        invalidCreateRequest.Active = true;
        invalidCreateRequest.Name = "   ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(invalidCreateRequest);

        Assert.True(result.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllActive();

        Assert.DoesNotContain(promotions, x =>
            ComparePromotionAndCreateRequest(invalidCreateRequest, x));
    }

    [Fact]
    public void GetAllForProduct_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        Assert.True(result.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllForProduct(productId.Value);

        Assert.Single(promotions);

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest, x));
    }

    [Fact]
    public void GetAllForProduct_ShouldFail_WhenInsertsAreInvalid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest invalidCreateRequest = GetValidPromotionCreateRequest((int)productId);

        invalidCreateRequest.Name = "   ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(invalidCreateRequest);

        Assert.True(result.Match(
            id => false,
            validationResult => true, 
            unexpectedFailureResult => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllForProduct((int)productId.Value);

        Assert.Empty(promotions);
    }

    [Fact]
    public void GetAllForSelectionOfProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest1 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(validProductCreateRequest1);

        int? productId1 = productInsertResult1.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        ProductCreateRequest validProductCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(validProductCreateRequest2);

        int? productId2 = productInsertResult2.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        ServicePromotionCreateRequest validCreateRequest1 = GetValidPromotionCreateRequest((int)productId1);
        ServicePromotionCreateRequest validCreateRequest2 = GetValidPromotionCreateRequest((int)productId1);
        ServicePromotionCreateRequest validCreateRequest3 = GetValidPromotionCreateRequest((int)productId2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> result1 = _promotionService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result2 = _promotionService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result3 = _promotionService.Insert(validCreateRequest3);

        Assert.True(result1.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        Assert.True(result2.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        Assert.True(result3.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        List<int> productIds = new() { productId1.Value, productId2.Value };

        IEnumerable<Promotion> promotions = _promotionService.GetAllForSelectionOfProducts(productIds);

        Assert.True(promotions.Count() == 3);

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest1, x));

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest2, x));

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest3, x));
    }

    [Fact]
    public void GetAllForSelectionOfProducts_ShouldOnlyGetTheDataWhichWasSuccessfullyInserted()
    {
        ProductCreateRequest validProductCreateRequest1 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(validProductCreateRequest1);

        int? productId1 = productInsertResult1.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        ProductCreateRequest validProductCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(validProductCreateRequest2);

        int? productId2 = productInsertResult2.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        ServicePromotionCreateRequest validCreateRequest1 = GetValidPromotionCreateRequest((int)productId1);
        ServicePromotionCreateRequest invalidCreateRequest2 = GetValidPromotionCreateRequest((int)productId1);

        invalidCreateRequest2.Name = "  ";

        ServicePromotionCreateRequest validCreateRequest3 = GetValidPromotionCreateRequest((int)productId2);

        OneOf<int, ValidationResult, UnexpectedFailureResult> result1 = _promotionService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result2 = _promotionService.Insert(invalidCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result3 = _promotionService.Insert(validCreateRequest3);

        Assert.True(result1.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        Assert.True(result2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.True(result3.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        List<int> productIds = new() { productId1.Value, productId2.Value };

        IEnumerable<Promotion> promotions = _promotionService.GetAllForSelectionOfProducts(productIds);

        Assert.Equal(2, promotions.Count());

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest1, x));

        Assert.DoesNotContain(promotions, x =>
            ComparePromotionAndCreateRequest(invalidCreateRequest2, x));

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest3, x));
    }

    [Fact]
    public void GetAllActiveForSelectionOfProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest1 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(validProductCreateRequest1);

        int? productId1 = productInsertResult1.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        ProductCreateRequest validProductCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(validProductCreateRequest2);

        int? productId2 = productInsertResult2.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        ServicePromotionCreateRequest validCreateRequest1 = GetValidPromotionCreateRequest((int)productId1);

        validCreateRequest1.Active = true;

        ServicePromotionCreateRequest validCreateRequest2 = GetValidPromotionCreateRequest((int)productId1);
        ServicePromotionCreateRequest validCreateRequest3 = GetValidPromotionCreateRequest((int)productId2);

        validCreateRequest3.Active = true;

        OneOf<int, ValidationResult, UnexpectedFailureResult> result1 = _promotionService.Insert(validCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result2 = _promotionService.Insert(validCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result3 = _promotionService.Insert(validCreateRequest3);

        Assert.True(result1.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        Assert.True(result2.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        Assert.True(result3.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        List<int> productIds = new() { productId1.Value, productId2.Value };

        IEnumerable<Promotion> promotions = _promotionService.GetAllActiveForSelectionOfProducts(productIds);

        Assert.Equal(2, promotions.Count());

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest1, x));

        Assert.Contains(promotions, x =>
            ComparePromotionAndCreateRequest(validCreateRequest3, x));
    }

    [Fact]
    public void GetAllActiveForSelectionOfProducts_ShouldFail_WhenInsertsAreInvalid()
    {
        ProductCreateRequest validProductCreateRequest1 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(validProductCreateRequest1);

        int? productId1 = productInsertResult1.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        ProductCreateRequest validProductCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(validProductCreateRequest2);

        int? productId2 = productInsertResult2.Match<int?>(
             id =>
             {
                 SchedulePromotionsForDeleteByProductIdAfterTest(id);

                 return id;
             },
             validationResult => null,
             unexpectedFailureResult => null);

        Assert.NotNull(productId1);
        Assert.True(productId1 > 0);

        Assert.NotNull(productId2);
        Assert.True(productId2 > 0);

        ServicePromotionCreateRequest invalidCreateRequest1 = GetValidPromotionCreateRequest((int)productId1);

        invalidCreateRequest1.Active = true;
        invalidCreateRequest1.Name = "   ";

        ServicePromotionCreateRequest invalidCreateRequest2 = GetValidPromotionCreateRequest((int)productId1);

        invalidCreateRequest2.Name = "   ";

        ServicePromotionCreateRequest invalidCreateRequest3 = GetValidPromotionCreateRequest((int)productId2);

        invalidCreateRequest3.Active = true;
        invalidCreateRequest3.Name = "   ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> result1 = _promotionService.Insert(invalidCreateRequest1);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result2 = _promotionService.Insert(invalidCreateRequest2);
        OneOf<int, ValidationResult, UnexpectedFailureResult> result3 = _promotionService.Insert(invalidCreateRequest3);

        Assert.True(result1.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.True(result2.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Assert.True(result3.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        List<int> productIds = new() { productId1.Value, productId2.Value };

        IEnumerable<Promotion> promotions = _promotionService.GetAllActiveForSelectionOfProducts(productIds);

        Assert.Empty(promotions);
    }

    [Fact]
    public void GetActiveForProduct_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = true;

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        Assert.True(result.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        Promotion? promotion = _promotionService.GetActiveForProduct(productId.Value);

        Assert.NotNull(promotion);

        Assert.True(ComparePromotionAndCreateRequest(validCreateRequest, promotion));
    }

    [Fact]
    public void GetActiveForProduct_ShouldFail_WhenInsertsAreInvalid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest invalidCreateRequest = GetValidPromotionCreateRequest((int)productId);

        invalidCreateRequest.Active = true;
        invalidCreateRequest.Name = "   ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(invalidCreateRequest);

        Assert.True(result.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        Promotion? promotion = _promotionService.GetActiveForProduct(productId.Value);

        Assert.Null(promotion);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ServicePromotionCreateRequest createRequest, bool expected)
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId;
        }

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(createRequest);

        Assert.Equal(expected, result.Match(
            id => id > 0,
            _ => false,
            _ => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllForProduct(productId.Value);

        if (expected)
        {
            Promotion promotion = promotions.Single();

            Assert.True(ComparePromotionAndCreateRequest(createRequest, promotion));
        }
        else
        {
            Assert.Empty(promotions);
        }
    }

    public static readonly List<object[]> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            GetValidPromotionCreateRequest(_useRequiredValue),
            true
        },

        new object[2]
        {
            new ServicePromotionCreateRequest()
            {
                Name = string.Empty,
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = false,
                StartDate = DateTime.Today.AddDays(-5),
                ExpirationDate = DateTime.Today.AddDays(7),
                MinimumQuantity = 2,
                MaximumQuantity = 120,
                QuantityIncrement = 123,

                RequiredProductIds = null,

                ExpQuantity = null,
                SoldQuantity = 1782,
                Consignation = 1,
                Points = 2,
                TimeStamp = null,

                ProductId = _useRequiredValue,
                CampaignId = null,
                RegistrationId = null,
                PromotionVisualizationId = null,
            },
            false
        },

        new object[2]
        {
            new ServicePromotionCreateRequest()
            {
                Name = "    ",
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = false,
                StartDate = DateTime.Today.AddDays(-5),
                ExpirationDate = DateTime.Today.AddDays(7),
                MinimumQuantity = 2,
                MaximumQuantity = 120,
                QuantityIncrement = 123,

                RequiredProductIds = null,

                ExpQuantity = null,
                SoldQuantity = 1782,
                Consignation = 1,
                Points = 2,
                TimeStamp = null,

                ProductId = _useRequiredValue,
                CampaignId = null,
                RegistrationId = null,
                PromotionVisualizationId = null,
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(Update_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Update_ShouldSucceedOrFail_InAnExpectedManner(ServicePromotionUpdateRequest updateRequest, bool expected)
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        if (updateRequest.ProductId == _useRequiredValue)
        {
            updateRequest.ProductId = (int)productId;
        }

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        OneOf<int, ValidationResult, UnexpectedFailureResult> insertResult = _promotionService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            id => id > 0,
            validationResult => false,
            unexpectedFailureResult => false));

        int promotionId = insertResult.AsT0;

        if (updateRequest.Id == _useRequiredValue)
        {
            updateRequest.Id = promotionId;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _promotionService.Update(updateRequest);

        Assert.Equal(expected, result.Match(
            success => true,
            validationResult => false,
            unexpectedFailureResult => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllForProduct(productId.Value);

        Promotion promotion = promotions.Single();

        if (expected)
        {
            Assert.True(ComparePromotionAndUpdateRequest(updateRequest, promotion));
        }
        else
        {
            Assert.True(ComparePromotionAndCreateRequest(validCreateRequest, promotion));
        }
    }

    public static readonly List<object[]> Update_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            new ServicePromotionUpdateRequest()
            {
                Id = _useRequiredValue,
                Name = "2023_Q4",
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = false,
                StartDate = DateTime.Today.AddDays(-5),
                ExpirationDate = DateTime.Today.AddDays(7),
                MinimumQuantity = 2,
                MaximumQuantity = 120,
                QuantityIncrement = 123,

                RequiredProductIds = null,

                ExpQuantity = null,
                SoldQuantity = 1782,
                Consignation = 1,
                Points = 2,
                TimeStamp = null,

                ProductId = _useRequiredValue,
                CampaignId = null,
                RegistrationId = null,
                PromotionVisualizationId = null,
            },

            true
        },

        new object[2]
        {
            new ServicePromotionUpdateRequest()
            {
                Id = _useRequiredValue,
                Name = string.Empty,
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = false,
                StartDate = DateTime.Today.AddDays(-5),
                ExpirationDate = DateTime.Today.AddDays(7),
                MinimumQuantity = 2,
                MaximumQuantity = 120,
                QuantityIncrement = 123,

                RequiredProductIds = null,

                ExpQuantity = null,
                SoldQuantity = 1782,
                Consignation = 1,
                Points = 2,
                TimeStamp = null,

                ProductId = _useRequiredValue,
                CampaignId = null,
                RegistrationId = null,
                PromotionVisualizationId = null,
            },

            false
        },

        new object[2]
        {
            new ServicePromotionUpdateRequest()
            {
                Id = _useRequiredValue,
                Name = "    ",
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = false,
                StartDate = DateTime.Today.AddDays(-5),
                ExpirationDate = DateTime.Today.AddDays(7),
                MinimumQuantity = 2,
                MaximumQuantity = 120,
                QuantityIncrement = 123,

                RequiredProductIds = null,

                ExpQuantity = null,
                SoldQuantity = 1782,
                Consignation = 1,
                Points = 2,
                TimeStamp = null,

                ProductId = _useRequiredValue,
                CampaignId = null,
                RegistrationId = null,
                PromotionVisualizationId = null,
            },

            false
        },
    };

    [Fact]
    public void Delete_ShouldSucceed_WhenInsertIsValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = true;

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        int? promotionId = result.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(promotionId);
        Assert.True(promotionId > 0);

        bool success = _promotionService.Delete(promotionId.Value);

        Promotion? promotion = _promotionService.GetActiveForProduct((int)productId.Value);

        Assert.Null(promotion);
    }

    [Fact]
    public void Delete_ShouldFail_WhenIdIsInvalid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = true;

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        int? promotionId = result.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(promotionId);
        Assert.True(promotionId > 0);

        bool success = _promotionService.Delete(0);

        Promotion? promotion = _promotionService.GetActiveForProduct((int)productId.Value);

        Assert.NotNull(promotion);
    }

    [Fact]
    public void DeleteAllByProductId_ShouldSucceed_WhenInsertIsValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = true;

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        int? promotionId = result.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        bool success = _promotionService.DeleteAllByProductId(productId.Value);

        Promotion? promotion = _promotionService.GetActiveForProduct(productId.Value);

        Assert.Null(promotion);
    }

    [Fact]
    public void DeleteAllByProductId_ShouldFail_WhenInsertIsInvalid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<int, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        int? productId = productInsertResult.Match<int?>(
            id =>
            {
                SchedulePromotionsForDeleteByProductIdAfterTest(id);

                return id;
            },
            validationResult => null,
            unexpectedFailureResult => null);

        Assert.NotNull(productId);
        Assert.True(productId > 0);

        ServicePromotionCreateRequest invalidCreateRequest = GetValidPromotionCreateRequest((int)productId);

        invalidCreateRequest.Active = true;
        invalidCreateRequest.Name = "   ";

        OneOf<int, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(invalidCreateRequest);

        Assert.True(result.Match(
            id => false,
            validationResult => true,
            unexpectedFailureResult => false));

        bool success = _promotionService.DeleteAllByProductId(productId.Value);

        Promotion? promotion = _promotionService.GetActiveForProduct(productId.Value);

        Assert.Null(promotion);
    }

    private static bool ComparePromotionAndCreateRequest(ServicePromotionCreateRequest createRequest, Promotion promotion)
    {
        return (createRequest.Name == promotion.Name
        && createRequest.Source == promotion.Source
        && createRequest.Type == promotion.Type
        && createRequest.Status == promotion.Status
        && createRequest.SPOID == promotion.SPOID
        && createRequest.DiscountUSD == promotion.DiscountUSD
        && createRequest.DiscountEUR == promotion.DiscountEUR
        && createRequest.Active == promotion.Active
        && createRequest.StartDate == promotion.StartDate
        && createRequest.ExpirationDate == promotion.ExpirationDate
        && createRequest.MinimumQuantity == promotion.MinimumQuantity
        && createRequest.MaximumQuantity == promotion.MaximumQuantity
        && createRequest.QuantityIncrement == promotion.QuantityIncrement

        && ComparaNumberLists(createRequest.RequiredProductIds, promotion.RequiredProductIds)

        && createRequest.ExpQuantity == promotion.ExpQuantity
        && createRequest.SoldQuantity == promotion.SoldQuantity
        && createRequest.Consignation == promotion.Consignation
        && createRequest.Points == promotion.Points
        && createRequest.TimeStamp == promotion.TimeStamp

        && createRequest.ProductId == promotion.ProductId
        && createRequest.CampaignId == promotion.CampaignId
        && createRequest.RegistrationId == promotion.RegistrationId
        && createRequest.PromotionVisualizationId == promotion.PromotionVisualizationId);
    }

    private static bool ComparePromotionAndUpdateRequest(ServicePromotionUpdateRequest updateRequest, Promotion promotion)
    {
        return (updateRequest.Id == promotion.Id
        && updateRequest.Name == promotion.Name
        && updateRequest.Source == promotion.Source
        && updateRequest.Type == promotion.Type
        && updateRequest.Status == promotion.Status
        && updateRequest.SPOID == promotion.SPOID
        && updateRequest.DiscountUSD == promotion.DiscountUSD
        && updateRequest.DiscountEUR == promotion.DiscountEUR
        && updateRequest.Active == promotion.Active
        && updateRequest.StartDate == promotion.StartDate
        && updateRequest.ExpirationDate == promotion.ExpirationDate
        && updateRequest.MinimumQuantity == promotion.MinimumQuantity
        && updateRequest.MaximumQuantity == promotion.MaximumQuantity
        && updateRequest.QuantityIncrement == promotion.QuantityIncrement

        && ComparaNumberLists(updateRequest.RequiredProductIds, promotion.RequiredProductIds)

        && updateRequest.ExpQuantity == promotion.ExpQuantity
        && updateRequest.SoldQuantity == promotion.SoldQuantity
        && updateRequest.Consignation == promotion.Consignation
        && updateRequest.Points == promotion.Points
        && updateRequest.TimeStamp == promotion.TimeStamp

        && updateRequest.ProductId == promotion.ProductId
        && updateRequest.CampaignId == promotion.CampaignId
        && updateRequest.RegistrationId == promotion.RegistrationId
        && updateRequest.PromotionVisualizationId == promotion.PromotionVisualizationId);
    }

    private bool DeleteRangeByProductIds(params int[] productIds)
    {
        foreach (int productId in productIds)
        {
            bool success = _promotionService.DeleteAllByProductId(productId);

            if (!success) return false;
        }

        return true;
    }
}