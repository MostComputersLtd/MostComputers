using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.Category;
using OneOf;
using OneOf.Types;
using System.Data.SqlClient;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class CategoryRepository : RepositoryBase, ICategoryRepository
{
    public CategoryRepository(IRelationalDataAccess relationalDataAccess)
        :base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<Category> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {CategoriesTableName}
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<Category, dynamic>(getAllQuery, new { });
    }

    public Category? GetById(int id)
    {
        const string getByIdQuery =
            $"""
            SELECT * FROM {CategoriesTableName}
            WHERE CategoryID = @id;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<Category, dynamic>(getByIdQuery, new { id = id });
    }

    public OneOf<int, UnexpectedFailureResult> Insert(CategoryCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            IF @parentId IS NULL
            OR EXISTS (
                SELECT TOP 1 CategoryID FROM {CategoriesTableName}
                WHERE CategoryID = @parentId
            )
            INSERT INTO {CategoriesTableName}(Description, IsLeaf, S, rowguid, ProductsUpdateCounter, ParentId)
            OUTPUT INSERTED.CategoryID
            VALUES (@Description, @IsLeaf, @DisplayOrder, @RowGuid, @ProductsUpdateCounter, @parentId);
            """;

        var parameters = new
        {
            createRequest.Description,
            createRequest.IsLeaf,
            createRequest.DisplayOrder,
            createRequest.RowGuid,
            createRequest.ProductsUpdateCounter,
            parentId = createRequest.ParentCategoryId,
        };

        int? id = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertQuery, parameters);

        return (id is not null && id > 0) ? id.Value : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> Update(CategoryUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {CategoriesTableName}
            SET Description = @Description,
                S = @DisplayOrder,
                rowguid = @RowGuid,
                ProductsUpdateCounter = @ProductsUpdateCounter

            WHERE CategoryID = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.Description,
            updateRequest.DisplayOrder,
            updateRequest.RowGuid,
            updateRequest.ProductsUpdateCounter,
        };

        int rowsAffected = _relationalDataAccess.SaveData<dynamic>(updateQuery, parameters);

        return (rowsAffected > 0) ? new Success() : new UnexpectedFailureResult();
    }

    public bool Delete(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {CategoriesTableName}
            WHERE CategoryID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, new { id = id });

            return (rowsAffected > 0);
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

#pragma warning restore IDE0037 // Use inferred member name
}