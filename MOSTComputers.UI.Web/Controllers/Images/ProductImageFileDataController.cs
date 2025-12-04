using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Services.ProductImageFileManagement.Services.Contracts;
using OneOf;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Controllers.Images;

[ApiController]
[Route(ControllerRoute)]
public sealed class ProductImageFileDataController : ControllerBase
{
    public ProductImageFileDataController(IProductImageFileManagementService productImageFileManagementService)
    {
        _productImageFileManagementService = productImageFileManagementService;
    }

    internal const string ControllerRoute = ControllerCommonElements.ControllerRoutePrefix + "images/" + "imageFileData";

    private readonly IProductImageFileManagementService _productImageFileManagementService;

    [HttpGet("{fullFileName}")]
    public async Task<IActionResult> GetProductImageFileAsync(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName))
        {
            return new BadRequestObjectResult("The file name cannot be null or empty.");
        }

        OneOf<byte[], FileDoesntExistResult> getImageFileDataResult
            = await _productImageFileManagementService.GetImageAsync(fullFileName);

        return getImageFileDataResult.Match<IActionResult>(
            fileData =>
            {
                string? contentType = GetContentTypeFromExtension(fullFileName);

                if (string.IsNullOrWhiteSpace(contentType))
                {
                    contentType = "application/octet-stream";
                }

                return new FileContentResult(fileData, contentType!);
            },
            fileDoesntExistResult => new NotFoundObjectResult(fileDoesntExistResult.FileName));
    }
}