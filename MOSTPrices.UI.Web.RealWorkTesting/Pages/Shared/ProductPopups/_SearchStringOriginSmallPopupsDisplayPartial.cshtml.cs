using MOSTComputers.Services.SearchStringOrigin.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class SearchStringOriginSmallPopupsDisplayPartialModel
{
    public SearchStringOriginSmallPopupsDisplayPartialModel(
        int productId,
        IEnumerable<string> searchStringDataTotalCopyIds,
        List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringPartOriginDataList,
        bool makeSearchStringSearchItemVisibleAfterAddingData = false,
        string elementIdAndNamePrefix = "")
    {
        ProductId = productId;
        SearchStringDataTotalCopyIds = searchStringDataTotalCopyIds;
        SearchStringPartOriginDataList = searchStringPartOriginDataList;
        MakeSearchStringSearchItemVisibleAfterAddingData = makeSearchStringSearchItemVisibleAfterAddingData;
        ElementIdAndNamePrefix = elementIdAndNamePrefix;
    }

    public int ProductId { get; }
    public IEnumerable<string> SearchStringDataTotalCopyIds { get; }
    public IReadOnlyList<Tuple<string, List<SearchStringPartOriginData>?>>? SearchStringPartOriginDataList { get; }
    public bool MakeSearchStringSearchItemVisibleAfterAddingData { get; }
    public string ElementIdAndNamePrefix { get; }
}