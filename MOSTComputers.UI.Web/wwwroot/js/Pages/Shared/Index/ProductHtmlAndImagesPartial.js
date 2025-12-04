async function showProductHtmlAndImagesPopup(
    productId,
    modalId,
    modalDialogId,
    modalContentId)
{
    if (isNaN(parseInt(productId))) return;
    
    const url = "/Index" + "?handler=GetProductHtmlAndImagesPopup"
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