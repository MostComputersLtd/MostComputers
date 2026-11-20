import * as urlUtils from "../Common/UrlUtils.js";
import * as carouselModule from "../Primitives/Carousel.js";

const copyLinkButtonId = "copyLinkButton";
const copyLinkButtonTextId = "copyLinkButtonText";
const copyLinkButtonCheckmarkId = "copyLinkButtonCheckmark";

const carouselIndicatorName = "carouselIndicator";

const copyLinkButtonProductIdAttribute = "data-product-id";

const carouselIndicatorImageOrHtmlContentUrlAttribute = "data-image-or-html-content-url";

const copyLinkButtonCheckedClass = "link-copied";

const htmlContentInCarouselItemClass = "carousel-html-content-item";
const htmlContentInCarouselContainerClass = "carousel-html-content-container";

const htmlContentInCarouselIndicatorItemClass = "carousel-indicator-html-content-item";
const htmlContentInCarouselIndicatorContainerClass = "carousel-indicator-html-content-container";

export async function initEventsAndCarousel(carousel) {

    const copyLinkButton = document.getElementById(copyLinkButtonId);

    copyLinkButton.addEventListener("click", copyLinkToClipboardAndAnimateCopyButton)

    if (carousel) {

        const carouselIndicators = carousel.querySelectorAll(`[name='${carouselIndicatorName}']`);

        for (const carouselIndicator of carouselIndicators) {

            carouselIndicator.addEventListener("dblclick", openCarouselIndicatorDataUrlInNewWindow);
        }

        carouselModule.initNavigationEvents(carousel);

        await carouselModule.waitForAllImagesInContainerToDecode(carousel);

        await resizeHtmlContentInCarousel(carousel);

        await carouselModule.initAutoSlideEventsAsync(carousel, true, true);
    } 
}

export function onDocumentVisibilityChange(carousel) {

    if (!carousel) return;

    if (document.hidden) {

        carouselModule.stopAutoSlide(carousel);
    }
    else {
        carouselModule.startAutoSlide(carousel);
    }
}

function openCarouselIndicatorDataUrlInNewWindow(e) {

    const url = e.currentTarget.getAttribute(carouselIndicatorImageOrHtmlContentUrlAttribute);

    urlUtils.openDataUrlInNewWindow(url);
}

export async function resizeHtmlContentInCarousel(carousel) {

    const htmlItemsInCarouselViewport = [...carousel.getElementsByClassName(htmlContentInCarouselItemClass)];

    if (htmlItemsInCarouselViewport.length === 0) return;

    const htmlItemsInCarouselIndicators = [...carousel.getElementsByClassName(htmlContentInCarouselIndicatorItemClass)];

    for (const htmlItem of htmlItemsInCarouselViewport) {

        const htmlContainer = htmlItem.querySelector(`.${htmlContentInCarouselContainerClass}`)

        await carouselModule.waitForAllImagesInContainerToDecode(htmlContainer);

        const scale = Math.min(
            htmlItem.clientWidth / htmlContainer.scrollWidth,
            htmlItem.clientHeight / htmlContainer.scrollHeight,
            1
        );

        htmlContainer.style.transform = `scale(${scale})`;
    }

    for (const htmlItem of htmlItemsInCarouselIndicators) {

        const htmlContainer = htmlItem.querySelector(`.${htmlContentInCarouselIndicatorContainerClass}`)

        await carouselModule.waitForAllImagesInContainerToDecode(htmlContainer);

        const scale = Math.min(
            htmlItem.clientWidth / htmlContainer.scrollWidth,
            htmlItem.clientHeight / htmlContainer.scrollHeight,
            1
        );

        htmlItem.style.width = "inherit";
        htmlContainer.style.transform = `scale(${scale})`;
    }
}

async function copyLinkToClipboardAndAnimateCopyButton(e) {

    const productId = e.currentTarget.getAttribute(copyLinkButtonProductIdAttribute);

    const success = copyLinkToClipboard(`/product/productData/${productId}`);

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
