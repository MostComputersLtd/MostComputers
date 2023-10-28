using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Models.Requests.ProductCharacteristic;

public class ProductCharacteristicByNameAndCategoryIdUpdateRequest
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string? NewName { get; set; }
    public string? Meaning { get; set; }
    public int? DisplayOrder { get; set; }
    public short? Active { get; set; }
    public short? PKUserId { get; set; }
    public DateTime? LastUpdate { get; set; }
    public short? KWPrCh { get; set; }
}