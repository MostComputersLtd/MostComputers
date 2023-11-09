using MOSTComputers.Tests.Integration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.DAL.Tests.Integration;

public sealed class CustomWebApplicationFactory : WebApplicationFactoryWithContainer<MOSTComputers.Services.ProductRegister.IEntryPoint>, IAsyncLifetime
{
    protected override string get_connectionString => "Data Source=(local);Initial Catalog=MostDBNew_Test;Integrated Security=True;";

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}