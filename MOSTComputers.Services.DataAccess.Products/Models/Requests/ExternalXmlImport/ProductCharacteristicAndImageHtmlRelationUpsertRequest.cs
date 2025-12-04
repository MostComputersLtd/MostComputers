using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DataAccess.Products.Models.Requests.ExternalXmlImport;
public sealed class ProductCharacteristicAndImageHtmlRelationUpsertRequest
{
    public required int CategoryId { get; set; }
    public int? ProductCharacteristicId { get; set; }
    public required string HtmlName { get; set; }
}