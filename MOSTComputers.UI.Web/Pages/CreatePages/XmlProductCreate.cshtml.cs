using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services.Contracts;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.UI.Web.Validation;
using OneOf;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.Pages.CreatePages;

public class XmlProductCreateModel : PageModel
{
    public XmlProductCreateModel(
        IProductXmlToCreateRequestMappingService createRequestMapperService,
        IProductXmlToProductDisplayMappingService displayMappingService,
        IProductService productService,
        IProductDeserializeService productDeserializeService)
    {
        _createRequestMapperService = createRequestMapperService;
        _displayMappingService = displayMappingService;
        _productService = productService;
        _productDeserializeService = productDeserializeService;
    }

    private readonly IProductXmlToCreateRequestMappingService _createRequestMapperService;
    private readonly IProductXmlToProductDisplayMappingService _displayMappingService;
    private readonly IProductService _productService;
    private readonly IProductDeserializeService _productDeserializeService;

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
        if (DisplayCreateRequests is not null
            && DisplayCreateRequests.Count > 0)
        {
            return CreateProducts(DisplayCreateRequests);
        }

        OneOf<List<ProductCreateRequest>, ValidationResult, UnexpectedFailureResult, InvalidXmlResult> requestMappingResult
            = await _createRequestMapperService.GetProductCreateRequestsFromXmlAsync(xml);

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

        OneOf<List<XmlProductCreateDisplay>, ValidationResult> requestMappingResult
            = await _displayMappingService.GetProductXmlDisplayFromXmlAsync(xmlObjectData, xml);

        return requestMappingResult.Match(
            productCreateRequests =>
            {
                DisplayCreateRequests = new();

                for (int i = 0; i < productCreateRequests.Count; i++)
                {
                    XmlProductCreateDisplay item = productCreateRequests[i];
                    XmlProduct xmlItem = xmlObjectData.Products[i];

                    DisplayCreateRequests.Add(item);
                }

                return new OkResult();
            },
            validationResult => this.GetResultFromValidationResult(validationResult));
    }

    private IActionResult CreateProducts(List<XmlProductCreateDisplay> productDisplays)
    {
        foreach (var productDisplay in productDisplays)
        {
            ProductCreateRequest productCreateRequest = _displayMappingService.GetProductCreateRequestFromProductXmlDisplay(productDisplay);

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

    public IActionResult OnGetAlterFailedRequest(int requestIndex, string propertyName, int newCharacteristicId)
    {
        string newChatacteristicIdString = newCharacteristicId.ToString();

        XmlProductCreateDisplay displayAtIndex = DisplayCreateRequests[requestIndex];

        if (displayAtIndex.Properties is null || displayAtIndex.Characteristics is null) return BadRequest("Invalid characteristic index");

        DisplayPropertyCreateRequest? displayPropToUpdate = displayAtIndex.Properties.FirstOrDefault(x => x.Name == propertyName);

        if (displayPropToUpdate == null) return BadRequest("Invalid characteristic index");

        SelectListItem? newCharacteristicForProperty = null;
        int? newCharacteristicForPropertyIndex = null;

        List<SelectListItem> characteristicDataList = displayAtIndex.Characteristics.ToList();

        for (int i = 0; i < characteristicDataList.Count; i++)
        {
            SelectListItem characteristicData = characteristicDataList[i];

            if (characteristicData.Value == newChatacteristicIdString)
            {
                newCharacteristicForProperty = characteristicData;
                newCharacteristicForPropertyIndex = i;
            }
        }

        if (displayPropToUpdate == null) return BadRequest("Invalid characteristic Index");
        if (newCharacteristicForProperty == null) return BadRequest("Invalid characteristic ID");

        displayPropToUpdate.Name = newCharacteristicForProperty.Text;
        displayPropToUpdate.ProductCharacteristicId = int.Parse(newCharacteristicForProperty.Value);

        return Partial("_ProductCreateRequestListPartial", DisplayCreateRequests);
    }
}