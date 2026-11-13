import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as antiforgeryUtils from "../../../Common/AntiforgeryUtils.js";
import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"
import * as propertiesEditor from "./PropertiesEditor.js"
import * as productDocumentsEditor from "./ProductDocumentsEditor.js"
import * as productImagesEditor from "./ProductImagesEditor.js"
import * as promotionFilePopup from "./PromotionFilePopup.js"

const popupId = "singleProductEditorPopup";

const productIdHiddenInputId = "singleEditorProductIdHiddenInput";

const productNewStatusSelectId = "singleEditorProductNewStatusSelect";

const showProductDataPopupButtonId = "singleEditorShowProductDataPopupButton";
const showProductLegacyDisplayPopupButtonId = "singleEditorShowProductLegacyDisplayPopupButton";
const showPromotionProductFileEditorPopupButtonId = "singleEditorShowPromotionProductFileEditorPopupButton";
const showSearchStringPopupButtonId = "singleEditorShowSearchStringPopupButton";

const addImageFileButtonId = "singleEditorAddImageFileButton";
const addImageFileInputId = "singleEditorAddImageFileInput";
const addDocumentFileButtonId = "singleEditorAddDocumentFileButton";
const addDocumentFileInputId = "singleEditorAddDocumentFileInput";
const addImageFromClipboardFileButtonId = "singleEditorAddImageFromClipboardFileButton";

const productDataPopupContainerId = "singleEditorProductDataPopupContainer";
const productLegacyDisplayPopupContainerId = "singleEditorProductLegacyDisplayPopupContainer";
const promotionProductFileEditorPopupContainerId = "singleEditorPromotionProductFileEditorPopupContainer";
const searchStringPopupContainerId = "singleEditorSearchStringPopupContainer";
const unsavedChangesPopupContainerId = "singleEditorUnsavedChangesPopupContainer";

const saveButtonId = "singleProductEditorSaveButton";

const unsavedChangesPopupSaveButtonName = "unsavedChangesPopupSaveButton";
const unsavedChangesPopupExitButtonName = "unsavedChangesPopupExitButton";

const loadingClass = "loading";

export function initEvents(dialogElement) {

    const productNewStatusSelect = document.getElementById(productNewStatusSelectId);

    const showProductDataPopupButton = document.getElementById(showProductDataPopupButtonId);
    const showProductLegacyDisplayPopupButton = document.getElementById(showProductLegacyDisplayPopupButtonId);
    const showPromotionProductFileEditorPopupButton = document.getElementById(showPromotionProductFileEditorPopupButtonId);
    const showSearchStringPopupButton = document.getElementById(showSearchStringPopupButtonId);

    const addImageFileButton = document.getElementById(addImageFileButtonId);
    const addImageFileInput = document.getElementById(addImageFileInputId);

    const addDocumentFileButton = document.getElementById(addDocumentFileButtonId);
    const addDocumentFileInput = document.getElementById(addDocumentFileInputId);

    const addImageFromClipboardFileButton = document.getElementById(addImageFromClipboardFileButtonId);

    const saveButton = document.getElementById(saveButtonId);

    productNewStatusSelect.addEventListener("change", onProductStatusSelectChangedAsync);

    showProductDataPopupButton.addEventListener("click", showProductDataPopupAsync)
    showProductLegacyDisplayPopupButton.addEventListener("click", showProductLegacyDisplayPopupAsync)
    showPromotionProductFileEditorPopupButton.addEventListener("click", showPromotionProductFileEditorPopupAsync);
    showSearchStringPopupButton.addEventListener("click", showProductSearchStringPopupAsync);

    addImageFileButton.addEventListener("click", onAddImageFileButtonClick);
    addImageFileInput.addEventListener("change", onAddImageFileInputChangeAsync);

    addDocumentFileButton.addEventListener("click", onAddDocumentFileButtonClick);
    addDocumentFileInput.addEventListener("change", onAddDocumentFileInputChangeAsync);

    addImageFromClipboardFileButton.addEventListener("click", onAddImageFromClipboardButtonClickAsync);

    saveButton.addEventListener("click", onSaveButtonClicked);

    window.addEventListener("keydown", saveOnShortcut);

    const closeButton = dialogElement.querySelector(".btn-close");

    closeButton.onclick = null;

    closeButton.addEventListener("click", onCloseButtonClickedAsync);

    propertiesEditor.initEvents(dialogElement);
    productDocumentsEditor.initEvents(dialogElement);
    productImagesEditor.initEvents(dialogElement);

    editorChanges.setCallback(changeSaveButtonText);
}

