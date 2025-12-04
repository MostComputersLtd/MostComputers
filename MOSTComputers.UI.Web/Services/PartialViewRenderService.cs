using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MOSTComputers.UI.Web.Services.Contracts;

namespace MOSTComputers.UI.Web.Services;
internal class PartialViewRenderService : IPartialViewRenderService
{
    public PartialViewRenderService(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
    }

    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;

    public async Task<string> RenderPartialViewToStringAsync(PageModel pageModel, string viewName, object? model)
    {
        ActionContext actionContext = new(pageModel.HttpContext, pageModel.RouteData, pageModel.PageContext.ActionDescriptor);

        ViewEngineResult viewResult = _viewEngine.FindView(actionContext, viewName, false);

        if (viewResult.View is null)
        {
            throw new ArgumentNullException($"{viewName} does not match any available view");
        }

        using StringWriter stringWriter = new();

        ViewContext viewContext = new(
            actionContext,
            viewResult.View,
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            },
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            stringWriter,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);

        return stringWriter.ToString();
    }
}