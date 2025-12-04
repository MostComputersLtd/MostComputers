using OneOf;
using OneOf.Types;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.UI.Web.Blazor.Services.ProductEditor.Contracts;
using MOSTComputers.UI.Web.Blazor.Services.ExternalXmlImport.Contracts;

using static MOSTComputers.UI.Web.Blazor.Components.Pages.ProductEditor;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductImages;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.PromotionProductFileInfos;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;

namespace MOSTComputers.UI.Web.Blazor.Services.ProductEditor;

internal sealed class ProductEditorDataService : IProductEditorDataService
{
    public ProductEditorDataService(
        IProductXmlProvidingService productXmlProvidingService,
        IProductWorkStatusesService productWorkStatusesService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileService,
        IProductPropertyService productPropertyService,
        IPromotionService promotionService,
        IPromotionProductFileService promotionProductFileService,
        ISearchStringOriginService searchStringOriginService)
    {
        _productXmlProvidingService = productXmlProvidingService;
        _productWorkStatusesService = productWorkStatusesService;
        _productImageFileService = productImageFileService;
        _productPropertyService = productPropertyService;
        _productImageService = productImageService;
        _promotionService = promotionService;
        _searchStringOriginService = searchStringOriginService;
        _promotionProductFileService = promotionProductFileService;
    }

    private readonly IProductXmlProvidingService _productXmlProvidingService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IProductImageService _productImageService;
    private readonly IProductImageFileService _productImageFileService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IPromotionService _promotionService;
    private readonly IPromotionProductFileService _promotionProductFileService;
    private readonly ISearchStringOriginService _searchStringOriginService;

    public async Task<List<ProductEditorProductData>> GetProductEditorProductDatasAsync(List<Product> products, bool fetchLegacyHtmlData = false)
    {
        List<int> productIds = products.Select(x => x.Id)
            .ToList();

        //OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound>? getProductXmlDataResult = null;

        //if (fetchLegacyHtmlData)
        //{
        //    getProductXmlDataResult = await _productXmlProvidingService.GetProductXmlParsedAsync();
        //}

        List<ProductWorkStatuses> productStatuses = await _productWorkStatusesService.GetAllForProductsAsync(productIds);

        List<IGrouping<int, ProductImageFileData>> fileNameData
            = await _productImageFileService.GetAllInProductsAsync(productIds);

        Dictionary<int, int> directoryImagesCountForProduct = new();

        foreach (IGrouping<int, ProductImageFileData> fileNameDataGroup in fileNameData)
        {
            directoryImagesCountForProduct.Add(fileNameDataGroup.Key, fileNameDataGroup.Count());
        }

        List<ProductPropertiesForProductCountData> originalPropertiesCount
            = await _productPropertyService.GetCountOfAllInProductsAsync(productIds);

        List<IGrouping<int, ProductImageData>> originalImages
            = await _productImageService.GetAllInProductsWithoutFileDataAsync(productIds);

        List<ProductFirstImageExistsForProductData> firstImagesExist
            = await _productImageService.DoProductsHaveImagesInFirstImagesAsync(productIds);

        List<IGrouping<int?, Promotion>> productPromotions = await _promotionService.GetAllForSelectionOfProductsAsync(productIds);

        Dictionary<int, List<SearchStringPartOriginData>> searchStringOriginData
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginForProductsAsync(products);

        List<PromotionProductFileInfoForProductCountData> promotionFilesCount
            = await _promotionProductFileService.GetCountOfAllForProductsAsync(productIds);

        List<ProductEditorProductData> entries = new();

        foreach (Product product in products)
        {
            //LegacyXmlProduct? xmlProduct = null;

            //if (fetchLegacyHtmlData)
            //{ 
            //    xmlProduct = getProductXmlDataResult!.Value.Match(
            //        xmlObjectData => xmlObjectData.Products.FirstOrDefault(x => x.Id == product.Id),
            //        invalidXmlResult => null,
            //        notFound => null);
            //}

            int directoryImagesCount = directoryImagesCountForProduct.GetValueOrDefault(product.Id, 0);

            List<ProductImageFileData>? productImageFileNameInfos = fileNameData
                .FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            ProductPropertiesForProductCountData? productPropertiesForProductCountData = originalPropertiesCount
                .FirstOrDefault(x => x.ProductId == product.Id);

            List<ProductImageData>? originalProductImagesForProduct = originalImages
                .FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            ProductFirstImageExistsForProductData? productFirstImageExistsForProductData = firstImagesExist
                .FirstOrDefault(x => x.ProductId == product.Id);

            List<Promotion>? promotionsForProduct = productPromotions
                .FirstOrDefault(x => x.Key == product.Id)?
                .ToList();

            List<SearchStringPartOriginData>? searchStringOriginDataForProduct = searchStringOriginData.GetValueOrDefault(product.Id);

            PromotionProductFileInfoForProductCountData? promotionProductFileInfoForProductCountData
                = promotionFilesCount.FirstOrDefault(x => x.ProductId == product.Id);

            ProductWorkStatuses? productStatusData = productStatuses.FirstOrDefault(x => x.ProductId == product.Id);

            ProductEditorProductData entryForProduct = await GetProductEditorProductDataFromSharedDataInternalAsync(
                product,
                productStatusData,
                //xmlProduct,
                directoryImagesCount,
                productPropertiesForProductCountData?.PropertyCount ?? 0,
                productImageFileNameInfos ?? new(),
                originalProductImagesForProduct ?? new(),
                productFirstImageExistsForProductData?.FirstImageExists ?? false,
                promotionsForProduct ?? new(),
                searchStringOriginDataForProduct ?? new(),
                promotionProductFileInfoForProductCountData?.Count ?? 0);

            entries.Add(entryForProduct);
        }

        return entries;
    }