export function resizeElements(dialogElement) {
    propertiesEditor.resizeTextareas(dialogElement);
}

function changeSaveButtonText() {

    const anyChanges = editorChanges.anyChanges();

    const saveButton = document.getElementById(saveButtonId);

    if (anyChanges) {

        saveButton.textContent = "Save *";

        return;
    }

    saveButton.textContent = "Save";
}

export function registerData(jsonDataObject) {

    promotionFilePopup.initData(jsonDataObject.promotionProductFilesForEditor);

    delete jsonDataObject.promotionProductFilesForEditor;

    dataStore.saveData(jsonDataObject);
}

export function setOnSuccessfulSaveCallback(onSuccessfulSaveCallback) {

    const dialog = document.getElementById(popupId);

    dialog.__onSuccessfulSaveCallback = onSuccessfulSaveCallback;
}

export function clearAll() {
    clearWindowEvents();
    clearCallbacks();
    clearData();
}

export function clearData() {

    productImagesEditor.clearData();
    productDocumentsEditor.clearData();
    promotionFilePopup.clearInitializedData();

    editorChanges.clearChanges();
    dataStore.clearData();
}

export function clearWindowEvents() {
    window.removeEventListener("keydown", saveOnShortcut);
}

export function clearCallbacks() {

    const dialog = document.getElementById(popupId);

    dialog.__onSuccessfulSaveCallback = null;

    editorChanges.removeCallback();
}

export function getProductId() {

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    return parseInt(productIdHiddenInput.value);
}

