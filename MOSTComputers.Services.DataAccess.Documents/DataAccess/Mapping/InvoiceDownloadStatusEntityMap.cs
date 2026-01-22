using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.InvoiceDownloadStatusesTable;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;

internal sealed class InvoiceDownloadStatusEntityMap : EntityMap<InvoiceDownloadStatus>
{
    public InvoiceDownloadStatusEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumn);
        Map(x => x.ExportId).ToColumn(ExportIdColumn);
        Map(x => x.InvoiceId).ToColumn(InvoiceIdColumn);
        Map(x => x.ImportedStatus).ToColumn(ImportedStatusColumn);
        Map(x => x.Date).ToColumn(DateColumn);
        Map(x => x.UserName).ToColumn(UserNameColumn);
    }
}