using System.Text.Json.Serialization;
using FluentValidation.Results;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.UI.Web.Blazor.Components.Primitives;
using MOSTComputers.UI.Web.Blazor.Components.Product.Promotion.Files;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.Utils.OneOf;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Blazor.Components.PromotionFileEditor.SinglePromotionFileEditorPopup;
using static MOSTComputers.UI.Web.Blazor.Utils.AuthenticationUtils;
using static MOSTComputers.UI.Web.Blazor.Endpoints.EndpointErrorHandlingCommonElements;

namespace MOSTComputers.UI.Web.Blazor.Components.PromotionFileEditor;

internal static class PromotionFileEditorPageComponentEndpoints
{
    private sealed class SearchPromotionFilesRequest
    {
        [JsonPropertyName("userInputString")] public string? UserInputString { get; init; }
        [JsonPropertyName("validOnDate")] public DateOnly? ValidOnDate { get; init; }
    }

    private sealed class NewPromotionFileRequest
    {
        [JsonPropertyName("fileUrl")] public string? FileUrl { get; init; }
        [JsonPropertyName("contentType")] public string? ContentType { get; init; }
    }

    private sealed class PromotionFileCreateRequest
    {
        [FromForm(Name = "id")] public int? Id { get; init; }
        [FromForm(Name = "name")] public string? Name { get; init; }
        [FromForm(Name = "active")] public bool? Active { get; init; }
        [FromForm(Name = "validFrom")] public DateOnly? ValidFrom { get; init; }
        [FromForm(Name = "validTo")] public DateOnly? ValidTo { get; init; }
        [FromForm(Name = "description")] public string? Description { get; init; }
        [FromForm(Name = "relatedProducts")] public string? RelatedProducts { get; init; }
        [FromForm(Name = "preferredFileName")] public string? PreferredFileName { get; init; }
        [FromForm(Name = "selectedFile")] public IFormFile? SelectedFile { get; init; }
    }

    private sealed class PromotionFileUpdateRequest
    {
        [FromForm(Name = "id")] public int? Id { get; init; }
        [FromForm(Name = "name")] public string? Name { get; init; }
        [FromForm(Name = "active")] public bool? Active { get; init; }
        [FromForm(Name = "validFrom")] public DateOnly? ValidFrom { get; init; }
        [FromForm(Name = "validTo")] public DateOnly? ValidTo { get; init; }
        [FromForm(Name = "description")] public string? Description { get; init; }
        [FromForm(Name = "relatedProducts")] public string? RelatedProducts { get; init; }
        [FromForm(Name = "preferredFileName")] public string? PreferredFileName { get; init; }
        [FromForm(Name = "selectedFile")] public IFormFile? SelectedFile { get; init; }
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "promotionFileEditor";

    private const int _promotionFileMaxSize = 20 * 1024 * 1024;
    private static readonly TimeOnly _dayStartTimeOnly = new(0, 0, 0);

    private static readonly Popup.MarginOptions _singleEditorPopupMarginOptions = new() { Left = "5vw", Right = "5vw", Top = "5vh", Bottom = "5vh" };

    public static IEndpointConventionBuilder MapPromotionFileEditorPageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute)
            .RequireAuthorization(Policies.Cookie)
            .RequireAuthorization(policy => policy.RequireRole("ProductEditor"));

        endpointGroup.MapGet("/single/new", GetNewSinglePromotionFileEditorPopupAsync);
        endpointGroup.MapGet("/single/{promotionFileId:int}", GetSinglePromotionFileEditorPopupAsync);

        endpointGroup.MapPost("/search", SearchPromotionFilesAsync);

        endpointGroup.MapPost("/promotionFile", GetNewPromotionFileAsync);

        endpointGroup.MapPost("/create", CreatePromotionFileAsync)
            .AddEndpointFilter<AntiforgeryEndpointFilter>();

        endpointGroup.MapPut("/update", UpdatePromotionFileAsync)
            .AddEndpointFilter<AntiforgeryEndpointFilter>();

