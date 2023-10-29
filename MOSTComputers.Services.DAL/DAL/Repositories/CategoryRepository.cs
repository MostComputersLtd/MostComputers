using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.Category;
using OneOf;
using OneOf.Types;

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

    public OneOf<Success, ValidationResult> Insert(CategoryCreateRequest createRequest, IValidator<CategoryCreateRequest>? validator = null)
    {
        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(Description, IsLeaf, S, rowguid, ProductsUpdateCounter, ParentId)
            VALUES (@Description, @IsLeaf, @DisplayOrder, @RowGuid, @ProductsUpdateCounter, @ParentId);
            """;

        if (validator != null)
        {
            ValidationResult result = validator.Validate(createRequest);

            if (!result.IsValid) return result;
        }

        var parameters = new
        {
            createRequest.Description,
            createRequest.IsLeaf,
            createRequest.DisplayOrder,
            createRequest.RowGuid,
            createRequest.ProductsUpdateCounter,
            ParentId = createRequest.ParentCategoryId,
        };

        _relationalDataAccess.SaveData<Category, dynamic>(insertQuery, parameters);

        return new Success();
    }

    public OneOf<Success, ValidationResult> Update(CategoryUpdateRequest updateRequest, IValidator<CategoryUpdateRequest>? validator = null)
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

        if (validator != null)
        {
            ValidationResult result = validator.Validate(updateRequest);

            if (!result.IsValid) return result;
        }

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

        _relationalDataAccess.SaveData<Category, dynamic>(updateQuery, parameters);

        return new Success();
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
    }
}