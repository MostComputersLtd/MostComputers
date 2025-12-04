using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.UI.Web.Models.PromotionFiles;

namespace MOSTComputers.UI.Web.Services.Data.Contracts;
public interface IPromotionFileEditorDataService
{
    Task<List<PromotionFileInfoEditorDisplayData>> GetAllFilesAsync();
    Task<PromotionFileInfoEditorDisplayData?> GetFileAsync(int promotionFileId);
    List<PromotionFileInfoEditorDisplayData> GetEditorDisplayData(List<PromotionFileInfo> promotionFileInfos);
    PromotionFileInfoEditorDisplayData GetEditorDisplayData(PromotionFileInfo promotionFileInfo);
    Task<List<PromotionFileInfoEditorDisplayData>> GetSearchedFilesAsync(PromotionFilesSearchOptions promotionFilesSearchOptions);
    Task<List<PromotionFileInfoEditorDisplayData>> GetFilesAsync(IEnumerable<int> promotionFileIds);
}