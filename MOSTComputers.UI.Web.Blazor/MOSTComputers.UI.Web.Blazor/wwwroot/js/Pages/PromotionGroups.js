const promotionSearchInputId = "promotion-group-search-input";
const groupSelectId = "promotion-group-select";

const promotionListContainerId = "promotion-list-container";

const searchPromotionButtonId = "search-promotion-button";
const addPromotionButtonId = "add-promotion-button";

const promotionEditPanelContainerId = "promotion-edit-panel-container";
const promotionEditPanelId = "promotion-edit-panel";

const savePromotionButtonId = "save-promotion-button";

const promotionNameId = "promotion-name";
const promotionStartDateId = "promotion-start-date";
const promotionExpirationDateId = "promotion-expiration-date";
const promotionDisplayOrderId = "promotion-display-order";
const promotionGroupSelectId = "promotion-group-editor-select";
const promotionDisabledId = "promotion-disabled";
const promotionRestrictedId = "promotion-restricted";
const promotionMemberDefaultId = "promotion-member-default";
const promotionDefaultPriorityId = "promotion-default-priority";
const promotionHtmlId = "promotion-html";

const promotionImagesInputId = "promotion-images-input";
const promotionImagesListId = "promotion-images-list";

const antiforgeryTokenInputId = "__RequestVerificationToken";

const promotionImageListItemsName = "promotion-groups-image-list-item";
const promotionImagesName = "promotion-groups-image";

const promotionEditorPromotionIdAttribute = "data-current-promotion-id";
const promotionEditorImageIdAttribute = "data-current-promotion-image-id";

const loadingClass = "loading";

const imagesToUpload = [];

async function searchGroupPromotions()
{
    const searchOptions = getGroupPromotionSearchOptionsFromCurrentData();

    const response = await fetch("api/components/promotionGroups/search", {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            'Accept': "application/html",
        },
        body: JSON.stringify(searchOptions)
    });

    const searchResultHtml = await response.text();

    const promotionListContainer = document.getElementById(promotionListContainerId);

    promotionListContainer.innerHTML = searchResultHtml;
}

function getGroupPromotionSearchOptionsFromCurrentData()
{
    const searchInput = document.getElementById(promotionSearchInputId);
    const groupSelect = document.getElementById(groupSelectId);

    const searchInputValue = searchInput.value;
    const groupSelectValue = getIntegerOrNullFromString(groupSelect.value);

    return {
        SearchData: searchInputValue,
        PromotionGroupId: groupSelectValue
    };
}

