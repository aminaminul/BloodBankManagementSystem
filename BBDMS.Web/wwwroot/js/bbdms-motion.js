// Motion UI Controller
(function () {
    'use strict';

    var root = document.documentElement;
    var prefersReduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var enabled = root.classList.contains('bbdms-motion') && !prefersReduced;

    var AUTO_SELECTOR = [
        '.card-modern',
        '.stat-card',
        '.service-cat-card',
        '.svc-card',
        '.donor-row',
        '.filter-bar',
        '.stat-item',
        '.section-title',
        '.section-sub',
        '.eyebrow',
        '.hotline-item',
        '.page-hero-sm .container > *',
        '.hero-modern .container > .row > *'
    ].join(', ');

    var delays = new WeakMap();

    function markRise(element, delay) {
        if (!element || element.__moRisen) return;
        element.__moRisen = true;

        if (!enabled) {
            element.classList.remove('mo-reveal');
            return;
        }

        if (delay) element.style.setProperty('--mo-delay', delay + 'ms');
        element.classList.remove('mo-reveal');
        element.classList.add('mo-rise');
        element.addEventListener('animationend', function (event) {
            if (event.target !== element) return;
            element.classList.remove('mo-rise');
            element.style.removeProperty('--mo-delay');
            element.style.removeProperty('opacity');
        }, { once: true });
    }

    // Element Reveal
    function reveal(element, delay) {
        if (typeof element === 'string') element = document.querySelector(element);
        if (element) markRise(element, delay || 0);
    }

    function assignStaggerDelay(container, step, maxSteps) {
        var children = Array.prototype.slice.call(container.children);
        children.forEach(function (child, index) {
            var target = child.matches && child.matches(AUTO_SELECTOR) ? child : child.querySelector ? child.querySelector(AUTO_SELECTOR) : null;
            if (!target || delays.has(target)) return;
            delays.set(target, Math.min(index, maxSteps) * step);
        });
    }

    function collectTargets() {
        var targets = [];
        var add = function (el) {
            if (el.tagName === 'LI') return;
            targets.push(el);
        };

        document.querySelectorAll(AUTO_SELECTOR).forEach(add);
        document.querySelectorAll('[data-motion]').forEach(function (el) {
            if (el.getAttribute('data-motion') === 'none') return;
            add(el);
        });
        document.querySelectorAll('[data-motion-stagger] > *').forEach(add);

        document.querySelectorAll('.row').forEach(function (row) { assignStaggerDelay(row, 70, 8); });
        document.querySelectorAll('[data-motion-stagger]').forEach(function (group) { assignStaggerDelay(group, 90, 10); });

        var mobileSafe = [];
        document.querySelectorAll('[data-motion-delay]').forEach(function (el) {
            delays.set(el, parseInt(el.getAttribute('data-motion-delay'), 10) || 0);
        });

        targets.forEach(function (el) {
            if (el.__moRisen || el.getAttribute('data-motion') === 'none') return;
            if (el.closest('tbody')) return;
            mobileSafe.push(el);
        });

        return mobileSafe;
    }

    // Scroll Observer
    function observeReveals() {
        var targets = collectTargets();
        if (!targets.length) return;

        if (!('IntersectionObserver' in window)) {
            targets.forEach(function (el) { markRise(el, delays.get(el) || 0); });
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                markRise(entry.target, delays.get(entry.target) || 0);
                observer.unobserve(entry.target);
            });
        }, { threshold: 0, rootMargin: '0px 0px -40px 0px' });

        targets.forEach(function (el) {
            el.classList.add('mo-reveal');
            observer.observe(el);
        });

        window.addEventListener('beforeprint', function () {
            targets.forEach(function (el) { observer.unobserve(el); markRise(el, 0); });
        });
    }

    // Counter Animations
    function animateCounters() {
        var counters = document.querySelectorAll('.stat-val, .stat-number, .svc-count, [data-count-to]');
        counters.forEach(function (el) {
            if (el.children.length > 0 || el.__moCounted) return;

            var raw = (el.textContent || '').trim();
            if (!/^[\d,]+(\.\d+)?$/.test(raw)) return;

            var decimals = raw.indexOf('.') > -1 ? raw.split('.')[1].length : 0;
            var target = parseFloat(raw.replace(/,/g, ''));
            if (!isFinite(target)) return;
            el.__moCounted = true;
            if (!enabled || target < 10) return;

            var duration = 1100;
            var start = null;

            function frame(timestamp) {
                if (start === null) start = timestamp;
                var progress = Math.min((timestamp - start) / duration, 1);
                var eased = progress === 1 ? 1 : 1 - Math.pow(2, -10 * progress);
                var value = target * eased;
                el.textContent = value.toLocaleString('en-US', {
                    minimumFractionDigits: decimals,
                    maximumFractionDigits: decimals
                });
                if (progress < 1) window.requestAnimationFrame(frame);
                else el.textContent = raw;
            }

            el.textContent = '0';
            window.requestAnimationFrame(frame);
        });
    }

    // Progress Bar Animations
    function animateProgressBars() {
        document.querySelectorAll('.progress-bar[style*="width"]').forEach(function (bar) {
            if (bar.__moBar) return;
            bar.__moBar = true;

            var target = bar.style.width;
            if (!enabled) return;

            bar.style.width = '0%';
            window.setTimeout(function () {
                bar.style.transition = 'width .9s cubic-bezier(.16,1,.3,1)';
                bar.style.width = target;
                window.setTimeout(function () { bar.style.removeProperty('transition'); }, 1000);
            }, 180);
        });
    }

    // Timeline Animations
    function animateTimelines() {
        document.querySelectorAll('.track-timeline').forEach(function (timeline) {
            function draw() {
                if (timeline.__moDrawn) return;
                timeline.__moDrawn = true;
                timeline.classList.add('is-drawn');
                if (!enabled) return;
                Array.prototype.forEach.call(timeline.children, function (item, index) {
                    item.style.setProperty('--i', index);
                });
            }

            if (!('IntersectionObserver' in window)) { draw(); return; }

            var io = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (!entry.isIntersecting) return;
                    draw();
                    io.disconnect();
                });
            }, { threshold: 0 });
            io.observe(timeline);
        });
    }

    // Scroll State Tracking
    function initScrollState() {
        var lastState = false;
        var apply = function () {
            var scrolled = window.scrollY > 12;
            if (scrolled === lastState) return;
            lastState = scrolled;
            document.body.classList.toggle('mo-scrolled', scrolled);
        };

        window.addEventListener('scroll', function () { window.requestAnimationFrame(apply); }, { passive: true });
        apply();
    }

    // Tilt Hover Effect
    function initTiltHover() {
        if (!enabled || window.matchMedia('(hover: none)').matches) return;

        document.querySelectorAll('.stat-card').forEach(function (card) {
            card.addEventListener('pointermove', function (event) {
                var rect = card.getBoundingClientRect();
                card.style.setProperty('--mo-x', ((event.clientX - rect.left) / rect.width * 100).toFixed(1) + '%');
                card.style.setProperty('--mo-y', ((event.clientY - rect.top) / rect.height * 100).toFixed(1) + '%');
            });
        });
    }

    // Initialization
    function init() {
        observeReveals();
        animateCounters();
        animateProgressBars();
        animateTimelines();
        initScrollState();
        initTiltHover();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    window.bbdmsMotion = {
        reveal: reveal,
        revealAll: function () { collectTargets().forEach(function (el) { markRise(el, 0); }); },
        enabled: enabled
    };
})();
