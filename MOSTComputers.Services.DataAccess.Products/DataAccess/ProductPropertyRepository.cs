using Dapper;
using FluentValidation.Results;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Common;
using MOSTComputers.Services.DataAccess.Products.Configuration;
using MOSTComputers.Services.DataAccess.Products.DataAccess.Contracts;
using MOSTComputers.Services.DataAccess.Products.Models.Requests.ProductProperty;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.ProductProperties;
using OneOf;
using OneOf.Types;
using System.Data;
using static MOSTComputers.Services.DataAccess.Products.Utils.QueryUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils;
using static MOSTComputers.Services.DataAccess.Products.Utils.TableAndColumnNameUtils.PropertiesTable;

namespace MOSTComputers.Services.DataAccess.Products.DataAccess;
internal sealed class ProductPropertyRepository : IProductPropertyRepository
{
    public ProductPropertyRepository(
        [FromKeyedServices(ConfigureServices.ConnectionStringProviderServiceKey)] IConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    private readonly IConnectionStringProvider _connectionStringProvider;

    public async Task<List<ProductProperty>> GetAllAsync()
    {
        const string getAllInProductQuery =
            $"""
            SELECT {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductCharacteristicIdColumnName},
                {DisplayOrderColumnName} AS {DisplayOrderColumnAlias},
                {CharacteristicColumnName},
                {ValueColumnName},
                {XmlPlacementColumnName}
            FROM {PropertiesTableName}
            ORDER BY {ProductIdColumnName}, {DisplayOrderColumnName};
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductProperty> data = await dbConnection.QueryAsync<ProductProperty>(getAllInProductQuery, new { }, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<List<IGrouping<int, ProductProperty>>> GetAllInProductsAsync(IEnumerable<int> productIds)
    {
        const string getAllInProductQuery =
            $"""
            SELECT {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductCharacteristicIdColumnName},
                {DisplayOrderColumnName} AS {DisplayOrderColumnAlias},
                {CharacteristicColumnName},
                {ValueColumnName},
                {XmlPlacementColumnName}
            FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} IN @productIds
            ORDER BY {DisplayOrderColumnName};
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductProperty> propertiesForProducts = await ExecuteListQueryWithParametersInChunksAsync(
            productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return dbConnection.QueryAsync<ProductProperty>(getAllInProductQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        return propertiesForProducts
            .GroupBy(x => x.ProductId)
            .ToList();
    }

    public async Task<List<ProductPropertiesForProductCountData>> GetCountOfAllInProductsAsync(IEnumerable<int> productIds)
    {
        const string getCountOfAllInProductQuery =
            $"""
            SELECT {ProductIdColumnName} AS {ProductIdColumnAlias},
                COUNT(*) AS {CountColumnName}
            FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} IN @productIds
            GROUP BY {ProductIdColumnName};
            """;

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        List<ProductPropertiesForProductCountData> propertiesForProducts = await ExecuteListQueryWithParametersInChunksAsync(
            async productIdsChunk =>
            {
                var parameters = new
                {
                    productIds = productIdsChunk
                };

                return await dbConnection.QueryAsync<ProductPropertiesForProductCountData>(getCountOfAllInProductQuery, parameters, commandType: CommandType.Text);
            },
            productIds.AsList());

        return propertiesForProducts;
    }

    public async Task<List<ProductProperty>> GetAllInProductAsync(int productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductCharacteristicIdColumnName},
                {DisplayOrderColumnName} AS {DisplayOrderColumnAlias},
                {CharacteristicColumnName},
                {ValueColumnName},
                {XmlPlacementColumnName}
            FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} = @productId
            ORDER BY {DisplayOrderColumnName};
            """;

        var parameters = new
        {
            productId = productId
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        IEnumerable<ProductProperty> data = await dbConnection.QueryAsync<ProductProperty>(getAllInProductQuery, parameters, commandType: CommandType.Text);

        return data.AsList();
    }

    public async Task<int> GetCountOfAllInProductAsync(int productId)
    {
        const string getAllInProductQuery =
            $"""
            SELECT COUNT(*) AS {CountColumnName}
            FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} = @productId;
            """;

        var parameters = new
        {
            productId = productId
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<int>(getAllInProductQuery, parameters, commandType: CommandType.Text);
    }

    public async Task<ProductProperty?> GetByProductAndCharacteristicIdAsync(int productId, int characteristicId)
    {
        const string getByNameAndProductIdQuery =
            $"""
            SELECT {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductCharacteristicIdColumnName},
                {DisplayOrderColumnName} AS {DisplayOrderColumnAlias},
                {CharacteristicColumnName},
                {ValueColumnName},
                {XmlPlacementColumnName}
            FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} = @productId
            AND {ProductCharacteristicIdColumnName} = @characteristicId;
            """;

        var parameters = new
        {
            productId = productId,
            characteristicId = characteristicId
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<ProductProperty>(getByNameAndProductIdQuery, parameters, commandType: CommandType.Text);
    }

    public async Task<ProductProperty?> GetByNameAndProductIdAsync(string name, int productId)
    {
        const string getByNameAndProductIdQuery =
            $"""
            SELECT {ProductIdColumnName} AS {ProductIdColumnAlias},
                {ProductCharacteristicIdColumnName},
                {DisplayOrderColumnName} AS {DisplayOrderColumnAlias},
                {CharacteristicColumnName},
                {ValueColumnName},
                {XmlPlacementColumnName}
            FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} = @productId
            AND {CharacteristicColumnName} = @Name;
            """;

        var parameters = new
        {
            productId = productId,
            Name = name
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        return await dbConnection.QueryFirstOrDefaultAsync<ProductProperty>(getByNameAndProductIdQuery, parameters, commandType: CommandType.Text);
    }

    public async Task<OneOf<Success, ValidationResult, UnexpectedFailureResult>> InsertAsync(ProductPropertyCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {PropertiesTableName} ({ProductIdColumnName}, {ProductCharacteristicIdColumnName}, {DisplayOrderColumnName},
                {CharacteristicColumnName}, {ValueColumnName}, {XmlPlacementColumnName})
            SELECT @ProductId, @ProductCharacteristicId, @DisplayOrder, @Name, @Value, @XmlPlacement

            WHERE NOT EXISTS (SELECT 1 FROM {PropertiesTableName} WHERE {ProductIdColumnName} = @ProductId
                AND {ProductCharacteristicIdColumnName} = @ProductCharacteristicId)

            IF @@ROWCOUNT > 0
            BEGIN
                SELECT 1;
            END
            ELSE IF EXISTS (SELECT 1 FROM {PropertiesTableName} WHERE {ProductIdColumnName} = @ProductId
                AND {ProductCharacteristicIdColumnName} = @ProductCharacteristicId)
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
            ProductCharacteristicId = createRequest.ProductCharacteristicId,
            Name = createRequest.Name,
            DisplayOrder = createRequest.DisplayOrder,
            Value = createRequest.Value,
            XmlPlacement = createRequest.XmlPlacement,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? result = await dbConnection.ExecuteScalarAsync<int?>(insertQuery, parameters, commandType: CommandType.Text, commandTimeout: 600);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetInsertValidationResult(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpdateAsync(ProductPropertyUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {PropertiesTableName}
            SET {ValueColumnName} = @Value,
                {DisplayOrderColumnName} = ISNULL(@CustomDisplayOrder, {DisplayOrderColumnName}),
                {XmlPlacementColumnName} = @XmlPlacement

            WHERE {ProductIdColumnName} = @productId
            AND {ProductCharacteristicIdColumnName} = @productKeywordId;

            SELECT @@ROWCOUNT;
            """;

        var parameters = new
        {
            productId = updateRequest.ProductId,
            productKeywordId = updateRequest.ProductCharacteristicId,
            Value = updateRequest.Value,
            CustomDisplayOrder = updateRequest.CustomDisplayOrder,
            XmlPlacement = updateRequest.XmlPlacement,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? result = await dbConnection.ExecuteScalarAsync<int?>(updateQuery, parameters, commandType: CommandType.Text);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        return new Success();
    }

    public async Task<OneOf<Success, UnexpectedFailureResult>> UpsertAsync(ProductPropertyUpsertRequest upsertRequest)
    {
        const string upsertQuery =
            $"""
            IF NOT EXISTS (SELECT 1 FROM {PropertiesTableName} WHERE {ProductIdColumnName} = @productId
                AND {ProductCharacteristicIdColumnName} = @productKeywordId)
            BEGIN
                INSERT INTO {PropertiesTableName} ({ProductIdColumnName}, {ProductCharacteristicIdColumnName}, {DisplayOrderColumnName},
                    {CharacteristicColumnName}, {ValueColumnName}, {XmlPlacementColumnName})
                SELECT @productId, @productKeywordId, @DisplayOrder, @Name, @Value, @XmlPlacement
            END
            ELSE
            BEGIN
                UPDATE {PropertiesTableName}
                SET {ValueColumnName} = @Value,
                    {DisplayOrderColumnName} = @DisplayOrder,
                    {XmlPlacementColumnName} = @XmlPlacement

                WHERE {ProductIdColumnName} = @productId
                AND {ProductCharacteristicIdColumnName} = @productKeywordId;
            END

            SELECT @@ROWCOUNT;
            """;

        var parameters = new
        {
            productId = upsertRequest.ProductId,
            productKeywordId = upsertRequest.ProductCharacteristicId,
            DisplayOrder = upsertRequest.DisplayOrder,
            Name = upsertRequest.Name,
            Value = upsertRequest.Value,
            XmlPlacement = upsertRequest.XmlPlacement,
        };

        using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

        int? result = await dbConnection.ExecuteScalarAsync<int?>(upsertQuery, parameters, commandType: CommandType.Text);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        return new Success();
    }

    public async Task<bool> DeleteAllForProductAsync(int productId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} = @productId;
            """;

        try
        {
            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            var parameters = new { productId = productId };

            int rowsAffected = await dbConnection.ExecuteAsync(deleteByProductAndCharacteristicId, parameters, commandType: CommandType.Text);

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAllForCharacteristicAsync(int characteristicId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {PropertiesTableName}
            WHERE {ProductCharacteristicIdColumnName} = @characteristicId;
            """;

        try
        {
            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            var parameters = new { characteristicId = characteristicId };

            int rowsAffected = await dbConnection.ExecuteAsync(deleteByProductAndCharacteristicId, parameters, commandType: CommandType.Text);

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int productId, int characteristicId)
    {
        const string deleteByProductAndCharacteristicId =
            $"""
            DELETE FROM {PropertiesTableName}
            WHERE {ProductIdColumnName} = @productId
            AND {ProductCharacteristicIdColumnName} = @characteristicId
            """;

        try
        {
            var parameters = new
            {
                productId = productId,
                characteristicId = characteristicId
            };

            using SqlConnection dbConnection = new(_connectionStringProvider.ConnectionString);

            int rowsAffected = await dbConnection.ExecuteAsync(deleteByProductAndCharacteristicId, parameters, commandType: CommandType.Text);

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    internal static ValidationResult GetInsertValidationResult(int result)
    {
        ValidationResult output = new();

        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductPropertyCreateRequest.ProductCharacteristicId),
                "Id does not correspond to any known characteristic"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductPropertyCreateRequest.ProductCharacteristicId),
                "There is already a property in the product for that characteristic"));
        }
        else if (result == -3)
        {
            output.Errors.Add(new(nameof(ProductPropertyCreateRequest.ProductCharacteristicId),
               "The characteristic of this property is of a different category than the product"));
        }

        return output;
    }

    internal static void AddValidationErrorsForInsertWithCharacteristicIdResult(int result, ValidationResult output, int characteristicId)
    {
        if (result == -1)
        {
            output.Errors.Add(new(nameof(ProductPropertyCreateRequest.ProductCharacteristicId),
                $"Id {characteristicId} does not correspond to any known characteristic"));
        }
        else if (result == -2)
        {
            output.Errors.Add(new(nameof(ProductPropertyCreateRequest.ProductCharacteristicId),
                $"There is already a property in the product with characteristic id {characteristicId}"));
        }
    }
}