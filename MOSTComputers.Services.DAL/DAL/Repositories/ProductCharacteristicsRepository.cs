using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductCharacteristicsRepository : RepositoryBase, IProductCharacteristicsRepository
{
    private const string _tableName = "dbo.ProductKeyword";

    public ProductCharacteristicsRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<ProductCharacteristic> GetAllByCategoryId(uint categoryId)
    {
        const string getAllByCategoryIdQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getAllByCategoryIdQuery, new { categoryId = (int)categoryId });
    }

    public ProductCharacteristic? GetByCategoryIdAndName(uint categoryId, string name)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            AND Name = @Name;
            """;

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery, new { categoryId = (int)categoryId, Name = name }).FirstOrDefault();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(TID, Name, KeywordMeaning, S, Active, PKUserId, LastUpdate, KWPrCh)
            VALUES (@CategoryId, @Name, @Meaning, @DisplayOrder, @Active, @PKUserId, @LastUpdate, @KWPrCh)
            """;

        ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueName(createRequest.Name, (uint)createRequest.CategoryId!);

        if (!resultInternal.IsValid) return resultInternal;

        var parameters = new
        {
            createRequest.CategoryId,
            createRequest.Name,
            createRequest.Meaning,
            createRequest.DisplayOrder,
            createRequest.Active,
            createRequest.PKUserId,
            createRequest.LastUpdate,
            createRequest.KWPrCh,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductCharacteristic, dynamic>(insertQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest)
    {
        const string updateByIdQuery =
            $"""
            UPDATE {_tableName}
            SET Name = @Name,
                KeywordMeaning = @Meaning,
                S = @DisplayOrder,
                Active = @Active,
                PKUserID = @PKUserId,
                LastUpdate = @LastUpdate,
                KWPrCh = @KWPrCh

            WHERE ProductKeywordID = @id;
            """;

        const string getCategoryIdByIdQuery =
            $"""
            SELECT TID FROM {_tableName}
            WHERE ProductKeywordID = @id;
            """;

        int? categoryId = _relationalDataAccess.GetData<int?, dynamic>(getCategoryIdByIdQuery, new { id = updateRequest.Id }).FirstOrDefault();

        if (categoryId != null)
        {
            ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueName(updateRequest.Name, (uint)categoryId);

            if (!resultInternal.IsValid) return resultInternal;
        }

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.Name,
            updateRequest.Meaning,
            updateRequest.DisplayOrder,
            updateRequest.Active,
            updateRequest.PKUserId,
            updateRequest.LastUpdate,
            updateRequest.KWPrCh,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductCharacteristic, dynamic>(updateByIdQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest)
    {
        const string updateByNameAndCategoryIdQuery =
            $"""
            UPDATE {_tableName}
            SET Name = @Name,
                KeywordMeaning = @Meaning,
                S = @DisplayOrder,
                Active = @Active,
                PKUserID = @PKUserId,
                LastUpdate = @LastUpdate,
                KWPrCh = @KWPrCh

            WHERE TID = @categoryId
            AND Name = @OldName;
            """;

        ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueName(updateRequest.Name, (uint)updateRequest.CategoryId);

        if (!resultInternal.IsValid) return resultInternal;

        var parameters = new
        {
            categoryId = updateRequest.CategoryId,
            OldName = updateRequest.Name,
            Name = updateRequest.NewName,
            updateRequest.Meaning,
            updateRequest.DisplayOrder,
            updateRequest.Active,
            updateRequest.PKUserId,
            updateRequest.LastUpdate,
            updateRequest.KWPrCh,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductCharacteristic, dynamic>(updateByNameAndCategoryIdQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    private ValidationResult ValidateWhetherCharacteristicHasAUniqueName(string? name, uint categoryId)
    {
        ValidationResult output = new();

        const string checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery =
            $"""
            SELECT CASE WHEN EXISTS(
                SELECT * FROM {_tableName}
                WHERE TID = @categoryId
                AND Name = @Name
            ) THEN 1 ELSE 0 END;
            """;

        bool duplicateCharacteristicExists = _relationalDataAccess.GetData<bool, dynamic>(checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery, 
            new { categoryId, Name = name })
            .FirstOrDefault();

        if (!duplicateCharacteristicExists) return output;

            output.Errors.Add(
                new ValidationFailure(
                    nameof(ProductCharacteristic.Name),
                    "Cannot have a duplicate name value for items in the same category"));

        return output;
    }

    public bool Delete(uint id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE ProductKeywordID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductCharacteristic, dynamic>(deleteQuery, new { id = (int)id });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllForCategory(uint productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE TID = @productId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductCharacteristic, dynamic>(deleteQuery, new { productId = (int)productId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}