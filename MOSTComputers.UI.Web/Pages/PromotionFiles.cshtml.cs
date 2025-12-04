using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionFile;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.UI.Web.Models;
using MOSTComputers.UI.Web.Models.PromotionFiles;
using MOSTComputers.UI.Web.Models.PromotionFiles.DTOs;
using MOSTComputers.UI.Web.Services.Contracts;
using MOSTComputers.UI.Web.Services.Data.Contracts;
using MOSTComputers.UI.Web.Pages.Shared.PromotionFiles;

using static MOSTComputers.UI.Web.Utils.FilePathUtils;
using static MOSTComputers.UI.Web.Utils.PageCommonElements;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;
using static MOSTComputers.Utils.OneOf.AsyncMatchingExtensions;
using MOSTComputers.Services.ProductRegister.Models.Requests;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;

namespace MOSTComputers.UI.Web.Pages;

[Authorize]
public class PromotionFilesModel : PageModel
{
    public PromotionFilesModel(
        IPromotionFileEditorDataService promotionFileEditorDataService,
        IPromotionFileService promotionFileService,
        IPartialViewRenderService partialViewRenderService)
    {
        _promotionFileEditorDataService = promotionFileEditorDataService;
        _promotionFileService = promotionFileService;
        _partialViewRenderService = partialViewRenderService;
    }

    private readonly IPromotionFileEditorDataService _promotionFileEditorDataService;
    private readonly IPromotionFileService _promotionFileService;
    private readonly IPartialViewRenderService _partialViewRenderService;

    private const string _fileHasRelationsErrorName = "PromotionFileHasRelations";

    internal readonly string NotificationBoxId = "topNotificationBox";
    internal readonly string PromotionFilesTableContainerElementId = "promotionFilesTableContainer";

    internal readonly ModalData PromotionFileSingleEditorModalData = new()
    {
        ModalId = "promotionFileSingleEditor_modal",
        ModalDialogId = "promotionFileSingleEditor_modal_dialog",
        ModalContentId = "promotionFileSingleEditor_modal_content",
    };

    public PromotionFilesSearchOptions? SearchOptions { get; private set; }
    public IReadOnlyList<PromotionFileInfoEditorDisplayData>? PromotionFiles { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (SearchOptions is null) return Page();

        PromotionFiles = await _promotionFileEditorDataService.GetSearchedFilesAsync(SearchOptions);

        return Page();
    }

    public async Task<IStatusCodeActionResult> OnGetGetPromotionFileSingleEditorPartialAsync(int? promotionFileInfoId = null)
    {
        if (promotionFileInfoId is null)
        {
            return GetPromotionFileSingleEditorPartial(PromotionFileSingleEditorModalData, null);
        }

        PromotionFileInfoEditorDisplayData? existingPromotionFileInfo = await _promotionFileEditorDataService.GetFileAsync(promotionFileInfoId.Value);

        return GetPromotionFileSingleEditorPartial(PromotionFileSingleEditorModalData, existingPromotionFileInfo);
    }

    public async Task<IStatusCodeActionResult> OnPostSearchFilesAsync([FromBody] PromotionFilesSearchOptions? searchOptions = null)
    {
        List<PromotionFileInfoEditorDisplayData> promotionFileEditorDatas = await SearchFilesAsync(searchOptions);

        return GetPromotionFileViewPartial(promotionFileEditorDatas);
    }

