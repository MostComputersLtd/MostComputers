using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ImagesAndImageFilesComparison;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using OneOf;
using FluentValidation.Results;

using static MOSTComputers.UI.Web.RealWorkTesting.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ImagesAndImageFilesComparer;

public class ImageComparisonDataDisplayPartialModel
{
    public required ImageComparisonDataSourceEnum? DataSource { get; set; }
    public required ImageComparisonDataPlacementLocationEnum DataPlacementLocation { get; set; }

    public required string PartialViewContainerId { get; init; }
    public required string CategorySelectElementId { get; init; }
    public required string ProductIdInputElementId { get; init; }
    public int? CurrentCategoryId { get; set; }
    public int? CurrentProductId { get; set; }

    public required string ElementIdAndNamePrefix { get; init; }

    public required IImageComparisonDataService? ImageComparisonDataService { private get; init; }

    public async Task<IStatusCodeActionResult> GetImageComparisonDataAsync()
    {
        if (DataSource is null
            || ImageComparisonDataService is null)
        {
            return new BadRequestResult();
        }

        if (CurrentProductId is not null)
        {
            return await GetImageComparisonDataForProductAsync();
        }
        else if (CurrentCategoryId is not null)
        {
            return await GetImageComparisonDataForCategoryAsync();
        }

        return new BadRequestResult();
    }

    public async Task<IStatusCodeActionResult> GetImageComparisonDataForCategoryAsync()
    {
        if (CurrentCategoryId is null
            || ImageComparisonDataService is null
            || DataSource is null)
        {
            return new BadRequestResult();
        }

        OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException> getImageComparisonDataForCategoryResult
            = await ImageComparisonDataService.GetImagesForCategoryAsync(CurrentCategoryId.Value, DataSource.Value);

        return getImageComparisonDataForCategoryResult.Match(
            imageComparisonData => new OkObjectResult(imageComparisonData),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileDoesntExistResult => new NotFoundObjectResult(fileDoesntExistResult.FileName),
            notSupportedFileTypeResult =>
            {
                ValidationResult notSupportedFileTypeValidationResult = new();

                notSupportedFileTypeValidationResult.Errors.Add(new(nameof(notSupportedFileTypeResult.FileExtension),
                    $"This file type (.{notSupportedFileTypeResult.FileExtension}) is not supported"));

                return GetBadRequestResultFromValidationResult(notSupportedFileTypeValidationResult);
            },
            notImplementedException => new StatusCodeResult(500));
    }

    public async Task<IStatusCodeActionResult> GetImageComparisonDataForProductAsync()
    {
        if (CurrentProductId is null
            || ImageComparisonDataService is null
            || DataSource is null)
        {
            return new BadRequestResult();
        }

        OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException> getImageComparisonDataForProductResult
            = await ImageComparisonDataService.GetImagesForProductAsync(CurrentProductId.Value, DataSource.Value);

        return getImageComparisonDataForProductResult.Match(
            imageComparisonData => new OkObjectResult(imageComparisonData),
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            fileDoesntExistResult => new NotFoundObjectResult(fileDoesntExistResult.FileName),
            notSupportedFileTypeResult =>
            {
                ValidationResult notSupportedFileTypeValidationResult = new();

                notSupportedFileTypeValidationResult.Errors.Add(new(nameof(notSupportedFileTypeResult.FileExtension),
                    $"This file type (.{notSupportedFileTypeResult.FileExtension}) is not supported"));

                return GetBadRequestResultFromValidationResult(notSupportedFileTypeValidationResult);
            },
            notImplementedException => new StatusCodeResult(500));
    }
}