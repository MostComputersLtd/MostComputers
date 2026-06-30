using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.DataAccess.Products.DataAccess.ProductImages.Contracts;

using static MOSTComputers.Services.ProductRegister.Utils.SearchByIdsUtils;

namespace MOSTComputers.Services.ProductRegister.Services.ProductImages;

public sealed class ProductImageFileReadService : IProductImageFileReadService
{
    public ProductImageFileReadService(IProductImageFileDataRepository imageFileNameInfoRepository)
    {
        _imageFileNameInfoRepository = imageFileNameInfoRepository;
    }

    private readonly IProductImageFileDataRepository _imageFileNameInfoRepository;

    public async Task<List<ProductImageFileData>> GetAllAsync()
    {
        return await _imageFileNameInfoRepository.GetAllAsync();
    }
    
    public async Task<List<IGrouping<int, ProductImageFileData>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        productIds = RemoveValuesSmallerThanOne(productIds);

        return await _imageFileNameInfoRepository.GetAllInProductsAsync(productIds);
    }

    public async Task<List<ProductImageFileData>> GetAllInProductAsync(int productId)
    {
        if (productId <= 0) return new();

        return await _imageFileNameInfoRepository.GetAllInProductAsync(productId);
    }

    public async Task<ProductImageFileData?> GetByIdAsync(int id)
    {
        if (id <= 0) return null;

        return await _imageFileNameInfoRepository.GetByIdAsync(id);
    }

    public async Task<ProductImageFileData?> GetByProductIdAndImageIdAsync(int productId, int imageId)
    {
        if (productId <= 0 || imageId <= 0) return null;

        return await _imageFileNameInfoRepository.GetByProductIdAndImageIdAsync(productId, imageId);
    }

}
