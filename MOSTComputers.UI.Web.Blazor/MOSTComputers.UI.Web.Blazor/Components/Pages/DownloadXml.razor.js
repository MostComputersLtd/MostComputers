
export async function copyTextToClipboard(text)
{
    await navigator.clipboard.writeText(text);
}

export async function fetchAndDownloadXmlFile(url, fileName)
{
    const response = await fetch(url);

    if (!response.ok) return;

    await downloadXmlFile(response, fileName);
}

export async function fetchAndDownloadXmlFileWithBody(url, fileName, requestMethod = "POST", requestBodyAsJson = null)
{
    const response = await fetch(url,
    {
        method: requestMethod,
        headers: {
            'Content-Type': 'application/json'
        },
        body: (requestBodyAsJson == null) ? null : requestBodyAsJson
    });

    if (!response.ok) return;

    await downloadXmlFile(response, fileName);
}

async function downloadXmlFile(response, fileName)
{
    const blob = await response.blob();

    const link = document.createElement('a');

    link.href = URL.createObjectURL(blob);

    link.download = fileName || 'MOST_DOWNLOAD';

    link.target = "_blank";
    link.rel = "noopener noreferrer";

    document.body.appendChild(link);

    link.click();
    link.remove();

    URL.revokeObjectURL(link.href);
}