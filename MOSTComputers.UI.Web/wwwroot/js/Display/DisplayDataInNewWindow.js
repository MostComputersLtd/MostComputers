function openXmlDataInNewWindow(data)
{
    const blob = new Blob([data], { type: 'application/xml' });

    openDataInNewWindow(blob);
}

function openDataInNewWindow(blob)
{
    const url = URL.createObjectURL(blob);

    window.open(url, '_blank');
}

function openDataUrlInNewWindow(url)
{
    window.open(url, '_blank');
}