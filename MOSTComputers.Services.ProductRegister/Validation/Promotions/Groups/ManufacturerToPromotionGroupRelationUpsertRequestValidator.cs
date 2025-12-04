using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.Promotions.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Validation.Promotions.Groups;
internal sealed class ManufacturerToPromotionGroupRelationUpsertRequestValidator : AbstractValidator<ManufacturerToPromotionGroupRelationUpsertRequest>
{
    public ManufacturerToPromotionGroupRelationUpsertRequestValidator()
    {
        RuleFor(x => x.ManufacturerId).GreaterThan(0);
        RuleFor(x => x.PromotionGroupId).GreaterThan(0);
    }
}