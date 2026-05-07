const promotionGroupsListId = "promotionGroupsList";
const promotionGroupItemName = "promotionGroupItem";

const promotionImagesContainerId = "promotionImagesContainer";

const imageDataSourceAttribute = "data-src";
const imagePromotionGroupIdAttribute = "data-promotion-group-id";

const imageNotDisplayedClass = "not-displayed";
const promotionGroupItemSelectedClass = "selected";

async function selectGroupAndShowData(promotionGroupId, promotionGroupElementId, alwaysActivePromotionGroupId = null) {

    const promotionImagesComponentContainer = document.getElementById(promotionImagesContainerId);

    const promotionImagesContainer = promotionImagesComponentContainer.children[0];

    const imageContainerElements = [...promotionImagesContainer.getElementsByTagName("div")];

    for (let i = imageContainerElements.length - 1; i >= 0; i--) {

        const imageContainerElement = imageContainerElements[i];

        const imageElement = imageContainerElement.children[0];

        const imagePromotionGroupId = imageElement.getAttribute(imagePromotionGroupIdAttribute);

        const shouldImageBeVisible = parseInt(imagePromotionGroupId) === promotionGroupId;

        if (!shouldImageBeVisible) continue;

        const isHidden = imageContainerElement.classList.contains(imageNotDisplayedClass);

        if (isHidden) {

            const imageDataSource = imageElement.getAttribute(imageDataSourceAttribute);

            if (imageDataSource != null) {

                imageElement.src = imageDataSource;

                imageElement.removeAttribute(imageDataSourceAttribute);
            }

            imageContainerElement.classList.remove(imageNotDisplayedClass);
        }

        if (imageContainerElement !== promotionImagesContainer.firstElementChild)
        {
            promotionImagesContainer.insertBefore(imageContainerElement, promotionImagesContainer.firstElementChild);
        }

        imageContainerElements.splice(i, 1);
    }

    for (const imageContainerElement of imageContainerElements) {

        const imageElement = imageContainerElement.children[0];
        
        const imagePromotionGroupId = imageElement.getAttribute(imagePromotionGroupIdAttribute);

        const shouldImageBeVisible = parseInt(imagePromotionGroupId) === alwaysActivePromotionGroupId
            && alwaysActivePromotionGroupId != null;

        const isHidden = imageContainerElement.classList.contains(imageNotDisplayedClass);

        if (shouldImageBeVisible) {
            const imageDataSource = imageElement.getAttribute(imageDataSourceAttribute);

            if (imageDataSource != null) {
                imageElement.src = imageDataSource;

                imageElement.removeAttribute(imageDataSourceAttribute);
            }

            imageContainerElement.classList.remove(imageNotDisplayedClass);
        }
        else if (!shouldImageBeVisible && !isHidden) {

            imageContainerElement.classList.add(imageNotDisplayedClass);
        }
    }

    promotionImagesContainer.scrollTop = 0;

    const promotionGroupElement = document.getElementById(promotionGroupElementId);

    const promotionGroupsList = document.getElementById(promotionGroupsListId);

    const promotionGroupElements = promotionGroupsList.querySelectorAll("[name='" + promotionGroupItemName + "']");

    for (const element of promotionGroupElements) {

        if (element !== promotionGroupElement) {

            element.classList.remove(promotionGroupItemSelectedClass);

            continue;
        }

        element.classList.add(promotionGroupItemSelectedClass);
    }
}