using MOSTComputers.Models.Product.Models;
using MOSTComputers.UI.Web.RealWorkTesting.Models;

namespace MOSTComputers.UI.Web.RealWorkTesting.Pages.Shared.ProductPopups;

public sealed class ProductFullDisplayWithXmlPopupPartialModel
{
    public ProductFullDisplayWithXmlPopupPartialModel(ModalData modalData, Product product, string productXml, string productManifacturerSiteLink)
    {
        ModalData = modalData;
        Product = product;
        ProductXml = productXml;
        ProductManifacturerSiteLink = productManifacturerSiteLink;
    }

    public ModalData ModalData { get; }
    public Product Product { get; }
    public string ProductXml { get; }
    public string ProductManifacturerSiteLink { get; }
}