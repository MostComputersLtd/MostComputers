using Dapper.FluentMap.Mapping;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;

namespace MOSTComputers.Services.DAL.Mapping.ExternalXmlImport;

internal sealed class XmlImportProductFirstImageEntityMap : EntityMap<XmlImportProductFirstImage>
{
    public XmlImportProductFirstImageEntityMap()
    {
        Map(x => x.Id).ToColumn("ImageProductId");
        Map(x => x.HtmlData).ToColumn("HtmlData");
        Map(x => x.ImageData).ToColumn("Image");
        Map(x => x.ImageContentType).ToColumn("ImageFileExt");
        Map(x => x.DateModified).ToColumn("DateModified");
    }
}