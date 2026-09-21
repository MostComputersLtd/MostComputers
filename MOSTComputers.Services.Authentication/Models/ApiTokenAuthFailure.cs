namespace MOSTComputers.Services.Authentication.Models;

public enum ApiTokenAuthFailure
{
    NotFound = 0,
    Expired = 1,
    Revoked = 2
}
