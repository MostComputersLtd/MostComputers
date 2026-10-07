using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using OpenIddict.Validation.AspNetCore;

namespace MOSTComputers.UI.Web.Blazor.Utils;

internal static class OpenApiUtils
{
    public const string DocumentTitle = "Portal - MOST Computers API Reference";
    public const string DocumentDescription = "Welcome to the API Reference of MOSTComputers Portal"; 

    public static class SecuritySchemes
    {
        internal const string CookieApplicationSchemeName = "cookie";
        internal const string ApiClientRegisterSchemeName = "oauth2-api-register";
        internal const string ApiClientSchemeName = "oauth2-api";
    }

    public static readonly Dictionary<string, string> AuthenticationSchemesToOpenApiSecuritySchemesMap = new()
    {
        [IdentityConstants.ApplicationScheme] = SecuritySchemes.CookieApplicationSchemeName,
        [OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme] = SecuritySchemes.ApiClientSchemeName,
    };

    internal sealed class SecurityRequirementOperationTransformer(
        IAuthenticationSchemeProvider schemeProvider,
        IAuthorizationPolicyProvider policyProvider,
        IOptions<AuthorizationOptions> authOptionsAccessor)
        : IOpenApiOperationTransformer
    {
        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            HashSet<string>? authenticationSchemes = await GetAuthenticationSchemesFromEndpointAsync(context.Description);

            if (authenticationSchemes == null || authenticationSchemes.Count == 0) return;

            operation.Security ??= [];

            foreach (string authenticationScheme in authenticationSchemes)
            {
                string openApiSchemeName = AuthenticationSchemesToOpenApiSecuritySchemesMap[authenticationScheme];

                OpenApiSecuritySchemeReference securityScheme = new(openApiSchemeName, context.Document);

                List<string> scopes;

                if (authenticationScheme == OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
                {
                    scopes = GetOAuth2Scopes(context.Description);
                }
                else
                {
                    scopes = [];
                }

                OpenApiSecurityRequirement securityRequirement = new()
                {
                    { securityScheme, scopes }
                };

                operation.Security.Add(securityRequirement);
            }
        }

        private async Task<HashSet<string>?> GetAuthenticationSchemesFromEndpointAsync(ApiDescription apiDescription)
        {
            IList<object>? endpointMetadata = apiDescription?.ActionDescriptor?.EndpointMetadata;

            if (endpointMetadata == null) return null;

            foreach (object metadata in endpointMetadata)
            {
                if (metadata is IAllowAnonymous) return null;
            }

            AuthorizationOptions options = authOptionsAccessor.Value;

            List<IAuthorizeData> authorizeData = [];
            List<AuthorizationPolicy> policiesFromMetadata = [];

            foreach (object metadata in endpointMetadata)
            {
                if (metadata is IAuthorizeData data)
                {
                    authorizeData.Add(data);
                }

                if (metadata is AuthorizationPolicy policy)
                {
                    policiesFromMetadata.Add(policy);
                }
            }

            HashSet<string> effectiveSchemes = new(StringComparer.OrdinalIgnoreCase);

            bool hasAuthorizeAttributes = authorizeData.Count > 0;

            AuthorizationPolicy? fallbackOrDefaultPolicy = hasAuthorizeAttributes ? options.DefaultPolicy : options.FallbackPolicy;

            if (hasAuthorizeAttributes)
            {
                foreach (IAuthorizeData data in authorizeData)
                {
                    if (!string.IsNullOrWhiteSpace(data.AuthenticationSchemes))
                    {
                        string[] schemes = data.AuthenticationSchemes.Split(
                            ',',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        foreach (string scheme in schemes)
                        {
                            effectiveSchemes.Add(scheme);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(data.Policy))
                    {
                        AuthorizationPolicy? policy = await policyProvider.GetPolicyAsync(data.Policy);

                        if (policy != null)
                        {
                            foreach (string scheme in policy.AuthenticationSchemes)
                            {
                                effectiveSchemes.Add(scheme);
                            }
                        }
                    }
                } 
            }

            if (effectiveSchemes.Count == 0)
            {
                foreach (AuthorizationPolicy policy in policiesFromMetadata)
                {
                    foreach (string scheme in policy.AuthenticationSchemes)
                    {
                        effectiveSchemes.Add(scheme);
                    }
                }
            }

            if (effectiveSchemes.Count == 0)
            {
                if (fallbackOrDefaultPolicy?.AuthenticationSchemes.Count > 0)
                {
                    foreach (string scheme in fallbackOrDefaultPolicy.AuthenticationSchemes)
                    {
                        effectiveSchemes.Add(scheme);
                    }
                }
                else
                {
                    AuthenticationScheme? defaultAuthenticate = await schemeProvider.GetDefaultAuthenticateSchemeAsync();

                    if (defaultAuthenticate?.Name != null)
                    {
                        effectiveSchemes.Add(defaultAuthenticate.Name);
                    }
                }
            }

            return effectiveSchemes;
        }

        private static List<string> GetOAuth2Scopes(ApiDescription apiDescription)
        {
            List<string> scopes = new();

            foreach (object metadata in apiDescription.ActionDescriptor.EndpointMetadata)
            {
                if (metadata is not OpenApiOAuthEndpointDescriptionMetadata oauth2EndpointDescriptionMetadata) continue;

                foreach (string scope in oauth2EndpointDescriptionMetadata.Scopes)
                {
                    if (!scopes.Contains(scope))
                    {
                        scopes.Add(scope);
                    }
                }
            }

            return scopes;
        }
    }
}
