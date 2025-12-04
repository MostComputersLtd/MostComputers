using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DataAccess.Documents.Models.Requests.WarrantyCard;

public sealed class WarrantyCardSearchRequest
{
    public List<WarrantyCardByIdSearchRequest>? WarrantyCardByIdSearchRequests { get; set; }
    public List<WarrantyCardByStringSearchRequest>? WarrantyCardByStringSearchRequests { get; set; }
    public List<WarrantyCardByDateFilterRequest>? WarrantyCardByDateFilterRequests { get; set; }
}