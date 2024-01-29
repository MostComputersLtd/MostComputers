using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.ProductImageFileNameInfo;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductImageFileNameInfoRepository : RepositoryBase, IProductImageFileNameInfoRepository
{
    private const string _tableName = "dbo.ImageFileName";
    private const string _productsTableName = "dbo.MOSTPrices";

    public ProductImageFileNameInfoRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<ProductImageFileNameInfo> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {_tableName}
            ORDER BY CSTID, ImgNo;
            """;

        return _relationalDataAccess.GetData<ProductImageFileNameInfo, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllForProduct(uint productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId
            ORDER BY ImgNo;
            """;

        return _relationalDataAccess.GetData<ProductImageFileNameInfo, dynamic>(getAllForProductQuery, new { productId = (int)productId });
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @DisplayOrderInRange INT, @MaxDisplayOrderForProduct INT

            SELECT TOP 1 @MaxDisplayOrderForProduct = MAX(ImgNo) + 1 FROM {_tableName}
            WHERE CSTID = @productId;

            SELECT TOP 1 @DisplayOrderInRange = ISNULL(
                (SELECT TOP 1 @MaxDisplayOrderForProduct FROM {_tableName}
                WHERE @MaxDisplayOrderForProduct <= @DisplayOrder),
                @DisplayOrder);

            UPDATE {_tableName}
                SET ImgNo = ImgNo + 1
            WHERE CSTID = @productId
            AND ImgNo >= @DisplayOrderInRange;

            INSERT INTO {_tableName}(CSTID, ImgNo, ImgFileName, Active)
            VALUES (@productId, @DisplayOrderInRange, @FileName, @Active)
            """;


        ValidationResult result = ValidateWhetherProductWithGivenIdExists((uint)createRequest.ProductId);

        if (!result.IsValid) return result;

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.DisplayOrder,
            createRequest.FileName,
            createRequest.Active
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductImageFileNameInfo, dynamic>(insertQuery, parameters, doInTransaction: true);

        return (rowsAffected > 0) ? new Success() : new UnexpectedFailureResult();
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Update(ProductImageFileNameInfoUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            SET @NewDisplayOrder = 
            CASE 
                WHEN @NewDisplayOrder < 1 THEN 1
                WHEN @NewDisplayOrder > ISNULL((SELECT MAX(ImgNo) FROM ImageFileName WHERE CSTID = @productId), 1)
                    THEN ISNULL((SELECT MAX(ImgNo) FROM ImageFileName WHERE CSTID = @productId), 1)
                ELSE @NewDisplayOrder
            END;
            
            UPDATE {_tableName}
            SET ImgNo = 
                CASE
                    WHEN ImgNo = @DisplayOrder THEN @NewDisplayOrder
                    WHEN @DisplayOrder < @NewDisplayOrder AND ImgNo > @DisplayOrder AND ImgNo <= @NewDisplayOrder THEN ImgNo - 1
                    WHEN @DisplayOrder > @NewDisplayOrder AND ImgNo < @DisplayOrder AND ImgNo >= @NewDisplayOrder THEN ImgNo + 1
                    ELSE ImgNo
                END
            WHERE CSTID = @productId;
            
            UPDATE {_tableName}
            SET ImgFileName = @FileName,
                Active = @Active

            WHERE CSTID = @productId
            AND ImgNo = @NewDisplayOrder;
            """;

        const string updateQueryWithNoDisplayOrderChanges =
            $"""
            UPDATE {_tableName}
            SET ImgFileName = @FileName,
                Active = @Active

            WHERE CSTID = @productId
            AND ImgNo = @DisplayOrder;
            """;

        ValidationResult internalValidationResult = ValidateWhetherProductWithGivenIdExists((uint)updateRequest.ProductId);

        if (!internalValidationResult.IsValid) return internalValidationResult;

        if (updateRequest.NewDisplayOrder is null
            || updateRequest.NewDisplayOrder == updateRequest.DisplayOrder)
        {
            var parametersSimple = new
            {
                productId = updateRequest.ProductId,
                updateRequest.DisplayOrder,
                updateRequest.FileName,
                updateRequest.Active,
            };

            int rowsAffectedSimple = _relationalDataAccess.SaveData<ProductImageFileNameInfo, dynamic>(updateQueryWithNoDisplayOrderChanges, parametersSimple, doInTransaction: true);

            return (rowsAffectedSimple > 0) ? new Success() : new UnexpectedFailureResult();
        }

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.DisplayOrder,
            updateRequest.NewDisplayOrder,
            updateRequest.FileName,
            updateRequest.Active,
        };

        int rowsAffected = _relationalDataAccess.SaveData<ProductImageFileNameInfo, dynamic>(updateQuery, parameters, doInTransaction: true);

        return (rowsAffected > 0) ? new Success() : new UnexpectedFailureResult();
    }

    private ValidationResult ValidateWhetherProductWithGivenIdExists(uint productId, string propertyName = "ProductId")
    {
        const string checkWhetherProductsTableHasTheIdInTheRequest =
            $"""
            SELECT CASE WHEN EXISTS(
                SELECT CSTID FROM {_productsTableName}
                WHERE CSTID = @productId
            ) THEN 1 ELSE 0 END;
            """;

        ValidationResult result = new();

        var parametersForValidationQuery = new
        {
            productId = (int)productId
        };

        bool productWithGivenIdExists = _relationalDataAccess.GetData<bool, dynamic>(checkWhetherProductsTableHasTheIdInTheRequest, parametersForValidationQuery).FirstOrDefault();

        if (!productWithGivenIdExists)
        {
            result.Errors.Add(
                new(propertyName, "Id does not correspond to any product Id"));

            return result;
        }

        return result;
    }

    public bool DeleteAllForProductId(uint productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        var parameters = new
        {
            productId = (int)productId
        };

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductImageFileNameInfo, dynamic>(deleteQuery, parameters);

            return (rowsAffected > 0);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteByProductIdAndDisplayOrder(uint productId, int displayOrder)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId
            AND ImgNo = @DisplayOrder;

            UPDATE {_tableName}
                SET ImgNo = ImgNo - 1
            
            WHERE CSTID = @productId
            AND ImgNo > @DisplayOrder;
            """;

        var parameters = new
        {
            productId = (int)productId,
            DisplayOrder = displayOrder
        };

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<ProductImageFileNameInfo, dynamic>(deleteQuery, parameters, doInTransaction: true);

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}