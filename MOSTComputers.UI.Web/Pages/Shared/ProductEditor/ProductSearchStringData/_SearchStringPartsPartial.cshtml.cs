using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductEditor.ProductSearchStringData;
public class SearchStringPartsPartialModel
{
    public required ModalData ModalData { get; init; }
    public required int ProductId { get; init; }
    public string? ProductName { get; init; }
    public List<SearchStringPartOriginData>? SearchStringParts { get; init; }
}