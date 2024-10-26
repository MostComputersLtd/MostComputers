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
    private const string _categoriesTableName = "dbo.Categories";

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

    public ProductCharacteristic? GetById(int id)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE ProductKeywordID = @id;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery,
            new { id = id });
    }

    public ProductCharacteristic? GetByCategoryIdAndName(int categoryId, string name)
    {
        const string getByCategoryIdAndNameQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE TID = @categoryId
            AND Name = @Name;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery,
            new { categoryId = categoryId, Name = name });
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

    public IEnumerable<ProductCharacteristic> GetSelectionByIds(List<int> ids)
    {
        const string getSelectionByIdsQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE ProductKeywordID IN @ids;
            """;

        var parameters = new
        {
            ids = ids,
        };

        return _relationalDataAccess.GetData<ProductCharacteristic, dynamic>(getSelectionByIdsQuery, parameters);
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
        return _relationalDataAccess.GetDataFirstOrDefault<ProductCharacteristic, dynamic>(getByCategoryIdAndNameQuery, parameters);
    }

    public OneOf<int, ValidationResult, UnexpectedFailureResult> Insert(ProductCharacteristicCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @InsertedIdTable TABLE (Id INT);

            INSERT INTO {_tableName}(TID, Name, KeywordMeaning, S, Active, PKUserId, LastUpdate, KWPrCh)
            OUTPUT INSERTED.ProductKeywordID INTO @InsertedIdTable
            SELECT @CategoryId, @Name, @Meaning, @DisplayOrder, @Active, @PKUserId, @LastUpdate, @KWPrCh
            WHERE EXISTS (SELECT 1 FROM {_categoriesTableName} WHERE CategoryID = @CategoryId)
            AND NOT EXISTS (SELECT 1 FROM {_tableName} WHERE TID = @CategoryId AND Name = @Name);

            IF EXISTS (SELECT TOP 1 Id FROM @InsertedIdTable)
            BEGIN
                SELECT TOP 1 Id FROM @InsertedIdTable;
            END
            ELSE
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM {_categoriesTableName} WHERE CategoryID = @CategoryId)
                BEGIN
                    SELECT -1;
                END
                ELSE IF EXISTS (SELECT 1 FROM {_tableName} WHERE TID = @CategoryId AND Name = @Name)
                BEGIN
                    SELECT -2;
                END
                ELSE
                BEGIN
                    SELECT 0;
                END
            END
            """;
        
        //ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueNameForCreate(createRequest.Name, createRequest.CategoryId!.Value);

        //if (!resultInternal.IsValid) return resultInternal;

        var parameters = new
        {
            CategoryId = createRequest.CategoryId,
            Name = createRequest.Name,
            Meaning = createRequest.Meaning,
            DisplayOrder = createRequest.DisplayOrder,
            Active = createRequest.Active,
            PKUserId = createRequest.PKUserId,
            LastUpdate = createRequest.LastUpdate,
            KWPrCh = createRequest.KWPrCh,
        };

        int? idOrFailureResult = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertQuery, parameters);

        if (idOrFailureResult is null || idOrFailureResult == 0) return new UnexpectedFailureResult();

        if (idOrFailureResult > 0) return idOrFailureResult.Value;

        ValidationResult validationResult = GetValidationResultFromFailedInsert(idOrFailureResult.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    private static ValidationResult GetValidationResultFromFailedInsert(int result)
    {
        ValidationResult output = new();

        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductCharacteristicCreateRequest.CategoryId), "Id does not correspond to any known category"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductCharacteristicCreateRequest.Name), "Cannot have a duplicate name value for items in the same category"));
        }

        return output;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateById(ProductCharacteristicByIdUpdateRequest updateRequest)
    {
        const string updateByIdQuery =
            $"""
            DECLARE @originalCategoryId SMALLINT;

            SELECT TOP 1 @originalCategoryId = TID FROM {_tableName} WHERE ProductKeywordID = @id;

            UPDATE {_tableName}
            SET Name = @Name,
                KeywordMeaning = @Meaning,
                S = @DisplayOrder,
                Active = @Active,
                PKUserID = @PKUserId,
                LastUpdate = @LastUpdate,
                KWPrCh = @KWPrCh

            WHERE ProductKeywordID = @id
            AND NOT EXISTS (SELECT 1 FROM {_tableName} WHERE TID = @originalCategoryId AND Name = @Name AND ProductKeywordID <> @id)

            IF @@ROWCOUNT > 0
            BEGIN
                SELECT 1;
            END
            ELSE IF NOT EXISTS (SELECT 1 FROM {_tableName} WHERE ProductKeywordID = @id)
            BEGIN
                SELECT -1;
            END
            ELSE IF EXISTS (SELECT 1 FROM {_tableName} WHERE TID = @originalCategoryId AND Name = @Name AND ProductKeywordID <> @id)
            BEGIN
                SELECT -2;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;

        //ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueNameForUpdateById(updateRequest.Name, updateRequest.Id);

        //if (!resultInternal.IsValid) return resultInternal;

        var parameters = new
        {
            id = updateRequest.Id,
            Name = updateRequest.Name,
            Meaning = updateRequest.Meaning,
            DisplayOrder = updateRequest.DisplayOrder,
            Active = updateRequest.Active,
            PKUserId = updateRequest.PKUserId,
            LastUpdate = updateRequest.LastUpdate,
            KWPrCh = updateRequest.KWPrCh,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateByIdQuery, parameters);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetValidationResultFromFailedUpdateById(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
        ;
    }

    private static ValidationResult GetValidationResultFromFailedUpdateById(int result)
    {
        ValidationResult output = new();

        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductCharacteristicByIdUpdateRequest.Id), "Id does not correspond to any known product characteristic"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductCharacteristicByIdUpdateRequest.Name), "Cannot have a duplicate name value for items in the same category"));
        }

        return output;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByNameAndCategoryId(ProductCharacteristicByNameAndCategoryIdUpdateRequest updateRequest)
    {
        const string updateByNameAndCategoryIdQuery =
            $"""
            DECLARE @originalId SMALLINT

            SELECT @originalId = ProductKeywordID FROM {_tableName} WHERE TID = @categoryId AND Name = @OldName;

            UPDATE {_tableName}
            SET Name = @Name,
                KeywordMeaning = @Meaning,
                S = @DisplayOrder,
                Active = @Active,
                PKUserID = @PKUserId,
                LastUpdate = @LastUpdate,
                KWPrCh = @KWPrCh

            WHERE TID = @categoryId
            AND Name = @OldName
            AND NOT EXISTS (SELECT 1 FROM {_tableName} WHERE TID = @categoryId AND Name = @Name AND ProductKeywordID <> @originalId)

            IF @@ROWCOUNT > 0
            BEGIN
                SELECT 1;
            END
            ELSE IF NOT EXISTS (SELECT 1 FROM {_tableName} WHERE TID = @categoryId AND Name = @OldName)
            BEGIN
                SELECT -1;
            END
            ELSE IF EXISTS (SELECT 1 FROM {_tableName} WHERE TID = @categoryId AND Name = @Name AND ProductKeywordID <> @originalId)
            BEGIN
                SELECT -2;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;

        //ValidationResult resultInternal = ValidateWhetherCharacteristicHasAUniqueNameForUpdateByNameAndCategoryId(updateRequest.NewName, updateRequest.CategoryId);

        //if (!resultInternal.IsValid) return resultInternal;

        var parameters = new
        {
            categoryId = updateRequest.CategoryId,
            OldName = updateRequest.Name,
            Name = updateRequest.NewName,
            Meaning = updateRequest.Meaning,
            DisplayOrder = updateRequest.DisplayOrder,
            Active = updateRequest.Active,
            PKUserId = updateRequest.PKUserId,
            LastUpdate = updateRequest.LastUpdate,
            KWPrCh = updateRequest.KWPrCh,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateByNameAndCategoryIdQuery, parameters);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetValidationResultFromFailedUpdateByNameAndCategoryId(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    private static ValidationResult GetValidationResultFromFailedUpdateByNameAndCategoryId(int result)
    {
        ValidationResult output = new();

        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductCharacteristicByNameAndCategoryIdUpdateRequest.Name),
                "Name does not correspond to any known product characteristic"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductCharacteristicByNameAndCategoryIdUpdateRequest.NewName),
                "Cannot have a duplicate name value for items in the same category"));
        }

        return output;
    }

    //private ValidationResult ValidateWhetherCharacteristicHasAUniqueNameForCreate(string? name, int categoryId)
    //{
    //    ValidationResult output = new();

    //    const string checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery =
    //        $"""
    //        SELECT CASE WHEN EXISTS(
    //            SELECT * FROM {_tableName}
    //            WHERE TID = @categoryId
    //            AND Name = @Name
    //        ) THEN 1 ELSE 0 END;
    //        """;

    //    bool duplicateCharacteristicExists = _relationalDataAccess.GetDataFirstOrDefault<bool, dynamic>(checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery, 
    //        new { categoryId = categoryId, Name = name });

    //    if (!duplicateCharacteristicExists) return output;

    //        output.Errors.Add(
    //            new ValidationFailure(
    //                nameof(ProductCharacteristic.Name),
    //                "Cannot have a duplicate name value for items in the same category"));

    //    return output;
    //}

    //private ValidationResult ValidateWhetherCharacteristicHasAUniqueNameForUpdateById(string? name, int id)
    //{
    //    ValidationResult output = new();

    //    const string checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery =
    //        $"""
    //        DECLARE @originalCategoryId SMALLINT

    //        SELECT @originalCategoryId = TID FROM {_tableName} WHERE ProductKeywordID = @id
            
    //        SELECT CASE WHEN EXISTS(
    //            SELECT * FROM {_tableName}
    //            WHERE TID = @originalCategoryId
    //            AND Name = @Name
    //            AND ProductKeywordID <> @id
    //        ) THEN 1 ELSE 0 END;
    //        """;

    //    bool duplicateCharacteristicExists = _relationalDataAccess.GetDataFirstOrDefault<bool, dynamic>(checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery,
    //        new { id = id, Name = name });

    //    if (!duplicateCharacteristicExists) return output;

    //    output.Errors.Add(
    //        new ValidationFailure(
    //            nameof(ProductCharacteristic.Name),
    //            "Cannot have a duplicate name value for items in the same category"));

    //    return output;
    //}

    //private ValidationResult ValidateWhetherCharacteristicHasAUniqueNameForUpdateByNameAndCategoryId(string? newName, int categoryId)
    //{
    //    ValidationResult output = new();

    //    const string checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery =
    //        $"""
    //        SELECT CASE WHEN EXISTS(
    //            SELECT 1 FROM {_tableName}
    //            WHERE TID = @categoryId
    //            AND CAST(Name AS BINARY) = CAST(@NewName AS BINARY)
    //        ) THEN 1 ELSE 0 END
    //        """;

    //    bool duplicateCharacteristicExists = _relationalDataAccess.GetDataFirstOrDefault<bool, dynamic>(checkIfACharacteristicWithTheSameCategoryIdAndNameExistsQuery,
    //        new { categoryId = categoryId, NewName = newName });

    //    if (!duplicateCharacteristicExists) return output;

    //    output.Errors.Add(
    //        new ValidationFailure(
    //            nameof(ProductCharacteristic.Name),
    //            "Cannot have a duplicate name value for items in the same category"));

    //    return output;
    //}

    public bool Delete(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE ProductKeywordID = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, new { id = id });

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
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, new { categoryId = categoryId });

            if (rowsAffected <= 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

#pragma warning restore IDE0037 // Use inferred member name
}