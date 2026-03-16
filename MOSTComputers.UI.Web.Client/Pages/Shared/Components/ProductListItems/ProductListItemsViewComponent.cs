using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.Currencies;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.Currencies.Models;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.UI.Web.Client.Endpoints.Images;
using OneOf;
using OneOf.Types;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductListItems;

[ViewComponent(Name = ComponentName)]
public class ProductListItemsViewComponent : ViewComponent
{
    internal const string ComponentName = "ProductListItems";

    private readonly IProductImageFileDataRepository _productImageFileDataRepository;
    private readonly IPromotionService _promotionService;
    private readonly ICurrencyConversionService _currencyConversionService;
    private readonly ICurrencyVATService _currencyVATService;

    public ProductListItemsViewComponent(
        IProductImageFileDataRepository productImageFileDataRepository,
        IPromotionService promotionService,
        ICurrencyConversionService currencyConversionService,
        ICurrencyVATService currencyVATService)
    {
        _productImageFileDataRepository = productImageFileDataRepository;
        _promotionService = promotionService;
        _currencyConversionService = currencyConversionService;
        _currencyVATService = currencyVATService;
    }

    public async Task<IViewComponentResult> InvokeAsync(List<Product> products, Currency currency, Currency? secondaryCurrency = null)
    {
        if (products.Count == 0)
        {
            ProductListItemsModel emptyModel = new()
            {
                Products = new()
            };

            return View("Default", emptyModel);
        }

        List<ProductListItemsModel.ProductDisplayData> productDisplayDatas
            = await GetDisplayDataFromProductsAsync(products, currency, secondaryCurrency);

        ProductListItemsModel model = new()
        {
            Products = productDisplayDatas
        };

        return View("Default", model);
    }

    private async Task<List<ProductListItemsModel.ProductDisplayData>> GetDisplayDataFromProductsAsync(
        List<Product> products,
        Currency currency,
        Currency? secondaryCurrency)
    {
        List<ProductListItemsModel.ProductDisplayData> productDisplayDatas = new();

        IEnumerable<int> productIds = products.Select(x => x.Id);

        List<IGrouping<int, ProductImageFileData>> productImageFiles = await _productImageFileDataRepository.GetAllInProductsAsync(productIds);

        Dictionary<Product, ChangeCurrencyRequest> currencyChangeRequests = products
            .Where(productData => productData.Price.HasValue && productData.Currency.HasValue)
            .ToDictionary(product => product, productData =>
            {
                return new ChangeCurrencyRequest()
                {
                    Value = productData.Price!.Value,
                    CurrentCurrency = productData.Currency!.Value,
                    NewCurrency = currency
                };
            });

        Dictionary<Product, ChangeCurrencyRequest>? secondaryChangeRequests = null;

        if (secondaryCurrency is not null)
        {
            secondaryChangeRequests = products
                .Where(productData => productData.Price.HasValue && productData.Currency.HasValue)
                .ToDictionary(product => product, productData =>
                {
                    return new ChangeCurrencyRequest()
                    {
                        Value = productData.Price!.Value,
                        CurrentCurrency = productData.Currency!.Value,
                        NewCurrency = secondaryCurrency.Value,
                    };
                });
        }

        Dictionary<ChangeCurrencyRequest, OneOf<decimal, NotFound>> currencyChangeResults
            = await _currencyConversionService.ChangeCurrenciesAsync(currencyChangeRequests.Values.ToList());

        Dictionary<ChangeCurrencyRequest, OneOf<decimal, NotFound>>? secondaryCurrencyChangeResults = null;

        List<IGrouping<int?, Promotion>> promotionsForProducts = await _promotionService.GetAllForSelectionOfProductsAsync(productIds);

        Dictionary<int, List<Promotion>> promotionsByProductId = promotionsForProducts
            .Where(group => group.Key.HasValue)
            .ToDictionary(group => group.Key!.Value, group => group.ToList());

        if (secondaryChangeRequests is not null)
        {
            secondaryCurrencyChangeResults = await _currencyConversionService.ChangeCurrenciesAsync(secondaryChangeRequests.Values.ToList());
        }

        foreach (Product product in products)
        {
            string? firstImageUrl = null;

            IGrouping<int, ProductImageFileData>? productFiles = productImageFiles.Find(x => x.Key == product.Id);

            ProductImageFileData? firstFileWithName = productFiles?
                .Where(x => x.FileName is not null)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefault();

            if (firstFileWithName is not null)
            {
                firstImageUrl = CombinePathsWithSeparator('/', ProductImageFileDataEndpoints.EndpointGroupRoute, firstFileWithName.FileName ?? string.Empty);
            }

            bool hasPromotion = DoesProductHaveAnyPromotion(promotionsByProductId, product);

            ProductListItemsModel.PriceDisplayData? priceDisplayData = GetProductDisplayPriceData(
                currency, currencyChangeRequests, currencyChangeResults, product);

            ProductListItemsModel.PriceDisplayData? secondaryPriceDisplayData = GetProductDisplayPriceData(
                secondaryCurrency, secondaryChangeRequests, secondaryCurrencyChangeResults, product);

            ProductListItemsModel.ProductDisplayData productDisplayData = new()
            {
                Product = product,
                NeedsPromotionalBanner = hasPromotion,
                ProductImageUrl = firstImageUrl,
                PriceDisplayData = priceDisplayData,
                SecondaryPriceDisplayData = secondaryPriceDisplayData,
            };

            productDisplayDatas.Add(productDisplayData);
        }

        return productDisplayDatas;
    }

