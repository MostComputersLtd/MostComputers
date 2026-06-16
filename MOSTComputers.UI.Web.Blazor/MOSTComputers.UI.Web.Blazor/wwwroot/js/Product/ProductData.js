const copyLinkButtonId = "copyLinkButton";
const copyLinkButtonTextId = "copyLinkButtonText";
const copyLinkButtonCheckmarkId = "copyLinkButtonCheckmark";

const copyLinkButtonCheckedClass = "link-copied";

const htmlContentInCarouselItemClass = "carousel-html-content-item";
const htmlContentInCarouselContainerClass = "carousel-html-content-container";

const htmlContentInCarouselIndicatorItemClass = "carousel-indicator-html-content-item";
const htmlContentInCarouselIndicatorContainerClass = "carousel-indicator-html-content-container";

async function resizeHtmlContentInCarousel(carousel) {

    const htmlItemsInCarouselViewport = [...carousel.getElementsByClassName(htmlContentInCarouselItemClass)];

    if (htmlItemsInCarouselViewport.length === 0) return;

    const htmlItemsInCarouselIndicators = [...carousel.getElementsByClassName(htmlContentInCarouselIndicatorItemClass)];

    for (const htmlItem of htmlItemsInCarouselViewport) {

        const htmlContainer = htmlItem.querySelector(`.${htmlContentInCarouselContainerClass}`)

        await waitForAllImagesInContainerToLoad(htmlContainer);

        const scale = Math.min(
            htmlItem.clientWidth / htmlContainer.scrollWidth,
            htmlItem.clientHeight / htmlContainer.scrollHeight,
            1
        );

        htmlContainer.style.transform = `scale(${scale})`;
    }

    for (const htmlItem of htmlItemsInCarouselIndicators) {

        const htmlContainer = htmlItem.querySelector(`.${htmlContentInCarouselIndicatorContainerClass}`)

        await waitForAllImagesInContainerToLoad(htmlContainer);

        const scale = Math.min(
            htmlItem.clientWidth / htmlContainer.scrollWidth,
            htmlItem.clientHeight / htmlContainer.scrollHeight,
            1
        );

        htmlItem.style.width = "inherit";
        htmlContainer.style.transform = `scale(${scale})`;
    }
}

async function waitForAllImagesInContainerToLoad(container) {

    const images = [...container.querySelectorAll('img')];

    if (images.length === 0) return;

    await Promise.all(

        images.map(img => {

            if (img.complete) return Promise.resolve();

            return new Promise(resolve =>
                img.addEventListener('load', resolve, { once: true }));
        })
    );
}

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