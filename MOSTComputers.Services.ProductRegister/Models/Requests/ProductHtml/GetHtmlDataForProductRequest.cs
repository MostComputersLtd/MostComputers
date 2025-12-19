namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductHtml;

public sealed class GetHtmlDataForProductRequest
{
    public required MOSTComputers.Models.Product.Models.Product Product { get; set; }
    public List<MOSTComputers.Models.Product.Models.ProductProperty>? ProductProperties { get; set; }
}