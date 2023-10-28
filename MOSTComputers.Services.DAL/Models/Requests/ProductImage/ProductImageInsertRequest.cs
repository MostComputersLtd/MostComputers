using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Models.Requests.ProductImage;

public class ProductImageCreateRequest
{
    public int? CSTID { get; set; }
    public string? XML { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageFileExtension { get; set; }
    public DateTime? DateModified { get; } = DateTime.Now;
}