namespace MOSTComputers.UI.Web.RealWorkTesting.Models.Authentication;

public sealed class SignInRequest
{
    public required string Username { get; init; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public List<UserRoles>? Roles { get; set; }
}