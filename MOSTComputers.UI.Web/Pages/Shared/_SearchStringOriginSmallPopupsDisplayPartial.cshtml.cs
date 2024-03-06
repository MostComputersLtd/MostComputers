using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.UI.Web.Pages.Shared;

public class SearchStringOriginSmallPopupsDisplayPartialModel
{
    public SearchStringOriginSmallPopupsDisplayPartialModel(
        int productId,
        string searchStringDataTotalCopyId,
        List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringPartOriginDataList,
        bool makeSearchStringSearchItemVisibleAfterAddingData = false)
    {
        ProductId = productId;
        SearchStringDataTotalCopyId = searchStringDataTotalCopyId;
        SearchStringPartOriginDataList = searchStringPartOriginDataList;
        MakeSearchStringSearchItemVisibleAfterAddingData = makeSearchStringSearchItemVisibleAfterAddingData;
    }

    public int ProductId { get; }
    public string SearchStringDataTotalCopyId { get; }
    public IReadOnlyList<Tuple<string, List<SearchStringPartOriginData>?>>? SearchStringPartOriginDataList { get; }
    public bool MakeSearchStringSearchItemVisibleAfterAddingData { get; }
}