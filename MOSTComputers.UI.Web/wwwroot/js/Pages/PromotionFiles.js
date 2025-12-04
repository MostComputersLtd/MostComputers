$(document).ready(function()
{
    const enterKeyName = "Enter";

    document.addEventListener("keypress", (event) =>
    {
        if (event.key == enterKeyName)
        {
            searchFilesAsync(searchElementIds.UserSearchInputElementId,
                searchElementIds.ValidOnInputElementId,
                promotionFilesTableContainerElementId);
        }
    })
});

const fileHasRelationsErrorName = "PromotionFileHasRelations";

const searchElementIds =
{
    UserSearchInputElementId: "promotion-files-search-input",
    ValidOnInputElementId: "promotion-files-valid-on-input",
};

const promotionFilesTableContainerElementId = "promotionFilesTableContainer";

function singleEditorFileInputOnChange(fileInputElementId, fileNameDisplayTextElementId)
{
    if (fileNameDisplayTextElementId)
    {
        changeTextToMatchFile(fileInputElementId, fileNameDisplayTextElementId);
    }
}

function singleEditorFileZoneOnDragOver(dragEvent)
{
    dragEvent.preventDefault();
}

function singleEditorFileZoneOnDrop(dropEvent, fileInputElementId, fileNameDisplayTextElementId = null)
{
    const files = getFilesFromDataTransferAsFileList(dropEvent.dataTransfer);

    if (files.length <= 0) return;

    const fileInputElement = document.getElementById(fileInputElementId);

    fileInputElement.files = files;

    if (fileNameDisplayTextElementId)
    {
        changeTextToMatchFile(fileInputElementId, fileNameDisplayTextElementId);
    }
}

function changeTextToMatchFile(fileInputElementId, fileNameDisplayTextElementId)
{
    const fileInputElement = document.getElementById(fileInputElementId);

    const file = fileInputElement.files[0];

    if (!file) return;

    const fileNameDisplayTextElement = document.getElementById(fileNameDisplayTextElementId);

    fileNameDisplayTextElement.innerText = file.name;
}

