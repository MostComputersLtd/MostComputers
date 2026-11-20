import * as conversionUtils from "../Common/ConversionUtils.js"

export const carouselName = "customCarousel";

const prevControlName = "prevControl";
const nextControlName = "nextControl";

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

export function initNavigationEvents(carousel) {

    const prevControl = carousel.querySelector(`[name='${prevControlName}']`);
    const nextControl = carousel.querySelector(`[name='${nextControlName}']`);

    prevControl.addEventListener("click", onPrevControlClick);
    nextControl.addEventListener("click", onNextControlClick);

    const indicators = getCarouselIndicators(carousel);

    for (const indicator of indicators) {

        indicator.addEventListener("click", onIndicatorClicked);
    }
}

export async function initAutoSlideEventsAsync(carousel, startNow, stopWhenCursorIsOnTop) {

    if (startNow) {

        await waitForAllImagesInContainerToDecode(carousel);

        startAutoSlide(carousel);
    }

    if (stopWhenCursorIsOnTop) {

        carousel.addEventListener("mouseenter", onCarouselMouseEnter);
        carousel.addEventListener("mouseleave", onCarouselMouseLeave);
    }
}

function onCarouselMouseEnter(e) {

    const carousel = e.currentTarget;

    stopAutoSlide(carousel);
}

function onCarouselMouseLeave(e) {

    const carousel = e.currentTarget;

    startAutoSlide(carousel);
}

export function startAutoSlide(carousel) {

    const existingIntervalId = carousel.getAttribute(carouselAutoSlideIntervalIdAttribute);

    if (existingIntervalId != null) return;

    const intervalMs = carousel.getAttribute(carouselAutoSlideIntervalTimeMsAttribute);

    if (intervalMs == null) return;

    const intervalId = setInterval(() => {
        slideInDirection(carousel, false)
    },
    intervalMs);

    carousel.setAttribute(carouselAutoSlideIntervalIdAttribute, intervalId);
}

export function stopAutoSlide(carousel) {

    const intervalId = carousel.getAttribute(carouselAutoSlideIntervalIdAttribute);

    if (!intervalId) return;

    if (intervalId !== null) {

        clearInterval(intervalId);

        carousel.removeAttribute(carouselAutoSlideIntervalIdAttribute);
    }
}

function slideInDirection(carousel, goBackwards) {

    if (goBackwards) {
        prev(carousel);
    }
    else {
        next(carousel);
    }
}

function onPrevControlClick(e) {

    const carousel = e.currentTarget.closest(`[name='${carouselName}']`);

    prev(carousel);
}

function onNextControlClick(e) {

    const carousel = e.currentTarget.closest(`[name='${carouselName}']`);

    next(carousel);
}

function next(carousel) {

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttribute);

    const transitionTimeMs = getNumberOrZeroFromString(transitionTimeMsString);

    if (carouselItems.length <= 1) return;

    let activeItemIndex = getCurrentActiveItemIndex(carouselItems);

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

function prev(carousel) {

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttribute);

    const transitionTimeMs = getNumberOrZeroFromString(transitionTimeMsString);

    if (carouselItems.length <= 1) return;

    let activeItemIndex = getCurrentActiveItemIndex(carouselItems);

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

function onIndicatorClicked(e) {

    const indicator = e.currentTarget;

    const carousel = indicator.closest(`[name='${carouselName}']`);

    const indicators = getCarouselIndicators(carousel);

    const index = indicators.indexOf(indicator);

    goTo(carousel, index);
}

function goTo(carousel, index) {

    const carouselItems = [...carousel.querySelectorAll(`.${carouselItemClass}`)];

    const transitionTimeMsString = carousel.getAttribute(carouselTransitionTimeMsAttribute);

    const transitionTimeMs = getNumberOrZeroFromString(transitionTimeMsString);

    if (carouselItems.length <= 1) return;

    let activeItemIndex = getCurrentActiveItemIndex(carouselItems);

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

    let activeItemIndex = null;

    for (let i = 0; i < carouselItems.length; i++) {

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

    for (let i = 0; i < carouselIndicators.length; i++) {

        const indicator = carouselIndicators[i]

        if (indicator.classList.contains(activeIndicatorClass))
        {
            indicator.classList.remove(activeIndicatorClass);

            break;
        }
    }

    carouselIndicators[index].classList.add(activeIndicatorClass);
}

export async function waitForAllImagesInContainerToDecode(container) {

    const images = [...container.querySelectorAll('img')];

    waitForAllImagesToDecode(images);
}

export async function waitForAllImagesToDecode(images) {

    if (images.length === 0) return;

    await Promise.all(images.map(image => image.decode()));
}

function getNumberOrZeroFromString(stringValue) {

    const output = conversionUtils.getNumberOrNullFromString(stringValue);

    return output === null ? 0 : output;
}
