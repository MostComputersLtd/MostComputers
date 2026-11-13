using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Primitives;
using MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup;
using MOSTComputers.UI.Web.Blazor.Components.PromotionFileEditor;
using OneOf;
using static MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.EditorIdsGenerateUtils;

using ReadonlyEditorDataAlias
= MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.PromotionFileEditorPopup.PromotionProductFileInfoReadonlyDisplayData;

using EditorDataAlias
= MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.PromotionFileEditorPopup.PromotionProductFileInfoDisplayData;

using SingleEditorReadonlyDataAlias
= MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SinglePromotionFilePopup.PromotionProductFileInfoReadonlyData;

using SingleEditorDataAlias
= MOSTComputers.UI.Web.Blazor.Components.ProductEditor.SingleProductEditorPopup.SinglePromotionFilePopup.PromotionProductFileInfoEditData;
using MOSTComputers.UI.Web.Blazor.Components.Product.Promotion.Files;

namespace MOSTComputers.UI.Web.Blazor.Components.ProductEditor.Endpoints;

internal static class SingleProductEditorPromotionFileComponentEndpoints
{
    private sealed class PromotionProductFilePopupRequest
    {
        [JsonPropertyName("productId")] public required int ProductId { get; set; }
        [JsonPropertyName("promotionProductFiles")]
        public List<PromotionFileEditorPopup.PromotionProductFileEditorJsonData>? PromotionProductFiles { get; init; }
    }

