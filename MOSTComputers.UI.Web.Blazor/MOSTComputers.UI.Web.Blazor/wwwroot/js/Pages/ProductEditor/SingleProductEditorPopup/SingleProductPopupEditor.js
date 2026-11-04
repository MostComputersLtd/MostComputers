import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as antiforgeryUtils from "../../Common/AntiforgeryUtils.js";
import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"
import * as propertiesEditor from "./PropertiesEditor.js"
import * as productDocumentsEditor from "./ProductDocumentsEditor.js"
import * as productImagesEditor from "./ProductImagesEditor.js"
import * as promotionFilePopup from "./PromotionFilePopup.js"

const popupId = "singleEditorProductPopup";

const productIdHiddenInputId = "singleEditorProductIdHiddenInput";

const showProductDataPopupButtonId = "singleEditorShowProductDataPopupButton";
const showProductLegacyDisplayPopupButtonId = "singleEditorShowProductLegacyDisplayPopupButton";
const showSearchStringPopupButtonId = "singleEditorShowSearchStringPopupButton";

const productDataPopupContainerId = "singleEditorProductDataPopupContainer";
const productLegacyDisplayPopupContainerId = "singleEditorProductLegacyDisplayPopupContainer";
const promotionProductFileEditorPopupContainerId = "singleEditorPromotionProductFileEditorPopupContainer";
const searchStringPopupContainerId = "singleEditorSearchStringPopupContainer";
const unsavedChangesPopupContainerId = "singleEditorUnsavedChangesPopupContainer";

const productNewStatusSelectId = "singleEditorProductNewStatusSelect";
const addImageFileButtonId = "singleEditorAddImageFileButton";
const addImageFileInputId = "singleEditorAddImageFileInput";
const addDocumentFileButtonId = "singleEditorAddDocumentFileButton";
const addDocumentFileInputId = "singleEditorAddDocumentFileInput";
const addImageFromClipboardFileButtonId = "singleEditorAddImageFromClipboardFileButton";

const saveButtonContainerId = "singleProductEditorSaveButtonContainer";
const saveButtonId = "singleProductEditorSaveButton";

const unsavedChangesPopupSaveButtonName = "unsavedChangesPopupSaveButton";
const unsavedChangesPopupExitButtonName = "unsavedChangesPopupExitButton";

const loadingClass = "loading";

export function initEvents(dialogElement) {

    const productNewStatusSelect = document.getElementById(productNewStatusSelectId);

    const showProductDataPopupButton = document.getElementById(showProductDataPopupButtonId);
    const showProductLegacyDisplayPopupButton = document.getElementById(showProductLegacyDisplayPopupButtonId);
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

export function clearAll() {
    clearWindowEvents();
    clearCallbacks();
    clearData();
}

export function clearData() {

    productImagesEditor.clearObjectUrls();
    productDocumentsEditor.clearObjectUrls();
    promotionFilePopup.clearInitializedData();

    editorChanges.clearChanges();
    dataStore.clearData();
}

export function clearWindowEvents() {
    window.removeEventListener("keydown", saveOnShortcut);
}

export function clearCallbacks() {
    editorChanges.removeCallback();
}

async function onProductStatusSelectChangedAsync(e) {

    const productStatus = e.currentTarget.value;

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

    e.currentTarget.value = productNewStatusValue;
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

    const success = await productDocumentsEditor.addFileToProductDocumentsAsync(file);

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
        ProductImageUrls: productImageUrls,
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

            case "uploadedFile":
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

    const url = `api/components/productEditor/single/promotionFile/showPopup/${productId}`;

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

    const productImages = productImagesEditor.getProductImagesRequestData();

    const relatedImage = productImages.find(x =>
    {
        const itemType = x["$type"];

        return itemType == "promotionProductFile" && x.promotionFileEditorId == editorId;
    });

    if (!promotionProductFileData.shouldAddToImages) {

        if (relatedImage != null) {

            const relatedImageItem = document.getElementById(editorId);

            productImagesEditor.removeProductImage(relatedImageItem);
        }

        return;
    }

    productImagesEditor.changePromotionProductFileImageItemAsync(
        promotionProductFileData.editorId,
        promotionProductFileData.selectedFileId)
}

async function onPromotionProductFileEditorItemDeletedAsync(editorId) {

    const productImages = productImagesEditor.getProductImagesRequestData();

    const relatedImage = productImages.find(x =>
    {
        const itemType = x["$type"];

        return itemType == "promotionProductFile" && x.promotionFileEditorId == editorId;
    });

    if (relatedImage == null) return;

    const relatedImageItem = document.getElementById(editorId);

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

        const dialogElement = document.getElementById(popupId);

        dialogElement.close();
    }
}

