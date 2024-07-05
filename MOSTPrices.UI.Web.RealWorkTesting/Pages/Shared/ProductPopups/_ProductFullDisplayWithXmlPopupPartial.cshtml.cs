using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

#pragma warning disable IDE1006 // Naming Styles
public sealed class _ProductFullDisplayWithXmlPopupPartialModel
#pragma warning restore IDE1006 // Naming Styles
{
    public _ProductFullDisplayWithXmlPopupPartialModel(Product product, string productXml, string productManifacturerSiteLink)
    {
        Product = product;
        ProductXml = productXml;
        ProductManifacturerSiteLink = productManifacturerSiteLink;
    }

    public Product Product { get; }
    public string ProductXml { get; }
    public string ProductManifacturerSiteLink { get; }
}