async function onProductStatusSelectChangedAsync(e) {

    const productNewStatusSelect = e.currentTarget;

    const productStatus = productNewStatusSelect.value;

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    const productId = productIdHiddenInput.value;

    const url = `api/components/productEditor/single/updateProductWorkStatus/${productId}/${productStatus}`;

    const response = await fetch(url, {
        method: "PUT",
        headers: {
            "Accept": "application/json"
        },
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = await response.json();

    const productNewStatusValue = data.productNewStatus;

    productNewStatusSelect.value = productNewStatusValue;
}

function onAddImageFileButtonClick() {

    const addImageFileInput = document.getElementById(addImageFileInputId);

    addImageFileInput.click();
}

async function onAddImageFileInputChangeAsync(e) {

    const singleEditorAddImageFileInput = e.currentTarget;

    if (singleEditorAddImageFileInput.files.length === 0) {

        return;
    }

    const file = singleEditorAddImageFileInput.files.item(0);

    const success = await productImagesEditor.addFileToImagesAsync(file);

    if (!success) {
    }
}

function onAddDocumentFileButtonClick() {

    const addDocumentFileInput = document.getElementById(addDocumentFileInputId);

    addDocumentFileInput.click();
}

async function onAddDocumentFileInputChangeAsync(e) {

    const singleEditorAddImageFileInput = e.currentTarget;

    if (singleEditorAddImageFileInput.files.length === 0) {

        return;
    }

    const file = singleEditorAddImageFileInput.files.item(0);

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    const productId = productIdHiddenInput.value;

    const success = await productDocumentsEditor.addFileToProductDocumentsAsync(file, productId);

    if (!success) {
    }
}

async function onAddImageFromClipboardButtonClickAsync() {

    const items = await navigator.clipboard.read();

    let blob;

    for (const item of items) {

        const imageType = item.types.find(type => type.startsWith("image/"));

        if (imageType) {

            blob = await item.getType(imageType);

            break;
        }
    }

    if (!blob) {

        return;
    }

    const success = await productImagesEditor.addFileToImagesAsync(blob);

    if (!success) {
    }
}

async function showProductDataPopupAsync() {

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    const productId = productIdHiddenInput.value;

    const allPropertyData = propertiesEditor.getProductPropertyRequestData();

    const propertyRequests = [];

    for (const property of allPropertyData) {

        if (!property.active) continue;

        const propertyRequest = {
            productCharacteristicId: property.productCharacteristicId,
            value: property.value,
        };

        propertyRequests.push(propertyRequest);
    }

    const productImageUrls = [];

    for (const productImageItem of productImagesEditor.getProductImageItems()) {

        const imageUrl = productImagesEditor.getProductImageItemImageUrl(productImageItem);

        productImageUrls.push(imageUrl);
    }

    const body = {
        productProperties: propertyRequests,
        productImageUrls: productImageUrls,
    };

    const url = `api/components/productEditor/single/productDataPopup/${productId}`;

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        return;
    }

    const data = await response.text();

    const productDataPopupContainer = document.getElementById(productDataPopupContainerId);

    productDataPopupContainer.innerHTML = data;

    const dialog = productDataPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductLegacyDisplayPopupAsync() {

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    const productId = productIdHiddenInput.value;

    const allPropertyData = propertiesEditor.getProductPropertyRequestData();

    const propertyRequests = [];

    for (const property of allPropertyData) {

        if (!property.active) continue;

        const propertyRequest = {
            productCharacteristicId: property.productCharacteristicId,
            value: property.value,
        };

        propertyRequests.push(propertyRequest);
    }

    const productImageRequests = [];

    const productImageItems = productImagesEditor.getProductImageItems();

    for (let i = 0; i < productImageItems.length; i++) {

        const productImageItem = productImageItems[i];

        const productImageData = productImagesEditor.getProductImageItemRequestData(productImageItem);
        const imageUrl = productImagesEditor.getProductImageItemImageUrl(productImageItem);

        let imageRequest;

        switch (productImageData["$type"]) {

            case "promotionProductFile":
                imageRequest = {
                    imageUrl: imageUrl,
                    active: productImageData.active,
                    displayOrder: i,
                };

                break;

            case "image":
                imageRequest = {
                    imageUrl: imageUrl,
                    active: productImageData.saveFileRequest?.active,
                    displayOrder: i,
                };

                break;

            case "imageFile":
                imageRequest = {
                    imageUrl: imageUrl,
                    active: productImageData.active,
                    displayOrder: i,
                };

                break;

            case "uploadedImage":
                imageRequest = {
                    imageUrl: imageUrl,
                    active: productImageData.active,
                    displayOrder: i,
                };

                break;

            default:
                break;
        }

        productImageRequests.push(imageRequest);
    }

    const url = `api/components/productEditor/single/productLegacyDisplayPopup/${productId}`;

    const body = {
        productProperties: propertyRequests,
        productImages: productImageRequests
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        return;
    }

    const data = await response.text();

    const productLegacyDisplayPopupContainer = document.getElementById(productLegacyDisplayPopupContainerId);

    productLegacyDisplayPopupContainer.innerHTML = data;

    const dialog = productLegacyDisplayPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showPromotionProductFileEditorPopupAsync() {

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    const productId = productIdHiddenInput.value;

    const url = `api/components/productEditor/single/promotionFile/showPopup`;

    const promotionProductFileData = promotionFilePopup.getPromotionProductFileRequests();

    const body = {
        productId: productId,
        promotionProductFiles: promotionProductFileData,
    };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (!response.ok) {
        return;
    }

    const data = await response.text();

    const promotionProductFileEditorPopupContainer = document.getElementById(promotionProductFileEditorPopupContainerId);

    promotionProductFileEditorPopupContainer.innerHTML = data;

    const dialog = promotionProductFileEditorPopupContainer.querySelector("dialog");

    promotionFilePopup.initEvents(
        dialog,
        onPromotionProductFileEditorItemAddedAsync,
        onPromotionProductFileEditorItemUpdatedAsync,
        onPromotionProductFileEditorItemDeletedAsync);

    dialog.showModal();
}

async function onPromotionProductFileEditorItemAddedAsync(promotionProductFileData) {

    if (!promotionProductFileData.shouldAddToImages) return;

    await productImagesEditor.addNewPromotionProductFileToImagesAsync(
        promotionProductFileData.editorId,
        promotionProductFileData.selectedFileId);
}

async function onPromotionProductFileEditorItemUpdatedAsync(promotionProductFileData) {

    const editorId = promotionProductFileData.editorId;

    const relatedImageItem = productImagesEditor.getProductImageItemRelatedToPromotionFile(editorId);

    if (!promotionProductFileData.shouldAddToImages) {

        if (relatedImageItem != null) {

            productImagesEditor.removeProductImage(relatedImageItem);
        }

        return;
    }

    productImagesEditor.changePromotionProductFileImageItemAsync(
        editorId,
        promotionProductFileData.selectedFileId,
        promotionProductFileData.active);
}

async function onPromotionProductFileEditorItemDeletedAsync(editorId) {

    const relatedImageItem = productImagesEditor.getProductImageItemRelatedToPromotionFile(editorId);

    if (relatedImageItem == null) return;

    productImagesEditor.removeProductImage(relatedImageItem);
}

async function showProductSearchStringPopupAsync() {

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    const productId = productIdHiddenInput.value;

    const url = `api/components/productEditor/productSearchStringPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const searchStringPopupContainer = document.getElementById(searchStringPopupContainerId);

    searchStringPopupContainer.innerHTML = data;

    const dialog = searchStringPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function onCloseButtonClickedAsync() {

    const anyChanges = editorChanges.anyChanges();

    if (anyChanges) {

        await showUnsavedChangesPopup();
    }
    else {
        clearAll();

        const dialog = document.getElementById(popupId);

        dialog.close();
    }
}

async function showUnsavedChangesPopup() {

    const url = "api/components/productEditor/single/unsavedChangesPopup";

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const unsavedChangesPopupContainer = document.getElementById(unsavedChangesPopupContainerId);

    unsavedChangesPopupContainer.innerHTML = data;

    const dialog = unsavedChangesPopupContainer.querySelector("dialog");

    dialog.addEventListener("cancel", onUnsavedChangesPopupClose);

    const unsavedChangesPopupSaveButton = dialog.querySelector(`[name='${unsavedChangesPopupSaveButtonName}']`)
    const unsavedChangesPopupExitButton = dialog.querySelector(`[name='${unsavedChangesPopupExitButtonName}']`)

    unsavedChangesPopupSaveButton.addEventListener("click", onUnsavedChangesPopupSaveButtonClicked);
    unsavedChangesPopupExitButton.addEventListener("click", onUnsavedChangesPopupExitButtonClicked);

    dialog.showModal(); 
}

function onUnsavedChangesPopupClose(e) {

    e.preventDefault();
}

async function onUnsavedChangesPopupSaveButtonClicked() {

    let response = null;

    const saveButton = document.getElementById(saveButtonId);

    saveButton.classList.add(loadingClass);
    saveButton.setAttribute("disabled", "disabled");

    let formData;

    try {
        formData = getSaveRequestFromCurrentData();

        response = await saveAsync(formData);
    }
    finally {
        saveButton.classList.remove(loadingClass);
        saveButton.removeAttribute("disabled");
    }

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        const responseProblem = await response.json();

        if (responseProblem?.type === "/problems/validation") {

            const dataFromRequestEntry = formData.get("data");

            const dataFromRequest = JSON.parse(dataFromRequestEntry);

            propertiesEditor.displayValidationMessages(responseProblem.validationErrors, dataFromRequest.productProperties);
            productImagesEditor.displayValidationMessages(responseProblem.validationErrors);
            productDocumentsEditor.displayValidationMessages(responseProblem.validationErrors);
        }

        return;
    }

    const data = await response.json();

    const unsavedChangesPopupContainer = document.getElementById(unsavedChangesPopupContainerId);

    const unsavedChangesDialog = unsavedChangesPopupContainer.querySelector("dialog");

    unsavedChangesDialog.close();

    const singleProductEditorDialog = document.getElementById(popupId);

    const onSuccessfulSaveCallback = singleProductEditorDialog.__onSuccessfulSaveCallback;

    clearAll();

    const dialogContainer = singleProductEditorDialog.parentElement;

    dialogContainer.innerHTML = data;

    const newSingleProductEditorDialog = document.getElementById(popupId);

    const jsonDataScriptElement = document.getElementById(singleProductEditorPopupJsonDataScriptId);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    registerData(jsonData);

    jsonDataScriptElement.remove();

    initEvents(newSingleProductEditorDialog);

    newSingleProductEditorDialog.__onSuccessfulSaveCallback = onSuccessfulSaveCallback;

    newSingleProductEditorDialog.showModal();
}

async function onUnsavedChangesPopupExitButtonClicked() {

    clearAll();

    const unsavedChangesPopupContainer = document.getElementById(unsavedChangesPopupContainerId);

    const unsavedChangesDialog = unsavedChangesPopupContainer.querySelector("dialog");

    unsavedChangesDialog.close();

    const dialog = document.getElementById(popupId);

    dialog.close();
}

async function saveOnShortcut(e) {

    if (!e.isTrusted || !e.ctrlKey || e.key !== "s") return;

    console.log("EVENT Single Editor");

    e.preventDefault();
    e.stopPropagation();

    await onSaveButtonClicked();
}

async function onSaveButtonClicked() {

    let response = null;

    const saveButton = document.getElementById(saveButtonId);

    saveButton.classList.add(loadingClass);
    saveButton.setAttribute("disabled", "disabled");

    let formData;

    try {
        formData = getSaveRequestFromCurrentData();

        response = await saveAsync(formData);
    }
    finally {
        saveButton.classList.remove(loadingClass);
        saveButton.removeAttribute("disabled");
    }

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        const responseProblem = await response.json();

        if (responseProblem?.type === "/problems/validation") {

            const dataFromRequestEntry = formData.get("data");

            const dataFromRequest = JSON.parse(dataFromRequestEntry);

            propertiesEditor.displayValidationMessages(responseProblem.validationErrors, dataFromRequest.productProperties);
            productImagesEditor.displayValidationMessages(responseProblem.validationErrors);
            productDocumentsEditor.displayValidationMessages(responseProblem.validationErrors);
        }

        return;
    }

    const data = await response.json();

    const dialog = document.getElementById(popupId);

    const onSuccessfulSaveCallback = dialog.__onSuccessfulSaveCallback;

    clearAll();

    const dialogContainer = dialog.parentElement;

    dialogContainer.innerHTML = data;

    const newDialog = document.getElementById(popupId);

    const jsonDataScriptElement = document.getElementById(singleProductEditorPopupJsonDataScriptId);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    registerData(jsonData);

    jsonDataScriptElement.remove();

    initEvents(newDialog);

    newDialog.__onSuccessfulSaveCallback = onSuccessfulSaveCallback;

    newDialog.showModal();

    onSuccessfulSaveCallback();
}

async function saveAsync(requestData) {

    for (const [name, data] of requestData.entries()) {

        if (name === "data") {

            console.log(JSON.parse(data));

            continue;
        }

        console.log(`${name}: ${data.type}, ${data.size}`);
    }

    const url = `api/components/productEditor/single/save`;

    const response = await fetch(url, {
        method: "PUT",
        headers: {
            "Accept": "text/html, application/problem+json",
            [antiforgeryUtils.antiforgeryHeaderName]: antiforgeryUtils.getAntiforgeryTokenValue()
        },
        body: requestData
    });

    if (response.ok) {
        console.log("SUCCESS");
    }
    else if (response.status !== 400) {
        console.log(response);
    }

    return response;
}

function getSaveRequestFromCurrentData() {

    const formData = new FormData();

    const productIdHiddenInput = document.getElementById(productIdHiddenInputId);

    const productId = parseInt(productIdHiddenInput.value);

    const allPropertyData = propertiesEditor.getProductPropertyRequestData();

    const propertyRequests = [];

    for (const property of allPropertyData) {

        if (!property.active) continue;

        const propertyRequest = {
            productCharacteristicId: property.productCharacteristicId,
            value: property.value,
        };

        propertyRequests.push(propertyRequest);
    }

    const imageRequests = productImagesEditor.getProductImagesRequestData();

    for (const imageRequest of imageRequests) {

        const imageType = imageRequest["$type"];

        if (imageType !== "uploadedImage") continue;

        formData.append(imageRequest.editorId, imageRequest.uploadedFile);

        delete imageRequest.uploadedFile;
    }

    const promotionProductFileEditorDatas = promotionFilePopup.getPromotionProductFileRequests();

    const promotionProductFileRequests = [];

    for (const promotionProductFileEditData of promotionProductFileEditorDatas) {

        const promotionProductFileRequest = {
            editorId: promotionProductFileEditData.editorId,
            id: promotionProductFileEditData.id,
            promotionFileId: promotionProductFileEditData.promotionFileId,
            active: promotionProductFileEditData.active,
            validFrom: promotionProductFileEditData.validFrom,
            validTo: promotionProductFileEditData.validTo,
        };

        promotionProductFileRequests.push(promotionProductFileRequest);
    }

    const documentRequests = productDocumentsEditor.getProductDocumentRequests();

    const productDocuments = [];
    const uploadedProductDocuments = [];

    for (const documentRequest of documentRequests) {

        if (documentRequest.id) {

            productDocuments.push(documentRequest);

            continue;
        }

        formData.append(documentRequest.editorId, documentRequest.file);

        delete documentRequest.file;

        uploadedProductDocuments.push(documentRequest);
    }

    const request = {
        productId: productId,
        productProperties: propertyRequests,
        productImages: imageRequests,
        promotionProductFiles: promotionProductFileRequests,
        productDocuments: productDocuments,
        uploadedProductDocuments: uploadedProductDocuments,
    };

    formData.append("data", JSON.stringify(request));

    return formData;
}
