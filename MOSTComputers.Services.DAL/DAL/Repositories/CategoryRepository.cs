using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Category;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;
using System.Data.SqlClient;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class CategoryRepository : RepositoryBase, ICategoryRepository
{
    private const string _tableName = "dbo.Categories";

    public CategoryRepository(IRelationalDataAccess relationalDataAccess)
        :base(relationalDataAccess)
    {
    }

    public IEnumerable<Category> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {_tableName}
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<Category, dynamic>(getAllQuery, new { });
    }

    public Category? GetById(uint id)
    {
        const string getByIdQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CategoryID = @id;
            """;

        return _relationalDataAccess.GetData<Category, dynamic>(getByIdQuery, new { id }).FirstOrDefault();
    }

    public OneOf<Success, UnexpectedFailureResult> Insert(CategoryCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(Description, IsLeaf, S, rowguid, ProductsUpdateCounter, ParentId)
            VALUES (@Description, @IsLeaf, @DisplayOrder, @RowGuid, @ProductsUpdateCounter, @ParentId);
            """;

        var parameters = new
        {
            createRequest.Description,
            createRequest.IsLeaf,
            createRequest.DisplayOrder,
            createRequest.RowGuid,
            createRequest.ProductsUpdateCounter,
            ParentId = createRequest.ParentCategoryId,
        };

        int rowsAffected = _relationalDataAccess.SaveData<Category, dynamic>(insertQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> Update(CategoryUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {_tableName}
            SET Description = @Description,
                IsLeaf = @IsLeaf,
                S = @DisplayOrder,
                rowguid = @RowGuid,
                ProductsUpdateCounter = @ProductsUpdateCounter

            WHERE CategoryID = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.Description,
            updateRequest.IsLeaf,
            updateRequest.DisplayOrder,
            updateRequest.RowGuid,
            updateRequest.ProductsUpdateCounter,
            ParentId = updateRequest.ParentCategoryId,
        };

        int rowsAffected = _relationalDataAccess.SaveData<Category, dynamic>(updateQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public bool Delete(uint id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CategoryID = @id;
            """;
        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<Category, dynamic>(deleteQuery, new { id });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (SqlException)
        {
            return false;
        }
    }
}