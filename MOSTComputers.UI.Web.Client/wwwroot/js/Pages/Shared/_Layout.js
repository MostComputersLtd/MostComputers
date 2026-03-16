const showCollapsedNavbarListContainerClass = "show-items";

function toggleNavbarItems(navbarId) {
    const navbar = document.getElementById(navbarId);

    if (navbar.classList.contains(showCollapsedNavbarListContainerClass))
    {
        navbar.classList.remove(showCollapsedNavbarListContainerClass);

        return;
    }

    navbar.classList.add(showCollapsedNavbarListContainerClass);
}