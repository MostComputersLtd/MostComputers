using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class LocalChangesRepository : RepositoryBase, ILocalChangesRepository
{
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
        FROM {LocalChangesTableName}
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
        FROM {LocalChangesTableName}
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
        FROM {LocalChangesTableName}
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
        FROM {LocalChangesTableName}
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
        FROM {LocalChangesTableName}
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
        DELETE FROM {LocalChangesTableName}
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
        DELETE FROM {LocalChangesTableName}
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
        DELETE FROM {LocalChangesTableName}
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
        DELETE FROM {LocalChangesTableName}
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