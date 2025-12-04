using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MOSTComputers.UI.Web.Services.Contracts;
public interface IPartialViewRenderService
{
    Task<string> RenderPartialViewToStringAsync(PageModel pageModel, string viewName, object? model);
}