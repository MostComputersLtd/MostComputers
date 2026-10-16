using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Primitives;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Components.PromotionFileEditor;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorPromotionFileComponentEndpoints
{
    private sealed class PromotionProductFilePopupRequest
    {
        [JsonPropertyName("productId")] public required int ProductId { get; set; }
        [JsonPropertyName("promotionProductFiles")]
        public List<SingleProductEditorPopup.SingleProductEditorPopup.PromotionProductFileInfoDisplayData>? PromotionProductFiles { get; init; }
    }

    private sealed class PromotionProductFileEditorPopupItemRequest
    {
        [JsonPropertyName("existingEditorId")] public string? ExistingEditorId { get; set; }
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("promotionFileId")] public int? PromotionFileId { get; set; }
        [JsonPropertyName("validFrom")] public DateTime? ValidFrom { get; set; }
        [JsonPropertyName("validTo")] public DateTime? ValidTo { get; set; }
        [JsonPropertyName("active")] public bool Active { get; set; } = false;
        [JsonPropertyName("shouldDisplayDeleteButton")] public bool ShouldDisplayDeleteButton { get; set; } = false;
    } 

    private sealed class SinglePromotionProductFileEditorPopupRequest
    {
        [JsonPropertyName("existingEditorId")] public string? ExistingEditorId { get; set; }
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("promotionFileId")] public int? PromotionFileId { get; set; }
        [JsonPropertyName("validFrom")] public DateTime? ValidFrom { get; set; }
        [JsonPropertyName("validTo")] public DateTime? ValidTo { get; set; }
        [JsonPropertyName("active")] public bool Active { get; set; } = false;
        [JsonPropertyName("saveButtonText")] public string SaveButtonText { get; set; } = "Save";
        [JsonPropertyName("isInImage")] public bool IsInImage { get; set; } = false;
    } 

    private sealed class SearchPromotionFilesRequest
    {
        [JsonPropertyName("userInputString")] public string? UserInputString { get; init; }
        [JsonPropertyName("validOnDate")] public DateOnly? ValidOnDate { get; init; }
    }

    internal const string EndpointGroupUniqueRoute = "promotionFile";

    public static IEndpointConventionBuilder MapSingleProductEditorPromotionFileComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupUniqueRoute);

        endpointGroup.MapPost("/showPopup", ShowPromotionProductFilePopupAsync);
        endpointGroup.MapPost("/item", GetPromotionProductFileEditorItemAsync);

        endpointGroup.MapPost("/single/showPopup", ShowSinglePromotionProductFileEditorPopupAsync);
        endpointGroup.MapPost("/single/search", SearchPromotionFilesAsync);

        return endpointGroup;
    }

    private static async Task<IResult> ShowPromotionProductFilePopupAsync(
        [FromRoute] int productId,
        [FromBody] PromotionProductFilePopupRequest request,
        [FromServices] IProductService productService,
        [FromServices] IPromotionProductFileService promotionProductFileService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId);

        if (product == null)
        {
            return Results.Problem(statusCode: StatusCodes.Status404NotFound);
        }

        List<PromotionProductFileInfo> promotionProductFiles
            = await promotionProductFileService.GetAllForProductAsync(request.ProductId);

        List<PromotionFileEditorPopup.PromotionProductFileInfoDisplayData> promotionFileData = new();

        if  (request.PromotionProductFiles != null)
        {
            foreach (SingleProductEditorPopup.SingleProductEditorPopup.PromotionProductFileInfoDisplayData promotionFileEditorData
                in request.PromotionProductFiles)
            {
                PromotionProductFileInfo? promotionProductFile = null;

                if (promotionFileEditorData.Id != null)
                {
                    promotionProductFile = promotionProductFiles.Find(x => x.Id == promotionFileEditorData.Id);
                }

                PromotionFileEditorPopup.PromotionProductFileInfoDisplayData editorData = new()
                {
                    EditorId = promotionFileEditorData.EditorId,
                    Id = promotionFileEditorData.Id,
                    ValidFrom = promotionFileEditorData.ValidFrom,
                    ValidTo = promotionFileEditorData.ValidTo,
                    Active = promotionFileEditorData.Active,
                    CreateUserName = promotionProductFile?.CreateUserName,
                    CreateDate = promotionProductFile?.CreateDate,
                    LastUpdateUserName = promotionProductFile?.LastUpdateUserName,
                    LastUpdateDate = promotionProductFile?.LastUpdateDate,
                    ProductImageId = promotionProductFile?.ProductImageId,
                    PromotionFileInfo = promotionProductFile?.PromotionFileInfo,
                };

                promotionFileData.Add(editorData);
            }
        } 

        return new RazorComponentResult<PromotionFileEditorPopup>(new
        {
            IsVisible = true,
            Product = product,
            PromotionProductFiles = promotionFileData,
            PopupMarginOptions = new Popup.MarginOptions() { Left = "10vw", Top = "10vh", Right = "10vw", Bottom = "10vh" },
            ShouldDisplaySaveButtonInSingleEditor = true,
            ShouldDisplayAddButton = true,
            ShouldDisplayDeleteButton = true,
        });
    }

    private static async Task<IResult> GetPromotionProductFileEditorItemAsync(
        [FromBody] PromotionProductFileEditorPopupItemRequest request,
        [FromServices] IPromotionProductFileService promotionProductFileService)
    {
        PromotionProductFileInfo? promotionProductFile = null;

        if (request.Id != null)
        {
            promotionProductFile = await promotionProductFileService.GetByIdAsync(request.Id.Value);
        }

        string editorId = request.ExistingEditorId ?? GeneratePromotionFileId(null);

        PromotionFileEditorPopup.PromotionProductFileInfoDisplayData itemData = new()
        {
            EditorId = editorId,
            Id = request.Id,
            Active = request.Active,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            CreateUserName = promotionProductFile?.CreateUserName,
            CreateDate = promotionProductFile?.CreateDate,
            LastUpdateUserName = promotionProductFile?.LastUpdateUserName,
            LastUpdateDate = promotionProductFile?.LastUpdateDate,
            ProductImageId = promotionProductFile?.ProductImageId,
            PromotionFileInfo = promotionProductFile?.PromotionFileInfo,
        };

        return new RazorComponentResult<PromotionFileEditorPopupItem>(new
        {
            ItemData = itemData,
            ShouldDisplayDeleteButton = request.ShouldDisplayDeleteButton,
        });
    }

    private static async Task<IResult> ShowSinglePromotionProductFileEditorPopupAsync(
        [FromBody] SinglePromotionProductFileEditorPopupRequest request,
        [FromServices] IPromotionProductFileService promotionProductFileService)
    {
        PromotionProductFileInfo? promotionProductFile = null;

        if (request.Id != null)
        {
            promotionProductFile = await promotionProductFileService.GetByIdAsync(request.Id.Value);
        }

        string editorId = request.ExistingEditorId ?? GeneratePromotionFileId(null);

        SinglePromotionFilePopup.PromotionProductFileInfoEditData? popupData = new()
        {
            EditorId = editorId,
            Id = request.Id,
            PromotionFileId = request.PromotionFileId,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            Active = request.Active,
            CreateUserName = promotionProductFile?.CreateUserName,
            CreateDate = promotionProductFile?.CreateDate,
            LastUpdateUserName = promotionProductFile?.LastUpdateUserName,
            LastUpdateDate = promotionProductFile?.LastUpdateDate,
            IsInImage = request.IsInImage,
        };

        return new RazorComponentResult<SinglePromotionFilePopup>(new
        {
            IsVisible = true,
            PopupData = popupData,
            PopupMarginOptions = new Popup.MarginOptions() { Left = "15vw", Right = "15vw", Top = "15vh", Bottom = "15vh" },
            ShouldDisplaySaveButton = true,
            SaveButtonText = request.SaveButtonText,
        });
    }

    private static async Task<IResult> SearchPromotionFilesAsync(
        [FromBody] SearchPromotionFilesRequest request,
        [FromServices] IPromotionFileService promotionFileService)
    {
        string? userInputString = request.UserInputString;
        DateOnly? validOnDate = request.ValidOnDate;

        List<PromotionFileInfo> promotionFileInfos = await PromotionFileEditorPageComponentEndpoints.SearchPromotionFilesInternalAsync(
            userInputString, validOnDate, promotionFileService);

        List<PromotionFileSelectList.PromotionFileInfoListItem> promotionFileListItems = new();

        foreach (PromotionFileInfo promotionFileInfo in promotionFileInfos)
        {
            PromotionFileSelectList.PromotionFileInfoListItem listItem = new()
            {
                PromotionFile = promotionFileInfo,
                IsSelected = false,
            };

            promotionFileListItems.Add(listItem);
        }


        return new RazorComponentResult<PromotionFileSelectList>(new
        {
            PromotionFiles = promotionFileListItems,
        });
    }
}
