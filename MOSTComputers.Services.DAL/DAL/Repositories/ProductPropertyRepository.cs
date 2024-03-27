using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductProperty;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;
using System.Reflection.PortableExecutable;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductPropertyRepository : RepositoryBase, IProductPropertyRepository
{
    private const string _tableName = "dbo.ProductXML";
    private const string _productCharacteristicTableName = "dbo.ProductKeyword";
    private const string _productsTableName = "dbo.MOSTPrices";

    const string _getCharacteristicNameQuery =
        $"""
        SELECT TOP 1 Name FROM {_productCharacteristicTableName}
        WHERE ProductKeywordID = @ProductCharacteristicId
        """;

    const string _checkIfPropertyWithSameNameExistsByNameQuery =
        $"""
        SELECT CASE WHEN EXISTS(
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId
            AND Keyword COLLATE SQL_Latin1_General_CP1_CS_AS = @Name
        ) THEN 1 ELSE 0 END;
    
        """;

    const string _checkIfPropertyWithSameNameExistsByIdQuery =
        $"""
        SELECT CASE WHEN EXISTS(
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId
            AND ProductKeywordID = @characteristicId
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

        return _relationalDataAccess.GetData<ProductProperty, dynamic>(getAllInProductQuery, new { productId = (int)productId });
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

        return _relationalDataAccess.GetData<ProductProperty, dynamic>(getByNameAndProductIdQuery, new { Name = name, productId = (int)productId })
            .FirstOrDefault();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicId(ProductPropertyByCharacteristicIdCreateRequest createRequest)
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

        ValidationResult ValidateInternalConditions(ProductPropertyByCharacteristicIdCreateRequest createRequest, string? characteristicName)
        {
            ValidationResult result = new();

            if (characteristicName == null)
            {
                result.Errors.Add(new(nameof(ProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId), "Id does not correspond to any known characteristic"));

                return result;
            }

            bool duplicatePropertyExists = DoesDuplicatePropertyExist(createRequest.ProductId, characteristicName);

            if (duplicatePropertyExists)
            {
                result.Errors.Add(new(nameof(ProductPropertyByCharacteristicIdCreateRequest.ProductCharacteristicId), "There is already a property on the product for that characteristic"));

                return result;
            }

            return result;
        }
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> InsertWithCharacteristicName(ProductPropertyByCharacteristicNameCreateRequest createRequest)
    {
        const string getAllInProductQuery =
            $"""
            INSERT INTO {_tableName}(CSTID, ProductKeywordID, S, Keyword, KeywordValue, Discr)
            VALUES(@ProductId, @ProductCharacteristicId, @DisplayOrder, @Name, @Value, @XmlPlacement)
            """;

        const string getIdOfCharacteristicByNameAndProductId =
            $"""
            DECLARE @CategoryId INT

            SELECT TOP 1 @CategoryId = TID FROM {_productsTableName}
            WHERE CSTID = @productId;

            SELECT TOP 1 ProductKeywordID FROM {_productCharacteristicTableName}
            WHERE TID = @CategoryId
            AND Name = @Name;
            """;

        var parametersLocal = new
        {
            productId = createRequest.ProductId,
            Name = createRequest.ProductCharacteristicName
        };

        int? characteristicId = _relationalDataAccess.GetDataFirstOrDefault<int, dynamic>(getIdOfCharacteristicByNameAndProductId, parametersLocal);

        ValidationResult? result = ValidateInternalConditions(createRequest, characteristicId);

        if (!result.IsValid) return result;

        var parameters = new
        {
            createRequest.ProductId,
            ProductCharacteristicId = characteristicId,
            Name = createRequest.ProductCharacteristicName,
            createRequest.DisplayOrder,
            createRequest.Value,
            createRequest.XmlPlacement,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(getAllInProductQuery, parameters);

        return rowsAffected != 0 ? new Success() : new UnexpectedFailureResult();

        ValidationResult ValidateInternalConditions(ProductPropertyByCharacteristicNameCreateRequest createRequest, int? characteristicId)
        {
            ValidationResult result = new();

            if (characteristicId is null
                || characteristicId <= 0)
            {
                result.Errors.Add(new(nameof(ProductPropertyByCharacteristicNameCreateRequest.ProductCharacteristicName), "Name does not correspond to any known characteristic"));

                return result;
            }

            bool duplicatePropertyExists = DoesDuplicatePropertyExist(createRequest.ProductId, characteristicId);

            if (duplicatePropertyExists)
            {
                result.Errors.Add(new(nameof(ProductPropertyByCharacteristicNameCreateRequest.ProductCharacteristicName), "There is already a property on the product for that characteristic"));

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

        bool duplicatePropertyExists = _relationalDataAccess.GetData<bool, dynamic>(_checkIfPropertyWithSameNameExistsByNameQuery, parametersForDuplicateQuery)
            .FirstOrDefault();

        return duplicatePropertyExists;
    }

    bool DoesDuplicatePropertyExist(int productId, int? characteristicId)
    {
        var parametersForDuplicateQuery = new
        {
            productId,
            characteristicId
        };

        bool duplicatePropertyExists = _relationalDataAccess.GetData<bool, dynamic>(_checkIfPropertyWithSameNameExistsByIdQuery, parametersForDuplicateQuery)
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
            int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(deleteByProductAndCharacteristicId,
                new { productId = (int)productId, characteristicId = (int)characteristicId });

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
            int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(deleteByProductAndCharacteristicId, new { productId = (int)productId });

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
            int rowsAffected = _relationalDataAccess.SaveData<ProductProperty, dynamic>(deleteByProductAndCharacteristicId, new { characteristicId = (int)characteristicId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}