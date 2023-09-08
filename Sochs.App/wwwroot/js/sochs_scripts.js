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

// Each sound had its own audio source defined in markup in order to support layered sounds
function playClick() {
  var audio = document.getElementById('player_click');
  if (audio != null) {

    audio.load();

    var playPromise = audio.play();

    if (playPromise !== undefined) {
      playPromise.then(_ => {
        // Audio is playing
      })
        .catch(error => {
          // Playing error
        });
    }
  }
}

function playCompleteAchievement() {
  var audio = document.getElementById('player_complete_achievement');
  if (audio != null) {
    audio.load();

    var playPromise = audio.play();

    if (playPromise !== undefined) {
      playPromise.then(_ => {
        // Audio is playing
      })
        .catch(error => {
          // Playing error
        });
    }
  }
}

function playCompleteAllTasks() {
  var audio = document.getElementById('player_complete_all_tasks');
  if (audio != null) {
    audio.load();

    var playPromise = audio.play();

    if (playPromise !== undefined) {
      playPromise.then(_ => {
        // Audio is playing
      })
        .catch(error => {
          // Playing error
        });
    }
  }
}

function showAllDateEventToasts() {
  var allDateEventToasts = document.getElementsByClassName("dateEventToast");

  for (var i = 0; i < allDateEventToasts.length; i++) {
    bootstrap.Toast.getOrCreateInstance(allDateEventToasts[i]).show();
  }
  
}

function loadMinecraftServerData(url) {
  var component = document.getElementById("minecraft_server");

  if (component) {
    component.src = url;
  }
}