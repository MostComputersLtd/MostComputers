using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace MOSTComputers.UI.Web.RealWorkTesting.Utils;

internal static class PageCommonElements
{
    internal static async Task<string> RenderPartialViewToStringAsync(
        this PageModel pageModel,
        string viewName,
        object model,
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider)
    {
        ActionContext actionContext = new(pageModel.HttpContext, pageModel.RouteData, pageModel.PageContext.ActionDescriptor);

        ViewEngineResult viewResult = viewEngine.FindView(actionContext, viewName, false);

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
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            stringWriter,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);

        return stringWriter.ToString();
    }
}