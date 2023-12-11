using System.Data;

namespace MOSTComputers.Models.Product.Models;

public sealed class Promotion
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateTime? PromotionAddedDate { get; set; }
    public short? Source { get; set; } // 4 or 8
    public short? Type { get; set; } // 1 or 2
    public short? Status { get; set; } // 0, 1, 3 or 4
    public int? SPOID { get; set; }
    public decimal? DiscountUSD { get; set; }
    public decimal? DiscountEUR { get; set; }
    public bool? Active { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int? MinimumQuantity { get; set; }
    public int? MaximumQuantity { get; set; }
    public int? QuantityIncrement { get; set; }

    public List<uint>? RequiredProductIds { get; set; }
    public string? RequiredProductIdsString
    {
        get => RequiredProductIds is not null ? string.Join(',', RequiredProductIds) : null;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                RequiredProductIds = null;

                return;
            }

            try
            {
                RequiredProductIds = value?.Split(',')
                .Select(productId => uint.Parse(productId))
                .ToList();
            }
            catch (FormatException)
            {
                return;
            }

        }
    }

    public int? ExpQuantity { get; set; }
    public int? SoldQuantity { get; set; }
    public short? Consignation { get; set; }
    public float? Points { get; set; }
    public string? TimeStamp { get; set; }

    public int? ProductId { get; set; }
    public int? CampaignId { get; set; }
    public int? RegistrationId { get; set; }
    public int? PromotionVisualizationId { get; set; }
}