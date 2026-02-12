using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.ExternalXmlImport;
using MOSTComputers.Models.Product.Models.ProductImages;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Html.New;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models.Xml.Legacy;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.Legacy.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Html.New.Contracts;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services.Xml.Legacy.Contracts;
using MOSTComputers.Services.ProductRegister.Models.Requests.ProductHtml;
using MOSTComputers.Services.ProductRegister.Services.ExternalXmlImport.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductImages.Contracts;
using MOSTComputers.Services.ProductRegister.Services.ProductProperties.Contacts;
using MOSTComputers.UI.Web.Blazor.Services.ExternalXmlImport.Contracts;
using OneOf;
using OneOf.Types;

namespace MOSTComputers.UI.Web.Blazor.Components._Tests;

public sealed class DiagnoseDifferencesBetweenDataStores
{
    public enum DataSource
    {
        ProductProperties,
        HtmlProductInImagesOrImagesAll,
        LegacyXmlProduct
    };

    public sealed class ProductDataComparisonResult
    {
        public required MOSTComputers.Models.Product.Models.Product Product { get; init; }
        public bool AreDataSourcesIdentical { get; init; }
        public int DataSourceDeviations { get; init; }
        public List<ProductProperty>? ProductProperties { get; init; }
        public ProductImageData? ProductFirstImage { get; init; }
        public List<ProductImageData>? ProductImages { get; init; }
        public LegacyHtmlProduct? HtmlProduct { get; init; }
        public LegacyXmlProduct? LegacyXmlProduct { get; init; }
    }

    private const int _defaultLinkCharacteristicId = 1693;

    private readonly IProductImageService _productImageService;
    private readonly IProductPropertyService _productPropertyService;
    private readonly IProductXmlProvidingService _productXmlProvidingService;
    //private readonly IProductHtmlService _productHtmlService;
    private readonly ILegacyProductHtmlService _legacyProductHtmlService;
    private readonly IProductCharacteristicAndExternalXmlDataRelationService _productCharacteristicAndExternalXmlDataRelationService;

    public DiagnoseDifferencesBetweenDataStores(
        IProductImageService productImageService,
        IProductPropertyService productPropertyService,
        IProductXmlProvidingService productXmlProvidingService,
        //IProductHtmlService productHtmlService,
        ILegacyProductHtmlService legacyProductHtmlService,
        IProductCharacteristicAndExternalXmlDataRelationService productCharacteristicAndExternalXmlDataRelationService)
    {
        _productImageService = productImageService;
        _productPropertyService = productPropertyService;
        _productXmlProvidingService = productXmlProvidingService;
        //_productHtmlService = productHtmlService;
        _legacyProductHtmlService = legacyProductHtmlService;
        _productCharacteristicAndExternalXmlDataRelationService = productCharacteristicAndExternalXmlDataRelationService;
    }

