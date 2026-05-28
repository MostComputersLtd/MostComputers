namespace MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;

internal interface IGroupPromotionProductBindingsService
{
    Task<List<int>> GetAllProductIdsBoundToPromotionAsync(int promotionId);
}