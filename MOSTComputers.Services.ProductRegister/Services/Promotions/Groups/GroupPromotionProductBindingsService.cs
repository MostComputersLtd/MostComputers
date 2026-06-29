using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups;

internal sealed class GroupPromotionProductBindingsService : IGroupPromotionProductBindingsService
{
    private readonly IGroupPromotionContentsToProductsRepository _groupPromotionContentsToProductsRepository;

    public GroupPromotionProductBindingsService(IGroupPromotionContentsToProductsRepository groupPromotionContentsToProductsRepository)
    {
        _groupPromotionContentsToProductsRepository = groupPromotionContentsToProductsRepository;
    }

    public Task<Dictionary<int, List<int>>> GetAllAsync()
    {
        return _groupPromotionContentsToProductsRepository.GetAllAsync();
    }

    public Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId)
    {
        return _groupPromotionContentsToProductsRepository.GetAllProductIdsBoundToPromotionAsync(promotionId);
    }

    public Task<Dictionary<int, List<int>>> GetAllPromotionIdsBoundToProductsAsync(IEnumerable<int> productIds)
    {
        if (!productIds.Any()) return Task.FromResult(new Dictionary<int, List<int>>());

        return _groupPromotionContentsToProductsRepository.GetAllPromotionIdsBoundToProductsAsync(productIds);
    }

    public Task<List<int>> GetAllPromotionIdsBoundToProductAsync(int productId)
    {
        return _groupPromotionContentsToProductsRepository.GetAllPromotionIdsBoundToProductAsync(productId);
    }

    public async Task<OneOf<Success, NotFound>> UpsertAllAsync(int promotionId, List<int>? relatedProductIds)
    {
        if (promotionId <= 0) return new NotFound();

        await _groupPromotionContentsToProductsRepository.UpsertAllAsync(promotionId, relatedProductIds);

        return new Success();
    }
}