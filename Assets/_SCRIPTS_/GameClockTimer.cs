using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClockTimer:MonoBehaviour {

    public float startTime;
    public float Countdown;
    private float levelScore = 0;
    public Text gameClock;

    private bool minusPoints = false;
    private bool isFreeplay;

    public bool runTimer = false;

    public Spawning spawningController;
    public GameObject scoreBoard;
    public ScoreKeeper scoreKeeper;
    public Toggle pause;

    private int smallFontSize = 180;
    private int bigFontSize = 200;
    

    // Use this for initialization
    void Start() {
        //startTime = 50;
        Countdown = startTime;
        isFreeplay = GameObject.Find("MainMenuCanvas") != null ? GameObject.Find("MainMenuCanvas").GetComponent<StartGame>().isFreeplay : false;
        if(isFreeplay) {
            gameClock.enabled = false;
            scoreBoard.SetActive(false);
        } else {
            gameClock.fontSize = smallFontSize;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {

        if(spawningController.completedReaction == true && Countdown > 0) {
            runTimer = false;
            if(levelScore == 0) {
                levelScore = Countdown;
                Countdown = startTime;
                scoreKeeper.totalScore += levelScore;
            }

        } else if(runTimer && Countdown > 0) {
            if(levelScore != 0) {
                levelScore = 0;
                gameClock.color = new Color(0, 255, 223, 0.15f);
                gameClock.fontSize = smallFontSize;
            }

            Countdown -= 1;

            if(!minusPoints) {
                gameClock.text = Countdown.ToString();
            }

            if(Countdown == 1000) {
                gameClock.color = new Color(255, 206, 0, 0.4f);
                gameClock.fontSize = bigFontSize;
                if(isFreeplay) {
                    Countdown += 1;
                }
            }
            if(Countdown == 995) {
                gameClock.color = new Color(255, 206, 0, 0.2f);
                gameClock.fontSize = smallFontSize;
            }
            if(Countdown == 500) {
                gameClock.color = new Color(255, 0, 0, 0.4f);
                gameClock.fontSize = bigFontSize;
            }
            if(Countdown == 495) {
                gameClock.color = new Color(255, 0, 0, 0.2f);
                gameClock.fontSize = smallFontSize;
            }
            if(Countdown == 100) {
                gameClock.color = new Color(255, 0, 0, 0.4f);
                gameClock.fontSize = bigFontSize;
            }
        } else if(spawningController.completedReaction == false && Countdown <= 0) {
            gameClock.text = "You Lose";
            gameClock.fontSize = 100;
        } else if(spawningController.completedReaction == true && Countdown <= 0) {
            gameClock.text = "Never Give Up!";
            gameClock.color = new Color(0, 206, 0, 0.4f);
            gameClock.fontSize = 80;
        }
    }

    public IEnumerator MinusPoints(float points) {
        minusPoints = true;

        gameClock.color = new Color(255, 0, 0, 0.4f);
        gameClock.fontSize = bigFontSize;
        gameClock.text = "-" + points.ToString();
        Countdown -= points;
        yield return new WaitForSeconds(1);
        if(Countdown > 1001) {
            gameClock.color = new Color(0, 255, 223, 0.2f);
        } else if(Countdown <= 1001 && Countdown > 501) {
            gameClock.color = new Color(255, 206, 0, 0.2f);
        } else if(Countdown <= 501) {
            gameClock.color = new Color(255, 0, 0, 0.2f);
        }
        gameClock.fontSize = smallFontSize;
        gameClock.text = Countdown.ToString();

        minusPoints = false;
    }

    public void PauseGame() {
        if(pause.isOn) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
}
