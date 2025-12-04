namespace MOSTComputers.UI.Web.Models.PromotionFiles;

public sealed class PromotionFileInfoEditorDisplayData
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public required int Id { get; init; }
    public string? CreateUserName { get; init; }
    public required DateTime CreateDate { get; init; }
    public string? LastUpdateUserName { get; init; }
    public required DateTime LastUpdateDate { get; init; }
    public string? Name { get; init; }
    public bool Active { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
    public string FileName { get; init; }
    public string? Description { get; init; }
    public string? RelatedProductsString { get; init; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}