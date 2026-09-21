using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MOSTComputers.Services.Authentication.Contracts;
using MOSTComputers.Services.Identity.DAL.Contracts;
using MOSTComputers.Services.Identity.Models;
using MOSTComputers.Services.Identity.Models.Customers;
using System.Security.Claims;

namespace MOSTComputers.Services.Authentication;

internal sealed class CustomersAndEmployeesAuthenticationService(
    UserManager<PasswordsTableOnlyUser> userManager,
    SignInManager<PasswordsTableOnlyUser> signInManager,
    ICustomersViewLoginDataRepository customersViewLoginDataRepository,
    IHttpContextAccessor httpContextAccessor) : ICustomAuthenticationService
{
    private readonly UserManager<PasswordsTableOnlyUser> _userManager = userManager;
    private readonly SignInManager<PasswordsTableOnlyUser> _signInManager = signInManager;
    private readonly ICustomersViewLoginDataRepository _customersViewLoginDataRepository = customersViewLoginDataRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<SignInResult> PasswordSignInAsync(string username, string password, bool isPersistent, bool lockoutOnFailure)
    {
        PasswordsTableOnlyUser? userInManager = await _userManager.FindByNameAsync(username);

        if (userInManager != null)
        {
            return await _signInManager.PasswordSignInAsync(userInManager, password, isPersistent, lockoutOnFailure);
        }
        CheckPasswordResult checkPasswordResult = await CheckIfCustomerCredentialsExistAsync(username, password);

        if (checkPasswordResult != CheckPasswordResult.Success)
        {
            if (lockoutOnFailure)
            {
                return SignInResult.LockedOut;
            }

            return SignInResult.Failed;
        }

        CustomerLoginData? userInCustomersView = await _customersViewLoginDataRepository.GetLoginDataByUsernameAsync(username);

        if (userInCustomersView is null)
        {
            if (lockoutOnFailure)
            {
                return SignInResult.LockedOut;
            }

            return SignInResult.Failed;
        }

        List<Claim> claims = new()
        {
            new (ClaimTypes.NameIdentifier, userInCustomersView.Id.ToString()),
            new (ClaimTypes.Role, "CustomerInvoiceViewer"),
            new (ClaimTypes.Role, "CustomerOrderViewer"),
        };

        if (userInCustomersView.Username is not null)
        {
            Claim usernameClaim = new(ClaimTypes.Name, userInCustomersView.Username);

            claims.Add(usernameClaim);
        }

        if (userInCustomersView.ContactPerson is not null)
        {
            Claim contactPersonClaim = new("ContactPerson", userInCustomersView.ContactPerson);

            claims.Add(contactPersonClaim);
        }

        AuthenticationProperties authenticationProperties = new();

        if (isPersistent)
        {
            authenticationProperties.IsPersistent = true;
        }

        ClaimsIdentity identity = new(claims, IdentityConstants.ApplicationScheme);

        ClaimsPrincipal principal = new(identity);

        await _httpContextAccessor.HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme, principal, authenticationProperties);

        return SignInResult.Success;
    }

    public async Task<CheckPasswordResult> CheckIfCustomerCredentialsExistAsync(string username, string password)
    {
        return await _customersViewLoginDataRepository.IsPasswordEqualToExistingAsync(username, password);
    }

    public async Task RefreshSignInAsync()
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext;

        AuthenticateResult result = await httpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

        if (result?.Principal == null) return;

        await httpContext.SignInAsync(IdentityConstants.ApplicationScheme, result.Principal, result.Properties);
    }
}
