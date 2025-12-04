using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductProperty;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionProductFileInfo;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.Services.ProductRegister.Services.ProductStatus.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.Services.SearchStringOrigin.Services.Contracts;
using MOSTComputers.UI.Web.Controllers.Images;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.ProductEditor;
using MOSTComputers.UI.Web.Models.ProductEditor.DTOs;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductImages;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductImages.DTOs;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties;
using MOSTComputers.UI.Web.Models.ProductEditor.ProductProperties.DTOs;
using MOSTComputers.UI.Web.Models.ProductSearch;
using MOSTComputers.UI.Web.Models.PromotionFiles;
using MOSTComputers.UI.Web.Models.PromotionFiles.DTOs;
using MOSTComputers.UI.Web.Pages.Shared.ProductEditor;
using MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductImages;
using MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductProperties;
using MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductSearchStringData;
using MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductXmlData;
using MOSTComputers.UI.Web.Pages.Shared.ProductEditor.Promotions;
using MOSTComputers.UI.Web.Pages.Shared.ProductEditor.Promotions.PromotionFiles;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.UI.Web.Services.Data.Contracts;
using MOSTComputers.UI.Web.Services.Data.ProductEditor.Contracts;
using MOSTComputers.UI.Web.Services.Data.Search.Contracts;
using MOSTComputers.UI.Web.Services.ExternalXmlImport.Contracts;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Utils.PageCommonElements;
using static MOSTComputers.UI.Web.Utils.SelectListItemUtils;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;
using static MOSTComputers.Utils.Files.FilePathUtils;
using static MOSTComputers.Utils.OneOf.AsyncMatchingExtensions;

namespace MOSTComputers.UI.Web.Pages;

[Authorize]
public class ProductEditorModel : PageModel
{
    public ProductEditorModel(
        IProductEditorProductDataService imagesCompareEntryService,
        IImageExtensionFromContentTypeService fileExtensionFromContentTypeService,
        ITransactionExecuteService transactionExecuteService,
        ICategoryService categoryService,
        ISubCategoryService subCategoryService,
        IProductService productService,
        IProductSearchService productSearchService,
        IProductImageService productImageService,
        IProductImageFileService productImageFileAndFileNameInfoService,
        IProductPropertyService productPropertyService,
        IProductWorkStatusesService productWorkStatusesService,
        IProductWorkStatusesWorkflowService productWorkStatusesWorkflowService,
        IProductWorkStatusesChangesUpsertService productWorkStatusesChangesUpsertService,
        IProductCharacteristicService productCharacteristicService,
        IPromotionService promotionService,
        IPromotionProductFileService promotionProductFileService,
        IOriginalLocalChangesReadService originalLocalChangesReadService,
        IExchangeRateService exchangeRateService,
        IProductCharacteristicAndExternalXmlDataRelationService characteristicAndExternalXmlDataRelationService,
        IProductXmlProvidingService productXmlProvidingService,
        IProductLegacyXmlReadService productXmlOriginalXmlReadService,
        IProductXmlService productNewXmlService,
        IProductToXmlProductMappingService productToNewXmlProductMappingService,
        IProductHtmlService productHtmlService,
        ISearchStringOriginService searchStringOriginService,
        IPropertyEditorCharacteristicsService propertyEditorCharacteristicsService,
        IPromotionFileEditorDataService promotionFileInfoEditorDataService,
        IPartialViewRenderService partialViewRenderService)
    {
        _productEditorProductDataService = imagesCompareEntryService;
        _fileExtensionFromContentTypeService = fileExtensionFromContentTypeService;
        _transactionExecuteService = transactionExecuteService;
        _categoryService = categoryService;
        _subCategoryService = subCategoryService;
        _productService = productService;
        _productSearchService = productSearchService;
        _productImageService = productImageService;
		_productImageFileService = productImageFileAndFileNameInfoService;
        _productPropertyService = productPropertyService;
        _productWorkStatusesService = productWorkStatusesService;
        _productWorkStatusesWorkflowService = productWorkStatusesWorkflowService;
        _productWorkStatusesChangesUpsertService = productWorkStatusesChangesUpsertService;
        _productCharacteristicService = productCharacteristicService;
        _promotionService = promotionService;
        _promotionProductFileService = promotionProductFileService;
        _originalLocalChangesReadService = originalLocalChangesReadService;
        _exchangeRateService = exchangeRateService;
        _characteristicAndExternalXmlDataRelationService = characteristicAndExternalXmlDataRelationService;
        _productXmlProvidingService = productXmlProvidingService;
        _productXmlOriginalXmlReadService = productXmlOriginalXmlReadService;
        _productNewXmlService = productNewXmlService;
        _productToNewXmlProductMappingService = productToNewXmlProductMappingService;
        _productHtmlService = productHtmlService;
        _searchStringOriginService = searchStringOriginService;
        _propertyEditorCharacteristicsService = propertyEditorCharacteristicsService;
        _promotionFileEditorDataService = promotionFileInfoEditorDataService;
        _partialViewRenderService = partialViewRenderService;
    }

    private const int _defaultLinkCharacteristicId = 1693;

    private const int _lastAddedProductsSearchCount = 200;

    internal const string ProductDataTablePartialPath = "ProductEditor/_ProductDataTablePartial";
    internal const string ProductDataTableEntryPartialPath = "ProductEditor/_ProductDataTableRowPartial";

    public readonly string ProductTableContainerElementId = "productDataTableContainer";

    public readonly string ProductTableEntryElementIdPrefix = "productDataTableRow-";

    public readonly string NotificationBoxId = "topNotificationBox";

    public readonly ModalData ProductFullDisplayPartialModalData = new()
    {
        ModalId = "productsModal",
        ModalDialogId = "productsModalDialog",
        ModalContentId = "productsModalContent",
    };

    public readonly ModalData ProductHtmlAndImagesPartialModelData = new()
    {
        ModalId = "productHtmlAndImagesModal",
        ModalDialogId = "productHtmlAndImagesModalDialog",
        ModalContentId = "productHtmlAndImagesModalContent",
    };

    public readonly ModalData ProductPropertiesEditorPartialModalData = new()
    {
        ModalId = "productPropertiesEditor_modal",
        ModalDialogId = "productPropertiesEditor_modal_dialog",
        ModalContentId = "productPropertiesEditor_modal_content",
    };

    public readonly ModalData SearchStringPartsPartialModalData = new()
    {
        ModalId = "searchStringParts_modal",
        ModalDialogId = "searchStringParts_modal_dialog",
        ModalContentId = "searchStringParts_modal_content",
    };

    public readonly ModalData XmlViewPartialModalData = new()
    {
        ModalId = "xmlView_modal",
        ModalDialogId = "xmlView_modal_dialog",
        ModalContentId = "xmlView_modal_content",
    };

    public readonly ModalData ProductPropertiesPartialModalData = new()
    {
        ModalId = "properties_modal",
        ModalDialogId = "properties_modal_dialog",
        ModalContentId = "properties_modal_content",
    };

    public readonly ModalData OldXmlPropertiesPartialModalData = new()
    {
        ModalId = "oldXmlProperties_modal",
        ModalDialogId = "oldXmlProperties_modal_dialog",
        ModalContentId = "oldXmlProperties_modal_content",
    };

    public readonly ModalData ImagesPartialModalData = new()
    {
        ModalId = "images_modal",
        ModalDialogId = "images_modal_dialog",
        ModalContentId = "images_modal_content",
    };

    public readonly ModalData ImageFilesPartialModalData = new()
    {
        ModalId = "imageFiles_modal",
        ModalDialogId = "imageFiles_modal_dialog",
        ModalContentId = "imageFiles_modal_content",
    };

    public readonly ModalData ImageFileNameInfosPartialModalData = new()
    {
        ModalId = "infoPromotionView_modal",
        ModalDialogId = "infoPromotionView_modal_dialog",
        ModalContentId = "infoPromotionView_modal_content",
    };

    public readonly ModalData PromotionViewPartialModalData = new()
    {
        ModalId = "promotionView_modal",
        ModalDialogId = "promotionView_modal_dialog",
        ModalContentId = "promotionView_modal_content",
    };

    public readonly ModalData InfoPromotionViewPartialModalData = new()
    {
        ModalId = "infoPromotionView_modal",
        ModalDialogId = "infoPromotionView_modal_dialog",
        ModalContentId = "infoPromotionView_modal_content",
    };

    public readonly ModalData PromotionProductFileEditorPartialModalData = new()
    {
        ModalId = "promotionProductFilesEditor_modal",
        ModalDialogId = "promotionProductFilesEditor_modal_dialog",
        ModalContentId = "promotionProductFilesEditor_modal_content",
    };

    public readonly ModalData PromotionProductFileInfoSingleEditorPartialModalData = new()
    {
        ModalId = "productPromotionFileSingleEditor_modal",
        ModalDialogId = "productPromotionFileSingleEditor_modal_dialog",
        ModalContentId = "productPromotionFileSingleEditor_modal_content",
    };

    private readonly IProductEditorProductDataService _productEditorProductDataService;
    private readonly IImageExtensionFromContentTypeService _fileExtensionFromContentTypeService;
    private readonly ITransactionExecuteService _transactionExecuteService;
    private readonly ICategoryService _categoryService;
    private readonly ISubCategoryService _subCategoryService;
    private readonly IProductService _productService;
    private readonly IProductSearchService _productSearchService;
    private readonly IProductImageService _productImageService;
	private readonly IProductImageFileService _productImageFileService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductWorkStatusesService _productWorkStatusesService;
    private readonly IProductWorkStatusesWorkflowService _productWorkStatusesWorkflowService;
    private readonly IProductWorkStatusesChangesUpsertService _productWorkStatusesChangesUpsertService;
    private readonly IProductCharacteristicService _productCharacteristicService;
    private readonly IPromotionService _promotionService;
    private readonly IPromotionProductFileService _promotionProductFileService;
    private readonly IOriginalLocalChangesReadService _originalLocalChangesReadService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly IProductCharacteristicAndExternalXmlDataRelationService _characteristicAndExternalXmlDataRelationService;
    private readonly IProductXmlProvidingService _productXmlProvidingService;
    private readonly IProductLegacyXmlReadService _productXmlOriginalXmlReadService;
    private readonly IProductXmlService _productNewXmlService;
    private readonly IProductToXmlProductMappingService _productToNewXmlProductMappingService;
    private readonly IProductHtmlService _productHtmlService;
    private readonly ISearchStringOriginService _searchStringOriginService;
    private readonly IPropertyEditorCharacteristicsService _propertyEditorCharacteristicsService;
    private readonly IPromotionFileEditorDataService _promotionFileEditorDataService;
    private readonly IPartialViewRenderService _partialViewRenderService;

