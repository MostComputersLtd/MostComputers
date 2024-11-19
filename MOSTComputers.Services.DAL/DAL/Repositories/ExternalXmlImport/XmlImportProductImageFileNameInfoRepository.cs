using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts.ExternalXmlImport;
using MOSTComputers.Services.DAL.Models.Requests.ExternalXmlImport.ProductImageFileNameInfo;
using MOSTComputers.Services.DAL.Models.Requests.ProductImageFileNameInfo;
using OneOf;
using OneOf.Types;

using static MOSTComputers.Services.DAL.Utils.TableAndColumnNameUtils;

namespace MOSTComputers.Services.DAL.DAL.Repositories.ExternalXmlImport;

internal sealed class XmlImportProductImageFileNameInfoRepository : RepositoryBase, IXmlImportProductImageFileNameInfoRepository
{
    public XmlImportProductImageFileNameInfoRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

    public int? ImagesInImagesAllForProductCount { get; set; }
    public bool IsProductFirstImageInImages { get; set; }
    public IEnumerable<XmlImportProductImageFileNameInfo> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {ImageFileNamesTableName}
            ORDER BY CSTID, S;
            """;

        return _relationalDataAccess.GetData<XmlImportProductImageFileNameInfo, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<XmlImportProductImageFileNameInfo> GetAllInProduct(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            ORDER BY S;
            """;

