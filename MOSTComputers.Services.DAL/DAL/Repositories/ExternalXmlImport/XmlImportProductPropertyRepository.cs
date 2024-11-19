using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductProperty;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories.ExternalXmlImport;

internal sealed class XmlImportProductPropertyRepository : RepositoryBase, IXmlImportProductPropertyRepository
{
    internal const string insertWithCharacteristicIdQuery =
        $"""
        DECLARE @DefaultDisplayOrder INT, @Name VARCHAR(50);

        SELECT @DefaultDisplayOrder = S, @Name = Name
        FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}
        WHERE ProductKeywordID = @ProductCharacteristicId;

        INSERT INTO {PropertiesTableName} (CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr, XmlName, XmlDisplayOrder)
        SELECT @ProductId, @ProductCharacteristicId, ISNULL(@CustomDisplayOrder, @DefaultDisplayOrder), @Name, @Value, @XmlPlacement,
        @XmlName, @XmlDisplayOrder

        WHERE NOT EXISTS (SELECT 1 FROM {PropertiesTableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)

        IF @@ROWCOUNT > 0
        BEGIN
            SELECT 1;
        END
        ELSE IF EXISTS (SELECT 1 FROM {PropertiesTableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)
        BEGIN
            SELECT -2;
        END
        ELSE
        BEGIN
            SELECT 0;
        END
        """;

    public XmlImportProductPropertyRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<XmlImportProductProperty> GetAllInProduct(int productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT CSTID AS PropertyProductId, ProductKeywordID, S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr, XmlName, XmlDisplayOrder
            FROM {PropertiesTableName}
            WHERE CSTID = @productId
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<XmlImportProductProperty, dynamic>(getAllInProductQuery, new { productId = productId });
    }

    public XmlImportProductProperty? GetByNameAndProductId(string name, int productId)
    {
        const string getByNameAndProductIdQuery =
            $"""
            SELECT TOP 1 CSTID AS PropertyProductId, ProductKeywordID, S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr, XmlName, XmlDisplayOrder
            FROM {PropertiesTableName}
            WHERE CSTID = @productId
            AND Keyword = @Name;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<XmlImportProductProperty, dynamic>(getByNameAndProductIdQuery,
            new { productId = productId, Name = name });
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(
        XmlImportProductPropertyByCharacteristicIdCreateRequest createRequest)
    {
        var parameters = new
        {
            ProductId = createRequest.ProductId,
            ProductCharacteristicId = createRequest.ProductCharacteristicId,
            CustomDisplayOrder = createRequest.CustomDisplayOrder,
            Value = createRequest.Value,
            XmlPlacement = createRequest.XmlPlacement,
            XmlName = createRequest.XmlName,
            XmlDisplayOrder = createRequest.XmlDisplayOrder,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertWithCharacteristicIdQuery, parameters);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = RepositoryCommonElements.GetValidationResultFromFailedInsertWithCharacteristicIdResult(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(
        XmlImportProductPropertyByCharacteristicNameCreateRequest createRequest)
    {
        const string insertWithCharacteristicNameQuery =
            $"""
            DECLARE @CategoryId INT

            DECLARE @DefaultDisplayOrder INT;
            DECLARE @ProductCharacteristicId INT;

            SELECT TOP 1 @CategoryId = TID FROM {ProductsTableName}
            WHERE CSTID = @ProductId;

            SELECT @ProductCharacteristicId = ProductKeywordID, @DefaultDisplayOrder = S
            FROM {ProductCharacteristicAndExternalXmlDataRelationsTableName}

            WHERE TID = @CategoryId
            AND Name = @Name;

            INSERT INTO {PropertiesTableName} (CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr, XmlName, XmlDisplayOrder)
            SELECT @ProductId, @ProductCharacteristicId, ISNULL(@CustomDisplayOrder, @DefaultDisplayOrder), @Name, @Value, @XmlPlacement,
                @XmlName, @XmlDisplayOrder

            WHERE NOT EXISTS (SELECT 1 FROM {PropertiesTableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)

            IF @@ROWCOUNT > 0
            BEGIN
                SELECT 1;
            END
            ELSE IF EXISTS (SELECT 1 FROM {PropertiesTableName} WHERE CSTID = @ProductId AND ProductKeywordID = @ProductCharacteristicId)
            BEGIN
                SELECT -2;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;

        var parameters = new
        {
            ProductId = createRequest.ProductId,
            Name = createRequest.ProductCharacteristicName,
            CustomDisplayOrder = createRequest.CustomDisplayOrder,
            Value = createRequest.Value,
            XmlPlacement = createRequest.XmlPlacement,
            XmlName = createRequest.XmlName,
            XmlDisplayOrder = createRequest.XmlDisplayOrder,
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

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByXmlData(XmlImportProductPropertyUpdateByXmlDataRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {PropertiesTableName}
            SET KeywordValue = @Value,
                S = ISNULL(@CustomDisplayOrder, S),
                Discr = @XmlPlacement,
                ProductKeywordID = @productKeywordId,
                XmlName = ISNULL(@NewXmlName, XmlName),
                XmlDisplayOrder = ISNULL(@NewXmlDisplayOrder, XmlDisplayOrder)

            WHERE CSTID = @productId
            AND XmlName = @XmlName
            AND XmlDisplayOrder = @XmlDisplayOrder;

            SELECT @@ROWCOUNT;
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            productKeywordId = updateRequest.ProductCharacteristicId,
            Value = updateRequest.Value,
            CustomDisplayOrder = updateRequest.CustomDisplayOrder,
            XmlPlacement = updateRequest.XmlPlacement,
            XmlName = updateRequest.XmlName,
            XmlDisplayOrder = updateRequest.XmlDisplayOrder,
            NewXmlName = updateRequest.NewXmlName,
            NewXmlDisplayOrder = updateRequest.NewXmlDisplayOrder,
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

    public bool Delete(int productId, int characteristicId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {PropertiesTableName}
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
            DELETE FROM {PropertiesTableName}
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
            DELETE FROM {PropertiesTableName}
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