using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.Authentication.Models;

namespace MOSTComputers.Services.Authentication.DataAccess.Mapping;

using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils.ApiSecretsTable;

internal sealed class ApiSecretEntityMap : EntityMap<ApiSecret>
{
    public ApiSecretEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumn);
        Map(x => x.ClientId).ToColumn(ClientIdColumn);
        Map(x => x.SecretHash).ToColumn(SecretHashColumn);
        Map(x => x.CreatedAt).ToColumn(CreatedAtColumn);
        Map(x => x.ExpiresAt).ToColumn(ExpiresAtColumn);
        Map(x => x.RevokedAt).ToColumn(RevokedAtColumn);
        Map(x => x.LastUsedAt).ToColumn(LastUsedAtColumn);
    }
}
