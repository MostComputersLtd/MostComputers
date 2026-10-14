import * as conversionUtils from "../../Common/ConversionUtils.js"
import * as validationMessages from "../../Validation/ValidationMessages.js"
import * as validationVisualElements from "../../Validation/ValidationVisualElements.js"

const singlePromotionFileEditorHiddenIdInputId = "singlePromotionFileEditorHiddenIdInput";

const singlePromotionFileEditorSaveButtonId = "singlePromotionFileEditorSaveButton";

const singlePromotionFileEditorNameInputId = "singlePromotionFileEditorNameInput";
const singlePromotionFileEditorActiveInputId = "singlePromotionFileEditorActiveInput";
const singlePromotionFileEditorValidFromInputId = "singlePromotionFileEditorValidFromInput";
const singlePromotionFileEditorValidToInputId = "singlePromotionFileEditorValidToInput";
const singlePromotionFileEditorDescriptionInputId = "singlePromotionFileEditorDescriptionInput";
const singlePromotionFileEditorRequiredProductsInputId = "singlePromotionFileEditorRequiredProductsInput";

const singlePromotionFileEditorNameInputValidationMessagesId = "singlePromotionFileEditorNameInputValidationMessages";
const singlePromotionFileEditorValidToInputValidationMessagesId = "singlePromotionFileEditorValidToInputValidationMessages";
const singlePromotionFileEditorDescriptionInputValidationMessagesId = "singlePromotionFileEditorDescriptionInputValidationMessages";
const singlePromotionFileEditorRequiredProductsInputValidationMessagesId = "singlePromotionFileEditorRequiredProductsInputValidationMessages";

const singlePromotionFileEditorImageDropZoneId = "singlePromotionFileEditorImageDropZone";

const singlePromotionFileNewFileNameContainerId = "singlePromotionFileNewFileNameContainer";
const singlePromotionFileNewFileNameInputId = "singlePromotionFileNewFileNameInput";
const singlePromotionFileNewFileNameExtensionTextId = "singlePromotionFileNewFileNameExtensionText";
const singlePromotionFileNewFileNameInputValidationMessagesId = "singlePromotionFileNewFileNameInputValidationMessages";

const singlePromotionFileEditorFileButtonId = "singlePromotionFileEditorFileButton";

const singlePromotionFileEditorSelectedFileTextContainerId = "singlePromotionFileEditorSelectedFileTextContainer";
const singlePromotionFileEditorSelectedFileTextId = "singlePromotionFileEditorSelectedFileText";

const singlePromotionFileEditorSelectedFileDisplayContainerId = "singlePromotionFileEditorSelectedFileDisplayContainer";

const singlePromotionFileEditorFileInputId = "singlePromotionFileEditorFileInput";

const hiddenUntilIsVisibleClass = "element-show";

export const idFormFileName = "id";
const nameFormFileName = "name";
const activeFormFileName = "active";
const validFromFormFileName = "validFrom";
const validToFormFileName = "validTo";
const descriptionFormFileName = "description";
const relatedProductsStringFormFileName = "relatedProducts";
const preferredFileNameFormFileName = "preferredFileName";
const selectedFileFormFileName = "selectedFile";

const propertyNamesToValidationMessagesContainerIdsMap = new Map([
    ["Name", singlePromotionFileEditorNameInputValidationMessagesId],
    ["ValidTo", singlePromotionFileEditorValidToInputValidationMessagesId],
    ["Description", singlePromotionFileEditorDescriptionInputValidationMessagesId],
    ["RelatedProductsString", singlePromotionFileEditorRequiredProductsInputValidationMessagesId],
    ["NewFileData.FileName", singlePromotionFileNewFileNameInputValidationMessagesId],
]);

let selectedFile = null;
let selectedFileObjectUrl = null;

