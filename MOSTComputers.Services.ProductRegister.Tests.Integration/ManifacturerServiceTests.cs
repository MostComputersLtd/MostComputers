using FluentValidation.Results;
using MOSTComputers.Models.Product.Models;
using MOSTComputers.Models.Product.Models.Requests.Manifacturer;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Tests.Integration.Common.DependancyInjection;
using OneOf;
using OneOf.Types;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Tests.Integration;

[Collection(DefaultTestCollection.Name)]
public sealed class ManifacturerServiceTests : IntegrationTestBaseForNonWebProjects
{
    public ManifacturerServiceTests(IManifacturerService manifacturerService)
        : base(Startup.ConnectionString)
    {
        _manifacturerService = manifacturerService;
    }

    private readonly IManifacturerService _manifacturerService;

    private static readonly ManifacturerCreateRequest _validCreateRequest = new()
    {
        BGName = null,
        RealCompanyName = "HP@",
        DisplayOrder = 12,
        Active = true
    };

    private static readonly ManifacturerCreateRequest _invalidCreateRequest = new()
    {
        BGName = null,
        RealCompanyName = "HP@",
        DisplayOrder = -12,
        Active = true
    };

    private const int _useRequiredIdValue = -100;

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

    [Fact]
    public void GetAll_ShouldFail_WhenInsertsAreInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _manifacturerService.Insert(_invalidCreateRequest);
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result2 = _manifacturerService.Insert(_invalidCreateRequest);

        Assert.True(result1.IsT1);
        Assert.True(result2.IsT1);

        IEnumerable<Manifacturer> manifacturers = _manifacturerService.GetAll();

