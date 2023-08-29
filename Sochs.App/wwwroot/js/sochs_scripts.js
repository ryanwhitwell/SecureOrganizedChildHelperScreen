function setTheme(mode) {
  var root = document.getElementById("root");

  if (root) {
    var currentMode = root.getAttribute("data-bs-theme");

    // Only change modes if needed
    if (currentMode !== mode) {
      root.setAttribute("data-bs-theme", mode);
    }
  }
}