using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Services.Contracts;

namespace MOSTComputers.Services.ProductRegister.Services;
internal class ProductImageSaveService : IProductImageSaveService
{
    private readonly Dictionary<int, Dictionary<ProductImage, ProductImageFileNameInfo>> _productImageData = new();

    public bool AddImageToProduct(int productId, ProductImage image, ProductImageFileNameInfo productImageFileNameInfo)
    {
        Dictionary<ProductImage, ProductImageFileNameInfo>? productImageDictionary = _productImageData.GetValueOrDefault(productId);

        if (productImageDictionary is null)
        {
            _productImageData.Add(productId, new()
            {
                { image, productImageFileNameInfo }
            });

            return true;
        }

        productImageDictionary.Add(image, productImageFileNameInfo);

        return true;
    }

    public Dictionary<ProductImage, ProductImageFileNameInfo>? GetImagesForProduct(int productId)
    {
        return _productImageData.GetValueOrDefault(productId);
    }

    public bool AddOrUpdateImagesOfProduct(int productId, Dictionary<ProductImage, ProductImageFileNameInfo> productUpdatedImages)
    {
        Dictionary<ProductImage, ProductImageFileNameInfo>? productImageDictionary = _productImageData.GetValueOrDefault(productId);

        if (productImageDictionary is null)
        {
            _productImageData.Add(productId, productUpdatedImages);

            return true;
        }

        _productImageData[productId] = productUpdatedImages;

        return true;
    }
}