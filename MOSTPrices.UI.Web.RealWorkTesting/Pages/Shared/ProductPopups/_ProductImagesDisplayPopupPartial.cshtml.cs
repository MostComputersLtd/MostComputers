using MOSTComputers.UI.Web.RealWorkTesting.Models.Product;
using static MOSTComputers.UI.Web.RealWorkTesting.Utils.MappingUtils.ProductToDisplayDataMappingUtils;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public class ProductImagesDisplayPopupPartialModel
{
    public ProductImagesDisplayPopupPartialModel(ProductDisplayData productDisplayData)
    {
        ProductData = productDisplayData;

        if (productDisplayData.ImagesAndImageFileInfos is null
            || productDisplayData.ImagesAndImageFileInfos.Count <= 0) return;

        productDisplayData.ImagesAndImageFileInfos = OrderImagesAndImageFileNameInfos(productDisplayData.ImagesAndImageFileInfos);
    }

    public ProductDisplayData ProductData { get; set; }
}