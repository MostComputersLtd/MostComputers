const copyLinkButtonId = "copyLinkButton";
const copyLinkButtonTextId = "copyLinkButtonText";
const copyLinkButtonCheckmarkId = "copyLinkButtonCheckmark";

const copyLinkButtonCheckedClass = "link-copied";

async function copyLinkToClipboardAndAnimateCopyButton(uriRelativeToRoot) {
    const success = copyLinkToClipboard(uriRelativeToRoot);

    if (!success) return;

    const copyLinkButtonText = document.getElementById(copyLinkButtonTextId);
    const copyLinkButtonCheckmark = document.getElementById(copyLinkButtonCheckmarkId);

    if (!copyLinkButtonText || !copyLinkButtonCheckmark) return;

    copyLinkButtonText.classList.add(copyLinkButtonCheckedClass);
    copyLinkButtonCheckmark.classList.add(copyLinkButtonCheckedClass);

    setTimeout(() => removeCopyLinkButtonAnimation(copyLinkButtonText, copyLinkButtonCheckmark), 1000);
}

function removeCopyLinkButtonAnimation(copyLinkButtonText, copyLinkButtonCheckmark) {

    copyLinkButtonText.classList.remove(copyLinkButtonCheckedClass);
    copyLinkButtonCheckmark.classList.remove(copyLinkButtonCheckedClass);
}

async function copyLinkToClipboard(uriRelativeToRoot) {
    const url = new URL(uriRelativeToRoot, window.location.origin).href;

    try {
        await navigator.clipboard.writeText(url);

        return true;
    }
    catch (error) {
        return false;
    }
}