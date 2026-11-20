export const antiforgeryHeaderName = "RequestVerificationToken";
export const antiforgeryInputName = "__RequestVerificationToken";

export function getAntiforgeryTokenValue() {
    return document.querySelector('input[type = "hidden"][name = "__RequestVerificationToken"]').value;
}
