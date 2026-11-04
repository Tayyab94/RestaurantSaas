// Light and dark theme, stored in localStorage (works in the browser and in the MAUI WebView).
window.rsTheme = {
  get: function () {
    var theme = 'light';
    try { theme = localStorage.getItem('rs-theme') || 'light'; } catch (e) { }
    document.documentElement.setAttribute('data-bs-theme', theme);
    return theme;
  },
  set: function (theme) {
    try { localStorage.setItem('rs-theme', theme); } catch (e) { }
    document.documentElement.setAttribute('data-bs-theme', theme);
  }
};

// Apply the stored theme as early as possible to avoid a light flash.
(function () { try { window.rsTheme.get(); } catch (e) { } })();

// Sign-out after inactivity: tells the IdleGuard component when the user does something (at most every 15 seconds).
window.rsIdle = {
  _ref: null,
  _last: 0,
  _handler: null,
  _events: ['mousemove', 'keydown', 'click', 'touchstart', 'scroll'],
  start: function (ref) {
    this.stop();
    var self = this;
    this._ref = ref;
    this._handler = function () {
      var now = Date.now();
      if (now - self._last < 15000) { return; }
      self._last = now;
      if (self._ref) { self._ref.invokeMethodAsync('Activity').catch(function () { }); }
    };
    this._events.forEach(function (e) { document.addEventListener(e, self._handler, { passive: true }); });
  },
  stop: function () {
    var self = this;
    if (this._handler) {
      this._events.forEach(function (e) { document.removeEventListener(e, self._handler); });
    }
    this._handler = null;
    this._ref = null;
  }
};
