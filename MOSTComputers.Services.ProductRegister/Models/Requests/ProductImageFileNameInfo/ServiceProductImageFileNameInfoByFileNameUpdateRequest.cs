using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImageFileNameInfo;

public sealed class ServiceProductImageFileNameInfoByFileNameUpdateRequest
{
    public int ProductId { get; set; }
    public string FileName { get; set; }
    public string? NewFileName { get; set; }
    public int? NewDisplayOrder { get; set; }
    public bool? Active { get; set; }
}