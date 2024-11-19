using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.ProductStatuses;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductStatusesRepository : RepositoryBase, IProductStatusesRepository
{
    public ProductStatusesRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<ProductStatuses> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {ProductStatusesTableName}
            """;

        return _relationalDataAccess.GetData<ProductStatuses, dynamic>(getAllQuery, new { });
    }

    public ProductStatuses? GetByProductId(int productId)
    {
        const string getByProductIdQuery =
            $"""
            SELECT TOP 1 * FROM {ProductStatusesTableName}
            WHERE CSTID = @productId
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductStatuses, dynamic>(getByProductIdQuery, new { productId = productId });
    }

    public IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds)
    {
        const string getByProductIdQuery =
            $"""
            SELECT * FROM {ProductStatusesTableName}
            WHERE CSTID IN @productIds
            """;

        return _relationalDataAccess.GetData<ProductStatuses, dynamic>(getByProductIdQuery, new { productIds = productIds });
    }

    public OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @StatusCode INT = 0;

            IF EXISTS (
                SELECT 1 FROM {ProductStatusesTableName}
                WHERE CSTID = @productId
            ) SET @StatusCode = 1;

            IF NOT EXISTS (
                SELECT 1 FROM {ProductsTableName}
                WHERE CSTID = @productId
            ) SET @StatusCode = 2;

            IF @StatusCode = 0
            BEGIN
                INSERT INTO {ProductStatusesTableName}(CSTID, IsProcessed, NeedsToBeUpdated)
                VALUES(@productId, @IsProcessed, @NeedsToBeUpdated)
            END

            SELECT @StatusCode;
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            IsProcessed = createRequest.IsProcessed,
            NeedsToBeUpdated = createRequest.NeedsToBeUpdated,
        };

        int data = _relationalDataAccess.SaveDataAndReturnValue<int, dynamic>(insertQuery, parameters);

        if (data == 0) return new Success();

        ValidationResult validationResult = new();

        if (data == 1)
        {
            validationResult.Errors.Add(new(nameof(ProductStatuses.ProductId), "ProductId is invalid, because there is a duplicate of it in the records"));
        }

        else if (data == 2)
        {
            validationResult.Errors.Add(new(nameof(ProductStatuses.ProductId), "ProductId is invalid, because there is no product with this id"));
        }

        return validationResult;
    }

    public bool Update(ProductStatusesUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {ProductStatusesTableName}
            SET IsProcessed = @IsProcessed,
                NeedsToBeUpdated = @NeedsToBeUpdated

            WHERE CSTID = @productId
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            IsProcessed = updateRequest.IsProcessed,
            NeedsToBeUpdated = updateRequest.NeedsToBeUpdated,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteByProductId(int productId)
    {
        const string deleteByProductId =
            $"""
            DELETE FROM {ProductStatusesTableName}
            WHERE CSTID = @productId
            """;

        var parameters = new
        {
            productId = productId,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByProductId, parameters);

        return (rowsAffected > 0);
    }

#pragma warning restore IDE0037 // Use inferred member name
}