using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.XmlDownloads;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.XmlDownloadsTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess.Mapping;
internal sealed class XmlDownloadsEntityMap : EntityMap<XmlDownloadData>
{
    public XmlDownloadsEntityMap()
    {
        Map(x => x.XmlResourceType).ToColumn(XmlResourceTypeColumnName);
        Map(x => x.BID).ToColumn(BIDColumnName);
        Map(x => x.UserName).ToColumn(UserNameColumnName);
        Map(x => x.ContactPerson).ToColumn(ContactPersonColumnName);
        Map(x => x.TimeStamp).ToColumn(TimeStampColumnName);
    }
}