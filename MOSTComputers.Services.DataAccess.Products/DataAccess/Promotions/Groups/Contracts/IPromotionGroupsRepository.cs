using MOSTComputers.Models.Product.Models.Promotions.Groups;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Promotions.Groups.Contracts;
public interface IPromotionGroupsRepository
{
    Task<List<PromotionGroup>> GetAllAsync();
    Task<PromotionGroup?> GetByIdAsync(int id);
}