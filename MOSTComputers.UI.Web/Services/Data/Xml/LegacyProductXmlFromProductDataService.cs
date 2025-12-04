using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Controllers.Images;
using MOSTComputers.UI.Web.Services.Data.Xml.Contracts;

using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Services.Data.Xml;

internal sealed class LegacyProductXmlFromProductDataService : ILegacyProductXmlFromProductDataService
{
    public LegacyProductXmlFromProductDataService(
        IProductPropertyService productPropertyService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        ISubCategoryService subCategoryService,
        IPromotionService promotionService,
        ISearchStringOriginService searchStringOriginService,
        IExchangeRateService exchangeRateService,
        ILegacyProductToXmlProductMappingService productToXmlProductMappingService)
    {
        _productPropertyService = productPropertyService;
        _productImageService = productImageService;
        _productImageFileService = productImageFileService;
        _subCategoryService = subCategoryService;
        _promotionService = promotionService;
        _searchStringOriginService = searchStringOriginService;
        _exchangeRateService = exchangeRateService;
        _legacyProductToXmlProductMappingService = productToXmlProductMappingService;
    }

    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly ISubCategoryService _subCategoryService;
    private readonly IPromotionService _promotionService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ILegacyProductToXmlProductMappingService _legacyProductToXmlProductMappingService;

    public async Task<LegacyXmlObjectData> GetLegacyXmlObjectDataForProductsAsync(IEnumerable<Product> products, string hostPath)
    {
        List<LegacyXmlProduct> xmlProducts = await GetLegacyXmlProductsFromProductsAsync(products, hostPath);

        ExchangeRate? eurToBgnExchangeRate = await _exchangeRateService.GetForCurrenciesAsync(Currency.EUR, Currency.BGN);
        ExchangeRate? usdToBgnExchangeRate = await _exchangeRateService.GetForCurrenciesAsync(Currency.USD, Currency.BGN);

        DateTime exchangeRateValidTo = DateTime.Now.AddHours(1);

        LegacyXmlExchangeRatesEurAndUsdPerBgn xmlExchangeRates = new()
        {
            ExchangeRateEUR = (eurToBgnExchangeRate is not null) ? new() { ExchangeRatePerBGN = eurToBgnExchangeRate.Rate } : null,
            ExchangeRateUSD = (usdToBgnExchangeRate is not null) ? new() { ExchangeRatePerBGN = usdToBgnExchangeRate.Rate } : null,
            ValidTo = exchangeRateValidTo,
        };

        LegacyXmlObjectData xmlData = new()
        {
            Products = xmlProducts,
            DateOfExport = DateTime.Today,
            ExchangeRates = xmlExchangeRates,
        };

        return xmlData;
    }

    public async Task<List<LegacyXmlProduct>> GetLegacyXmlProductsFromProductsAsync(IEnumerable<Product> products, string hostPath)
    {
        IEnumerable<int> productIds = products.Select(product => product.Id);

        List<IGrouping<int, ProductProperty>>? propertiesForProducts = await _productPropertyService.GetAllInProductsAsync(productIds);
        List<IGrouping<int, ProductImageData>>? imagesForProducts = await _productImageService.GetAllInProductsWithoutFileDataAsync(productIds);
        List<IGrouping<int, ProductImageFileData>>? imageFileNameInfosForProducts = await _productImageFileService.GetAllInProductsAsync(productIds);

        List<SubCategory> productSubCategories = await _subCategoryService.GetAllByActivityAsync(true);

        List<IGrouping<int?, Promotion>> promotionsForProducts = await _promotionService.GetAllActiveForSelectionOfProductsAsync(productIds);

        Dictionary<int, List<SearchStringPartOriginData>>? searchStringPartsForProducts
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginForProductsAsync(products);

        List<LegacyXmlProduct> xmlProducts = new();

        foreach (Product product in products)
        {
            List<ProductProperty>? productProperties = propertiesForProducts?.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageData>? productImages = imagesForProducts?.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageFileData>? productImageFileNameInfos = imageFileNameInfosForProducts?.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<LegacyXmlProductImage>? xmlProductImages = GetXmlProductImagesFromProductImages(
                productImages,
                productImageFileNameInfos,
                hostPath);

            List<Promotion>? productPromotions = promotionsForProducts?.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            SubCategory? productSubCategory = null;

            if (product.SubCategoryId is not null)
            {
                productSubCategory = productSubCategories.FirstOrDefault(x => x.Id == product.SubCategoryId.Value);
            }

            List<LegacySearchStringPartInfo>? searchStringPartInfos = null;

            List<SearchStringPartOriginData>? searchStringParts = searchStringPartsForProducts.FirstOrDefault(x => x.Key == product.Id)
                .Value;

            if (searchStringParts is not null)
            {
                searchStringPartInfos = GetSearchStringDataForXmlFromSearchStringData(searchStringParts);
            }

            LegacyXmlProduct xmlProduct = _legacyProductToXmlProductMappingService.MapProductDataToXmlProduct(
                product,
                productProperties,
                xmlProductImages,
                productPromotions,
                productSubCategory,
                searchStringPartInfos);

            xmlProducts.Add(xmlProduct);
        }

        return xmlProducts;
    }

