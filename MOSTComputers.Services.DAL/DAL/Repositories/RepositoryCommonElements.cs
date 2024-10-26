using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using System.Text;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal static class RepositoryCommonElements
{
    internal static string GetDelimeteredListFromIds(List<int> ids)
    {
        StringBuilder sb = new();

        for (int i = 0; i < ids.Count - 1; i++)
        {
            int id = ids[i];
            sb.Append(id);
            sb.Append(", ");
        }

        sb.Append(ids[^1]);

        return sb.ToString();
    }

    internal static ValidationResult GetValidationResultFromFailedInsertWithCharacteristicIdResult(int result)
    {
        ValidationResult output = new();
        AddValidationErrorsForInsertWithCharacteristicIdResult(result, output);

        return output;
    }

    internal static ValidationResult GetValidationResultFromFailedInsertWithCharacteristicIdResult(int result, int characteristicId)
    {
        ValidationResult output = new();
        AddValidationErrorsForInsertWithCharacteristicIdResult(result, output, characteristicId);

        return output;
    }

    internal static void AddValidationErrorsForInsertWithCharacteristicIdResult(int result, ValidationResult output)
    {
        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId),
                "Id does not correspond to any known characteristic"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId),
                "There is already a property in the product for that characteristic"));
        }
        else if (result == -3)
        {
            output.Errors.Add(new(nameof(ProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId),
               "The characteristic of this property is of a different category than the product"));
        }
    }

    internal static void AddValidationErrorsForInsertWithCharacteristicIdResult(int result, ValidationResult output, int characteristicId)
    {
        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId),
                $"Id {characteristicId} does not correspond to any known characteristic"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId),
                $"There is already a property in the product with characteristic id {characteristicId}"));
        }
    }
}