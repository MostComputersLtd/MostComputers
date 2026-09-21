namespace MOSTComputers.Services.Authentication.DataAccess;

internal static class TableAndColumnNameUtils
{
    internal const string ApiSecretsTableName = "[dbo].[ApiSecrets]";
    internal const string ApiSecretPermissionsTableName = "[dbo].[ApiSecretPermissions]";
    internal const string ApiTokensTableName = "[dbo].[ApiTokens]";

    internal static class ApiSecretsTable
    {
        internal const string IdColumn = "Id";
        internal const string ClientIdColumn = "BID";
        internal const string SecretHashColumn = "SecretHash";
        internal const string CreatedAtColumn = "CreatedAt";
        internal const string ExpiresAtColumn = "ExpiresAt";
        internal const string RevokedAtColumn = "RevokedAt";
        internal const string LastUsedAtColumn = "LastUsedAt";
    }

    internal static class ApiSecretPermissionsTable
    {
        internal const string SecretIdColumn = "SecretId";
        internal const string ValueColumn = "Value";
    }

    internal static class ApiTokensTable
    {
        internal const string IdColumn = "Id";
        internal const string SecretIdColumn = "SecretId";
        internal const string ClientIdColumn = "BID";
        internal const string TokenHashColumn = "TokenHash";
        internal const string CreatedAtColumn = "CreatedAt";
        internal const string ExpiresAtColumn = "ExpiresAt";
        internal const string RevokedAtColumn = "RevokedAt";
        internal const string LastUsedAtColumn = "LastUsedAt";
    }
}
