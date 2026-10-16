import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as antiforgeryUtils from "../../Common/AntiforgeryUtils.js";
import * as dataStore from "./DataStore.js"
import * as editorChanges from "./EditorChanges.js"
import * as propertiesEditor from "./PropertiesEditor.js"
import * as productDocumentsEditor from "./ProductDocumentsEditor.js"
import * as productImagesEditor from "./ProductImagesEditor.js"
import * as promotionFilePopup from "./PromotionFilePopup.js"

const singleEditorProductIdHiddenInputId = "singleEditorProductIdHiddenInput";

const singleEditorShowProductDataPopupButtonId = "singleEditorShowProductDataPopupButton";
const singleEditorShowProductLegacyDisplayPopupButtonId = "singleEditorShowProductLegacyDisplayPopupButton";
const singleEditorShowSearchStringPopupButtonId = "singleEditorShowSearchStringPopupButton";

const singleEditorProductDataPopupContainerId = "singleEditorProductDataPopupContainer";
const singleEditorProductLegacyDisplayPopupContainerId = "singleEditorProductLegacyDisplayPopupContainer";
const singleEditorPromotionProductFileEditorPopupContainerId = "singleEditorPromotionProductFileEditorPopupContainer";
const singleEditorSearchStringPopupContainerId = "singleEditorSearchStringPopupContainer";

const singleEditorProductNewStatusSelectId = "singleEditorProductNewStatusSelect";
const singleEditorAddImageFileButtonId = "singleEditorAddImageFileButton";
const singleEditorAddImageFileInputId = "singleEditorAddImageFileInput";
const singleEditorAddDocumentFileButtonId = "singleEditorAddDocumentFileButton";
const singleEditorAddDocumentFileInputId = "singleEditorAddDocumentFileInput";
const singleEditorAddImageFromClipboardFileButtonId = "singleEditorAddImageFromClipboardFileButton";

const singleProductEditorSaveButtonId = "singleProductEditorSaveButton";

export function initEvents(dialogElement) {

    const singleEditorProductNewStatusSelect = document.getElementById(singleEditorProductNewStatusSelectId);

    singleEditorProductNewStatusSelect.addEventListener("change", onProductStatusSelectChangedAsync);

    const singleEditorShowProductDataPopupButton = document.getElementById(singleEditorShowProductDataPopupButtonId);

    singleEditorShowProductDataPopupButton.addEventListener("click", showProductDataPopupAsync)

    const singleEditorShowProductLegacyDisplayPopupButton = document.getElementById(singleEditorShowProductLegacyDisplayPopupButtonId);

    singleEditorShowProductLegacyDisplayPopupButton.addEventListener("click", showProductLegacyDisplayPopupAsync)

    const singleEditorShowSearchStringPopupButton = document.getElementById(singleEditorShowSearchStringPopupButtonId);

    singleEditorShowSearchStringPopupButton.addEventListener("click", showProductSearchStringPopupAsync);

    const singleEditorAddImageFileButton = document.getElementById(singleEditorAddImageFileButtonId);

    singleEditorAddImageFileButton.addEventListener("click", onAddImageFileButtonClick);

    const singleEditorAddImageFileInput = document.getElementById(singleEditorAddImageFileInputId);

    singleEditorAddImageFileInput.addEventListener("change", onAddImageFileInputChangeAsync);

    const singleEditorAddDocumentFileButton = document.getElementById(singleEditorAddDocumentFileButtonId);

    singleEditorAddDocumentFileButton.addEventListener("click", onAddDocumentFileButtonClick);

    const singleEditorAddDocumentFileInput = document.getElementById(singleEditorAddDocumentFileInputId);

    singleEditorAddDocumentFileInput.addEventListener("change", onAddDocumentFileInputChangeAsync);

    const singleEditorAddImageFromClipboardFileButton = document.getElementById(singleEditorAddImageFromClipboardFileButtonId);

    singleEditorAddImageFromClipboardFileButton.addEventListener("click", onAddImageFromClipboardButtonClickAsync);

    const singleProductEditorSaveButton = document.getElementById(singleProductEditorSaveButtonId);

    singleProductEditorSaveButton.addEventListener("click", onSaveButtonClicked);

    propertiesEditor.initEvents(dialogElement);
    productDocumentsEditor.initEvents(dialogElement);
    productImagesEditor.initEvents(dialogElement);
}

export function clearCurrentChanges() {

    productImagesEditor.clearObjectUrls();
    editorChanges.clearChanges();
}

