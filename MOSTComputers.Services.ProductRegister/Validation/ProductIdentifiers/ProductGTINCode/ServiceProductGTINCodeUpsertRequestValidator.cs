using FluentValidation;
using MOSTComputers.Models.Product.Models.ProductIdentifiers;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductIdentifiers.ProductGTINCode;

using static MOSTComputers.Services.ProductRegister.Validation.CommonValueConstraints.ProductGTINCodeConstraints;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductIdentifiers.ProductGTINCode;

internal sealed class ServiceProductGTINCodeUpsertRequestValidator : AbstractValidator<ServiceProductGTINCodeUpsertRequest>
{
    public ServiceProductGTINCodeUpsertRequestValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.CodeType).NotNull();
        RuleFor(x => x.CodeTypeAsText).NotEmpty().MaximumLength(CodeTypeAsTextMaxLength);

        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(ValueMaxLength)
            .ValidGTINCodeForType(x => x.CodeType);

        RuleFor(x => x.UpsertUserName).NotEmpty().MaximumLength(Math.Min(CreateUserNameMaxLength, LastUpdateUserNameMaxLength));
    }
}