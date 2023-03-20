using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper:MonoBehaviour {
    public int totalScore = 0;
    public Text scoreGUI;

    public void AddPoints(int points) {
        totalScore += points;
        scoreGUI.text = totalScore.ToString();
    }
}