export function initEvents(dialogElement, saveButtonCallback = null) {

    const singlePromotionFileEditorImageDropZone = document.getElementById(singlePromotionFileEditorImageDropZoneId);

    singlePromotionFileEditorImageDropZone.addEventListener("dragenter", registerPromotionFileDropZone);
    singlePromotionFileEditorImageDropZone.addEventListener("dragover", onPromotionFileDropZoneDragOver);

    const singlePromotionFileEditorFileButton = document.getElementById(singlePromotionFileEditorFileButtonId);

    singlePromotionFileEditorFileButton.addEventListener("click", onFileButtonClicked);

    const singlePromotionFileEditorFileInput = document.getElementById(singlePromotionFileEditorFileInputId);

    singlePromotionFileEditorFileInput.addEventListener("change", onFileInputChangeAsync);

    if (saveButtonCallback != null) {

        const singlePromotionFileEditorSaveButton = document.getElementById(singlePromotionFileEditorSaveButtonId);

        singlePromotionFileEditorSaveButton.addEventListener("click", saveButtonCallback);
    }

    dialogElement.addEventListener("close", onClose);
}

function onClose() {

    if (selectedFileObjectUrl != null) {

        URL.revokeObjectURL(selectedFileObjectUrl); 
    }

    selectedFileObjectUrl = null;
    selectedFile = null;
}

export function getCurrentData() {

    const singlePromotionFileEditorHiddenIdInput = document.getElementById(singlePromotionFileEditorHiddenIdInputId);

    const singlePromotionFileEditorNameInput = document.getElementById(singlePromotionFileEditorNameInputId);
    const singlePromotionFileEditorActiveInput = document.getElementById(singlePromotionFileEditorActiveInputId);
    const singlePromotionFileEditorValidFromInput = document.getElementById(singlePromotionFileEditorValidFromInputId);
    const singlePromotionFileEditorValidToInput = document.getElementById(singlePromotionFileEditorValidToInputId);
    const singlePromotionFileEditorDescriptionInput = document.getElementById(singlePromotionFileEditorDescriptionInputId);
    const singlePromotionFileEditorRequiredProductsInput = document.getElementById(singlePromotionFileEditorRequiredProductsInputId);
    const singlePromotionFileNewFileNameInput = document.getElementById(singlePromotionFileNewFileNameInputId);

    let id = null;

    if (singlePromotionFileEditorHiddenIdInput) {

        id = conversionUtils.getNumberOrNullFromString(singlePromotionFileEditorHiddenIdInput.value);
    }

    const name = singlePromotionFileEditorNameInput.value;
    const active = singlePromotionFileEditorActiveInput.checked;
    const validFrom = singlePromotionFileEditorValidFromInput.value;
    const validTo = singlePromotionFileEditorValidToInput.value;
    const description = singlePromotionFileEditorDescriptionInput.value;
    const relatedProductsString = singlePromotionFileEditorRequiredProductsInput.value;
    const preferredFileName = singlePromotionFileNewFileNameInput.value;

    const data = new FormData();

    if (id !== null) {
        data.append(idFormFileName, id);
    }

    data.append(nameFormFileName, name);
    data.append(activeFormFileName, active);
    data.append(validFromFormFileName, validFrom);
    data.append(validToFormFileName, validTo);
    data.append(descriptionFormFileName, description);
    data.append(relatedProductsStringFormFileName, relatedProductsString);
    data.append(preferredFileNameFormFileName, preferredFileName);
    data.append(selectedFileFormFileName, selectedFile);

    return data;
}

export function changeSaveButtonText(text) {

    const singlePromotionFileEditorSaveButton = document.getElementById(singlePromotionFileEditorSaveButtonId);

    singlePromotionFileEditorSaveButton.textContent = text;
}

export function clearValidationMessages() {

    validationVisualElements.clearValidationMessages(
        propertyNamesToValidationMessagesContainerIdsMap);
}

export function displayValidationMessages(validationProblem) {

    validationVisualElements.displayValidationMessages(
        validationProblem,
        propertyNamesToValidationMessagesContainerIdsMap,
        validationMessages.getDefaultValidationMessage)
}

function registerPromotionFileDropZone() {

    const dropzoneElement = e.currentTarget;

    dropzoneElement.addEventListener("drop", onDropChangeInputFilesAndTriggerChangeEvent, { once: true });
}

function onDropChangeInputFilesAndTriggerChangeEvent(dropEvent) {

    const files = getFirstFileFromDataTransferAsFileList(dropEvent.dataTransfer);

    if (files.length <= 0) return;

    const fileInputElement = document.getElementById(singlePromotionFileEditorFileInputId);

    fileInputElement.files = files;

    fileInputElement.dispatchEvent(new Event("change"));
}

