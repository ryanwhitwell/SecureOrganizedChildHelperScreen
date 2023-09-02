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

function playClick() {
  var audio = document.getElementById('player_click');
  if (audio != null) {
    audio.load();
    audio.play();
  }
}

function playCompleteAchievement() {
  var audio = document.getElementById('player_complete_achievement');
  if (audio != null) {
    audio.load();
    audio.play();
  }
}

function playCompleteAllTasks() {
  var audio = document.getElementById('player_complete_all_tasks');
  if (audio != null) {
    audio.load();
    audio.play();
  }
}