using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductCharacteristic;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories;
#pragma warning disable IDE0037 // Use inferred member name

internal sealed class ProductCharacteristicsRepository : RepositoryBase, IProductCharacteristicsRepository
{
    private const string _tableName = "dbo.ProductKeyword";

    public ProductCharacteristicsRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<ProductCharacteristic> GetAllCharacteristicsAndSearchStringAbbreviationsByCategoryId(int categoryId)
    {
        const string getAllCharacteristicsAndSearchStringAbbreviationsByCategoryIdQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getAllCharacteristicsAndSearchStringAbbreviationsByCategoryIdQuery,
            new { categoryId = categoryId });
    }

    public IEnumerable<ProductCharacteristic> GetAllCharacteristicsByCategoryId(int categoryId)
    {
        const string getAllCharacteristicsByCategoryIdQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            AND KWPrCh = 1
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getAllCharacteristicsByCategoryIdQuery,
            new { categoryId = categoryId });
    }

    public IEnumerable<ProductCharacteristic> GetAllSearchStringAbbreviationsByCategoryId(int categoryId)
    {
        const string getAllSearchStringAbbreviationsByCategoryIdQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            AND KWPrCh = 0
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getAllSearchStringAbbreviationsByCategoryIdQuery,
            new { categoryId = categoryId });
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetCharacteristicsAndSearchStringAbbreviationsForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        const string getCharacteristicsAndSearchStringAbbreviationsForSelectionOfCategoryIdsQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID IN @categoryIds
            ORDER BY S;
            """;

        IEnumerable<ProductCharacteristic> data = _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(
            getCharacteristicsAndSearchStringAbbreviationsForSelectionOfCategoryIdsQuery, new { categoryIds = categoryIds });

        return data.GroupBy(x => (int)x.CategoryId!);
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetCharacteristicsForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        const string getCharacteristicsForSelectionOfCategoryIdsQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID IN @categoryIds
            AND KWPrCh = 1
            ORDER BY S;
            """;

        IEnumerable<ProductCharacteristic> data = _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(
            getCharacteristicsForSelectionOfCategoryIdsQuery, new { categoryIds = categoryIds });

        return data.GroupBy(x => (int)x.CategoryId!);
    }

    public IEnumerable<IGrouping<int, ProductCharacteristic>> GetSearchStringAbbreviationsForSelectionOfCategoryIds(IEnumerable<int> categoryIds)
    {
        const string getSearchStringAbbreviationsForSelectionOfCategoryIdsQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID IN @categoryIds
            AND KWPrCh = 0
            ORDER BY S;
            """;

        IEnumerable<ProductCharacteristic> data = _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(
            getSearchStringAbbreviationsForSelectionOfCategoryIdsQuery, new { categoryIds = categoryIds });

        return data.GroupBy(x => (int)x.CategoryId!);
    }

    public ProductCharacteristic? GetById(uint id)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE ProductKeywordID = @id;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery,
            new { id = (int)id });
    }

    public ProductCharacteristic? GetByCategoryIdAndName(int categoryId, string name)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            AND Name = @Name;
            """;

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery,
            new { categoryId = categoryId, Name = name }).FirstOrDefault();
    }

    public IEnumerable<ProductCharacteristic> GetSelectionByCategoryIdAndNames(int categoryId, List<string> names)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            AND Name IN @Names;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Names = names,
        };

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery, parameters);
    }

    public ProductCharacteristic? GetByCategoryIdAndNameAndCharacteristicType(
        int categoryId,
        string name,
        ProductCharacteristicTypeEnum productCharacteristicType)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            AND Name = @Name
            AND KWPrCh = @productCharacteristicType;
            """;

        var parameters = new
        {
            categoryId = categoryId,
            Name = name,
            productCharacteristicType = (int)productCharacteristicType
        };
        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery, parameters)
            .FirstOrDefault();
    }

    public OneOf<uint, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(TID, Name, KeywordMeaning, S, Active, PKUserId, LastUpdate, KWPrCh)
            OUTPUT INSERTED.ProductKeywordID
            VALUES (@CategoryId, @Name, @Meaning, @DisplayOrder, @Active, @PKUserId, @LastUpdate, @KWPrCh)
            """;

        ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueNameForCreate(createRequest.Name, (uint)createRequest.CategoryId!);

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

        int? id = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertQuery, parameters);

        return (id is not null && id > 0) ? (uint)id : new UnexpectedFailureResult();
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

        ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueNameForUpdateById(updateRequest.Name, (uint)updateRequest.Id);

        if (!resultInternal.IsValid) return resultInternal;

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

        ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueNameForUpdateByNameAndCategoryId(updateRequest.NewName, (uint)updateRequest.CategoryId);

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

    private ValidationResult ValidateWhetherCharacteristicHasAUniqueNameForCreate(string? name, uint categoryId)
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
            new { categoryId = (int)categoryId, Name = name })
            .FirstOrDefault();

        if (!duplicateCharacteristicExists) return output;

            output.Errors.Add(
                new ValidationFailure(
                    nameof(ProductCharacteristic.Name),
                    "Cannot have a duplicate name value for items in the same category"));

        return output;
    }

    private ValidationResult ValidateWhetherCharacteristicHasAUniqueNameForUpdateById(string? name, uint id)
    {
        ValidationResult output = new();

        const string checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery =
            $"""
            DECLARE @originalCategoryId SMALLINT

            SELECT @originalCategoryId = TID FROM {_tableName} WHERE ProductKeywordID = @id
            
            SELECT CASE WHEN EXISTS(
                SELECT * FROM {_tableName}
                WHERE TID = @originalCategoryId
                AND Name = @Name
                AND ProductKeywordID <> @id
            ) THEN 1 ELSE 0 END;
            """;

        bool duplicateCharacteristicExists = _relationalDataAccess.GetData<bool, dynamic>(checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery,
            new { id = (int)id, Name = name })
            .FirstOrDefault();

        if (!duplicateCharacteristicExists) return output;

        output.Errors.Add(
            new ValidationFailure(
                nameof(ProductCharacteristic.Name),
                "Cannot have a duplicate name value for items in the same category"));

        return output;
    }

    private ValidationResult ValidateWhetherCharacteristicHasAUniqueNameForUpdateByNameAndCategoryId(string? newName, uint categoryId)
    {
        ValidationResult output = new();

        const string checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery =
            $"""
            SELECT CASE WHEN EXISTS(
                SELECT 1 FROM dbo.ProductKeyword
                WHERE TID = @categoryId
                AND CAST(Name AS BINARY) = CAST(@NewName AS BINARY)
            ) THEN 1 ELSE 0 END
            """;

        bool duplicateCharacteristicExists = _relationalDataAccess.GetData<bool, dynamic>(checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery,
            new { categoryId = (int)categoryId, NewName = newName })
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

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllForCategory(int categoryId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE TID = @categoryId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductCharacteristic, dynamic>(deleteQuery, new { categoryId = categoryId });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}

#pragma warning restore IDE0037 // Use inferred member name