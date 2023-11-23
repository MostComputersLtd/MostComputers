using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Product;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductServiceTests(IProductService productService)
        : base(Startup.ConnectionString)
    {
        _productService = productService;
    }

    private readonly IProductService _productService;

    private static readonly ProductCreateRequest _validCreateRequest = new()
    {
        Name = "Nameee",
        AdditionalWarrantyPrice = 0.00M,
        AdditionalWarrantyTermMonths = 0,
        StandardWarrantyPrice = "3.00",
        StandardWarrantyTermMonths = 36,
        DisplayOrder = 13123,
        Status = ProductStatusEnum.Call,
        PlShow = 0,
        Price1 = 125.65M,
        DisplayPrice = 126.79M,
        Price3 = 128.99M,
        Currency = CurrencyEnum.EUR,
        RowGuid = Guid.NewGuid(),
        Promotionid = null,
        PromRid = null,
        PromotionPictureId = null,
        PromotionExpireDate = null,
        AlertPictureId = null,
        AlertExpireDate = null,
        PriceListDescription = null,
        PartNumber1 = "dsadasdasd",
        PartNumber2 = "12123",
        SearchString = "DC CDKC 33567",

        Properties = new()
        {
            new() { ProductCharacteristicId = 127, DisplayOrder = 12312, Value = "asdaddrer", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
            new() { ProductCharacteristicId = 129, DisplayOrder = 12332, Value = "asdaddrer", XmlPlacement = XMLPlacementEnum.InBottomInThePropertiesList },
        },
        Images = new(),
        ImageFileNames = new(),

        CategoryID = 7,
        ManifacturerId = 12,
        SubCategoryId = null,
    };

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ProductCreateRequest productCreateRequest, bool expected)
    {
        _productService.Insert(productCreateRequest);
    }

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static List<object[]> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data = new()
    {
        new object[2]
        {
            _validCreateRequest,
            true
        }
    };
#pragma warning restore CA2211 // Non-constant fields should not be visible
}