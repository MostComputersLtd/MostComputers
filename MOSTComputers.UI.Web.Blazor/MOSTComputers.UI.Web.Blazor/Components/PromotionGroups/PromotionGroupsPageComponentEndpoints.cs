using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MOSTComputers.Models.FileManagement.Models;
using MOSTComputers.Models.Product.Models.Promotions.Groups;
using MOSTComputers.Models.Product.Models.Validation;
using MOSTComputers.Services.DataAccess.Products.Models.Responses.Promotions.GroupPromotionImages;
using MOSTComputers.Services.ProductRegister.Models.Requests.PromotionGroups;
using MOSTComputers.Services.ProductRegister.Models.Responses;
using MOSTComputers.Services.ProductRegister.Services.Contracts;
using MOSTComputers.Services.ProductRegister.Services.Promotions.Groups.Contracts;
using MOSTComputers.UI.Web.Blazor.Endpoints;
using MOSTComputers.UI.Web.Blazor.Endpoints.Images;
using OneOf;
using OneOf.Types;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
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
        public bool? ActiveOnly { get; set; }
    }

    public sealed class GroupPromotionImageData
    {
        public int? Index { get; set; }
        public string? ImageUrl { get; set; }
    }

    public sealed class PromotionGroupLogoImageData
    {
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
        public List<GroupPromotionImageKeepRequest>? ImageIdsToKeep { get; set; }
        public List<GroupPromotionImageCreateRequest>? PromotionImageCreateRequests { get; set; }
    }

    public sealed class GroupPromotionImageKeepRequest
    {
        public int? Id { get; set; }
        public int? ImageIndex { get; set; }
    }

    public sealed class GroupPromotionImageCreateRequest
    {
        public IFormFile? ImageFile { get; set; }
        public int? ImageIndex { get; set; }
    }

    public sealed class PromotionGroupCreateRequest
    {
        public string? Name { get; init; }
        public int? DisplayOrder { get; init; }
        public IFormFile? LogoImage { get; init; }
    }

    public sealed class PromotionGroupUpdateRequest
    {
        public int? Id { get; init; }
        public string? Name { get; init; }
        public int? DisplayOrder { get; init; }
        public bool? PreserveOldImage { get; init; }
        public IFormFile? NewLogoImage { get; init; }
    }

    private readonly struct InvalidImageRequestResult { }

    internal const string EndpointGroupRoute = EndpointRoutingCommonElements.ApiEndpointPathPrefix + "components/" + "promotionGroups";

    private const string _blobUrlSchema = "blob:";

    private const string _serverImageRepresentationStart = "|/imageStart/%|";
    private const string _serverImageRepresentationEnd = "|/imageEnd/%|";

    private const string _serverImageIndexPrefix = "index=";
    private const int _maxPromotionImageSize = 10 * 1024 * 1024;
    private const int _maxPromotionGroupLogoImageSize = 10 * 1024 * 1024;

    public static IEndpointConventionBuilder MapPromotionGroupPageComponentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder endpointGroup = endpoints.MapGroup(EndpointGroupRoute)
            .RequireAuthorization(x => x.RequireRole("PromotionEditor"));

        endpointGroup.MapGet("/editor/new", GetGroupPromotionEditorForNewItemAsync);
        endpointGroup.MapGet("/editor/{id:int?}", GetGroupPromotionEditorForExistingItemAsync);

        endpointGroup.MapGet("/groupEditorPopup/new", GetPromotionGroupEditorForNewItem);
        endpointGroup.MapGet("/groupEditorPopup/{id:int?}", GetPromotionGroupEditorForExistingItemAsync);

        endpointGroup.MapGet("/relatedProduct/{productId:int?}", GetGroupPromotionRelatedProductComponentAsync);
        endpointGroup.MapGet("/addRelatedProductsPopup", GetAddRelatedProductsToPromotionPopupComponentAsync);

        endpointGroup.MapPost("/search", GetGroupPromotionsListAsync);

        endpointGroup.MapPost("/images", GetGroupPromotionImageComponent);
        endpointGroup.MapPost("/groupImages", GetPromotionGroupImageComponent);

        endpointGroup.MapPost("/create", InsertNewPromotionAsync);
        endpointGroup.MapPost("/createGroup", InsertNewPromotionGroupAsync);

        endpointGroup.MapPut("/update", UpdatePromotionAsync);
        endpointGroup.MapPut("/updateGroup", UpdatePromotionGroupAsync);

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

        if (searchData?.ActiveOnly == true)
        {
            groupPromotions = GetOnlyActivePromotions(groupPromotions);
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

    private static List<GroupPromotionContent> GetOnlyActivePromotions(List<GroupPromotionContent> groupPromotions)
    {
        List<GroupPromotionContent> output = new();

        foreach (GroupPromotionContent groupPromotion in groupPromotions)
        {
            if (groupPromotion.Disabled == true) continue;

            if (groupPromotion.ExpirationDate is not null
                && groupPromotion.ExpirationDate < DateTime.Now)
            {
                continue;
            }

            output.Add(groupPromotion);
        }

        return output;
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
                MinDateValue = groupPromotionService.GetMinStartDate(),
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
            MinDateValue = groupPromotionService.GetMinStartDate(),
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

    private static async Task<IResult> GetPromotionGroupEditorForExistingItemAsync(
        [FromRoute] int id,
        [FromServices] IPromotionGroupService promotionGroupService)
    {
        PromotionGroup? promotionGroup = await promotionGroupService.GetByIdAsync(id);

        if (promotionGroup == null) return Results.NotFound();

        return new RazorComponentResult<PromotionGroupEditorPopup>(new
        {
            PopupData = new PromotionGroupEditorPopup.PromotionGroupEditorData()
            {
                IsVisible = false,
                Id = promotionGroup.Id,
                Name = promotionGroup.Name,
                Header = promotionGroup.Header,
                LogoImage = promotionGroup.LogoImage,
                LogoContentType = promotionGroup.LogoContentType,
                DisplayOrder = promotionGroup.DisplayOrder,
                IsDefault = promotionGroup.IsDefault,
                ShowEmptyForLogged = promotionGroup.ShowEmptyForLogged,
                ShowEmptyForNonLogged = promotionGroup.ShowEmptyForNonLogged,
            }
        });
    }

    private static RazorComponentResult<PromotionGroupEditorPopup> GetPromotionGroupEditorForNewItem()
    {
        return new RazorComponentResult<PromotionGroupEditorPopup>(new
        {
            PopupData = new PromotionGroupEditorPopup.PromotionGroupEditorData()
            {
                IsVisible = false,
                Id = null,
                Name = null,
                Header = null,
                LogoImage = null,
                LogoContentType = null,
                DisplayOrder = null,
                IsDefault = false,
                ShowEmptyForLogged = false,
                ShowEmptyForNonLogged = false,
            }
        });
    }

    private static RazorComponentResult<GroupPromotionAddRelatedProductsPopup> GetAddRelatedProductsToPromotionPopupComponentAsync(
        [FromQuery] int? selectedPromotionGroupId = null)
    {
        return new RazorComponentResult<GroupPromotionAddRelatedProductsPopup>(new
        {
            SelectedPromotionGroupId = selectedPromotionGroupId,
        });
    }

    private static async Task<IResult> GetGroupPromotionRelatedProductComponentAsync(
        [FromRoute] int? productId,
        [FromServices] IProductService productService)
    {
        if (productId == null) return Results.BadRequest();

        MOSTComputers.Models.Product.Models.Product? product = await productService.GetByIdAsync(productId.Value);

        return new RazorComponentResult<GroupPromotionEditorRelatedProduct>(new
        {
            Product = product,
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

    private static IResult GetPromotionGroupImageComponent(
        HttpContext httpContext,
        [FromBody] PromotionGroupLogoImageData imageUrlData)
    {
        string? imageUrl = imageUrlData?.ImageUrl;

        if (imageUrl == null
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

        return new RazorComponentResult<PromotionGroupEditorPopupImage>(new
        {
            ImageUrl = imageUrl,
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
                if (imageFile.Length > _maxPromotionImageSize)
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

                imageCreateRequests.Add(imageCreateRequest);
            }
        }

        string? htmlContent = groupPromotionCreateRequest.HtmlContent?.Trim();

        if (htmlContent != null)
        {
            OneOf<string, InvalidImageRequestResult> transformHtmlContentResult
                = TransformImagesInHtml(htmlContent, groupPromotionService.GetValidHtmlContentImageUrlReference);

            if (!transformHtmlContentResult.IsT0)
            {
                return transformHtmlContentResult.Match(
                    x => throw new UnreachableException(),
                    invalidImageRequestResult => Results.BadRequest());
            }

            htmlContent = transformHtmlContentResult.AsT0;
        }

        ServiceGroupPromotionContentCreateRequest createRequest = new()
        {
            Name = groupPromotionCreateRequest.Name,
            GroupId = groupPromotionCreateRequest.GroupId,
            HtmlContent = htmlContent,
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

        List<GroupPromotionImageCreateRequest>? promotionImageCreateRequests = groupPromotionUpdateRequest.PromotionImageCreateRequests;
        List<GroupPromotionImageKeepRequest>? imageIdsToKeep = groupPromotionUpdateRequest.ImageIdsToKeep;

        int totalImagesCount = (promotionImageCreateRequests?.Count ?? 0) + (imageIdsToKeep?.Count ?? 0);

        ServiceGroupPromotionImageUpsertRequest[] imageUpsertRequests = new ServiceGroupPromotionImageUpsertRequest[totalImagesCount];

        if (promotionImageCreateRequests != null)
        {
            foreach (GroupPromotionImageCreateRequest imageCreateRequest in promotionImageCreateRequests)
            {
                if (imageCreateRequest.ImageIndex == null
                    || imageCreateRequest.ImageIndex < 0
                    || imageCreateRequest.ImageIndex > totalImagesCount)
                {
                    return Results.BadRequest();
                }

                IFormFile? imageFile = imageCreateRequest.ImageFile;

                if (imageFile == null
                    || imageFile.Length > _maxPromotionImageSize)
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

                int imageIndex = imageCreateRequest.ImageIndex.Value;

                if (imageUpsertRequests[imageIndex] != null) return Results.BadRequest();

                imageUpsertRequests[imageIndex] = imageUpsertRequest;
            }
        }

        if (imageIdsToKeep != null)
        {
            List<GroupPromotionImage> currentImages
                = await groupPromotionImageService.GetAllInPromotionAsync(groupPromotionUpdateRequest.Id.Value);

            List<GroupPromotionImageFileData> currentImageFiles
                = await groupPromotionImageFileService.GetAllInPromotionAsync(groupPromotionUpdateRequest.Id.Value);

            foreach (GroupPromotionImageKeepRequest imageKeepRequest in imageIdsToKeep)
            {
                if (imageKeepRequest.ImageIndex == null
                    || imageKeepRequest.ImageIndex < 0
                    || imageKeepRequest.ImageIndex > totalImagesCount)
                {
                    return Results.BadRequest();
                }

                if (imageKeepRequest.Id == null || imageKeepRequest.ImageIndex == null) return Results.BadRequest();

                int idToKeep = imageKeepRequest.Id.Value;

                GroupPromotionImage? image = currentImages.FirstOrDefault(x => x.Id == idToKeep);
                GroupPromotionImageFileData? imageFile = currentImageFiles.FirstOrDefault(x => x.ImageId == idToKeep);

                if (image == null || imageFile == null) continue;

                string contentTypeFromImage = image.ContentType!.Trim();

                if (contentTypeFromImage.StartsWith("image/."))
                {
                    int indexOfDot = contentTypeFromImage.IndexOf('.');

                    contentTypeFromImage = contentTypeFromImage.Remove(indexOfDot, 1);
                }

                if (contentTypeFromImage.Equals("image/jpg", StringComparison.OrdinalIgnoreCase))
                {
                    contentTypeFromImage = "image/jpeg";
                }

                ServiceGroupPromotionImageUpsertRequest imageUpsertRequest = new()
                {
                    ExistingImageId = idToKeep,
                    ContentType = contentTypeFromImage,
                    Image = image.Image!,
                    FileExtension = Path.GetExtension(imageFile.FileName),
                    CustomFileNameWithoutExtension = null,
                };

                int imageIndex = imageKeepRequest.ImageIndex.Value;

                if (imageUpsertRequests[imageIndex] != null) return Results.BadRequest();

                imageUpsertRequests[imageIndex] = imageUpsertRequest;
            }
        }

        string? htmlContent = groupPromotionUpdateRequest.HtmlContent?.Trim();

        if (htmlContent != null)
        {
            OneOf<string, InvalidImageRequestResult> transformHtmlContentResult
                = TransformImagesInHtml(htmlContent, groupPromotionService.GetValidHtmlContentImageUrlReference);

            if (!transformHtmlContentResult.IsT0)
            {
                return transformHtmlContentResult.Match(
                    x => throw new UnreachableException(),
                    invalidImageRequestResult => Results.BadRequest());
            }

            htmlContent = transformHtmlContentResult.AsT0;
        }

        ServiceGroupPromotionContentUpdateRequest updateRequest = new()
        {
            Id = groupPromotionUpdateRequest.Id.Value,
            Name = groupPromotionUpdateRequest.Name,
            GroupId = groupPromotionUpdateRequest.GroupId,
            HtmlContent = htmlContent,
            StartDate = groupPromotionUpdateRequest.StartDate,
            ExpirationDate = groupPromotionUpdateRequest.ExpirationDate,
            DisplayOrder = groupPromotionUpdateRequest.DisplayOrder,
            Disabled = groupPromotionUpdateRequest.Disabled,
            Restricted = groupPromotionUpdateRequest.Restricted,
            MemberOfDefaultGroup = groupPromotionUpdateRequest.MemberOfDefaultGroup,
            DefaultGroupPriority = groupPromotionUpdateRequest.DefaultGroupPriority,
            ImageRequests = imageUpsertRequests.ToList(),
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

    private static async Task<IResult> InsertNewPromotionGroupAsync(
        [FromForm] PromotionGroupCreateRequest createRequest,
        [FromServices] IPromotionGroupService promotionGroupService)
    {
        PromotionGroupLogo? promotionGroupLogo = null;

        IFormFile? imageFile = createRequest.LogoImage;

        if (imageFile != null)
        {
            if (imageFile.Length > _maxPromotionGroupLogoImageSize)
            {
                return Results.BadRequest();
            }

            Stream imageStream = imageFile.OpenReadStream();

            using MemoryStream memoryStream = new();

            await imageStream.CopyToAsync(memoryStream);

            promotionGroupLogo = new()
            {
                ContentType = imageFile.ContentType,
                Image = memoryStream.ToArray(),
            };
        }

        ServicePromotionGroupCreateRequest innerCreateRequest = new()
        {
            Name = createRequest.Name,
            Header = string.Empty,
            DisplayOrder = createRequest.DisplayOrder,
            IsDefault = false,
            Logo = promotionGroupLogo,
            ShowEmptyForLogged = false,
            ShowEmptyForNonLogged = false,
        };

        OneOf<int, ValidationResult, UnexpectedFailureResult> createResult
             = await promotionGroupService.InsertAsync(innerCreateRequest);

        return createResult.Match(
            id => Results.Ok(id),
            validationResult => Results.BadRequest(),
            unexpectedFailureResult => Results.StatusCode(500));
    }

    private static async Task<IResult> UpdatePromotionGroupAsync(
        [FromForm] PromotionGroupUpdateRequest updateRequest,
        [FromServices] IPromotionGroupService promotionGroupService)
    {
        if (updateRequest.Id == null) return Results.BadRequest();

        int id = updateRequest.Id.Value;

        if (updateRequest.PreserveOldImage == true
            && updateRequest.NewLogoImage != null)
        {
            return Results.BadRequest();
        }

        PromotionGroupLogo? promotionGroupLogo = null;

        IFormFile? imageFile = updateRequest.NewLogoImage;

        if (imageFile != null)
        {
            if (imageFile.Length > _maxPromotionGroupLogoImageSize)
            {
                return Results.BadRequest();
            }

            Stream imageStream = imageFile.OpenReadStream();

            using MemoryStream memoryStream = new();

            await imageStream.CopyToAsync(memoryStream);

            promotionGroupLogo = new()
            {
                ContentType = imageFile.ContentType,
                Image = memoryStream.ToArray(),
            };
        }
        else if (updateRequest.PreserveOldImage == true)
        {
            PromotionGroup? existingPromotionGroup = await promotionGroupService.GetByIdAsync(id);

            if (existingPromotionGroup == null) return Results.NotFound();

            promotionGroupLogo = new()
            {
                ContentType = existingPromotionGroup.LogoContentType!,
                Image = existingPromotionGroup.LogoImage!,
            };
        }

        ServicePromotionGroupUpdateRequest updateRequestInner = new()
        {
            Id = id,
            Name = updateRequest.Name,
            Header = string.Empty,
            DisplayOrder = updateRequest.DisplayOrder,
            IsDefault = false,
            Logo = promotionGroupLogo,
            ShowEmptyForLogged = false,
            ShowEmptyForNonLogged = false,
        };

        OneOf<Success, OneOf.Types.NotFound, ValidationResult> updateResult
             = await promotionGroupService.UpdateAsync(updateRequestInner);

        return updateResult.Match(
            success => Results.Ok(),
            notFound => Results.NotFound(),
            validationResult => Results.BadRequest());
    }


    private static OneOf<string, InvalidImageRequestResult> TransformImagesInHtml(
        string htmlContent,
        Func<int, string> getHtmlRepresentationFromImageRequestIndex)
    {
        return ReplaceClientImageRepresentationToRequestRepresentation(htmlContent, getHtmlRepresentationFromImageRequestIndex);

        // HtmlDocument htmlContentParsed = new();

        // htmlContentParsed.LoadHtml(htmlContent);

        // foreach (HtmlNode htmlNode in htmlContentParsed.DocumentNode.DescendantsAndSelf())
        // {
        //     if (htmlNode.NodeType == HtmlNodeType.Text)
        //     {
        //         OneOf<string, InvalidImageRequestResult> replaceImageDataResult
        //             = ReplaceClientImageRepresentationToRequestRepresentation(htmlNode.InnerText, getHtmlRepresentationFromImageRequestIndex);

        //         if (replaceImageDataResult.IsT1) return replaceImageDataResult.AsT1;

        //         htmlNode.InnerHtml = replaceImageDataResult.AsT0;
        //     }

        //     foreach (HtmlAttribute htmlAttributeInNode in htmlNode.Attributes)
        //     {
        //         OneOf<string, InvalidImageRequestResult> replaceImageDataResult
        //             = ReplaceClientImageRepresentationToRequestRepresentation(htmlAttributeInNode.Value, getHtmlRepresentationFromImageRequestIndex);

        //         if (replaceImageDataResult.IsT1) return replaceImageDataResult.AsT1;

        //         htmlAttributeInNode.Value = replaceImageDataResult.AsT0;
        //     }
        // }

        //return htmlContentParsed.DocumentNode.WriteContentTo();
    }

    private static OneOf<string, InvalidImageRequestResult> ReplaceClientImageRepresentationToRequestRepresentation(
        string input,
        Func<int, string> getHtmlRepresentationFromImageRequestIndex)
    {
        int currentlyScannedIndex = 0;

        StringBuilder stringBuilder = new();

        while (true)
        {
            int indexOfStart = input.IndexOf(_serverImageRepresentationStart, currentlyScannedIndex);

            if (indexOfStart == -1)
            {
                stringBuilder.Append(input[currentlyScannedIndex..]);

                break;
            }

            stringBuilder.Append(input, currentlyScannedIndex, indexOfStart - currentlyScannedIndex);

            int indexOfEnd = input.IndexOf(_serverImageRepresentationEnd, indexOfStart + _serverImageRepresentationStart.Length);

            if (indexOfEnd == -1)
            {
                stringBuilder.Append(input[indexOfStart..]);

                break;
            }

            string resourceData = input[(indexOfStart + _serverImageRepresentationStart.Length)..indexOfEnd];

            OneOf<int, InvalidImageRequestResult> getImageIndexResult = GetImageRequestIndexFromClientData(resourceData);

            if (getImageIndexResult.IsT1)
            {
                return getImageIndexResult.AsT1;
            }

            string replacement = getHtmlRepresentationFromImageRequestIndex(getImageIndexResult.AsT0);

            stringBuilder.Append(replacement);

            currentlyScannedIndex = indexOfEnd + _serverImageRepresentationEnd.Length;
        }

        return stringBuilder.ToString();
    }

    private static OneOf<int, InvalidImageRequestResult> GetImageRequestIndexFromClientData(string clientData)
    {
        int clientImageRequestIndexPrefixIndex = clientData.IndexOf(_serverImageIndexPrefix);

        int imageIndexStartIndex = (clientImageRequestIndexPrefixIndex + _serverImageIndexPrefix.Length);

        string imageIndexAsString = clientData[imageIndexStartIndex..];

        if (int.TryParse(imageIndexAsString, out int imageIndex))
        {
            return imageIndex;
        }

        return new InvalidImageRequestResult();
    }

    private static bool DoesUriMatchOriginWithRequest(Uri uri, HttpRequest? httpRequest)
    {
        if (httpRequest is null) return false;

        return string.Equals(uri.Host, httpRequest.Host.Host, StringComparison.OrdinalIgnoreCase);
    }
}
