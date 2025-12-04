//using FluentValidation.Results;
//using OneOf;
//using OneOf.Types;
//using MOSTComputers.Models.Product.Models.ProductStatuses;
//using MOSTComputers.Services.DataAccess.Common;
//using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductStatuses;
//using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;

//using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
//using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.ProductStatusesTable;

//namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
//internal sealed class ProductStatusesRepository : IProductStatusesRepository
//{
//    public ProductStatusesRepository(IRelationalDataAccess relationalDataAccess)
//    {
//        _relationalDataAccess = relationalDataAccess;
//    }

//    private readonly IRelationalDataAccess _relationalDataAccess;

//    public IEnumerable<ProductStatuses> GetAll()
//    {
//        const string getAllQuery =
//            $"""
//            SELECT * FROM {ProductStatusesTableName}
//            """;

//        return _relationalDataAccess.GetData<ProductStatuses, dynamic>(getAllQuery, new { });
//    }

//    public ProductStatuses? GetByProductId(int productId)
//    {
//        const string getByProductIdQuery =
//            $"""
//            SELECT TOP 1 * FROM {ProductStatusesTableName}
//            WHERE {ProductIdColumnName} = @productId
//            """;
        
//        var parameters = new { productId = productId };

//        return _relationalDataAccess.GetDataFirstOrDefault<ProductStatuses, dynamic>(getByProductIdQuery, parameters);
//    }

//    public IEnumerable<ProductStatuses> GetSelectionByProductIds(IEnumerable<int> productIds)
//    {
//        const string getByProductIdQuery =
//            $"""
//            SELECT * FROM {ProductStatusesTableName}
//            WHERE {ProductIdColumnName} IN @productIds
//            """;

//        var parameters = new { productIds = productIds };

//        return _relationalDataAccess.GetData<ProductStatuses, dynamic>(getByProductIdQuery, parameters);
//    }

//    public OneOf<Success, ValidationResult> InsertIfItDoesntExist(ProductStatusesCreateRequest createRequest)
//    {
//        const string insertQuery =
//            $"""
//            DECLARE @StatusCode INT = 0;

//            IF EXISTS (
//                SELECT 1 FROM {ProductStatusesTableName}
//                WHERE {ProductIdColumnName} = @productId
//            ) SET @StatusCode = 1;

//            IF @StatusCode = 0
//            BEGIN
//                INSERT INTO {ProductStatusesTableName}({ProductIdColumnName}, {IsProcessedColumnName}, {NeedsToBeUpdatedColumnName})
//                VALUES(@productId, @IsProcessed, @NeedsToBeUpdated)
//            END

//            SELECT @StatusCode;
//            """;

//        var parameters = new
//        {
//            productId = createRequest.ProductId,
//            IsProcessed = createRequest.IsProcessed,
//            NeedsToBeUpdated = createRequest.NeedsToBeUpdated,
//        };

//        int data = _relationalDataAccess.SaveDataAndReturnValue<int, dynamic>(insertQuery, parameters);

//        if (data == 0) return new Success();

//        ValidationResult validationResult = new();

//        if (data == 1)
//        {
//            validationResult.Errors.Add(new(nameof(ProductStatuses.ProductId), "ProductId is invalid, because there is a duplicate of it in the records"));
//        }

//        return validationResult;
//    }

//    public bool Update(ProductStatusesUpdateRequest updateRequest)
//    {
//        const string updateQuery =
//            $"""
//            UPDATE {ProductStatusesTableName}
//            SET {IsProcessedColumnName} = @IsProcessed,
//                {NeedsToBeUpdatedColumnName} = @NeedsToBeUpdated

//            WHERE {ProductIdColumnName} = @productId
//            """;

//        var parameters = new
//        {
//            productId = updateRequest.ProductId,
//            IsProcessed = updateRequest.IsProcessed,
//            NeedsToBeUpdated = updateRequest.NeedsToBeUpdated,
//        };

//        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateQuery, parameters);

//        return rowsAffected > 0;
//    }

//    public bool DeleteByProductId(int productId)
//    {
//        const string deleteByProductId =
//            $"""
//            DELETE FROM {ProductStatusesTableName}
//            WHERE {ProductIdColumnName} = @productId
//            """;

//        var parameters = new
//        {
//            productId = productId,
//        };

//        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByProductId, parameters);

//        return rowsAffected > 0;
//    }
//}