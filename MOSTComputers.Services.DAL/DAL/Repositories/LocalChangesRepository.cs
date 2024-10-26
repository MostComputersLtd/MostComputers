using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class LocalChangesRepository : RepositoryBase, ILocalChangesRepository
{
    private const string _tableName = "dbo.Changes4Web";

    public LocalChangesRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name
    public IEnumerable<LocalChangeData> GetAll()
    {
        const string getAllQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp 
        FROM {_tableName}
        ORDER BY TimeStamp;
        """;

        return _relationalDataAccess.GetData<LocalChangeData, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<LocalChangeData> GetAllForTable(string tableName)
    {
        const string getAllForTableQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {_tableName}
        WHERE TableName = @tableName
        ORDER BY TimeStamp;
        """;

        var parameters = new
        {
            tableName = tableName
        };

        return _relationalDataAccess.GetData<LocalChangeData, dynamic>(getAllForTableQuery, parameters);
    }

    public IEnumerable<LocalChangeData> GetAllForOperationType(ChangeOperationTypeEnum changeOperationType)
    {
        const string getAllForOperationTypeQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {_tableName}
        WHERE Operation = @changeOperationType
        ORDER BY TimeStamp;
        """;

        var parameters = new
        {
            changeOperationType = (int)changeOperationType
        };

        return _relationalDataAccess.GetData<LocalChangeData, dynamic>(getAllForOperationTypeQuery, parameters);
    }

    public LocalChangeData? GetById(int id)
    {
        const string getByIdQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {_tableName}
        WHERE PK = @id;
        """;

        var parameters = new
        {
            id = id
        };

        return _relationalDataAccess.GetDataFirstOrDefault<LocalChangeData, dynamic>(getByIdQuery, parameters);
    }

    public LocalChangeData? GetByTableNameAndElementIdAndOperationType(string tableName, int elementId, ChangeOperationTypeEnum changeOperationType)
    {
        const string getByTableNameAndElementIdQuery =
        $"""
        SELECT PK AS LocalChangePK, ID AS LocalChangeID, Operation AS LocalChangeOperation, 
            TableName AS LocalChangeTableName, TimeStamp AS LocalChangeTimeStamp
        FROM {_tableName}
        WHERE TableName = @tableName
        AND ID = @elementId
        AND Operation = @changeOperationType;
        """;

        var parameters = new
        {
            tableName = tableName,
            elementId = elementId,
            changeOperationType = (int)changeOperationType
        };

        return _relationalDataAccess.GetDataFirstOrDefault<LocalChangeData, dynamic>(getByTableNameAndElementIdQuery, parameters);
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

    public bool DeleteByTableNameAndElementId(string tableName, int elementId)
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