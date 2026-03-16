using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.Currencies.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Client.Endpoints.Images;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData.DefaultModel;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData;

[ViewComponent(Name = ComponentName)]
public sealed class ProductDataViewComponent : ViewComponent
{
    public class ProductDataExistingData
    {
        public Product? Product { get; set; }
        public ProductPriceData? PriceData { get; set; }

        public List<ProductPropertyDisplayData>? ProductProperties { get; set; }
        public List<ProductImageDisplayData>? ProductImages { get; set; }
        public List<SearchStringPartOriginData>? ProductSearchStringParts { get; set; }
        public List<Promotion>? ProductPromotions { get; set; }
    }

    internal const string ComponentName = "ProductData";

    private readonly IProductPropertyCrudService _productPropertyCrudService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IProductImageFileDataRepository _productImageFileDataRepository;
    private readonly IPromotionService _promotionService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly ICurrencyConversionService _currencyConversionService;
    private readonly ICurrencyVATService _currencyVATService;

    public ProductDataViewComponent(
        IProductPropertyCrudService productPropertyService,
        IProductCharacteristicService productCharacteristicService,
        IProductImageFileDataRepository productImageFileDataRepository,
        IPromotionService promotionService,
        ISearchStringOriginService searchStringOriginService,
        ICurrencyConversionService currencyConversionService,
        ICurrencyVATService currencyVATService)
    {
        _productPropertyCrudService = productPropertyService;
        _productCharacteristicService = productCharacteristicService;
        _productImageFileDataRepository = productImageFileDataRepository;
        _promotionService = promotionService;
        _searchStringOriginService = searchStringOriginService;
        _currencyConversionService = currencyConversionService;
        _currencyVATService = currencyVATService;
    }

    public async Task<IViewComponentResult> InvokeAsync(ProductDataExistingData existingData, bool showProductIdData = true, string? dialogId = null)
    {
        if (dialogId is not null)
        {
            PopupModel popupModel = new()
            {
                ExistingData = existingData,
                DialogId = dialogId,
            };

            return View("Popup", popupModel);
        }

        DefaultModel model = await GetDefaultModelAsync(existingData, showProductIdData);

        return View("Default", model);
    }

    private async Task<DefaultModel> GetDefaultModelAsync(ProductDataExistingData existingData, bool showProductIdData)
    {
        Product? product = existingData.Product;

        if (product is null)
        {
            return new();
        }

        List<ProductPropertyDisplayData>? propertiesInData = existingData.ProductProperties;

        propertiesInData ??= await GetPropertiesForProductAsync(product);

        List<ProductImageDisplayData>? productImagesInData = existingData.ProductImages;

        if (productImagesInData is null)
        {
            List<ProductImageFileData> productImagesFiles = await _productImageFileDataRepository.GetAllInProductAsync(product.Id);

            productImagesInData = new();

            foreach (ProductImageFileData productImageData in productImagesFiles)
            {
                if (productImageData.FileName is null) continue;

                ProductImageDisplayData imageDisplayData = new()
                {
                    ImageSrc = "~/" + CombinePathsWithSeparator('/', ProductImageFileDataEndpoints.EndpointGroupRoute, productImageData.FileName),
                };

                productImagesInData.Add(imageDisplayData);
            }
        }

        List<Promotion>? productPromotions = existingData.ProductPromotions;

        if (productPromotions is null)
        {
            List<Promotion> promotions = await _promotionService.GetAllForProductAsync(product.Id);

            productPromotions = promotions.Where(x =>
            {
                if (x.Id != product.PromotionPid && x.Id != product.PromotionRid) return false;

                return (x.StartDate is null || x.StartDate <= DateTime.Now) && (x.ExpirationDate is null || x.ExpirationDate >= DateTime.Now);
            })
                .ToList();
        }

        List<SearchStringPartOriginData>? productSearchStringParts = existingData.ProductSearchStringParts;

        productSearchStringParts ??= await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(
                product.SearchString, product.CategoryId);

        ProductPriceData? productPriceData = existingData.PriceData;

        productPriceData ??= await GetProductPriceDataAsync(product, existingData.PriceData);

        DefaultModel productDataModel = new()
        {
            Product = product,
            PriceData = productPriceData,
            ProductProperties = propertiesInData,
            ProductImages = productImagesInData,
            ProductPromotions = productPromotions,
            ProductSearchStringParts = productSearchStringParts,
            ShowProductIdData = showProductIdData,
        };

        return productDataModel;
    }

