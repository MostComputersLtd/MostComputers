using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public sealed class ProductFullDisplayWithXmlPopupPartialModel
{
    public ProductFullDisplayWithXmlPopupPartialModel(Product product, string productXml, string productManifacturerSiteLink)
    {
        Product = product;
        ProductXml = productXml;
        ProductManifacturerSiteLink = productManifacturerSiteLink;
    }

    public Product Product { get; }
    public string ProductXml { get; }
    public string ProductManifacturerSiteLink { get; }
}