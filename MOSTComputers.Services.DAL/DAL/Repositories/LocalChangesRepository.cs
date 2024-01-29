using MOSTComputers.Models.Product.Models.Changes;
using MOSTComputers.Models.Product.Models.Changes.Local;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        SELECT * FROM {_tableName}
        ORDER BY TimeStamp;
        """;

        return _relationalDataAccess.GetData<LocalChangeData, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<LocalChangeData> GetAllForTable(string tableName)
    {
        const string getAllForTableQuery =
        $"""
        SELECT * FROM {_tableName}
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
        SELECT * FROM {_tableName}
        WHERE Operation = @changeOperationType
        ORDER BY TimeStamp;
        """;

        var parameters = new
        {
            changeOperationType = (int)changeOperationType
        };

        return _relationalDataAccess.GetData<LocalChangeData, dynamic>(getAllForOperationTypeQuery, parameters);
    }

    public LocalChangeData? GetById(uint id)
    {
        const string getByIdQuery =
        $"""
        SELECT * FROM {_tableName}
        WHERE PK = @id;
        """;

        var parameters = new
        {
            id = (int)id
        };

        return _relationalDataAccess.GetDataFirstOrDefault<LocalChangeData, dynamic>(getByIdQuery, parameters);
    }

    public LocalChangeData? GetByTableNameAndElementId(string tableName, int elementId)
    {
        const string getByTableNameAndElementIdQuery =
        $"""
        SELECT * FROM {_tableName}
        WHERE TableName = @tableName
        AND ID = @elementId;
        """;

        var parameters = new
        {
            tableName = tableName,
            elementId = elementId
        };

        return _relationalDataAccess.GetDataFirstOrDefault<LocalChangeData, dynamic>(getByTableNameAndElementIdQuery, parameters);
    }

    public bool DeleteById(uint id)
    {
        const string deleteByIdQuery =
        $"""
        DELETE FROM {_tableName}
        WHERE PK = @id;
        """;

        var parameters = new
        {
            id = (int)id
        };

        int rowsAffected = _relationalDataAccess.SaveData<LocalChangeData, dynamic>(deleteByIdQuery, parameters);

        return (rowsAffected > 0);
    }

    public bool DeleteRangeByIds(IEnumerable<uint> ids)
    {
        const string deleteByIdQuery =
        $"""
        DELETE FROM {_tableName}
        WHERE PK IN @ids;
        """;

        var parameters = new
        {
            ids = ids.Select(id => (int)id)
        };

        int rowsAffected = _relationalDataAccess.SaveData<LocalChangeData, dynamic>(deleteByIdQuery, parameters);

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

        int rowsAffected = _relationalDataAccess.SaveData<LocalChangeData, dynamic>(deleteByTableNameAndElementIdQuery, parameters);

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

        int rowsAffected = _relationalDataAccess.SaveData<LocalChangeData, dynamic>(deleteByTableNameAndElementIdsQuery, parameters);

        return (rowsAffected > 0);
    }
}
#pragma warning restore IDE0037 // Use inferred member name