using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class ProductImageFileNameInfoRepository : RepositoryBase, IProductImageFileNameInfoRepository
{
    public ProductImageFileNameInfoRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public IEnumerable<ProductImageFileNameInfo> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {ImageFileNamesTableName}
            ORDER BY CSTID, S;
            """;

        return _relationalDataAccess.GetData<ProductImageFileNameInfo, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<ProductImageFileNameInfo> GetAllInProduct(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<ProductImageFileNameInfo, dynamic>(getAllForProductQuery, new { productId = productId });
    }

    public ProductImageFileNameInfo? GetByProductIdAndImageNumber(int productId, int imageNumber)
    {
        const string getAllForProductQuery =
            $"""
            SELECT TOP 1 * FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND ImageNumber = @imageNumber
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductImageFileNameInfo, dynamic>(getAllForProductQuery,
            new { productId = productId, imageNumber = imageNumber });
    }

    public ProductImageFileNameInfo? GetByFileName(string fileName)
    {
        const string getAllForProductQuery =
            $"""
            SELECT TOP 1 * FROM {ImageFileNamesTableName}
            WHERE ImgFileName = @fileName
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<ProductImageFileNameInfo, dynamic>(getAllForProductQuery,
            new { fileName = fileName });
    }

    public int? GetHighestImageNumber(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT MAX(ImageNumber) FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<int?, dynamic>(getAllForProductQuery,
            new { productId = productId });
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(ProductImageFileNameInfoCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            DECLARE @DisplayOrderInRange INT, @MaxDisplayOrderForProduct INT

            SELECT TOP 1 @MaxDisplayOrderForProduct = MAX(S) + 1 FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId;

            SELECT TOP 1 @DisplayOrderInRange = ISNULL(
                (SELECT TOP 1 @MaxDisplayOrderForProduct FROM {ImageFileNamesTableName}
                WHERE @MaxDisplayOrderForProduct <= @DisplayOrder),
                @DisplayOrder);

            IF EXISTS (SELECT 1 FROM {ProductsTableName} WHERE CSTID = @productId)
            BEGIN
                UPDATE {ImageFileNamesTableName}
                    SET S = S + 1
                WHERE CSTID = @productId
                AND S >= @DisplayOrderInRange;

                INSERT INTO {ImageFileNamesTableName}(CSTID, ImageNumber, S, ImgFileName, Active)
                SELECT @productId, ISNULL((SELECT MAX(ImageNumber) + 1 FROM {ImageFileNamesTableName} WHERE CSTID = @productId), 1),
                @DisplayOrderInRange, @FileName, @Active
                WHERE EXISTS (SELECT 1 FROM {ProductsTableName} WHERE CSTID = @productId);
            END
            IF @@ROWCOUNT > 0
            BEGIN
                SELECT 1;
            END
            ELSE IF NOT EXISTS(SELECT 1 FROM {ProductsTableName} WHERE CSTID = @productId)
            BEGIN
                SELECT -1;
            END
            ELSE
            BEGIN
                SELECT 0;
            END
            """;


        //ValidationResult result = ValidateWhetherProductWithGivenIdExists((uint)createRequest.ProductId);

        //if (!result.IsValid) return result;

        var parameters = new
        {
            productId = createRequest.ProductId,
            DisplayOrder = createRequest.DisplayOrder,
            FileName = createRequest.FileName,
            Active = createRequest.Active
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertQuery, parameters, doInTransaction: true);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetValidationResultFromFailedOperation(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(ProductImageFileNameInfoByImageNumberUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            DECLARE @DisplayOrder INT;
            DECLARE @MaxDisplayOrder INT;
            
            SELECT TOP 1 @DisplayOrder = S FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND ImageNumber = @ImageNumber;

            SELECT @MaxDisplayOrder = ISNULL((SELECT COUNT(*) FROM {ImageFileNamesTableName} WHERE CSTID = @productId), 1);

            SET @NewDisplayOrder = 
            CASE 
                WHEN @NewDisplayOrder IS NULL THEN NULL
                WHEN @NewDisplayOrder < 1 THEN 1
                WHEN @NewDisplayOrder > @MaxDisplayOrder THEN @MaxDisplayOrder
                ELSE @NewDisplayOrder
            END;

            IF EXISTS (SELECT 1 FROM {ProductsTableName} WHERE CSTID = @productId)
            BEGIN       
                UPDATE {ImageFileNamesTableName}
                SET S = 
                    CASE
                        WHEN S = @DisplayOrder AND ImageNumber = @ImageNumber THEN @NewDisplayOrder
                        WHEN @DisplayOrder < @NewDisplayOrder AND S > @DisplayOrder AND S <= @NewDisplayOrder THEN S - 1
                        WHEN @DisplayOrder > @NewDisplayOrder AND S < @DisplayOrder AND S >= @NewDisplayOrder THEN S + 1
                        ELSE S
                    END
                WHERE CSTID = @productId;

                UPDATE {ImageFileNamesTableName}
                SET ImgFileName = @FileName,
                    Active = @Active

                WHERE CSTID = @productId
                AND ImageNumber = @ImageNumber;

                IF @@ROWCOUNT > 0
                BEGIN
                    SELECT 1;
                END
                ELSE
                BEGIN
                    SELECT 0;
                END
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        const string updateQueryWithNoDisplayOrderChanges =
            $"""
            IF EXISTS (SELECT 1 FROM {ProductsTableName} WHERE CSTID = @productId)
            BEGIN 
                UPDATE {ImageFileNamesTableName}
                SET ImgFileName = @FileName,
                    Active = @Active

                WHERE CSTID = @productId
                AND ImageNumber = @ImageNumber;

            SELECT @@ROWCOUNT;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        //ValidationResult internalValidationResult = ValidateWhetherProductWithGivenIdExists((uint)updateRequest.ProductId);

        //if (!internalValidationResult.IsValid) return internalValidationResult;

        if (!updateRequest.ShouldUpdateDisplayOrder)
        {
            var parametersSimple = new
            {
                productId = updateRequest.ProductId,
                ImageNumber = updateRequest.ImageNumber,
                FileName = updateRequest.FileName,
                Active = updateRequest.Active,
            };

            int? resultSimple = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateQueryWithNoDisplayOrderChanges, parametersSimple, doInTransaction: true);

            return GetOutputBasedOnQueryResult(resultSimple);
        }

        var parameters = new
        {
            productId = updateRequest.ProductId,
            NewDisplayOrder = updateRequest.NewDisplayOrder,
            ImageNumber = updateRequest.ImageNumber,
            FileName = updateRequest.FileName,
            Active = updateRequest.Active,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateQuery, parameters, doInTransaction: true);

        return GetOutputBasedOnQueryResult(result);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(ProductImageFileNameInfoByFileNameUpdateRequest updateRequest)
    {
        const string updateByFileNameQuery =
            $"""
            DECLARE @DisplayOrder INT;
            DECLARE @MaxDisplayOrder INT;
            
            SELECT TOP 1 @DisplayOrder = S
            FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND ImgFileName = @FileName;

            SELECT @MaxDisplayOrder = ISNULL((SELECT COUNT(*) FROM {ImageFileNamesTableName} WHERE CSTID = @productId), 1);

            SET @NewDisplayOrder = 
            CASE 
                WHEN @NewDisplayOrder IS NULL THEN NULL
                WHEN @NewDisplayOrder < 1 THEN 1
                WHEN @NewDisplayOrder > @MaxDisplayOrder THEN @MaxDisplayOrder
                ELSE @NewDisplayOrder
            END;

            IF EXISTS (SELECT 1 FROM {ProductsTableName} WHERE CSTID = @productId)
            BEGIN       
                UPDATE {ImageFileNamesTableName}
                SET S = 
                    CASE
                        WHEN S = @DisplayOrder AND ImgFileName = @FileName THEN @NewDisplayOrder
                        WHEN @DisplayOrder < @NewDisplayOrder AND S > @DisplayOrder AND S <= @NewDisplayOrder THEN S - 1
                        WHEN @DisplayOrder > @NewDisplayOrder AND S < @DisplayOrder AND S >= @NewDisplayOrder THEN S + 1
                        ELSE S
                    END
                WHERE CSTID = @productId;

                UPDATE {ImageFileNamesTableName}
                SET ImgFileName = ISNULL(@NewFileName, ImgFileName),
                    Active = @Active

                WHERE CSTID = @productId
                AND ImgFileName = @FileName;

                SELECT @@ROWCOUNT;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        const string updateByFileNameWithoutDisplayOrderChanges =
            $"""
            IF EXISTS (SELECT 1 FROM {ProductsTableName} WHERE CSTID = @productId)
            BEGIN 
                UPDATE {ImageFileNamesTableName}
                SET ImgFileName = ISNULL(@NewFileName, ImgFileName),
                    Active = @Active
            
                WHERE CSTID = @productId
                AND ImgFileName = @FileName;
            
                SELECT @@ROWCOUNT;
            END
            ELSE
            BEGIN
                SELECT -1;
            END
            """;

        //ValidationResult internalValidationResult = ValidateWhetherProductWithGivenIdExists((uint)updateRequest.ProductId);

        //if (!internalValidationResult.IsValid) return internalValidationResult;

        if (!updateRequest.ShouldUpdateDisplayOrder)
        {
            var parametersSimple = new
            {
                productId = updateRequest.ProductId,
                FileName = updateRequest.FileName,
                NewFileName = updateRequest.NewFileName,
                Active = updateRequest.Active,
            };

            int? resultSimple = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(
                updateByFileNameWithoutDisplayOrderChanges, parametersSimple, doInTransaction: true);

            return GetOutputBasedOnQueryResult(resultSimple);
        }

        var parameters = new
        {
            productId = updateRequest.ProductId,
            FileName = updateRequest.FileName,
            NewDisplayOrder = updateRequest.NewDisplayOrder,
            NewFileName = updateRequest.NewFileName,
            Active = updateRequest.Active,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateByFileNameQuery, parameters, doInTransaction: true);

        return GetOutputBasedOnQueryResult(result);
    }

    private static OneOf<Success, ValidationResult, UnexpectedFailureResult> GetOutputBasedOnQueryResult(int? result)
    {
        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetValidationResultFromFailedOperation(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    private static ValidationResult GetValidationResultFromFailedOperation(int result)
    {
        ValidationResult simpleValidationResult = new();

        if (result == -1)
        {
            simpleValidationResult.Errors.Add(new(nameof(ProductImageFileNameInfoByImageNumberUpdateRequest.ProductId),
                "Id does not correspond to any known product"));
        }

        return simpleValidationResult;
    }

    //private ValidationResult ValidateWhetherProductWithGivenIdExists(uint productId, string propertyName = "ProductId")
    //{
    //    const string checkWhetherProductsTableHasTheIdInTheRequest =
    //        $"""
    //        SELECT CASE WHEN EXISTS(
    //            SELECT CSTID FROM {_productsTableName}
    //            WHERE CSTID = @productId
    //        ) THEN 1 ELSE 0 END;
    //        """;

    //    ValidationResult result = new();

    //    var parametersForValidationQuery = new
    //    {
    //        productId = (int)productId
    //    };

    //    bool productWithGivenIdExists = _relationalDataAccess.GetData<bool, dynamic>(checkWhetherProductsTableHasTheIdInTheRequest, parametersForValidationQuery).FirstOrDefault();

    //    if (!productWithGivenIdExists)
    //    {
    //        result.Errors.Add(new(propertyName, "Id does not correspond to any product Id"));

    //        return result;
    //    }

    //    return result;
    //}

    public bool DeleteAllForProductId(int productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId;
            """;

        var parameters = new
        {
            productId = productId
        };

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, parameters);

            return (rowsAffected > 0);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteByProductIdAndImageNumber(int productId, int imageNumber)
    {
        const string deleteQuery =
            $"""
            DECLARE @DisplayOrder INT;
            
            SELECT TOP 1 @DisplayOrder = S
            FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND ImageNumber = @ImageNumber;

            DELETE FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND ImageNumber = @ImageNumber;

            UPDATE {ImageFileNamesTableName}
                SET S = S - 1
            
            WHERE CSTID = @productId
            AND S > @DisplayOrder;

            UPDATE {ImageFileNamesTableName}
            SET ImageNumber = ImageNumber - 1
            
            WHERE CSTID = @productId
            AND ImageNumber > @ImageNumber;
            """;

        var parameters = new
        {
            productId = productId,
            ImageNumber = imageNumber
        };

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, parameters, doInTransaction: true);
            
            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteByProductIdAndDisplayOrder(int productId, int displayOrder)
    {
        const string deleteQuery =
            $"""
            DECLARE @DeletedItemImageNumber INT;

            SELECT TOP 1 @DeletedItemImageNumber = ImageNumber
            FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND S = @DisplayOrder;

            DELETE FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND S = @DisplayOrder;

            UPDATE {ImageFileNamesTableName}
                SET S = S - 1
            
            WHERE CSTID = @productId
            AND S > @DisplayOrder;

            UPDATE {ImageFileNamesTableName}
            SET ImageNumber = ImageNumber - 1
            
            WHERE CSTID = @productId
            AND ImageNumber > @DeletedItemImageNumber;
            """;

        var parameters = new
        {
            productId = productId,
            DisplayOrder = displayOrder
        };

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, parameters, doInTransaction: true);

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