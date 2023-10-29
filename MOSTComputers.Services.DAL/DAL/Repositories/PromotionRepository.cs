using MOSTComputers.Services.DAL.Models;
using static MOSTComputers.Services.DAL.DAL.Repositories.RepositoryCommonElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;
using FluentValidation.Results;
using FluentValidation;
using MOSTComputers.Services.DAL.Models.Requests.Promotions;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class PromotionRepository : RepositoryBase, IPromotionRepository
{
    private const string _tableName = "dbo.Promotions";

    public PromotionRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

    public IEnumerable<Promotion> GetAll()
    {
        const string getAllQuery =
            $"""
            SELECT * FROM {_tableName}
            """;

        return _relationalDataAccess.GetData<Promotion, dynamic>(getAllQuery, new { });
    }

    public IEnumerable<Promotion> GetAllActive()
    {
        const string getAllActiveQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE Active = 1;
            """;

        return _relationalDataAccess.GetData<Promotion, dynamic>(getAllActiveQuery, new { });
    }

    public IEnumerable<Promotion> GetAllForProduct(uint productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<Promotion, dynamic>(getAllForProductQuery, new { productId });
    }

    public IEnumerable<Promotion> GetAllForSelectionOfProducts(List<uint> productIds)
    {
        const string getAllForSelectionOfProductsQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID IN
            """;

        string queryWithIds = getAllForSelectionOfProductsQuery + $" ({GetDelimeteredListFromIds(productIds)});";

        return _relationalDataAccess.GetData<Promotion, dynamic>(queryWithIds, new { });
    }

    public IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<uint> productIds)
    {
        const string getAllActiveForSelectionOfProductsQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE Active = 1
            AND CSTID IN
            """;

        string queryWithIds = getAllActiveForSelectionOfProductsQuery + $" ({GetDelimeteredListFromIds(productIds)});";

        return _relationalDataAccess.GetData<Promotion, dynamic>(queryWithIds, new { });
    }

    public Promotion? GetActiveForProduct(uint productId)
    {
        const string getActiveForProductQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId
            AND Active = 1;
            """;

        return _relationalDataAccess.GetData<Promotion, dynamic>(getActiveForProductQuery, new { productId })
            .FirstOrDefault();
    }

    public OneOf<Success, ValidationResult> Insert(PromotionCreateRequest createRequest, IValidator<PromotionCreateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult validationResult = validator.Validate(createRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(CSTID, ChgDate, PromSource, PromType, Status, SPOID, PromotionUSD, PromotionEUR, Active, StartDate,
            ExpDate, MinQty, MaxQty, CampaignID, QtyIncrement, RequiredCSTIDs, ExpQty, SoldQty, PromotionName, Consignation, Points, RegistrationID,
            Timestamp, PromotionVisualizationId)

            VALUES(@ProductId, @ChgDate, @Source, @Type, @Status, @SPOID, @DiscountUSD, @DiscountEUR, @Active, @StartDate,
            @ExpirationDate, @MinimumQuantity, @MaximumQuantity, @CampaignId, @QuantityIncrement, @RequiredProductIdsString, @ExpQuantity, @SoldQuantity,
            @Name, @Consignation, @Points, @RegistrationId, @TimeStamp, @PromotionVisualizationId)
            """;

        var parameters = new
        {
            createRequest.ProductId,
            createRequest.ChgDate,
            createRequest.Source,
            createRequest.Type,
            createRequest.Status,
            createRequest.SPOID,
            createRequest.DiscountUSD,
            createRequest.DiscountEUR,
            createRequest.Active,
            createRequest.StartDate,
            createRequest.ExpirationDate,
            createRequest.MinimumQuantity,
            createRequest.MaximumQuantity,
            createRequest.CampaignId,
            createRequest.QuantityIncrement,
            RequiredProductIdsString = GetRequiredProductIdsStringFromList(createRequest.RequiredProductIds),
            createRequest.ExpQuantity,
            createRequest.SoldQuantity,
            createRequest.Name,
            createRequest.Consignation,
            createRequest.Points,
            createRequest.TimeStamp,
            createRequest.RegistrationId,
            createRequest.PromotionVisualizationId
        };

        _relationalDataAccess.SaveData<Promotion, dynamic>(insertQuery, parameters);

        return new Success();
    }

    public OneOf<Success, ValidationResult> Update(PromotionUpdateRequest updateRequest, IValidator<PromotionUpdateRequest>? validator = null)
    {
        if (validator is not null)
        {
            ValidationResult validationResult = validator.Validate(updateRequest);

            if (!validationResult.IsValid) return validationResult;
        }

        const string updateQuery =
            $"""
            UPDATE {_tableName}
            SET CSTID = @ProductId,
                ChgDate = @ChgDate,
                PromSource = @Source,
                PromType = @Type,
                Status = @Status,
                SPOID = @SPOID,
                PromotionUSD = @DiscountUSD,
                PromotionEUR = @DiscountEUR,
                Active = @Active,
                StartDate = @StartDate,
                ExpDate = @ExpirationDate,
                MinQty = @MinimumQuantity,
                MaxQty = @MaximumQuantity,
                CampaignID = @CampaignId,
                QtyIncrement = @QuantityIncrement,
                RequiredCSTIDs = @RequiredProductIdsString,
                ExpQty = @ExpQuantity,
                SoldQty = @SoldQuantity,
                PromotionName = @Name,
                Consignation = @Consignation,
                Points = @Points,
                RegistrationID = @RegistrationId,
                Timestamp = @TimeStamp,
                PromotionVisualizationId = @PromotionVisualizationId

            WHERE PromotionID = @id;
            """;

        var parameters = new
        {
            id = updateRequest.Id,
            updateRequest.ProductId,
            updateRequest.ChgDate,
            updateRequest.Source,
            updateRequest.Type,
            updateRequest.Status,
            updateRequest.SPOID,
            updateRequest.DiscountUSD,
            updateRequest.DiscountEUR,
            updateRequest.Active,
            updateRequest.StartDate,
            updateRequest.ExpirationDate,
            updateRequest.MinimumQuantity,
            updateRequest.MaximumQuantity,
            updateRequest.CampaignId,
            updateRequest.QuantityIncrement,
            RequiredProductIdsString = GetRequiredProductIdsStringFromList(updateRequest.RequiredProductIds),
            updateRequest.ExpQuantity,
            updateRequest.SoldQuantity,
            updateRequest.Name,
            updateRequest.Consignation,
            updateRequest.Points,
            updateRequest.TimeStamp,
            updateRequest.RegistrationId,
            updateRequest.PromotionVisualizationId
        };

        _relationalDataAccess.SaveData<Promotion, dynamic>(updateQuery, parameters);

        return new Success();
    }

    public bool Delete(uint id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE PromotionId = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<Promotion, dynamic>(deleteQuery, new { id });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllByProductId(uint productId)
    {
        const string deleteAllByProductIdQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<Promotion, dynamic>(deleteAllByProductIdQuery, new { productId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static string GetRequiredProductIdsStringFromList(List<uint>? requiredProductIds)
    {
        if (requiredProductIds is null || requiredProductIds.Count == 0) return string.Empty;

        return string.Join(',', requiredProductIds);
    }
}