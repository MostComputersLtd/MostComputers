const roleValueAttributeName = "data-selectedRoleValue";
const validationFieldAttributeName = "data-valmsg-for";
const validationSummaryAttributeName = "data-validation-summary";

const validationFieldTextClassName = "text-danger";

const roleSelectedElementClassName = "role-element-selected";

function toggleRoleSelect(roleElementId, roleSelectCheckboxId)
{
    const roleElement = document.getElementById(roleElementId);
    const roleSelectCheckbox = document.getElementById(roleSelectCheckboxId);

    roleSelectCheckbox.checked = !roleSelectCheckbox.checked;

    if (roleSelectCheckbox.checked)
    {
        roleElement.classList.add(roleSelectedElementClassName);
    }
    else
    {
        roleElement.classList.remove(roleSelectedElementClassName);
    }
}

async function redirectToSignInPage(usage, returnUrlInputElementId)
{
    const returnUrlInputElement = document.getElementById(returnUrlInputElementId);

    if (returnUrlInputElement == null) return;

    const returnUrl = returnUrlInputElement.value;

    return await redirectToSignInPageCommon(usage, returnUrl);
}

async function redirectToSignInPageCommon(usage, returnUrl)
{
    var url = "/Accounts/SignIn";

    if (usage != null)
    {
        url += "?usage=" + usage;
    }

    if (returnUrl != null
        && returnUrl.length > 0)
    {
        const startingSymbol = (usage != null) ? "&" : "?";

        url += startingSymbol + "returnUrl=" + returnUrl;
    }

    window.location.href = url;
}

function displayValidationMessages(responseJson)
{
    const validationSummary = document.querySelector('[' + validationSummaryAttributeName + ']');

    const responseJsonEntries = [...Object.entries(responseJson)];

    for (var i = 0; i < responseJsonEntries.length; i++)
    {
        [validationErrorKey, validationMessages] = responseJsonEntries[i];

        if (validationErrorKey == null) continue;

        const validationDisplayElement = document.querySelector('[' + validationFieldAttributeName + '="' + validationErrorKey + '"]');

        if (!Array.isArray(validationMessages))
        {
            const validationElement = createValidationMessageElement(validationMessages);

            if (validationDisplayElement == null)
            {
                validationSummary.appendChild(validationElement);
            }
            else
            {
                validationDisplayElement.appendChild(validationElement);
            }

            continue;
        }

        validationMessages.forEach(function (message)
        {
            const validationElement = createValidationMessageElement(message);

            if (validationDisplayElement == null)
            {
                validationSummary.appendChild(validationElement);

                return;
            }

            validationDisplayElement.appendChild(validationElement);
        });
    }
}

function clearValidationFields()
{
    const validationSummary = document.querySelector('[' + validationSummaryAttributeName + ']');

    const validationDisplayElements = [...document.querySelectorAll('[' + validationFieldAttributeName + ']')];

    validationSummary.innerHTML = "";

    validationDisplayElements.forEach(function (element)
    {
        element.innerHTML = "";
    });
}

function createValidationMessageElement(message)
{
    const validationMessageElement = document.createElement("span");

    validationMessageElement.classList.add(validationFieldTextClassName);

    validationMessageElement.textContent = message;

    return validationMessageElement;
}