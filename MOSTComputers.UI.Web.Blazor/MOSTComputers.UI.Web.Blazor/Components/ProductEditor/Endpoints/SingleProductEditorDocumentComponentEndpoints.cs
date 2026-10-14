using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SingleProductEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorDocumentComponentEndpoints
{
    private sealed class ProductDocumentPopupRequest
    {
        public string? FileUrl { get; set; }
    }

    internal const string EndpointGroupUniqueRoute = "productDocument";

    private static readonly JsonSerializerOptions _jsonDataOptions = new()
    {
        Encoder = JavaScriptEncoder.Default,
    };

    private const string _filePreviewUrlFormFieldName = "previewUrl";
    private const string _fileIndexFormFieldName = "index";
    private const string _fileProductIdFormFieldName = "productId";

    private const int _productDocumentFileUploadSize = 20 * 1024 * 1024;

    private static readonly string[] _validIFrameUriSchemes = ["http", "https", "blob"];

    public static IEndpointConventionBuilder MapSingleProductEditorDocumentComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupUniqueRoute);

        endpointGroup.MapPost("/add", (Delegate)GetProductDocumentListItemAsync);
        endpointGroup.MapPost("/showPopup/{productId:int}",
            GetProductDocumentPopupDataAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetProductDocumentListItemAsync(HttpContext httpContext)
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

        StringValues productIdValue = form[_fileProductIdFormFieldName];

        if (productIdValue.Count == 0
            || !int.TryParse(productIdValue[0], out int productId))
        {
            return Results.BadRequest("An index is required");
        }

        IFormFileCollection? selectedFiles = form.Files;

        if (selectedFiles == null || selectedFiles.Count == 0)
        {
            return Results.BadRequest("A file is required");
        }

        IFormFile? file = selectedFiles[0];

        if (file is null
            || file.Length == 0
            || file.Length > _productDocumentFileUploadSize
            || file.ContentType != "application/pdf")
        {
            return Results.BadRequest("Missing or invalid file");
        }

        ProductDocumentDisplayData productDocumentDisplayData = new()
        {
            EditorId = GenerateDocumentIdForNewDocument(),
            Index = index,
            BrowserFilePath = previewUrl,
            ExistingDocument = null,
            CurrentDescription = string.Empty,
            FileExtension = ".pdf",
        };

        string parametersJsonData = JsonSerializer.Serialize(productDocumentDisplayData, _jsonDataOptions);

        return new RazorComponentResult<ProductDocumentListItem>(new
        {
            ItemData = productDocumentDisplayData,
            ProductId = productId,
            JsonData = parametersJsonData
        });
    }

    private static async Task<IResult> GetProductDocumentPopupDataAsync(
        [FromServices] IProductService productService,
        [FromBody] ProductDocumentPopupRequest productDocumentPopupRequest,
        [FromRoute(Name = "productId")] int productId)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.NotFound();
        }

        if (productDocumentPopupRequest == null
            || string.IsNullOrWhiteSpace(productDocumentPopupRequest.FileUrl)
            || !Uri.TryCreate(productDocumentPopupRequest.FileUrl, UriKind.RelativeOrAbsolute, out Uri? fileUri))
        {
            return Results.BadRequest("Missing or invalid document url");
        }

        if (fileUri.IsAbsoluteUri
            && !_validIFrameUriSchemes.Contains(fileUri.Scheme))
        {
            return Results.BadRequest("Missing or invalid document url");
        }

        return new RazorComponentResult<ProductDocumentPopup>(new
        {
            PopupData = new ProductDocumentPopup.ProductDocumentPopupData()
            {
                IsVislble = true,
                Product = product,
                DocumentPath = productDocumentPopupRequest.FileUrl,
            }
        });
    }
}
