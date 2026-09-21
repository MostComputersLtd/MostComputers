import * as fetchWithAuthRedirect from "../Authentication/FetchWithAuthRedirect.js";

const ordersSearchStatusesSelectId = "ordersSearchStatusesSelect";
const ordersSearchClientSelectId = "ordersSearchClientSelect";
const ordersSearchInputId = "ordersSearchInput";

const ordersSearchButtonId = "ordersSearchButton";
const exportPricesButtonId = "exportPricesButton";
const exportXmlButtonId = "exportXmlButton";

const searchResultsContainerId = "searchResultsContainer";
const orderViewContainerId = "orderViewContainer";

const searchResultItemName = "searchResultItem";

const loadingClass = "loading";
const exportButtonDisplayedClass = "displayed";
const searchResultSelectedClass = "search-result-item-selected";

const searchResultItemOrderIdAttribute = "data-order-id";

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initEvents);
}
else {
    initEvents();
}

function initEvents() {

    const ordersSearchButton = document.getElementById(ordersSearchButtonId);
    const exportPricesButton = document.getElementById(exportPricesButtonId);
    const exportXmlButton = document.getElementById(exportXmlButtonId);

    ordersSearchButton.addEventListener("click", getSearchResultsAndDisplayAsync);
    exportPricesButton.addEventListener("click", exportPricesToXmlAsync);
    exportXmlButton.addEventListener("click", exportOrderToXmlAsync);
}

async function getSearchResultsAndDisplayAsync() {

    const url = "api/components/orders/search";

    const ordersSearchButton = document.getElementById(ordersSearchButtonId);

    let response = null;

    try {
        ordersSearchButton.classList.add(loadingClass);

        const searchOptions = getSearchOptionsFromCurrentData();

        response = await fetch(url, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/html'
            },
            body: JSON.stringify(searchOptions)
        });
    }
    finally {
        ordersSearchButton.classList.remove(loadingClass);
    }

    if (response === null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const searchResults = await response.text();

    const searchResultsContainer = document.getElementById(searchResultsContainerId);

    searchResultsContainer.innerHTML = searchResults;

    const searchResultItems = [...searchResultsContainer.querySelectorAll(`[name="${searchResultItemName}"]`)];

    for (const searchResultItem of searchResultItems) {

        searchResultItem.addEventListener("click", function(e) {

            const item = e.currentTarget;
            const orderId = item.getAttribute(searchResultItemOrderIdAttribute);

            getOrderByIdAndDisplayAsync(orderId, item.id);
        });
    }
}

