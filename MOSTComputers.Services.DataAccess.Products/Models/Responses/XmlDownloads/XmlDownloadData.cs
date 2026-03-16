namespace MOSTComputers.Services.DataAccess.Products.Models.Responses.XmlDownloads;
public sealed class XmlDownloadData
{
    public DateTime TimeStamp { get; init; }
    public required string XmlResourceType { get; init; }
    public int? BID { get; init; }
    public string? UserName { get; init; }
    public string? ContactPerson { get; init; }
}