        Assert.DoesNotContain(manifacturers,
            x => x.RealCompanyName == _invalidCreateRequest.RealCompanyName
            && x.BGName == _invalidCreateRequest.BGName
            && x.DisplayOrder == _invalidCreateRequest.DisplayOrder
            && x.Active == _invalidCreateRequest.Active);
    }

    [Fact]
    public void GetById_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _manifacturerService.Insert(_validCreateRequest);

        Assert.True(result1.IsT0);

        uint id1 = result1.AsT0;

        Manifacturer? manifacturer = _manifacturerService.GetById(id1);

        Assert.NotNull(manifacturer);

        Assert.Equal(id1, (uint)manifacturer.Id);
        Assert.Equal(_validCreateRequest.RealCompanyName, manifacturer.RealCompanyName);
        Assert.Equal(_validCreateRequest.BGName, manifacturer.BGName);
        Assert.Equal(_validCreateRequest.DisplayOrder, manifacturer.DisplayOrder);
        Assert.Equal(_validCreateRequest.Active, manifacturer.Active);

        // Deterministic delete
        DeleteRange(id1);
    }

    [Fact]
    public void GetById_ShouldFail_WhenIdIsInvalid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _manifacturerService.Insert(_validCreateRequest);

        Assert.True(result1.IsT0);

        uint id = result1.AsT0;

        Manifacturer? manifacturer = _manifacturerService.GetById(0);

        Assert.Null(manifacturer);

        // Deterministic delete
        DeleteRange(id);
    }

    [Theory]
    [MemberData(nameof(Insert_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Insert_ShouldSucceedOrFail_InAnExpectedManner(ManifacturerCreateRequest createRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> result1 = _manifacturerService.Insert(createRequest);

        if (expected)
        {
            Assert.True(result1.IsT0);

            uint id1 = result1.AsT0;

            Manifacturer? manifacturer = _manifacturerService.GetById(id1);

            Assert.Equal(expected, manifacturer is not null);

            Assert.NotNull(manifacturer);

            Assert.Equal(id1, (uint)manifacturer.Id);
            Assert.Equal(createRequest.RealCompanyName, manifacturer.RealCompanyName);
            Assert.Equal(createRequest.BGName, manifacturer.BGName);
            Assert.Equal(createRequest.DisplayOrder, manifacturer.DisplayOrder);
            Assert.Equal(createRequest.Active, manifacturer.Active);

            // Deterministic delete
            DeleteRange(id1);
        }
    }

    public static List<object[]> Insert_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            _validCreateRequest,
            true
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = "Еич Пи @",
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = null,
                RealCompanyName = "HP@",
                DisplayOrder = null,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = string.Empty,
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = "       ",
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = null,
                RealCompanyName = string.Empty,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = null,
                RealCompanyName = "     ",
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = null,
                RealCompanyName = "HP@",
                DisplayOrder = 0,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerCreateRequest()
            {
                BGName = null,
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = false
            },
            false
        },
    };

    [Theory]
    [MemberData(nameof(Update_ShouldSucceedOrFail_InAnExpectedManner_Data))]
    public void Update_ShouldSucceedOrFail_InAnExpectedManner(ManifacturerUpdateRequest updateRequest, bool expected)
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _manifacturerService.Insert(_validCreateRequest);

        Assert.True(insertResult.IsT0);

        uint id1 = insertResult.AsT0;

        if (updateRequest.Id == _useRequiredIdValue)
        {
            updateRequest.Id = (int)id1;
        }

        OneOf<Success, ValidationResult, UnexpectedFailureResult> updateResult = _manifacturerService.Update(updateRequest);

        Assert.Equal(expected,
            updateResult.Match(
            _ => true,
            _ => false,
            _ => false));

        Manifacturer? updatedManifacturer = _manifacturerService.GetById(id1);

        Assert.NotNull(updatedManifacturer);

        if (expected)
        {
            Assert.Equal(id1, (uint)updatedManifacturer.Id);
            Assert.Equal(updateRequest.RealCompanyName, updatedManifacturer.RealCompanyName);
            Assert.Equal(updateRequest.BGName, updatedManifacturer.BGName);
            Assert.Equal(updateRequest.DisplayOrder, updatedManifacturer.DisplayOrder);
            Assert.Equal(updateRequest.Active, updatedManifacturer.Active);
        }
        else
        {
            Assert.Equal(id1, (uint)updatedManifacturer.Id);
            Assert.Equal(_validCreateRequest.RealCompanyName, updatedManifacturer.RealCompanyName);
            Assert.Equal(_validCreateRequest.BGName, updatedManifacturer.BGName);
            Assert.Equal(_validCreateRequest.DisplayOrder, updatedManifacturer.DisplayOrder);
            Assert.Equal(_validCreateRequest.Active, updatedManifacturer.Active);
        }

        // Deterministic delete
        DeleteRange(id1);
    }

    public static List<object[]> Update_ShouldSucceedOrFail_InAnExpectedManner_Data => new()
    {
        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = null,
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = "Еич Пи @",
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = null,
                RealCompanyName = "HP@",
                DisplayOrder = null,
                Active = true
            },
            true
        },

        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = string.Empty,
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = "       ",
                RealCompanyName = "HP@",
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = null,
                RealCompanyName = string.Empty,
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = null,
                RealCompanyName = "     ",
                DisplayOrder = 12,
                Active = true
            },
            false
        },

        new object[2]
        {
            new ManifacturerUpdateRequest()
            {
                Id = _useRequiredIdValue,
                BGName = null,
                RealCompanyName = "HP@",
                DisplayOrder = 0,
                Active = true
            },
            false
        },

        //new object[2]
        //{
        //    new ManifacturerUpdateRequest()
        //    {
        //        Id = _useRequiredIdValue,
        //        BGName = null,
        //        RealCompanyName = "HP@",
        //        DisplayOrder = 12,
        //        Active = 0
        //    },
        //    false
        //},
    };

    [Fact]
    public void Delete_ShouldSucceed_WhenInsertIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _manifacturerService.Insert(_validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id = insertResult.AsT0;

        bool success = _manifacturerService.Delete(id);

        Manifacturer? manifacturer = _manifacturerService.GetById(id);

        Assert.Null(manifacturer);

        // Deterministic delete (just in case delete fails in test)
        DeleteRange(id);
    }

    [Fact]
    public void Delete_ShouldFail_WhenIdIsValid()
    {
        OneOf<uint, ValidationResult, UnexpectedFailureResult> insertResult = _manifacturerService.Insert(_validCreateRequest);

        Assert.True(insertResult.Match(
            _ => true,
            _ => false,
            _ => false));

        uint id = insertResult.AsT0;

        bool success = _manifacturerService.Delete(0);

        Manifacturer? manifacturer = _manifacturerService.GetById(id);

        Assert.NotNull(manifacturer);
        Assert.False(success);

        // Deterministic delete
        DeleteRange(id);
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