using MOSTComputers.Services.SearchStringOrigin.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class SearchStringOriginSmallPopupsDisplayPartialModel
{
    public SearchStringOriginSmallPopupsDisplayPartialModel(
        ModalData modalData,
        int productId,
        IEnumerable<string> searchStringDataTotalCopyIds,
        List<Tuple<string, List<SearchStringPartOriginData>?>>? searchStringPartOriginDataList,
        bool makeSearchStringSearchItemVisibleAfterAddingData = false,
        string elementIdAndNamePrefix = "")
    {
        ModalData = modalData;
        ProductId = productId;
        SearchStringDataTotalCopyIds = searchStringDataTotalCopyIds;
        SearchStringPartOriginDataList = searchStringPartOriginDataList;
        MakeSearchStringSearchItemVisibleAfterAddingData = makeSearchStringSearchItemVisibleAfterAddingData;
        ElementIdAndNamePrefix = elementIdAndNamePrefix;
    }

    public ModalData ModalData { get; }
    public int ProductId { get; }
    public IEnumerable<string> SearchStringDataTotalCopyIds { get; }
    public IReadOnlyList<Tuple<string, List<SearchStringPartOriginData>?>>? SearchStringPartOriginDataList { get; }
    public bool MakeSearchStringSearchItemVisibleAfterAddingData { get; }
    public string ElementIdAndNamePrefix { get; }
}