function getFirstFileFromDataTransferAsFileList(dataTransfer) {

    const files = getFilesFromDataTransfer(dataTransfer);

    const outputDataTransfer = new DataTransfer();

    if (files.length > 0) {

        const file = files[0];

        outputDataTransfer.items.add(file);
    }

    return outputDataTransfer.files;
}

function getFilesFromDataTransfer(dataTransfer) {

    const files = [];

    if (dataTransfer.items) {

        for (const item of dataTransfer.items) {

            if (item.kind !== "file") continue;

            const file = item.getAsFile();

            files.push(file);
        }

        return files;
    }

    for (const file of dataTransfer.files) {

        files.push(file);
    }

    return files;
}

function onPromotionFileDropZoneDragOver(e) {
    
    e,preventDefault();
}

function onFileButtonClicked() {

    const singlePromotionFileEditorFileInput = document.getElementById(singlePromotionFileEditorFileInputId);

    singlePromotionFileEditorFileInput.click();
}

async function onFileInputChangeAsync(e) {

    const singleEditorAddImageFileInput = e.currentTarget;

    if (singleEditorAddImageFileInput.files.length === 0) {

        return;
    }

    const file = singleEditorAddImageFileInput.files.item(0);

    if (file == null || file.Size == 0) {
        return;
    }

    const singlePromotionFileNewFileNameInput = document.getElementById(singlePromotionFileNewFileNameInputId);

    const fileExtensionIndex = file.name.lastIndexOf('.');

    const fileNameExtension = file.name.substring(fileExtensionIndex);

    let shouldReplaceNewFileNameInput = !singlePromotionFileNewFileNameInput.value;

    if (!shouldReplaceNewFileNameInput && selectedFile && selectedFile.name) {

        const selectedFileExtensionIndex = selectedFile.name.lastIndexOf('.');
        const selectedFileNameWithoutExtension = selectedFile.name.substring(0, selectedFileExtensionIndex);

        const singlePromotionFileEditorHiddenIdInput = document.getElementById(singlePromotionFileEditorHiddenIdInputId);

        if (!singlePromotionFileEditorHiddenIdInput) {

            shouldReplaceNewFileNameInput = singlePromotionFileNewFileNameInput.value === selectedFileNameWithoutExtension;
        }
    }

    if (shouldReplaceNewFileNameInput) {

        const fileNameWithoutExtension = file.name.substring(0, fileExtensionIndex);

        singlePromotionFileNewFileNameInput.value = fileNameWithoutExtension;
    }

    if (selectedFileObjectUrl) {

        URL.revokeObjectURL(selectedFileObjectUrl);
    }

    selectedFile = file;
    selectedFileObjectUrl = URL.createObjectURL(file);

    const singlePromotionFileNewFileNameContainer = document.getElementById(singlePromotionFileNewFileNameContainerId);
    const singlePromotionFileNewFileNameExtensionText = document.getElementById(singlePromotionFileNewFileNameExtensionTextId);
    const singlePromotionFileEditorSelectedFileTextContainer = document.getElementById(singlePromotionFileEditorSelectedFileTextContainerId);
    const singlePromotionFileEditorSelectedFileText = document.getElementById(singlePromotionFileEditorSelectedFileTextId);
    const singlePromotionFileEditorSelectedFileDisplayContainer = document.getElementById(singlePromotionFileEditorSelectedFileDisplayContainerId);

    singlePromotionFileNewFileNameContainer.classList.add(hiddenUntilIsVisibleClass);
    singlePromotionFileNewFileNameExtensionText.textContent = fileNameExtension;

    singlePromotionFileEditorSelectedFileTextContainer.classList.add(hiddenUntilIsVisibleClass);
    singlePromotionFileEditorSelectedFileText.textContent = file.name;

    const fileComponentBody = {
        fileUrl: selectedFileObjectUrl,
        contentType: file.type
    };

    const fileComponentResponse = await fetch("api/components/promotionFileEditor/promotionFile", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "text/html"
        },
        body: JSON.stringify(fileComponentBody)
    });

    if (fileComponentResponse.ok) {

        singlePromotionFileEditorSelectedFileDisplayContainer.classList.add(hiddenUntilIsVisibleClass);

        singlePromotionFileEditorSelectedFileDisplayContainer.innerHTML = await fileComponentResponse.text();
    }
    else {
    }
}