    private sealed class PromotionProductFileEditorPopupItemRequest
    {
        [JsonPropertyName("existingEditorId")] public string? ExistingEditorId { get; set; }
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("promotionFileId")] public int? PromotionFileId { get; set; }
        [JsonPropertyName("validFrom")] public DateTime? ValidFrom { get; set; }
        [JsonPropertyName("validTo")] public DateTime? ValidTo { get; set; }
        [JsonPropertyName("active")] public bool Active { get; set; } = false;
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
        [FromBody] PromotionProductFilePopupRequest request,
        [FromServices] IProductService productService,
        [FromServices] IPromotionService promotionService,
        [FromServices] IPromotionProductFileService promotionProductFileService,
        [FromServices] IPromotionFileService promotionFileService)
    {
        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(request.ProductId);

        if (product == null)
        {
            return Results.Problem(statusCode: StatusCodes.Status404NotFound);
        }

        List<PromotionProductFileInfo> promotionProductFiles
            = await promotionProductFileService.GetAllForProductAsync(request.ProductId);

        List<PromotionFileInfo> promotionFiles = [];

        List<int> additionalPromotionFileIds = [];

        if (request.PromotionProductFiles != null)
        {
            foreach (PromotionFileEditorPopup.PromotionProductFileEditorJsonData promotionFileEditorData in request.PromotionProductFiles)
            {
                if (promotionFileEditorData.PromotionFileId == null) continue;

                PromotionProductFileInfo? promotionProductFile
                    = promotionProductFiles.Find(x => x.PromotionFileInfoId == promotionFileEditorData.PromotionFileId);

                if (promotionProductFile != null)
                {
                    promotionFiles.Add(promotionProductFile.PromotionFileInfo);

                    continue;
                }

                additionalPromotionFileIds.Add(promotionFileEditorData.PromotionFileId.Value);
            }
        }

        List<PromotionFileInfo> promotionFilesFromService = await promotionFileService.GetByIdsAsync(additionalPromotionFileIds);

        promotionFiles.AddRange(promotionFilesFromService);

        List<EditorDataAlias> promotionFileData = new();
        List<PromotionFileEditorPopup.PromotionProductFileEditorJsonData> promotionFileJsonData = new();

        if (request.PromotionProductFiles != null)
        {
            foreach (PromotionFileEditorPopup.PromotionProductFileEditorJsonData promotionFileEditorData in request.PromotionProductFiles)
            {
                PromotionProductFileInfo? promotionProductFile = null;
                PromotionFileInfo? promotionFile = promotionFiles.Find(x => x.Id == promotionFileEditorData.PromotionFileId);

                EditorDataAlias editorData = new()
                {
                    EditorId = promotionFileEditorData.EditorId,
                    Id = promotionFileEditorData.Id,
                    ValidFrom = promotionFileEditorData.ValidFrom,
                    ValidTo = promotionFileEditorData.ValidTo,
                    Active = promotionFileEditorData.Active,
                    ProductImageId = promotionProductFile?.ProductImageId,
                    PromotionFileInfo = promotionFile,
                };

                PromotionFileEditorPopup.PromotionProductFileEditorJsonData jsonData = new()
                {
                    EditorId = promotionFileEditorData.EditorId,
                    Id = promotionFileEditorData.Id,
                    ValidFrom = promotionFileEditorData.ValidFrom,
                    ValidTo = promotionFileEditorData.ValidTo,
                    Active = promotionFileEditorData.Active,
                    PromotionFileId = promotionFile?.Id,
                };

                promotionFileData.Add(editorData);
                promotionFileJsonData.Add(jsonData);
            }
        }

        List<Promotion> promotions = await promotionService.GetAllForProductAsync(product.Id);

        Promotion? pidPromotion = null;
        Promotion? ridPromotion = null;

        DateTime now = DateTime.Now;

        foreach (Promotion promotion in promotions)
        {
            if (promotion.StartDate > now || promotion.ExpirationDate < now)
            {
                continue;
            }

            if (promotion.Id == product.PromotionPid)
            {
                pidPromotion = promotion;
            }
            else if (promotion.Id == product.PromotionRid)
            {
                ridPromotion = promotion;
            }
        }

        OneOf<List<ReadonlyEditorDataAlias>, List<EditorDataAlias>> promotionFileDataWrapped
            = OneOf<List<ReadonlyEditorDataAlias>, List<EditorDataAlias>>.FromT1(promotionFileData);

        return new RazorComponentResult<PromotionFileEditorPopup>(new
        {
            IsVisible = true,
            Product = product,
            PromotionProductFiles = promotionFileDataWrapped,
            PidPromotion = pidPromotion,
            RidPromotion = ridPromotion,
            PopupMarginOptions = new Popup.MarginOptions() { Left = "10vw", Top = "10vh", Right = "10vw", Bottom = "10vh" },
        });
    }

    private static async Task<IResult> GetPromotionProductFileEditorItemAsync(
        [FromBody] PromotionProductFileEditorPopupItemRequest request,
        [FromServices] IPromotionProductFileService promotionProductFileService,
        [FromServices] IPromotionFileService promotionFileService)
    {
        PromotionProductFileInfo? promotionProductFile = null;
        PromotionFileInfo? promotionFile = null;

        if (request.Id != null)
        {
            promotionProductFile = await promotionProductFileService.GetByIdAsync(request.Id.Value);

            if (promotionProductFile != null
                && request.PromotionFileId == promotionProductFile.PromotionFileInfoId)
            {
                promotionFile = promotionProductFile.PromotionFileInfo;
            }
        }

        if (promotionFile == null
            && request.PromotionFileId != null)
        {
            promotionFile = await promotionFileService.GetByIdAsync(request.PromotionFileId.Value);
        }

        string editorId = request.ExistingEditorId ?? GeneratePromotionFileId(null);

        EditorDataAlias itemData = new()
        {
            EditorId = editorId,
            Id = request.Id,
            Active = request.Active,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            ProductImageId = promotionProductFile?.ProductImageId,
            PromotionFileInfo = promotionFile,
        };

        OneOf<ReadonlyEditorDataAlias, EditorDataAlias> itemDataWrapped
            = OneOf<ReadonlyEditorDataAlias, EditorDataAlias>.FromT1(itemData);

        return new RazorComponentResult<PromotionFileEditorPopupItem>(new
        {
            ItemData = itemDataWrapped,
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

        SingleEditorDataAlias popupData = new()
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
            SaveButtonText = request.SaveButtonText,
        };

        OneOf<SingleEditorReadonlyDataAlias, SingleEditorDataAlias> popupDataWrapped
            = OneOf<SingleEditorReadonlyDataAlias, SingleEditorDataAlias>.FromT1(popupData);

        return new RazorComponentResult<SinglePromotionFilePopup>(new
        {
            IsVisible = true,
            PopupData = popupDataWrapped,
            PopupMarginOptions = new Popup.MarginOptions() { Left = "15vw", Right = "15vw", Top = "15vh", Bottom = "15vh" },
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
