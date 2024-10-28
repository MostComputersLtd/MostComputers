using FluentValidation.Results;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ImagesAndImageFilesComparison;
using OneOf;

namespace MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
public interface IImageComparisonDataService
{
    Task<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException>> GetImagesForCategoryAsync(int categoryId, ImageComparisonDataSourceEnum dataSourceEnum);
    Task<OneOf<List<ImageComparisonData>, ValidationResult, FileDoesntExistResult, NotSupportedFileTypeResult, NotImplementedException>> GetImagesForProductAsync(int productId, ImageComparisonDataSourceEnum dataSource);
}