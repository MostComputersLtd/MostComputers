import * as fetchWithAuthRedirect from "../../Authentication/FetchWithAuthRedirect.js";
import * as popup from "../../Primitives/Popup.js";
import * as common from "./Common.js";
import * as imageResize from "./ImageResize.js";

let promotionGroupImageToUpload = null;

export async function openPromotionGroupEditorPopup(id = null) {

    const promotionGroupEditorPopupHtmlResponse = await getPromotionGroupEditorPopupDataResponse(id);

    if (promotionGroupEditorPopupHtmlResponse == null) return;

    const promotionGroupEditorPopupHtml = await promotionGroupEditorPopupHtmlResponse.text();

    const promotionGroupEditorPopupContainer = document.getElementById(common.promotionGroupEditorPopupContainerId);

    promotionGroupEditorPopupContainer.innerHTML = promotionGroupEditorPopupHtml;

    const promotionGroupSaveButton = document.getElementById(common.promotionGroupSaveButtonId);

    promotionGroupSaveButton.addEventListener("click", () => savePromotionGroup(id));

    const promotionGroupLogoChangeButton = document.getElementById(common.promotionGroupLogoChangeButtonId);

    promotionGroupLogoChangeButton.addEventListener("click", changePromotionGroupLogo);

    const promotionGroupImageResizeButton = document.getElementById(common.promotionGroupImageResizeButtonId);

    if (promotionGroupImageResizeButton) {

        promotionGroupImageResizeButton.addEventListener("click", openResizePopupForPromotionGroupImage);
    }

    const promotionGroupImageDeleteButton = document.getElementById(common.promotionGroupImageDeleteButtonId);

    if (promotionGroupImageDeleteButton) {

        promotionGroupImageDeleteButton.addEventListener("click", removePromotionGroupImage);
    }

    const promotionGroupEditorPopup = document.getElementById(common.promotionGroupEditorPopupId);

    popup.attachCloseEvents(promotionGroupEditorPopup);

    promotionGroupEditorPopup.showModal();
}

