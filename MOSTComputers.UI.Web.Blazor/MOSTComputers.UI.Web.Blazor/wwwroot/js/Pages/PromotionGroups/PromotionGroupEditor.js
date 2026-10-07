import * as fetchWithAuthRedirect from "../../Authentication/FetchWithAuthRedirect.js";
import * as common from "./Common.js";

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

    const promotionGroupImageDeleteButton = document.getElementById(common.promotionGroupImageDeleteButtonId);

    if (promotionGroupImageDeleteButton) {

        promotionGroupImageDeleteButton.addEventListener("click", removePromotionGroupImage);
    }

    const promotionGroupEditorPopup = document.getElementById(common.promotionGroupEditorPopupId);

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

    const newImageElementHtml = await getPromotionGroupLogoImage(fileObjectUrl);

    if (promotionGroupImageToUpload != null) {

        URL.revokeObjectURL(promotionGroupImageToUpload.fileObjectUrl);
    }

    const promotionGroupLogoImageDisplay = document.getElementById(common.promotionGroupLogoImageDisplayId);

    promotionGroupLogoImageDisplay.innerHTML = newImageElementHtml;

    promotionGroupImageToUpload = {
        File: file,
        fileObjectUrl: fileObjectUrl
    };

     const promotionGroupLogoChangeButton = document.getElementById(common.promotionGroupLogoChangeButtonId);

    promotionGroupLogoChangeButton.innerText = "Change Image";

    const promotionGroupImageDeleteButton = document.getElementById(common.promotionGroupImageDeleteButtonId);

    promotionGroupImageDeleteButton.addEventListener("click", removePromotionGroupImage);
}

async function getPromotionGroupLogoImage(imageUrl) {

    const promotionLogoGroupImageOptions = getPromotionGroupLogoImageOptions(imageUrl);

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

function getPromotionGroupLogoImageOptions(imageUrl) {

    return {
        ImageUrl: imageUrl
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

function getPromotionGroupCreateRequestFromCurrentData() {

    const promotionGroupNameInput = document.getElementById(common.promotionGroupNameInputId);
    const promotionGroupDisplayOrderInput = document.getElementById(common.promotionGroupDisplayOrderInputId);

    const formData = new FormData();

    const promotionGroupName = promotionGroupNameInput.value;
    const promotionGroupDisplayOrder = common.getIntegerOrNullFromString(promotionGroupDisplayOrderInput.value);

    formData.append("Name", promotionGroupName);
    formData.append("DisplayOrder", promotionGroupDisplayOrder);

    if (promotionGroupImageToUpload != null) {

        let blob = promotionGroupImageToUpload.File;

        formData.append("LogoImage", blob);
    }

    return formData;
}

function getPromotionGroupUpdateRequestFromCurrentData(id) {

    const promotionGroupNameInput = document.getElementById(common.promotionGroupNameInputId);
    const promotionGroupDisplayOrderInput = document.getElementById(common.promotionGroupDisplayOrderInputId);

    const formData = new FormData();

    const promotionGroupName = promotionGroupNameInput.value;
    const promotionGroupDisplayOrder = common.getIntegerOrNullFromString(promotionGroupDisplayOrderInput.value);

    formData.append("Id", id);
    formData.append("Name", promotionGroupName);
    formData.append("DisplayOrder", promotionGroupDisplayOrder);

    if (promotionGroupImageToUpload != null) {

        let blob = promotionGroupImageToUpload.File;

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

export function removePromotionGroupImage() {

    const promotionGroupEditLogoImageContainer = document.getElementById(common.promotionGroupEditLogoImageContainerId);

    promotionGroupEditLogoImageContainer.remove();

    const promotionGroupLogoChangeButton = document.getElementById(common.promotionGroupLogoChangeButtonId);

    promotionGroupLogoChangeButton.innerText = "Add Image";
}