    private async Task<List<ProductPropertyDisplayData>> GetPropertiesForProductAsync(Product product)
    {
        List<int> relatedCategoryIds = [-1];

        if (product.CategoryId is not null)
        {
            relatedCategoryIds.Add(product.CategoryId.Value);
        }

        IEnumerable<ProductCharacteristicType> productCharacteristicTypes = [
            ProductCharacteristicType.ProductCharacteristic,
            ProductCharacteristicType.Link
        ];

        List<ProductCharacteristic> productCharacteristics = await _productCharacteristicService.GetAllByCategoryIdsAndTypesAsync(
            relatedCategoryIds, productCharacteristicTypes, true);

        List<ProductProperty> productProperties = await _productPropertyCrudService.GetAllInProductAsync(product.Id);

        return GetProductPropertiesFromProductData(productProperties, productCharacteristics);
    }

    private static List<ProductPropertyDisplayData> GetProductPropertiesFromProductData(
        List<ProductProperty> productProperties,
        List<ProductCharacteristic> productCharacteristics)
    {
        List<ProductPropertyDisplayData> propertiesInData = new();

        foreach (ProductProperty property in productProperties)
        {
            ProductCharacteristic productCharacteristic = productCharacteristics
                .First(characteristic => characteristic.Id == property.ProductCharacteristicId);

            ProductPropertyDisplayData mappedProperty = new()
            {
                ProductCharacteristic = new()
                {
                    Id = productCharacteristic!.Id,
                    CategoryId = productCharacteristic.CategoryId,
                    Active = productCharacteristic.Active,
                    KWPrCh = productCharacteristic.KWPrCh
                },
                Name = property.Characteristic,
                Value = property.Value,
                DisplayOrder = property.DisplayOrder
            };

            propertiesInData.Add(mappedProperty);
        }

        return propertiesInData;
    }

    private async Task<ProductPriceData?> GetProductPriceDataAsync(Product product, ProductPriceData? productPriceData)
    {
        DisplayPrice? displayPrice = null;
        DisplayPrice? secondaryDisplayPrice = null;

        if (productPriceData?.DisplayPrice is not null)
        {
            displayPrice = new()
            {
                Amount = productPriceData.DisplayPrice.Amount,
                Currency = productPriceData.DisplayPrice.Currency,
            };
        }

        if (productPriceData?.SecondaryDisplayPrice is not null)
        {
            secondaryDisplayPrice = new()
            {
                Amount = productPriceData.SecondaryDisplayPrice.Value.Amount,
                Currency = productPriceData.SecondaryDisplayPrice.Value.Currency,
            };
        }

        if (product.Price is not null && product.Currency is not null)
        {
            displayPrice ??= await ChangeCurrencyAndAddVATToSingleItemAsync(product.Price.Value, product.Currency.Value, Currency.EUR);
            secondaryDisplayPrice ??= await ChangeCurrencyAndAddVATToSingleItemAsync(product.Price.Value, product.Currency.Value, Currency.BGN);
        }

        if (displayPrice is null) return null;

        return new()
        {
            DisplayPrice = displayPrice.Value,
            SecondaryDisplayPrice = secondaryDisplayPrice,
        };
    }

    private async Task<DisplayPrice?> ChangeCurrencyAndAddVATToSingleItemAsync(decimal amount, Currency originalCurrency, Currency newCurrency)
    {
        OneOf<decimal, NotFound> getPriceInNewCurrencyResult = await _currencyConversionService.ChangeCurrencyAsync(
            amount, originalCurrency, newCurrency);

        if (getPriceInNewCurrencyResult.IsT0)
        {
            decimal priceWithVAT = _currencyVATService.AddVATToPriceUsingDefaultRate(getPriceInNewCurrencyResult.AsT0, 1);

            return new()
            {
                Amount = priceWithVAT,
                Currency = Currency.EUR,
            };
        }

        return null;
    }
}