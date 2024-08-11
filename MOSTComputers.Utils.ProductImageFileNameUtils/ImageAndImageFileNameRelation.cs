using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Utils.ProductImageFileNameUtils;
public sealed class ImageAndImageFileNameRelation
{
    public ImageAndImageFileNameRelation(ProductImage? productImage, ProductImageFileNameInfo? productImageFileNameInfo)
    {
        ProductImage = productImage;
        ProductImageFileNameInfo = productImageFileNameInfo;
    }

    public ProductImage? ProductImage { get; }
    public ProductImageFileNameInfo? ProductImageFileNameInfo { get; }
}