    public async Task<List<ProductDataComparisonResult>> CompareDataSourcesForProductsAsync(
        List<MOSTComputers.Models.Product.Models.Product> products,
        DataSource[] dataSources,
        Func<ProductDataComparisonResult, Task>? onNewElementAdded = null)
    {
        if (dataSources.Length < 2) return new();

        dataSources = dataSources.Distinct().ToArray();

        if (dataSources.Length < 2) return new();

        bool shouldFetchLegacyXml = dataSources.Contains(DataSource.LegacyXmlProduct);
        bool shouldFetchImages = dataSources.Contains(DataSource.HtmlProductInImagesOrImagesAll);
        bool shouldFetchProperties = dataSources.Contains(DataSource.ProductProperties);

        LegacyXmlObjectData? legacyXmlObjectData = null;

        if (shouldFetchLegacyXml)
        {
            OneOf<LegacyXmlObjectData, InvalidXmlResult, NotFound> getProductXmlResult = await _productXmlProvidingService.GetProductXmlParsedAsync();

            if (getProductXmlResult.IsT0)
            {
                legacyXmlObjectData = getProductXmlResult.AsT0;
            }
        }

        List<int> productIds = products.Select(p => p.Id).ToList();

        List<IGrouping<int, ProductImageData>>? allProductImages = null;
        List<ProductImageData>? productFirstImages = null;
        List<IGrouping<int, ProductProperty>>? allProductProperties = null;

        if (productIds.Count > 10000)
        {
            if (shouldFetchImages)
            {
                productFirstImages = await _productImageService.GetAllFirstImagesWithoutFileDataForAllProductsAsync();
                allProductImages = await _productImageService.GetAllWithoutFileDataAsync();
            }

            if (shouldFetchProperties)
            {
                allProductProperties = await _productPropertyService.GetAllAsync();
            }
        }
        else
        {
            if (shouldFetchImages)
            {
                productFirstImages = await _productImageService.GetFirstImagesWithoutFileDataForSelectionOfProductsAsync(productIds);
                allProductImages = await _productImageService.GetAllInProductsWithoutFileDataAsync(productIds);
            }

            if (shouldFetchProperties)
            {
                allProductProperties = await _productPropertyService.GetAllInProductsAsync(productIds);
            }
        }

        List<MOSTComputers.Models.Product.Models.ExternalXmlImport.ProductCharacteristicAndExternalXmlDataRelation> productIdToExternalXmlDataRelations
            = await _productCharacteristicAndExternalXmlDataRelationService.GetAllAsync();

        List<ProductDataComparisonResult> comparisonResults = new();

        foreach (MOSTComputers.Models.Product.Models.Product product in products)
        {
            ProductImageData? firstImage = productFirstImages?.FirstOrDefault(image => image.ProductId == product.Id);

            ProductImageData? imageWithHtml = firstImage;

            List<ProductImageData>? productImages = allProductImages?.FirstOrDefault(x => x.Key == product.Id)?.ToList();

            if (imageWithHtml?.HtmlData == null)
            {
                imageWithHtml = productImages?.FirstOrDefault(image => image.HtmlData != null);
            }

            LegacyHtmlProduct? htmlProduct = null;

            if (imageWithHtml?.HtmlData != null)
            {
                htmlProduct = _legacyProductHtmlService.ParseProductHtml(imageWithHtml.HtmlData);
            }

            LegacyXmlProduct? legacyXmlProduct = legacyXmlObjectData?.Products.FirstOrDefault(xmlProduct => xmlProduct.Id == product.Id);

            List<ProductProperty>? productProperties = allProductProperties?.FirstOrDefault(x => x.Key == product.Id)?.ToList();

            int dataSourceDeviations = AreDataSourcesIdentical(
                dataSources,
                productProperties,
                htmlProduct,
                legacyXmlProduct,
                productIdToExternalXmlDataRelations);

            //if (areDataSourcesIdentical) continue;

            ProductDataComparisonResult result = new()
            {
                Product = product,
                AreDataSourcesIdentical = dataSourceDeviations == 0,
                DataSourceDeviations = dataSourceDeviations,
                ProductProperties = productProperties,
                ProductFirstImage = firstImage,
                ProductImages = productImages,
                HtmlProduct = htmlProduct,
                LegacyXmlProduct = legacyXmlProduct,
            };

            comparisonResults.Add(result);

            if (onNewElementAdded != null)
            {
                await onNewElementAdded.Invoke(result);
            }
        }

        return comparisonResults;
    }