export function registerData(jsonDataObject) {

    dataStore.saveData(jsonDataObject);
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

    const singleEditorAddImageFileInput = document.getElementById(singleEditorAddImageFileInputId);

    singleEditorAddImageFileInput.click();
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

    const singleEditorAddDocumentFileInput = document.getElementById(singleEditorAddDocumentFileInputId);

    singleEditorAddDocumentFileInput.click();
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

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/single/productDataPopup/${productId}`;

    const dataStoreData = dataStore.loadData();

    const body = {
        productProperties: dataStoreData.productProperties,
        productLinks: dataStoreData.productLinks,
        productImages: dataStoreData.productImages
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

    const singleEditorProductDataPopupContainer = document.getElementById(singleEditorProductDataPopupContainerId);

    singleEditorProductDataPopupContainer.innerHTML = data;

    const dialog = singleEditorProductDataPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showProductLegacyDisplayPopupAsync() {

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/single/productLegacyDisplayPopup/${productId}`;

    const dataStoreData = dataStore.loadData();

    const body = {
        productProperties: dataStoreData.productProperties,
        productLinks: dataStoreData.productLinks,
        productImages: dataStoreData.productImages
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

    const singleEditorProductLegacyDisplayPopupContainer = document.getElementById(singleEditorProductLegacyDisplayPopupContainerId);

    singleEditorProductLegacyDisplayPopupContainer.innerHTML = data;

    const dialog = singleEditorProductLegacyDisplayPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function showPromotionProductFileEditorPopupAsync() {

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/single/promotionFile/showPopup/${productId}`;

    const dataStoreData = dataStore.loadData();

    const body = {
        productId: productId,
        promotionProductFiles: dataStoreData.promotionProductFiles,
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

    const singleEditorPromotionProductFileEditorPopupContainer = document.getElementById(singleEditorPromotionProductFileEditorPopupContainerId);

    singleEditorPromotionProductFileEditorPopupContainer.innerHTML = data;

    const dialog = singleEditorPromotionProductFileEditorPopupContainer.querySelector("dialog");

    promotionFilePopup.initEvents(
        dialog,
        onPromotionProductFileEditorItemAddedAsync,
        onPromotionProductFileEditorItemUpdatedAsync,
        onPromotionProductFileEditorItemDeletedAsync);

    dialog.showModal();
}

async function onPromotionProductFileEditorItemAddedAsync(promotionProductFileData) {

    if (!promotionProductFileData.shouldAddToImages) return;

    const imageCreateResult = await productImagesEditor.addNewPromotionProductFileToImagesAsync(
        promotionProductFileData.editorId,
        promotionProductFileData.selectedFileId);
}

async function onPromotionProductFileEditorItemUpdatedAsync(promotionProductFileData) {

    const editorId = promotionProductFileData.editorId;

    const dataStoreData = dataStore.loadData();

    const relatedImageData = dataStoreData.productImages.find(x => x.relatedPromotionFileEditorId == editorId);

    if (!promotionProductFileData.shouldAddToImages) {

        if (relatedImageData != null) {

            const relatedImageItem = document.getElementById(editorId);

            productImagesEditor.removeProductImage(relatedImageItem);
        }

        return;
    }

    if (relatedImageData == null) {

        const imageCreateResult = await productImagesEditor.addNewPromotionProductFileToImagesAsync(
            editorId,
            promotionProductFileData.selectedFileId);
    }
    else if (relatedImageData.relatedImageFile != null) {

        productImagesEditor.changeProductImageFileActive(editorId, promotionProductFileData.active);
    }
}

async function onPromotionProductFileEditorItemDeletedAsync(editorId) {

    const dataStoreData = dataStore.loadData();

    const relatedImageData = dataStoreData.productImages.find(x => x.relatedPromotionFileEditorId == editorId);

    if (relatedImageData == null) return;

    const relatedImageItem = document.getElementById(editorId);

    productImagesEditor.removeProductImage(relatedImageItem);
}

async function showProductSearchStringPopupAsync() {

    const singleEditorProductIdHiddenInput = document.getElementById(singleEditorProductIdHiddenInputId);

    const productId = singleEditorProductIdHiddenInput.value;

    const url = `api/components/productEditor/productSearchStringPopup/${productId}`;

    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) return;

    const data = await response.text();

    const singleEditorSearchStringPopupContainer = document.getElementById(singleEditorSearchStringPopupContainerId);

    singleEditorSearchStringPopupContainer.innerHTML = data;

    const dialog = singleEditorSearchStringPopupContainer.querySelector("dialog");

    dialog.showModal();
}

async function onSaveButtonClicked() {

    const dataStoreData = dataStore.loadData();

    const allProperties = dataStoreData.productProperties.concat(dataStoreData.productLinks);

    const activeProperties = [];

    for (const property of allProperties) {

        if (!property.active) continue;

        activeProperties.push(property);
    }

    const request = {
        productProperties: activeProperties,
        productImages: dataStoreData.productImages,
        promotionProductFiles: dataStoreData.promotionProductFiles,
        productDocuments: dataStoreData.productDocuments,
    };

    const url = `api/components/productEditor/single/save`;

    const response = await fetch(url, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            [antiforgeryUtils.antiforgeryHeaderName]: antiforgeryUtils.getAntiforgeryTokenValue()
        },
        body: JSON.stringify(request)
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect) return;

    if (!response.ok) {

        return;
    }

    const data = await response.json();
}
