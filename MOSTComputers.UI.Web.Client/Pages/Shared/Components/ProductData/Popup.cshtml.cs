using MOSTComputers.Models.Product.Models;
using static MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData.ProductDataViewComponent;

namespace MOSTComputers.UI.Web.Client.Pages.Shared.Components.ProductData;

public sealed class PopupModel
{
    public required string DialogId { get; set; }
    public ProductDataExistingData? ExistingData { get; set; }
}