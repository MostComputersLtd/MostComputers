using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Razor;
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
        var actionContext = new ActionContext(pageModel.HttpContext, pageModel.RouteData, pageModel.PageContext.ActionDescriptor);

        var viewResult = viewEngine.FindView(actionContext, viewName, false);

        if (viewResult.View == null)
        {
            throw new ArgumentNullException($"{viewName} does not match any available view");
        }

        using (var stringWriter = new StringWriter())
        {
            var viewContext = new ViewContext(
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
}