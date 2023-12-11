using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductStatuses;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductStatusesRepository : RepositoryBase, IProductStatusesRepository
{
    const string _tableName = "dbo.ProductStatuses";
    const string _productTableName = "dbo.MOSTPrices";


    public ProductStatusesRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<ProductStatuses> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {_tableName}
            """;

        return _relationalDataAccess.GetData<ProductStatuses, dynamic>(getAllQuery, new { });
    }

    public ProductStatuses? GetByProductId(uint productId)
    {
        const string getByProductIdQuery =
            $"""
            SELECT TOP 1 * FROM {_tableName}
            WHERE CSTID = @productId
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductStatuses, dynamic>(getByProductIdQuery, new { productId = (int)productId });
    }

    public IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<uint> productIds)
    {
        const string getByProductIdQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID IN @productIds
            """;

        return _relationalDataAccess.GetData<ProductStatuses, dynamic>(getByProductIdQuery, new { productIds = productIds.Select(productId => (int)productId) });
    }

    public OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            IF EXISTS (
                SELECT 1 FROM {_tableName}
                WHERE CSTID = @productId
            ) RETURN 1;

            IF NOT EXISTS (
                SELECT 1 FROM {_productTableName}
                WHERE CSTID = @productId
            ) RETURN 2;

            INSERT INTO {_tableName}(CSTID, IsProcessed, NeedsToBeUpdated)
            VALUES(@productId, @IsProcessed, @NeedsToBeUpdated)

            RETURN 0;
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.IsProcessed,
            createRequest.NeedsToBeUpdated,
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
            UPDATE {_tableName}
            SET IsProcessed = @IsProcessed,
                NeedsToBeUpdated = @NeedsToBeUpdated

            WHERE CSTID = @productId
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.IsProcessed,
            updateRequest.NeedsToBeUpdated,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductStatuses, dynamic>(updateQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteByProductId(uint productId)
    {
        const string deleteByProductId =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId
            """;

        var parameters = new
        {
            productId = (int)productId,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductStatuses, dynamic>(deleteByProductId, parameters);

        return (rowsAffected > 0);
    }
}