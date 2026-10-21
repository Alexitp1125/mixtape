// Light/dark theme. The button with data-theme-toggle switches it; the choice is kept in localStorage.
(function () {
    function saved() {
        try {
            return localStorage.getItem('theme') || 'dark';
        } catch {
            return 'dark';
        }
    }

    function apply(theme) {
        document.documentElement.dataset.theme = theme;
    }

    // Delegated, so it keeps working after Blazor's enhanced navigation replaces the page content.
    document.addEventListener('click', function (e) {
        if (!e.target.closest('[data-theme-toggle]')) {
            return;
        }

        const theme = document.documentElement.dataset.theme === 'light' ? 'dark' : 'light';
        apply(theme);

        try {
            localStorage.setItem('theme', theme);
        } catch {
            // Storage blocked: the theme still changes for this page.
        }
    });

    // Enhanced navigation resets the attributes of <html>; put the theme back.
    window.Blazor?.addEventListener('enhancedload', function () {
        apply(saved());
    });
})();
