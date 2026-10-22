/* =============================================================================
   BBDMS Motion — "Vital Signs"
   -----------------------------------------------------------------------------
   Rules we hold ourselves to:
   • Nothing is hidden unless html.bbdms-motion is present (set in <head> only when
     the visitor has not asked for reduced motion), so content is never blocked.
   • Only transform/opacity are animated — no layout thrash, no scroll handlers
     doing work per element (one rAF-throttled passive listener).
   • Elements already in view on load animate in immediately with a small
     stagger; everything else waits for an IntersectionObserver.
   • Desktop-only affordances (magnetic buttons) check pointer capability.
   ============================================================================= */
(function () {
    'use strict';

    var doc = document;
    var root = doc.documentElement;
    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    var enabled = root.classList.contains('bbdms-motion') && !reduceMotion;
    var hasIO = 'IntersectionObserver' in window;

    /* Elements that get a reveal even on pages that do not opt in explicitly.
       Kept to containers/labels rather than every card to stay cheap. */
    var AUTO_SELECTOR = [
        '.page-head > .container > *',
        '.section-head',
        '.card-modern',
        '.stat-card',
        '.hotline-item',
        '.empty-state',
        '.track-timeline',
        '.chart-frame'
    ].join(', ');

    var STAGGER_TARGETS = [
        '.stat-strip',
        '.service-index',
        '.group-grid',
        '.capacity-grid',
        '.steps',
        '.ledger',
        '.row'
    ].join(', ');

    var delayMap = new WeakMap();
    var seen = new WeakSet();

    /* ── Stagger assignment ─────────────────────────────────────────────── */
    function assignStagger(group, step, max) {
        var children = Array.prototype.slice.call(group.children);
        children.forEach(function (child, index) {
            if (!child || child.nodeType !== 1) return;
            if (delayMap.has(child)) return;
            delayMap.set(child, Math.min(index, max) * step);
        });
    }

    function collectGroups() {
        doc.querySelectorAll('[data-motion-stagger]').forEach(function (group) {
            assignStagger(group, 70, 9);
        });
        doc.querySelectorAll(STAGGER_TARGETS).forEach(function (group) {
            if (group.hasAttribute('data-motion-stagger')) return;
            if (group.matches('.row') && group.closest('.ledger')) return;
            assignStagger(group, 65, 8);
        });
    }

    /* ── Targets ─────────────────────────────────────────────────────────── */
    function collectTargets() {
        var targets = [];
        var push = function (el) {
            if (!el || seen.has(el) || el.hasAttribute('data-motion') && el.getAttribute('data-motion') === 'none') return;
            if (el.closest('tbody')) return;
            targets.push(el);
        };

        doc.querySelectorAll('[data-motion]').forEach(push);
        doc.querySelectorAll('[data-motion-stagger] > *').forEach(push);
        doc.querySelectorAll(AUTO_SELECTOR).forEach(function (el) {
            // Cards inside a staggered group are handled by the group itself.
            if (el.parentElement && el.parentElement.hasAttribute('data-motion-stagger')) return;
            push(el);
        });

        return targets;
    }

    function reveal(el, delay) {
        if (!el || seen.has(el)) return;
        seen.add(el);
        el.classList.remove('mo-reveal');
        if (!enabled) return;
        el.style.setProperty('--mo-delay', (delay || 0) + 'ms');
        el.classList.add('mo-rise');
        el.addEventListener('animationend', function (event) {
            if (event.target !== el) return;
            el.classList.remove('mo-rise');
            el.style.removeProperty('--mo-delay');
        }, { once: true });
    }

    /* ── Counters ────────────────────────────────────────────────────────── */
    function animateCounter(el) {
        if (el.dataset.counted === '1') return;
        el.dataset.counted = '1';

        var declared = el.getAttribute('data-count-to');
        var raw = declared !== null ? declared : (el.textContent || '').trim();
        if (!/^\d[\d,]*$/.test(raw)) { el.textContent = raw; return; }

        var target = parseInt(raw.replace(/,/g, ''), 10);
        if (!isFinite(target)) { el.textContent = raw; return; }
        if (!enabled || target < 5 || doc.hidden) { el.textContent = target.toLocaleString('en-US'); return; }

        var duration = 700;
        var start = null;
        var finished = false;
        var settle = function () {
            if (finished) return;
            finished = true;
            el.textContent = target.toLocaleString('en-US');
        };

        el.textContent = '0';

        function frame(now) {
            if (finished) return;
            if (start === null) start = now;
            var progress = Math.min((now - start) / duration, 1);
            var eased = 1 - Math.pow(1 - progress, 3);
            el.textContent = Math.round(target * eased).toLocaleString('en-US');
            if (progress < 1) window.requestAnimationFrame(frame);
            else finished = true;
        }
        window.requestAnimationFrame(frame);
        // If frames are throttled (hidden tab, backgrounded browser) the value
        // must still end up correct rather than stuck at zero.
        window.setTimeout(settle, duration + 450);
    }

    function initCounters() {
        var counters = doc.querySelectorAll('[data-count-to], .stat-val, .stat-number, .svc-count');
        if (!counters.length) return;
        if (!hasIO) { counters.forEach(animateCounter); return; }

        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                io.unobserve(entry.target);
                animateCounter(entry.target);
            });
        }, { threshold: 0.4 });

        counters.forEach(function (counter) {
            // Only numeric leaves are animated.
            if (counter.querySelector('i') || counter.querySelector('span')) return;
            var text = (counter.textContent || '').trim();
            if (!/^\d[\d,]*$/.test(text)) return;
            io.observe(counter);
        });
    }

    /* ── Meters: draw on enter ───────────────────────────────────────────── */
    function initMeters() {
        var fills = doc.querySelectorAll('.meter-fill');
        if (!fills.length) return;
        fills.forEach(function (fill) {
            var width = fill.style.width;
            fill.dataset.targetWidth = width;
            if (enabled) fill.style.width = '0%';
        });
        var draw = function (fill) {
            fill.style.width = fill.dataset.targetWidth || fill.style.width;
        };
        if (!enabled || !hasIO) { fills.forEach(draw); return; }
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                io.unobserve(entry.target);
                window.setTimeout(function () { draw(entry.target); }, 120);
            });
        }, { threshold: 0.3 });
        fills.forEach(function (fill) { io.observe(fill); });
    }

    /* ── Timelines (live tracking pages) ─────────────────────────────────── */
    function initTimelines() {
        doc.querySelectorAll('.track-timeline').forEach(function (timeline) {
            var draw = function () {
                if (timeline.dataset.drawn === '1') return;
                timeline.dataset.drawn = '1';
                timeline.classList.add('is-drawn');
                Array.prototype.forEach.call(timeline.children, function (item, index) {
                    item.style.setProperty('--i', index);
                });
            };
            if (!hasIO) { draw(); return; }
            var io = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (!entry.isIntersecting) return;
                    io.disconnect();
                    draw();
                });
            }, { threshold: 0 });
            io.observe(timeline);
        });
    }

    /* ── Scroll progress ─────────────────────────────────────────────────── */
    function initScrollProgress() {
        var bar = doc.querySelector('[data-scroll-progress]');
        if (!bar) return;
        var raf = null;
        var update = function () {
            raf = null;
            var height = doc.documentElement.scrollHeight - window.innerHeight;
            var progress = height > 0 ? Math.min(Math.max(window.scrollY / height, 0), 1) : 0;
            bar.style.transform = 'scaleX(' + progress.toFixed(4) + ')';
        };
        window.addEventListener('scroll', function () {
            if (raf !== null) return;
            raf = window.requestAnimationFrame(update);
        }, { passive: true });
        window.addEventListener('resize', update, { passive: true });
        update();
    }

    /* ── Magnetic CTAs (pointer devices only) ────────────────────────────── */
    function initMagnetic() {
        if (!enabled || !window.matchMedia || !window.matchMedia('(hover: hover) and (pointer: fine)').matches) return;

        doc.querySelectorAll('[data-magnetic]').forEach(function (el) {
            var strength = 6;
            el.addEventListener('pointermove', function (event) {
                var rect = el.getBoundingClientRect();
                var dx = (event.clientX - rect.left - rect.width / 2) / rect.width;
                var dy = (event.clientY - rect.top - rect.height / 2) / rect.height;
                el.style.transform = 'translate3d(' + (dx * strength).toFixed(2) + 'px,' + (dy * strength).toFixed(2) + 'px,0)';
            });
            el.addEventListener('pointerleave', function () {
                el.style.transform = '';
            });
        });
    }

    /* ── Reveal orchestration ────────────────────────────────────────────── */
    function initReveals() {
        collectGroups();
        var targets = collectTargets();
        if (!targets.length) return;

        if (!enabled) {
            targets.forEach(function (el) { seen.add(el); });
            return;
        }

        // Above the fold: play on load with a short cascade.
        var viewportHeight = window.innerHeight || 800;
        var aboveFold = [];
        var belowFold = [];

        targets.forEach(function (el) {
            var rect = el.getBoundingClientRect();
            if (rect.top < viewportHeight * 0.92) aboveFold.push(el);
            else belowFold.push(el);
        });

        aboveFold.slice(0, 8).forEach(function (el, index) {
            window.setTimeout(function () { reveal(el, delayMap.get(el) || index * 60); }, 30);
        });

        if (!hasIO) {
            belowFold.forEach(function (el) { reveal(el, 0); });
            return;
        }

        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                io.unobserve(entry.target);
                reveal(entry.target, delayMap.get(entry.target) || 0);
            });
        }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

        belowFold.forEach(function (el) {
            el.classList.add('mo-reveal');
            io.observe(el);
        });

        // Printing must never show blank sections.
        window.addEventListener('beforeprint', function () {
            belowFold.forEach(function (el) {
                io.unobserve(el);
                el.classList.remove('mo-reveal');
                el.classList.add('mo-rise');
            });
        });
    }

    /* ── Init ────────────────────────────────────────────────────────────── */
    var initialized = false;
    function init() {
        if (initialized) return;
        initialized = true;
        initReveals();
        initCounters();
        initMeters();
        initTimelines();
        initScrollProgress();
        initMagnetic();
    }

    if (doc.readyState === 'loading') {
        doc.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Public hooks for pages that inject content later.
    window.bbdmsMotion = {
        reveal: reveal,
        revealAll: function () { collectTargets().forEach(function (el) { reveal(el, 0); }); },
        enabled: enabled
    };
})();
