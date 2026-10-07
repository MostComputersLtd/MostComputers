import * as urlUtils from "../../Common/UrlUtils.js";

export function imageDataButtonOpenRoute(e) {

    const imageRoute = e.target.getAttribute(imageFileButtonImageRouteAttribute);

    urlUtils.openDataUrlInNewWindow(imageRoute);
}