async function getOrderByIdAndDisplayAsync(orderId, searchResultElementId = null) {

    const url = `api/components/orders/order/${orderId}`;

    const response = await fetch(url, {
        method: "GET",
        headers: {
            'Accept': 'application/html'
        }
    });

    if (response === null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    if (searchResultElementId != null) {

        const searchResultsContainer = document.getElementById(searchResultsContainerId);

        const selectedElement = searchResultsContainer.querySelector(`.${searchResultSelectedClass}`);

        if (selectedElement != null) {
            selectedElement.classList.remove(searchResultSelectedClass);
        }

        const searchResultElement = document.getElementById(searchResultElementId);

        searchResultElement.classList.add(searchResultSelectedClass);
    }

    const exportPricesButton = document.getElementById(exportPricesButtonId);
    const exportXmlButton = document.getElementById(exportXmlButtonId);

    exportPricesButton.classList.add(exportButtonDisplayedClass);
    exportXmlButton.classList.add(exportButtonDisplayedClass);

    const orderView = await response.text();

    const orderViewContainer = document.getElementById(orderViewContainerId);

    orderViewContainer.innerHTML = orderView;
}

function getSearchOptionsFromCurrentData() {

    const ordersSearchStatusesSelect = document.getElementById(ordersSearchStatusesSelectId);
    const ordersSearchClientSelect = document.getElementById(ordersSearchClientSelectId);
    const ordersSearchInput = document.getElementById(ordersSearchInputId);

    const ordersSearchStatus = getNumberOrNullFromString(ordersSearchStatusesSelect.value);

    let ordersSearchClientId = null;

    if (ordersSearchClientSelect !== null)
    {
        ordersSearchClientId = getNumberOrNullFromString(ordersSearchClientSelect.value);
    }

    return {
        OrderStatus: ordersSearchStatus,
        ClientId: ordersSearchClientId,
        UserInputString: ordersSearchInput.value,
    };
}

async function exportPricesToXmlAsync() {

    const searchResultsContainer = document.getElementById(searchResultsContainerId);

    const selectedElement = searchResultsContainer.querySelector(`.${searchResultSelectedClass}`);

    if (selectedElement == null) return;

    const orderId = selectedElement.getAttribute(searchResultItemOrderIdAttribute);

    const url = `api/order/xml/${orderId}/prices`;

    const exportPricesButton = document.getElementById(exportPricesButtonId);

    let response = null;

    try {
        exportPricesButton.classList.add(loadingClass);

        response = await fetch(url, {
            method: "GET",
            headers: {
                'Accept': 'application/html'
            }
        });
    }
    finally {
        exportPricesButton.classList.remove(loadingClass);
    }

    if (response === null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const fileName = `MOST_XML_ORDER_PRICES_${orderId}`;

    await downloadFile(response, fileName);
}

async function exportOrderToXmlAsync() {

    const searchResultsContainer = document.getElementById(searchResultsContainerId);

    const selectedElement = searchResultsContainer.querySelector(`.${searchResultSelectedClass}`);

    if (selectedElement == null) return;

    const orderId = selectedElement.getAttribute(searchResultItemOrderIdAttribute);

    const url = `api/order/xml/${orderId}`;

    const exportXmlButton = document.getElementById(exportXmlButtonId);

    let response = null;

    try {
        exportXmlButton.classList.add(loadingClass);

        response = await fetch(url, {
            method: "GET",
            headers: {
                'Accept': 'application/html'
            }
        });
    }
    finally {
        exportXmlButton.classList.remove(loadingClass);
    }

    if (response === null) return;

    const didNotRedirect = fetchWithAuthRedirect.handleAuthRedirect(response);

    if (!didNotRedirect || !response.ok) return;

    const fileName = `MOST_XML_ORDER_${orderId}`;

    await downloadFile(response, fileName);
}

async function applyForRemoteAccessSecret() {

    // To Request a secret you need to use HTTP Basic Authentication,
    // by concatenating your current username and password with a single ':' and no space in between,
    // then encode the whole string using base64 and send a POST Request to 'https://portal.mostbg.com/api/auth/secret',
    // including the base64 data in your Authorization header.
    // If successful you should recieve a 201 Created response with json data in the form of:
    // {
    //     secret: "<YOUR_SECRET>"
    // }
    //
    // NOTE: You must use HTTPS, HTTP-only Requests are denied
    // WARNING: You can only have 1 secret per-user, if you try again after obtaining one, you will get a 409 Confict result

    const url = "https://portal.mostbg.com/api/auth/secret";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Authorization': 'Basic <base64(username:password)>'
        }
    });

    if (response.status === 401) {

        console.log("This only happens with bad credentials");

        return;
    }

    if (response.status !== 201) {

        console.log(response.statusText);

        return;
    }

    const secretJson = await response.json();

    const secret = secretJson.secret;

    console.log(secret);
}

async function applyForAnAccessToken() {

    // To Request a token you need to use HTTP Bearer Authentication,
    // by including the secret (no-encoding needed) in the Authorization header.
    // You need to send a POST request to 'https://portal.mostbg.com/api/auth/token'
    // If successful you should recieve a 201 Created response with json data in the form of:
    // {
    //     token: "<YOUR_TOKEN>",
    //     createdAt: "<ISO8601_DATE>",
    //     expiresAt: "<ISO8601_DATE>"
    // }
    //
    // NOTE: you must use HTTPS, HTTP-only Requests are denied

    const url = "https://portal.mostbg.com/api/auth/token";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Authorization': 'Bearer <YOUR_SECRET>'
        }
    });

    if (response.status === 401) {

        console.log("This only happens with malformed header, expired or revoked secret");

        return;
    }

    if (response.status !== 201) {

        console.log(response.statusText);

        return;
    }

    const tokenJson = await response.json();

    const token = tokenJson.token;
    const createdAt = tokenJson.createdAt;
    const expiresAt = tokenJson.expiresAt;

    console.log(token);
    console.log(createdAt);
    console.log(expiresAt);
}

async function readInvoice() {

    const url = "https://portal.mostbg.com/api/documents/invoice/xml";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Authorization': 'Bearer <YOUR_SECRET>'
        }
    });

    if (response.status === 401) {

        console.log("This only happens with malformed header, expired or revoked secret");

        return;
    }

    if (response.status !== 201) {

        console.log(response.statusText);

        return;
    }

    const tokenJson = await response.json();
}



function getNumberOrNullFromString(stringValue) {

    if (stringValue == null || stringValue === "") return null;

    var output = null;

    const parsedNumber = parseInt(stringValue);

    if (!isNaN(parsedNumber)) {
        output = parsedNumber;
    }

    return output;
}

async function downloadFile(response, fileName) {

    const blob = await response.blob();

    const link = document.createElement('a');

    link.href = URL.createObjectURL(blob);

    link.download = fileName || 'download';

    link.target = "_blank";
    link.rel = "noopener";

    document.body.appendChild(link);

    link.click();
    link.remove();

    URL.revokeObjectURL(link.href);
}
