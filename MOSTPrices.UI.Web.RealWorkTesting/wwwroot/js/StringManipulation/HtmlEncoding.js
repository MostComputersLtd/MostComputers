function encodeHTML(str)
{
    var div = document.createElement('div');

    div.innerText = str;

    var encoded = div.innerHTML;

    div.remove();

    return encoded;
}