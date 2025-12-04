using OneOf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.Models.ProductSearch;
using MOSTComputers.UI.Web.Services.Data.Search.Contracts;
using MOSTComputers.UI.Web.Services.Data.Xml.Contracts;

using static MOSTComputers.UI.Web.Utils.SelectListItemUtils;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

namespace MOSTComputers.UI.Web.Pages;

public sealed class XmlDownloadModel : PageModel
{
    public XmlDownloadModel(
        ICategoryService categoryService,
        IProductService productService,
        IProductSearchService productSearchService,
        IProductXmlService productXmlService,
        IProductXmlValidationService productXmlValidationService,
        IProductXmlFromProductDataService productXmlFromProductDataService)
    {
        _categoryService = categoryService;
        _productService = productService;
        _productSearchService = productSearchService;
        _productNewXmlService = productXmlService;
        _productXmlValidationService = productXmlValidationService;
        _newProductXmlFromProductDataService = productXmlFromProductDataService;
    }

    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IProductSearchService _productSearchService;
    private readonly IProductXmlService _productNewXmlService;
    private readonly IProductXmlValidationService _productXmlValidationService;
    private readonly IProductXmlFromProductDataService _newProductXmlFromProductDataService;

    public void OnGet()
    {
    }

    public async Task<List<SelectListItem>> GetAllCategorySelectListItemsAsync(int? selectedCategoryId = null)
    {
        List<Category> categories = await _categoryService.GetAllAsync();

        bool isAnySelected = selectedCategoryId is not null;

        SelectListItem defaultSelectListItem = new("Всички", null, isAnySelected, false);

        List<SelectListItem> selectListItems = GetCategorySelectListItems(categories, selectedCategoryId, defaultSelectListItem);

        return selectListItems;
    }

    public async Task<IStatusCodeActionResult> OnPostGetXmlDataForSearchedProductsAsync([FromBody] ProductSearchRequest? searchOptions = null)
    {
        string hostPath = $"{Request.Scheme}://{Request.Host}";

        List<Product> products = await GetProductsFromSearchOptionsAsync(searchOptions);

        ProductsXmlFullData xmlObjectData = await _newProductXmlFromProductDataService.GetXmlObjectDataForProductsAsync(products, hostPath);

        for (int i = 0; i < xmlObjectData.Products.Count; i++)
        {
            XmlProduct xmlProduct = xmlObjectData.Products[i];

            if (_productXmlValidationService.IsValidXmlProduct(xmlProduct)) continue;

            xmlObjectData.Products.RemoveAt(i);

            i--;
        }

        OneOf<string, InvalidXmlResult> getXmlFromDataResult = await _productNewXmlService.TrySerializeProductsXmlAsync(xmlObjectData);

        return getXmlFromDataResult.Match<IStatusCodeActionResult>(
            xml => new OkObjectResult(xml),
            invalidXmlResult => BadRequest());
    }

    private async Task<List<Product>> GetProductsFromSearchOptionsAsync(ProductSearchRequest? productSearchRequest = null)
    {
        if (productSearchRequest is null)
        {
            return await _productService.GetAllAsync();
        }

        return await _productSearchService.SearchProductsAsync(productSearchRequest);
    }
}