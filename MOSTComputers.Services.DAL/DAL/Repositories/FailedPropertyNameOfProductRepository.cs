using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.FailureData;
using MOSTComputers.Models.Product.Models.FailureData.Requests.FailedPropertyNameOfProduct;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class FailedPropertyNameOfProductRepository : RepositoryBase, IFailedPropertyNameOfProductRepository
{
    private const string _tableName = "FailedPropertyNamesOfProducts";
    public FailedPropertyNameOfProductRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<FailedPropertyNameOfProduct> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {_tableName}
            """;

        return _relationalDataAccess.GetData<FailedPropertyNameOfProduct, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<FailedPropertyNameOfProduct> GetAllForProduct(uint productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<FailedPropertyNameOfProduct, dynamic>(getAllForProductQuery, new { productId = (int)productId });
    }

    public IEnumerable<FailedPropertyNameOfProduct> GetAllForSelectionOfProducts(IEnumerable<uint> productIds)
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID IN @productIds;
            """;

        return _relationalDataAccess.GetData<FailedPropertyNameOfProduct, dynamic>(getAllQuery, new { productIds = productIds.Select(productId => (int)productId) });
    }

    public bool Insert(FailedPropertyNameOfProductCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            IF NOT EXISTS(
                SELECT 1 FROM {_tableName}
                WHERE CSTID = @productId
                AND PropertyName = @PropertyName
            )
            INSERT INTO {_tableName}(CSTID, PropertyName)
            VALUES (@productId, PropertyName)
            """;

        var parameters = new
        {
            productId = (int)createRequest.ProductId,
            createRequest.PropertyName
        };

        int rowsAffected = _relationalDataAccess.SaveData<FailedPropertyNameOfProduct, dynamic>(insertQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool MultiInsert(FailedPropertyNameOfProductMultiCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            IF NOT EXISTS(
                SELECT 1 FROM {_tableName}
                WHERE CSTID = @productId
                AND PropertyName = @PropertyNames
            )
            INSERT INTO {_tableName}(CSTID, PropertyName)
            VALUES (@productId, @PropertyNames)
            """;

        var parameters = new
        {
            productId = (int)createRequest.ProductId,
            PropertyNames = createRequest.PropertyNames.AsEnumerable()
        };

        int rowsAffected = _relationalDataAccess.SaveData<FailedPropertyNameOfProduct, dynamic>(insertQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool Update(FailedPropertyNameOfProductUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {_tableName}
                SET PropertyName = @NewPropertyName

            WHERE CSTID = @productId
            AND PropertyName = @OldPropertyName
            """;

        var parameters = new
        {
            productId = (int)updateRequest.ProductId,
            updateRequest.OldPropertyName,
            updateRequest.NewPropertyName,
        };

        int rowsAffected = _relationalDataAccess.SaveData<FailedPropertyNameOfProduct, dynamic>(updateQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool Delete(uint productId, string propertyName)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId
            AND PropertyName = @PropertyName;
            """;

        var parameters = new
        {
            productId = (int)productId,
            PropertyName = propertyName
        };

        int rowsAffected = _relationalDataAccess.SaveData<FailedPropertyNameOfProduct, dynamic>(deleteQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteAllForProduct(uint productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        int rowsAffected = _relationalDataAccess.SaveData<FailedPropertyNameOfProduct, dynamic>(deleteQuery, new { productId = (int)productId });

        return (rowsAffected > 0);
    }

    public bool DeleteAllForSelectionOfProducts(IEnumerable<uint> productIds)
    {
        const string getAllQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID IN @productIds;
            """;

        int rowsAffected = _relationalDataAccess.SaveData<FailedPropertyNameOfProduct, dynamic>(getAllQuery, new { productIds = productIds.Select(productId => (int)productId) });

        return (rowsAffected > 0);
    }
}