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

    const url = `api/documents/order/xml/${orderId}/prices`;

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

    const url = `api/documents/order/xml/${orderId}`;

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

async function applyForRemoteAccessClient() {

    // To Request an API client_id and client_secret you need to use HTTP Basic Authentication,
    // by concatenating your current username and password with a single ':' and no space in between,
    // then encode the whole string using base64 and send a POST Request to 'https://portal.mostbg.com/api/auth/register/secret',
    // including the base64 data in your Authorization header.
    // If successful you should recieve a 201 Created response with json data in the form of:
    // {
    //     client_id: "<YOUR_CLIENT_ID>"
    //     client_secret: "<YOUR_CLIENT_SECRET>"
    // }
    //
    // NOTE: You must use HTTPS, HTTP-only Requests are denied
    // WARNING: You can only have 1 client per-user, if you try again after obtaining one, you will get a 409 Confict result

    const username = "<YOUR_USERNAME>";
    const password = "<YOUR_PASSWORD>";

    const authenticationData = btoa(`${username}:${password}`);

    const url = "https://portal.mostbg.com/api/auth/register/secret";

    const response = await fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Authorization': `Basic ${authenticationData}`
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

    const client_id = secretJson.client_id;
    const client_secret = secretJson.client_secret;

    console.log(client_id);
    console.log(client_secret);
}

async function applyForAnAccessToken() {

    // To Request a token you need to use OAuth 2.0 Client Credentials Flow Authentication,
    // by including the client_id and client_secret (no-encoding needed) in the Authorization header.
    // You need to send a POST request to 'https://portal.mostbg.com/api/auth/token'
    // If successful you should recieve a 201 Created response with json data in the form of:
    // {
    //     "access_token": "<YOUR_TOKEN>",
    //     "token_type": "Bearer",
    //     "expires_in": "<SECONDS_UNTIL_EXPIRATION>"
    // }
    //
    // NOTE: you must use HTTPS, HTTP-only Requests are denied

    // Scopes specify what operations you are requesting to do with this token:
    const scopes = "invoices.read warrantyCards.read orders.read";

    const url = "https://portal.mostbg.com/api/auth/connect/token";
    
    const response = fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        },
        body: new URLSearchParams({
            grant_type: "client_credentials",
            client_id: "<YOUR_CLIENT_ID>",
            client_secret: "<YOUR_CLIENT_SECRET>",
            scope: scopes
        })
    });

    if (response.status === 401) {

        console.log("This only happens with malformed request or invalid client_id and client_secret");

        return;
    }

    if (response.status !== 201) {

        console.log(response.statusText);

        return;
    }

    const tokenJson = await response.json();

    const access_token = tokenJson.access_token;
    const token_type = tokenJson.token_type;
    const expires_in = tokenJson.expires_in;

    console.log(access_token);
    console.log(token_type);
    console.log(expires_in);
}

async function readInvoice() {

    const invoiceNumber = "<YOUR_INVOICE_NUMBER>"

    const url = `https://portal.mostbg.com/api/documents/invoice/xml/number=${invoiceNumber}`;

    const response = await fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/xml',
            'Authorization': 'Bearer <YOUR_ACCESS_TOKEN>'
        }
    });

    if (response.status === 401) {

        console.log("This only happens with malformed header, expired or revoked token");

        return;
    }

    if (response.status !== 200) {

        console.log(response.statusText);

        return;
    }

    const invoiceXmlData = response.text();

    console.log(invoiceXmlData);
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