        return _relationalDataAccess.GetData<XmlImportProductImageFileNameInfo, dynamic>(getAllForProductQuery, new { productId });
    }

    public XmlImportProductImageFileNameInfo? GetByProductIdAndImageNumber(int productId, int imageNumber)
    {
        const string getAllForProductQuery =
            $"""
            SELECT TOP 1 * FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId
            AND ImageNumber = @imageNumber
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<XmlImportProductImageFileNameInfo, dynamic>(getAllForProductQuery,
            new { productId, imageNumber });
    }

    public XmlImportProductImageFileNameInfo? GetByFileName(string fileName)
    {
        const string getAllForProductQuery =
            $"""
            SELECT TOP 1 * FROM {ImageFileNamesTableName}
            WHERE ImgFileName = @fileName
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<XmlImportProductImageFileNameInfo, dynamic>(getAllForProductQuery,
            new { fileName });
    }

    public int? GetHighestImageNumber(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT MAX(ImageNumber) FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetDataFirstOrDefault<int?, dynamic>(getAllForProductQuery,
            new { productId });
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> Insert(XmlImportProductImageFileNameInfoCreateRequest createRequest)
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

            UPDATE {ImageFileNamesTableName}
                SET S = S + 1
            WHERE CSTID = @productId
            AND S >= @DisplayOrderInRange;

            INSERT INTO {ImageFileNamesTableName}(CSTID, ImageNumber, S, ImgFileName, Active, ImagesAllForProductCount, IsProductFirstImageInImages)
            SELECT @productId, ISNULL((SELECT MAX(ImageNumber) + 1 FROM {ImageFileNamesTableName} WHERE CSTID = @productId), 1),
            @DisplayOrderInRange, @FileName, @Active, @ImagesInImagesAllForProductCount, @IsProductFirstImageInImages

            IF @@ROWCOUNT > 0
            BEGIN
                SELECT 1;
            END
            BEGIN
                SELECT 0;
            END
            """;

        var parameters = new
        {
            productId = createRequest.ProductId,
            createRequest.DisplayOrder,
            createRequest.FileName,
            createRequest.Active,
            createRequest.ImagesInImagesAllForProductCount,
            createRequest.IsProductFirstImageInImages,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertQuery, parameters, doInTransaction: true);

        if (result is null || result == 0) return new UnexpectedFailureResult();

        if (result > 0) return new Success();

        ValidationResult validationResult = GetValidationResultFromFailedOperation(result.Value);

        return validationResult.IsValid ? new UnexpectedFailureResult() : validationResult;
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByImageNumber(XmlImportProductImageFileNameInfoByImageNumberUpdateRequest updateRequest)
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
                Active = @Active,
                ImagesAllForProductCount = @ImagesInImagesAllForProductCount,
                IsProductFirstImageInImages = @IsProductFirstImageInImages

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
            """;

        const string updateQueryWithNoDisplayOrderChanges =
            $"""
            UPDATE {ImageFileNamesTableName}
            SET ImgFileName = @FileName,
                Active = @Active,
                ImagesAllForProductCount = @ImagesInImagesAllForProductCount,
                IsProductFirstImageInImages = @IsProductFirstImageInImages

            WHERE CSTID = @productId
            AND ImageNumber = @ImageNumber;

            SELECT @@ROWCOUNT;
            """;

        if (!updateRequest.ShouldUpdateDisplayOrder)
        {
            var parametersSimple = new
            {
                productId = updateRequest.ProductId,
                updateRequest.ImageNumber,
                updateRequest.FileName,
                updateRequest.Active,
                updateRequest.ImagesInImagesAllForProductCount,
                updateRequest.IsProductFirstImageInImages,
            };

            int? resultSimple = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateQueryWithNoDisplayOrderChanges, parametersSimple, doInTransaction: true);

            return GetOutputBasedOnQueryResult(resultSimple);
        }

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.NewDisplayOrder,
            updateRequest.ImageNumber,
            updateRequest.FileName,
            updateRequest.Active,
            updateRequest.ImagesInImagesAllForProductCount,
            updateRequest.IsProductFirstImageInImages,
        };

        int? result = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(updateQuery, parameters, doInTransaction: true);

        return GetOutputBasedOnQueryResult(result);
    }

    public OneOf<Success, ValidationResult, UnexpectedFailureResult> UpdateByFileName(XmlImportProductImageFileNameInfoByFileNameUpdateRequest updateRequest)
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
                Active = @Active,
                ImagesAllForProductCount = @ImagesInImagesAllForProductCount,
                IsProductFirstImageInImages = @IsProductFirstImageInImages

            WHERE CSTID = @productId
            AND ImgFileName = @FileName;

            SELECT @@ROWCOUNT;
            """;

        const string updateByFileNameWithoutDisplayOrderChanges =
            $"""
            UPDATE {ImageFileNamesTableName}
            SET ImgFileName = ISNULL(@NewFileName, ImgFileName),
                Active = @Active,
                ImagesAllForProductCount = @ImagesInImagesAllForProductCount,
                IsProductFirstImageInImages = @IsProductFirstImageInImages
            
            WHERE CSTID = @productId
            AND ImgFileName = @FileName;
            
            SELECT @@ROWCOUNT;
            """;

        if (!updateRequest.ShouldUpdateDisplayOrder)
        {
            var parametersSimple = new
            {
                productId = updateRequest.ProductId,
                updateRequest.FileName,
                updateRequest.NewFileName,
                updateRequest.Active,
                updateRequest.ImagesInImagesAllForProductCount,
                updateRequest.IsProductFirstImageInImages,
            };

            int? resultSimple = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(
                updateByFileNameWithoutDisplayOrderChanges, parametersSimple, doInTransaction: true);

            return GetOutputBasedOnQueryResult(resultSimple);
        }

        var parameters = new
        {
            productId = updateRequest.ProductId,
            updateRequest.FileName,
            updateRequest.NewDisplayOrder,
            updateRequest.NewFileName,
            updateRequest.Active,
            updateRequest.ImagesInImagesAllForProductCount,
            updateRequest.IsProductFirstImageInImages,
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

    public bool DeleteAllForProductId(int productId)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {ImageFileNamesTableName}
            WHERE CSTID = @productId;
            """;

        var parameters = new
        {
            productId
        };

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<dynamic>(deleteQuery, parameters);

            return rowsAffected > 0;
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
            productId,
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
            productId,
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