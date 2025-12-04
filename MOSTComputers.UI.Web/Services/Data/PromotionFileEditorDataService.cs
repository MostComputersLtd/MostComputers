using MOSTComputers.Models.Product.Models.Promotions.Files;
using MOSTComputers.Services.ProductRegister.Services.Promotions.PromotionFiles.Contracts;
using MOSTComputers.UI.Web.Models.PromotionFiles;
using MOSTComputers.UI.Web.Services.Data.Contracts;

namespace MOSTComputers.UI.Web.Services.Data;
internal sealed class PromotionFileEditorDataService : IPromotionFileEditorDataService
{
    public PromotionFileEditorDataService(IPromotionFileService promotionFileInfoService)
    {
        _promotionFileInfoService = promotionFileInfoService;
    }

    private readonly IPromotionFileService _promotionFileInfoService;

    public async Task<List<PromotionFileInfoEditorDisplayData>> GetAllFilesAsync()
    {
        List<PromotionFileInfo> promotionFileInfos = await _promotionFileInfoService.GetAllAsync();

        return GetEditorDisplayData(promotionFileInfos);
    }

    public async Task<List<PromotionFileInfoEditorDisplayData>> GetSearchedFilesAsync(PromotionFilesSearchOptions promotionFilesSearchOptions)
    {
        List<PromotionFileInfo> promotionFileInfos = await _promotionFileInfoService.GetAllAsync();

        List<PromotionFileInfoEditorDisplayData> promotionFileEditorData = GetEditorDisplayData(promotionFileInfos);

        return GetSearchedPromotionFileData(promotionFilesSearchOptions, promotionFileEditorData);
    }

    public async Task<List<PromotionFileInfoEditorDisplayData>> GetFilesAsync(IEnumerable<int> promotionFileIds)
    {
        List<PromotionFileInfo>? promotionFileInfos = await _promotionFileInfoService.GetByIdsAsync(promotionFileIds);

        return GetEditorDisplayData(promotionFileInfos);
    }

    public async Task<PromotionFileInfoEditorDisplayData?> GetFileAsync(int promotionFileId)
    {
        PromotionFileInfo? promotionFileInfo = await _promotionFileInfoService.GetByIdAsync(promotionFileId);

        if (promotionFileInfo is null) return null;

        return GetEditorDisplayData(promotionFileInfo);
    }

    public List<PromotionFileInfoEditorDisplayData> GetEditorDisplayData(List<PromotionFileInfo> promotionFileInfos)
    {
        List<PromotionFileInfoEditorDisplayData> output = new();

        foreach (PromotionFileInfo promotionFileInfo in promotionFileInfos)
        {
            PromotionFileInfoEditorDisplayData promotionFileEditorData
                = MapEditorDisplayData(promotionFileInfo);

            output.Add(promotionFileEditorData);
        }

        return output;
    }

    public PromotionFileInfoEditorDisplayData GetEditorDisplayData(PromotionFileInfo promotionFileInfo)
    {
        return MapEditorDisplayData(promotionFileInfo);
    }

    private static List<PromotionFileInfoEditorDisplayData> GetSearchedPromotionFileData(
        PromotionFilesSearchOptions searchOptions,
        List<PromotionFileInfoEditorDisplayData> allPromotionFileInfos)
    {
        List<PromotionFileInfoEditorDisplayData> output = new();

        string? userInputString = searchOptions.UserInputString?.Trim();

        bool parseUserInputSuccess = int.TryParse(userInputString, out int searchedId);

        foreach (PromotionFileInfoEditorDisplayData promotionFileInfo in allPromotionFileInfos)
        {
            if (searchOptions.ValidOnDate is not null)
            {
                if (promotionFileInfo.ValidFrom is not null
                    && searchOptions.ValidOnDate < DateOnly.FromDateTime(promotionFileInfo.ValidFrom.Value))
                {
                    continue;
                }

                if (promotionFileInfo.ValidTo is not null
                    && searchOptions.ValidOnDate > DateOnly.FromDateTime(promotionFileInfo.ValidTo.Value))
                {
                    continue;
                }
            }

            if (string.IsNullOrWhiteSpace(userInputString))
            {
                output.Add(promotionFileInfo);

                continue;
            }

            if (promotionFileInfo.Name is not null
                && promotionFileInfo.Name.Contains(userInputString))
            {
                output.Add(promotionFileInfo);

                continue;
            }

            if (promotionFileInfo.FileName is not null
                && promotionFileInfo.FileName.Contains(userInputString))
            {
                output.Add(promotionFileInfo);

                continue;
            }

            if (parseUserInputSuccess && promotionFileInfo.Id == searchedId)
            {
                output.Add(promotionFileInfo);
            }
        }

        return output;
    }

    private static PromotionFileInfoEditorDisplayData MapEditorDisplayData(PromotionFileInfo promotionFileInfo)
    {
        return new PromotionFileInfoEditorDisplayData()
        {
            Id = promotionFileInfo.Id,
            CreateUserName = promotionFileInfo.CreateUserName,
            CreateDate = promotionFileInfo.CreateDate,
            LastUpdateUserName = promotionFileInfo.LastUpdateUserName,
            LastUpdateDate = promotionFileInfo.LastUpdateDate,
            Name = promotionFileInfo.Name,
            Active = promotionFileInfo.Active,
            ValidFrom = promotionFileInfo.ValidFrom,
            ValidTo = promotionFileInfo.ValidTo,
            FileName = promotionFileInfo.FileName,
            Description = promotionFileInfo.Description,
            RelatedProductsString = promotionFileInfo.RelatedProductsString,
        };
    }
}