        endpointGroup.MapDelete("/delete/{promotionFileId:int}", DeletePromotionFileAsync)
            .AddEndpointFilter<AntiforgeryEndpointFilter>();

        return endpointGroup;
    }

    private static async Task<IResult> SearchPromotionFilesAsync(
        [FromBody] SearchPromotionFilesRequest request,
        [FromServices] IPromotionFileService promotionFileService)
    {
        string? userInputString = request.UserInputString;
        DateOnly? validOnDate = request.ValidOnDate;

        List<PromotionFileInfo> promotionFileInfos = new();

        List<PromotionFileInfo> promotionFiles = await promotionFileService.GetAllAsync();

        bool parseUserInputSuccess = int.TryParse(userInputString, out int searchedId);

        foreach (PromotionFileInfo promotionFileInfo in promotionFiles)
        {
            if (validOnDate is not null)
            {
                if (promotionFileInfo.ValidFrom is not null
                    && validOnDate < DateOnly.FromDateTime(promotionFileInfo.ValidFrom.Value))
                {
                    continue;
                }

                if (promotionFileInfo.ValidTo is not null
                    && validOnDate > DateOnly.FromDateTime(promotionFileInfo.ValidTo.Value))
                {
                    continue;
                }
            }

            if (string.IsNullOrWhiteSpace(userInputString))
            {
                promotionFileInfos.Add(promotionFileInfo);

                continue;
            }

            if (promotionFileInfo.Name is not null
                && promotionFileInfo.Name.Contains(userInputString))
            {
                promotionFileInfos.Add(promotionFileInfo);

                continue;
            }

            if (promotionFileInfo.FileName is not null
                && promotionFileInfo.FileName.Contains(userInputString))
            {
                promotionFileInfos.Add(promotionFileInfo);

                continue;
            }

            if (parseUserInputSuccess && promotionFileInfo.Id == searchedId)
            {
                promotionFileInfos.Add(promotionFileInfo);
            }
        }

        return new RazorComponentResult<PromotionFilesTable>(new
        {
            PromotionFileInfos = promotionFileInfos,
        });
    }

    private static async Task<IResult> GetNewSinglePromotionFileEditorPopupAsync()
    {
        CreatePromotionFileEditData createData = new()
        {
            Active = true,
        };

        return new RazorComponentResult<SinglePromotionFileEditorPopup>(new
        {
            IsVisible = true,
            PromotionFileEditOptions = OneOf<CreatePromotionFileEditData, UpdatePromotionFileEditData>.FromT0(createData),
            PopupMarginOptions = _singleEditorPopupMarginOptions,
            ShouldDisplaySaveButton = true,
            SaveButtonText = "Add"
        });
    }

    private static async Task<IResult> GetSinglePromotionFileEditorPopupAsync(
        [FromRoute(Name = "promotionFileId")] int promotionFileId,
        [FromServices] IPromotionFileService promotionFileService)
    {
        PromotionFileInfo? promotionFileInfo = await promotionFileService.GetByIdAsync(promotionFileId);

        if (promotionFileInfo == null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: $"Promotion file (ID: {promotionFileId}) not found.");
        }

        UpdatePromotionFileEditData updateData = new()
        {
            Id = promotionFileInfo.Id,
            Name = promotionFileInfo.Name,
            Active = promotionFileInfo.Active,
            ValidFrom = promotionFileInfo.ValidFrom,
            ValidTo = promotionFileInfo.ValidTo,
            FileName = promotionFileInfo.FileName,
            Description = promotionFileInfo.Description,
            RelatedProductsString = promotionFileInfo.RelatedProductsString,
            CreateUserName = promotionFileInfo.CreateUserName,
            CreateDate = promotionFileInfo.CreateDate,
            LastUpdateUserName = promotionFileInfo.LastUpdateUserName,
            LastUpdateDate = promotionFileInfo.LastUpdateDate,
        };

        return new RazorComponentResult<SinglePromotionFileEditorPopup>(new
        {
            IsVisible = true,
            PromotionFileEditOptions = OneOf<CreatePromotionFileEditData, UpdatePromotionFileEditData>.FromT1(updateData),
            PopupMarginOptions = _singleEditorPopupMarginOptions,
            ShouldDisplaySaveButton = true,
            SaveButtonText = "Save"
        });
    }

    private static async Task<IResult> GetNewPromotionFileAsync(
        [FromBody] NewPromotionFileRequest request)
    {
        if (request == null
            || string.IsNullOrWhiteSpace(request.FileUrl)
            || string.IsNullOrWhiteSpace(request.ContentType))
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Invalid request");

        }

        PromotionFile.FilePathOptions fileOptions = PromotionFile.FilePathOptions.CreateForOtherFile(
                request.FileUrl, request.ContentType);

        return new RazorComponentResult<PromotionFile>(new
        {
            FileOptions = fileOptions,
            AdditionalAttributes = new Dictionary<string, object>()
            {
                ["class"] = "full-parent-width full-parent-height",
            },
        });
    }

    private static async Task<IResult> CreatePromotionFileAsync(
        HttpContext httpContext,
        [FromForm] PromotionFileCreateRequest request,
        [FromServices] IPromotionFileService promotionFileService)
    {
        IFormFile? selectedFileData = request.SelectedFile;

        if (selectedFileData is null || selectedFileData.Length == 0)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Missing or invalid file");
        }

        if (selectedFileData.Length > _promotionFileMaxSize)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"File too large");
        }

        DateTime? validFrom = null;

        if (request.ValidFrom != null)
        {
            validFrom = new DateTime(request.ValidFrom.Value, _dayStartTimeOnly);
        }

        DateTime? validTo = null;

        if (request.ValidTo != null)
        {
            validTo = new DateTime(request.ValidTo.Value, _dayStartTimeOnly);
        }

        string? preferredNameWithoutExtension = request.PreferredFileName;

        string currentUserName = httpContext.User.Identity?.Name!;

        string fileName = selectedFileData.FileName.Trim();

        string? newFileName = fileName;

        if (!string.IsNullOrWhiteSpace(preferredNameWithoutExtension))
        {
            newFileName = preferredNameWithoutExtension + Path.GetExtension(fileName);
        }

        byte[] fileData = await GetFileDataFromBrowserFileAsync(selectedFileData);

        string? name = GetNullWhenEmptyOrWhitespace(request.Name);
        string? description = GetNullWhenEmptyOrWhitespace(request.Description);
        string? relatedProductsString = GetNullWhenEmptyOrWhitespace(request.RelatedProducts);

        CreatePromotionFileRequest createPromotionFileRequest = new()
        {
            FileData = fileData,
            Name = name,
            Active = request.Active ?? false,
            ValidFrom = validFrom,
            ValidTo = validTo,
            FileName = newFileName,
            Description = description,
            RelatedProductsString = relatedProductsString,
            CreateUserName = currentUserName,
        };

        OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> createResult
            = await promotionFileService.InsertAsync(createPromotionFileRequest);

        return await createResult.MatchAsync<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult, IResult>(
            async id =>
            {
                PromotionFileInfo? promotionFileInfo = await promotionFileService.GetByIdAsync(createResult.AsT0);

                return new RazorComponentResult<PromotionFilesTableItem>(new
                {
                    PromotionFileInfo = promotionFileInfo,
                });
            },
            validationResult => ValidationResultProblem(validationResult),
            fileAlreadyExistsResult =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"File '{fileAlreadyExistsResult.FileName} already exists");
            },
            unexpectedFailureResult =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: GenericErrorDetail);
            });
    }

    private static async Task<IResult> UpdatePromotionFileAsync(
        HttpContext httpContext,
        [FromForm] PromotionFileUpdateRequest request,
        [FromServices] IPromotionFileService promotionFileService)
    {
        if (request.Id == null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Id must have value");
        }

        int existingPromotionFileId = request.Id.Value;

        PromotionFileInfo? promotionFile = await promotionFileService.GetByIdAsync(existingPromotionFileId);

        if (promotionFile == null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"Promotion file (ID: {existingPromotionFileId}) does not exist");
        }

        IFormFile? selectedFileData = request.SelectedFile;

        if (selectedFileData != null)
        {
            if (selectedFileData.Length == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"Missing or invalid file");
            }

            if (selectedFileData.Length > _promotionFileMaxSize)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"File too large");
            }
        } 

        DateTime? validFrom = null;

        if (request.ValidFrom != null)
        {
            validFrom = new DateTime(request.ValidFrom.Value, _dayStartTimeOnly);
        }

        DateTime? validTo = null;

        if (request.ValidTo != null)
        {
            validTo = new DateTime(request.ValidTo.Value, _dayStartTimeOnly);
        }

        string? preferredNameWithoutExtension = request.PreferredFileName;

        string currentUserName = httpContext.User.Identity?.Name!;

        FileData? newFileData = null;

        if (selectedFileData is not null)
        {
            string newFileName = selectedFileData.Name.Trim();

            if (!string.IsNullOrWhiteSpace(preferredNameWithoutExtension))
            {
                newFileName = preferredNameWithoutExtension + Path.GetExtension(selectedFileData.Name);
            }

            byte[] fileData = await GetFileDataFromBrowserFileAsync(selectedFileData);

            newFileData = new()
            {
                FileName = newFileName[1..],
                Data = fileData,
            };
        }
        else if (!string.IsNullOrWhiteSpace(preferredNameWithoutExtension))
        {
            Stream? promotionFileDataStream = await promotionFileService.GetFileDataByIdAsync(existingPromotionFileId);

            if (promotionFileDataStream is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: GenericErrorDetail);
            }

            using MemoryStream memoryStream = new();

            await promotionFileDataStream.CopyToAsync(memoryStream);

            byte[] fileData = memoryStream.ToArray();

            newFileData = new()
            {
                FileName = preferredNameWithoutExtension + Path.GetExtension(promotionFile.FileName),
                Data = fileData,
            };
        }

        string? name = GetNullWhenEmptyOrWhitespace(request.Name);
        string? description = GetNullWhenEmptyOrWhitespace(request.Description);
        string? relatedProductsString = GetNullWhenEmptyOrWhitespace(request.RelatedProducts);

        UpdatePromotionFileRequest updatePromotionFileRequest = new()
        {
            Id = existingPromotionFileId,
            Name = name,
            Active = request.Active ?? false,
            ValidFrom = validFrom,
            ValidTo = validTo,
            Description = description,
            RelatedProductsString = relatedProductsString,
            NewFileData = newFileData,
            UpdateUserName = currentUserName,
        };

        OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updateResult
            = await promotionFileService.UpdateAsync(updatePromotionFileRequest);

        return await updateResult.MatchAsync<
            Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult, IResult>(
            async success =>
            {
                PromotionFileInfo? promotionFileInfo = await promotionFileService.GetByIdAsync(existingPromotionFileId);

                return new RazorComponentResult<PromotionFilesTableItem>(new
                {
                    PromotionFileInfo = promotionFileInfo,
                });
            },
            validationResult => ValidationResultProblem(validationResult),
            fileDoesntExistResult =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: "File does not exist");
            },
            fileAlreadyExistsResult =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: "File already exists");
            },
            unexpectedFailureResult =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: GenericErrorDetail);
            });
    }

    private static async Task<byte[]> GetFileDataFromBrowserFileAsync(IFormFile file)
    {
        using MemoryStream memoryStream = new();

        await file.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    private static string? GetNullWhenEmptyOrWhitespace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return value;
    }

    private static async Task<IResult> DeletePromotionFileAsync(
        [FromRoute(Name = "promotionFileId")] int promotionFileId,
        [FromServices] IPromotionFileService promotionFileService)
    {
        OneOf<Success, OneOf.Types.NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult> deleteFileResult
            = await promotionFileService.DeleteAsync(promotionFileId);

        return deleteFileResult.Match(
            success => Results.Ok(),
            notFound =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"Promotion file (ID: {promotionFileId}) not found.");
            },
            promotionFileHasRelationsResult =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: $"Promotion file has relations");
            },
            fileDoesntExistResult =>
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: GenericErrorDetail);
            });
    }
}
