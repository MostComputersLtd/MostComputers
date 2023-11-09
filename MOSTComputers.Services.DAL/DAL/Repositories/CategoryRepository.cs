using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Category;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
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

        return _relationalDataAccess.GetData<Category, dynamic>(getByIdQuery, new { id = (int)id }).FirstOrDefault();
    }

    public OneOf<Success, UnexpectedFailureResult> Insert(CategoryCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            IF EXISTS (
                SELECT 1 FROM {_tableName}
                WHERE CategoryID = @parentId
            )
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
                S = @DisplayOrder,
                rowguid = @RowGuid,
                ProductsUpdateCounter = @ProductsUpdateCounter

            WHERE CategoryID = @id;
            """;

        var parameters = new
        {
            id = (int)updateRequest.Id,
            updateRequest.Description,
            updateRequest.DisplayOrder,
            updateRequest.RowGuid,
            updateRequest.ProductsUpdateCounter,
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
            int rowsAffected = _relationalDataAccess.SaveData<Category, dynamic>(deleteQuery, new { id = (int)id });

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