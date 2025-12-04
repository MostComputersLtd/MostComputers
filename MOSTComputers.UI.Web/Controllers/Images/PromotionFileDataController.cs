using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Services.PromotionFileManagement.Services.Contracts;

using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Controllers.Images;

[ApiController]
[Route(ControllerRoute)]
public sealed class PromotionFileDataController : ControllerBase
{
    public PromotionFileDataController(IPromotionFileManagementService promotionFileManagementService)
    {
        _promotionFileManagementService = promotionFileManagementService;
    }

    internal const string ControllerRoute = ControllerCommonElements.ControllerRoutePrefix + "images/" + "promotionFileData";

    private readonly IPromotionFileManagementService _promotionFileManagementService;

    [HttpGet("{fullFileName}")]
    public async Task<IActionResult> GetPromotionFileAsync(string fullFileName)
    {
        if (string.IsNullOrWhiteSpace(fullFileName))
        {
            return new BadRequestObjectResult("The file name cannot be null or empty.");
        }

        byte[]? fileData = await _promotionFileManagementService.GetFileAsync(fullFileName);

        if (fileData is null)
        {
            return new NotFoundObjectResult($"The file was not found: {fullFileName}.");
        }

        string? contentType = GetContentTypeFromExtension(fullFileName);

        if (string.IsNullOrWhiteSpace(contentType))
        {
            contentType = "application/octet-stream";
        }

        return new FileContentResult(fileData, contentType!);
    }
}