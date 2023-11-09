using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Tests.Integration;

[CollectionDefinition(Name)]
public sealed class DefaultTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "DefaultTestCollection";
}