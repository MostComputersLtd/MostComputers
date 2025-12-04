using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.UI.Web.Pages.Shared.ProductSearchString;

public sealed class SearchStringPartsTablePartialModel
{
    public required List<SearchStringPartOriginData> SearchStringParts { get; init; }
}