const fullScreenButtonName = "popupFullScreenButton";
const normalScreenIconName = "normalScreenFullScreenButtonIcon";
const fullScreenIconName = "fullScreenFullScreenButtonIcon";

const popupNormalLeftMarginAttribute = "data-normal-left-margin";
const popupNormalRightMarginAttribute = "data-normal-right-margin";
const popupNormalTopMarginAttribute = "data-normal-top-margin";
const popupNormalBottomMarginAttribute = "data-normal-bottom-margin";

const isDialogFullScreenAttribute = "data-is-dialog-full-screen";

const displayNoneClass = "display-none";

export function attachFullScreenButtonEvent(dialog) {

    const fullScreenButton = dialog.querySelector(`[name='${fullScreenButtonName}']`);

    fullScreenButton.addEventListener("click", toggleFullScreenAndChangeFullScreenButton);
}

function toggleFullScreenAndChangeFullScreenButton(e) {

    const fullScreenButton = e.currentTarget;

    const dialog = fullScreenButton.closest("dialog");

    const normalScreenIcon = fullScreenButton.querySelector(`[name='${normalScreenIconName}']`);
    const fullScreenIcon = fullScreenButton.querySelector(`[name='${fullScreenIconName}']`);

    const htmlElement = document.querySelector("html");

    const hasScrollbar = htmlElement.style.overflowY != "hidden"
        && htmlElement.style.overflowY != "show"
        && htmlElement.scrollHeight > htmlElement.clientHeight;

    let verticalScrollBarWidthPixels = 0;

    if (hasScrollbar)
    {
        verticalScrollBarWidthPixels = 16;
    }

    const isDialogFullScreenAttributeValue = dialog.getAttribute(isDialogFullScreenAttribute);

    if (isDialogFullScreenAttributeValue == null || isDialogFullScreenAttributeValue !== "true") {

        goFullScreen(dialog, verticalScrollBarWidthPixels);

        dialog.setAttribute(isDialogFullScreenAttribute, "true");

        if (normalScreenIcon) {
            normalScreenIcon.classList.add(displayNoneClass);
        }

        if (fullScreenIcon) {
            fullScreenIcon.classList.remove(displayNoneClass);
        }

        return;
    }

    goBackToNormalScreen(dialog);

    dialog.setAttribute(isDialogFullScreenAttribute, "false");

    if (fullScreenIcon) {
        fullScreenIcon.classList.add(displayNoneClass);
    }

    if (normalScreenIcon) {
        normalScreenIcon.classList.remove(displayNoneClass);
    }
}

function goFullScreen(dialog, additionalRightMarginPixels) {

    dialog.setAttribute(popupNormalLeftMarginAttribute, dialog.style.marginLeft);
    dialog.setAttribute(popupNormalRightMarginAttribute, dialog.style.marginRight);
    dialog.setAttribute(popupNormalTopMarginAttribute, dialog.style.marginTop);
    dialog.setAttribute(popupNormalBottomMarginAttribute, dialog.style.marginBottom);
    dialog.setAttribute(isDialogFullScreenAttribute, true);

    dialog.style.marginLeft = "0vw";

    if (additionalRightMarginPixels > 0) {
        dialog.style.marginRight = `calc(0vw + ${additionalRightMarginPixels}px)`;
    }
    else {
        dialog.style.marginRight = "0vw";
    }

    dialog.style.marginTop = "0vh";
    dialog.style.marginBottom = "0vh";

    const popupContent = dialog.querySelector(".popup-content");

    if (additionalRightMarginPixels) {
        popupContent.style.width = `calc(100vw - ${additionalRightMarginPixels}px)`;
    }
    else {
        popupContent.style.width = "100vw";
    }

    popupContent.style.height = "100vh";
}

function goBackToNormalScreen(dialog) {

    dialog.style.marginLeft = dialog.getAttribute(popupNormalLeftMarginAttribute);
    dialog.style.marginRight = dialog.getAttribute(popupNormalRightMarginAttribute);
    dialog.style.marginTop = dialog.getAttribute(popupNormalTopMarginAttribute);
    dialog.style.marginBottom = dialog.getAttribute(popupNormalBottomMarginAttribute);

    const popupContent = dialog.querySelector(".popup-content");

    popupContent.style.width = `calc(100vw - ${dialog.style.marginLeft} - ${dialog.style.marginRight})`;
    popupContent.style.height = `calc(100vh - ${dialog.style.marginTop} - ${dialog.style.marginBottom})`;
}
