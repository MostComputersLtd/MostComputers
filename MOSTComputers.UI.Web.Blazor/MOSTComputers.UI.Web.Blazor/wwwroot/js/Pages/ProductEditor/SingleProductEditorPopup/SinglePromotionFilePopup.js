import * as fetchWithAuthRedirect from "../../../Authentication/FetchWithAuthRedirect.js"
import * as promotionFileSelectList from "./PromotionFileSelectList.js";

const editorFileIdHiddenInputId = "singleProductPromotionFileEditorEditorFileIdHiddenInput";
const promotionFileIdHiddenInputId = "singleProductPromotionFileEditorPromotionFileIdHiddenInput";

const saveButtonId = "singleProductPromotionFileEditorSaveButton";
const activeInputId = "singleProductPromotionFileEditorActiveInput";
const validFromInputId = "singleProductPromotionFileEditorValidFromInput";
const validToInputId = "singleProductPromotionFileEditorValidToInput";
const shouldAddToImagesInputId = "singleProductPromotionFileEditorShouldAddToImagesInput";

const promotionFileValidOnInputId = "singlePromotionProductFileEditorPromotionFileValidOnInput";
const promotionFileSearchInputId = "singlePromotionProductFileEditorPromotionFileSearchInput"

const promotionFileSearchButtonId = "singlePromotionProductFileEditorSearchButton";

const promotionFileListContainerId = "singlePromotionProductFileEditorPromotionFileListContainer";

const shouldAddToImagesInputIsInImageAttribute = "data-is-in-image";

const imageExtensions = new Set([
    ".jpg", ".jpeg", ".png", ".gif", ".webp",
    ".bmp", ".svg", ".ico", ".avif", ".tif", ".tiff"
]);

function isImageFile(fileName) {
    const dotIndex = fileName.lastIndexOf(".");

    if (dotIndex <= 0 || dotIndex === fileName.length - 1) {
        return false;
    }

    return imageExtensions.has(fileName.slice(dotIndex).toLowerCase());
}

export function initEvents(saveButtonClickedCallback) {

    const saveButton = document.getElementById(saveButtonId);

    saveButton.addEventListener("click", saveButtonClickedCallback);

    const searchButton = document.getElementById(promotionFileSearchButtonId);

    searchButton.addEventListener("click", searchPromotionFilesFromCurrentData);
}

export async function initializePromotionFile() {

    await searchPromotionFilesAsync(null, null);

    const shouldAddToImagesInput = document.getElementById(shouldAddToImagesInputId);

    const isInImageAttribute = shouldAddToImagesInput.getAttribute(shouldAddToImagesInputIsInImageAttribute);

    if (isInImageAttribute === "true") {

        shouldAddToImagesInput.checked = true;
    }
    else {

        onPromotionFileListChanged(null);
    }

    const promotionFileIdHiddenInput = document.getElementById(promotionFileIdHiddenInputId);

    if (!promotionFileIdHiddenInput) return;

    const promotionFileId = parseInt(promotionFileIdHiddenInput.value);

    const allElementsInSelectList = promotionFileSelectList.getAllElementsData();

    for (const elementData of allElementsInSelectList) {

        if (elementData.id == promotionFileId) {

            selectElement(elementData.element);

            break;
        }
    } 
}

export function getPopupData() {

    const editorFileIdHiddenInput = document.getElementById(editorFileIdHiddenInputId);
    const activeInput = document.getElementById(activeInputId);
    const validFromInput = document.getElementById(validFromInputId);
    const validToInput = document.getElementById(validToInputId);
    const shouldAddToImagesInput = document.getElementById(shouldAddToImagesInputId);

    const promotionFileListContainer = document.getElementById(promotionFileListContainerId);

    const promotionFileList = promotionFileListContainer.querySelector("ul");

    const selectedPromotionFileData = promotionFileSelectList.getSelectedElementData(promotionFileList);

    return {
        editorId: editorFileIdHiddenInput.value,
        active: activeInput.checked,
        validFrom: validFromInput.value,
        validTo: validToInput.value,
        shouldAddToImages: shouldAddToImagesInput.checked,
        selectedFileId: selectedPromotionFileData.id,
        selectedFileName: selectedPromotionFileData.name,
        selectedFileFilename: selectedPromotionFileData.filename,
    };
}

export function changeSaveButtonText(text) {

    const saveButton = document.getElementById(saveButtonId);

    saveButton.textContent = text;
}

async function searchPromotionFilesFromCurrentData() {

    const validOnInput = document.getElementById(promotionFileValidOnInputId);
    const searchInput = document.getElementById(promotionFileSearchInputId);

    let validOnDate = validOnInput.value;

    if (!validOnDate) {

        validOnDate = null;
    }

    await searchPromotionFilesAsync(validOnDate, searchInput.value);

    onPromotionFileListChanged(null);
}

async function searchPromotionFilesAsync(validOnDate, searchText) {

    const body = {
        userInputString: searchText,
        validOnDate: validOnDate,
    };

    const url = "api/components/productEditor/single/promotionFile/search";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(body)
    });

    if (response == null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const data = await response.text();

    const promotionFileListContainer = document.getElementById(promotionFileListContainerId);

    promotionFileListContainer.innerHTML = data;

    const promotionFileList = promotionFileListContainer.querySelector("ul");

    promotionFileSelectList.initEvents(promotionFileList, onPromotionFileListChanged);
}

function onPromotionFileListChanged(promotionFileData) {

    const shouldAddToImagesInput = document.getElementById(shouldAddToImagesInputId);

    if (promotionFileData == null
        || !isImageFile(promotionFileData.filename)) {

        shouldAddToImagesInput.checked = false;
        shouldAddToImagesInput.setAttribute("disabled", "disabled");

        return;
    }

    shouldAddToImagesInput.checked = true;
    shouldAddToImagesInput.removeAttribute("disabled");
}
