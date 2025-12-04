using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.Product.Models.ProductIdentifiers;

public sealed class ProductGTINCode
{
    public required int ProductId { get; init; }
    public required ProductGTINCodeType CodeType { get; init; }
    public required string CodeTypeAsText { get; init; }
    public required string Value { get; init; }

    public required string CreateUserName { get; init; }
    public required DateTime CreateDate { get; init; }
    public required string LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
}