async function showUnsavedChangesPopup() {

    const url = "api/components/productEditor/single/unsavedChangesPopup/";

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

    const saveButtonContainer = document.getElementById(saveButtonContainerId);
    const saveButton = document.getElementById(saveButtonId);

    saveButtonContainer.classList.add(loadingClass);
    saveButton.setAttribute("disabled");

    try {
        response = await saveFromCurrentDataAsync();
    }
    finally {
        saveButtonContainer.classList.remove(loadingClass);
        saveButton.removeAttribute("disabled");
    }

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = await response.json();

    clearAll();

    const unsavedChangesPopupContainer = document.getElementById(unsavedChangesPopupContainerId);

    const unsavedChangesDialog = unsavedChangesPopupContainer.querySelector("dialog");

    unsavedChangesDialog.close();

    const singleProductEditorDialog = document.getElementById(popupId);

    const dialogContainer = singleProductEditorDialog.parentElement;

    dialogContainer.innerHTML = data;

    const newSingleProductEditorDialog = document.getElementById(popupId);

    const jsonDataScriptElement = document.getElementById(singleProductEditorPopupJsonDataScriptId);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    registerData(jsonData);

    jsonDataScriptElement.remove();

    newSingleProductEditorDialog.showModal();
}

async function onUnsavedChangesPopupExitButtonClicked() {

    clearAll();

    const unsavedChangesPopupContainer = document.getElementById(unsavedChangesPopupContainerId);

    const unsavedChangesDialog = unsavedChangesPopupContainer.querySelector("dialog");

    unsavedChangesDialog.close();

    const dialogElement = document.getElementById(popupId);

    dialogElement.close();
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

    const saveButtonContainer = document.getElementById(saveButtonContainerId);
    const saveButton = document.getElementById(saveButtonId);

    saveButtonContainer.classList.add(loadingClass);
    saveButton.setAttribute("disabled");

    try {
        response = await saveFromCurrentDataAsync();
    }
    finally {
        saveButtonContainer.classList.remove(loadingClass);
        saveButton.removeAttribute("disabled");
    }

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = await response.json();

    clearAll();

    const dialog = document.getElementById(popupId);

    const dialogContainer = dialog.parentElement;

    dialogContainer.innerHTML = data;

    const newDialog = document.getElementById(popupId);

    const jsonDataScriptElement = document.getElementById(singleProductEditorPopupJsonDataScriptId);

    const jsonData = JSON.parse(jsonDataScriptElement.textContent);

    registerData(jsonData);

    jsonDataScriptElement.remove();

    newDialog.showModal();
}

async function saveFromCurrentDataAsync() {

    const formData = new FormData();

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

        if (imageType !== "uploadedFile") continue;

        formData.append(editorId, imageRequest.uploadedFile);

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
        productProperties: propertyRequests,
        productImages: imageRequests,
        promotionProductFiles: promotionProductFileRequests,
        productDocuments: productDocuments,
        uploadedProductDocuments: uploadedProductDocuments,
    };

    formData.append("data", JSON.stringify(request));

    const url = `api/components/productEditor/single/save`;

    return await fetch(url, {
        method: "PUT",
        headers: {
            [antiforgeryUtils.antiforgeryHeaderName]: antiforgeryUtils.getAntiforgeryTokenValue()
        },
        body: formData
    }); 
}
