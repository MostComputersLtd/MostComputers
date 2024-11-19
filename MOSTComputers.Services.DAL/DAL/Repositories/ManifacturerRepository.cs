using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ManifacturerRepository : RepositoryBase, IManifacturerRepository
{
    public ManifacturerRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<Manifacturer> GetAll()
    {
        const string getAllQuery = 
            $"""
            SELECT MfrID AS PersonalManifacturerId, BGName, Name, S AS ManifacturerDisplayOrder, Active
            FROM {ManifacturersTableName}
            ORDER BY S
            """;

        return _relationalDataAccess.GetData<Manifacturer, dynamic>(getAllQuery, new { });
    }

    public Manifacturer? GetById(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT MfrID AS PersonalManifacturerId, BGName, Name, S AS ManifacturerDisplayOrder, Active
            FROM {ManifacturersTableName}
            WHERE MfrID = @id;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<Manifacturer, dynamic>(getByIdQuery, new { id = id });
    }

    public OneOf<int, UnexpectedFailureResult> Insert(ManifacturerCreateRequest insertRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {ManifacturersTableName}(MfrID, BGName, Name, S, Active)
            OUTPUT INSERTED.MfrID
            VALUES (ISNULL((SELECT MAX(MfrID) + 1 FROM {ManifacturersTableName}), 1), @BGName, @Name, @DisplayOrder, @Active)
            """;

        var parameters = new
        {
            BGName = insertRequest.BGName,
            Name = insertRequest.RealCompanyName,
            DisplayOrder = insertRequest.DisplayOrder,
            Active = insertRequest.Active,
        };

        double? id = _relationalDataAccess.SaveDataAndReturnValue<double?, dynamic>(insertQuery, parameters);

        return (id is not null && id > 0) ? (int)id : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> Update(ManifacturerUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {ManifacturersTableName}
            SET BGName = @BGName,
                Name = @Name,
                S = @DisplayOrder,
                Active = @Active
            WHERE MfrID = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            BGName = updateRequest.BGName,
            Name = updateRequest.RealCompanyName,
            DisplayOrder = updateRequest.DisplayOrder,
            Active = updateRequest.Active,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public bool Delete(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {ManifacturersTableName}
            WHERE MfrID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, new { id = id });

            return rowsAffected > 0;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

#pragma warning restore IDE0037 // Use inferred member name
}