    private ProductListItemsModel.PriceDisplayData? GetProductDisplayPriceData(
        Currency? currency,
        Dictionary<Product, ChangeCurrencyRequest>? currencyChangeRequests,
        Dictionary<ChangeCurrencyRequest, OneOf<decimal, NotFound>>? currencyChangeResults,
        Product product)
    {
        if (currency == null || currencyChangeRequests == null || currencyChangeResults == null) return null;

        decimal? displayPrice = null;

        if (product.Price is not null && product.Currency is not null)
        {
            OneOf<decimal, NotFound>? getPriceResult = currencyChangeResults.TryGetValue(
                currencyChangeRequests[product], out OneOf<decimal, NotFound> result)
                    ? result
                    : null;

            if (getPriceResult is not null && getPriceResult.Value.IsT0)
            {
                decimal priceWithVat = _currencyVATService.AddVATToPriceUsingDefaultRate(getPriceResult.Value.AsT0, 1);

                displayPrice = priceWithVat;
            }
        }

        ProductListItemsModel.PriceDisplayData? priceDisplayData = null;

        if (displayPrice.HasValue)
        {
            priceDisplayData = new()
            {
                Amount = displayPrice.Value,
                Currency = currency.Value,
            };
        }

        return priceDisplayData;
    }

    private static bool DoesProductHaveAnyPromotion(Dictionary<int, List<Promotion>> promotionsByProductId, Product product)
    {
        bool isInfoPromotionActive = product.AlertExpireDate is null || product.AlertExpireDate >= DateTime.Now;

        int? promotionPictureId = product.AlertPictureId;

        if (promotionPictureId is null || promotionPictureId <= 0)
        {
            promotionPictureId = product.PromotionPictureId;
        }

        bool hasPromotion = isInfoPromotionActive && promotionPictureId > 0;

        if (hasPromotion) return true;
        
        if (product.PromotionPid > 0 || product.PromotionRid > 0)
        {
            if (promotionsByProductId.TryGetValue(product.Id, out List<Promotion>? promotions))
            {
                foreach (Promotion? promotion in promotions)
                {
                    if ((promotion.Id == product.PromotionPid || promotion.Id == product.PromotionRid)
                        && IsPromotionActive(promotion))
                    {
                        hasPromotion = true;

                        break;
                    }
                }
            }
        }

        return hasPromotion;
    }

    private static bool IsPromotionActive(Promotion promotion)
    {
        return (promotion.StartDate is null || promotion.StartDate <= DateTime.Now)
            && (promotion.ExpirationDate is null || promotion.ExpirationDate >= DateTime.Now);
    }
}