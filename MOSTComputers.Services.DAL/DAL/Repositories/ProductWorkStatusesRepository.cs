using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductWorkStatusesRepository : RepositoryBase, IProductWorkStatusesRepository
{
    private const string _tableName = "dbo.TodoProductWorkStatuses";
    private const string _productTableName = "dbo.MOSTPrices";

    public ProductWorkStatusesRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name
    public IEnumerable<ProductWorkStatuses> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT Id AS ProductWorkStatusId, CSTID AS ProductWorkStatusProductId, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus
            FROM {_tableName}
            """;

        return _relationalDataAccess.GetData<ProductWorkStatuses, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<ProductWorkStatuses> GetAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum)
    {
        const string getAllWithProductNewStatusQuery =
            $"""
            SELECT Id AS ProductWorkStatusId, CSTID AS ProductWorkStatusProductId, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus
            FROM {_tableName}
            WHERE ProductNewStatus = @productNewStatus
            """;

        return _relationalDataAccess.GetData<ProductWorkStatuses, dynamic>(getAllWithProductNewStatusQuery,
            new { productNewStatus = (int)productNewStatusEnum });
    }

    public IEnumerable<ProductWorkStatuses> GetAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum)
    {
        const string getAllWithProductXmlStatusQuery =
            $"""
            SELECT Id AS ProductWorkStatusId, CSTID AS ProductWorkStatusProductId, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus
            FROM {_tableName}
            WHERE ProductXmlReadyStatus = @productXmlStatus
            """;

        return _relationalDataAccess.GetData<ProductWorkStatuses, dynamic>(getAllWithProductXmlStatusQuery,
            new { productXmlStatus = (int)productXmlStatusEnum });
    }

    public IEnumerable<ProductWorkStatuses> GetAllWithReadyForImageInsert(bool readyForImageInsert)
    {
        const string getAllWithReadyForImageInsertQuery =
            $"""
            SELECT Id AS ProductWorkStatusId, CSTID AS ProductWorkStatusProductId, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus
            FROM {_tableName}
            WHERE ReadyForImageInsertStatus = @readyForImageInsert
            """;

        return _relationalDataAccess.GetData<ProductWorkStatuses, dynamic>(getAllWithReadyForImageInsertQuery,
            new { readyForImageInsert = readyForImageInsert });
    }

    public ProductWorkStatuses? GetById(int productWorkStatusesId)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 Id AS ProductWorkStatusId, CSTID AS ProductWorkStatusProductId, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus
            FROM {_tableName}
            WHERE Id = @productWorkStatusesId
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductWorkStatuses, dynamic>(getByIdQuery,
            new { productWorkStatusesId = productWorkStatusesId });
    }

    public ProductWorkStatuses? GetByProductId(int productId)
    {
        const string getAllByProductIdQuery =
            $"""
            SELECT TOP 1 Id AS ProductWorkStatusId, CSTID AS ProductWorkStatusProductId, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus
            FROM {_tableName}
            WHERE CSTID = @productId
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductWorkStatuses, dynamic>(getAllByProductIdQuery,
            new { productId = productId });
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertIfItDoesntExist(ProductWorkStatusesCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @Status INT = 0;
            DECLARE @InsertedIdTable TABLE (Id INT);

            IF EXISTS (
                SELECT 1 FROM {_tableName}
                WHERE CSTID = @productId
            ) SET @Status = -1;

            IF NOT EXISTS (
                SELECT 1 FROM {_productTableName}
                WHERE CSTID = @productId
            ) SET @Status = -2;

            IF @Status = 0
            BEGIN
                INSERT INTO {_tableName} (CSTID, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus)
                OUTPUT INSERTED.Id INTO @InsertedIdTable
                VALUES(@productId, @ProductNewStatus, @ProductXmlStatus, @ReadyForImageInsert)
            END

            SELECT ISNULL((SELECT TOP 1 Id FROM @InsertedIdTable), @Status);
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            ProductNewStatus = (int)createRequest.ProductNewStatus,
            ProductXmlStatus = (int)createRequest.ProductXmlStatus,
            ReadyForImageInsert = createRequest.ReadyForImageInsert,
        };

        int? result = _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition<dynamic, int?>(
            actionInTransaction: paramsLocal => InsertAndExtractData(insertQuery, paramsLocal),
            shouldCommit: data => data is not null && data > 0,
            parameters);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return result.Value;

        ValidationResult validationResult = GetValidationResultFromFailedInsertResult(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;

        int? InsertAndExtractData(string insertQuery, dynamic paramsLocal)
        {
            return _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertQuery, paramsLocal);
        }

        //(int, int) InsertAndExtractData(string insertQuery, dynamic paramsLocal)
        //{
        //    string? data = _relationalDataAccess.SaveDataAndReturnValue<string, dynamic>(insertQuery, paramsLocal);

        //    if (data is null) return (-1, -1);

        //    int indexOfSlash = data.IndexOf('/');

        //    if (indexOfSlash < 0)
        //    {
        //        bool parseStatusOnlySuccess = int.TryParse(data, out int statusCodeOnlyResult);

        //        return (-1, statusCodeOnlyResult);
        //    }

        //    string productWorkStatusIdAsString = data[..indexOfSlash];
        //    string statusCodeAsString = data[(indexOfSlash + 1)..];

        //    bool parseProductWorkStatusIdSuccess = int.TryParse(productWorkStatusIdAsString, out int productWorkStatusId);
        //    bool parseStatusCodeSuccess = int.TryParse(statusCodeAsString, out int statusCode);

        //    if (parseProductWorkStatusIdSuccess && parseStatusCodeSuccess)
        //    {
        //        return (productWorkStatusId, statusCode);
        //    }

        //    return (-1, -1);
        //}
    }

    private static ValidationResult GetValidationResultFromFailedInsertResult(int result)
    {
        ValidationResult validationResult = new();

        if (result == 1)
        {
            validationResult.Errors.Add(new(nameof(ProductStatuses.ProductId), "Product status already exists for this product"));
        }
        else if (result == 2)
        {
            validationResult.Errors.Add(new(nameof(ProductStatuses.ProductId), "ProductId is invalid"));
        }

        return validationResult;
    }

    public bool UpdateById(ProductWorkStatusesUpdateByIdRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {_tableName}
            SET ProductNewStatus = @ProductNewStatus,
                ProductXmlReadyStatus = @ProductXmlStatus,
                ReadyForImageInsertStatus = @ReadyForImageInsert
            
            WHERE Id = @id
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            ProductNewStatus = updateRequest.ProductNewStatus,
            ProductXmlStatus = updateRequest.ProductXmlStatus,
            ReadyForImageInsert = updateRequest.ReadyForImageInsert,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateQuery, parameters);

        return rowsAffected > 0;
    }

    public bool UpdateByProductId(ProductWorkStatusesUpdateByProductIdRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {_tableName}
            SET ProductNewStatus = @ProductNewStatus,
                ProductXmlReadyStatus = @ProductXmlStatus,
                ReadyForImageInsertStatus = @ReadyForImageInsert
            
            WHERE CSTID = @productId
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            ProductNewStatus = updateRequest.ProductNewStatus,
            ProductXmlStatus = updateRequest.ProductXmlStatus,
            ReadyForImageInsert = updateRequest.ReadyForImageInsert,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateQuery, parameters);

        return rowsAffected > 0;
    }

    public bool DeleteAll()
    {
        const string deleteAllQuery =
            $"""
            DELETE FROM {_tableName}
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteAllQuery, new { });

        return rowsAffected > 0;
    }

    public bool DeleteAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum)
    {
        const string deleteAllWithProductNewStatusQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE ProductNewStatus = @productNewStatus
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteAllWithProductNewStatusQuery,
            new { productNewStatus = (int)productNewStatusEnum });

        return rowsAffected > 0;
    }

    public bool DeleteAllWithProductXmlStatus(ProductXmlStatusEnum productXmlStatusEnum)
    {
        const string deleteAllWithProductXmlStatusQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE ProductXmlReadyStatus = @productXmlStatus
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteAllWithProductXmlStatusQuery,
            new { productXmlStatus = (int)productXmlStatusEnum });

        return rowsAffected > 0;
    }

    public bool DeleteAllWithReadyForImageInsert(bool readyForImageInsert)
    {
        const string deleteAllWithReadyForImageInsertQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE ReadyForImageInsertStatus = @readyForImageInsert
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteAllWithReadyForImageInsertQuery,
            new { readyForImageInsert = readyForImageInsert });

        return rowsAffected > 0;
    }

    public bool DeleteById(int productWorkStatusesId)
    {
        const string deleteByIdQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE Id = @productWorkStatusesId
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByIdQuery,
            new { productWorkStatusesId = productWorkStatusesId });

        return rowsAffected > 0;
    }

    public bool DeleteByProductId(int productId)
    {
        const string deleteAllByProductIdQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteAllByProductIdQuery,
            new { productId = productId });

        return rowsAffected > 0;
    }
#pragma warning restore IDE0037 // Use inferred member name
}