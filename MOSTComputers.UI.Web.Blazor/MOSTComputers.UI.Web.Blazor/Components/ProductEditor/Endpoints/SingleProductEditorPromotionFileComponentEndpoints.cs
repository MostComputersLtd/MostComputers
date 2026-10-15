using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Primitives;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Components.PromotionFileEditor;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorPromotionFileComponentEndpoints
{
    private sealed class SearchPromotionFilesRequest
    {
        [JsonPropertyName("userInputString")] public string? UserInputString { get; init; }
        [JsonPropertyName("validOnDate")] public DateOnly? ValidOnDate { get; init; }
    }

    internal const string EndpointGroupUniqueRoute = "promotionFile";

    public static IEndpointConventionBuilder MapSingleProductEditorPromotionFileComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupUniqueRoute);

        endpointGroup.MapGet("/single/showPopup", ShowSinglePromotionProductFileEditorPopupAsync);

        endpointGroup.MapPost("/single/search", SearchPromotionFilesAsync);

        return endpointGroup;
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
