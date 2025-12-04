using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Controllers.Images;

[ApiController]
[Route(ControllerRoute)]
public sealed class ProductImageDataController : ControllerBase
{
    public ProductImageDataController(IProductImageService productImageService)
    {
        _productImageService = productImageService;
    }

    internal const string ControllerRoute = ControllerCommonElements.ControllerRoutePrefix + "images/" + "originalImageData";

    private readonly IProductImageService _productImageService;

    [HttpGet("{imageId}")]
    public async Task<IActionResult> GetProductImageAsync(int imageId)
    {
        ProductImage? productImage = await _productImageService.GetByIdInAllImagesAsync(imageId);

        if (productImage is null)
        {
            return new NotFoundObjectResult(imageId);
        }

        string? contentType = productImage.ImageContentType;

        bool isKnownContentType = IsKnownContentType(contentType);

        if (!isKnownContentType)
        {
            contentType = "application/octet-stream";
        }

        byte[] imageData = productImage.ImageData ?? Array.Empty<byte>();

        return new FileContentResult(imageData, contentType!);
    }
}