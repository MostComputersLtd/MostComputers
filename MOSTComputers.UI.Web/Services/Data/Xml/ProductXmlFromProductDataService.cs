using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
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

internal sealed class ProductXmlFromProductDataService : IProductXmlFromProductDataService
{
    public ProductXmlFromProductDataService(
        IProductPropertyService productPropertyService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        ISubCategoryService subCategoryService,
        IPromotionService promotionService,
        ISearchStringOriginService searchStringOriginService,
        IExchangeRateService exchangeRateService,
        IProductToXmlProductMappingService productToXmlProductMappingService)
    {
        _productPropertyService = productPropertyService;
        _productImageService = productImageService;
        _productImageFileService = productImageFileService;
        _subCategoryService = subCategoryService;
        _promotionService = promotionService;
        _searchStringOriginService = searchStringOriginService;
        _exchangeRateService = exchangeRateService;
        _productToXmlProductMappingService = productToXmlProductMappingService;
    }

    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly ISubCategoryService _subCategoryService;
    private readonly IPromotionService _promotionService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly IProductToXmlProductMappingService _productToXmlProductMappingService;

    public async Task<ProductsXmlFullData> GetXmlObjectDataForProductsAsync(IEnumerable<Product> products, string hostPath)
    {
        List<XmlProduct> xmlProducts = await GetXmlProductsFromProductsAsync(products, hostPath);

        ExchangeRate? usdToBgnExchangeRate = await _exchangeRateService.GetForCurrenciesAsync(Currency.EUR, Currency.USD);

        DateTime exchangeRateValidTo = DateTime.Now.AddHours(1);

        XmlExchangeRates xmlExchangeRates = new()
        {
            ExchangeRateUSD = (usdToBgnExchangeRate is not null) ? new() { ExchangeRatePerEUR = usdToBgnExchangeRate.Rate } : null,
            ValidTo = exchangeRateValidTo,
        };

        ProductsXmlFullData xmlData = new()
        {
            Products = xmlProducts,
            DateOfExport = DateTime.Today,
            ExchangeRates = xmlExchangeRates,
        };

        return xmlData;
    }

    public async Task<List<XmlProduct>> GetXmlProductsFromProductsAsync(IEnumerable<Product> products, string hostPath)
    {
        IEnumerable<int> productIds = products.Select(product => product.Id);

        List<IGrouping<int, ProductProperty>>? propertiesForProducts = await _productPropertyService.GetAllInProductsAsync(productIds);
        List<IGrouping<int, ProductImageData>>? imagesForProducts = await _productImageService.GetAllInProductsWithoutFileDataAsync(productIds);
        List<IGrouping<int, ProductImageFileData>>? imageFileNameInfosForProducts = await _productImageFileService.GetAllInProductsAsync(productIds);

        List<SubCategory> productSubCategories = await _subCategoryService.GetAllByActivityAsync(true);

        List<IGrouping<int?, Promotion>> promotionsForProducts = await _promotionService.GetAllActiveForSelectionOfProductsAsync(productIds);

        Dictionary<int, List<SearchStringPartOriginData>>? searchStringPartsForProducts
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginForProductsAsync(products);

        List<XmlProduct> xmlProducts = new();

        foreach (Product product in products)
        {
            List<ProductProperty>? productProperties = propertiesForProducts?.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageData>? productImages = imagesForProducts?.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<ProductImageFileData>? productImageFileNameInfos = imageFileNameInfosForProducts?.FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<XmlProductImage>? xmlProductImages = GetXmlProductImagesFromProductImages(
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

            List<XmlSearchStringPartInfo>? searchStringPartInfos = null;

            List<SearchStringPartOriginData>? searchStringParts = searchStringPartsForProducts.FirstOrDefault(x => x.Key == product.Id)
                .Value;

            if (searchStringParts is not null)
            {
                searchStringPartInfos = GetSearchStringDataForXmlFromSearchStringData(searchStringParts);
            }

            XmlProduct xmlProduct = _productToXmlProductMappingService.MapProductDataToXmlProduct(
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

    private static List<XmlProductImage> GetXmlProductImagesFromProductImages(
       List<ProductImageData>? productImages = null,
       List<ProductImageFileData>? productImageFileNameInfos = null,
       string? applicationRootPath = null)
    {
        List<XmlProductImage> output = new();

        if (productImages is null && productImageFileNameInfos is null) return output;

        if (productImages is not null)
        {
            foreach (ProductImageData productImage in productImages)
            {
                XmlProductImage xmlImage = GetXmlImageFromProductImageId(productImage.Id, applicationRootPath);

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

                XmlProductImage xmlImage = GetXmlImageFromProductImageFileName(
                    productImageFileNameInfo.FileName, applicationRootPath);

                output.Add(xmlImage);
            }
        }

        return output;
    }

    private static XmlProductImage GetXmlImageFromProductImageId(int productImageId, string? applicationRootPath = null)
    {
        return new()
        {
            PictureUrl = CombinePathsWithSeparator('/', applicationRootPath ?? string.Empty, ProductImageDataController.ControllerRoute, productImageId.ToString()),
        };
    }

    private static XmlProductImage GetXmlImageFromProductImageFileName(string fileName, string? applicationRootPath = null)
    {
        return new()
        {
            PictureUrl = CombinePathsWithSeparator('/', applicationRootPath ?? string.Empty, ProductImageFileDataController.ControllerRoute, fileName),
        };
    }

    private static List<XmlSearchStringPartInfo> GetSearchStringDataForXmlFromSearchStringData(List<SearchStringPartOriginData> searchStringOriginData)
    {
        List<XmlSearchStringPartInfo> output = new();

        foreach (SearchStringPartOriginData searchStringPartOriginData in searchStringOriginData)
        {
            string? description = searchStringPartOriginData.Characteristic.Meaning;

            XmlSearchStringPartInfo searchStringPartInfo = new()
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