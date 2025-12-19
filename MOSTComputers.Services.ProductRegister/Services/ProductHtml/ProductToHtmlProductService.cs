using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductHtml;
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
        IEnumerable<int> productIds = products.Select(x => x.Id);

        List<IGrouping<int, ProductProperty>> productProperties = await _productPropertyCrudService.GetAllInProductsAsync(productIds);

        List<GetHtmlDataForProductRequest> requests = new();

        foreach (Product product in products)
        {
            List<ProductProperty>? propertiesForProduct = productProperties.FirstOrDefault(x => x.Key == product.Id)?.ToList();

            requests.Add(new GetHtmlDataForProductRequest
            {
                Product = product,
                ProductProperties = propertiesForProduct
            });
        }

        return GetHtmlProductDataFromProducts(requests);
    }

    public HtmlProductsData GetHtmlProductDataFromProducts(List<GetHtmlDataForProductRequest> requests)
    {
        List<HtmlProduct> htmlProducts = new();

        foreach (GetHtmlDataForProductRequest request in requests)
        {
            HtmlProduct htmlProduct = new()
            {
                Id = request.Product.Id,
                Name = request.Product.Name,
                Properties = new(),
            };

            if (request.ProductProperties is not null)
            {
                foreach (ProductProperty property in request.ProductProperties)
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