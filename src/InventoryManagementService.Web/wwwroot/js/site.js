// Theme toggle
(function () {
    const themeToggle = document.getElementById('themeToggle');
    const themeIcon = document.getElementById('themeIcon');
    const html = document.documentElement;

    const savedTheme = localStorage.getItem('theme') || 'light';
    html.setAttribute('data-bs-theme', savedTheme);
    updateIcon(savedTheme);

    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            const current = html.getAttribute('data-bs-theme');
            const next = current === 'light' ? 'dark' : 'light';
            html.setAttribute('data-bs-theme', next);
            localStorage.setItem('theme', next);
            updateIcon(next);
        });
    }

    function updateIcon(theme) {
        if (themeIcon) {
            themeIcon.className = theme === 'light' ? 'bi bi-moon' : 'bi bi-sun';
        }
    }
})();

// Like button pulse animation
$(document).on('click', '#likeBtn', function () {
    const btn = $(this);
    btn.addClass('like-pulse');
    setTimeout(() => btn.removeClass('like-pulse'), 300);
});

// AJAX setup for antiforgery token
$(document).ajaxSend(function (e, xhr) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    if (token) {
        xhr.setRequestHeader('RequestVerificationToken', token);
    }
});
