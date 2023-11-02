using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.XMLDataOperations.Models;
using MOSTComputers.Services.XMLDataOperations.Services;
using MOSTComputers.UI.Web.Services;
using OneOf;

namespace MOSTComputers.UI.Web.Pages.CreatePages;

public class XmlProductCreateModel : PageModel
{
    public XmlProductCreateModel(ProductXmlToCreateRequestMapperService mapperService)
    {
        _mapperService = mapperService;
    }

    private readonly ProductXmlToCreateRequestMapperService _mapperService;

    public void OnGet()
    {
    }

    public async Task OnPostAsync(string? XMLInput)
    {
        if (string.IsNullOrEmpty(XMLInput)) return;

        OneOf<List<ProductCreateRequest>, ValidationResult, UnexpectedFailureResult> requests
            = await _mapperService.GetProductCreateRequestsFromXmlTextAsync(XMLInput);
    }
}