using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;

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
    public string? Name { get; set; }
    public decimal? AdditionalWarrantyPrice { get; set; }
    public long? AdditionalWarrantyTermMonths { get; set; }
    public string? StandardWarrantyPrice { get; set; }
    public long? StandardWarrantyTermMonths { get; set; }
    public int? DisplayOrder { get; set; }
    public ProductStatusEnum? Status { get; set; }
    public int? PlShow { get; set; }
    public decimal? Price1 { get; set; }
    public decimal? DisplayPrice { get; set; }
    public decimal? Price3 { get; set; }
    public CurrencyEnum? Currency { get; set; }
    public int? Promotionid { get; set; }
    public int? PromRid { get; set; }
    public short? PromotionPictureId { get; set; }
    public DateTime? PromotionExpireDate { get; set; }
    public short? AlertPictureId { get; set; }
    public DateTime? AlertExpireDate { get; set; }
    public string? PriceListDescription { get; set; }
    public string? PartNumber1 { get; set; }
    public string? PartNumber2 { get; set; }
    public string? SearchString { get; set; }

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
        IEnumerable<ProductCharacteristic> productCharacteristics = _productCharacteristicService.GetAllByCategoryId(categoryId)
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