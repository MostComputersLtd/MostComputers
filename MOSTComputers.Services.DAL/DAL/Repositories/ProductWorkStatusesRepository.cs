using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ProductStatuses;
using MOSTComputers.Models.Product.Models.Requests.ProductWorkStatuses;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductWorkStatusesRepository : RepositoryBase, IProductWorkStatusesRepository
{
    const string _tableName = "dbo.TodoProductWorkStatuses";
    const string _productTableName = "dbo.MOSTPrices";

    public ProductWorkStatusesRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

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
            new { productNewStatus = (int)productXmlStatusEnum });
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
            new { readyForImageInsert });
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
            new { productWorkStatusesId });
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
            new { productId });
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> InsertIfItDoesntExist(ProductWorkStatusesCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @TEMP_ProductWorkStatusesInsertIdStoring_Table TABLE (IdOrStatus INT);

            DECLARE @StatusCode INT = 0;

            DECLARE @Result VARCHAR(50);

            IF EXISTS (
                SELECT 1 FROM {_tableName}
                WHERE CSTID = @productId
            ) SET @StatusCode = 1;

            IF NOT EXISTS (
                SELECT 1 FROM {_productTableName}
                WHERE CSTID = @productId
            ) SET @StatusCode = 2;

            IF @StatusCode = 0
            BEGIN
                INSERT INTO {_tableName}(CSTID, ProductNewStatus, ProductXmlReadyStatus, ReadyForImageInsertStatus)
                OUTPUT INSERTED.Id INTO @TEMP_ProductWorkStatusesInsertIdStoring_Table
                VALUES(@productId, @ProductNewStatus, @ProductXmlStatus, @ReadyForImageInsert)
            END

            INSERT INTO @TEMP_ProductWorkStatusesInsertIdStoring_Table (IdOrStatus)
            VALUES (@StatusCode);

            SELECT @Result = COALESCE(@Result + '/', '') + CAST(IdOrStatus AS VARCHAR(10))
            FROM @TEMP_ProductWorkStatusesInsertIdStoring_Table;

            SELECT @Result;
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            ProductNewStatus = (int)createRequest.ProductNewStatus,
            ProductXmlStatus = (int)createRequest.ProductXmlStatus,
            createRequest.ReadyForImageInsert,
        };

        (int, int) data = _relationalDataAccess.SaveDataInTransactionScopeUsingActionAndCommitOnCondition<dynamic, (int, int)>(
            actionInTransaction: paramsLocal => InsertAndExtractData(insertQuery, paramsLocal),
            shouldCommit: tuple => tuple.Item1 > 0 && tuple.Item2 == 0,
            parameters);

        if (data.Item2 < 0) return new UnexpectedFailureResult();

        if (data.Item2 == 0) return data.Item1;

        return GetValidationResultFromStatus(data.Item2);

        (int, int) InsertAndExtractData(string insertQuery, dynamic paramsLocal)
        {
            string? data = _relationalDataAccess.SaveDataAndReturnValue<string, dynamic>(insertQuery, paramsLocal);

            if (data is null) return (-1, -1);

            int indexOfSlash = data.IndexOf('/');

            if (indexOfSlash < 0)
            {
                bool parseStatusOnlySuccess = int.TryParse(data, out int statusCodeOnlyResult);

                return (-1, statusCodeOnlyResult);
            }

            string productWorkStatusIdAsString = data[..indexOfSlash];
            string statusCodeAsString = data[(indexOfSlash + 1)..];

            bool parseProductWorkStatusIdSuccess = int.TryParse(productWorkStatusIdAsString, out int productWorkStatusId);
            bool parseStatusCodeSuccess = int.TryParse(statusCodeAsString, out int statusCode);

            if (parseProductWorkStatusIdSuccess && parseStatusCodeSuccess)
            {
                return (productWorkStatusId, statusCode);
            }

            return (-1, -1);
        }
    }

    private static OneOf<int, ValidationResult, UnexpectedFailureResult> GetValidationResultFromStatus(int status)
    {
        ValidationResult validationResult = new();

        if (status == 1)
        {
            validationResult.Errors.Add(new(nameof(ProductStatuses.ProductId), "ProductId is invalid"));
        }

        else if (status == 2)
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
            updateRequest.ProductNewStatus,
            updateRequest.ProductXmlStatus,
            updateRequest.ReadyForImageInsert,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(updateQuery, parameters);

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
            updateRequest.ProductNewStatus,
            updateRequest.ProductXmlStatus,
            updateRequest.ReadyForImageInsert,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(updateQuery, parameters);

        return rowsAffected > 0;
    }

    public bool DeleteAll()
    {
        const string deleteAllQuery =
            $"""
            DELETE FROM {_tableName}
            """;

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(deleteAllQuery, new { });

        return rowsAffected > 0;
    }

    public bool DeleteAllWithProductNewStatus(ProductNewStatusEnum productNewStatusEnum)
    {
        const string deleteAllWithProductNewStatusQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE ProductNewStatus = @productNewStatus
            """;

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(deleteAllWithProductNewStatusQuery,
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

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(deleteAllWithProductXmlStatusQuery,
            new { productNewStatus = (int)productXmlStatusEnum });

        return rowsAffected > 0;
    }

    public bool DeleteAllWithReadyForImageInsert(bool readyForImageInsert)
    {
        const string deleteAllWithReadyForImageInsertQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE ReadyForImageInsertStatus = @readyForImageInsert
            """;

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(deleteAllWithReadyForImageInsertQuery,
            new { readyForImageInsert });

        return rowsAffected > 0;
    }

    public bool DeleteById(int productWorkStatusesId)
    {
        const string deleteByIdQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE Id = @productWorkStatusesId
            """;

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(deleteByIdQuery,
            new { productWorkStatusesId });

        return rowsAffected > 0;
    }

    public bool DeleteByProductId(int productId)
    {
        const string deleteAllByProductIdQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId
            """;

        int rowsAffected = _relationalDataAccess.SaveData<ProductWorkStatuses, dynamic>(deleteAllByProductIdQuery,
            new { productId });

        return rowsAffected > 0;
    }
}