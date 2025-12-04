using MOSTComputers.Models.Product.Models.Promotions;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Contracts;
using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;

namespace MOSTComputers.Services.ProductRegister.Services.Promotions;
internal sealed class PromotionService : IPromotionService
{
    public PromotionService(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    private readonly IPromotionRepository _promotionRepository;

    public async Task<List<Promotion>> GetAllAsync()
    {
        return await _promotionRepository.GetAllAsync();
    }

    public async Task<List<Promotion>> GetAllActiveAsync()
    {
        return await _promotionRepository.GetAllActiveAsync();
    }

    public async Task<List<IGrouping<int?, Promotion>>> GetAllActiveForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds.Distinct());

        return await _promotionRepository.GetAllActiveForSelectionOfProductsAsync(productIds);
    }

    public async Task<List<Promotion>> GetAllForProductAsync(int productId)
    {
        return await _promotionRepository.GetAllForProductAsync(productId);
    }

    public async Task<List<IGrouping<int?, Promotion>>> GetAllForSelectionOfProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds.Distinct());

        return await _promotionRepository.GetAllForSelectionOfProductsAsync(productIds);
    }

    public async Task<List<Promotion>> GetAllActiveForProductAsync(int productId)
    {
        return await _promotionRepository.GetAllActiveForProductAsync(productId);
    }
}