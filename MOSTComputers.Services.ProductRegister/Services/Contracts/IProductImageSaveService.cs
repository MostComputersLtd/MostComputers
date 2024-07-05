using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.ProductRegister.Services.Contracts;
public interface IProductImageSaveService
{
    bool AddImageToProduct(int productId, ProductImage image, ProductImageFileNameInfo productImageFileNameInfo);
    bool AddOrUpdateImagesOfProduct(int productId, Dictionary<ProductImage, ProductImageFileNameInfo> productUpdatedImages);
    Dictionary<ProductImage, ProductImageFileNameInfo>? GetImagesForProduct(int productId);
}