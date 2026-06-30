using MOSTComputers.Models.Product.Models.ProductImages;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;

public interface IProductImageFileReadService
{
    Task<List<ProductImageFileData>> GetAllAsync();
    Task<List<IGrouping<int, ProductImageFileData>>> GetAllInProductsAsync(IEnumerable<int> productIds);
    Task<List<ProductImageFileData>> GetAllInProductAsync(int productId);
    Task<ProductImageFileData?> GetByIdAsync(int id);
    Task<ProductImageFileData?> GetByProductIdAndImageIdAsync(int productId, int imageId);
}
