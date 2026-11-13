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
    private sealed class ProductImageFromPromotionRequest
    {
        [JsonPropertyName("existingEditorId")] public string? ExistingEditorId { get; init; } = null;
        [JsonPropertyName("relatedPromotionFileEditorId")] public required string RelatedPromotionFileEditorId { get; init; }
        [JsonPropertyName("promotionFileId")] public required int PromotionFileId { get; init; }
        [JsonPropertyName("index")] public required int Index { get; init; }
        [JsonPropertyName("active")] public required bool Active { get; init; }
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
        endpointGroup.MapPost("/promotionFile", GetProductImageFromPromotionFileAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetProductImageFromExternalFileAsync(HttpContext httpContext)
    {
        IFormCollection form = httpContext.Request.Form;

        StringValues previewUrlValue = form[_filePreviewUrlFormFieldName];

        if (previewUrlValue.Count == 0)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "A preview url is required");
        }

        string previewUrl = previewUrlValue[0]!;

        StringValues indexValue = form[_fileIndexFormFieldName];

        if (indexValue.Count == 0
            || !int.TryParse(indexValue[0], out int index))
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "An index is required");
        }

        StringValues imageWidthValue = form[_productImageFileWidthFormFieldName];

        if (imageWidthValue.Count == 0
            || !int.TryParse(imageWidthValue[0], out int imageWidth))
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "An image width is required");
        }

        StringValues imageHeightValue = form[_productImageFileHeightFormFieldName];

        if (imageHeightValue.Count == 0
            || !int.TryParse(imageHeightValue[0], out int imageHeight))
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "An image height is required");
        }

        IFormFileCollection? selectedFiles = form.Files;

        if (selectedFiles == null || selectedFiles.Count == 0)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "A file is required");
        }

        IFormFile file = selectedFiles[0];

        if (file.Length == 0)
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "A file is required");
        }

        if (!IsImageContentType(file.ContentType))
        {
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "File is not an image file");
        }

        string editorId = $"from-browser-file-{Guid.NewGuid()}";

        ProductImageUploadedFileListItem.UploadedProductImageFileDisplayData uploadedFileData = new()
        {
            EditorId = editorId,
            Index = index,
            UploadedFile = new()
            {
                FileName = file.Name,
                ContentType = file.ContentType,
                PreviewUrl = previewUrl,
                FileSize = file.Length,
                WidthPixels = imageWidth,
                HeightPixels = imageHeight,
            },
            ImageData = new()
            {
                Active = true
            },
        };

        ProductImageUploadedFileListItem.UploadedProductImageFileRecoveryJsonData uploadedFileJsonRecoveryData = new()
        {
            EditorId = editorId,
            UploadedFileUrl = previewUrl,
            ImageFileData = new()
            {
                Active = true,
            }
        };

        string parametersJsonData = JsonSerializer.Serialize(uploadedFileJsonRecoveryData, _jsonDataOptions);

        var output = new
        {
            ItemData = uploadedFileData,
            JsonData = parametersJsonData,
        };

        return new RazorComponentResult<ProductImageUploadedFileListItem>(output);
    }

    private static async Task<IResult> GetProductImageFromPromotionFileAsync(
        [FromBody] ProductImageFromPromotionRequest request,
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

        string editorId = request.ExistingEditorId ?? EditorIdsGenerateUtils.GenerateImageId(null, null, null);

        PromotionFileImageListItem.PromotionFileImageItemDisplayData promotionFileImageData = new()
        {
            EditorId = editorId,
            Index = request.Index,
            PromotionFileData = new()
            {
                Name = promotionFile.Name,
                FileName = promotionFile.FileName,
            },
            ImageData = new()
            {
                Active = request.Active,
            },
            RelatedPromotionFileEditorId = request.RelatedPromotionFileEditorId,
        };

        PromotionFileImageListItem.PromotionFileImageItemRecoveryJsonData promotionFileImageRecoveryJsonData = new()
        {
            EditorId = editorId,
            ImageFileData = new()
            {
                Active = request.Active,
            },
            RelatedPromotionFileEditorId = request.RelatedPromotionFileEditorId,
        };

        string parametersJsonData = JsonSerializer.Serialize(promotionFileImageRecoveryJsonData, _jsonDataOptions);

        var output = new
        {
            ItemData = promotionFileImageData,
            JsonData = parametersJsonData,
        };

        return new RazorComponentResult<PromotionFileImageListItem>(output);
    }
}
