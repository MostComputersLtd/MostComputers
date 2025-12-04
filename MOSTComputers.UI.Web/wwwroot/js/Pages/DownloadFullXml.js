async function openFullLegacyXmlInNewWindow()
{
    const legacyXmlText = await getFullLegacyXml();

    if (!legacyXmlText) return;

    openXmlDataInNewWindow(legacyXmlText);
}

async function getFullLegacyXml()
{
    const response = await fetch('/DownloadFullXml?handler=GetLegacyXml',
    {
        method: 'GET',
        headers:
        {
            'Accept': 'application/xml'
        }
    });

    redirectIfResponseIsRedirected(response);

    if (!response.ok) return;

    return await response.text();
}

async function openFullXmlInNewWindow()
{
    const xmlText = await getFullXml();

    if (!xmlText) return;

    openXmlDataInNewWindow(xmlText);
}

async function getFullXml()
{
    const response = await fetch('/DownloadFullXml?handler=GetNewXml',
    {
        method: 'GET',
        headers:
        {
            'Accept': 'application/xml'
        }
    });

    if (!response.ok) return;

    return await response.text();
}