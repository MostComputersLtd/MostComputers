using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductProperties;

public class ProductPropertyWithoutCharacteristicDisplayForPropertyEditorPartialModel
{
    public required ProductProperty ProductProperty { get; set; }
    public required IEnumerable<SelectListItem> ProductCharacteristicsForSelect { get; set; }
    public required uint PropertyIndex { get; set; }
}
