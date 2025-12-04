using FluentValidation;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage.FileRelated;

using static MOSTComputers.Services.ProductRegister.Validation.CommonElements;

namespace MOSTComputers.Services.ProductRegister.Validation.ProductImage.FileRelated;

internal sealed class FileForImageUpsertRequestValidator : AbstractValidator<FileForImageUpsertRequest>
{
    public FileForImageUpsertRequestValidator()
    {
        RuleFor(x => x.CustomDisplayOrder).NullOrGreaterThan(0);
        RuleFor(x => x.UpsertUserName).NotNullOrWhiteSpace();
    }
}