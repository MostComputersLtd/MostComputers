using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.Manifacturer;

internal sealed class ManifacturerCreateRequestValidator : AbstractValidator<ManifacturerCreateRequest>
{
    public ManifacturerCreateRequestValidator()
    {
        RuleFor(x => x.DisplayOrder).Must(NullOrGreaterThanZero);
        RuleFor(x => x.RealCompanyName).Must(IsNotEmptyOrWhiteSpace);
        RuleFor(x => x.BGName).Must(IsNotEmptyOrWhiteSpace);
    }
}