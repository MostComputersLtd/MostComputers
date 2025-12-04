using Microsoft.AspNetCore.Mvc.Rendering;

namespace MOSTComputers.UI.Web.Pages.Shared.Barcodes;

public sealed class ProductBarcodeReaderGTINCodeResultPartialModel
{
    public required int ElementIndex { get; init; }
    public string? GTINCode { get; init; }
    public List<SelectListItem>? AllowedGTINCodeTypesSelectListItems { get; init; }
    public required bool IncludeSaveButton { get; init; }
}