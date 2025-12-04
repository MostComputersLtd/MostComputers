using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Utils.Caching;
internal static class CachingDefaults
{
    internal static readonly TimeSpan EmptyValuesCacheAbsoluteExpiration = TimeSpan.FromSeconds(15);
}