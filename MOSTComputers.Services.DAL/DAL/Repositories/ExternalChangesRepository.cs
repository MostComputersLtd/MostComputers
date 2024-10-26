using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.External;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ExternalChangesRepository : RepositoryBase, IExternalChangesRepository
{
    private const string _tableName = "dbo.Changes";

    public ExternalChangesRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name
    public IEnumerable<ExternalChangeData> GetAll()
    {
        const string getAllQuery =
        $"""
        SELECT PK AS ExternalChangePK, ID AS ExternalChangeID, Operation AS ExternalChangeOperation, TableName AS ExternalChangeTableName
        FROM {_tableName};
        """;

        return _relationalDataAccess.GetData<ExternalChangeData, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<ExternalChangeData> GetAllForTable(string tableName)
    {
        const string getAllForTableQuery =
        $"""
        SELECT PK AS ExternalChangePK, ID AS ExternalChangeID, Operation AS ExternalChangeOperation, TableName AS ExternalChangeTableName
        FROM {_tableName}
        WHERE TableName = @tableName;
        """;

        var parameters = new
        {
            tableName = tableName
        };

        return _relationalDataAccess.GetData<ExternalChangeData, dynamic>(getAllForTableQuery, parameters);
    }

    public IEnumerable<ExternalChangeData> GetAllForOperationType(ChangeOperationTypeEnum changeOperationType)
    {
        const string getAllForOperationTypeQuery =
        $"""
        SELECT PK AS ExternalChangePK, ID AS ExternalChangeID, Operation AS ExternalChangeOperation, TableName AS ExternalChangeTableName
        FROM {_tableName}
        WHERE Operation = @changeOperationType;
        """;

        var parameters = new
        {
            changeOperationType = (int)changeOperationType
        };

        return _relationalDataAccess.GetData<ExternalChangeData, dynamic>(getAllForOperationTypeQuery, parameters);
    }

    public IEnumerable<ExternalChangeData> GetAllByTableNameAndElementId(string tableName, int elementId)
    {
        const string getByTableNameAndElementIdQuery =
        $"""
        SELECT PK AS ExternalChangePK, ID AS ExternalChangeID, Operation AS ExternalChangeOperation, TableName AS ExternalChangeTableName
        FROM {_tableName}
        WHERE TableName = @tableName
        AND ID = @elementId;
        """;

        var parameters = new
        {
            tableName = tableName,
            elementId = elementId
        };

        return _relationalDataAccess.GetData<ExternalChangeData, dynamic>(getByTableNameAndElementIdQuery, parameters);
    }

    public ExternalChangeData? GetById(int id)
    {
        const string getByIdQuery =
        $"""
        SELECT PK AS ExternalChangePK, ID AS ExternalChangeID, Operation AS ExternalChangeOperation, TableName AS ExternalChangeTableName
        FROM {_tableName}
        WHERE PK = @id;
        """;

        var parameters = new
        {
            id = id
        };

        return _relationalDataAccess.GetDataFirstOrDefault<ExternalChangeData, dynamic>(getByIdQuery, parameters);
    }

    public bool DeleteById(int id)
    {
        const string deleteByIdQuery =
        $"""
        DELETE FROM {_tableName}
        WHERE PK = @id;
        """;

        var parameters = new
        {
            id = id
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByIdQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum operationType)
    {
        const string deleteByTableNameAndElementIdQuery =
        $"""
        DELETE FROM {_tableName}
        WHERE TableName = @tableName
        AND ID = @elementId
        AND Operation = @operationType;
        """;

        var parameters = new
        {
            tableName = tableName,
            elementId = elementId,
            operationType = (int)operationType,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByTableNameAndElementIdQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteRangeByIds(IEnumerable<int> ids)
    {
        const string deleteByIdQuery =
        $"""
        DELETE FROM {_tableName}
        WHERE PK IN @ids;
        """;

        var parameters = new
        {
            ids = ids
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByIdQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteAllByTableNameAndElementId(string tableName, int elementId)
    {
        const string deleteByTableNameAndElementIdQuery =
        $"""
        DELETE FROM {_tableName}
        WHERE TableName = @tableName
        AND ID = @elementId;
        """;

        var parameters = new
        {
            tableName = tableName,
            elementId = elementId
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByTableNameAndElementIdQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteRangeByTableNameAndElementIds(string tableName, IEnumerable<int> elementIds)
    {
        const string deleteByTableNameAndElementIdsQuery =
        $"""
        DELETE FROM {_tableName}
        WHERE TableName = @tableName
        AND ID IN @elementIds;
        """;

        var parameters = new
        {
            tableName = tableName,
            elementIds = elementIds
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByTableNameAndElementIdsQuery, parameters);

        return (rowsAffected > 0);
    }

#pragma warning restore IDE0037 // Use inferred member name
}