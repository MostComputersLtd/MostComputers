using FluentValidation;
using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;

namespace MOSTComputers.Services.LocalChangesHandling.Validation;

internal sealed class LocalChangeDataValidatorForGivenTableAndOperationType : AbstractValidator<LocalChangeData>
{
    public LocalChangeDataValidatorForGivenTableAndOperationType(string tableName, ChangeOperationTypeEnum changeOperationType)
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TableName).Must(tableName => !string.IsNullOrWhiteSpace(tableName))
            .WithMessage("Argument must not be empty or whitespace")
            .Equal(tableName);

        RuleFor(x => x.OperationType).Equal(changeOperationType);
        RuleFor(x => x.TableElementId).GreaterThan(0);
        RuleFor(x => x.TimeStamp).GreaterThan(new DateTime(0));
    }
}