    public async Task<IStatusCodeActionResult> OnPostAddNewFileAsync(
        [FromForm(Name = "promotionFileInsertData")] PromotionFileInsertDTO promotionFileData,
        [FromForm(Name = "searchOptions")] PromotionFilesSearchOptions? searchOptions = null)
    {
        if (promotionFileData.File is null) return BadRequest();

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        using MemoryStream stream = new();

        promotionFileData.File.CopyTo(stream);

        byte[] fileData = stream.ToArray();

        if (fileData.Length <= 0) return BadRequest();

        string fileName = Path.GetFileName(promotionFileData.File.FileName.Trim());

        CreatePromotionFileRequest promotionFileInfoCreateRequest = new()
        {
            Name = promotionFileData.Name,
            FileName = fileName,
            FileData = fileData,
            Active = promotionFileData.Active,
            ValidFrom = promotionFileData.ValidFrom,
            ValidTo = promotionFileData.ValidTo,
            Description = promotionFileData.Description,
            RelatedProductsString = promotionFileData.RelatedProductsString,
            CreateUserName = currentUserName
        };

        OneOf<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult> insertPromotionFileInfoResult
            = await _promotionFileService.InsertAsync(promotionFileInfoCreateRequest);

        return await insertPromotionFileInfoResult.MatchAsync<int, ValidationResult, FileAlreadyExistsResult, UnexpectedFailureResult, IStatusCodeActionResult>(
            async id =>
            {
                List<PromotionFileInfoEditorDisplayData> promotionFileInfosInEditor = await SearchFilesAsync(searchOptions);

                PromotionFileInfoEditorDisplayData? promotionFileEditorData
                   = promotionFileInfosInEditor.FirstOrDefault(x => x.Id == id);

                if (promotionFileEditorData is null)
                {
                    promotionFileEditorData = await _promotionFileEditorDataService.GetFileAsync(id);

                    if (promotionFileEditorData is not null)
                    {
                        promotionFileInfosInEditor.Add(promotionFileEditorData);
                    }
                }

                string promotionFileViewPartialAsString = await GetPromotionFileViewPartialAsStringAsync(promotionFileInfosInEditor);
                string promotionFileSingleEditorPartialAsString = await GetPromotionFileSingleEditorPartialAsStringAsync(
                    PromotionFileSingleEditorModalData, promotionFileEditorData);

                return new JsonResult(new
                {
                    promotionFileViewPartialAsString = promotionFileViewPartialAsString,
                    promotionFileSingleEditorPartialAsString = promotionFileSingleEditorPartialAsString,
                });
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IStatusCodeActionResult> OnPutUpdateFileAsync(
        [FromForm(Name = "promotionFileUpdateData")] PromotionFileUpdateDTO promotionFileData,
        [FromForm(Name = "searchOptions")] PromotionFilesSearchOptions? searchOptions = null)
    {
        if (promotionFileData.Id is null)
        {
            return BadRequest();
        }

        string? currentUserName = GetUserName(User);

        if (string.IsNullOrWhiteSpace(currentUserName)) return Unauthorized();

        FileData? fileUpdateData = null;

        if (promotionFileData.File is not null)
        {
            using MemoryStream stream = new();

            promotionFileData.File.CopyTo(stream);

            byte[] newFileData = stream.ToArray();

            string? newFileName = promotionFileData.File.FileName.Trim();

            if (string.IsNullOrWhiteSpace(newFileName)) return BadRequest();

            fileUpdateData = new()
            {
                FileName = newFileName,
                Data = newFileData,
            };
        }

        int promotionFileInfoId = promotionFileData.Id.Value;

        UpdatePromotionFileRequest promotionFileInfoUpdateRequest = new()
        {
            Id = promotionFileInfoId,
            Name = promotionFileData.Name,
            NewFileData = fileUpdateData,
            Active = promotionFileData.Active,
            ValidFrom = promotionFileData.ValidFrom,
            ValidTo = promotionFileData.ValidTo,
            Description = promotionFileData.Description,
            RelatedProductsString = promotionFileData.RelatedProductsString,
            UpdateUserName = currentUserName,
        };

        OneOf<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult> updatePromotionFileInfoResult
            = await _promotionFileService.UpdateAsync(promotionFileInfoUpdateRequest);

        return await updatePromotionFileInfoResult.MatchAsync<Success, ValidationResult, FileDoesntExistResult, FileAlreadyExistsResult, UnexpectedFailureResult, IStatusCodeActionResult>(
            async success =>
            {
                List<PromotionFileInfoEditorDisplayData> promotionFileInfosInEditor = await SearchFilesAsync(searchOptions);

                PromotionFileInfoEditorDisplayData? promotionFileEditorData
                   = promotionFileInfosInEditor.FirstOrDefault(x => x.Id == promotionFileInfoId);

                if (promotionFileEditorData is null)
                {
                    promotionFileEditorData = await _promotionFileEditorDataService.GetFileAsync(promotionFileInfoId);

                    if (promotionFileEditorData is not null)
                    {
                        promotionFileInfosInEditor.Add(promotionFileEditorData);
                    }
                }
                string promotionFileViewPartialAsString = await GetPromotionFileViewPartialAsStringAsync(promotionFileInfosInEditor);
                string promotionFileSingleEditorPartialAsString = await GetPromotionFileSingleEditorPartialAsStringAsync(
                    PromotionFileSingleEditorModalData, promotionFileEditorData);

                return new JsonResult(new
                {
                    promotionFileViewPartialAsString = promotionFileViewPartialAsString,
                    promotionFileSingleEditorPartialAsString = promotionFileSingleEditorPartialAsString,
                });
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileDoesntExistResult => BadRequest(fileDoesntExistResult),
            fileAlreadyExistsResult => BadRequest(fileAlreadyExistsResult),
            unexpectedFailureResult => StatusCode(StatusCodes.Status500InternalServerError));
    }

    public async Task<IStatusCodeActionResult> OnDeleteDeleteFileAsync(int promotionFileId,
        [FromBody] PromotionFilesSearchOptions? searchOptions = null)
    {
        if (promotionFileId <= 0) return BadRequest();

        OneOf<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult> deletePromotionFileResult
            = await _promotionFileService.DeleteAsync(promotionFileId);

        return await deletePromotionFileResult.MatchAsync<Success, NotFound, PromotionFileHasRelationsResult, FileDoesntExistResult, IStatusCodeActionResult>(
            async success =>
            {
                List<PromotionFileInfoEditorDisplayData> promotionFileInfosInEditor = await SearchFilesAsync(searchOptions);

                return GetPromotionFileViewPartial(promotionFileInfosInEditor);
            },
            notFound => NotFound(),
            promotionFileHasRelationsResult =>
            {
                var promotionFileHasRelations = new
                {
                    ErrorName = _fileHasRelationsErrorName,
                    Data = promotionFileHasRelationsResult
                };

                return BadRequest(promotionFileHasRelations);
            },
            fileDoesntExistResult => NotFound(fileDoesntExistResult));
    }

    private async Task<List<PromotionFileInfoEditorDisplayData>> SearchFilesAsync(PromotionFilesSearchOptions? searchOptions = null)
    {
        if (searchOptions is null)
        {
            return await _promotionFileEditorDataService.GetAllFilesAsync();
        }
        
        return await _promotionFileEditorDataService.GetSearchedFilesAsync(searchOptions);
    }

    private PartialViewResult GetPromotionFileViewPartial(List<PromotionFileInfoEditorDisplayData>? promotionFileInfos)
    {
        PromotionFileViewPartialModel promotionFileViewPartialModel = new()
        {
            ContainerElementId = PromotionFilesTableContainerElementId,
            PromotionFiles = promotionFileInfos,
            PromotionFileSingleEditorModalData = PromotionFileSingleEditorModalData,
            NotificationBoxId = NotificationBoxId,
        };

        return Partial("PromotionFiles/_PromotionFileViewPartial", promotionFileViewPartialModel);
    }

    private async Task<string> GetPromotionFileViewPartialAsStringAsync(List<PromotionFileInfoEditorDisplayData>? promotionFileInfos)
    {
        PromotionFileViewPartialModel promotionFileViewPartialModel = new()
        {
            ContainerElementId = PromotionFilesTableContainerElementId,
            PromotionFiles = promotionFileInfos,
            PromotionFileSingleEditorModalData = PromotionFileSingleEditorModalData,
            NotificationBoxId = NotificationBoxId,
        };

        return await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, "PromotionFiles/_PromotionFileViewPartial", promotionFileViewPartialModel);
    }

    private PartialViewResult GetPromotionFileSingleEditorPartial(ModalData modalData,
        PromotionFileInfoEditorDisplayData? existingPromotionFileInfo)
    {
        PromotionFileSingleEditorPartialModel promotionFileSingleEditorPartialModel = new()
        {
            ModalData = modalData,
            PromotionFilesTableContainerElementId = PromotionFilesTableContainerElementId,
            ExistingPromotionFileInfo = existingPromotionFileInfo,
        };

        return Partial("PromotionFiles/_PromotionFileSingleEditorPartial", promotionFileSingleEditorPartialModel);
    }

    private async Task<string> GetPromotionFileSingleEditorPartialAsStringAsync(ModalData modalData,
        PromotionFileInfoEditorDisplayData? existingPromotionFileInfo)
    {
        PromotionFileSingleEditorPartialModel promotionFileSingleEditorPartialModel = new()
        {
            ModalData = modalData,
            PromotionFilesTableContainerElementId = PromotionFilesTableContainerElementId,
            ExistingPromotionFileInfo = existingPromotionFileInfo,
        };

        return await _partialViewRenderService.RenderPartialViewToStringAsync(
            this, "PromotionFiles/_PromotionFileSingleEditorPartial", promotionFileSingleEditorPartialModel);
    }
}