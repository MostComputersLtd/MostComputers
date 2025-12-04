namespace MOSTComputers.Models.Product.Models.Promotions;

public sealed class Promotion
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public DateTime? PromotionAddedDate { get; init; }
    public short? Source { get; init; } // 4 or 8
    public short? Type { get; init; } // 1 or 2
    public short? Status { get; init; } // 0, 1, 3 or 4
    public int? SPOID { get; init; }
    public decimal? DiscountUSD { get; init; }
    public decimal? DiscountEUR { get; init; }
    public bool? Active { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? ExpirationDate { get; init; }
    public int? MinimumQuantity { get; init; }
    public int? MaximumQuantity { get; init; }
    public int? QuantityIncrement { get; init; }

    public List<int>? RequiredProductIds { get; private init; }
    public string? RequiredProductIdsString
    {
        get => RequiredProductIds is not null ? string.Join(',', RequiredProductIds) : null;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                RequiredProductIds = null;

                return;
            }

            string[] newRequiredProductIdsAsStrings = value.Split(',');

            List<int> newRequiredProductIds = new();

            foreach (string requiredProductIdAsString in newRequiredProductIdsAsStrings)
            {
                bool parseIdSuccess = int.TryParse(requiredProductIdAsString, out int requiredProductId);

                if (!parseIdSuccess) return;

                newRequiredProductIds.Add(requiredProductId);
            }

            RequiredProductIds = newRequiredProductIds;
        }
    }

    public int? ExpQuantity { get; init; }
    public int? SoldQuantity { get; init; }
    public short? Consignation { get; init; }
    public float? Points { get; init; }
    public string? TimeStamp { get; init; }

    public int? ProductId { get; init; }
    public int? CampaignId { get; init; }
    public int? RegistrationId { get; init; }
    public int? PromotionVisualizationId { get; init; }
}