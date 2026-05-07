const productDataDialogCarouselId = "productDataCarousel";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", onStartOfPage);
} else {
    onStartOfPage();
}

document.addEventListener("visibilitychange", handleVisibilityChange)

function onStartOfPage() {
    startAutoSlide(true, productDataDialogCarouselId);
}

function handleVisibilityChange() {

    if (document.hidden) {
        stopAutoSlide(productDataDialogCarouselId);
    }
    else {
        startAutoSlide(true, productDataDialogCarouselId);
    }
}
