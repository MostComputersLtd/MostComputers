using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using OneOf;
using OneOf.Types;
using static MOSTComputers.UI.Web.Blazor.Components.PromotionGroups.GroupPromotionEditor;
using static MOSTComputers.UI.Web.Blazor.Components.PromotionGroups.GroupPromotionEditorImage;
using static MOSTComputers.Utils.Files.FilePathUtils;

namespace MOSTComputers.UI.Web.Blazor.Components.PromotionGroups;

public static class PromotionGroupsPageComponentEndpoints
{
    public sealed class GroupPromotionSearchData
    {
        public string? SearchData { get; set; }
        public int? PromotionGroupId { get; set; }
    }

    public sealed class GroupPromotionImageData
    {
        public int? Index { get; set; }
        public string? ImageUrl { get; set; }
    }

    public sealed class GroupPromotionCreateRequest
    {
        public string? Name { get; set; }
        public int? GroupId { get; set; }
        public string? HtmlContent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? Disabled { get; set; }
        public bool? Restricted { get; set; }
        public bool? MemberOfDefaultGroup { get; set; }
        public int? DefaultGroupPriority { get; set; }
        public List<IFormFile>? PromotionImageCreateRequests { get; set; }
    }

    public sealed class GroupPromotionUpdateRequest
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? GroupId { get; set; }
        public string? HtmlContent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? Disabled { get; set; }
        public bool? Restricted { get; set; }
        public bool? MemberOfDefaultGroup { get; set; }
        public int? DefaultGroupPriority { get; set; }
        public List<int>? ImageIdsToKeep { get; set; }
        public List<IFormFile>? PromotionImageCreateRequests { get; set; }
    }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "promotionGroups";

    private const string _blobUrlSchema = "blob:";

    public static IEndpointConventionBuilder MapPromotionGroupPageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute);

        endpointGroup.MapGet("/editor/{id:int?}", GetGroupPromotionEditorForExistingItemAsync);
        endpointGroup.MapGet("/editor/new", GetGroupPromotionEditorForNewItemAsync);

        endpointGroup.MapPost("/search", GetGroupPromotionsListAsync);
        endpointGroup.MapPost("/images", GetGroupPromotionImageComponent);
        endpointGroup.MapPost("/create", InsertNewPromotionAsync);

        endpointGroup.MapPut("/update", UpdatePromotionAsync);

        return endpointGroup;
    }

    private static async Task<IResult> GetGroupPromotionsListAsync(
        HttpContext httpContext,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromBody] GroupPromotionSearchData? searchData = null)
    {
        httpContext.Response.ContentType = "application/html";

        List<GroupPromotionContent> groupPromotions;

        if (searchData?.PromotionGroupId != null)
        {
            groupPromotions = await groupPromotionService.GetAllInGroupAsync(searchData.PromotionGroupId.Value);
        }
        else
        {
            groupPromotions = await groupPromotionService.GetAllAsync();
        }

        if (searchData?.SearchData is not null)
        {
            List<GroupPromotionContent> filteredGroupPromotions = new();

            string searchDataTrimmed = searchData.SearchData.Trim();

            if (int.TryParse(searchDataTrimmed, out int idInSearch))
            {
                int contentWithIdIndex = groupPromotions.FindIndex(x => x.Id == idInSearch);

                if (contentWithIdIndex >= 0)
                {
                    GroupPromotionContent contentWithId = groupPromotions[contentWithIdIndex];

                    filteredGroupPromotions.Add(contentWithId);

                    groupPromotions.RemoveAt(contentWithIdIndex);
                }
            }    

            string[] searchTerms = searchData.SearchData.Split(' ');

            foreach (GroupPromotionContent groupPromotionContent in groupPromotions)
            {
                if (groupPromotionContent.Name is null) continue;

                bool containsAllSearchTerms = true;

                foreach (string searchTerm in searchTerms)
                {
                    if (!groupPromotionContent.Name.Contains(searchTerm))
                    {
                        containsAllSearchTerms = false;

                        break;
                    }
                }

                if (containsAllSearchTerms)
                {
                    filteredGroupPromotions.Add(groupPromotionContent);
                }
            }

            groupPromotions = filteredGroupPromotions;
        }

        return new RazorComponentResult<GroupPromotionList>(new
        {
            Promotions = groupPromotions,
        });
    }

    private static async Task<IResult> GetGroupPromotionEditorForNewItemAsync(
        HttpContext httpContext,
        [FromServices] IPromotionGroupService promotionGroupService,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromServices] IGroupPromotionImageFileService groupPromotionImageFileService)
    {
        return await GetGroupPromotionEditorAsync(httpContext, promotionGroupService, groupPromotionService, groupPromotionImageFileService, null);
    }

    private static async Task<IResult> GetGroupPromotionEditorForExistingItemAsync(
        HttpContext httpContext,
        [FromServices] IPromotionGroupService promotionGroupService,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromServices] IGroupPromotionImageFileService groupPromotionImageFileService,
        [FromRoute] int? id = null)
    {
        if (id == null) return Results.NotFound();

        return await GetGroupPromotionEditorAsync(httpContext, promotionGroupService, groupPromotionService, groupPromotionImageFileService, id);
    }

    private static async Task<IResult> GetGroupPromotionEditorAsync(
        HttpContext httpContext,
        IPromotionGroupService promotionGroupService,
        IGroupPromotionService groupPromotionService,
        IGroupPromotionImageFileService groupPromotionImageFileService,
        int? id)
    {
        List<PromotionGroup> promotionGroups = await promotionGroupService.GetAllAsync();

        if (id == null)
        {
            GroupPromotionContentEditorData newPromotionData = new()
            {
                Id = null,
                Name = null,
                GroupId = null,
                HtmlContent = null,
                StartDate = null,
                ExpirationDate = null,
                DisplayOrder = null,
                DateModified = null,
                Disabled = null,
                Restricted = null,
                MemberOfDefaultGroup = null,
                DefaultGroupPriority = null,
                Images = new(),
            };

            return new RazorComponentResult<GroupPromotionEditor>(new
            {
                PromotionData = newPromotionData,
                AvailablePromotionGroups = promotionGroups,
            });
        }

        GroupPromotionContent? groupPromotionContent = await groupPromotionService.GetByIdAsync(id.Value);

        if (groupPromotionContent == null)
        {
            return Results.NotFound(id.Value);
        }

        HttpRequest httpRequest = httpContext.Request;

        string baseUrl = $"https://{httpRequest.Host}{httpRequest.PathBase}/";

        List<GroupPromotionImageFileData> groupPromotionImageFiles = await groupPromotionImageFileService.GetAllInPromotionAsync(id.Value);

        List<GroupPromotionImageEditorData> groupPromotionImageDatas = new();

        for (int i = 0; i < groupPromotionImageFiles.Count; i++)
        {
            GroupPromotionImageFileData imageFile = groupPromotionImageFiles[i];

            GroupPromotionImageEditorData imageData = new()
            {
                Index = i,
                ImageId = imageFile.ImageId,
                ImageUrl = CombinePathsWithSeparator('/', baseUrl, GroupPromotionImageFileEndpoints.EndpointGroupRoute, imageFile.Id.ToString()),
                Title = imageFile.FileName,
            };

            groupPromotionImageDatas.Add(imageData);
        }

        GroupPromotionContentEditorData groupPromotionContentEditorData = new()
        {
            Id = groupPromotionContent.Id,
            Name = groupPromotionContent.Name,
            GroupId = groupPromotionContent.GroupId,
            HtmlContent = groupPromotionContent.HtmlContent,
            StartDate = groupPromotionContent.StartDate,
            ExpirationDate = groupPromotionContent.ExpirationDate,
            DisplayOrder = groupPromotionContent.DisplayOrder,
            DateModified = groupPromotionContent.DateModified,
            Disabled = groupPromotionContent.Disabled,
            Restricted = groupPromotionContent.Restricted,
            MemberOfDefaultGroup = groupPromotionContent.MemberOfDefaultGroup,
            DefaultGroupPriority = groupPromotionContent.DefaultGroupPriority,
            Images = groupPromotionImageDatas,
        };

        return new RazorComponentResult<GroupPromotionEditor>(new
        {
            PromotionData = groupPromotionContentEditorData,
            AvailablePromotionGroups = promotionGroups,
        });
    }

    private static IResult GetGroupPromotionImageComponent(
        HttpContext httpContext,
        [FromBody] GroupPromotionImageData imageUrlData)
    {
        string? imageUrl = imageUrlData?.ImageUrl;

        if (imageUrlData?.Index == null
            || imageUrl == null
            || !Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
        {
            return Results.NotFound();
        }

        string? imageUrlToCompare = imageUrl;

        if (imageUrl.StartsWith(_blobUrlSchema))
        {
            imageUrlToCompare = imageUrl[_blobUrlSchema.Length..];
        }

        if (!Uri.TryCreate(imageUrlToCompare, UriKind.Absolute, out Uri? result)
            || !DoesUriMatchOriginWithRequest(result, httpContext.Request))
        {
            return Results.BadRequest();
        }

        return new RazorComponentResult<GroupPromotionEditorImage>(new
        {
            Image = new GroupPromotionImageEditorData()
            {
                Index = imageUrlData.Index.Value,
                ImageUrl = imageUrl,
                Title = imageUrl,
            }
        });
    }

    private static async Task<IResult> InsertNewPromotionAsync(
        [FromForm] GroupPromotionCreateRequest groupPromotionCreateRequest,
        [FromServices] IGroupPromotionService groupPromotionService)
    {
        List<ServiceGroupPromotionImageCreateRequest> imageCreateRequests = new();

        if (groupPromotionCreateRequest.PromotionImageCreateRequests != null)
        {
            foreach (IFormFile imageFile in groupPromotionCreateRequest.PromotionImageCreateRequests)
            {
                if (imageFile.Length > 10 * 1024 * 1024)
                {
                    return Results.BadRequest();
                }

                Stream imageStream = imageFile.OpenReadStream();

                using MemoryStream memoryStream = new();

                await imageStream.CopyToAsync(memoryStream);

                ServiceGroupPromotionImageCreateRequest imageCreateRequest = new()
                {
                    Image = memoryStream.ToArray(),
                    ContentType = imageFile.ContentType,
                    FileExtension = Path.GetExtension(imageFile.FileName),
                    CustomFileNameWithoutExtension = null,
                };
            }
        }

        ServiceGroupPromotionContentCreateRequest createRequest = new()
        {
            Name = groupPromotionCreateRequest.Name,
            GroupId = groupPromotionCreateRequest.GroupId,
            HtmlContent = groupPromotionCreateRequest.HtmlContent,
            StartDate = groupPromotionCreateRequest.StartDate,
            ExpirationDate = groupPromotionCreateRequest.ExpirationDate,
            DisplayOrder = groupPromotionCreateRequest.DisplayOrder,
            Disabled = groupPromotionCreateRequest.Disabled,
            Restricted = groupPromotionCreateRequest.Restricted,
            MemberOfDefaultGroup = groupPromotionCreateRequest.MemberOfDefaultGroup,
            DefaultGroupPriority = groupPromotionCreateRequest.DefaultGroupPriority,
            PromotionImageCreateRequests = imageCreateRequests,
        };

        OneOf<GroupPromotionCreateResult, ValidationResult, ImageFileAlreadyExistsResult, UnexpectedFailureResult> result
            = await groupPromotionService.InsertAsync(createRequest);

        return result.Match(
            createResult => Results.Ok(createResult.Id),
            validationResult => Results.BadRequest(),
            imageFileAlreadyExistsResult => Results.BadRequest(),
            unexpectedFailureResult => Results.StatusCode(500));
    }

    private static async Task<IResult> UpdatePromotionAsync(
        [FromForm] GroupPromotionUpdateRequest groupPromotionUpdateRequest,
        [FromServices] IGroupPromotionService groupPromotionService,
        [FromServices] IGroupPromotionImageCrudService groupPromotionImageService,
        [FromServices] IGroupPromotionImageFileService groupPromotionImageFileService)
    {
        if (groupPromotionUpdateRequest.Id == null) return Results.BadRequest();

        List<ServiceGroupPromotionImageUpsertRequest> imageUpsertRequests = new();

        if (groupPromotionUpdateRequest.PromotionImageCreateRequests != null)
        {
            foreach (IFormFile imageFile in groupPromotionUpdateRequest.PromotionImageCreateRequests)
            {
                if (imageFile.Length > 10 * 1024 * 1024)
                {
                    return Results.BadRequest();
                }

                Stream imageStream = imageFile.OpenReadStream();

                using MemoryStream memoryStream = new();

                await imageStream.CopyToAsync(memoryStream);

                ServiceGroupPromotionImageUpsertRequest imageUpsertRequest = new()
                {
                    Image = memoryStream.ToArray(),
                    ContentType = imageFile.ContentType,
                    FileExtension = Path.GetExtension(imageFile.FileName),
                    CustomFileNameWithoutExtension = null,
                };

                imageUpsertRequests.Add(imageUpsertRequest);
            }
        }

        if (groupPromotionUpdateRequest.ImageIdsToKeep != null)
        {
            List<GroupPromotionImage> currentImages
                = await groupPromotionImageService.GetAllInPromotionAsync(groupPromotionUpdateRequest.Id.Value);

            List<GroupPromotionImageFileData> currentImageFiles
                = await groupPromotionImageFileService.GetAllInPromotionAsync(groupPromotionUpdateRequest.Id.Value);

            foreach (int idToKeep in groupPromotionUpdateRequest.ImageIdsToKeep)
            {
                GroupPromotionImage? image = currentImages.FirstOrDefault(x => x.Id == idToKeep);
                GroupPromotionImageFileData? imageFile = currentImageFiles.FirstOrDefault(x => x.ImageId == idToKeep);

                if (image == null || imageFile == null) continue;

                string contentTypeFromImage = image.ContentType!.Trim();

                if (contentTypeFromImage.StartsWith("image/."))
                {
                    int indexOfDot = contentTypeFromImage.IndexOf('.');

                    contentTypeFromImage = contentTypeFromImage.Remove(indexOfDot, 1);
                }

                ServiceGroupPromotionImageUpsertRequest imageUpsertRequest = new()
                {
                    ExistingImageId = idToKeep,
                    ContentType = contentTypeFromImage,
                    Image = image.Image!,
                    FileExtension = Path.GetExtension(imageFile.FileName),
                    CustomFileNameWithoutExtension = null,
                };

                imageUpsertRequests.Add(imageUpsertRequest);
            }
        }

        ServiceGroupPromotionContentUpdateRequest updateRequest = new()
        {
            Id = groupPromotionUpdateRequest.Id.Value,
            Name = groupPromotionUpdateRequest.Name,
            GroupId = groupPromotionUpdateRequest.GroupId,
            HtmlContent = groupPromotionUpdateRequest.HtmlContent,
            StartDate = groupPromotionUpdateRequest.StartDate,
            ExpirationDate = groupPromotionUpdateRequest.ExpirationDate,
            DisplayOrder = groupPromotionUpdateRequest.DisplayOrder,
            Disabled = groupPromotionUpdateRequest.Disabled,
            Restricted = groupPromotionUpdateRequest.Restricted,
            MemberOfDefaultGroup = groupPromotionUpdateRequest.MemberOfDefaultGroup,
            DefaultGroupPriority = groupPromotionUpdateRequest.DefaultGroupPriority,
            ImageRequests = imageUpsertRequests,
        };

        OneOf<Success, OneOf.Types.NotFound, ValidationResult, ImageFileAlreadyExistsResult, FileDoesntExistResult, UnexpectedFailureResult> result
            = await groupPromotionService.UpdateAsync(updateRequest);

        return result.Match(
            createResult => Results.Ok(),
            notFound => Results.BadRequest(),
            validationResult => Results.BadRequest(),
            imageFileAlreadyExistsResult => Results.BadRequest(),
            fileDoesntExistResult => Results.BadRequest(),
            unexpectedFailureResult => Results.StatusCode(500));
    }

    private static bool DoesUriMatchOriginWithRequest(Uri uri, HttpRequest? httpRequest)
    {
        return uri.Host == httpRequest?.Host.Host
            && uri.Port == httpRequest?.Host.Port;
    }
}