    private static List<LegacyXmlProductImage> GetXmlProductImagesFromProductImages(
       List<ProductImageData>? productImages = null,
       List<ProductImageFileData>? productImageFileNameInfos = null,
       string? applicationRootPath = null)
    {
        List<LegacyXmlProductImage> output = new();

        if (productImages is null && productImageFileNameInfos is null) return output;

        if (productImages is not null)
        {
            foreach (ProductImageData productImage in productImages)
            {
                LegacyXmlProductImage xmlImage = GetXmlImageFromProductImageId(productImage.Id, null, applicationRootPath);

                output.Add(xmlImage);
            }
        }

        if (productImageFileNameInfos is not null)
        {
            IEnumerable<ProductImageFileData> remainingProductImageFileNameInfos = productImageFileNameInfos
                .Where(imageFile => !productImages?.Exists(image => image.Id == imageFile.ImageId) ?? false);

            foreach (ProductImageFileData productImageFileNameInfo in remainingProductImageFileNameInfos)
            {
                if (productImageFileNameInfo.FileName is null) continue;

                LegacyXmlProductImage xmlImage = GetXmlImageFromProductImageFileName(
                    productImageFileNameInfo.FileName, productImageFileNameInfo.DisplayOrder, applicationRootPath);

                output.Add(xmlImage);
            }
        }

        return output;
    }

    private static LegacyXmlProductImage GetXmlImageFromProductImageId(int productImageId, int? displayOrder = null, string? applicationRootPath = null)
    {
        string imageUrl = CombinePathsWithSeparator('/', applicationRootPath ?? string.Empty, ProductImageDataController.ControllerRoute, productImageId.ToString());

        return new()
        {
            PictureUrl = imageUrl,
            ThumbnailUrl = imageUrl,
            DisplayOrder = displayOrder,
        };
    }

    private static LegacyXmlProductImage GetXmlImageFromProductImageFileName(string fileName, int? displayOrder = null, string? applicationRootPath = null)
    {
        string imageFileUrl = CombinePathsWithSeparator('/', applicationRootPath ?? string.Empty, ProductImageFileDataController.ControllerRoute, fileName);

        return new()
        {
            PictureUrl = imageFileUrl,
            ThumbnailUrl = imageFileUrl,
            DisplayOrder = displayOrder,
        };
    }

    private static List<LegacySearchStringPartInfo> GetSearchStringDataForXmlFromSearchStringData(List<SearchStringPartOriginData> searchStringOriginData)
    {
        List<LegacySearchStringPartInfo> output = new();

        foreach (SearchStringPartOriginData searchStringPartOriginData in searchStringOriginData)
        {
            string? description = searchStringPartOriginData.Characteristic.Meaning;

            LegacySearchStringPartInfo searchStringPartInfo = new()
            {
                Name = searchStringPartOriginData.SearchStringPart,
                Description = description,
            };

            output.Add(searchStringPartInfo);
        }

        return output;
    }

    private static DateTime RoundDateTimeToNextHours(DateTime dateTime, int hours)
    {
        DateTime dateTimeRoundedToHour = new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

        return dateTimeRoundedToHour.AddHours(hours);
    }
}