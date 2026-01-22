using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DataAccess.Documents.Models;
public sealed class InvoiceDownloadStatus
{
    public int Id { get; init; }
    public int? ExportId { get; init; }
    public int? InvoiceId { get; init; }
    public string? ImportedStatus { get; init; }
    public DateTime? Date { get; init; }
    public string? UserName { get; init; }
}