    public async Task<ProductEditorProductData> GetProductEditorProductDataAsync(Product product, bool fetchLegacyHtmlData = false)
    {
        LegacyXmlProduct? xmlProduct = null;

        if (fetchLegacyHtmlData)
        {
            OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlDataResult
                = await _productXmlProvidingService.GetProductXmlParsedAsync();

            xmlProduct = getProductXmlDataResult.Match(
                xmlObjectData => xmlObjectData.Products.FirstOrDefault(x => x.Id == product.Id),
                invalidXmlResult => null,
                notFound => null);
        }

        ProductWorkStatuses? productStatuses = await _productWorkStatusesService.GetByProductIdAsync(product.Id);

        return await GetProductEditorProductDataFromSharedDataInternalAsync(product, productStatuses);
        //return await GetProductEditorProductDataFromSharedDataInternalAsync(product, productStatuses, xmlProduct);
    }

    private async Task<ProductEditorProductData> GetProductEditorProductDataFromSharedDataInternalAsync(
        Product product,
        ProductWorkStatuses? productStatuses = null,
        //LegacyXmlProduct? xmlProduct = null,
        int? directoryImagesCount = null,
        int? productPropertiesCount = null,
        List<ProductImageFileData>? fileNameData = null,
        List<ProductImageData>? originalImages = null,
        bool? firstImageExists = null,
        List<Promotion>? productPromotions = null,
        List<SearchStringPartOriginData>? searchStringParts = null,
        int? promotionFilesCount = null)
    {
        productPropertiesCount ??= await _productPropertyService.GetCountOfAllInProductAsync(product.Id);

        fileNameData ??= await _productImageFileService.GetAllInProductAsync(product.Id);

        directoryImagesCount ??= fileNameData.Count;

        originalImages ??= await _productImageService.GetAllInProductWithoutFileDataAsync(product.Id);

        firstImageExists ??= await _productImageService.DoesProductHaveImageInFirstImagesAsync(product.Id);

        searchStringParts ??= await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(product.SearchString, product.CategoryId);

        productPromotions ??= await _promotionService.GetAllForProductAsync(product.Id);

        promotionFilesCount ??= await _promotionProductFileService.GetCountOfAllForProductAsync(product.Id);

        ProductEditorProductData imagesCompareTableEntry = new()
        {
            Product = product,
            ProductStatuses = productStatuses,
            //LegacyXmlProduct = xmlProduct,
            DirectoryImagesCount = directoryImagesCount.Value,
            ProductPropertiesCount = productPropertiesCount ?? 0,
            FileNameData = fileNameData,
            OriginalImagesCount = originalImages.Count,
            OriginalFirstImageExists = firstImageExists.Value,
            ProductPromotions = productPromotions,
            SearchStringParts = searchStringParts,
            PromotionFilesCount = promotionFilesCount,
        };


        return imagesCompareTableEntry;
    }
}