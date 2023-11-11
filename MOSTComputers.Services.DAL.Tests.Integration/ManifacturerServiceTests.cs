using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

public sealed class ManifacturerServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ManifacturerServiceTests(IManifacturerService manifacturerService)
        : base(Startup.ConnectionString)
    {
        _manifacturerService = manifacturerService;
    }

    private readonly IManifacturerService _manifacturerService;

    private readonly ManifacturerCreateRequest _validCreateRequest = new()
    {
        BGName = null,
        RealCompanyName = "HP@",
        DisplayOrder = 12,
        Active = 1
    };

    [Fact]
    public void GetAll_ShouldSucceed_WhenInsertsAreValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _manifacturerService.Insert(_validCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result2 = _manifacturerService.Insert(_validCreateRequest);

        Assert.True(result1.IsT0);
        Assert.True(result2.IsT0);

        IEnumerable<Manifacturer> manifacturers = _manifacturerService.GetAll();

        Assert.True(manifacturers.Count() >= 2);

        uint id1 = result1.AsT0;
        uint id2 = result2.AsT0;

        Assert.Contains(manifacturers, x => x.Id == id1);
        Assert.Contains(manifacturers, x => x.Id == id2);

        // Deterministic delete
        DeleteRange(id1, id2);
    }

    private bool DeleteRange(params uint[] ids)
    {
        foreach (uint id in ids)
        {
            bool success = _manifacturerService.Delete(id);

            if (!success) return false;
        }

        return true;
    }
}