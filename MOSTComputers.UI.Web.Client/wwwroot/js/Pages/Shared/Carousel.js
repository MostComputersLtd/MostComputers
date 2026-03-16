const activeTransformStyle = "translateX(0%)";
const leftTransformStyle = "translateX(-100%)";
const rightTransformStyle = "translateX(100%)";

const carouselTransitionTimeMsAttribute = "data-custom-carousel-transition-time-ms";
const carouselAutoSlideIntervalIdAttribute = "data-custom-carousel-auto-slide-interval-id";
const carouselAutoSlideIntervalTimeMsAttribute = "data-custom-carousel-auto-slide-interval-time-ms";

const carouselItemClass = "custom-carousel-slide";
const activeItemClass = "active-carousel-item";

const carouselIndicatorsListClass = "custom-carousel-indicators";
const activeIndicatorClass = "active-carousel-indicator";

function setDefaultTransitionStyle(element, transitionTimeMs) {

    element.style.transitionProperty = "all"
    element.style.transitionTimingFunction = "ease"
    element.style.transitionDuration = `${transitionTimeMs}ms`
}

function startAutoSlide(allowAutoSlide, carouselId) {

    if (!allowAutoSlide) return;

    const carousel = document.getElementById(carouselId);

    if (!carousel) return;

    const existingIntervalId = carousel.getAttribute(carouselAutoSlideIntervalIdAttribute);

    if (existingIntervalId != null) return;

    const intervalMs = carousel.getAttribute(carouselAutoSlideIntervalTimeMsAttribute);

    if (intervalMs == null) return;

    const intervalId = setInterval(() => {
        slideInDirection(carouselId, false)
    },
    intervalMs);

    carousel.setAttribute(carouselAutoSlideIntervalIdAttribute, intervalId);
}

function stopAutoSlide(carouselId) {

    const carousel = document.getElementById(carouselId);

    if (!carousel) return;

    const intervalId = carousel.getAttribute(carouselAutoSlideIntervalIdAttribute);

    if (!intervalId) return;

    if (intervalId !== null) {
        clearInterval(intervalId);

        carousel.removeAttribute(carouselAutoSlideIntervalIdAttribute);
    }
}

function slideInDirection(carouselId, goBackwards) {

    if (goBackwards) {
        prev(carouselId);
    }
    else {
        next(carouselId);
    }
}

function next(carouselId) {

    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttribute);

    const transitionTimeMs = getNumberOrZeroFromString(transitionTimeMsString);

    if (carouselItems.length <= 1) return;

    var activeItemIndex = getCurrentActiveItemIndex(carouselItems);

    if (activeItemIndex == null) return;

    const currentItem = carouselItems[activeItemIndex];

    setDefaultTransitionStyle(currentItem, transitionTimeMs);
    currentItem.style.transform = leftTransformStyle;
    currentItem.classList.remove(activeItemClass);

    activeItemIndex = (activeItemIndex + 1) % carouselItems.length;

    const nextItem = carouselItems[activeItemIndex];

    nextItem.style.transition = "";
    nextItem.style.transform = rightTransformStyle;

    void nextItem.offsetWidth;

    setActiveIndicatorAt(getCarouselIndicators(carousel), activeItemIndex);

    setDefaultTransitionStyle(nextItem, transitionTimeMs);
    nextItem.style.transform = activeTransformStyle;
    nextItem.classList.add(activeItemClass);

    carousel.setAttribute("aria-activedescendant", nextItem.id);
}

function prev(carouselId) {

    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttribute);

    const transitionTimeMs = getNumberOrZeroFromString(transitionTimeMsString);

    if (carouselItems.length <= 1) return;

    var activeItemIndex = getCurrentActiveItemIndex(carouselItems);

    if (activeItemIndex == null) return;

    const currentItem = carouselItems[activeItemIndex];

    currentItem.classList.remove(activeItemClass);
    setDefaultTransitionStyle(currentItem, transitionTimeMs);
    currentItem.style.transform = rightTransformStyle;

    activeItemIndex = (activeItemIndex - 1);

    if (activeItemIndex < 0)
    {
        activeItemIndex = carouselItems.length - 1;
    }

    const prevItem = carouselItems[activeItemIndex];

    prevItem.style.transition = "";
    prevItem.style.transform = leftTransformStyle;

    void prevItem.offsetWidth;

    setActiveIndicatorAt(getCarouselIndicators(carousel), activeItemIndex);

    prevItem.classList.add(activeItemClass);
    setDefaultTransitionStyle(prevItem, transitionTimeMs);
    prevItem.style.transform = activeTransformStyle;

    carousel.setAttribute("aria-activedescendant", prevItem.id);
}

function goTo(carouselId, index) {

    const carousel = document.getElementById(carouselId);

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttribute);

    const transitionTimeMs = getNumberOrZeroFromString(transitionTimeMsString);

    if (carouselItems.length <= 1) return;

    var activeItemIndex = getCurrentActiveItemIndex(carouselItems);

    if (activeItemIndex == index) return;

    const currentItem = carouselItems[activeItemIndex];

    currentItem.classList.remove(activeItemClass);
    setDefaultTransitionStyle(currentItem, transitionTimeMs);
    currentItem.style.transform = (index > activeItemIndex) ? leftTransformStyle : rightTransformStyle;

    const nextItem = carouselItems[index];

    nextItem.style.transition = "";
    nextItem.style.transform = (index > activeItemIndex) ? rightTransformStyle : leftTransformStyle;

    void nextItem.offsetWidth;

    setActiveIndicatorAt(getCarouselIndicators(carousel), index);

    nextItem.classList.add(activeItemClass);
    setDefaultTransitionStyle(nextItem, transitionTimeMs);
    nextItem.style.transform = activeTransformStyle;

    carousel.setAttribute("aria-activedescendant", nextItem.id);
}

function getCurrentActiveItemIndex(carouselItems) {
    var activeItemIndex = null;

    for (var i = 0; i < carouselItems.length; i++) {
        const item = carouselItems[i];

        if (item.classList.contains(activeItemClass)) {
            activeItemIndex = i;

            break;
        }
    }
    return activeItemIndex;
}

function getCarouselIndicators(carousel)
{
    const carouselList = carousel.querySelector(`.${carouselIndicatorsListClass}`);

    const carouselIndicators = [...carouselList.children];

    return carouselIndicators;
}

function setActiveIndicatorAt(carouselIndicators, index)
{
    if (index < 0 || index >= carouselIndicators.length) return;

    for (var i = 0; i < carouselIndicators.length; i++) {

        const indicator = carouselIndicators[i]

        if (indicator.classList.contains(activeIndicatorClass))
        {
            indicator.classList.remove(activeIndicatorClass);

            break;
        }
    }

    carouselIndicators[index].classList.add(activeIndicatorClass);
}

function getNumberOrZeroFromString(stringValue) {
    if (stringValue == null && stringValue != "") return 0;

    var output = 0;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}