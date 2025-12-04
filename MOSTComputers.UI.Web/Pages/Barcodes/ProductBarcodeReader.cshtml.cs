using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductIdentifiers.ProductGTINCode;
using MOSTComputers.Services.ProductRegister.Services.ProductIdentifiers.Contracts;
using MOSTComputers.UI.Web.Pages.Shared.Barcodes;
using OneOf;
using OneOf.Types;

using static MOSTComputers.UI.Web.Utils.PageCommonElements;
using static MOSTComputers.UI.Web.Utils.SelectListItemUtils;
using static MOSTComputers.UI.Web.Validation.ValidationCommonElements;

namespace MOSTComputers.UI.Web.Pages.Barcodes;

[AllowAnonymous]
public sealed class ProductBarcodeReaderModel : PageModel
{
    public ProductBarcodeReaderModel(IProductGTINCodeService productGTINCodeService)
    {
        _productGTINCodeService = productGTINCodeService;
    }

    internal const string ProductBarcodeReaderGTINCodeResultPartialPath = "Barcodes/_ProductBarcodeReaderGTINCodeResultPartial";

    private readonly IProductGTINCodeService _productGTINCodeService;

    public PartialViewResult OnGetGetGTINCodeListItemPartial(
        int elementIndex,
        string gtinCode,
        int? selectedGtinCodeType = null,
        bool? enableSaveButton = null)
    {
        return GetGTINCodeListItemPartial(elementIndex, gtinCode, selectedGtinCodeType, enableSaveButton);
    }

    public async Task<IActionResult> OnPostUpsertGTINCodeToProductAsync(int elementIndex, string gtinCode, int? codeType = null, int? productId = null)
    {
        string? currentUserName = GetUserName(User);

        if (currentUserName is null) return Unauthorized();

        if (codeType is null || productId == null)
        {
            List<SelectListItem> allowedGTINCodeTypesSelectListItems
                = GetAllowedGTINCodeTypesSelectListItems(gtinCode, null);

            ProductBarcodeReaderGTINCodeResultPartialModel model = new()
            {
                ElementIndex = elementIndex,
                GTINCode = gtinCode,
                AllowedGTINCodeTypesSelectListItems = allowedGTINCodeTypesSelectListItems,
                IncludeSaveButton = allowedGTINCodeTypesSelectListItems.Count > 0,
            };
            
            return Partial(ProductBarcodeReaderGTINCodeResultPartialPath, model);
        }

        ProductGTINCodeType? gtinCodeType = ProductGTINCodeType.FromValue(codeType.Value);

        if (gtinCodeType is null)
        {
            return BadRequest("Invalid GTIN code type specified.");
        }

        bool isValidFormat = gtinCodeType.IsValid(gtinCode);

        if (!isValidFormat) return BadRequest("Invalid barcode format.");

        ServiceProductGTINCodeUpsertRequest upsertRequest = new()
        {
            ProductId = productId.Value,
            CodeType = gtinCodeType,
            CodeTypeAsText = GetStringFromProductGTINCodeType(gtinCodeType)!,
            Value = gtinCode,
            UpsertUserName = currentUserName,
        };

        OneOf<Success, ValidationResult, UnexpectedFailureResult> createProductGTINCodeResult
            = await _productGTINCodeService.UpsertAsync(upsertRequest);

        return createProductGTINCodeResult.Match<IActionResult>(
            success =>
            {
                return GetGTINCodeListItemPartial(elementIndex, gtinCode, gtinCodeType.Value, true);
            },
            validationResult => GetBadRequestResultFromValidationResult(validationResult),
            unexpectedFailure => StatusCode(500));
    }

    public List<SelectListItem> GetAllowedGTINCodeTypesSelectListItems(
        string eanCode, ProductGTINCodeType? gtinCodeType = null)
    {
        List<ProductGTINCodeType> allowedGTINCodeTypes = ProductGTINCodeType.AllTypes
            .Where(x => x.IsValid(eanCode))
            .ToList();

        List<SelectListItem> allowedGTINCodeTypesSelectListItems
            = GetProductGTINCodeTypeSelectListItems(allowedGTINCodeTypes, gtinCodeType);

        return allowedGTINCodeTypesSelectListItems;
    }

    public IActionResult OnPostSerialNumber(string serialNumber)
    {
        return Page();
    }

    public async Task<IActionResult> OnDeleteDeleteProductGTINCodeAsync(int productId, int gtinCodeType)
    {
        ProductGTINCodeType? productGTINCodeType = ProductGTINCodeType.FromValue(gtinCodeType);

        if (productGTINCodeType is null)
        {
            return BadRequest("Invalid GTIN code type specified.");
        }

        OneOf<Success, NotFound> deleteResult = await _productGTINCodeService.DeleteAsync(productId, productGTINCodeType);

        return deleteResult.Match<IActionResult>(
            success => new OkResult(),
            notFound => NotFound());
    }

    public PartialViewResult GetGTINCodeListItemPartial(
        int elementIndex,
        string gtinCode,
        int? selectedGtinCodeType = null,
        bool? enableSaveButton = null)
    {
        ProductBarcodeReaderGTINCodeResultPartialModel model
            = GetGTINCodeListItemModelPartial(elementIndex, gtinCode, selectedGtinCodeType, enableSaveButton);

        return Partial(ProductBarcodeReaderGTINCodeResultPartialPath, model);
    }

    private ProductBarcodeReaderGTINCodeResultPartialModel GetGTINCodeListItemModelPartial(
        int elementIndex,
        string gtinCode,
        int? selectedGtinCodeType = null,
        bool? enableSaveButton = null)
    {
        ProductGTINCodeType? gtinCodeType = null;

        if (selectedGtinCodeType is not null)
        {
            gtinCodeType = ProductGTINCodeType.FromValue(selectedGtinCodeType.Value);
        }

        List<SelectListItem> allowedGTINCodeTypesSelectListItems
            = GetAllowedGTINCodeTypesSelectListItems(gtinCode, gtinCodeType);

        enableSaveButton ??= allowedGTINCodeTypesSelectListItems.Count > 0;

        return new()
        {
            ElementIndex = elementIndex,
            GTINCode = gtinCode,
            AllowedGTINCodeTypesSelectListItems = allowedGTINCodeTypesSelectListItems,
            IncludeSaveButton = enableSaveButton.Value,
        };
    }

    private void AddItemToTempDataList<T>(string tempDataName, T item)
    {
        if (TempData[tempDataName] is not List<T> tempDataList)
        {
            tempDataList = new List<T>();

            TempData[tempDataName] = tempDataList;
        }

        tempDataList.Add(item);
    }
}