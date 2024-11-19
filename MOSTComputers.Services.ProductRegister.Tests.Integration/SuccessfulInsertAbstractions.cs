using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Models.Requests.ToDoLocalChanges;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using OneOf;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;
internal static class SuccessfulInsertAbstractions
{
    public static int InsertProductAndGetIdOrThrow(IProductService productService, ProductCreateRequest productCreateRequest)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> insertProductResult = productService.Insert(productCreateRequest);

        int productId = -1;

        bool isProductInsertSuccessful = insertProductResult.Match(
            id =>
            {
                productId = id;

                return true;
            },
            validationResult => false,
            unexpectedFailureResult => false);

        Assert.True(isProductInsertSuccessful);
        Assert.True(productId > 0);

        return productId;
    }

    public static int? InsertToDoLocalChangeAndGetIdOrNull(
        IToDoLocalChangesService toDoLocalChangesService,
        ServiceToDoLocalChangeCreateRequest toDoLocalChangeCreateRequest)
    {
        OneOf<int, ValidationResult, UnexpectedFailureResult> toDoChangesInsertResult = toDoLocalChangesService.Insert(toDoLocalChangeCreateRequest);

        int? toDoChangesId1 = toDoChangesInsertResult.Match<int?>(
            id => id,
            validationResult => null,
            unexpectedFailureResult => null);

        return toDoChangesId1;
    }
}