    private static int AreDataSourcesIdentical(
        DataSource[] dataSources,
        List<ProductProperty>? productProperties,
        LegacyHtmlProduct? legacyHtmlProduct,
        LegacyXmlProduct? legacyXmlProduct,
        List<ProductCharacteristicAndExternalXmlDataRelation>? legacyXmlRelations)
    {
        int propsToUseCount = productProperties?.Count ?? 0;
        int htmlPropsToUseCount = legacyHtmlProduct?.Properties?.Count ?? 0;
        int legacyXmlPropsToUseCount = legacyXmlProduct?.Properties?.Count ?? 0;

        bool areAllSourcesFull = true;

        foreach (DataSource dataSource in dataSources)
        {
            int dataCount = dataSource switch
            {
                DataSource.ProductProperties => propsToUseCount,
                DataSource.HtmlProductInImagesOrImagesAll => htmlPropsToUseCount,
                DataSource.LegacyXmlProduct => legacyXmlPropsToUseCount,
                _ => throw new NotSupportedException($"Data source not supported: {dataSource}")
            };

            if (dataCount == 0)
            {
                areAllSourcesFull = false;
                break;
            }
        }

        if (!areAllSourcesFull)
        {
            int maxDataSourceCount = 0;

            foreach (DataSource dataSource in dataSources)
            {
                int dataCount = dataSource switch
                {
                    DataSource.ProductProperties => propsToUseCount,
                    DataSource.HtmlProductInImagesOrImagesAll => htmlPropsToUseCount,
                    DataSource.LegacyXmlProduct => legacyXmlPropsToUseCount,
                    _ => throw new NotSupportedException($"Data source not supported: {dataSource}")
                };

                if (maxDataSourceCount < dataCount)
                {
                    maxDataSourceCount = dataCount;
                }
            }

            return maxDataSourceCount;
        }

        if (dataSources.Contains(DataSource.ProductProperties))
        {
            List<LegacyHtmlProductProperty>? htmlProperties = legacyHtmlProduct?.Properties is not null ? new(legacyHtmlProduct.Properties) : null;
            List<LegacyXmlProductProperty>? xmlProperties = legacyXmlProduct?.Properties is not null ? new(legacyXmlProduct.Properties) : null;

            int deviationsInHtml = 0;
            int deviationsInXml = 0;

            foreach (ProductProperty property in productProperties!)
            {
                bool matchInHtml = false;

                if (dataSources.Contains(DataSource.HtmlProductInImagesOrImagesAll) && htmlProperties != null)
                {
                    for (int i = 0; i < htmlProperties.Count; i++)
                    {
                        LegacyHtmlProductProperty htmlProp = htmlProperties[i];

                        if (htmlProp.Name == property.Characteristic)
                        {
                            matchInHtml = true;
                        }
                        else if (property.ProductCharacteristicId == _defaultLinkCharacteristicId
                            && legacyHtmlProduct!.VendorUrl is not null)
                        {
                            matchInHtml = true;
                        }

                        if (matchInHtml)
                        {
                            htmlProperties.RemoveAt(i);
                            break;
                        }
                    }

                    if (!matchInHtml)
                    {
                        deviationsInHtml++;
                    }
                }

                if (dataSources.Contains(DataSource.LegacyXmlProduct) && xmlProperties != null)
                {
                    bool matchInXml = false;

                    for (int i = 0; i < xmlProperties.Count; i++)
                    {
                        LegacyXmlProductProperty xmlProp = xmlProperties[i];

                        int? xmlPropOrder = int.TryParse(xmlProp.Order, out int order) ? order : null;

                        if (xmlProp.Name == property.Characteristic
                            || legacyXmlRelations?.FirstOrDefault(x => x.XmlName == xmlProp.Name && x.XmlDisplayOrder == xmlPropOrder) is not null)
                        {
                            matchInXml = true;
                        }
                        else if (property.ProductCharacteristicId == _defaultLinkCharacteristicId
                            && legacyXmlProduct!.VendorUrl is not null)
                        {
                            matchInXml = true;
                        }

                        if (matchInXml)
                        {
                            xmlProperties.RemoveAt(i);
                            break;
                        }
                    }

                    if (!matchInXml)
                    {
                        deviationsInXml++;
                    }
                }
            }

            deviationsInHtml += htmlProperties?.Count ?? 0;
            deviationsInXml += xmlProperties?.Count ?? 0;

            return deviationsInHtml > deviationsInXml ? deviationsInHtml : deviationsInXml;
        }

        int totalDeviationsBetweenSources = 0;

        List<LegacyXmlProductProperty>? xmlPropertiesLast = new(legacyXmlProduct!.Properties);

        foreach (LegacyHtmlProductProperty htmlProp in legacyHtmlProduct!.Properties!)
        {
            LegacyXmlProductProperty? matchingXmlProperty = xmlPropertiesLast.FirstOrDefault(x => x.Name == htmlProp.Name);

            if (matchingXmlProperty == null)
            {
                totalDeviationsBetweenSources++;
            }

        }

        return totalDeviationsBetweenSources;
    }
}