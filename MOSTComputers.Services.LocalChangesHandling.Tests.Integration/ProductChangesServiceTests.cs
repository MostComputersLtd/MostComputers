using Microsoft.Extensions.DependencyInjection;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.ProductRegister.Models.Requests.Category;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using FluentValidation.Results;
using MOSTComputers.Models.Product.Models.Validation;
using OneOf;
using static MOSTComputers.Services.LocalChangesHandling.Tests.Integration.CommonTestElements;
using MOSTComputers.Services.LocalChangesHandling.Services.Contracts;

namespace MOSTComputers.Services.LocalChangesHandling.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ProductChangesServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ProductChangesServiceTests(IProductChangesService productChangesService)
        : base(Startup.ConnectionString)
    {
        _productChangesService = productChangesService;
    }

    private readonly IProductChangesService _productChangesService;

    private const int _insertRequiredIdValue = -100;
}