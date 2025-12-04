using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
public sealed class ManufacturerToPromotionGroupRelationUpsertRequest
{
    public int ManufacturerId { get; set; }
    public int PromotionGroupId { get; set; }
}