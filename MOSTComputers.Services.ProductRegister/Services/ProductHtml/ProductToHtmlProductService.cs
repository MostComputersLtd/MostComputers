using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductHtml.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;

namespace MOSTComputers.Services.ProductRegister.Services.ProductHtml;
internal sealed class ProductToHtmlProductService : IProductToHtmlProductService
{
    public ProductToHtmlProductService(
        IProductService productService,
        IProductPropertyCrudService productPropertyCrudService)
    {
        _productService = productService;
        _productPropertyCrudService = productPropertyCrudService;
    }

    private readonly IProductService _productService;
    private readonly IProductPropertyCrudService _productPropertyCrudService;

    public async Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(List<int> productIds)
    {
        List<Product> products = await _productService.GetByIdsAsync(productIds);

        return await GetHtmlProductDataFromProductsAsync(products);
    }

    public async Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(params Product[] products)
    {
        return await GetHtmlProductDataFromProductsAsync((IEnumerable<Product>)products);
    }

    public async Task<HtmlProductsData> GetHtmlProductDataFromProductsAsync(IEnumerable<Product> products)
    {
        List<HtmlProduct> htmlProducts = new();

        IEnumerable<int> productIds = products.Select(x => x.Id);

        List<IGrouping<int, ProductProperty>> productProperties = await _productPropertyCrudService.GetAllInProductsAsync(productIds);

        foreach (Product product in products)
        {
            HtmlProduct htmlProduct = new()
            {
                Id = product.Id,
                Name = product.Name,
                Properties = new(),
            };

            IGrouping<int, ProductProperty>? propertiesForProduct = productProperties.FirstOrDefault(x => x.Key == product.Id);

            if (propertiesForProduct is not null)
            {
                foreach (ProductProperty property in propertiesForProduct)
                {
                    HtmlProductProperty htmlProductProperty = new()
                    {
                        Name = property.Characteristic,
                        Order = property.DisplayOrder?.ToString(),
                        Value = property.Value,
                    };

                    htmlProduct.Properties.Add(htmlProductProperty);
                }
            }

            htmlProducts.Add(htmlProduct);
        }

        return new HtmlProductsData()
        {
            Products = htmlProducts
        };
    }
}