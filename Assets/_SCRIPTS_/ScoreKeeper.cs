using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper:MonoBehaviour {

    public float totalScore = 0;
    public UnityEngine.UI.Text scoreGUI;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(scoreGUI.text != "Score: " + totalScore.ToString()) {
            scoreGUI.text = "Score: " + totalScore.ToString();
        }
    }


}
