using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;

internal sealed class GroupPromotionProductBindingsService : IGroupPromotionProductBindingsService
{
    private readonly IGroupPromotionContentsToProductsRepository _groupPromotionContentsToProductsRepository;

    public GroupPromotionProductBindingsService(IGroupPromotionContentsToProductsRepository groupPromotionContentsToProductsRepository)
    {
        _groupPromotionContentsToProductsRepository = groupPromotionContentsToProductsRepository;
    }

    public Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId)
    {
        return _groupPromotionContentsToProductsRepository.GetAllProductIdsBoundToPromotionAsync(promotionId);
    }
}