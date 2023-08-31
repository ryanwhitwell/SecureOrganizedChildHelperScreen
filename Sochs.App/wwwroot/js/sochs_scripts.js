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

function playAudioFile(src) {
  console.log("playing audio");
  var audio = document.getElementById('player');
  if (audio != null) {
    var audioSource = document.getElementById('playerSource');
    if (audioSource != null) {
      audioSource.src = src;
      audio.load();
      audio.play();
    }
  }
}