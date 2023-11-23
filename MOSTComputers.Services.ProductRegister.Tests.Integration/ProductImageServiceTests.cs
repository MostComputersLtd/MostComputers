using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Requests.ProductImage;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOSTComputers.Services.ProductRegister.Tests.Integration.CommonTestElements;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductImageServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductImageServiceTests(
        IProductImageService productImageService,
        IProductService productService)
        : base(Startup.ConnectionString)
    {
        _productImageService = productImageService;
        _productService = productService;
    }

    private const int _useRequiredIdValue = -100;

    private const string _useRequiredNameForUpdateValue = "Use required name for update";

    private readonly IProductImageService _productImageService;
    private readonly IProductService _productService;

    private byte[] LoadImage()
    {
        var pictureFile = Directory.GetFiles("../../../Images/RND_PC_IMG.png").Single();

        return Encoding.ASCII.GetBytes(pictureFile);
    }

    [Fact]
    public void GetAllFirstImagesForSelectionOfProducts_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> productInsertResult = _productService.Insert(ValidProductCreateRequest);

        Assert.True(productInsertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint productId = productInsertResult.AsT0;

        ServiceProductFirstImageCreateRequest createRequest1 = new() { ImageData = LoadImage(), ImageFileExtension = "image/png", ProductId = (int)productId, XML = null };
    }
}