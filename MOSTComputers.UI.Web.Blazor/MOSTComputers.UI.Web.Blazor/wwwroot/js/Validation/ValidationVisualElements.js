export function clearValidationMessages(validationMessagesContainer) {

    while (validationMessagesContainer.lastElementChild) {

        validationMessagesContainer.removeChild(validationMessagesContainer.lastElementChild);
    }
}

export function displayValidationMessages(
    validationErrors,
    validationMessagesContainer,
    getValidationMessageFromValidationErrorFunc) {

    for (const [_, validationErrorObjects] of Object.entries(validationErrors)) {

        for (const validationError of validationErrorObjects) {

            const defaultMessage = getValidationMessageFromValidationErrorFunc(validationError);

            createMessageAndInsertInContainer(validationMessagesContainer, defaultMessage);
        }
    }
}

export function clearValidationMessagesWithMap(propertyNamesToValidationMessagesContainerIdsMap) {

    for (const [_, validationMessagesId] of propertyNamesToValidationMessagesContainerIdsMap) {

        const validationMessagesContainer = document.getElementById(validationMessagesId);

        while (validationMessagesContainer.lastElementChild) {

            validationMessagesContainer.removeChild(validationMessagesContainer.lastElementChild);
        }
    }
}

export function displayValidationMessagesWithMap(
    validationErrors,
    propertyNamesToValidationMessagesContainerIdsMap,
    getValidationMessageFromValidationErrorFunc) {

    for (const [name, validationErrorObjects] of Object.entries(validationErrors)) {

        const validationMessagesId = propertyNamesToValidationMessagesContainerIdsMap.get(name);

        if (validationMessagesId === undefined) {

            console.log(name);

            continue;
        }

        const validationMessagesContainer = document.getElementById(validationMessagesId);

        for (const validationError of validationErrorObjects) {

            const defaultMessage = getValidationMessageFromValidationErrorFunc(validationError);

            createMessageAndInsertInContainer(validationMessagesContainer, defaultMessage);
        }
    }
}

export function createMessageAndInsertInContainer(
    containerElement,
    text,
    insertIndex = null,
    additionalAttributes = null) {

    const messageElement = createMessage(text, additionalAttributes);

    if (insertIndex != null && containerElement.childElementCount < insertIndex) {

        containerElement.insertBefore(messageElement, containerElement.children[insertIndex]);
    }
    else {

        containerElement.appendChild(messageElement);
    }
}

export function createMessage(text, additionalAttributes = null) {

    const newItem = document.createElement("li");

    newItem.textContent = text;

    if (additionalAttributes) {

        for (const [name, value] of additionalAttributes) {

            newItem.setAttribute(name, value);
        }
    }

    return newItem;
}
