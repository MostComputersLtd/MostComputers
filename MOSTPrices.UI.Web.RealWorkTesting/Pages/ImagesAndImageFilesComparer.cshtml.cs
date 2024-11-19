using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Models.ImagesAndImageFilesComparison;
using MOSTComputers.UI.Web.RealWorkTesting.Services.Contracts;
using MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ImagesAndImageFilesComparer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages;

[Authorize]
public class ImagesAndImageFilesComparerModel : PageModel
{
    public ImagesAndImageFilesComparerModel(ICategoryService categoryService, IImageComparisonDataService imageComparisonDataService)
    {
        _categoryService = categoryService;
        _imageComparisonDataService = imageComparisonDataService;
    }

    private const string _categorySelectElementId = "images_comparer_category_select";
    private const string _productIdInputElementId = "images_comparer_product_id_input";

    private readonly ICategoryService _categoryService;
    private readonly IImageComparisonDataService _imageComparisonDataService;

    public static ImageComparisonDataDisplayPartialModel FirstImagesToCompare { get; private set; } = new()
    {
        ImageComparisonDataService = null,
        DataSource = null,
        ElementIdAndNamePrefix = GetElementIdAndNamePrefixBasedOnLocation(ImageComparisonDataPlacementLocationEnum.FirstImageDataList)!,
        PartialViewContainerId = GetPartialViewContainerIdBasedOnLocation(ImageComparisonDataPlacementLocationEnum.FirstImageDataList)!,
        CategorySelectElementId = _categorySelectElementId,
        ProductIdInputElementId = _productIdInputElementId,
        DataPlacementLocation = ImageComparisonDataPlacementLocationEnum.FirstImageDataList
    };

    public static ImageComparisonDataDisplayPartialModel SecondImagesToCompare { get; private set; } = new()
    {
        ImageComparisonDataService = null,
        DataSource = null,
        ElementIdAndNamePrefix = GetElementIdAndNamePrefixBasedOnLocation(ImageComparisonDataPlacementLocationEnum.SecondImageDataList)!,
        PartialViewContainerId = GetPartialViewContainerIdBasedOnLocation(ImageComparisonDataPlacementLocationEnum.SecondImageDataList)!,
        CategorySelectElementId = _categorySelectElementId,
        ProductIdInputElementId = _productIdInputElementId,
        DataPlacementLocation = ImageComparisonDataPlacementLocationEnum.SecondImageDataList
    };

    public IStatusCodeActionResult OnPostGetImageDataForCategory(
        int categoryId,
        ImageComparisonDataPlacementLocationEnum imageComparisonDataPlacementLocation,
        ImageComparisonDataSourceEnum dataSource)
    {
        string? elementIdAndNamePrefix = GetElementIdAndNamePrefixBasedOnLocation(imageComparisonDataPlacementLocation);
        string? partialViewContainerId = GetPartialViewContainerIdBasedOnLocation(imageComparisonDataPlacementLocation);

        if (elementIdAndNamePrefix is null
            || partialViewContainerId is null)
        {
            return StatusCode(500, new NotSupportedException());
        }

        ImageComparisonDataDisplayPartialModel imageComparisonDataDisplayPartialModel = new()
        {
            DataSource = dataSource,
            DataPlacementLocation = imageComparisonDataPlacementLocation,
            ElementIdAndNamePrefix = elementIdAndNamePrefix,
            PartialViewContainerId = partialViewContainerId,
            CategorySelectElementId = _categorySelectElementId,
            ProductIdInputElementId = _productIdInputElementId,
            ImageComparisonDataService = _imageComparisonDataService,
            CurrentCategoryId = categoryId,
        };

        AddDataToCorrectModel(imageComparisonDataPlacementLocation, imageComparisonDataDisplayPartialModel);

        return Partial("ImagesAndImageFilesComparer/_ImageComparisonDataDisplayPartial", imageComparisonDataDisplayPartialModel);
    }

    public IStatusCodeActionResult OnPostGetImageDataForProduct(
       int productId,
       ImageComparisonDataPlacementLocationEnum imageComparisonDataPlacementLocation,
       ImageComparisonDataSourceEnum dataSource)
    {
        string? elementIdAndNamePrefix = GetElementIdAndNamePrefixBasedOnLocation(imageComparisonDataPlacementLocation);
        string? partialViewContainerId = GetPartialViewContainerIdBasedOnLocation(imageComparisonDataPlacementLocation);

        if (elementIdAndNamePrefix is null
            || partialViewContainerId is null)
        {
            return StatusCode(500, new NotSupportedException());
        }

        ImageComparisonDataDisplayPartialModel imageComparisonDataDisplayPartialModel = new()
        {
            DataSource = dataSource,
            DataPlacementLocation = imageComparisonDataPlacementLocation,
            ElementIdAndNamePrefix = elementIdAndNamePrefix,
            PartialViewContainerId = partialViewContainerId,
            CategorySelectElementId = _categorySelectElementId,
            ProductIdInputElementId = _productIdInputElementId,
            ImageComparisonDataService = _imageComparisonDataService,
            CurrentProductId = productId,
        };

        AddDataToCorrectModel(imageComparisonDataPlacementLocation, imageComparisonDataDisplayPartialModel);

        return Partial("ImagesAndImageFilesComparer/_ImageComparisonDataDisplayPartial", imageComparisonDataDisplayPartialModel);
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _categoryService.GetAll();
    }

    private static string? GetElementIdAndNamePrefixBasedOnLocation(
        ImageComparisonDataPlacementLocationEnum imageComparisonDataPlacementLocation)
    {
        return imageComparisonDataPlacementLocation switch
        {
            ImageComparisonDataPlacementLocationEnum.FirstImageDataList => "first_image_comparison_data_display",
            ImageComparisonDataPlacementLocationEnum.SecondImageDataList => "second_image_comparison_data_display",
            _ => null
        };
    }

    private static string? GetPartialViewContainerIdBasedOnLocation(
        ImageComparisonDataPlacementLocationEnum imageComparisonDataPlacementLocation)
    {
        return imageComparisonDataPlacementLocation switch
        {
            ImageComparisonDataPlacementLocationEnum.FirstImageDataList => "images_comparer_first_images_container",
            ImageComparisonDataPlacementLocationEnum.SecondImageDataList => "images_comparer_second_images_container",
            _ => null
        };
    }

    private static void AddDataToCorrectModel(
        ImageComparisonDataPlacementLocationEnum imageComparisonDataLocation,
        ImageComparisonDataDisplayPartialModel imageComparisonDataDisplayPartialModel)
    {
        if (imageComparisonDataLocation == ImageComparisonDataPlacementLocationEnum.FirstImageDataList)
        {
            FirstImagesToCompare = imageComparisonDataDisplayPartialModel;
        }
        else
        {
            SecondImagesToCompare = imageComparisonDataDisplayPartialModel;
        }
    }
}