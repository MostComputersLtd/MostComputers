using FluentValidation;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductIdentifiers.ProductSerialNumber;

using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductSerialNumberConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductIdentifiers.ProductSerialNumber;
internal sealed class ProductSerialNumberCreateRequestValidator : AbstractValidator<ProductSerialNumberCreateRequest>
{
    public ProductSerialNumberCreateRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(SerialNumberMaxLength);
    }
}