namespace MOSTComputers.UI.Web.Models.Authentication;

public sealed class LogInRequest
{
    public required string Username { get; init; }
    public required string Password { get; set; }
}