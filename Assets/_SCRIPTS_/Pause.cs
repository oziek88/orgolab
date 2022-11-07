using UnityEngine;

public class Pause : MonoBehaviour {

    protected bool paused;

    void Update() {
        if (!paused) {  Time.timeScale = 1; }
        if (paused) { Time.timeScale = 0; }
    }

    void OnPauseGame() { paused = true; }

    void OnResumeGame() { paused = false; }
}
