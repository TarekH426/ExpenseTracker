// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Theme toggle with persistence
(function () {
  var root = document.documentElement;
  var btn = document.getElementById('themeToggle');
  var stored = localStorage.getItem('et-theme');
  if (!stored) {
    // Try system preference on first visit
    stored = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }
  applyTheme(stored);

  if (btn) {
    btn.addEventListener('click', function () {
      var next = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
      applyTheme(next);
    });
  }

  function applyTheme(theme) {
    if (theme === 'dark') {
      root.setAttribute('data-theme', 'dark');
      if (btn) btn.textContent = '☀️';
    } else {
      root.removeAttribute('data-theme');
      if (btn) btn.textContent = '🌙';
    }
    localStorage.setItem('et-theme', theme);
  }
})();

// Animated nav underline
(function () {
  var menu = document.querySelector('.et-nav-menu');
  if (!menu) return;
  var indicator = menu.querySelector('.et-nav-indicator');
  var links = Array.prototype.slice.call(menu.querySelectorAll('a.nav-link'));

  function setFrom(el) {
    if (!el) { indicator.style.opacity = 0; return; }
    var rect = el.getBoundingClientRect();
    var base = menu.getBoundingClientRect();
    var width = Math.max(24, rect.width * 0.45);
    var left = rect.left + (rect.width - width) / 2 - base.left;
    indicator.style.transform = 'translateX(' + left + 'px)';
    indicator.style.width = width + 'px';
    indicator.style.opacity = 1;
  }

  function activeLink() {
    var path = location.pathname.toLowerCase();
    var best = links.find(function (a) { return a.getAttribute('href') && path.startsWith(a.getAttribute('href').toLowerCase()); });
    return best || links[0];
  }

  // Hover / focus handlers
  links.forEach(function (a) {
    a.addEventListener('mouseenter', function () { setFrom(a); });
    a.addEventListener('focus', function () { setFrom(a); });
  });
  menu.addEventListener('mouseleave', function () { setFrom(activeLink()); });

  // Initialize to active
  setFrom(activeLink());
})();
