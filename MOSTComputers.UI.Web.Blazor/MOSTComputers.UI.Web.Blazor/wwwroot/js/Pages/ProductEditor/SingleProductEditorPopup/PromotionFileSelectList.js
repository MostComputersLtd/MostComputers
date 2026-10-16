const itemClass = "promotion-file-list-item";
const selectedItemClass = "selected-list-item";

const listItemPromotionFileIdAttribute = "data-promotion-file-id";
const listItemPromotionFileNameAttribute = "data-promotion-file-name";
const listItemPromotionFileFilenameAttribute = "data-promotion-file-file-name";


export function initEvents(promotionFileList, selectionChangedCallback) {

    const listItems = promotionFileList.querySelectorAll(`.${itemClass}`);

    promotionFileList.__selectionChangedCallback = selectionChangedCallback;

    for (const listItem of listItems) {

        listItem.addEventListener("click", changeSelection);
    }
}

export function getSelectedElementData(promotionFileList) {

    const selectedElement = promotionFileList.querySelector(`.${itemClass}.${selectedItemClass}`);

    return getElementData(selectedElement);
}

export function getAllElementsData(promotionFileList) {

    const listItems = promotionFileList.querySelectorAll(`.${itemClass}`);

    const data = [];

    for (const listItem of listItems) {

        const elementData = getElementData(listItem);

        data.push(elementData);
    }

    return data;
}

function changeSelection(e) {

    const elementToSelect = e.currentTarget;

    selectElement(elementToSelect);
}

export function selectElement(elementToSelect) {

    const promotionFileList = elementToSelect.parentElement;

    const selectedElements = promotionFileList.querySelectorAll(`.${itemClass}.${selectedItemClass}`); 

    for (const selectedElement of selectedElements) {

        selectedElement.classList.remove(selectedItemClass);
    }

    let elementData = null;

    if (elementToSelect.classList.contains(selectedItemClass)) {

        elementToSelect.classList.remove(selectedItemClass);
    }
    else {
        elementToSelect.classList.add(selectedItemClass);

        elementData = getElementData(elementToSelect);
    }

    if (promotionFileList.__selectionChangedCallback != null) {

        promotionFileList.__selectionChangedCallback(elementData);
    }
}

function getElementData(promotionFileListItem) {

    const promotionFileId = promotionFileListItem.getAttribute(listItemPromotionFileIdAttribute);
    const promotionFileName = promotionFileListItem.getAttribute(listItemPromotionFileNameAttribute);
    const promotionFileFilename = promotionFileListItem.getAttribute(listItemPromotionFileFilenameAttribute);

    return {
        id: promotionFileId,
        name: promotionFileName,
        filename: promotionFileFilename,
        element: promotionFileListItem,
    };
}
