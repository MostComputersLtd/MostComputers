using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.Authentication.Models;

namespace MOSTComputers.Services.Authentication.DataAccess.Mapping;

using static MOSTComputers.Services.Authentication.DataAccess.TableAndColumnNameUtils.ApiTokensTable;

internal sealed class ApiTokenEntityMap : EntityMap<ApiToken>
{
    public ApiTokenEntityMap()
    {
        Map(x => x.Id).ToColumn(IdColumn);
        Map(x => x.SecretId).ToColumn(SecretIdColumn);
        Map(x => x.ClientId).ToColumn(ClientIdColumn);
        Map(x => x.TokenHash).ToColumn(TokenHashColumn);
        Map(x => x.CreatedAt).ToColumn(CreatedAtColumn);
        Map(x => x.ExpiresAt).ToColumn(ExpiresAtColumn);
        Map(x => x.RevokedAt).ToColumn(RevokedAtColumn);
        Map(x => x.LastUsedAt).ToColumn(LastUsedAtColumn);
    }
}
