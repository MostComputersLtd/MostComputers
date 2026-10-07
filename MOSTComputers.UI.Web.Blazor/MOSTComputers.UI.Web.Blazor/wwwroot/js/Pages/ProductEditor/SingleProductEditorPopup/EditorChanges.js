const changedProperties = new Map();
const changedImages = new Map();
const changedPromotionFiles = new Map();
const changedDocuments = new Map();

export const ItemChangeState = {
    Added: 0,
    Updated: 1,
    Deleted: 2
};

export function setPropertyChanged(propertyData, itemChangeState)
{
    setDataChanged(changedProperties, propertyData, itemChangeState);
}

export function setImageChanged(imageData, itemChangeState)
{
    setDataChanged(changedImages, imageData, itemChangeState);
}

export function setPromotionProductFileChanged(promotionProductFileData, itemChangeState)
{
    setDataChanged(changedPromotionFiles, promotionProductFileData, itemChangeState);
}

export function setDocumentChanged(productDocumentData, itemChangeState)
{
    setDataChanged(changedDocuments, productDocumentData, itemChangeState);
}

function setDataChanged(changedItems, changedItem, itemChangeState)
{
    const currentItemChangeState = changedItems.get(changedItem);

    if (currentItemChangeState !== undefined)
    {
        if (itemChangeState == ItemChangeState.Added && currentItemChangeState == ItemChangeState.Deleted)
        {
            changedItems.delete(changedItem);
        }
        else if (itemChangeState == ItemChangeState.Deleted && currentItemChangeState == ItemChangeState.Added)
        {
            changedItems.delete(changedItem);
        }
        else if (itemChangeState == ItemChangeState.Updated && currentItemChangeState == ItemChangeState.Added)
        {
            return;
        }
        else
        {
            changedItems[changedItem] = itemChangeState;
        }

        return;
    }

    changedItems.add(changedItem, itemChangeState);
}

export function markDataSaved(propertyData)
{
    changedProperties.delete(propertyData);
}

export function markDataSaved(imageData)
{
    changedImages.delete(imageData);
}

export function markDataSaved(promotionProductFileData)
{
    changedPromotionFiles.delete(promotionProductFileData);
}

export function markDataSaved(productDocumentData)
{
    changedDocuments.delete(productDocumentData);
}