async function openPromotionEditorForPromotion(id = null)
{
    let response;

    if (id == null)
    {
        response = await fetch("api/components/promotionGroups/editor/new", {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }
    else
    {
        response = await fetch(`api/components/promotionGroups/editor/${id}`, {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }

    const promotionEditorHtml = await response.text();

    const promotionEditPanelContainer = document.getElementById(promotionEditPanelContainerId);

    clearAndFreeImagesToUpload();

    promotionEditPanelContainer.innerHTML = promotionEditorHtml;

    expandTextAreaToNeededHeight(promotionHtmlId);
}

function clearAndFreeImagesToUpload() {
    for (let i = imagesToUpload.length - 1; i >= 0; i--) {

        const imageObject = imagesToUpload[i];

        URL.revokeObjectURL(imageObject.fileObjectUrl);

        imagesToUpload.splice(i, 1);
    }
}

async function addNewImageToEditor() {
    const promotionImagesInput = document.getElementById(promotionImagesInputId);

    promotionImagesInput.addEventListener("change", onSelectedImageToAddToPromotion, {once: true});

    promotionImagesInput.click();
}

async function onSelectedImageToAddToPromotion(e)
{
    const file = e.target.files[0];

    const fileObjectUrl = URL.createObjectURL(file);

    imagesToUpload.push({
        File: file,
        fileObjectUrl: fileObjectUrl
    });

    const promotionImagesList = document.getElementById(promotionImagesListId)

    const promotionImageHtml = await getPromotionImage(fileObjectUrl);

    promotionImagesList.innerHTML += promotionImageHtml;
}

async function getPromotionImage(imageUrl) {
    const promotionImageOptions = getPromotionImageOptions(imageUrl);

    const response = await fetch("api/components/promotionGroups/images", {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            'Accept': "application/html",
        },
        body: JSON.stringify(promotionImageOptions)
    });

    if (!response.ok) return null;

    return await response.text();
}

function getPromotionImageOptions(imageUrl) {

    const imageList = document.getElementById(promotionImagesListId);

    const imageListItemElements = [...imageList.querySelectorAll(`[name='${promotionImageListItemsName}']`)];

    let index = 0;

    for (const imageListItemElement of imageListItemElements) {

        const elementId = imageListItemElement.id;

        const indexOfSeparator = elementId.lastIndexOf('-');

        const indexAsString = elementId.substring(indexOfSeparator + 1);

        const elementIndex = parseInt(indexAsString);

        if (elementIndex >= index) {

            index = elementIndex + 1;
        }
    }

    return {
        Index: index,
        ImageUrl: imageUrl,
    }
}

async function savePromotion() {
    const promotionEditPanel = document.getElementById(promotionEditPanelId);

    const editorCurrentId = promotionEditPanel.getAttribute(promotionEditorPromotionIdAttribute);

    const savePromotionButton = document.getElementById(savePromotionButtonId);

    savePromotionButton.classList.add(loadingClass);

    try {
        if (editorCurrentId === "")
        {
            await createPromotion();

            return;
        }

        await updatePromotion();
    }
    finally {
        savePromotionButton.classList.remove(loadingClass);
    }
    
}

async function createPromotion() {
    const createRequest = getPromotionCreateRequestFromCurrentData();

    if (createRequest == null) return;

    const response = await fetch("api/components/promotionGroups/create", {
        method: "POST",
        headers: {
            "RequestVerificationToken": document.getElementById(antiforgeryTokenInputId).value
        },
        body: createRequest
    });

    if (!response.ok) return;

    const newPromotionIdAsString = await response.text();

    const newPromotionId = parseInt(newPromotionIdAsString);

    await openPromotionEditorForPromotion(newPromotionId);
}

function getPromotionCreateRequestFromCurrentData()
{
    const editor = document.getElementById(promotionEditPanelId);

    if (!editor) return null;

    const promotionName = document.getElementById(promotionNameId);
    const promotionHtmlContent = document.getElementById(promotionHtmlId);
    const promotionStartDate = document.getElementById(promotionStartDateId);
    const promotionExpirationDate = document.getElementById(promotionExpirationDateId);
    const promotionDisplayOrder = document.getElementById(promotionDisplayOrderId);
    const promotionGroupSelect = document.getElementById(promotionGroupSelectId);
    const promotionDisabled = document.getElementById(promotionDisabledId);
    const promotionRestricted = document.getElementById(promotionRestrictedId);
    const promotionMemberOfDefaultGroup = document.getElementById(promotionMemberDefaultId);
    const promotionDefaultGroupPriority = document.getElementById(promotionDefaultPriorityId);

    const formData = new FormData();

    formData.append("Name", promotionName.value);

    const groupId = getIntegerOrNullFromString(promotionGroupSelect.value);

    if (groupId != null) {

        formData.append("GroupId", groupId);
    }

    formData.append("HtmlContent", promotionHtmlContent.value);

    const startDate = promotionStartDate.value;

    if (startDate != null && startDate != '') {

        formData.append("StartDate", startDate);
    }

    const expirationDate = promotionExpirationDate.value;

    if (expirationDate != null && expirationDate != '') {

        formData.append("ExpirationDate", expirationDate);
    }

    const displayOrder = getIntegerOrNullFromString(promotionDisplayOrder.value);

    if (displayOrder != null) {

        formData.append("DisplayOrder", displayOrder);
    }

    formData.append("Disabled", promotionDisabled.checked);
    formData.append("Restricted", promotionRestricted.checked);
    formData.append("MemberOfDefaultGroup", promotionMemberOfDefaultGroup.checked);

    const defaultGroupDisplayOrder = getIntegerOrNullFromString(promotionDefaultGroupPriority.value);

    if (defaultGroupDisplayOrder) {

        formData.append("DefaultGroupPriority", defaultGroupDisplayOrder);
    }

    for (let i = 0; i < imagesToUpload.length; i++)
    {
        const imageObject = imagesToUpload[i];

        const blob = imageObject.File;

        formData.append(`PromotionImageCreateRequests[${i}]`, blob);
    }

    return formData;
}

async function updatePromotion() {
    const editor = document.getElementById(promotionEditPanelId);

    if (!editor) return;

    const promotionIdAsString = editor.getAttribute(promotionEditorPromotionIdAttribute);

    const promotionId = getIntegerOrNullFromString(promotionIdAsString);

    if (promotionId == null || isNaN(promotionId)) return;

    const updateRequest = getPromotionUpdateRequestFromCurrentData(promotionId);

    const response = await fetch("api/components/promotionGroups/update", {
        method: "PUT",
        headers: {
            "RequestVerificationToken": document.getElementById(antiforgeryTokenInputId).value
        },
        body: updateRequest
    });

    if (!response.ok) return;

    await openPromotionEditorForPromotion(promotionId);
}

function getPromotionUpdateRequestFromCurrentData(promotionId)
{
    const promotionName = document.getElementById(promotionNameId);
    const promotionHtmlContent = document.getElementById(promotionHtmlId);
    const promotionStartDate = document.getElementById(promotionStartDateId);
    const promotionExpirationDate = document.getElementById(promotionExpirationDateId);
    const promotionDisplayOrder = document.getElementById(promotionDisplayOrderId);
    const promotionGroupSelect = document.getElementById(promotionGroupSelectId);
    const promotionDisabled = document.getElementById(promotionDisabledId);
    const promotionRestricted = document.getElementById(promotionRestrictedId);
    const promotionMemberOfDefaultGroup = document.getElementById(promotionMemberDefaultId);
    const promotionDefaultGroupPriority = document.getElementById(promotionDefaultPriorityId);

    const imageList = document.getElementById(promotionImagesListId);

    const imageElements = [...imageList.querySelectorAll(`[name='${promotionImagesName}']`)];

    const formData = new FormData();

    formData.append("Id", promotionId);
    formData.append("Name", promotionName.value);

    const groupId = getIntegerOrNullFromString(promotionGroupSelect.value);

    if (groupId != null) {
        formData.append("GroupId", groupId);
    }

    formData.append("HtmlContent", promotionHtmlContent.value);

    const startDate = promotionStartDate.value;

    if (startDate != null && startDate != '') {
        formData.append("StartDate", startDate);
    }

    const expirationDate = promotionExpirationDate.value;

    if (expirationDate != null && expirationDate != '') {
        formData.append("ExpirationDate", expirationDate);
    }

    const displayOrder = getIntegerOrNullFromString(promotionDisplayOrder.value);

    if (displayOrder != null) {
        formData.append("DisplayOrder", displayOrder);
    }

    formData.append("Disabled", promotionDisabled.checked);
    formData.append("Restricted", promotionRestricted.checked);
    formData.append("MemberOfDefaultGroup", promotionMemberOfDefaultGroup.checked);

    const defaultGroupDisplayOrder = getIntegerOrNullFromString(promotionDefaultGroupPriority.value);

    if (defaultGroupDisplayOrder) {
        formData.append("DefaultGroupPriority", defaultGroupDisplayOrder);
    }

    for (let i = 0; i < imageElements.length; i++)
    {
        const imageElement = imageElements[i];

        const existingImageId = imageElement.getAttribute(promotionEditorImageIdAttribute);

        if (existingImageId == null || existingImageId == '') continue;

        formData.append(`ImageIdsToKeep[${i}]`, existingImageId);
    }

    for (let i = 0; i < imagesToUpload.length; i++)
    {
        const imageObject = imagesToUpload[i];

        const blob = imageObject.File;

        formData.append(`PromotionImageCreateRequests[${i}]`, blob);
    }

    return formData;
}

function removePromotionImage(imageElementId, imageUrl) {
    const imageElement = document.getElementById(imageElementId);

    if (imageElement) {

        imageElement.remove();
    }

    for (let i = 0; i < imagesToUpload.length; i++) {

        const imageToUpload = imagesToUpload[i];

        if (imageToUpload.fileObjectUrl == imageUrl) {

            imagesToUpload.splice(i, 1);

            break;
        }
    }
}

function expandTextAreaToNeededHeight(textareaId)
{
    const textarea = document.getElementById(textareaId);

    textarea.style.height = 'auto';

    textarea.style.height = (textarea.scrollHeight + 1.6) + 'px';
}

function getNumberOrNull(numberValue) {
    return typeof numberValue === "number" ? numberValue : null;
}

function getIntegerOrNullFromString(stringValue) {

    if (stringValue == null || stringValue === "") return null;

    var output = null;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}
