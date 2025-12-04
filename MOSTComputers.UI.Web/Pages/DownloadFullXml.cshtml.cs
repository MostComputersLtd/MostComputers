using OneOf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.New.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.Services.Data.Xml.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.New.ProductData;

namespace MOSTComputers.UI.Web.Pages;

public class DownloadFullXmlModel : PageModel
{
    public DownloadFullXmlModel(
        IProductService productService,
        ILegacyProductXmlService legacyProductXmlService,
        ILegacyProductXmlFromProductDataService legacyProductXmlFromProductDataService,
        ILegacyProductXmlValidationService legacyProductXmlValidationService,
        IProductXmlService productXmlService,
        IProductXmlFromProductDataService productXmlFromProductDataService,
        IProductXmlValidationService productXmlValidationService)
    {
        _productService = productService;
        _legacyProductXmlService = legacyProductXmlService;
        _legacyProductXmlFromProductDataService = legacyProductXmlFromProductDataService;
        _legacyProductXmlValidationService = legacyProductXmlValidationService;
        _productXmlService = productXmlService;
        _newProductXmlFromProductDataService = productXmlFromProductDataService;
        _productXmlValidationService = productXmlValidationService;
    }

    private readonly IProductService _productService;
    private readonly ILegacyProductXmlService _legacyProductXmlService;
    private readonly ILegacyProductXmlFromProductDataService _legacyProductXmlFromProductDataService;
    private readonly ILegacyProductXmlValidationService _legacyProductXmlValidationService;
    private readonly IProductXmlService _productXmlService;
    private readonly IProductXmlFromProductDataService _newProductXmlFromProductDataService;
    private readonly IProductXmlValidationService _productXmlValidationService;

    public async Task<IStatusCodeActionResult> OnGetGetLegacyXmlAsync()
    {
        List<Product> allProducts = await _productService.GetAllAsync();

        IEnumerable<Product> products = allProducts
            .Where(x => x.Status != ProductStatus.Unavailable);

        string hostPath = $"{Request.Scheme}://{Request.Host}";

        LegacyXmlObjectData xmlObjectData = await _legacyProductXmlFromProductDataService.GetLegacyXmlObjectDataForProductsAsync(products, hostPath);

        for (int i = 0; i < xmlObjectData.Products.Count; i++)
        {
            LegacyXmlProduct xmlProduct = xmlObjectData.Products[i];

            if (_legacyProductXmlValidationService.IsValidXmlProduct(xmlProduct)) continue;

            xmlObjectData.Products.RemoveAt(i);

            i--;
        }

        OneOf<string, InvalidXmlResult> getProductXmlResult = _legacyProductXmlService.TrySerializeProductsXml(xmlObjectData, false, true);

        return getProductXmlResult.Match<IStatusCodeActionResult>(
            xml => new OkObjectResult(xml),
            invalidXmlResult => BadRequest(invalidXmlResult));
    }

    public async Task<IStatusCodeActionResult> OnGetGetNewXmlAsync()
    {
        List<Product> allProducts = await _productService.GetAllAsync();

        IEnumerable<Product> products = allProducts
            .Where(x => x.Status != ProductStatus.Unavailable);

        string hostPath = $"{Request.Scheme}://{Request.Host}";

        ProductsXmlFullData xmlObjectData = await _newProductXmlFromProductDataService.GetXmlObjectDataForProductsAsync(products, hostPath);

        for (int i = 0; i < xmlObjectData.Products.Count; i++)
        {
            XmlProduct xmlProduct = xmlObjectData.Products[i];

            if (_productXmlValidationService.IsValidXmlProduct(xmlProduct)) continue;

            xmlObjectData.Products.RemoveAt(i);

            i--;
        }

        OneOf<string, InvalidXmlResult> getProductXmlResult = await _productXmlService.TrySerializeProductsXmlAsync(xmlObjectData);

        return getProductXmlResult.Match<IStatusCodeActionResult>(
            xml => new OkObjectResult(xml),
            invalidXmlResult => BadRequest(invalidXmlResult));
    }
}