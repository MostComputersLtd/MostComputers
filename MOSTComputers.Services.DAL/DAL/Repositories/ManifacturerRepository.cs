using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Manifacturer;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ManifacturerRepository : RepositoryBase, IManifacturerRepository
{
    private const string _tableName = "dbo.Manifacturer";

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

        return _relationalDataAccess.GetData<Manifacturer, dynamic>(getByIdQuery, new { id }).FirstOrDefault();
    }

    public OneOf<Success, ValidationResult> Insert(ManifacturerInsertRequest insertRequest, IValidator<ManifacturerInsertRequest>? validator = null)
    {
        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(BGName, Name, S, Active)
            VALUES (@BGName, @Name, @DisplayOrder, @Active)
            """;

        if (validator is not null)
        {
            ValidationResult result = validator.Validate(insertRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            insertRequest.BGName,
            Name = insertRequest.RealCompanyName,
            insertRequest.DisplayOrder,
            insertRequest.Active,
        };

        _relationalDataAccess.SaveData<Manifacturer, dynamic>(insertQuery, parameters);

        return new Success();
    }

    public OneOf<Success, ValidationResult> Update(ManifacturerUpdateRequest updateRequest, IValidator<ManifacturerUpdateRequest>? validator = null)
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

        if (validator is not null)
        {
            ValidationResult result = validator.Validate(updateRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.BGName,
            Name = updateRequest.RealCompanyName,
            updateRequest.DisplayOrder,
            updateRequest.Active,
        };

        _relationalDataAccess.SaveData<Manifacturer, dynamic>(updateQuery, parameters);

        return new Success();
    }

    public bool TryDelete(uint id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE MfrID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<Manifacturer, dynamic>(deleteQuery, new { id });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}