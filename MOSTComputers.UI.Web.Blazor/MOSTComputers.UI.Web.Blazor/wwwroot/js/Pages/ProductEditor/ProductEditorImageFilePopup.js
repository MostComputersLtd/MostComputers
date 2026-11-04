import * as urlUtils from "../../Common/UrlUtils.js";

const imageFileButtonImageRouteAttribute = "data-image-route";

export function imageDataButtonOpenRoute(e) {

    const imageRoute = e.target.getAttribute(imageFileButtonImageRouteAttribute);

    urlUtils.openDataUrlInNewWindow(imageRoute);
}
