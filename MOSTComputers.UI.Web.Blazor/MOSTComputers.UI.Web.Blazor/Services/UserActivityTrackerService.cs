namespace MOSTComputers.UI.Web.Blazor.Services;

internal sealed class UserActivityTrackerService
{
    public DateTime LastActivityUtc { get; set; } = DateTime.UtcNow;
}