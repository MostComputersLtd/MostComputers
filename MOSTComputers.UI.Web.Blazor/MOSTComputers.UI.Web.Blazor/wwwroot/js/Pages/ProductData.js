import * as carouselModule from "../Primitives/Carousel.js";
import * as productData from "../Product/ProductData.js";

const productDataDialogCarouselId = "productDataCarousel";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onStartOfPage);
} else {
    await onStartOfPage();
}

document.addEventListener("visibilitychange", handleVisibilityChange);
window.addEventListener("resize", handleResize);

async function onStartOfPage() {

    const productDataDialogCarousel = document.querySelector(`[name='${carouselModule.carouselName}']`);

    await productData.initEventsAndCarousel(productDataDialogCarousel);
}

function handleVisibilityChange() {

    const productDataDialogCarousel = document.querySelector(`[name='${carouselModule.carouselName}']`);

    if (!productDataDialogCarousel) return;

    productData.onDocumentVisibilityChange(productDataDialogCarousel);
}

async function handleResize() {

    const productDataDialogCarousel = document.querySelector(`[name='${carouselModule.carouselName}']`);

    if (!productDataDialogCarousel) return;

    await productData.resizeHtmlContentInCarousel(productDataDialogCarousel);
}
