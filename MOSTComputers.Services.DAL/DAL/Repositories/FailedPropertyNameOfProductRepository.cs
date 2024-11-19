using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.FailureData.FailedPropertyNameOfProduct;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class FailedPropertyNameOfProductRepository : RepositoryBase, IFailedPropertyNameOfProductRepository
{
    public FailedPropertyNameOfProductRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<FailedPropertyNameOfProduct> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {FailedPropertyNamesOfProductsTableName}
            """;

        return _relationalDataAccess.GetData<FailedPropertyNameOfProduct, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<FailedPropertyNameOfProduct> GetAllForProduct(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {FailedPropertyNamesOfProductsTableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<FailedPropertyNameOfProduct, dynamic>(getAllForProductQuery,
            new { productId = productId });
    }

    public IEnumerable<FailedPropertyNameOfProduct> GetAllForSelectionOfProducts(IEnumerable<int> productIds)
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {FailedPropertyNamesOfProductsTableName}
            WHERE CSTID IN @productIds;
            """;

        return _relationalDataAccess.GetData<FailedPropertyNameOfProduct, dynamic>(getAllQuery,
            new { productIds = productIds });
    }

    public bool Insert(FailedPropertyNameOfProductCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            IF NOT EXISTS(
                SELECT 1 FROM {FailedPropertyNamesOfProductsTableName}
                WHERE CSTID = @productId
                AND PropertyName = @PropertyName
            )
            INSERT INTO {FailedPropertyNamesOfProductsTableName}(CSTID, PropertyName)
            VALUES (@productId, PropertyName)
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.PropertyName
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(insertQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool MultiInsert(FailedPropertyNameOfProductMultiCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            IF NOT EXISTS(
                SELECT 1 FROM {FailedPropertyNamesOfProductsTableName}
                WHERE CSTID = @productId
                AND PropertyName = @PropertyNames
            )
            INSERT INTO {FailedPropertyNamesOfProductsTableName}(CSTID, PropertyName)
            VALUES (@productId, @PropertyNames)
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            PropertyNames = createRequest.PropertyNames.AsEnumerable()
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(insertQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool Update(FailedPropertyNameOfProductUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {FailedPropertyNamesOfProductsTableName}
                SET PropertyName = @NewPropertyName

            WHERE CSTID = @productId
            AND PropertyName = @OldPropertyName
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.OldPropertyName,
            updateRequest.NewPropertyName,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool Delete(int productId, string propertyName)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {FailedPropertyNamesOfProductsTableName}
            WHERE CSTID = @productId
            AND PropertyName = @PropertyName;
            """;

        var parameters = new
        {
            productId = productId,
            PropertyName = propertyName
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteAllForProduct(int productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {FailedPropertyNamesOfProductsTableName}
            WHERE CSTID = @productId;
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery,
            new { productId = productId });

        return (rowsAffected > 0);
    }

    public bool DeleteAllForSelectionOfProducts(IEnumerable<int> productIds)
    {
        const string getAllQuery =
            $"""
            DELETE FROM {FailedPropertyNamesOfProductsTableName}
            WHERE CSTID IN @productIds;
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(getAllQuery,
            new { productIds = productIds });

        return (rowsAffected > 0);
    }
#pragma warning restore IDE0037 // Use inferred member name
}