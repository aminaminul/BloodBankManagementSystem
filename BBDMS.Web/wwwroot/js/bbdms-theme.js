// Theme Controller
(function () {
    'use strict';

    var COOKIE = 'bbdms.theme';
    var STORAGE_KEY = 'bbdms-theme';
    var root = document.documentElement;

    function isDark(value) { return value === 'dark'; }
    function normalize(value) { return isDark(value) ? 'dark' : 'light'; }

    function readCookie() {
        var match = document.cookie.match(/(?:^|;\s*)bbdms\.theme=([^;]+)/);
        return match ? normalize(decodeURIComponent(match[1])) : null;
    }

    function readStorage() {
        try { return normalize(localStorage.getItem(STORAGE_KEY)); } catch (e) { return null; }
    }

    function writeCookie(theme) {
        var secure = location.protocol === 'https:' ? '; Secure' : '';
        document.cookie = COOKIE + '=' + theme + '; path=/; max-age=31536000; SameSite=Lax' + secure;
    }

    function writeStorage(theme) {
        try { localStorage.setItem(STORAGE_KEY, theme); } catch (e) { }
    }

    function currentTheme() {
        return normalize(root.getAttribute('data-theme'));
    }

    function syncButtons(theme) {
        var dark = isDark(theme);
        document.querySelectorAll('[data-theme-toggle]').forEach(function (button) {
            button.setAttribute('aria-pressed', dark ? 'true' : 'false');
            button.setAttribute('aria-label', dark ? 'Switch to light mode' : 'Switch to dark mode');
            button.setAttribute('title', dark ? 'Light mode' : 'Dark mode');
        });
    }

    function apply(theme, options) {
        var value = normalize(theme);
        var settings = options || {};

        root.setAttribute('data-theme', value);
        root.style.colorScheme = value;
        writeCookie(value);
        writeStorage(value);
        syncButtons(value);

        if (settings.notify !== false) {
            window.dispatchEvent(new CustomEvent('bbdms:themechange', { detail: { theme: value } }));
        }
    }

    // Persist Theme Selection
    function persist(theme) {
        var form = document.getElementById('themeForm');
        if (!form) return;

        var data = new FormData(form);
        data.set('theme', theme);

        fetch(form.getAttribute('action'), {
            method: 'POST',
            body: data,
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest', 'Accept': 'application/json' }
        }).catch(function () { });
    }

    document.addEventListener('click', function (event) {
        var toggle = event.target.closest('[data-theme-toggle]');
        if (!toggle) return;

        event.preventDefault();
        var next = isDark(currentTheme()) ? 'light' : 'dark';
        apply(next);
        persist(next);
    });

    // OS Preference Change Listener
    if (window.matchMedia) {
        var media = window.matchMedia('(prefers-color-scheme: dark)');
        var onSystemChange = function (event) {
            if (readCookie() || readStorage()) return;
            apply(event.matches ? 'dark' : 'light');
        };
        if (media.addEventListener) media.addEventListener('change', onSystemChange);
        else if (media.addListener) media.addListener(onSystemChange);
    }

    // Initialize Theme
    apply(currentTheme(), { notify: false });

    window.bbdmsTheme = { apply: apply, current: currentTheme };
})();
