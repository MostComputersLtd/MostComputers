using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;

namespace MOSTComputers.Services.DAL.Mapping.ExternalXmlImport;

internal sealed class XmlImportProductImageFileNameInfoEntityMap : EntityMap<XmlImportProductImageFileNameInfo>
{
    public XmlImportProductImageFileNameInfoEntityMap()
    {
        Map(x => x.ProductId).ToColumn("CSTID");
        Map(x => x.DisplayOrder).ToColumn("S");
        Map(x => x.ImageNumber).ToColumn("ImageNumber");
        Map(x => x.FileName).ToColumn("ImgFileName");
        Map(x => x.Active).ToColumn("Active");
        Map(x => x.ImagesInImagesAllForProductCount).ToColumn("ImagesAllForProductCount");
        Map(x => x.IsProductFirstImageInImages).ToColumn("IsProductFirstImageInImages");
    }
}