async function getPromotionGroupEditorPopupDataResponse(id = null) {

    let response;

    if (id == null) {

        response = await fetch("api/components/promotionGroups/groupEditorPopup/new", {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }

    else {
        response = await fetch(`api/components/promotionGroups/groupEditorPopup/${id}`, {
            method: "GET",
            headers: {
                'Accept': "application/html",
            }
        });
    }

    if (response == null) return null;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return null;

    return response;
}

export function changePromotionGroupLogo() {

    const logoImageInput = document.getElementById(common.promotionGroupLogoInputId);

    logoImageInput.addEventListener("change", onLogoImageInputChanged, { once: true });

    logoImageInput.click();
}

async function onLogoImageInputChanged(e) {

    const file = e.target.files[0];

    if (!file) return;

    const fileObjectUrl = URL.createObjectURL(file);

    const newImageElementHtml = await getPromotionGroupLogoImage(fileObjectUrl, file.type);

    if (promotionGroupImageToUpload != null) {

        URL.revokeObjectURL(promotionGroupImageToUpload.fileObjectUrl);
    }

    const promotionGroupLogoImageDisplay = document.getElementById(common.promotionGroupLogoImageDisplayId);

    promotionGroupLogoImageDisplay.innerHTML = newImageElementHtml;

    promotionGroupImageToUpload = {
        file: file,
        fileObjectUrl: fileObjectUrl
    };

    const promotionGroupLogoChangeButton = document.getElementById(common.promotionGroupLogoChangeButtonId);

    promotionGroupLogoChangeButton.innerText = "Change Image";

    const promotionGroupImageResizeButton = document.getElementById(common.promotionGroupImageResizeButtonId);
    const promotionGroupImageDeleteButton = document.getElementById(common.promotionGroupImageDeleteButtonId);

    promotionGroupImageResizeButton.addEventListener("click", openResizePopupForPromotionGroupImage);
    promotionGroupImageDeleteButton.addEventListener("click", removePromotionGroupImage);
}

async function getPromotionGroupLogoImage(imageUrl, contentType) {

    const promotionLogoGroupImageOptions = getPromotionGroupLogoImageOptions(imageUrl, contentType);

    const response = await fetch("api/components/promotionGroups/groupImages", {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            'Accept': "application/html",
        },
        body: JSON.stringify(promotionLogoGroupImageOptions)
    });

    if (!response.ok) return null;

    return await response.text();
}

function getPromotionGroupLogoImageOptions(imageUrl, contentType) {

    return {
        ImageUrl: imageUrl,
        ContentType: contentType,
    }
}

export async function savePromotionGroup(id = null) {

    const promotionGroupSaveButton = document.getElementById(common.promotionGroupSaveButtonId);

    try {

        promotionGroupSaveButton.classList.add(common.loadingClass);

        if (id == null) {

            await createPromotionGroup();

            return;
        }

        await updatePromotionGroup(id);
    }
    finally {
        promotionGroupSaveButton.classList.remove(common.loadingClass);
    }
}

async function createPromotionGroup() {

    const createRequest = getPromotionGroupCreateRequestFromCurrentData();

    if (createRequest == null) return;

    const response = await fetch("api/components/promotionGroups/createGroup", {
        method: "POST",
        headers: {
            "RequestVerificationToken": document.getElementById(common.antiforgeryTokenInputId).value
        },
        body: createRequest
    });

    if (!response.ok) return;

    const newPromotionIdAsString = await response.text();

    const newPromotionId = parseInt(newPromotionIdAsString);

    openPromotionGroupEditorPopup(newPromotionId);
}

function getPromotionGroupCreateRequestFromCurrentData() {

    const promotionGroupNameInput = document.getElementById(common.promotionGroupNameInputId);
    const promotionGroupDisplayOrderInput = document.getElementById(common.promotionGroupDisplayOrderInputId);
    const promotionGroupRelatedManufacturerSelect = document.getElementById(common.promotionGroupRelatedManufacturerSelectId);

    const formData = new FormData();

    const name = promotionGroupNameInput.value;
    const displayOrder = common.getIntegerOrNullFromString(promotionGroupDisplayOrderInput.value);
    const relatedManufacturerId = common.getIntegerOrNullFromString(promotionGroupRelatedManufacturerSelect.value);

    formData.append("Name", name);

    if (displayOrder != null) {

        formData.append("DisplayOrder", displayOrder);
    }

    if (relatedManufacturerId != null) {

        formData.append("RelatedManufacturerId", relatedManufacturerId);
    }

    if (promotionGroupImageToUpload != null) {

        let blob = promotionGroupImageToUpload.file;

        formData.append("LogoImage", blob);
    }

    return formData;
}

async function updatePromotionGroup(id) {

    const updateRequest = getPromotionGroupUpdateRequestFromCurrentData(id);

    if (updateRequest == null) return;

    const response = await fetch("api/components/promotionGroups/updateGroup", {
        method: "PUT",
        headers: {
            "RequestVerificationToken": document.getElementById(common.antiforgeryTokenInputId).value
        },
        body: updateRequest
    });

    if (!response.ok) return;

    await openPromotionGroupEditorPopup(id);
}

function getPromotionGroupUpdateRequestFromCurrentData(id) {

    const promotionGroupNameInput = document.getElementById(common.promotionGroupNameInputId);
    const promotionGroupDisplayOrderInput = document.getElementById(common.promotionGroupDisplayOrderInputId);
    const promotionGroupRelatedManufacturerSelect = document.getElementById(common.promotionGroupRelatedManufacturerSelectId);

    const formData = new FormData();

    const name = promotionGroupNameInput.value;
    const displayOrder = common.getIntegerOrNullFromString(promotionGroupDisplayOrderInput.value);
    const relatedManufacturerId = common.getIntegerOrNullFromString(promotionGroupRelatedManufacturerSelect.value);

    formData.append("Id", id);
    formData.append("Name", name);

    if (displayOrder != null) {

        formData.append("DisplayOrder", displayOrder);
    }

    if (relatedManufacturerId != null) {

        formData.append("RelatedManufacturerId", relatedManufacturerId);
    }

    if (promotionGroupImageToUpload != null) {

        let blob = promotionGroupImageToUpload.file;

        formData.append("NewLogoImage", blob);
        formData.append("PreserveOldImage", false);
    }
    else {
        const promotionGroupEditLogoImageContainer = document.getElementById(common.promotionGroupEditLogoImageContainerId);

        if (!promotionGroupEditLogoImageContainer) {
            formData.append("PreserveOldImage", false);
        }
        else {
            formData.append("PreserveOldImage", true);
        }
    }

    return formData;
}

export async function openResizePopupForPromotionGroupImage(e) {

    const promotionGroupIdHiddenInput = document.getElementById(common.promotionGroupIdHiddenInputId);

    const promotionGroupId = common.getIntegerOrNullFromString(promotionGroupIdHiddenInput?.value);

    let headerImageDataText = "Logo";

    const promotionGroupImage = document.getElementById(common.promotionGroupImageId);

    const imageContentType = e.currentTarget.getAttribute(common.promotionGroupEditorImageContentTypeAttribute);

    const imageResizePopupResponse = await imageResize.getImageResizePopupAsync(
        promotionGroupImage,
        headerImageDataText,
        150,
        null,
        null,
        imageContentType,
        {
            left: "60vw",
            right: "15vw",
            top: "15vh",
            bottom: "15vh"
        });

    if (imageResizePopupResponse == null) return null;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(imageResizePopupResponse);

    if (!didNotRedirect || !imageResizePopupResponse.ok) return null;

    const imageResizePopupHtml = await imageResizePopupResponse.text();

    const imageResizePopupContainer = document.getElementById(common.imageResizePopupContainerId);

    imageResizePopupContainer.innerHTML = imageResizePopupHtml;

    imageResize.initEventsAndInitialDataAsync(onImageResizeConfirmButtonClicked);

    const dialog = imageResizePopupContainer.querySelector("dialog");

    popup.attachCloseEvents(dialog);

    dialog.showModal();
}

async function onImageResizeConfirmButtonClicked() {

    const resizedImageData = await imageResize.getResizedImageAsync();

    const originalFileType = resizedImageData.originalFileType;
    const resizedImageBlob = resizedImageData.blob;

    const fileForResizedImage = new File([resizedImageBlob], "unusedFileName", {
        type: originalFileType
    });

    const fileBlobUrl = URL.createObjectURL(resizedImageBlob);

    const promotionGroupImage = document.getElementById(common.promotionGroupImageId);
    const promotionGroupImageResizeButton = document.getElementById(common.promotionGroupImageResizeButtonId);

    promotionGroupImage.src = fileBlobUrl,

    promotionGroupImageResizeButton.setAttribute(common.promotionGroupEditorImageContentTypeAttribute, originalFileType);

    if (promotionGroupImageToUpload != null) {

        URL.revokeObjectURL(promotionGroupImageToUpload.fileObjectUrl);
    }
     
    promotionGroupImageToUpload = {
        file: fileForResizedImage,
        fileObjectUrl: fileBlobUrl
    };
}

export function removePromotionGroupImage() {

    const promotionGroupEditLogoImageContainer = document.getElementById(common.promotionGroupEditLogoImageContainerId);

    promotionGroupEditLogoImageContainer.remove();

    const promotionGroupLogoChangeButton = document.getElementById(common.promotionGroupLogoChangeButtonId);

    promotionGroupLogoChangeButton.innerText = "Add Image";
}
