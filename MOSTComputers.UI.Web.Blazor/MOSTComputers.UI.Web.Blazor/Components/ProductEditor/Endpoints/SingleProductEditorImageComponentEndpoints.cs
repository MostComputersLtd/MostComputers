using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorImageComponentEndpoints
{
    private sealed class ProductImageFromNewPromotionRequest
    {
        [JsonPropertyName("editorId")] public required string EditorId { get; init; }
        [JsonPropertyName("promotionFileId")] public int PromotionFileId { get; init; }
        [JsonPropertyName("index")] public int Index { get; init; }
    }

    internal const string EndpointGroupUniqueRoute = "productImage";

    private static readonly JsonSerializerOptions _jsonDataOptions = new()
    {
        Encoder = JavaScriptEncoder.Default,
    };

    private const string _filePreviewUrlFormFieldName = "previewUrl";
    private const string _fileIndexFormFieldName = "index";
    private const string _productImageFileWidthFormFieldName = "width";
    private const string _productImageFileHeightFormFieldName = "height";

    public static IEndpointConventionBuilder MapSingleProductEditorImageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupUniqueRoute);

        endpointGroup.MapPost("/add", (Delegate)GetProductImageFromExternalFileAsync); 

        return endpointGroup;
    }

    private static async Task<IResult> GetProductImageFromExternalFileAsync(HttpContext httpContext)
    {
        IFormCollection form = httpContext.Request.Form;

        StringValues previewUrlValue = form[_filePreviewUrlFormFieldName];

        if (previewUrlValue.Count == 0)
        {
            return Results.BadRequest("A preview url is required");
        }

        string previewUrl = previewUrlValue[0]!;

        StringValues indexValue = form[_fileIndexFormFieldName];

        if (indexValue.Count == 0
            || !int.TryParse(indexValue[0], out int index))
        {
            return Results.BadRequest("An index is required");
        }

        StringValues imageWidthValue = form[_productImageFileWidthFormFieldName];

        if (imageWidthValue.Count == 0
            || !int.TryParse(imageWidthValue[0], out int imageWidth))
        {
            return Results.BadRequest("An image width is required");
        }

        StringValues imageHeightValue = form[_productImageFileHeightFormFieldName];

        if (imageHeightValue.Count == 0
            || !int.TryParse(imageHeightValue[0], out int imageHeight))
        {
            return Results.BadRequest("An image height is required");
        }

        IFormFileCollection? selectedFiles = form.Files;

        if (selectedFiles == null || selectedFiles.Count == 0)
        {
            return Results.BadRequest("A file is required");
        }

        IFormFile? file = selectedFiles[0];

        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("A file is required");
        }

        if (!IsImageContentType(file.ContentType))
        {
            return Results.BadRequest("File is not an image file");
        }

        ProductImageFileDisplayData productImageFileDisplayData = new()
        {
            ExistingFileInfoId = null,
            ExistingImageId = null,
            FileName = null,
            DisplayOrder = index,
            Active = true
        };

        ProductImageDisplayData productImageDisplayData = new()
        {
            ExistingImageId = null,
            ContentType = file.ContentType,
            DateModified = null,
        };

        ProductImageListItemDisplayData productImageListItemDisplayData = new()
        {
            EditorId = $"from-browser-file-{Guid.NewGuid()}",
            Index = index,
            ImageFileUrl = previewUrl,
            UploadedImageFileData = new()
            {
                FileName = file.Name,
                ContentType = file.ContentType,
                FileSize = file.Length,
                ImageSize = new()
                {
                    WidthPixels = imageWidth,
                    HeightPixels = imageHeight,
                },
                PreviewUrl = previewUrl,
            },

            RelatedImage = productImageDisplayData,
            RelatedImageFile = productImageFileDisplayData,
            RelatedPromotionFileEditorId = null,
        };

        string parametersJsonData = JsonSerializer.Serialize(productImageListItemDisplayData, _jsonDataOptions);

        var output = new
        {
            ItemData = productImageListItemDisplayData,
            JsonData = parametersJsonData,
        };

        return new RazorComponentResult<ProductImageListItem>(output);
    }

    private static async Task<IResult> GetProductImageFromNewPromotionFileAsync(
        [FromBody] ProductImageFromNewPromotionRequest request,
        [FromServices] IPromotionFileService promotionFileService)
    {
        PromotionFileInfo? promotionFile = await promotionFileService.GetByIdAsync(request.PromotionFileId);

        if (promotionFile is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest);
        }

        string? contentType = GetContentTypeFromExtension(promotionFile.FileName);

        if (!IsImageContentType(contentType))
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest);
        }

        ProductImageDisplayData? productImageDisplayData = new()
        {
            ExistingImageId = null,
            ContentType = contentType,
            DateModified = DateTime.Now,
        };

        ProductImageFileDisplayData? productImageFileDisplayData = new()
        {
            ExistingFileInfoId = null,
            ExistingImageId = null,
            FileName = null,
            DisplayOrder = request.Index + 1,
            Active = true
        };

        ProductImageListItemDisplayData productImageListItemDisplayData = new()
        {
            EditorId = EditorIdsGenerateUtils.GenerateImageId(null, null, null),
            Index = request.Index,
            ImageFileUrl = $"{PromotionFileDataEndpoints.EndpointGroupRoute}/{promotionFile.FileName}",
            RelatedImage = productImageDisplayData,
            RelatedImageFile = productImageFileDisplayData,
            RelatedPromotionFileEditorId = request.EditorId,
        };

        string parametersJsonData = JsonSerializer.Serialize(productImageListItemDisplayData, _jsonDataOptions);

        var output = new
        {
            ItemData = productImageListItemDisplayData,
            JsonData = parametersJsonData,
        };

        return new RazorComponentResult<ProductImageListItem>(output);
    }
}
