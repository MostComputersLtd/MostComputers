export function openDialog(dialogId, asModal = true) {
    const dialog = document.getElementById(dialogId);

    if (dialog == null) return;

    if (asModal) {

        dialog.showModal();

        return;
    }

    dialog.show();
}

export function closeDialog(dialogId) {
    const dialog = document.getElementById(dialogId);

    if (dialog == null) return;

    dialog.close();
}

async function overrideDefaultCloseEvent(e) {
    e.preventDefault();

    console.log("Prevented default");

    await e.target._customDotNetRef.invokeMethodAsync("OnDefaultCloseAsync");
}

export async function attachAutoCloseEvent(dialogId, dotNetRef) {
    const dialog = document.getElementById(dialogId);

    if (dialog == null) return;

    dialog._customDotNetRef = dotNetRef;

    dialog.addEventListener("cancel", overrideDefaultCloseEvent);
}

export async function detachAutoCloseEvent(dialogId) {
    const dialog = document.getElementById(dialogId);

    if (dialog == null) return;

    dialog.removeEventListener("cancel", overrideDefaultCloseEvent);

    dialog._customDotNetRef = null;
}