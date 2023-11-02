using FluentValidation;
using FluentValidation.Results;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models;
using MOSTComputers.Services.DAL.Models.Requests.ProductProperty;
using MOSTComputers.Services.DAL.Models.Responses;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductPropertyRepository : RepositoryBase, IProductPropertyRepository
{
    private const string _tableName = "dbo.ProductXML";

    const string _getCharacteristicNameQuery =
        $"""
        SELECT TOP 1 Name FROM dbo.ProductKeyword
        WHERE ProductKeywordID = @ProductCharacteristicId
        """;

    const string _checkIfPropertyWithSameNameExistsQuery =
        $"""
        SELECT CASE WHEN EXISTS(
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId
            AND Keyword = @Name
        ) THEN 1 ELSE 0 END;
        """;

    public ProductPropertyRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<ProductProperty> GetAllInProduct(uint productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT CSTID AS PropertyProductId, ProductKeywordID, S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr
            FROM {_tableName}
            WHERE CSTID = @productId
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<ProductProperty, dynamic>(getAllInProductQuery, new { productId });
    }

    public ProductProperty? GetByNameAndProductId(string name, uint productId)
    {
        const string getByNameAndProductIdQuery =
            $"""
            SELECT CSTID AS PropertyProductId, ProductKeywordID, S AS PropertyDisplayOrder, Keyword, KeywordValue, Discr
            FROM {_tableName}
            WHERE CSTID = @productId
            AND Keyword = @Name;
            """;

        return _relationalDataAccess.GetData<ProductProperty, dynamic>(getByNameAndProductIdQuery, new { Name = name, productId }).FirstOrDefault();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductPropertyCreateRequest createRequest)
    {
        const string getAllInProductQuery =
            $"""
            INSERT INTO {_tableName}(CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr)
            VALUES(@ProductId, @ProductCharacteristicId, @DisplayOrder, @Name, @Value, @XmlPlacement)
            """;

        string? characteristicName = GetNameOfCharacteristic(createRequest.ProductCharacteristicId!.Value);

        ValidationResult? result = ValidateInternalConditions(createRequest, characteristicName);

        if (!result.IsValid) return result;

        var parameters = new
        {
            createRequest.ProductId,
            createRequest.ProductCharacteristicId,
            createRequest.DisplayOrder,
            Name = characteristicName,
            createRequest.Value,
            createRequest.XmlPlacement,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(getAllInProductQuery, parameters);

        return rowsAffected != 0 ? new Success() : new UnexpectedFailureResult();

        ValidationResult ValidateInternalConditions(ProductPropertyCreateRequest createRequest, string? characteristicName)
        {
            ValidationResult result = new();

            if (characteristicName == null)
            {
                result.Errors.Add(new(nameof(ProductPropertyCreateRequest.ProductCharacteristicId), "Id does not correspond to any known characteristic"));

                return result;
            }

            bool duplicatePropertyExists = DoesDuplicatePropertyExist(createRequest.ProductId, characteristicName);

            if (duplicatePropertyExists)
            {
                result.Errors.Add(new(nameof(ProductPropertyCreateRequest.ProductCharacteristicId), "There is already a property on the product for that characteristic"));

                return result;
            }

            return result;
        }
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductPropertyUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {_tableName}
            SET KeywordValue = @Value,
                S = @DisplayOrder,
                Discr = @XmlPlacement

            WHERE CSTID = @productId
            AND ProductKeywordID = @productKeywordId
            """;

        ValidationResult? result;

        string? characteristicName = GetNameOfCharacteristic(updateRequest.ProductCharacteristicId!.Value);

        result = ValidateInternalConditions(characteristicName);

        if (!result.IsValid) return result;

        var parameters = new
        {
            updateRequest.Value,
            updateRequest.DisplayOrder,
            updateRequest.XmlPlacement,
            productId = updateRequest.ProductId,
            productKeywordId = updateRequest.ProductCharacteristicId,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(updateQuery, parameters);

        return rowsAffected != 0 ? new Success() : new UnexpectedFailureResult();

        static ValidationResult ValidateInternalConditions(string? characteristicName)
        {
            ValidationResult result = new();

            if (characteristicName == null)
            {
                result.Errors.Add(new(nameof(ProductPropertyUpdateRequest.ProductCharacteristicId), "Id does not correspond to any known characteristic"));

                return result;
            }

            return result;
        }
    }

    string? GetNameOfCharacteristic(int productCharacteristicId)
    {
        return _relationalDataAccess.GetData<string, dynamic>(_getCharacteristicNameQuery, new { ProductCharacteristicId = productCharacteristicId })
            .FirstOrDefault();
    }

    bool DoesDuplicatePropertyExist(int productId, string? characteristicName)
    {
        var parametersForDuplicateQuery = new
        {
            productId,
            Name = characteristicName
        };

        bool duplicatePropertyExists = _relationalDataAccess.GetData<bool, dynamic>(_checkIfPropertyWithSameNameExistsQuery, parametersForDuplicateQuery)
            .FirstOrDefault();

        return duplicatePropertyExists;
    }

    public bool Delete(uint productId, uint characteristicId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId
            AND ProductKeywordID = @characteristicId
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(deleteByProductAndCharacteristicId, new { productId, characteristicId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllForProduct(uint productId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(deleteByProductAndCharacteristicId, new { productId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllForCharacteristic(uint characteristicId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {_tableName}
            WHERE ProductKeywordID = @characteristicId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(deleteByProductAndCharacteristicId, new { characteristicId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}