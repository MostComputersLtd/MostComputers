using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Requests.Promotion;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class PromotionServiceTests : IntegrationTestBaseForNonWebProjects
{
    public PromotionServiceTests(
        IPromotionService promotionService,
        IProductService productService)
        : base(Startup.ConnectionString)
    {
        _promotionService = promotionService;
        _productService = productService;
    }

    private readonly IPromotionService _promotionService;
    private readonly IProductService _productService;

    private const int _useRequiredValue = -100;

    [Fact]
    public void GetAll_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        IEnumerable<Promotion> promotions = _promotionService.GetAll();

        Assert.NotEmpty(promotions);

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest, x));
    }

    [Fact]
    public void GetAllActive_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = 1;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        IEnumerable<Promotion> promotions = _promotionService.GetAllActive();

        Assert.NotEmpty(promotions);

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest, x));
    }

    [Fact]
    public void GetAllForProduct_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        IEnumerable<Promotion> promotions = _promotionService.GetAllForProduct(productId);

        Assert.True(promotions.Count() == 1);

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest, x));
    }

    [Fact]
    public void GetAllForSelectionOfProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest1 = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(validProductCreateRequest1);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ProductCreateRequest validProductCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(validProductCreateRequest2);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = productInsertResult2.AsT0;

        ServicePromotionCreateRequest validCreateRequest1 = GetValidPromotionCreateRequest((int)productId1);
        ServicePromotionCreateRequest validCreateRequest2 = GetValidPromotionCreateRequest((int)productId1);
        ServicePromotionCreateRequest validCreateRequest3 = GetValidPromotionCreateRequest((int)productId2);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _promotionService.Insert(validCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result2 = _promotionService.Insert(validCreateRequest2);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result3 = _promotionService.Insert(validCreateRequest3);

        Assert.True(result1.Match(_ => true, _ => false, _ => false));
        Assert.True(result2.Match(_ => true, _ => false, _ => false));
        Assert.True(result3.Match(_ => true, _ => false, _ => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllForSelectionOfProducts(new() { productId1, productId2 });

        Assert.True(promotions.Count() == 3);

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest1, x));

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest2, x));

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest3, x));
    }

    [Fact]
    public void GetAllActiveForSelectionOfProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest1 = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult1 = _productService.Insert(validProductCreateRequest1);

        Assert.True(productInsertResult1.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId1 = productInsertResult1.AsT0;

        ProductCreateRequest validProductCreateRequest2 = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult2 = _productService.Insert(validProductCreateRequest2);

        Assert.True(productInsertResult2.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId2 = productInsertResult2.AsT0;

        ServicePromotionCreateRequest validCreateRequest1 = GetValidPromotionCreateRequest((int)productId1);

        validCreateRequest1.Active = 1;

        ServicePromotionCreateRequest validCreateRequest2 = GetValidPromotionCreateRequest((int)productId1);
        ServicePromotionCreateRequest validCreateRequest3 = GetValidPromotionCreateRequest((int)productId2);

        validCreateRequest3.Active = 1;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _promotionService.Insert(validCreateRequest1);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result2 = _promotionService.Insert(validCreateRequest2);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result3 = _promotionService.Insert(validCreateRequest3);

        Assert.True(result1.Match(_ => true, _ => false, _ => false));
        Assert.True(result2.Match(_ => true, _ => false, _ => false));
        Assert.True(result3.Match(_ => true, _ => false, _ => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllActiveForSelectionOfProducts(new() { productId1, productId2 });

        Assert.True(promotions.Count() == 2);

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest1, x));

        Assert.Contains(promotions, x =>
        ComparePromotionAndCreateRequest(validCreateRequest3, x));
    }

    [Fact]
    public void GetActiveForProduct_ShouldSucceed_WhenInsertsAreValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

       
        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = 1;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        Assert.True(result.Match(_ => true, _ => false, _ => false));

        Promotion? promotion = _promotionService.GetActiveForProduct(productId);

        Assert.NotNull(promotion);

        Assert.True(ComparePromotionAndCreateRequest(validCreateRequest, promotion));
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ServicePromotionCreateRequest createRequest, bool expected)
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        if (createRequest.ProductId == _useRequiredValue)
        {
            createRequest.ProductId = (int)productId;
        }

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(createRequest);

        Assert.Equal(expected, result.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllForProduct(productId);

        Promotion promotion = promotions.Single();

        Assert.True(ComparePromotionAndCreateRequest(createRequest, promotion));
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
                Active = 0,
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
                Active = 0,
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

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        if (updateRequest.ProductId == _useRequiredValue)
        {
            updateRequest.ProductId = (int)productId;
        }

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _promotionService.Insert(validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        OneOf<Success, ValidationResult, UnexpectedFailureResult> result = _promotionService.Update(updateRequest);

        Assert.Equal(expected, result.Match(
            _ => true,
            _ => false,
            _ => false));

        IEnumerable<Promotion> promotions = _promotionService.GetAllForProduct(productId);

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
                Name = "2023_Q4",
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = 0,
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
                Name = string.Empty,
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = 0,
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
                Name = "    ",
                Source = 1,
                Type = 2,
                Status = 3,
                SPOID = null,
                DiscountUSD = 4.99M,
                DiscountEUR = 4.99M,
                Active = 0,
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

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = 1;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        Assert.True(result.Match(
            _ => true,
            _ => false,
            _ => false));

        uint promotionId = result.AsT0;

        bool success = _promotionService.Delete(promotionId);

        Promotion? promotion = _promotionService.GetActiveForProduct(productId);

        Assert.Null(promotion);
    }

    [Fact]
    public void DeleteAllByProductId_ShouldSucceed_WhenInsertIsValid()
    {
        ProductCreateRequest validProductCreateRequest = GetValidProductCreateRequestUsingRandomData();

        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(validProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ServicePromotionCreateRequest validCreateRequest = GetValidPromotionCreateRequest((int)productId);

        validCreateRequest.Active = 1;

        OneOf<uint, ValidationResult, UnexpectedFailureResult> result = _promotionService.Insert(validCreateRequest);

        Assert.True(result.Match(
            _ => true,
            _ => false,
            _ => false));

        uint promotionId = result.AsT0;

        bool success = _promotionService.DeleteAllByProductId(promotionId);

        Promotion? promotion = _promotionService.GetActiveForProduct(productId);

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
}