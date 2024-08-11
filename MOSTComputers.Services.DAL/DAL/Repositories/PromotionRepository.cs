using static MOSTComputers.Services.DAL.DAL.Repositories.RepositoryCommonElements;
using OneOf;
using OneOf.Types;
using MOSTComputers.Services.DAL.DAL.Repositories.Contracts;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Models.Product.Models.Requests.Promotion;

namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal sealed class PromotionRepository : RepositoryBase, IPromotionRepository
{
    private const string _tableName = "dbo.Promotions";

    public PromotionRepository(IRelationalDataAccess relationalDataAccess)
        : base(relationalDataAccess)
    {
    }

#pragma warning disable IDE0037 // Use inferred member name

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

    public IEnumerable<Promotion> GetAllForProduct(int productId)
    {
        const string getAllForProductQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        return _relationalDataAccess.GetData<Promotion, dynamic>(getAllForProductQuery, new { productId = productId });
    }

    public IEnumerable<Promotion> GetAllForSelectionOfProducts(List<int> productIds)
    {
        const string getAllForSelectionOfProductsQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID IN
            """;

        string queryWithIds = getAllForSelectionOfProductsQuery + $" ({GetDelimeteredListFromIds(productIds)});";

        return _relationalDataAccess.GetData<Promotion, dynamic>(queryWithIds, new { });
    }

    public IEnumerable<Promotion> GetAllActiveForSelectionOfProducts(List<int> productIds)
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

    public Promotion? GetActiveForProduct(int productId)
    {
        const string getActiveForProductQuery =
            $"""
            SELECT * FROM {_tableName}
            WHERE CSTID = @productId
            AND Active = 1;
            """;

        return _relationalDataAccess.GetData<Promotion, dynamic>(getActiveForProductQuery, new { productId = productId })
            .FirstOrDefault();
    }

    public OneOf<int, UnexpectedFailureResult> Insert(PromotionCreateRequest createRequest)
    {
        const string insertQuery =
            $"""
            INSERT INTO {_tableName}(PromotionID, CSTID, ChgDate, PromSource, PromType, Status, SPOID, PromotionUSD, PromotionEUR, Active, StartDate,
            ExpDate, MinQty, MaxQty, CampaignID, QtyIncrement, RequiredCSTIDs, ExpQty, SoldQty, PromotionName, Consignation, Points, RegistrationID,
            Timestamp, PromotionVisualizationId)
            OUTPUT INSERTED.PromotionID
            VALUES(ISNULL((SELECT MAX(PromotionID) + 1 FROM {_tableName}), 1), @ProductId, @PromotionAddedDate, @Source, @Type, @Status, @SPOID, @DiscountUSD, @DiscountEUR, @Active, @StartDate,
            @ExpirationDate, @MinimumQuantity, @MaximumQuantity, @CampaignId, @QuantityIncrement, @RequiredProductIdsString, @ExpQuantity, @SoldQuantity,
            @Name, @Consignation, @Points, @RegistrationId, @TimeStamp, @PromotionVisualizationId)
            """;

        var parameters = new
        {
            createRequest.ProductId,
            createRequest.PromotionAddedDate,
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

        int? id = _relationalDataAccess.SaveDataAndReturnValue<int?, dynamic>(insertQuery, parameters);

        return (id is not null && id > 0) ? id.Value : new UnexpectedFailureResult();
    }

    public OneOf<Success, UnexpectedFailureResult> Update(PromotionUpdateRequest updateRequest)
    {
        const string updateQuery =
            $"""
            UPDATE {_tableName}
            SET CSTID = @ProductId,
                ChgDate = @PromotionAddedDate,
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
            updateRequest.PromotionAddedDate,
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

        int rowsAffected = _relationalDataAccess.SaveData<Promotion, dynamic>(updateQuery, parameters);

        return (rowsAffected != 0) ? new Success() : new UnexpectedFailureResult();
    }

    public bool Delete(int id)
    {
        const string deleteQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE PromotionId = @id;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<Promotion, dynamic>(deleteQuery, new { id = id });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteAllByProductId(int productId)
    {
        const string deleteAllByProductIdQuery =
            $"""
            DELETE FROM {_tableName}
            WHERE CSTID = @productId;
            """;

        try
        {
            int rowsAffected = _relationalDataAccess.SaveData<Promotion, dynamic>(deleteAllByProductIdQuery, new { productId = productId });

            if (rowsAffected == 0) return false;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

#pragma warning restore IDE0037 // Use inferred member name

    private static string GetRequiredProductIdsStringFromList(List<int>? requiredProductIds)
    {
        if (requiredProductIds is null || requiredProductIds.Count == 0) return string.Empty;

        return string.Join(',', requiredProductIds);
    }
}