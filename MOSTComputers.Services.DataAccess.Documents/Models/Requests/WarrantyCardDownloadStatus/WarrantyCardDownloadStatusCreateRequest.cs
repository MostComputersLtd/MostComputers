using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCardDownloadStatus;
public sealed class WarrantyCardDownloadStatusCreateRequest
{
    public int? ExportId { get; init; }
    public int? OrderId { get; init; }
    public string? ImportedStatus { get; init; }
    public DateTime? Date { get; init; }
    public string? UserName { get; init; }
}