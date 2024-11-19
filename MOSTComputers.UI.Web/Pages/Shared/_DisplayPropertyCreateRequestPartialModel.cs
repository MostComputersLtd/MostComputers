using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOSTComputers.UI.Web.Models;

namespace MOSTComputers.UI.Web.Pages.Shared;

public class DisplayPropertyCreateRequestPartialModel : PageModel
{
    public List<DisplayPropertyCreateRequest> DisplayProperties { get; init; }
    public IEnumerable<SelectListItem>? DisplayProductCharacteristics { get; init; }
    public int DisplayIndex { get; init; }

    public DisplayPropertyCreateRequestPartialModel(
        List<DisplayPropertyCreateRequest> displayProperties,
        IEnumerable<SelectListItem>? displayProductCharacteristics,
        int displayIndex)
    {
        DisplayProperties = displayProperties;
        DisplayProductCharacteristics = displayProductCharacteristics;
        DisplayIndex = displayIndex;
    }

    public void OnGet()
    {
    }
}
