using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Primitives;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.Utils.Files.ContentTypeUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorImageComponentEndpoints
{
    internal const string EndpointGroupUniqueRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "productEditor";

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
            RelatedPromotionFile = null
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
