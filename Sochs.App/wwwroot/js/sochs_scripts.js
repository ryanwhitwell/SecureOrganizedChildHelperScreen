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

    

    // audio.load();
    // audio.play();
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



    // audio.load();
    // audio.play();
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



    // audio.load();
    // audio.play();
  }
}