    public ProductDataTablePartialModel? ProductDataTableModel { get; private set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<List<SelectListItem>> GetAllCategorySelectListItemsAsync(int? selectedCategoryId = null)
    {
        List<Category> categories = await _categoryService.GetAllAsync();

        bool isAnySelected = selectedCategoryId is not null;

        List<SelectListItem> selectListItems = GetCategorySelectListItems(categories, selectedCategoryId, new("Всички", string.Empty, !isAnySelected, false));

        return selectListItems;
    }

    public async Task<IActionResult> OnPostGetProductPropertiesEditorPartialAsync(int productId, int? productDataElementIndex = null)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        return await GetProductPropertiesEditorPartialAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);
    }

    public async Task<IActionResult> OnPostGetSearchStringPartsEditorPartialAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        List<SearchStringPartOriginData>? searchStringOriginData
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(product.SearchString, product.CategoryId);

        return GetSearchStringPartsFromImageTableEntry(SearchStringPartsPartialModalData, product, searchStringOriginData);
    }

    public async Task<IActionResult> OnPostGetXmlViewPartialAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        string hostPath = $"{Request.Scheme}://{Request.Host}";

        return await GetXmlViewFromImageTableEntryAsync(XmlViewPartialModalData, product, hostPath);
    }

    public async Task<IActionResult> OnPostGetOldXmlDataAsync(int productId)
    {
        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getXmlObjectDataResult
            = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getXmlObjectDataResult.MatchAsync<LegacyXmlObjectData, InvalidXmlResult, NotFound, IStatusCodeActionResult>(
            async xmlObjectData =>
            {
                LegacyXmlProduct? xmlProduct = xmlObjectData.Products.FirstOrDefault(x => x.Id == productId);

                if (xmlProduct is null) return NotFound();

                string oldXmlData = await _productXmlOriginalXmlReadService.GetProductXmlAsync(xmlProduct.UId);

                return new OkObjectResult(oldXmlData);
            },
            invalidXmlResult => BadRequest(invalidXmlResult),
            notFound => NotFound());
    }

    public async Task<IActionResult> OnPostGetPropertiesDisplayPartialAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        return await GetPropertiesDisplayFromImageTableEntryAsync(ProductPropertiesPartialModalData, product);
    }

    public async Task<IActionResult> OnPostGetOldXmlPropertiesDisplayPartialAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getXmlObjectDataResult
            = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getXmlObjectDataResult.MatchAsync(
            async xmlObjectData =>
            {
                LegacyXmlProduct? xmlProduct = xmlObjectData.Products.FirstOrDefault(x => x.Id == productId);

                return (IStatusCodeActionResult)await GetXmlPropertiesDisplayFromImageTableEntryAsync(OldXmlPropertiesPartialModalData, product, xmlProduct);
            },
            invalidXmlResult => BadRequest(invalidXmlResult),
            notFound => NotFound());
    }

    public async Task<IActionResult> OnPostGetImagesDisplayPartialAsync(int productId, bool isOriginalData = true, bool firstImageOnly = false)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        return await GetImagesDisplayFromImageTableEntryAsync(ImagesPartialModalData, product, isOriginalData, firstImageOnly);
    }

    public async Task<IActionResult> OnPostGetImageFileNameInfosDisplayPartialAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        return await GetImageFileNameInfosDisplayFromImageTableEntryAsync(ImageFileNameInfosPartialModalData, product);
    }

    public async Task<IActionResult> OnPostGetPromotionViewPartialAsync(int productId, int promotionId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        List<Promotion> promotions = await _promotionService.GetAllForProductAsync(productId);

        Promotion? promotion = promotions.FirstOrDefault(x => x.Id == promotionId);

        if (promotion is null || promotion.ProductId != productId) return BadRequest();

        return GetPromotionPartialFromImageTableEntry(PromotionViewPartialModalData, product, promotion);
    }

    public async Task<IActionResult> OnPostGetInfoPromotionViewPartialAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        return GetInfoPromotionPartialFromImageTableEntry(InfoPromotionViewPartialModalData, product);
    }

    public async Task<IActionResult> OnPostGetPromotionFilesSelectableTablePartialAsync(
        int? selectedPromotionProductFileInfoId = null,
        [FromBody] PromotionFilesSearchOptions? searchOptions = null)
    {
        if (searchOptions is null)
        {
            List<PromotionFileInfoEditorDisplayData> promotionFileEditorDatas = await _promotionFileEditorDataService.GetAllFilesAsync();

            return GetPromotionFileTableViewPartial(promotionFileEditorDatas, selectedPromotionProductFileInfoId);
        }
        
        List<PromotionFileInfoEditorDisplayData> searchedPromotionFileEditorDatas
            = await _promotionFileEditorDataService.GetSearchedFilesAsync(searchOptions);

        return GetPromotionFileTableViewPartial(searchedPromotionFileEditorDatas, selectedPromotionProductFileInfoId);
    }
    
    private PartialViewResult GetPromotionFileTableViewPartial(
        List<PromotionFileInfoEditorDisplayData> promotionFileEditorDatas,
        int? selectedPromotionProductFileId = null)
    {
        int? selectedPromotionFileIndex = null;

        if (selectedPromotionProductFileId is not null)
        {
            selectedPromotionFileIndex = promotionFileEditorDatas.FindIndex(x => x.Id == selectedPromotionProductFileId);
        }

        PromotionFileSelectableTablePartialModel model = new()
        {
            PromotionFiles = promotionFileEditorDatas,
            SelectedPromotionFileIndex = selectedPromotionFileIndex,
        };

        return Partial("ProductEditor/Promotions/PromotionFiles/_PromotionFileSelectableTablePartial", model);
    }

    public async Task<IActionResult> OnPostGetPromotionProductFileEditorPartialAsync(int productId, int? productDataElementIndex = null)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        PromotionProductFileEditorPartialModel promotionProductFileEditorPartialModel
            = await GetPromotionProductFileEditorPartialModelForProductAsync(product, productDataElementIndex);

        return GetPromotionProductFileEditorPartial(promotionProductFileEditorPartialModel);
    }

    public async Task<IActionResult> OnPostGetPromotionProductFileSingleEditorPartialAsync(int productId,
        int? promotionProductFileInfoId = null,
        int? productDataElementIndex = null)
    {
        if (PromotionProductFileEditorPartialModalData.ModalContentId is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        PromotionProductFileInfo? promotionProductFileInfo = null;

        if (promotionProductFileInfoId is not null)
        {
            promotionProductFileInfo = await _promotionProductFileService.GetByIdAsync(promotionProductFileInfoId.Value);

            if (promotionProductFileInfo is null
                || promotionProductFileInfo.ProductId != productId)
            {
                return BadRequest();
            }
        }

        List<PromotionFileInfoEditorDisplayData> possiblePromotionFileInfos = await _promotionFileEditorDataService.GetAllFilesAsync();

        PromotionProductFileInfoSingleEditorPartialModel model = GetPromotionProductFileSingleEditorPartialModel(
            PromotionProductFileInfoSingleEditorPartialModalData,
            productId,
            PromotionProductFileEditorPartialModalData.ModalContentId,
            promotionProductFileInfo,
            possiblePromotionFileInfos,
            productDataElementIndex);

        return Partial("ProductEditor/Promotions/PromotionFiles/_PromotionProductFileInfoSingleEditorPartial", model);
    }

    public async Task<IStatusCodeActionResult> OnGetGetProductXmlByIdAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        string hostPath = $"{Request.Scheme}://{Request.Host}";

        OneOf<string, InvalidXmlResult> getXmlFromEntryResult = await TryGetXmlFromProductEntryAsync(product, hostPath);

        return getXmlFromEntryResult.Match<IStatusCodeActionResult>(
            productXml => Content(productXml, "application/xml"),
            invalidXmlResult => BadRequest(invalidXmlResult));
    }

    private async Task<PartialViewResult> GetProductPropertiesEditorPartialAsync(ModalData modalData, Product product, int? productDataElementIndex = null)
    {
        ProductPropertiesEditorPartialModel productPropertiesEditorPartialModel
            = await GetProductPropertiesEditorModelAsync(modalData, product, productDataElementIndex);

        return Partial("ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", productPropertiesEditorPartialModel);
    }

    private async Task<ProductPropertiesEditorPartialModel> GetProductPropertiesEditorModelAsync(ModalData modalData, Product product, int? productDataElementIndex = null)
    {
        List<ProductImageDisplayData> productImageDisplayDatas = new();

        List<ProductImageData> productImages = await _productImageService.GetAllInProductWithoutFileDataAsync(product.Id);

        foreach (ProductImageData productImage in productImages)
        {
            ProductImageDisplayData productImageDisplayData = new()
            {
                ProductId = productImage.ProductId,
                ExistingImageId = productImage.Id,
                ContentType = productImage.ImageContentType,
                HtmlData = productImage.HtmlData,
                DateModified = productImage.DateModified,
            };

            productImageDisplayDatas.Add(productImageDisplayData);
        }

        List<ProductImageFileDisplayData> productImageFileDisplayDatas = new();

        List<ProductImageFileData> productImageFiles = await _productImageFileService.GetAllInProductAsync(product.Id);

        foreach (ProductImageFileData productImageFile in productImageFiles)
        {
            ProductImageFileDisplayData productImageFileDisplayData = new()
            {
                ProductId = productImageFile.ProductId,
                ExistingFileInfoId = productImageFile.Id,
                FileName = productImageFile.FileName,
                DisplayOrder = productImageFile.DisplayOrder,
                Active = productImageFile.Active,
            };

            productImageFileDisplayDatas.Add(productImageFileDisplayData);
        }

        List<int> relatedCategoryIds = [-1];

        if (product.CategoryId is not null)
        {
            relatedCategoryIds.Add(product.CategoryId.Value);
        }

        List<ProductCharacteristicType> productCharacteristicTypes = [ProductCharacteristicType.ProductCharacteristic, ProductCharacteristicType.Link];

        List<ProductPropertyEditorPropertyData> propertyEditorDataList = await _propertyEditorCharacteristicsService.GetProductPropertyEditorDataForProductAsync(
            product.Id, relatedCategoryIds, productCharacteristicTypes);

        List<Promotion> productPromotions = await _promotionService.GetAllForProductAsync(product.Id);

        List<SearchStringPartOriginData>? searchStringParts
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(product.SearchString, product.CategoryId);

        ProductWorkStatuses? productStatuses = await _productWorkStatusesService.GetByProductIdAsync(product.Id);

        List<PromotionProductFileInfo> promotionProductFiles = await _promotionProductFileService.GetAllForProductAsync(product.Id);

        string? relatedProductTableRowElementId = null;

        if (productDataElementIndex is not null)
        {
            relatedProductTableRowElementId = $"{ProductTableEntryElementIdPrefix}{productDataElementIndex}";
        }

        ProductPropertiesEditorPartialModel productPropertiesEditorPartialModel = new()
        {
            ModalData = modalData,
            ProductFullDisplayPartialModalData = ProductFullDisplayPartialModalData,
            ProductHtmlAndImagesPartialModelData = ProductHtmlAndImagesPartialModelData,
            PromotionProductFileEditorPartialModalData = PromotionProductFileEditorPartialModalData,
            SearchStringPartsPartialModalData = SearchStringPartsPartialModalData,

            ProductTableContainerElementId = ProductTableContainerElementId,

            Product = product,

            ProductStatuses = productStatuses,

            PropertyDataList = propertyEditorDataList,

            ProductImages = productImageDisplayDatas,
            ProductImageFileNames = productImageFileDisplayDatas,

            Promotions = productPromotions,
            SearchStringParts = searchStringParts,

            PromotionProductFileInfos = promotionProductFiles,

            RelatedProductTableRowElementId = relatedProductTableRowElementId,
        };

        return productPropertiesEditorPartialModel;
    }

    
    private PartialViewResult GetSearchStringPartsFromImageTableEntry(
        ModalData modalData,
        Product product,
        List<SearchStringPartOriginData>? searchStringParts = null)
    {
        SearchStringPartsPartialModel searchStringPartsPartialModel = new()
        {
            ModalData = modalData,
            ProductId = product.Id,
            ProductName = product.Name,
            SearchStringParts = searchStringParts
        };

        return Partial("ProductEditor/ProductSearchStringData/_SearchStringPartsPartial", searchStringPartsPartialModel);
    }

    private async Task<PartialViewResult> GetXmlViewFromImageTableEntryAsync(ModalData modalData, Product product, string? applicationRootPath = null)
    {
        OneOf<string, InvalidXmlResult> getXmlFromEntryResult = await TryGetXmlFromProductEntryAsync(product, applicationRootPath);

        XmlViewPartialModel xmlViewPartialModel = new()
        {
            ModalData = modalData,
            Product = product,
            ProductXml = getXmlFromEntryResult
        };

        return Partial("ProductEditor/ProductXmlData/_XmlViewPartial", xmlViewPartialModel);
    }

    private async Task<PartialViewResult> GetPropertiesDisplayFromImageTableEntryAsync(ModalData modalData, Product product)
    {
        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(product.Id);

        ProductPropertiesPartialModel productPropertiesPartialModel = new()
        {
            ModalData = modalData,
            Product = product,
            ProductProperties = productProperties,
        };

        return Partial("ProductEditor/ProductProperties/_ProductPropertiesPartial", productPropertiesPartialModel);
    }

    private async Task<PartialViewResult> GetXmlPropertiesDisplayFromImageTableEntryAsync(
        ModalData modalData,
        Product product,
        LegacyXmlProduct? xmlProduct = null)
    {
        List<ProductCharacteristicAndExternalXmlDataRelation>? alreadyAddedRelations = null;

        int? categoryId = product.CategoryId;

        if (categoryId is not null)
        {
            alreadyAddedRelations = await _characteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryIdAsync(categoryId.Value);
        }

        XmlPropertiesPartialModel xmlPropertiesPartialModel = new()
        {
            ModalData = modalData,
            ImagesCompareTableContainerElementId = ProductTableContainerElementId,
            XmlProduct = xmlProduct,
            PropertyRelations = alreadyAddedRelations,
        };

        return Partial("ProductEditor/ProductXmlData/_XmlPropertiesPartial", xmlPropertiesPartialModel);
    }

    private async Task<PartialViewResult> GetImagesDisplayFromImageTableEntryAsync(ModalData modalData, Product product, bool isOriginalData = true, bool firstImageOnly = false)
    {
        List<ProductImageData>? imagesToDisplay = null;

        if (firstImageOnly)
        {
            ProductImage? firstImage;

            if (isOriginalData)
            {
                firstImage = await _productImageService.GetByProductIdInFirstImagesAsync(product.Id);
            }
            else
            {
				firstImage = await _productImageService.GetByProductIdInFirstImagesAsync(product.Id);
			}

            if (firstImage is not null)
            {
                ProductImageData firstImageWithoutFileData = new()
                {
                    Id = firstImage.Id,
                    ProductId = firstImage.ProductId,
                    ImageContentType = firstImage.ImageContentType,
                    DateModified = firstImage.DateModified,
                    HtmlData = firstImage.HtmlData,
                };

                imagesToDisplay = [firstImageWithoutFileData];
            }
        }
        else
        {
            if (isOriginalData)
            {
                imagesToDisplay = await _productImageService.GetAllInProductWithoutFileDataAsync(product.Id);
            }
            else
            {
				imagesToDisplay = await _productImageService.GetAllInProductWithoutFileDataAsync(product.Id);
			}
		}

        ImagesDisplayPartialModel imageFilesDisplayPartialModel = new()
        {
            ModalData = modalData,
            ProductId = product.Id,
            ProductName = product.Name,
            Images = imagesToDisplay,
            IsOriginalData = isOriginalData,
            NotificationBoxId = NotificationBoxId,
        };

        return Partial("ProductEditor/ProductImages/_ImagesDisplayPartial", imageFilesDisplayPartialModel);
    }

    private async Task<PartialViewResult> GetImageFileNameInfosDisplayFromImageTableEntryAsync(ModalData modalData, Product product)
    {
        ProductImageFileNameInfoViewPartialModel productImageFileNameInfoViewPartialModel = new()
        {
            ModalData = modalData,
            ProductId = product.Id,
            ProductName = product.Name,
            ImageFileNameInfos = await _productImageFileService.GetAllInProductAsync(product.Id),
        };

        return Partial("ProductEditor/ProductImages/_ProductImageFileNameInfoViewPartial", productImageFileNameInfoViewPartialModel);
    }

    private async Task<OneOf<string, InvalidXmlResult>> TryGetXmlFromProductEntryAsync(Product product, string? applicationBasePath = null)
    {
        List<XmlSearchStringPartInfo>? searchStringParts = await TryGetXmlSearchStringPartsFromSearchStringAsync(
            product.SearchString, product.CategoryId);

        SubCategory? productSubCategory = null;

        if (product.SubCategoryId is not null)
        {
            productSubCategory = await _subCategoryService.GetByIdAsync(product.SubCategoryId.Value);
        }

        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(product.Id);
        List<ProductImageData> productImages = await _productImageService.GetAllInProductWithoutFileDataAsync(product.Id);
        List<ProductImageFileData> productImagesFileNameInfos = await _productImageFileService.GetAllInProductAsync(product.Id);

        List<XmlProductImage> xmlProductImages = GetXmlProductImagesFromProductImages(
            productImages, productImagesFileNameInfos, applicationBasePath);

        XmlProduct xmlProduct = _productToNewXmlProductMappingService.MapProductDataToXmlProduct(
            product,
            productProperties: productProperties,
            xmlProductImages: xmlProductImages,
            productSubCategory: productSubCategory,
            xmlProductPromotions: null,
            searchStringPartInfos: searchStringParts);

        ExchangeRate? usdToEurExchangeRate = await _exchangeRateService.GetForCurrenciesAsync(Currency.EUR, Currency.USD);

        XmlExchangeRates xmlExchangeRates = new()
        {
            ExchangeRateUSD = (usdToEurExchangeRate is not null) ? new() { ExchangeRatePerEUR = usdToEurExchangeRate.Rate } : null,
            ValidTo = DateTime.Today,
        };

        ProductsXmlFullData xmlObjectData = new()
        {
            Products = new() { xmlProduct },
            DateOfExport = DateTime.Today,
            ExchangeRates = xmlExchangeRates,
        };

        OneOf<string, InvalidXmlResult> serializeProductXmlResult
            = await _productNewXmlService.TrySerializeProductsXmlAsync(xmlObjectData);

        return serializeProductXmlResult;
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

    private async Task<List<XmlSearchStringPartInfo>?> TryGetXmlSearchStringPartsFromSearchStringAsync(string? searchString, int? categoryId)
    {
        List<SearchStringPartOriginData>? searchStringOriginData
            = await _searchStringOriginService.GetSearchStringPartsAndDataAboutTheirOriginAsync(searchString, categoryId);

        if (searchStringOriginData is null) return null;

        List<XmlSearchStringPartInfo> output = GetSearchStringDataForXmlFromSearchStringData(searchStringOriginData);

        return output;
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

    private PartialViewResult GetPromotionPartialFromImageTableEntry(ModalData modalData, Product product, Promotion promotion)
    {
        PromotionViewPartialModel promotionViewPartialModel = new()
        {
            ModalData = modalData,
            Product = product,
            Promotion = promotion,
        };

        return Partial("ProductEditor/Promotions/_PromotionViewPartial", promotionViewPartialModel);
    }

    private PartialViewResult GetInfoPromotionPartialFromImageTableEntry(ModalData modalData, Product product)
    {
        InfoPromotionViewPartialModel infoPromotionViewPartialModel = new()
        {
            ModalData = modalData,
            Product = product,
        };

        return Partial("ProductEditor/Promotions/_InfoPromotionViewPartial", infoPromotionViewPartialModel);
    }

    private PartialViewResult GetPromotionProductFileEditorPartial(
        PromotionProductFileEditorPartialModel promotionProductFileEditorPartialModel)
    {
        return Partial("ProductEditor/Promotions/PromotionFiles/_PromotionProductFileEditorPartial", promotionProductFileEditorPartialModel);
    }

    private PromotionProductFileInfoSingleEditorPartialModel GetPromotionProductFileSingleEditorPartialModel(
        ModalData modalData,
        int productId,
        string promotionProductFilesTableContainerElementId,
        PromotionProductFileInfo? promotionFileInfo = null,
        List<PromotionFileInfoEditorDisplayData>? possiblePromotionFileInfos = null,
        int? productDataElementIndex = null)
    {
        string? relatedProductTableRowElementId = null;

        if (productDataElementIndex is not null)
        {
            relatedProductTableRowElementId = $"{ProductTableEntryElementIdPrefix}{productDataElementIndex}";
        }

        PromotionProductFileInfoSingleEditorPartialModel model = new()
        {
            ModalData = modalData,
            ProductId = productId,
            PromotionProductFilesTableContainerElementId = promotionProductFilesTableContainerElementId,
            ExistingPromotionProductFileInfo = promotionFileInfo,
            PossiblePromotionFileInfos = possiblePromotionFileInfos,
            ProductPropertiesEditorContainerElementId = ProductPropertiesEditorPartialModalData.ModalContentId,
            ProductTableContainerElementId = ProductTableContainerElementId,
            RelatedProductTableRowElementId = relatedProductTableRowElementId,
        };

        return model;
    }

    public async Task<IActionResult> OnPostGetImageFilesDisplayPartialAsync(int productId)
    {
        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        return await GetImageFilesDisplayFromImageTableEntryAsync(product);
    }

    private async Task<PartialViewResult> GetImageFilesDisplayFromImageTableEntryAsync(Product product)
    {
        List<ProductImageFileData> productImagesFileNameInfos = await _productImageFileService.GetAllInProductAsync(product.Id);

        ImageFilesDisplayPartialModel imageFilesDisplayPartialModel = new()
        {
            ModalData = ImageFilesPartialModalData,
            ProductId = product.Id,
            ProductName = product.Name,
            ImageFileNames = productImagesFileNameInfos,
            NotificationBoxId = NotificationBoxId
        };

        return Partial("ProductEditor/ProductImages/_ImageFilesDisplayPartial", imageFilesDisplayPartialModel);
    }

    private async Task<string> GetProductDataTablePartialAsStringAsync(ProductEditorSearchOptions? searchOptions)
    {
        ProductDataTablePartialModel productDataTablePartialModel
            = await GetPartialModelFromSearchOptionsAsync(searchOptions);

        string productTablePartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, ProductDataTablePartialPath, productDataTablePartialModel);

        return productTablePartialAsString;
    }

    private async Task<string> GetProductDataTableEntryPartialAsStringAsync(Product product, int elementIndex)
    {
        ProductEditorProductData productEditorProductData = await _productEditorProductDataService.GetProductEditorProductDataAsync(product);

        return await GetProductDataTableEntryPartialAsStringAsync(productEditorProductData, elementIndex);
    }

    private async Task<string> GetProductDataTableEntryPartialAsStringAsync(ProductEditorProductData productEditorProductData, int elementIndex)
    {
        ProductDataTableRowPartialModel productDataTablePartialModel
            = GetProductDataTableEntryPartialModel(productEditorProductData, elementIndex);

        string productTablePartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, ProductDataTableEntryPartialPath, productDataTablePartialModel);

        return productTablePartialAsString;
    }

    public async Task<IActionResult> OnPostGetAllSearchedProductsAsync([FromBody] ProductEditorSearchOptions? searchOptions = null)
    {
        if (searchOptions?.ProductNewStatusSearchOptions is not null)
        {
            string? currentUserName = GetUserName(User);

            if (currentUserName is null) return Unauthorized();

            await _productWorkStatusesChangesUpsertService.UpsertChangesIntoProductStatusesAsync(currentUserName);
        }

        ProductDataTablePartialModel productDataTablePartialModel = await GetPartialModelFromSearchOptionsAsync(searchOptions);

        return Partial(ProductDataTablePartialPath, productDataTablePartialModel);
    }

    private async Task<ProductDataTablePartialModel> GetPartialModelFromSearchOptionsAsync(ProductEditorSearchOptions? searchOptions)
    {
        List<Product> products = await GetProductsFromSearchOptionsAsync(searchOptions);

        List<ProductEditorProductData> entries = await _productEditorProductDataService.GetProductEditorProductDatasAsync(products);

        if (searchOptions?.ProductNewStatusSearchOptions != ProductEditorProductNewStatusSearchOptions.LastAdded)
        {
            entries = entries.OrderByDescending(x => x.ProductStatuses?.Id ?? 0)
                .ToList();
        }

        ProductDataTablePartialModel productDataTablePartialModel = GetProductDataTablePartialModel(entries);

        return productDataTablePartialModel;
    }

    private async Task<List<Product>> GetProductsFromSearchOptionsAsync(ProductEditorSearchOptions? searchOptions)
    {
        if (searchOptions is null)
        {
            return await _productService.GetAllAsync();
        }

        if (!string.IsNullOrWhiteSpace(searchOptions.UserInputString))
        {
            ProductSearchRequest userInputSearchRequest = new()
            {
                UserInputString = searchOptions.UserInputString,
            };

            return await _productSearchService.SearchProductsAsync(userInputSearchRequest);
        }

        List<ProductNewStatusSearchOptions>? productNewStatusSearchOptions
            = GetSearchOptionsFromEditorSearchOptions(searchOptions.ProductNewStatusSearchOptions);

        bool filterByLastAdded = productNewStatusSearchOptions?.Contains(ProductNewStatusSearchOptions.LastAdded) ?? false;

        int? maxResultCount = filterByLastAdded ? _lastAddedProductsSearchCount : null;

        ProductSearchRequest productSearchRequest = new()
        {
            CategoryId = searchOptions.CategoryId,
            ProductNewStatuses = productNewStatusSearchOptions,
            ProductStatus = searchOptions.ProductStatusSearchOptions,
            PromotionSearchOptions = searchOptions.PromotionSearchOptions,
            MaxResultCount = maxResultCount,
        };

        return await _productSearchService.SearchProductsAsync(productSearchRequest);
    }

    private static List<ProductNewStatusSearchOptions>? GetSearchOptionsFromEditorSearchOptions(
        ProductEditorProductNewStatusSearchOptions? productEditorProductNewStatusSearchOptions)
    {
        return productEditorProductNewStatusSearchOptions switch
        {
            ProductEditorProductNewStatusSearchOptions.New => [ProductNewStatusSearchOptions.New],
            ProductEditorProductNewStatusSearchOptions.WorkInProgress => [ProductNewStatusSearchOptions.WorkInProgress],
            ProductEditorProductNewStatusSearchOptions.NewAndWorkInProgress => [ProductNewStatusSearchOptions.New, ProductNewStatusSearchOptions.WorkInProgress],
            ProductEditorProductNewStatusSearchOptions.ReadyForUse => [ProductNewStatusSearchOptions.ReadyForUse],
            ProductEditorProductNewStatusSearchOptions.Postponed1 => [ProductNewStatusSearchOptions.Postponed1],
            ProductEditorProductNewStatusSearchOptions.Postponed2 => [ProductNewStatusSearchOptions.Postponed2],
            ProductEditorProductNewStatusSearchOptions.LastAdded => [ProductNewStatusSearchOptions.LastAdded],
            ProductEditorProductNewStatusSearchOptions.LastAddedNew => [ProductNewStatusSearchOptions.LastAdded, ProductNewStatusSearchOptions.New],
            _ => null
        };
    }

    private ProductDataTablePartialModel GetProductDataTablePartialModel(List<ProductEditorProductData> entries)
    {
        return new()
        {
            ProductDisplayDatas = entries,

            ProductTableEntryElementIdPrefix = ProductTableEntryElementIdPrefix,

            PartialViewContainerElementId = ProductTableContainerElementId,
            ProductPropertiesEditorPartialModalData = ProductPropertiesEditorPartialModalData,
            SearchStringPartsPartialModalData = SearchStringPartsPartialModalData,
            XmlViewPartialModalData = XmlViewPartialModalData,
            ProductPropertiesPartialModalData = ProductPropertiesPartialModalData,
            OldXmlPropertiesPartialModalData = OldXmlPropertiesPartialModalData,
            ImagesPartialModalData = ImagesPartialModalData,
            ImageFilesPartialModalData = ImageFilesPartialModalData,
            ImageFileNameInfosPartialModalData = ImageFileNameInfosPartialModalData,
            PromotionViewPartialModalData = PromotionViewPartialModalData,
            InfoPromotionViewPartialModalData = InfoPromotionViewPartialModalData,
            PromotionProductFileEditorPartialModalData = PromotionProductFileEditorPartialModalData,
            PromotionProductFileInfoSingleEditorPartialModalData = PromotionProductFileInfoSingleEditorPartialModalData,
        };
    }

    private ProductDataTableRowPartialModel GetProductDataTableEntryPartialModel(ProductEditorProductData productEditorProductData, int elementIndex)
    {
        return new()
        {
            ProductData = productEditorProductData,
            ElementIndex = elementIndex,
            ProductTableEntryElementIdPrefix = ProductTableEntryElementIdPrefix,

            PartialViewContainerElementId = ProductTableContainerElementId,
            ProductPropertiesEditorPartialModalData = ProductPropertiesEditorPartialModalData,
            SearchStringPartsPartialModalData = SearchStringPartsPartialModalData,
            XmlViewPartialModalData = XmlViewPartialModalData,
            ProductPropertiesPartialModalData = ProductPropertiesPartialModalData,
            OldXmlPropertiesPartialModalData = OldXmlPropertiesPartialModalData,
            ImagesPartialModalData = ImagesPartialModalData,
            ImageFilesPartialModalData = ImageFilesPartialModalData,
            ImageFileNameInfosPartialModalData = ImageFileNameInfosPartialModalData,
            PromotionViewPartialModalData = PromotionViewPartialModalData,
            InfoPromotionViewPartialModalData = InfoPromotionViewPartialModalData,
            PromotionProductFileEditorPartialModalData = PromotionProductFileEditorPartialModalData,
        };
    }

    public async Task<IActionResult> OnPutUpdateProductNewStatusAsync(
        int productId,
        ProductNewStatus productNewStatus,
        int? productDataElementIndex = null)
    {
        if (productNewStatus == ProductNewStatus.WorkInProgress)
        {
            return BadRequest();
        }

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrEmpty(currentUserName)) return Unauthorized();

        ProductWorkStatuses? productStatus = await _productWorkStatusesService.GetByProductIdAsync(productId);

        ServiceProductWorkStatusesUpsertRequest productWorkStatusesUpsertRequest = new()
        {
            ProductId = productId,
            ProductNewStatus = productNewStatus,
            ProductXmlStatus = productStatus?.ProductXmlStatus ?? ProductXmlStatus.NotReady,
            ReadyForImageInsert = productStatus?.ReadyForImageInsert ?? false,
            UpsertUserName = currentUserName,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> updateStatusesResult
            = await _productWorkStatusesService.UpsertByProductIdAsync(productWorkStatusesUpsertRequest);

        return await updateStatusesResult.MatchAsync(
            async id =>
            {
                Product? product = await _productService.GetByIdAsync(productId);

                if (product is null) return StatusCode(StatusCodes.Status500InternalServerError);

                ProductPropertiesEditorPartialModel productPropertiesEditorPartialModel
                    = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

                string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", productPropertiesEditorPartialModel);

                string? productDataTableRowPartialString = null;
                
                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
                    productDataTableRowPartialString = productDataTableRowPartialString,
                };

                return new JsonResult(response);
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnPostAddNewPromotionFileToProductAsync(
        [FromForm(Name = "promotionProductFileInsertData")] PromotionProductFileInsertDTO promotionProductFileInsertData,
        [FromForm(Name = "productDataElementIndex")] int? productDataElementIndex = null)
    {
        if (promotionProductFileInsertData.ProductId is null
            || promotionProductFileInsertData.PromotionFileInfoId is null)
        {
            return BadRequest();
        }

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        Product? product = await _productService.GetByIdAsync(promotionProductFileInsertData.ProductId.Value);

        if (product is null) return NotFound();

        ServicePromotionProductImageCreateRequest? servicePromotionProductImageCreateRequest = null;

        if (promotionProductFileInsertData.ShouldAddToImagesAll)
        {
            OneOf<string, InvalidXmlResult> getProductHtmlResult = await GetProductHtmlFromSavedDataAsync(product);

            if (!getProductHtmlResult.IsT0) return BadRequest();

            string productHtml = getProductHtmlResult.AsT0;

            servicePromotionProductImageCreateRequest = new()
            {
                HtmlData = productHtml,
                ImageFileCreateRequest = new()
                {
                    Active = promotionProductFileInsertData.Active,
                    CustomDisplayOrder = null,
                },
            };
        }

        ServicePromotionProductFileCreateRequest promotionProductFileInfoCreateRequest = new()
        {
            ProductId = promotionProductFileInsertData.ProductId.Value,
            PromotionFileInfoId = promotionProductFileInsertData.PromotionFileInfoId.Value,
            ValidFrom = promotionProductFileInsertData.ValidFrom,
            ValidTo = promotionProductFileInsertData.ValidTo,
            Active = promotionProductFileInsertData.Active,
            CreateInProductImagesRequest = servicePromotionProductImageCreateRequest,
            CreateUserName = currentUserName,
        };

        OneOf<int, ValidationResult, FileSaveFailureResult, FileAlreadyExistsResult, UnexpectedFailureResult> promotionProductFileInfoCreateResult
            = await _promotionProductFileService.InsertAsync(promotionProductFileInfoCreateRequest);

        return await promotionProductFileInfoCreateResult.MatchAsync(
            async id =>
            {
                PromotionProductFileEditorPartialModel model = await GetPromotionProductFileEditorPartialModelForProductAsync(product, productDataElementIndex);

                string promotionProductFileInfoTablePartialAsString
                    = await GetPromotionProductFileEditorPartialAsStringAsync(model);

                PromotionProductFileInfo? insertedPromotionProductFileInfo
                    = model.PromotionProductFileInfos?.FirstOrDefault(x => x.Id == id);

                if (insertedPromotionProductFileInfo is null
                    || PromotionProductFileEditorPartialModalData.ModalContentId is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                List<PromotionFileInfoEditorDisplayData> possiblePromotionFileInfos = await _promotionFileEditorDataService.GetAllFilesAsync();

                string promotionProductFileInfoSingleEditorPartialAsString = await GetPromotionProductFileSingleEditorPartialAsStringAsync(
                    PromotionProductFileInfoSingleEditorPartialModalData,
                    product.Id,
                    PromotionProductFileEditorPartialModalData.ModalContentId,
                    insertedPromotionProductFileInfo,
                    possiblePromotionFileInfos,
                    productDataElementIndex);

                ProductPropertiesEditorPartialModel productPropertiesEditorModel
                    = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

                string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", productPropertiesEditorModel);

                string? productDataTableRowPartialString = null;

                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productDataTableRowPartialString = productDataTableRowPartialString,
                    productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
                    promotionProductFileInfoTablePartialAsString = promotionProductFileInfoTablePartialAsString,
                    promotionProductFileInfoSingleEditorPartialAsString = promotionProductFileInfoSingleEditorPartialAsString,
                };

                return new JsonResult(response);
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileSaveFailureResult => BadRequest(),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnPutUpdatePromotionProductFileAsync(
        [FromForm(Name = "promotionProductFileUpdateData")] PromotionProductFileUpdateDTO promotionProductFileUpdateData,
        [FromForm(Name = "productDataElementIndex")] int? productDataElementIndex = null)
    {
        if (promotionProductFileUpdateData.Id is null)
        {
            return BadRequest();
        }

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        PromotionProductFileInfo? promotionFileInfo
            = await _promotionProductFileService.GetByIdAsync(promotionProductFileUpdateData.Id.Value);

        if (promotionFileInfo is null) return NotFound(promotionProductFileUpdateData.Id);

        Product? product = await _productService.GetByIdAsync(promotionFileInfo.ProductId);

        if (product is null) return NotFound();

        ServicePromotionProductImageUpsertRequest? servicePromotionProductImageUpsertRequest = null;

        if (promotionProductFileUpdateData.ShouldAddToImagesAll)
        {
            OneOf<string, InvalidXmlResult> getProductHtmlResult = await GetProductHtmlFromSavedDataAsync(product);

            if (!getProductHtmlResult.IsT0) return BadRequest();

            string productHtml = getProductHtmlResult.AsT0;

            servicePromotionProductImageUpsertRequest = new()
            {
                HtmlData = productHtml,
                ImageFileUpsertRequest = new()
                {
                    Active = promotionProductFileUpdateData.Active,
                    CustomDisplayOrder = null,
                },
            };
        }

        ServicePromotionProductFileUpdateRequest promotionProductFileInfoCreateRequest = new()
        {
            Id = promotionProductFileUpdateData.Id.Value,
            NewPromotionFileInfoId = promotionProductFileUpdateData.PromotionFileInfoId ?? promotionFileInfo.PromotionFileInfoId,
            ValidFrom = promotionProductFileUpdateData.ValidFrom,
            ValidTo = promotionProductFileUpdateData.ValidTo,
            Active = promotionProductFileUpdateData.Active,
            UpsertInProductImagesRequest = servicePromotionProductImageUpsertRequest,
            UpdateUserName = currentUserName,
        };
        
        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> promotionProductFileInfoCreateResult
            = await _promotionProductFileService.UpdateAsync(promotionProductFileInfoCreateRequest);

        return await promotionProductFileInfoCreateResult.MatchAsync<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult, IStatusCodeActionResult>(
            async success =>
            {
                PromotionProductFileEditorPartialModel model = await GetPromotionProductFileEditorPartialModelForProductAsync(product, productDataElementIndex);

                string promotionProductFileInfoEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/Promotions/PromotionFiles/_PromotionProductFileEditorPartial", model);

                ProductPropertiesEditorPartialModel productPropertiesEditorModel
                    = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

                string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", productPropertiesEditorModel);

                string? productDataTableRowPartialString = null;

                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productDataTableRowPartialString = productDataTableRowPartialString,
                    productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
                    promotionProductFileInfoEditorPartialAsString = promotionProductFileInfoEditorPartialAsString,
                };

                return new JsonResult(response);
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileSaveFailureResult => BadRequest(),
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnDeleteDeletePromotionProductFileAsync(
        int promotionProductFileId,
        int? productDataElementIndex = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        PromotionProductFileInfo? existingPromotionProductFileInfo = await _promotionProductFileService.GetByIdAsync(promotionProductFileId);

        if (existingPromotionProductFileInfo is null) return NotFound(promotionProductFileId);

        Product? product = await _productService.GetByIdAsync(existingPromotionProductFileInfo.ProductId);

        if (product is null) return StatusCode(StatusCodes.Status500InternalServerError);

        OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult> deletePromotionProductFileInfoResult
            = await _promotionProductFileService.DeleteAsync(promotionProductFileId, currentUserName);
        
        return await deletePromotionProductFileInfoResult.MatchAsync<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult, IStatusCodeActionResult>(
            async success =>
            {
                PromotionProductFileEditorPartialModel model = await GetPromotionProductFileEditorPartialModelForProductAsync(product, productDataElementIndex);

                string promotionProductFileInfoEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/Promotions/PromotionFiles/_PromotionProductFileEditorPartial", model);

                ProductPropertiesEditorPartialModel productPropertiesEditorModel
                    = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

                string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", productPropertiesEditorModel);

                string? productDataTableRowPartialString = null;

                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productDataTableRowPartialString = productDataTableRowPartialString,
                    productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
                    promotionProductFileInfoEditorPartialAsString = promotionProductFileInfoEditorPartialAsString,
                };

                return new JsonResult(response);
            },
            notFound => NotFound(notFound),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    private async Task<OneOf<string, InvalidXmlResult>> GetProductHtmlFromSavedDataAsync(Product product)
    {
        List<ProductProperty> productProperties = await _productPropertyService.GetAllInProductAsync(product.Id);

        List<HtmlProductProperty> htmlProductProperties = new();

        foreach (ProductProperty productProperty in productProperties)
        {
            HtmlProductProperty htmlProductProperty = new()
            {
                Name = productProperty.Characteristic,
                Value = productProperty.Value,
                Order = productProperty.DisplayOrder?.ToString(),
            };

            htmlProductProperties.Add(htmlProductProperty);
        }

        HtmlProduct htmlProduct = new()
        {
            Id = product.Id,
            Name = product.Name,
            Properties = htmlProductProperties,
        };

        HtmlProductsData htmlProductsData = new()
        {
            Products = [htmlProduct]
        };

        return _productHtmlService.TryGetHtmlFromProducts(htmlProductsData);
    }

    private async Task<PromotionProductFileEditorPartialModel> GetPromotionProductFileEditorPartialModelForProductAsync(Product product, int? productDataElementIndex = null)
    {
        List<PromotionProductFileInfo> promotionProductFileInfos = await _promotionProductFileService.GetAllForProductAsync(product.Id);

        List<Promotion> promotions = await _promotionService.GetAllForProductAsync(product.Id);

        string? relatedProductTableRowElementId = null;

        if (productDataElementIndex is not null)
        {
            relatedProductTableRowElementId = $"{ProductTableEntryElementIdPrefix}{productDataElementIndex}";
        }

        PromotionProductFileEditorPartialModel promotionProductFileEditorPartialModel = new()
        {
            ModalData = PromotionProductFileEditorPartialModalData,
            PromotionProductFileInfoSingleEditorPartialModalData = PromotionProductFileInfoSingleEditorPartialModalData,
            ProductPropertiesEditorContainerElementId = ProductPropertiesEditorPartialModalData.ModalContentId,
            ProductTableContainerElementId = ProductTableContainerElementId,
            Product = product,
            Promotions = promotions,
            PromotionProductFileInfos = promotionProductFileInfos,
            RelatedProductTableRowElementId = relatedProductTableRowElementId,
        };

        return promotionProductFileEditorPartialModel;
    }

    private async Task<string> GetPromotionProductFileEditorPartialAsStringAsync(PromotionProductFileEditorPartialModel model)
    {
        return await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, "ProductEditor/Promotions/PromotionFiles/_PromotionProductFileEditorPartial", model);
    }

    private async Task<string> GetPromotionProductFileSingleEditorPartialAsStringAsync(
        ModalData modalData,
        int productId,
        string promotionProductFilesTableContainerElementId,
        PromotionProductFileInfo? promotionFileInfo = null,
        List<PromotionFileInfoEditorDisplayData>? possiblePromotionFileInfos = null,
        int? productDataElementIndex = null)
    {
        PromotionProductFileInfoSingleEditorPartialModel model = GetPromotionProductFileSingleEditorPartialModel(
            modalData,
            productId,
            promotionProductFilesTableContainerElementId,
            promotionFileInfo,
            possiblePromotionFileInfos,
            productDataElementIndex);

        return await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, "ProductEditor/Promotions/PromotionFiles/_PromotionProductFileInfoSingleEditorPartial", model);
    }

    public async Task<IActionResult> OnPostSaveAllMatchedXmlPropertiesAndImagesForProductsAsync(
        [FromBody] SaveAllMatchedXmlPropertiesAndImagesForProductsRequestBodyDTO requestBody)
    {
        string? createUserName = GetUserName(User);

        if (createUserName is null) return Unauthorized();

        List<Product> products = new();

        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlDataResult = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getProductXmlDataResult.MatchAsync(
            (Func<LegacyXmlObjectData, Task<IStatusCodeActionResult>>)(async productXmlData =>
            {
                foreach (int productId in requestBody.ProductIds)
                {
                    Product? product = await _productService.GetByIdAsync(productId);

                    if (product is null) continue;

                    LegacyXmlProduct? xmlProduct = productXmlData.Products.FirstOrDefault(x => x.Id == productId);

                    List<ProductImage> originalImages = await this._productImageService.GetAllInProductAsync(productId);

                    IStatusCodeActionResult saveXmlPropertiesAndImagesOfProduct
                        = await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                            () => SaveAllXmlPropertiesAndAllImagesOfProductAsync(product, originalImages, createUserName, xmlProduct),
                            result => result.StatusCode == 200);

                    if (saveXmlPropertiesAndImagesOfProduct.StatusCode != 200) return saveXmlPropertiesAndImagesOfProduct;
                }

                ProductDataTablePartialModel productDataTablePartialModel
                    = await GetPartialModelFromSearchOptionsAsync(requestBody.SearchOptions);

                return base.Partial(ProductDataTablePartialPath, productDataTablePartialModel);
            }),
            invalidXmlResult => base.BadRequest(),
            notFound => base.NotFound());
    }

    public async Task<IActionResult> OnPostSaveAllMatchedXmlPropertiesAndImagesAsync(int productId,
        [FromBody] ProductEditorSearchOptions? searchOptions = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return BadRequest();

        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlDataResult
            = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getProductXmlDataResult.MatchAsync(
            (Func<LegacyXmlObjectData, Task<IActionResult>>)(async productXmlData =>
            {
                LegacyXmlProduct? xmlProduct = productXmlData.Products.FirstOrDefault(x => x.Id == productId);

                List<ProductImage> originalImages = await this._productImageService.GetAllInProductAsync(productId);

                IStatusCodeActionResult saveXmlPropertiesAndImagesOfProduct
                    = await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
                        () => SaveAllXmlPropertiesAndAllImagesOfProductAsync(product, originalImages, currentUserName, xmlProduct),
                        result => result.StatusCode == 200);

                if (saveXmlPropertiesAndImagesOfProduct.StatusCode != 200) return saveXmlPropertiesAndImagesOfProduct;

                ProductDataTablePartialModel productDataTablePartialModel
                    = await GetPartialModelFromSearchOptionsAsync(searchOptions);

                return base.Partial(ProductDataTablePartialPath, productDataTablePartialModel);
            }),
            invalidXmlResult => base.BadRequest(invalidXmlResult),
            notFound => base.NotFound());
    }

    private async Task<IStatusCodeActionResult> SaveAllXmlPropertiesAndAllImagesOfProductAsync(
        Product product, List<ProductImage> images, string createUserName, LegacyXmlProduct? xmlProduct = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        if (xmlProduct is not null)
        {
            IStatusCodeActionResult saveMatchedXmlPropertiesResult = await SaveAllMatchedXmlPropertiesAsync(product, xmlProduct);

            if (saveMatchedXmlPropertiesResult.StatusCode != 200) return saveMatchedXmlPropertiesResult;
        }

        IStatusCodeActionResult saveAllImagesResult = await SaveAllImagesForProductToDirectoryAsync(product.Id, images, createUserName);

        return saveAllImagesResult;
    }

    private async Task<IStatusCodeActionResult> SaveAllMatchedXmlPropertiesAsync(Product product, LegacyXmlProduct xmlProduct)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        if (product.CategoryId is null)
        {
            return BadRequest();
        }

        List<ProductCharacteristicAndExternalXmlDataRelation> propertyRelationshipsFromProductCategory
            = await _characteristicAndExternalXmlDataRelationService.GetAllWithSameCategoryIdAsync(product.CategoryId.Value);

        List<ProductPropertyForProductUpsertRequest> productPropertyUpsertRequests = new();

        foreach (LegacyXmlProductProperty xmlProperty in xmlProduct.Properties)
        {
            ProductCharacteristicAndExternalXmlDataRelation? propertyRelationship = propertyRelationshipsFromProductCategory
                .FirstOrDefault(x => x.XmlName == xmlProperty.Name && x.XmlDisplayOrder.ToString() == xmlProperty.Order);

            if (propertyRelationship is null
                || propertyRelationship.ProductCharacteristicId is null)
            {
                continue;
            }

            ProductPropertyForProductUpsertRequest productPropertyUpsertRequest = new()
            {
                ProductCharacteristicId = propertyRelationship.ProductCharacteristicId.Value,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
                Value = xmlProperty.Value,
            };

            productPropertyUpsertRequests.Add(productPropertyUpsertRequest);
        }

        if (xmlProduct.VendorUrl is not null)
        {
            ProductPropertyForProductUpsertRequest defaultLinkProductPropertyUpsertRequest = new()
            {
                ProductCharacteristicId = _defaultLinkCharacteristicId,
                XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
                Value = xmlProduct.VendorUrl,
            };

            productPropertyUpsertRequests.Add(defaultLinkProductPropertyUpsertRequest);
        }

        ProductPropertyUpsertAllForProductRequest productPropertyUpsertAllRequest = new()
        {
            ProductId = product.Id,
            NewProperties = productPropertyUpsertRequests,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertAllResult
            = await _productPropertyService.UpsertAllProductPropertiesAsync(productPropertyUpsertAllRequest, currentUserName);

        return upsertAllResult.Match(
            success => new OkResult(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500));
    }

    public async Task<IActionResult> OnPostSaveXmlPropertyForProductToLocalPropertiesAsync(
        int productId,
        int relationshipId,
        [FromBody] ProductEditorSearchOptions? productEditorSearchOptions = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return BadRequest();

        OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlDataResult
            = await _productXmlProvidingService.GetProductXmlParsedAsync();

        return await getProductXmlDataResult.MatchAsync(
            async productXmlData =>
            {
                LegacyXmlProduct? xmlProduct = productXmlData.Products.FirstOrDefault(x => x.Id == productId);

                ProductCharacteristicAndExternalXmlDataRelation? propertyRelationship
                    = await _characteristicAndExternalXmlDataRelationService.GetByIdAsync(relationshipId);

                if (propertyRelationship is null
                    || propertyRelationship.CategoryId != product.CategoryId
                    || propertyRelationship.ProductCharacteristicId is null)
                {
                    return BadRequest();
                }

                LegacyXmlProductProperty? xmlPropertyToSave = xmlProduct?.Properties
                    .FirstOrDefault(x => x.Name == propertyRelationship.XmlName && x.Order == propertyRelationship.XmlDisplayOrder.ToString());

                if (xmlPropertyToSave is null) return BadRequest();

                IStatusCodeActionResult insertPropertyActionResult = await UpsertXmlPropertyInPropertiesAsync(
                    productId, xmlPropertyToSave, propertyRelationship, currentUserName);

                if (insertPropertyActionResult.StatusCode != 200) return insertPropertyActionResult;

                ProductDataTablePartialModel productDataTablePartialModel
                    = await GetPartialModelFromSearchOptionsAsync(productEditorSearchOptions);

                return Partial(ProductDataTablePartialPath, productDataTablePartialModel);
            },
            invalidXmlResult => BadRequest(invalidXmlResult),
            notFound => NotFound());
    }

    private async Task<IStatusCodeActionResult> UpsertXmlPropertyInPropertiesAsync(
        int productId,
        LegacyXmlProductProperty xmlProperty,
        ProductCharacteristicAndExternalXmlDataRelation propertyRelationship,
        string currentUserName)
    {
        if (propertyRelationship.ProductCharacteristicId is null) return BadRequest();

        ProductPropertyUpdateRequest productPropertyUpdateRequest = new()
        {
            ProductCharacteristicId = propertyRelationship.ProductCharacteristicId.Value,
            ProductId = productId,
            XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList,
            Value = xmlProperty.Value,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> upsertPropertyResult
            = await _productPropertyService.UpsertAsync(productPropertyUpdateRequest, currentUserName);

        return upsertPropertyResult.Match(
            success => new OkResult(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    private async Task<IStatusCodeActionResult> SaveAllImagesForProductToDirectoryAsync(int productId, List<ProductImage> images, string createUserName)
    {
        IStatusCodeActionResult saveDistinctImagesResult = await _transactionExecuteService.ExecuteActionInTransactionAndCommitWithConditionAsync(
            () => SaveProductImagesToDirectoryAsync(productId, images, createUserName),
            result => result.StatusCode == 200);

        return saveDistinctImagesResult;
    }

    private async Task<IStatusCodeActionResult> SaveProductImagesToDirectoryAsync(
        int productId, List<ProductImage> images, string upsertUserName)
    {
        List<ProductImageWithFileForProductUpsertRequest> imageUpsertRequests = new();

        foreach (ProductImage image in images)
        {
            if (image.ImageData is null)
            {
                ValidationFailure validationFailure = new(nameof(ProductImage.ImageData), "Image Data cannot be empty");

                ValidationResult validationResult = new([validationFailure]);

                return GetBadRequestResultFromValidationResult(validationResult);
            }

            OneOf<string, ValidationResult> getFileExtensionResult = _fileExtensionFromContentTypeService.GetFileExtensionFromImageContentType(image.ImageContentType);

            if (!getFileExtensionResult.IsT0)
            {
                return getFileExtensionResult.Match(
                    fileExtension => StatusCode(StatusCodes.Status500InternalServerError),
                    validationResult => GetBadRequestResultFromValidationResult(validationResult));
            }

            ProductImageWithFileForProductUpsertRequest productImageWithFileUpsertRequest = new()
            {
                ExistingImageId = null,
                FileExtension = getFileExtensionResult.AsT0,
                ImageData = image.ImageData,
                HtmlData = image.HtmlData,
                FileUpsertRequest = new()
                {
                    Active = true,
                    CustomDisplayOrder = null,
                }
            };

            imageUpsertRequests.Add(productImageWithFileUpsertRequest);
        }

        OneOf<Success, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertAllResult
            = await _productImageService.UpsertFirstAndAllImagesWithFilesForProductAsync(productId, imageUpsertRequests, upsertUserName);

        return upsertAllResult.Match(
            success => new OkResult(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileSaveFailureResult => BadRequest(),
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnPostUpsertImageAsync(int? productDataElementIndex = null,
        [FromForm(Name = "imageUpsertData")] ProductImageUpsertDTO? imageUpsertData = null)
    {
        if (imageUpsertData is null
            || imageUpsertData.IncludeHtmlDataView is null
            || imageUpsertData.ProductId is null
            || imageUpsertData.File is null)
        {
            return BadRequest();
        }

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        using MemoryStream stream = new();

        imageUpsertData.File.CopyTo(stream);

        byte[] fileData = stream.ToArray();

        if (fileData.Length <= 0) return BadRequest();

        string fileName = Path.GetFileName(imageUpsertData.File.FileName.Trim());

        string fileExtension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(fileExtension)) return BadRequest();

        int productId = imageUpsertData.ProductId.Value;

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        FileForImageUpsertRequest? fileForImageUpsertRequest = null;

        if (imageUpsertData.FileUpsertData is not null)
        {
            fileForImageUpsertRequest = new()
            {
                CustomDisplayOrder = imageUpsertData.FileUpsertData.DisplayOrder,
                Active = imageUpsertData.FileUpsertData.Active,
                UpsertUserName = currentUserName,
            };
        }

        ProductImageWithFileUpsertRequest imageWithFileUpsertRequest = new()
        {
            ProductId = productId,
            ExistingImageId = imageUpsertData.ExistingImageId,
            FileExtension = fileExtension,
            ImageData = fileData,
            HtmlData = null,
            FileUpsertRequest = fileForImageUpsertRequest,
        };

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> upsertResult
            = await _productImageService.UpsertInAllImagesWithFileAsync(imageWithFileUpsertRequest);

        if (!upsertResult.IsT0)
        {
            return upsertResult.Match(
                ids => StatusCode(StatusCodes.Status500InternalServerError),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                fileSaveFailureResult => BadRequest(),
                fileDoesntExistResult => NotFound(fileDoesntExistResult.FileName),
                fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
                unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
        }

        OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
            = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                productId, ProductNewStatus.WorkInProgress, currentUserName);

        if (!upsertProductStatusResult.IsT0)
        {
            return upsertProductStatusResult.Match(
                statusId => StatusCode(StatusCodes.Status500InternalServerError),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
        }

        ProductPropertiesEditorPartialModel model
            = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

        string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", model);

        string? productDataTableRowPartialString = null;

        if (productDataElementIndex is not null)
        {
            productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
        }

        var response = new
        {
            productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
            productDataTableRowPartialString = productDataTableRowPartialString,
        };

        return new JsonResult(response);
    }

    public async Task<IStatusCodeActionResult> OnPutUpdateImageDisplayOrderAsync(
        int productId,
        int imageId,
        int newDisplayOrder,
        int? productDataElementIndex = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        ProductImageFileData? imageFileData = await _productImageFileService.GetByProductIdAndImageIdAsync(productId, imageId);

        ProductImage? image = await _productImageService.GetByIdInAllImagesAsync(imageId);

        if (image is null) return BadRequest();

        if (image.ImageData is null) return StatusCode(StatusCodes.Status500InternalServerError);

        if (image.ProductId != productId) return BadRequest();

        OneOf<string, ValidationResult> getFileExtensionResult = _fileExtensionFromContentTypeService.GetFileExtensionFromImageContentType(image.ImageContentType);

        if (!getFileExtensionResult.IsT0)
        {
            return getFileExtensionResult.Match(
                fileExtension => StatusCode(StatusCodes.Status500InternalServerError),
                validationResult => GetBadRequestResultFromValidationResult(validationResult));
        }

        string fileExtension = getFileExtensionResult.AsT0;

        ProductImageWithFileUpdateRequest productImageFileNameInfoCreateRequest = new()
        {
            ImageId = imageId,
            FileExtension = fileExtension,
            ImageData = image.ImageData,
            HtmlData = image.HtmlData,
            CustomDisplayOrder = newDisplayOrder,
            Active = true,
            UpdateUserName = currentUserName,
        };

        OneOf<ImageAndFileIdsInfo, ValidationResult, FileSaveFailureResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateImageAndFileResult
            = await _productImageService.UpdateInAllImagesWithFileAsync(productImageFileNameInfoCreateRequest);

        if (!updateImageAndFileResult.IsT0)
        {
            return updateImageAndFileResult.Match(
                imageAndFileIds => StatusCode(StatusCodes.Status500InternalServerError),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                fileSaveFailureResult => BadRequest(),
                fileDoesntExistResult => NotFound(fileDoesntExistResult),
                fileAlreadyExistsResult => BadRequest(),
                unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
        }

        ProductPropertiesEditorPartialModel model
            = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

        string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", model);

        string? productDataTableRowPartialString = null;

        if (productDataElementIndex is not null)
        {
            productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
        }

        var response = new
        {
            productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
            productDataTableRowPartialString = productDataTableRowPartialString,
        };

        return new JsonResult(response);
    }

    public async Task<IActionResult> OnDeleteDeleteImageAsync(
        int imageId,
        bool deleteFile,
        int? productDataElementIndex = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        ProductImageData? image = await _productImageService.GetByIdInAllImagesWithoutFileDataAsync(imageId);

        if (image is null) return NotFound();

        if (image.ProductId is null) return StatusCode(StatusCodes.Status500InternalServerError);

        Product? product = await _productService.GetByIdAsync(image.ProductId.Value);

        if (product is null) return StatusCode(StatusCodes.Status500InternalServerError);

        if (!deleteFile)
        {
            OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult> deleteImageResult
                = await _productImageService.DeleteInAllImagesByIdAsync(imageId, currentUserName);

            return await deleteImageResult.MatchAsync(
                async success =>
                {
                    OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
                        = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                            image.ProductId.Value, ProductNewStatus.WorkInProgress, currentUserName);

                    if (!upsertProductStatusResult.IsT0)
                    {
                        return upsertProductStatusResult.Match(
                            statusId => StatusCode(StatusCodes.Status500InternalServerError),
                            validationResult => GetBadRequestResultFromValidationResult(validationResult),
                            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
                    }

                    ProductPropertiesEditorPartialModel model
                        = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

                    string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                        this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", model);

                    string? productDataTableRowPartialString = null;

                    if (productDataElementIndex is not null)
                    {
                        productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                    }

                    var response = new
                    {
                        productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
                        productDataTableRowPartialString = productDataTableRowPartialString,
                    };

                    return new JsonResult(response);
                },
                notFound => NotFound(),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
        }

        OneOf<Success, NotFound, ValidationResult, FileDoesntExistResult, UnexpectedFailureResult> deleteImageAndFileResult
            = await _productImageService.DeleteInAllImagesByIdWithFileAsync(imageId, currentUserName);

        return await deleteImageAndFileResult.MatchAsync(
            async success =>
            {
                OneOf<int?, ValidationResult, UnexpectedFailureResult> upsertProductStatusResult
                    = await _productWorkStatusesWorkflowService.UpsertProductNewStatusToGivenStatusIfItsNewAsync(
                        image.ProductId.Value, ProductNewStatus.WorkInProgress, currentUserName);

                if (!upsertProductStatusResult.IsT0)
                {
                    return upsertProductStatusResult.Match(
                        statusId => StatusCode(StatusCodes.Status500InternalServerError),
                        validationResult => GetBadRequestResultFromValidationResult(validationResult),
                        unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
                }

                ProductPropertiesEditorPartialModel model
                    = await GetProductPropertiesEditorModelAsync(ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

                string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", model);

                string? productDataTableRowPartialString = null;

                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
                    productDataTableRowPartialString = productDataTableRowPartialString,
                };

                return new JsonResult(response);
            },
            notFound => NotFound(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileDoesntExistResult => NotFound(fileDoesntExistResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnPostAddNewPropertyAsync(
        int productId,
        ProductCharacteristicType productCharacteristicType,
        int propertyEditorPropertyGroupValue,
        int? productDataElementIndex = null,
        [FromBody] List<ProductPropertyEditorPropertyDTO>? productPropertyEditorDataList = null)
    {
        ProductPropertyEditorPropertyGroup? propertyEditorPropertyGroup = ProductPropertyEditorPropertyGroup.GetFromValue(propertyEditorPropertyGroupValue);

        if (propertyEditorPropertyGroup is null) return NotFound();

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null
            || product.CategoryId is null)
        {
            return BadRequest();
        }

        int highestTableRowIndex = GetHighestTableRowIndex(productPropertyEditorDataList);

        List<int> relatedCategoryIds = [-1];

        if (product.CategoryId is not null)
        {
            relatedCategoryIds.Add(product.CategoryId.Value);
        }

        ProductPropertyEditorPropertyData propertyData = await _propertyEditorCharacteristicsService.GetPropertyEditorPropertyDataAsync(product, null, productCharacteristicType);

        ProductPropertiesEditorSinglePropertyPartialModel singlePropertyPartialModel = GeneratePropertyEditorData(
            productId, highestTableRowIndex + 1, propertyData, propertyEditorPropertyGroup, productDataElementIndex);

        return Partial("ProductEditor/ProductProperties/_ProductPropertiesEditorSinglePropertyPartial", singlePropertyPartialModel);
    }

    private static int GetHighestTableRowIndex(List<ProductPropertyEditorPropertyDTO>? productPropertyEditorDataList)
    {
        int highestTableRowIndex = -1;

        if (productPropertyEditorDataList is null) return highestTableRowIndex;

        foreach (ProductPropertyEditorPropertyDTO productPropertyEditorData in productPropertyEditorDataList)
        {
            if (highestTableRowIndex >= productPropertyEditorData.PropertyIndex) continue;

            highestTableRowIndex = productPropertyEditorData.PropertyIndex;
        }

        return highestTableRowIndex;
    }

    public async Task<IActionResult> OnPutChangePropertyCharacteristicIdAsync(
        int productId,
        int propertyIndex,
        int oldCharacteristicId,
        int propertyEditorPropertyGroupValue,
        [FromBody] ProductPropertyEditorUpsertRequestBodyDTO requestBody,
        int? productDataElementIndex = null)
    {
        if (requestBody?.ProductPropertyEditorDataList is null) return BadRequest();

        ProductPropertyEditorPropertyDTO? productPropertyEditorData = requestBody.ProductPropertyEditorDataList.FirstOrDefault(
           x => x.PropertyIndex == propertyIndex && x.PropertyGroup == propertyEditorPropertyGroupValue);

        if (productPropertyEditorData?.ProductCharacteristicId is null) return BadRequest();

        ProductPropertyEditorPropertyGroup? propertyEditorPropertyGroup = ProductPropertyEditorPropertyGroup.GetFromValue(propertyEditorPropertyGroupValue);

        if (propertyEditorPropertyGroup is null) return NotFound();

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        if (propertyIndex < 0
            || propertyIndex >= requestBody.ProductPropertyEditorDataList.Count)
        {
            return BadRequest();
        }

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null
            || product.CategoryId is null)
        {
            return BadRequest();
        }

        int newCharacteristicId = productPropertyEditorData.ProductCharacteristicId.Value;

        OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult> propertyChangeIdResult
            = await _productPropertyService.ChangePropertyCharacteristicIdAsync(
                productId, oldCharacteristicId, newCharacteristicId, currentUserName);

        return await propertyChangeIdResult.MatchAsync<Success, NotFound, ValidationResult, UnexpectedFailureResult, IStatusCodeActionResult>(
            async success =>
            {
                ProductWorkStatuses? productStatuses = await _productWorkStatusesService.GetByProductIdAsync(productId);

                ProductCharacteristic? characteristic = await _productCharacteristicService.GetByIdAsync(newCharacteristicId);

                ProductPropertyEditorPropertyData propertyData
                    = await _propertyEditorCharacteristicsService.GetPropertyEditorPropertyDataAsync(product, newCharacteristicId, characteristic?.KWPrCh);

                ProductPropertiesEditorSinglePropertyPartialModel propertyPartialModel
                    = GeneratePropertyEditorData(productId, propertyIndex, propertyData, propertyEditorPropertyGroup, productDataElementIndex);

                string propertyPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorSinglePropertyPartial", propertyPartialModel);

                string? productDataTableRowPartialString = null;

                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productNewStatus = (int?)productStatuses?.ProductNewStatus,

                    propertyPartialAsString = propertyPartialAsString,

                    productDataTableRowPartialString = productDataTableRowPartialString,
                };

                return new JsonResult(response);
            },
            notFound => NotFound(),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }
    
    public async Task<IActionResult> OnPutUpsertPropertyAsync(
        int productId,
        int propertyIndex,
        int propertyEditorPropertyGroupValue,
        [FromBody] ProductPropertyEditorUpsertRequestBodyDTO requestBody,
        int? productDataElementIndex = null)
    {
        if (requestBody.ProductPropertyEditorDataList is null) return BadRequest();

        ProductPropertyEditorPropertyDTO? productPropertyEditorData = requestBody.ProductPropertyEditorDataList.FirstOrDefault(
            x => x.PropertyIndex == propertyIndex && x.PropertyGroup == propertyEditorPropertyGroupValue);

        if (productPropertyEditorData?.ProductCharacteristicId is null) return BadRequest();

        ProductPropertyEditorPropertyGroup? propertyEditorPropertyGroup = ProductPropertyEditorPropertyGroup.GetFromValue(propertyEditorPropertyGroupValue);

        if (propertyEditorPropertyGroup is null) return NotFound();

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        if (propertyIndex < 0
            || propertyIndex >= requestBody.ProductPropertyEditorDataList.Count)
        {
            return BadRequest();
        }

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null
            || product.CategoryId is null)
        {
            return BadRequest();
        }
        
        int characteristicId = productPropertyEditorData.ProductCharacteristicId.Value;

        ProductCharacteristic? characteristic = await _productCharacteristicService.GetByIdAsync(characteristicId);

        if (characteristic is null) return NotFound();

        ProductPropertyUpdateRequest propertyUpsertRequest = new()
        {
            ProductId = productId,
            ProductCharacteristicId = characteristicId,
            Value = productPropertyEditorData.Value,
            XmlPlacement = productPropertyEditorData.XmlPlacement ?? 0,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> propertyUpsertResult
            = await _productPropertyService.UpsertAsync(propertyUpsertRequest, currentUserName);

        return await propertyUpsertResult.MatchAsync<Success, ValidationResult, UnexpectedFailureResult, IStatusCodeActionResult>(
            async success =>
            {
                ProductWorkStatuses? productStatuses = await _productWorkStatusesService.GetByProductIdAsync(productId);

                ProductPropertyEditorPropertyData propertyData = await _propertyEditorCharacteristicsService.GetPropertyEditorPropertyDataAsync(product, characteristicId, characteristic.KWPrCh);

                ProductPropertiesEditorSinglePropertyPartialModel propertyPartialModel
                    = GeneratePropertyEditorData(productId, propertyIndex, propertyData, propertyEditorPropertyGroup, productDataElementIndex);

                string propertyPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorSinglePropertyPartial", propertyPartialModel);

                string? productDataTableRowPartialString = null;

                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productNewStatus = (int?)productStatuses?.ProductNewStatus,

                    propertyPartialAsString = propertyPartialAsString,

                    productDataTableRowPartialString = productDataTableRowPartialString,
                };

                return new JsonResult(response);
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnPutUpsertAllProductPropertiesAsync(
        int productId,
        [FromBody] ProductPropertyEditorUpsertRequestBodyDTO requestBody,
        int? productDataElementIndex = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null)
        {
            return NotFound();
        }

        List<ProductPropertyForProductUpsertRequest> productPropertyUpsertRequests = new();

        if (requestBody.ProductPropertyEditorDataList?.Count > 0)
        {
            foreach (ProductPropertyEditorPropertyDTO productPropertyEditorData in requestBody.ProductPropertyEditorDataList)
            {
                if (productPropertyEditorData.ProductCharacteristicId is null) continue;

                ProductPropertyForProductUpsertRequest productPropertyUpsertRequest = new()
                {
                    ProductCharacteristicId = productPropertyEditorData.ProductCharacteristicId.Value,
                    Value = productPropertyEditorData.Value,
                    XmlPlacement = productPropertyEditorData.XmlPlacement,
                };

                productPropertyUpsertRequests.Add(productPropertyUpsertRequest);
            }
        }

        ProductPropertyUpsertAllForProductRequest upsertAllPropertiesForProductRequest = new()
        {
            ProductId = productId,
            NewProperties = productPropertyUpsertRequests,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> changeAllPropertiesResult
            = await _productPropertyService.UpsertAllProductPropertiesAsync(upsertAllPropertiesForProductRequest, currentUserName);

        return await changeAllPropertiesResult.MatchAsync<Success, ValidationResult, UnexpectedFailureResult, IStatusCodeActionResult>(
            async success =>
            {
                ProductPropertiesEditorPartialModel model = await GetProductPropertiesEditorModelAsync(
                    ProductPropertiesEditorPartialModalData, product, productDataElementIndex);

                string productPropertiesEditorPartialAsString = await _partialViewRenderService.RenderPartialViewToStringAsync(
                    this, "ProductEditor/ProductProperties/_ProductPropertiesEditorPartial", model);

                string? productDataTableRowPartialString = null;

                if (productDataElementIndex is not null)
                {
                    productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
                }

                var response = new
                {
                    productPropertiesEditorPartialAsString = productPropertiesEditorPartialAsString,
                    productDataTableRowPartialString = productDataTableRowPartialString,
                };

                return new JsonResult(response);
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IActionResult> OnDeleteDeletePropertyAsync(
        int productId,
        int productCharacteristicId,
        int? productDataElementIndex = null)
    {
        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        Product? product = await _productService.GetByIdAsync(productId);

        if (product is null) return NotFound();

        OneOf<Success, NotFound, ValidationResult, UnexpectedFailureResult> propertyDeleteResult
            = await _productPropertyService.DeleteAsync(productId, productCharacteristicId, currentUserName);

        if (!propertyDeleteResult.IsT0)
        {
            return propertyDeleteResult.Match(
                success => StatusCode(StatusCodes.Status500InternalServerError),
                notFound => NotFound(),
                validationResult => GetBadRequestResultFromValidationResult(validationResult),
                unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
        }

        ProductWorkStatuses? productStatuses = await _productWorkStatusesService.GetByProductIdAsync(productId);

        string? productDataTableRowPartialString = null;

        if (productDataElementIndex is not null)
        {
            productDataTableRowPartialString = await GetProductDataTableEntryPartialAsStringAsync(product, productDataElementIndex.Value);
        }

        var response = new
        {
            productNewStatus = (int?)productStatuses?.ProductNewStatus,
            productDataTableRowPartialString = productDataTableRowPartialString,
        };

        return new JsonResult(response);
    }

    private ProductPropertiesEditorSinglePropertyPartialModel GeneratePropertyEditorData(
        int productId,
        int tableRowIndex,
        ProductPropertyEditorPropertyData propertyData,
        ProductPropertyEditorPropertyGroup propertyEditorPropertyGroup,
        int? productDataElementIndex = null)
    {
        string? relatedProductTableRowElementId = null;

        if (productDataElementIndex is not null)
        {
            relatedProductTableRowElementId = $"{ProductTableEntryElementIdPrefix}{productDataElementIndex}";
        }

        ProductPropertiesEditorSinglePropertyPartialModel singlePropertyPartialModel = new()
        {
            ProductId = productId,
            PropertyData = propertyData,
            PropertyIndex = tableRowIndex,
            PropertyEditorPropertyGroup = propertyEditorPropertyGroup,
            ProductTableContainerElementId = ProductTableContainerElementId,
            NotificationBoxElementId = NotificationBoxId,
            RelatedProductTableRowElementId = relatedProductTableRowElementId,
        };

        return singlePropertyPartialModel;
    }
}