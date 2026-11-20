import * as groupPromotionsDisplay from "../Promotions/GroupPromotionsDisplay.js";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initEventsAndVisuals);
}
else {
    initEventsAndVisuals();
}

window.addEventListener("pageshow", groupPromotionsDisplay.scrollToUserFocusedElement);

function initEventsAndVisuals() {

    groupPromotionsDisplay.initEvents();
    groupPromotionsDisplay.scrollToUserFocusedElement();
}
