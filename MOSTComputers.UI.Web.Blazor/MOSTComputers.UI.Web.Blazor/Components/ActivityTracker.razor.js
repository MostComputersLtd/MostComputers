let lastActive = new Date();

function update() {
    lastActive = new Date();
}
export function initializeActivityTracking() {

    const events = getActivityTrackingEvents();

    events.forEach(evt => document.addEventListener(evt, update, {
        passive: true
    }));
}

export function uninitializeActivityTracking() {

    const events = getActivityTrackingEvents();

    events.forEach(evt => document.removeEventListener(evt, update));
}

function getActivityTrackingEvents()
{
    return ["mousemove", "mousedown", "keydown", "touchstart", "touchmove", "wheel", "pointerdown", "pointermove"];
}

export function getLastActivityTime() {

    return lastActive.toISOString();
}