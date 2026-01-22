let lastActive = new Date();

function update() {
    lastActive = new Date();
}
export function initializeActivityTracking() {

    const events = getActivityTrackingEvents();

    events.forEach(evt => document.addEventListener(evt, update, {
        passive: true
    }));

    console.log("Activity tracking initialized.");
}

export function uninitializeActivityTracking() {

    const events = getActivityTrackingEvents();

    events.forEach(evt => document.removeEventListener(evt, update));

    console.log("Activity tracking uninitialized.");
}

function getActivityTrackingEvents()
{
    return ["mousemove", "mousedown", "keydown", "touchstart", "touchmove", "wheel", "pointerdown", "pointermove"];
}

export function getLastActivityTime() {

    console.log(`READ: Last activity time = ${lastActive}.`);

    return lastActive.toISOString();
}