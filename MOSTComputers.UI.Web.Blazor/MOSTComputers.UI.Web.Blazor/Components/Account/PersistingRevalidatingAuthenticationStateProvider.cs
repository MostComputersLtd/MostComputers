using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.UI.Web.Blazor.Client;
using MOSTComputers.UI.Web.Blazor.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace MOSTComputers.UI.Web.Blazor.Components.Account;
// This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
// every 30 minutes an interactive circuit is connected. It also uses PersistentComponentState to flow the
// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
internal sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private static readonly TimeSpan _maxUserIdleTime = TimeSpan.FromMinutes(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PersistentComponentState _state;
    private readonly UserActivityTrackerService _userActivityTrackerService;
    private readonly IdentityOptions _options;

    private readonly PersistingComponentStateSubscription _subscription;

    private Task<AuthenticationState>? _authenticationStateTask;

    public PersistingRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        PersistentComponentState persistentComponentState,
        UserActivityTrackerService userActivityTrackerService,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        _scopeFactory = serviceScopeFactory;
        _state = persistentComponentState;
        _userActivityTrackerService = userActivityTrackerService;
        _options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;

        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(1);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        Claim? roleClaim = authenticationState.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

        bool isAdmin = roleClaim?.Value.Contains("Admin") ?? false;

        if (!isAdmin && !IsUserActive())
        {
            return false;
        }

        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

        UserManager<PasswordsTableOnlyUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<PasswordsTableOnlyUser>>();

        bool isSecurityStampValid = await ValidateSecurityStampAsync(userManager, authenticationState.User);

        return isSecurityStampValid;
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<PasswordsTableOnlyUser> userManager, ClaimsPrincipal principal)
    {
        PasswordsTableOnlyUser? user = await userManager.GetUserAsync(principal);

        if (user is null)
        {
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }
        else
        {
            string? principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);

            string userStamp = await userManager.GetSecurityStampAsync(user);

            return principalStamp == userStamp;
        }
    }

    private bool IsUserActive()
    {
        return DateTime.UtcNow - _userActivityTrackerService.LastActivityUtc < _maxUserIdleTime;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (_authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        AuthenticationState authenticationState = await _authenticationStateTask;

        ClaimsPrincipal principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            string? userId = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
            string? username = principal.FindFirst(_options.ClaimsIdentity.UserNameClaimType)?.Value;

            if (userId != null && username != null)
            {
                _state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId,
                    Username = username,
                });
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        _subscription.Dispose();

        AuthenticationStateChanged -= OnAuthenticationStateChanged;

        base.Dispose(disposing);
    }
}