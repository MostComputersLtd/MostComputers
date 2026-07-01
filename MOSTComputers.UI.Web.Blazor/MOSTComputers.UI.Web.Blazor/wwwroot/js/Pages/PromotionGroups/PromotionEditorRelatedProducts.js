import * as common from "./Common.js";
import * as relatedProducts from "./RelatedProducts.js";

export function attachEvents(productRelatedElement) {

    const promotionEditRelatedProductRemoveButton = productRelatedElement.querySelector(`[name='${common.relatedProductRemoveButtonName}']`);

    const productIdAttribute = productRelatedElement.getAttribute(common.promotionRelatedProductIdAttribute);

    const productId = common.getIntegerOrNullFromString(productIdAttribute);

    promotionEditRelatedProductRemoveButton.addEventListener("click",
        () => removeItemAndId(productRelatedElement, productId));
}

function removeItemAndId(productRelatedElement, productId) {

    removeItem(productRelatedElement);

    relatedProducts.removeFromExistingProductIds(productId);
}

export function removeItem(promotionEditRelatedProductElement) {

    promotionEditRelatedProductElement.remove();
}
