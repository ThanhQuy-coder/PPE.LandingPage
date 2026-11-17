(function () {
  var observer = null;

  function showAll() {
    var els = document.querySelectorAll('[data-reveal]');
    for (var i = 0; i < els.length; i++) {
      els[i].classList.add('is-visible');
    }
  }

  function observeReveals() {
    if (window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
      showAll();
      return;
    }

    if (!('IntersectionObserver' in window)) {
      showAll();
      return;
    }

    if (!observer) {
      observer = new IntersectionObserver(
        function (entries) {
          for (var i = 0; i < entries.length; i++) {
            if (entries[i].isIntersecting) {
              entries[i].target.classList.add('is-visible');
              observer.unobserve(entries[i].target);
            }
          }
        },
        { threshold: 0.08, rootMargin: '0px 0px -30px 0px' }
      );
    }

    var els = document.querySelectorAll('[data-reveal]:not(.is-visible)');
    for (var i = 0; i < els.length; i++) {
      observer.observe(els[i]);
    }
  }

  window.ghUi = {
    observeReveals: observeReveals
  };

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', observeReveals);
  } else {
    observeReveals();
  }
})();
