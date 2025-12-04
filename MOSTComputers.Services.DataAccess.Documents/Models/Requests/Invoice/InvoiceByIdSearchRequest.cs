using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.Invoice;

public sealed class InvoiceByIdSearchRequest
{
    public required int Id { get; set; }
    public required InvoiceByIdSearchOptions SearchOption { get; set; }
}