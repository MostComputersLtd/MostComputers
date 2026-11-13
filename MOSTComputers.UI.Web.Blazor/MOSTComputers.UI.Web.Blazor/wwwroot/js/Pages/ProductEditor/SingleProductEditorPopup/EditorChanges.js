const changedProperties = new Map();
const changedImages = new Map();
const changedPromotionFiles = new Map();
const changedDocuments = new Map();

let onEditorChangeOrMarkCallback = null;

export const ItemChangeState = {
    Added: 0,
    Updated: 1,
    Deleted: 2
};

export function setCallback(callback) {

    onEditorChangeOrMarkCallback = callback;
}

export function removeCallback() {

    onEditorChangeOrMarkCallback = null;
}

export function setPropertyChanged(propertyData, itemChangeState) {

    setDataChanged(changedProperties, propertyData, itemChangeState);

    runCallback();
}

export function setImageChanged(imageData, itemChangeState) {

    setDataChanged(changedImages, imageData, itemChangeState);

    runCallback();
}

export function setPromotionProductFileChanged(promotionProductFileData, itemChangeState) {

    setDataChanged(changedPromotionFiles, promotionProductFileData, itemChangeState);

    runCallback();
}

export function setDocumentChanged(productDocumentData, itemChangeState) {

    setDataChanged(changedDocuments, productDocumentData, itemChangeState);

    runCallback();
}

function setDataChanged(changedItems, changedItem, itemChangeState) {

    const currentItemChangeState = changedItems.get(changedItem);

    if (currentItemChangeState !== undefined) {

        if (itemChangeState == ItemChangeState.Added && currentItemChangeState == ItemChangeState.Deleted) {

            changedItems.delete(changedItem);
        }
        else if (itemChangeState == ItemChangeState.Deleted && currentItemChangeState == ItemChangeState.Added) {

            changedItems.delete(changedItem);
        }
        else if (itemChangeState == ItemChangeState.Updated && currentItemChangeState == ItemChangeState.Added) {

            return;
        }
        else {
            changedItems.set(changedItem, itemChangeState);
        }

        return;
    }

    changedItems.set(changedItem, itemChangeState);
}

export function anyChanges() {

    return changedProperties.size > 0
        || changedImages.size > 0
        || changedPromotionFiles.size > 0
        || changedDocuments.size > 0;
}

export function clearChanges() {

    changedProperties.clear();
    changedImages.clear();
    changedPromotionFiles.clear();
    changedDocuments.clear();
}

function runCallback() {

    if (onEditorChangeOrMarkCallback == null) return;

    onEditorChangeOrMarkCallback();
}
