using MOSTComputers.Models.Product.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Models;
using MOSTComputers.Services.HTMLAndXMLDataOperations.Services;
using OneOf;

namespace MOSTComputers.Services.HTMLAndXMLDataOperations;

/*
 * This is a dummy class, whose only purpose is to feed the root project path into the [CallerFilePath] callerFilePath argument
 * of the ProductHtmlService.GetHtmlFromProductInternal and ProductHtmlService.TryGetHtmlFromProductInternal calls below,
 * so that i can use it to retrieve the root_project_folder/XSL/Product.xsl file.
 * After considering other solutions, i decided this is the best, since the configuration remains unchanged, regardless of the hosting details.
 * This will work as long as this file is in the root project folder.
 * 
 *  - I didnt use the appsettings.json or the builder.Environment.ContentRootPath of the UI project, since i would need to make assumtions about
 * the UI project's location, relative to the location of this project, and the name of this project's containing folder. Also the Product.xsl
 * access should be something managed by this project, as its this project's responsability, and the used xsl file will not change,
 * regardless of hosting details, or when this project is used by different front-end projects.
 * 
 *  - I avoided hardcoding the paths, including relative ones for the same purpose.
 *  
 *  - I needed to include this class, because, if i didn't, i would need to make assumptions about where the ProductHtmlService class is
 *  located in the project, or about the name of this project's containing folder.
 *  
 * By using this solution, i only need to make an assumption about where the Product.xsl file is located in the project,
 * which is unavoidable and acceptable, since static files like this won't be moved.
*/
internal static class RootProjectFolderProductHtmlMethodCaller
{
    internal static string CallGetProductToHtmlMethod(ProductHtmlService service, Product product)
    {
        // Here is the where the CallerFilePath is fed to the GetHtmlFromProductInternal call (because the second argument has a default value)
        return service.GetHtmlFromProductInternal(product);
    }

    internal static OneOf<string, InvalidXmlResult> CallTryGetProductToHtmlMethod(ProductHtmlService service, Product product)
    {
        // Here is the where the CallerFilePath is fed to the GetHtmlFromProductInternal call (because the second argument has a default value)
        return service.TryGetHtmlFromProductInternal(product);
    }
}