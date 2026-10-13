// =====================================================================
// LearnHub - site.js
// Client-side enhancements: form validation, progress helpers, alerts.
// =====================================================================

// Highlight invalid fields as soon as the user leaves them
(function () {
    document.querySelectorAll("form").forEach(function (form) {
        form.addEventListener("blur", function (e) {
            if (e.target.matches("input, select, textarea")) {
                // let the browser apply :user-invalid styling after interaction
                e.target.classList.add("touched");
            }
        }, true);
    });
})();

// Auto-dismiss flash alerts after 5 seconds
window.setTimeout(function () {
    document.querySelectorAll(".alert-dismissible").forEach(function (el) {
        var btn = el.querySelector(".btn-close");
        if (btn) btn.click();
    });
}, 5000);

// Member dashboard: keep the progress slider + number input in sync
(function () {
    document.querySelectorAll("[data-lh-progress-range]").forEach(function (range) {
        var output = document.querySelector(range.getAttribute("data-lh-progress-range"));
        if (!output) return;
        range.addEventListener("input", function () {
            output.textContent = range.value + "%";
        });
    });
})();

// Member dashboard: +/- quick progress buttons update the hidden value
(function () {
    document.querySelectorAll("[data-lh-step-target]").forEach(function (btn) {
        btn.addEventListener("click", function () {
            var input = document.getElementById(btn.getAttribute("data-lh-step-target"));
            if (!input) return;
            var step = parseInt(btn.getAttribute("data-lh-step"), 10) || 10;
            var next = Math.min(100, Math.max(0, (parseInt(input.value, 10) || 0) + step));
            input.value = next;
            var label = document.getElementById(input.id + "-label");
            if (label) label.textContent = next + "%";
        });
    });
})();

// Confirm destructive actions (delete buttons with data-lh-confirm)
(function () {
    document.querySelectorAll("form[data-lh-confirm]").forEach(function (form) {
        form.addEventListener("submit", function (e) {
            if (!window.confirm(form.getAttribute("data-lh-confirm"))) {
                e.preventDefault();
            }
        });
    });
})();
