using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Services;
using MOSTComputers.UI.Web.Validation;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.Pages.CreatePages;

public class XmlProductCreateModel : PageModel
{
    public XmlProductCreateModel(ProductXmlToCreateRequestMapperService mapperService, IProductService productService, ProductDeserializeService productDeserializeService)
    {
        _mapperService = mapperService;
        _productService = productService;
        _productDeserializeService = productDeserializeService;
    }

    private readonly ProductXmlToCreateRequestMapperService _mapperService;
    private readonly IProductService _productService;
    private readonly ProductDeserializeService _productDeserializeService;

    public static List<ProductCreateRequest> CreateRequests { get; set; } = new();
    public static List<XmlProductCreateDisplay> DisplayCreateRequests { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetShowOnlyAsync(string? xmlInput)
    {
        if (string.IsNullOrEmpty(xmlInput)) return BadRequest("The input is empty");

        IActionResult actionResult = await MapXmlToRequestsAndUpdateUIAsync(xmlInput);

        return actionResult;
    }

    public async Task<IActionResult> OnPostAsync(string? XMLInput)
    {
        if (string.IsNullOrEmpty(XMLInput)) return BadRequest("The input is empty");

        IActionResult actionResult = await MapXmlToRequestsAndCreateProductsAsync(XMLInput);

        return actionResult;
    }

    private async Task<IActionResult> MapXmlToRequestsAndCreateProductsAsync(string xml)
    {
        if (CreateRequests is not null
            && CreateRequests.Count > 0)
        {
            return CreateProducts(CreateRequests);
        }

        OneOf<List<ProductCreateRequest>, ValidationResult, UnexpectedFailureResult, InvalidXmlResult> requestMappingResult
            = await _mapperService.GetProductCreateRequestsFromXmlAsync(xml);

        return requestMappingResult.Match(
            productCreateRequests => CreateProducts(productCreateRequests),
            validationResult => this.GetResultFromValidationResult(validationResult),
            unexpectedFailureResult => StatusCode(500),
            invalidXmlResult => BadRequest(invalidXmlResult.Text ?? "The input is not valid xml"));
    }

    private async Task<IActionResult> MapXmlToRequestsAndUpdateUIAsync(string xml)
    {
        OneOf<XmlObjectData?, InvalidXmlResult> result = _productDeserializeService.TryDeserializeProductsXml(xml);

        if (result.IsT1)
        {
            return BadRequest(result.AsT1.Text ?? "The input is not valid xml");
        }

        XmlObjectData? xmlObjectData = result.AsT0;

        if (xmlObjectData is null) return StatusCode(500);

        OneOf<List<ProductCreateRequest>, ValidationResult> requestMappingResult
            = await _mapperService.GetProductCreateRequestsFromXmlAsync(xmlObjectData, xml);

        return requestMappingResult.Match(
            productCreateRequests =>
            {
                DisplayCreateRequests = new();
                CreateRequests = new();

                for (int i = 0; i < productCreateRequests.Count; i++)
                {
                    ProductCreateRequest item = productCreateRequests[i];
                    XmlProduct xmlItem = xmlObjectData.Products[i];

                    CreateRequests.Add(item);

                    DisplayCreateRequests.Add(_mapperService.GetProductXmlDisplayFromProductData(xmlItem, item));
                }

                return new OkResult();
            },
            validationResult => this.GetResultFromValidationResult(validationResult));
    }

    private IActionResult CreateProducts(List<ProductCreateRequest> productCreateRequests)
    {
        foreach (var productCreateRequest in productCreateRequests)
        {
            OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _productService.Insert(productCreateRequest);

            bool isSuccessResult = false;

            IActionResult insertActionResult = insertResult.Match(
                id =>
                {
                    isSuccessResult = true;

                    return Page();
                },
                validationResult => this.GetResultFromValidationResult(validationResult),
                unexpectedFailureResult => StatusCode(500));

            if (!isSuccessResult)
            {
                return insertActionResult;
            }
        }

        return Page();
    }

    public IActionResult OnGetPartialView()
    {
        return Partial("_ProductCreateRequestListPartial", DisplayCreateRequests);
    }
}