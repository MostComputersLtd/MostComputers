namespace MOSTComputers.UI.Web.Blazor.Endpoints;

public sealed class OpenApiOAuthEndpointDescriptionMetadata(List<string> scopes)
{
    public List<string> Scopes { get; init; } = scopes;
}
