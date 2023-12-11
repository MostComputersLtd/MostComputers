using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ManifacturerRepository : RepositoryBase, IManifacturerRepository
{
    private const string _tableName = "dbo.Manufacturer";

    public ManifacturerRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<Manifacturer> GetAll()
    {
        const string getAllQuery = 
            $"""
            SELECT MfrID AS PersonalManifacturerId, BGName, Name, S AS ManifacturerDisplayOrder, Active
            FROM {_tableName}
            ORDER BY S
            """;

        return _relationalDataAccess.GetData<Manifacturer, dynamic>(getAllQuery, new { });
    }

    public Manifacturer? GetById(uint id)
    {
        const string getByIdQuery =
            $"""
            SELECT MfrID AS PersonalManifacturerId, BGName, Name, S AS ManifacturerDisplayOrder, Active
            FROM {_tableName}
            WHERE MfrID = @id;
            """;

        return _relationalDataAccess.GetData<Manifacturer, dynamic>(getByIdQuery, new { id = (int)id }).FirstOrDefault();
    }

    public OneOf<uint, UnexpectedFailureResult> Insert(ManifacturerCreateRequest insertRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(MfrID, BGName, Name, S, Active)
            OUTPUT INSERTED.MfrID
            VALUES ((SELECT MAX(MfrID) + 1 FROM {_tableName}), @BGName, @Name, @DisplayOrder, @Active)
            """;

        var parameters = new
        {
            insertRequest.BGName,
            Name = insertRequest.RealCompanyName,
            insertRequest.DisplayOrder,
            insertRequest.Active,
        };

        double? id = _relationalDataAccess.SaveDataAndReturnValue<double?, dynamic>(insertQuery, parameters);

        return (id is not null && id > 0) ? (uint)id : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {_tableName}
            SET BGName = @BGName,
                Name = @Name,
                S = @DisplayOrder,
                Active = @Active
            WHERE MfrID = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.BGName,
            Name = updateRequest.RealCompanyName,
            updateRequest.DisplayOrder,
            updateRequest.Active,
        };

        int rowsAffected = _relationalDataAccess.SaveData<Manifacturer, dynamic>(updateQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public bool Delete(uint id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE MfrID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<Manifacturer, dynamic>(deleteQuery, new { id = (int)id });

            return rowsAffected > 0;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}