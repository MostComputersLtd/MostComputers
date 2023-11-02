using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.UI.Web.Pages.CreatePages;

public class ProductCreateModel : PageModel
{
    public ProductCreateModel(
        ICategoryService categoryService,
        IManifacturerService manifacturerService,
        IProductCharacteristicService productCharacteristicService)
    {
        _categoryService = categoryService;
        _manifacturerService = manifacturerService;
        _productCharacteristicService = productCharacteristicService;
    }

    private readonly ICategoryService _categoryService;
    private readonly IManifacturerService _manifacturerService;
    private readonly IProductCharacteristicService _productCharacteristicService;

    [Required(AllowEmptyStrings = false, ErrorMessage = $"{nameof(Id)} must not be empty")]
    public int Id { get; set; }
    public string? SearchString { get; set; }
    public decimal? AddWrr { get; set; }
    public long? AddWrrTerm { get; set; }
    public string? AddWrrDef { get; set; }
    public long? DefWrrTerm { get; set; }
    public int? DisplayOrder { get; set; }
    public ProductStatusEnum? Status { get; set; }
    public int? PlShow { get; set; }
    public decimal? Price1 { get; set; }
    public decimal? Price2 { get; set; }
    public decimal? Price3 { get; set; }
    public CurrencyEnum? Currency { get; set; }
    public int? PromPid { get; set; }
    public int? PromRid { get; set; }
    public short? PromPictureId { get; set; }
    public DateTime? PromExpDate { get; set; }
    public short? AlertPictureId { get; set; }
    public DateTime? AlertExpDate { get; set; }
    public string? PriceListDescription { get; set; }
    public string? SplModel1 { get; set; }
    public string? SplModel2 { get; set; }
    public string? SplModel3 { get; set; }

    public List<CurrentProductPropertyCreateRequest>? Properties { get; set; }
    public List<CurrentProductImageCreateRequest>? Images { get; set; }
    public List<CurrentProductImageFileNameInfoCreateRequest>? ImageFileNames { get; set; }

    public int? CategoryID { get; set; }
    public short? ManifacturerId { get; set; }
    public int? SubCategoryId { get; set; }

    public List<SelectListItem> CategorySelectItems { get; set; }
    public List<SelectListItem> ManifacturerSelectItems { get; set; }
    public List<SelectListItem> ProductCharacteristicsForGivenCategory { get; set; }

    public void OnGet()
    {
        CategorySelectItems_Initialize();
        ManifacturerSelectItems_Initialize();
        OnGetProductCharacteristicsForGivenCategory_Initialize(12);
    }

    private void CategorySelectItems_Initialize()
    {
        CategorySelectItems = _categoryService.GetAll()
                    .Select(category => new SelectListItem() { Text = category.Description, Value = category.Id.ToString() })
                    .OrderBy(x => x.Text)
                    .ToList();

        CategorySelectItems.Add(new() { Selected = true, Disabled = true, Text = "-- Select a Category --" });
    }

    private void ManifacturerSelectItems_Initialize()
    {
        ManifacturerSelectItems = _manifacturerService.GetAll()
                    .Select(manifacturer => new SelectListItem() { Text = manifacturer.RealCompanyName, Value = manifacturer.Id.ToString() })
                    .OrderBy(x => x.Text)
                    .ToList();

        ManifacturerSelectItems.Add(new() { Selected = true, Disabled = true, Text = "-- Select a Manifacturer --" });
    }

    public IActionResult OnGetProductCharacteristicsForGivenCategory_Initialize(int categoryId)
    {
        IEnumerable<ProductCharacteristic> productCharacteristics = _productCharacteristicService.GetAllByCategoryId((uint)categoryId)
            .Where(x => !string.IsNullOrEmpty(x.Name));

        ProductCharacteristicsForGivenCategory = productCharacteristics
            .Select(characteristic => new SelectListItem() { Text = characteristic.Name, Value = characteristic.Id.ToString() })
            .ToList();

        Properties ??= new List<CurrentProductPropertyCreateRequest> ();

        foreach (var item in productCharacteristics)
        {
            Properties.Add(new CurrentProductPropertyCreateRequest { ProductCharacteristicId = item.Id });
        }

        return StatusCode(200);
    }

    public void OnPost()
    {
    }
}