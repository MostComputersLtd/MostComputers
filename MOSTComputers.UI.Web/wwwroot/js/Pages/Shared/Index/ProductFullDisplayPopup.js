async function showProductFullDisplayPopup(
    productId,
    modalId,
    modalDialogId,
    modalContentId)
{
    if (isNaN(parseInt(productId))) return;
    
    const url = "/Index" + "?handler=GetProductDataPopup"
        + "&productId=" + productId;

    const response = await fetch(url,
    {
        method: 'GET',
        headers:
        {
            'Content-Type': 'application/json',
            'RequestVerificationToken':
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
    });

    redirectIfResponseIsRedirected(response);

    if (response.status !== 200) return;

    const responseData = await response.text();

    openModalWithData(modalId, modalDialogId, modalContentId, responseData);
}

async function openProductXmlDataInNewWindowById(productId)
{
    const response = await getProductXmlDataById(productId);

    redirectIfResponseIsRedirected(response);

    if (response.status != 200) return;

    const xmlData = await response.text();

    openXmlDataInNewWindow(xmlData);
}

async function getProductXmlDataById(productId)
{
    if (isNaN(parseInt(productId))) return;

    const url = "/ProductEditor" + "?handler=GetProductXmlById"
        + "&productId=" + productId;

    return await fetch(url,
        {
            method: 'GET',
            headers:
            {
                'Content-Type': 'application/json',
                'RequestVerificationToken':
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
        });
}