using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.Models.Product.Models;
using static MOSTComputers.UI.Web.Pages.ProductCompareEditorModel;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductCompareEditor.ProductProperties;

public class ProductPropertyInCompareEditorWithoutCharacteristicPartialModel
{
    public required ProductProperty ProductProperty { get; set; }
    public required IEnumerable<SelectListItem> ProductCharacteristicsForSelect { get; set; }
    public required uint PropertyIndex { get; set; }
    public required string ElementIdAndNamePrefix { get; set; }


    private int _productFirstOrSecond;
    public required FirstOrSecondProductEnum ProductFirstOrSecondEnum { init => _productFirstOrSecond = (int)value; }
    public int ProductFirstOrSecond => _productFirstOrSecond;
}