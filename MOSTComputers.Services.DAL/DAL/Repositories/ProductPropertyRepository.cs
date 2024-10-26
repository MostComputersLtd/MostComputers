using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductPropertyRepository : RepositoryBase, IProductPropertyRepository
{
    private const string _tableName = "dbo.ProductXML";
    private const string _productCharacteristicsTableName = "dbo.ProductKeyword";
    private const string _productsTableName = "dbo.MOSTPrices";

    //private const string _getCharacteristicNameQuery =
    //    $"""
    //    SELECT TOP 1 Name FROM {_productCharacteristicTableName}
    //    WHERE ProductKeywordID = @ProductCharacteristicId
    //    """;

    //private const string _checkIfPropertyWithSameNameExistsByNameQuery =
    //    $"""
    //    SELECT CASE WHEN EXISTS(
    //        SELECT * FROM {_tableName}
    //        WHERE CSTID = @productId
    //        AND Keyword COLLATE SQL_Latin1_General_CP1_CS_AS = @Name
    //    ) THEN 1 ELSE 0 END;
    //    """;

    //private const string _checkIfPropertyWithSameNameExistsByIdQuery =
    //    $"""
    //    SELECT CASE WHEN EXISTS(
    //        SELECT * FROM {_tableName}
    //        WHERE CSTID = @productId
    //        AND ProductKeywordID = @characteristicId
    //    ) THEN 1 ELSE 0 END;

    //    """;

    internal const string insertWithCharacteristicIdQuery =
        $"""
        DECLARE @DefaultDisplayOrder INT, @Name VARCHAR(50);

        SELECT @DefaultDisplayOrder = S, @Name = Name
        FROM {_productCharacteristicsTableName}
        WHERE ProductKeywordID = @ProductCharacteristicId;

        INSERT INTO {_tableName} (CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr)
        SELECT @ProductId, @ProductCharacteristicId, ISNULL(@CustomDisplayOrder, @DefaultDisplayOrder), @Name, @Value, @XmlPlacement

        WHERE EXISTS (SELECT 1 FROM {_productCharacteristicsTableName} WHERE ProductKeywordID = @ProductCharacteristicId)
        AND NOT EXISTS (SELECT 1 FROM {_tableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)

        IF @@ROWCOUNT > 0
        BEGIN
            SELECT 1;
        END
        ELSE IF NOT EXISTS (SELECT 1 FROM {_productCharacteristicsTableName} WHERE ProductKeywordID = @ProductCharacteristicId)
        BEGIN
            SELECT -1;
        END
        ELSE IF EXISTS (SELECT 1 FROM {_tableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)
        BEGIN
            SELECT -2;
        END
        ELSE
        BEGIN
            SELECT 0;
        END
        """;

    public ProductPropertyRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<ProductProperty> GetAllInProduct(int productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT CSTID AS PropertyProductId, ProductKeywordID, S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr
            FROM {_tableName}
            WHERE CSTID = @productId
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<ProductProperty, dynamic>(getAllInProductQuery, new { productId = productId });
    }

    public ProductProperty? GetByNameAndProductId(string name, int productId)
    {
        const string getByNameAndProductIdQuery =
            $"""
            SELECT CSTID AS PropertyProductId, ProductKeywordID, S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr
            FROM {_tableName}
            WHERE CSTID = @productId
            AND Keyword = @Name;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductProperty, dynamic>(getByNameAndProductIdQuery,
            new { productId = productId, Name = name });
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(ProductPropertyByCharacteristicIdCreateRequest createRequest)
    {
        

        //string? characteristicName = GetNameOfCharacteristic(createRequest.ProductCharacteristicId);

        //ValidationResult? result = ValidateInternalConditions(createRequest, characteristicName);

        //if (!result.IsValid) return result;

        var parameters = new
        {
            ProductId = createRequest.ProductId,
            ProductCharacteristicId = createRequest.ProductCharacteristicId,
            CustomDisplayOrder = createRequest.CustomDisplayOrder,
            Value = createRequest.Value,
            XmlPlacement = createRequest.XmlPlacement,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertWithCharacteristicIdQuery, parameters);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = RepositoryCommonElements.GetValidationResultFromFailedInsertWithCharacteristicIdResult(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(ProductPropertyByCharacteristicNameCreateRequest createRequest)
    {
        const string insertWithCharacteristicNameQuery =
            $"""
            DECLARE @CategoryId INT

            DECLARE @DefaultDisplayOrder INT;
            DECLARE @ProductCharacteristicId INT;

            SELECT TOP 1 @CategoryId = TID FROM {_productsTableName}
            WHERE CSTID = @ProductId;

            SELECT @ProductCharacteristicId = ProductKeywordID, @DefaultDisplayOrder = S
            FROM {_productCharacteristicsTableName}

            WHERE TID = @CategoryId
            AND Name = @Name;

            INSERT INTO {_tableName} (CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr)
            SELECT @ProductId, @ProductCharacteristicId, ISNULL(@CustomDisplayOrder, @DefaultDisplayOrder), @Name, @Value, @XmlPlacement

            WHERE EXISTS (SELECT 1 FROM {_productCharacteristicsTableName} WHERE TID = @CategoryId AND Name = @Name)
            AND NOT EXISTS (SELECT 1 FROM {_tableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)

            IF @@ROWCOUNT > 0
            BEGIN
                SELECT 1;
            END
            ELSE IF NOT EXISTS (SELECT 1 FROM {_productCharacteristicsTableName} WHERE TID = @CategoryId AND Name = @Name)
            BEGIN
                SELECT -1;
            END
            ELSE IF EXISTS (SELECT 1 FROM {_tableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)
            BEGIN
                SELECT -2;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;

        //const string getIdOfCharacteristicByNameAndProductIdQuery =
        //    $"""
        //    DECLARE @CategoryId INT

        //    SELECT TOP 1 @CategoryId = TID FROM {_productsTableName}
        //    WHERE CSTID = @productId;

        //    SELECT TOP 1 ProductKeywordID FROM {_productCharacteristicTableName}
        //    WHERE TID = @CategoryId
        //    AND Name = @Name;
        //    """;

        //var parametersLocal = new
        //{
        //    productId = createRequest.ProductId,
        //    Name = createRequest.ProductCharacteristicName
        //};

        //int? characteristicId = _relationalDataAccess.GetDataFirstOrDefault<int, dynamic>(getIdOfCharacteristicByNameAndProductIdQuery, parametersLocal);

        //ValidationResult? result = ValidateInternalConditions(createRequest, characteristicId);

        //if (!result.IsValid) return result;

        var parameters = new
        {
            ProductId = createRequest.ProductId,
            Name = createRequest.ProductCharacteristicName,
            CustomDisplayOrder = createRequest.CustomDisplayOrder,
            Value = createRequest.Value,
            XmlPlacement = createRequest.XmlPlacement,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertWithCharacteristicNameQuery, parameters);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetValidationResultFromFailedInsertWithCharacteristicNameResult(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    private static ValidationResult GetValidationResultFromFailedInsertWithCharacteristicNameResult(int result)
    {
        ValidationResult output = new();

        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductPropertyByCharacteristicNameCreateRequest.ProductCharacteristicName),
                "Name does not correspond to any known characteristic"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductPropertyByCharacteristicNameCreateRequest.ProductCharacteristicName),
                "There is already a property in the product for that characteristic"));
        }

        return output;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            IF EXISTS (SELECT 1 FROM {_productCharacteristicsTableName} WHERE ProductKeywordID = @productKeywordId)
            BEGIN
                UPDATE {_tableName}
                SET KeywordValue = @Value,
                    S = ISNULL(@CustomDisplayOrder, S),
                    Discr = @XmlPlacement

                WHERE CSTID = @productId
                AND ProductKeywordID = @productKeywordId;

                SELECT @@ROWCOUNT;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        //ValidationResult? result;

        //string? characteristicName = GetNameOfCharacteristic(updateRequest.ProductCharacteristicId);

        //result = ValidateInternalConditions(characteristicName);

        //if (!result.IsValid) return result;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            productKeywordId = updateRequest.ProductCharacteristicId,
            Value = updateRequest.Value,
            CustomDisplayOrder = updateRequest.CustomDisplayOrder,
            XmlPlacement = updateRequest.XmlPlacement,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateQuery, parameters);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetValidationResultFromFailedUpdateResult(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    private static ValidationResult GetValidationResultFromFailedUpdateResult(int result)
    {
        ValidationResult output = new();

        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductPropertyUpdateRequest.ProductCharacteristicId), "Id does not correspond to any known characteristic"));
        }

        return output;
    }

    //string? GetNameOfCharacteristic(int productCharacteristicId)
    //{
    //    return _relationalDataAccess.GetDataFirstOrDefault<string, dynamic>(_getCharacteristicNameQuery,
    //        new { ProductCharacteristicId = productCharacteristicId });
    //}

    //bool DoesDuplicatePropertyExist(int productId, int? characteristicId)
    //{
    //    var parametersForDuplicateQuery = new
    //    {
    //        productId = productId,
    //        characteristicId = characteristicId
    //    };

    //    bool duplicatePropertyExists = _relationalDataAccess.GetDataFirstOrDefault<bool, dynamic>(_checkIfPropertyWithSameNameExistsByIdQuery,
    //        parametersForDuplicateQuery);

    //    return duplicatePropertyExists;
    //}

    public bool Delete(int productId, int characteristicId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId
            AND ProductKeywordID = @characteristicId
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByProductAndCharacteristicId,
                new { productId = productId, characteristicId = characteristicId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllForProduct(int productId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByProductAndCharacteristicId,
                new { productId = productId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllForCharacteristic(int characteristicId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {_tableName}
            WHERE ProductKeywordID = @characteristicId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteByProductAndCharacteristicId,
                new { characteristicId = characteristicId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
#pragma warning restore IDE0037 // Use inferred member name
}