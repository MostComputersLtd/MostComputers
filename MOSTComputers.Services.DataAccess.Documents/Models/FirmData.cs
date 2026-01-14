using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DataAccess.Documents.Models;
public sealed class FirmData
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Order { get; set; }
    public string? Info { get; set; }
    public int? InvoiceNumber { get; set; }
    public string? Address { get; set; }
    public string? MPerson { get; set; }
    public string? TaxNumber { get; set; }
    public string? Bulstat { get; set; }
}