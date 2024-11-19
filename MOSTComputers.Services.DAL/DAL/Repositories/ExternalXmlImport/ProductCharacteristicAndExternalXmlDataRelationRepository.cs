using Dapper;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories.ExternalXmlImport;

internal sealed class ProductCharacteristicAndExternalXmlDataRelationRepository : RepositoryBase, IProductCharacteristicAndExternalXmlDataRelationRepository
{
    public ProductCharacteristicAndExternalXmlDataRelationRepository(IRelationalDataAccess relationalDataAccess) : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public List<ProductCharacteristicAndExternalXmlDataRelation> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            """;

        return _relationalDataAccess.GetData<ProductCharacteristicAndExternalXmlDataRelation, dynamic>(getAllQuery, new { })
            .AsList();
    }

    public List<ProductCharacteristicAndExternalXmlDataRelation> GetAllWithSameCategoryId(int categoryId)
    {
        const string getAllWithSameCategoryIdQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE TID = @categoryId;
            """;

        return _relationalDataAccess.GetData<ProductCharacteristicAndExternalXmlDataRelation, dynamic>(
            getAllWithSameCategoryIdQuery, new { categoryId = categoryId })
            .AsList();
    }

    public List<ProductCharacteristicAndExternalXmlDataRelation> GetAllWithSameCategoryIdAndXmlName(int categoryId, string xmlName)
    {
        const string getAllWithSameCategoryIdAndXmlNameQuery =
            $"""
            SELECT * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE TID = @categoryId
            AND XmlName = @Name;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = xmlName
        };

        return _relationalDataAccess.GetData<ProductCharacteristicAndExternalXmlDataRelation, dynamic>(getAllWithSameCategoryIdAndXmlNameQuery, parameters)
            .AsList();
    }

    public ProductCharacteristicAndExternalXmlDataRelation? GetById(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT TOP 1 * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE Id = @id;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductCharacteristicAndExternalXmlDataRelation, dynamic>(
            getByIdQuery, new { id = id });
    }

    public ProductCharacteristicAndExternalXmlDataRelation? GetByCharacteristicId(int characteristicId)
    {
        const string getByCharacteristicIdQuery =
            $"""
            SELECT TOP 1 * FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE ProducKeywordID = @characteristicId;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductCharacteristicAndExternalXmlDataRelation, dynamic>(
            getByCharacteristicIdQuery, new { characteristicId = characteristicId });
    }

    public OneOf<Success, UnexpectedFailureResult> UpsertByCharacteristicId(ProductCharacteristicAndExternalXmlDataRelationUpsertRequest createRequest)
    {
        const string upsertByCharacteristicIdQuery =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName} WHERE TID = @CategoryId AND XmlName = @XmlName AND XmlDisplayOrder = @XmlDisplayOrder)
            OR (@ProductCharacteristicId = NULL)
            BEGIN
                INSERT INTO {ProductCharacteristicAndExternalXmlDataRelationsTableName} (TID, TIDName, ProducKeywordID, ProducKeywordName, ProducKeywordMeaning, XmlName, XmlDisplayOrder)
                VALUES (@CategoryId, @CategoryName, @ProductCharacteristicId, @ProductCharacteristicName,
                    @ProductCharacteristicMeaning, @XmlName, @XmlDisplayOrder)
            END
            ELSE
            BEGIN
                UPDATE {ProductCharacteristicAndExternalXmlDataRelationsTableName}
                SET TIDName = @CategoryName,
                    ProducKeywordID = @ProductCharacteristicId,
                    ProducKeywordName = @ProductCharacteristicName,
                    ProducKeywordMeaning = @ProductCharacteristicMeaning

                WHERE TID = @CategoryId
                AND XmlName = @XmlName
                AND XmlDisplayOrder = @XmlDisplayOrder;
            END
            """;

        var parameters = new
        {
            CategoryId = createRequest.CategoryId,
            CategoryName = createRequest.CategoryName,
            ProductCharacteristicId = createRequest.ProductCharacteristicId,
            ProductCharacteristicName = createRequest.ProductCharacteristicName,
            ProductCharacteristicMeaning = createRequest.ProductCharacteristicMeaning,
            XmlName = createRequest.XmlName,
            XmlDisplayOrder = createRequest.XmlDisplayOrder,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(upsertByCharacteristicIdQuery, parameters);

        return (rowsAffected > 0) ? new Success() : new UnexpectedFailureResult();
    }

    public bool DeleteAllWithSameCategoryId(int categoryId)
    {
        const string deleteAllWithSameCategoryIdQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE TID = @categoryId;
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(
            deleteAllWithSameCategoryIdQuery, new { categoryId = categoryId });

        return rowsAffected > 0;
    }

    public bool DeleteAllWithSameCategoryIdAndXmlName(int categoryId, string xmlName)
    {
        const string deleteAllWithSameCategoryIdAndXmlNameQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE TID = @categoryId
            AND XmlName = @Name;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = xmlName
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteAllWithSameCategoryIdAndXmlNameQuery, parameters);

        return rowsAffected > 0;
    }

    public bool DeleteById(int id)
    {
        const string deleteByIdQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE Id = @id;
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(
            deleteByIdQuery, new { id = id });

        return rowsAffected > 0;
    }

    public bool DeleteByCharacteristicId(int characteristicId)
    {
        const string deleteByCharacteristicIdQuery =
            $"""
            DELETE FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
            WHERE ProducKeywordID = @characteristicId;
            """;

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(
            deleteByCharacteristicIdQuery, new { characteristicId = characteristicId });

        return rowsAffected > 0;
    }

#pragma warning restore IDE0037 // Use inferred member name
}