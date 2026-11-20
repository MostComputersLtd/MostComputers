const closeButtonName = "popupCloseButton";

export function attachCloseEvents(dialog, closeHandler = null, cancelEventNotCancellableHandler = null) {

    dialog._closeHandler = closeHandler;
    dialog._cancelEventNotCancellableHandler = cancelEventNotCancellableHandler;

    dialog.addEventListener("cancel", onDialogCancel);

    attachCloseButtonEvent(dialog);
}

function attachCloseButtonEvent(dialog) {

    const closeButton = dialog.querySelector(`[name='${closeButtonName}']`)

    if (closeButton) {

        closeButton.addEventListener("click", onCloseButtonClick);
    }
}

export function attachStandaloneCloseButtonEvent(dialog) {

    const closeButton = dialog.querySelector(`[name='${closeButtonName}']`)

    if (closeButton) {

        closeButton.addEventListener("click", onStandaloneCloseButtonClick);
    }
}

function onDialogCancel(e) {

    const dialog = e.currentTarget;

    if (e.cancelable) {

        if (dialog._closeHandler) {

            dialog._closeHandler(e);
        }
    }
    else if (dialog._cancelEventNotCancellableHandler) {

        dialog._cancelEventNotCancellableHandler(e);
    }
}

function onCloseButtonClick(e) {

    const dialog = e.currentTarget.closest("dialog");

    if (typeof dialog.requestClose === "function") {

        dialog.requestClose();
        return;
    }

    const cancelEvent = new Event("cancel", {
        cancelable: true
    });

    const wasNotCancelled = dialog.dispatchEvent(cancelEvent);

    if (wasNotCancelled) {
        dialog.close();
    }
}

function onStandaloneCloseButtonClick(e) {

    const dialog = e.currentTarget.closest("dialog");

    dialog.close();
}