async function searchFilesAsync(userSearchInputElementId, validOnInputElementId, promotionFilesTableContainerElementId)
{
    const userSearchInputElement = document.getElementById(userSearchInputElementId);
    const validOnInputElement = document.getElementById(validOnInputElementId);

    const userSearchInputData = (userSearchInputElement.value != null) ? userSearchInputElement.value.trim() : null;

    const validOn = isNaN(Date.parse(validOnInputElement.value)) ? null : validOnInputElement.value;

    const searchOptions = getSearchOptionsDataAsObject(userSearchInputData, validOn);

    const url = pageLocationInUrl + "?handler=SearchFiles";

    const response = await fetch(url,
        {
            method: 'POST',
            headers:
            {
                'Content-Type': "application/json",
                'RequestVerificationToken':
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(searchOptions)
        });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    const promotionFilesTableContainerElement = document.getElementById(promotionFilesTableContainerElementId);

    if (promotionFilesTableContainerElement != null)
    {
        promotionFilesTableContainerElement.innerHTML = responseData;
    }
}

async function openPromotionFileSingleEditorPartial(promotionFileInfoId,
    promotionFileSingleEditorModalId, promotionFileSingleEditorModalDialogId, promotionFileSingleEditorModalContentId)
{
    const url = pageLocationInUrl + "?handler=GetPromotionFileSingleEditorPartial"
        + "&promotionFileInfoId=" + promotionFileInfoId;

    const response = await fetch(url,
    {
        method: 'GET',
        headers:
        {
            'Content-Type': "application/json",
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        }
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(promotionFileSingleEditorModalId, promotionFileSingleEditorModalDialogId, promotionFileSingleEditorModalContentId, responseData);
}

const pageLocationInUrl = "/PromotionFiles";

const promotionFileSingleEditorDataNames =
{
    id: "Id",
    name: "Name",
    active: "Active",
    validFrom: "ValidFrom",
    validTo: "ValidTo",
    description: "Description",
    requiredProducts: "RelatedProductsString",
    file: "File",
};

async function addNewFileAndDisplayChanges(
    nameInputElementId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    descriptionInputElementId,
    requiredProductsInputElementId,
    fileInputElementId,
    promotionFilesTableContainerElementId,
    promotionFileSingleEditorModalContentElementId,
    addNewFileButtonElementId = null,
    addNewFileButtonLoaderElementId = null)
{
    const addNewFilePromise = addNewFile(
        nameInputElementId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        descriptionInputElementId,
        requiredProductsInputElementId,
        fileInputElementId);

    const response = await awaitWithCallbacks(addNewFilePromise,
        function () { toggleViews(addNewFileButtonElementId, addNewFileButtonLoaderElementId); },
        function () { toggleViews(addNewFileButtonElementId, addNewFileButtonLoaderElementId); });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.json();

    const promotionFilesTableContainerElement = document.getElementById(promotionFilesTableContainerElementId);
    const promotionFileSingleEditorModalContentElement = document.getElementById(promotionFileSingleEditorModalContentElementId);

    promotionFilesTableContainerElement.innerHTML = responseData.promotionFileViewPartialAsString;
    promotionFileSingleEditorModalContentElement.innerHTML = responseData.promotionFileSingleEditorPartialAsString;
}

async function addNewFile(
    nameInputElementId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    descriptionInputElementId,
    requiredProductsInputElementId,
    fileInputElementId)
{
    const promotionFileInfoSingleEditorData = getImageSingleEditorInsertData(
        "promotionFileInsertData",
        nameInputElementId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        descriptionInputElementId,
        requiredProductsInputElementId,
        fileInputElementId);

    const searchOptions = getCurrentSearchOptionsDataAsObject();

    promotionFileInfoSingleEditorData.append(`searchOptions.UserInputString`, searchOptions.UserInputString)
    promotionFileInfoSingleEditorData.append(`searchOptions.ValidOnDate`, searchOptions.ValidOnDate)
   
    const url = pageLocationInUrl + "?handler=AddNewFile";

    return fetch(url,
    {
        method: 'POST',
        headers:
        {
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: promotionFileInfoSingleEditorData
    });
}

async function updateFileAndDisplayChanges(
    promotionFileId,
    nameInputElementId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    descriptionInputElementId,
    requiredProductsInputElementId,
    fileInputElementId,
    promotionFilesTableContainerElementId,
    promotionFileSingleEditorModalContentElementId,
    updateFileButtonElementId = null,
    updateFileButtonLoaderElementId = null)
{
    const updateFilePromise = updateFile(
        promotionFileId,
        nameInputElementId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        descriptionInputElementId,
        requiredProductsInputElementId,
        fileInputElementId);

    const response = await awaitWithCallbacks(updateFilePromise,
        function () { toggleViews(updateFileButtonElementId, updateFileButtonLoaderElementId); },
        function () { toggleViews(updateFileButtonElementId, updateFileButtonLoaderElementId); });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.json();

    const promotionFilesTableContainerElement = document.getElementById(promotionFilesTableContainerElementId);
    const promotionFileSingleEditorModalContentElement = document.getElementById(promotionFileSingleEditorModalContentElementId);

    promotionFilesTableContainerElement.innerHTML = responseData.promotionFileViewPartialAsString;
    promotionFileSingleEditorModalContentElement.innerHTML = responseData.promotionFileSingleEditorPartialAsString;
}

async function updateFile(
    promotionFileId,
    nameInputElementId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    descriptionInputElementId,
    requiredProductsInputElementId,
    fileInputElementId)
{
    const promotionFileInfoSingleEditorData = getImageSingleEditorUpdateData(
        "promotionFileUpdateData",
        promotionFileId,
        nameInputElementId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        descriptionInputElementId,
        requiredProductsInputElementId,
        fileInputElementId);

    const searchOptions = getCurrentSearchOptionsDataAsObject();

    promotionFileInfoSingleEditorData.append(`searchOptions.UserInputString`, searchOptions.UserInputString)
    promotionFileInfoSingleEditorData.append(`searchOptions.ValidOnDate`, searchOptions.ValidOnDate)
   
    const url = pageLocationInUrl + "?handler=UpdateFile";

    return fetch(url,
    {
        method: 'PUT',
        headers:
        {
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        body: promotionFileInfoSingleEditorData
    });
}

async function deleteFile(promotionFileId, promotionFilesTableContainerElementId, notificationBoxId)
{
    const url = pageLocationInUrl + "?handler=DeleteFile"
        + "&promotionFileId=" + promotionFileId;

    const response = await fetch(url,
    {
        method: 'DELETE',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val(),
        },
        body: JSON.stringify(getCurrentSearchOptionsDataAsObject())
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200)
    {
        if (response.status === 400)
        {
            const responseErrorData = await response.json();

            const errorName = responseErrorData.errorName;

            if (errorName === fileHasRelationsErrorName)
            {
                showNotificationWithText(notificationBoxId,
                    "Promotion file is used by products",
                    "notificationBox-long-message",
                    10000);
            }
        }

        return;
    }

    const responseData = await response.text();

    const promotionFilesTableContainerElement = document.getElementById(promotionFilesTableContainerElementId);

    promotionFilesTableContainerElement.innerHTML = responseData;
}

function getCurrentSearchOptionsDataAsObject()
{
    const userSearchInputElement = document.getElementById(searchElementIds.UserSearchInputElementId);
    const validOnInputElement = document.getElementById(searchElementIds.ValidOnInputElementId);

    const userSearchInputData = (userSearchInputElement.value != null) ? userSearchInputElement.value.trim() : null;

    const validOn = isNaN(Date.parse(validOnInputElement.value)) ? null : validOnInputElement.value;

    return getSearchOptionsDataAsObject(userSearchInputData, validOn);
}

function getSearchOptionsDataAsObject(userSearchInputData = null, validOn = null)
{
    return {
        UserInputString: userSearchInputData,
        ValidOnDate: validOn,
    }
}

function getImageSingleEditorInsertData(
    formDataModelName,
    nameInputElementId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    descriptionInputElementId,
    requiredProductsInputElementId,
    fileInputElementId)
{
    const nameInputElement = document.getElementById(nameInputElementId);
    const activeInputElement = document.getElementById(activeInputElementId);
    const validFromInputElement = document.getElementById(validFromInputElementId);
    const validToInputElement = document.getElementById(validToInputElementId);
    const descriptionInputElement = document.getElementById(descriptionInputElementId);
    const requiredProductsInputElement = document.getElementById(requiredProductsInputElementId);
    const fileInputElement = document.getElementById(fileInputElementId);

    const name = (nameInputElement.value != null) ? nameInputElement.value.trim() : null;
    const active = activeInputElement.checked;
    const validFrom = validFromInputElement.value;
    const validTo = validToInputElement.value;
    const description = (descriptionInputElement.value != null) ? descriptionInputElement.value.trim() : null;
    const requiredProductsString = (requiredProductsInputElement.value != null) ? requiredProductsInputElement.value.trim() : null;
    const file = fileInputElement.files[0];

    const formData = new FormData();

    formData.append(`${formDataModelName}.${promotionProductFileSingleEditorDataNames.active}`, active);

    formData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.name}`, name);
    formData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.active}`, active);
    formData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.validFrom}`, validFrom);
    formData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.validTo}`, validTo);
    formData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.description}`, description);
    formData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.requiredProducts}`, requiredProductsString);
    formData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.file}`, file);

    return formData;
}

function getImageSingleEditorUpdateData(
    formDataModelName,
    id,
    nameInputElementId,
    activeInputElementId,
    validFromInputElementId,
    validToInputElementId,
    descriptionInputElementId,
    requiredProductsInputElementId,
    fileInputElementId)
{
    const promotionFileInfoSingleEditorData = getImageSingleEditorInsertData(
        formDataModelName,
        nameInputElementId,
        activeInputElementId,
        validFromInputElementId,
        validToInputElementId,
        descriptionInputElementId,
        requiredProductsInputElementId,
        fileInputElementId);

    promotionFileInfoSingleEditorData.append(`${formDataModelName}.${promotionFileSingleEditorDataNames.id}`, id);

    return promotionFileInfoSingleEditorData;
}