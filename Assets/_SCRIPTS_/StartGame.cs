using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame:MonoBehaviour {
    public bool isFreeplay;

    public void Timed() {
        isFreeplay = false;

        DontDestroyOnLoad(this.gameObject);
        foreach(Transform child in this.gameObject.transform) {
            child.gameObject.SetActive(false);
        }

        SceneManager.LoadScene("game");
    }

    public void FreePlay() {
        isFreeplay = true;

        DontDestroyOnLoad(this.gameObject);
        foreach(Transform child in this.gameObject.transform) {
            child.gameObject.SetActive(false);
        }

        SceneManager.LoadScene("game");
    }

}
