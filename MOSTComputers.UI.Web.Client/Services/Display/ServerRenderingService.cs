using Azure;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;

namespace MOSTComputers.UI.Web.Client.Services.Display;

public class ServerRenderingService
{
    private class FakeView() : IView
    {
        public string Path => string.Empty;

        public Task RenderAsync(ViewContext context)
        {
            return Task.CompletedTask;
        }
    }

    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IViewComponentHelper _viewComponentHelper;

    public ServerRenderingService(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IViewComponentHelper viewComponentHelper)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _viewComponentHelper = viewComponentHelper;
    }

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
            pageModel.TempData,
            stringWriter,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);

        return stringWriter.ToString();
    }

    public async Task<string> RenderComponentAsync(PageModel pageModel, string viewName, object? model)
    {
        using var writer = new StringWriter();

        ViewContext viewContext = new(
            pageModel.PageContext,
            new FakeView(),
            new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            },
            pageModel.TempData,
            writer,
            new HtmlHelperOptions());

        (_viewComponentHelper as IViewContextAware)?.Contextualize(viewContext);

        IHtmlContent result = await _viewComponentHelper.InvokeAsync(viewName, model);

        result.WriteTo(writer, HtmlEncoder.Default);

        return writer.ToString();
    }
}